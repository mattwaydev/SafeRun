// Enemigo.cs — IA basica (TAD 4)
// POO: herencia de Personaje, polimorfismo en Atacar()
using UnityEngine;

namespace SafeRun.Entities
{
    public enum TipoAcoso { Ciberacoso, Fisico, Exclusion }

    public class Enemigo : Personaje
    {
        [SerializeField] protected TipoAcoso tipoAcoso;
        [SerializeField] protected float agresividad = 1f;
        [SerializeField] protected float rangoDeteccion = 5f;
        [SerializeField] protected Transform objetivoInicial;

        [Header("Combate")]
        [SerializeField] protected float rangoAtaque = 1.5f;
        [SerializeField] protected float danioAtaque = 15f;
        [SerializeField] protected float cooldownAtaque = 2f;

        protected Transform _objetivoIA;
        protected float _timerAtaque;
        protected bool _confundido;
        protected float _tiempoConfusion;
        protected float _danioConfusionPorSegundo;
        protected bool _jugadorDetectado;

        protected override void Start()
        {
            base.Start();
            _timerAtaque = cooldownAtaque;
            if (objetivoInicial != null)
            {
                _objetivoIA = objetivoInicial;
            }
            else
            {
                var jugador = FindAnyObjectByType<Jugador>();
                if (jugador != null) _objetivoIA = jugador.transform;
            }
        }

        protected virtual void Update()
        {
            if (_objetivoIA == null) return;

            if (_confundido)
            {
                _tiempoConfusion -= Time.deltaTime;
                if (_rb != null)
                    _rb.linearVelocity = Vector2.zero;

                RecibirDanio(_danioConfusionPorSegundo * Time.deltaTime);

                if (_tiempoConfusion <= 0f)
                {
                    _confundido = false;
                    _danioConfusionPorSegundo = 0f;
                    Debug.Log($"[SafeRun] {nombre} supera la confusion");
                }
                return;
            }

            float dist = Vector2.Distance(transform.position, _objetivoIA.position);

            if (!_jugadorDetectado)
            {
                if (dist < rangoDeteccion)
                {
                    _jugadorDetectado = true;
                    Debug.Log($"[SafeRun] {nombre} detecta al jugador");
                }
                else
                {
                    Mover(Vector2.zero);
                    _timerAtaque = cooldownAtaque;
                    return;
                }
            }

            if (dist < rangoAtaque)
            {
                Mover(Vector2.zero);
                _timerAtaque -= Time.deltaTime;
                if (_timerAtaque <= 0f)
                {
                    Atacar();
                    _timerAtaque = cooldownAtaque;
                }
            }
            else if (dist < rangoDeteccion)
            {
                PatrullarIA();
            }
            else
            {
                Mover(Vector2.zero);
            }
        }

        public virtual void PatrullarIA()
        {
            if (_objetivoIA == null) return;
            Vector2 dir = (_objetivoIA.position - transform.position).normalized;
            Mover(dir * agresividad);
        }

        public override void Atacar()
        {
            if (_objetivoIA == null) return;
            if (_animator != null)
            {
                if (HasParameter("attack"))
                    _animator.SetTrigger("attack");
                else if (HasParameter("Attack"))
                    _animator.SetTrigger("Attack");
            }
            Jugador jugador = _objetivoIA.GetComponent<Jugador>();
            if (jugador != null)
            {
                jugador.RecibirDanio(danioAtaque);
                Debug.Log($"[SafeRun] {nombre} ataca a {jugador.Nombre} con {tipoAcoso} — {danioAtaque} dano");
            }
            LanzarMensaje();
        }

        public virtual void LanzarMensaje()
        {
            Debug.Log($"[SafeRun] {nombre} lanza mensaje de tipo {tipoAcoso}");
        }

        public virtual void ActivarConfusion(float duracion, float danoPorSegundo)
        {
            _confundido = true;
            _tiempoConfusion = duracion;
            _danioConfusionPorSegundo = danoPorSegundo;
            Debug.Log($"[SafeRun] {nombre} queda confundido por {duracion}s");
        }

        public bool EstaConfundido => _confundido;

        public void SetObjetivo(Transform t) => _objetivoIA = t;
    }
}

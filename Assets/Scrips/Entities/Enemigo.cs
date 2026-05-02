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

        protected Transform _objetivoIA;             

        protected override void Start()       
        {
            base.Start();
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
            float dist = Vector2.Distance(transform.position, _objetivoIA.position);
            if (dist < rangoDeteccion) PatrullarIA();
        }

        public virtual void PatrullarIA()
        {
            if (_objetivoIA == null) return;
            Vector2 dir = (_objetivoIA.position - transform.position).normalized;
            _rb.linearVelocity = dir * velocidad * agresividad;
        }

        public override void Atacar() => LanzarMensaje();

        public virtual void LanzarMensaje()
        {
            Debug.Log($"[SafeRun] {nombre} lanza mensaje de tipo {tipoAcoso}");
        }

        public void SetObjetivo(Transform t) => _objetivoIA = t;
    }
}

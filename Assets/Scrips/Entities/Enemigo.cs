// Enemigo.cs — IA basica (TAD 4)
// POO: herencia de Personaje, polimorfismo en Atacar()
using UnityEngine;
using SafeRun.Core;

namespace SafeRun.Entities
{
    public enum TipoAcoso { Ciberacoso, Fisico, Exclusion }

    public class Enemigo : Personaje
    {
        protected bool _conteoMuerteRegistrado;
        private SpriteRenderer _spriteRenderer;
        [SerializeField] protected TipoAcoso tipoAcoso;
        [SerializeField] protected float agresividad = 1f;
        [SerializeField] protected float rangoDeteccion = 5f;
        [SerializeField] protected Transform objetivoInicial;

        [SerializeField] protected float rangoAtaque = 1.5f;
        [SerializeField] protected float danioAtaque = 15f;
        [SerializeField] protected float cooldownAtaque = 2f;

        [SerializeField] protected BalaEnemigo balaPrefab;
        [SerializeField] protected Transform puntoDisparo;
        [SerializeField] protected float rangoAtaqueDistancia = 6f;
        [SerializeField] protected float tiempoPreparacion = 0.7f;
        [SerializeField] protected float danioBala = 10f;
        [SerializeField] protected float errorPunteria = 8f;

        [Header("Confusion (Espejo de Emociones)")]
        [SerializeField] protected Color colorConfusion = new Color(0.55f, 0.85f, 1f, 1f);

        protected Transform _objetivoIA;
        protected float _timerAtaque;
        protected bool _confundido;
        protected float _tiempoConfusion;
        protected float _danioConfusionPorSegundo;
        protected bool _jugadorDetectado;
        protected Color _colorOriginal = Color.white;
        protected bool _colorOriginalGuardado;

        protected bool _preparandoDisparo;
        protected float _timerPreparacion;
        protected Vector2 _direccionDisparo;

        protected override void Start()
        {
            base.Start();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null && !_colorOriginalGuardado)
            {
                _colorOriginal = _spriteRenderer.color;
                _colorOriginalGuardado = true;
            }
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

                if (_spriteRenderer != null)
                {
                    float pulso = 0.5f + 0.5f * Mathf.Sin(Time.time * 8f);
                    _spriteRenderer.color = Color.Lerp(_colorOriginal, colorConfusion, 0.55f + 0.35f * pulso);
                }

                if (_tiempoConfusion <= 0f)
                {
                    _confundido = false;
                    _danioConfusionPorSegundo = 0f;
                    if (_spriteRenderer != null)
                        _spriteRenderer.color = _colorOriginal;
                    Debug.Log($"[SafeRun] {nombre} supera la confusion");
                }
                return;
            }

            if (_preparandoDisparo)
            {
                Mover(Vector2.zero);
                _timerPreparacion -= Time.deltaTime;
                if (_timerPreparacion <= 0f)
                {
                    DispararBala();
                    _preparandoDisparo = false;
                    _timerAtaque = cooldownAtaque;
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

            bool aDistancia = tipoAcoso == TipoAcoso.Ciberacoso && balaPrefab != null;
            float rangoEfectivo = aDistancia ? rangoAtaqueDistancia : rangoAtaque;

            if (dist < rangoEfectivo)
            {
                Mover(Vector2.zero);
                _timerAtaque -= Time.deltaTime;
                if (_timerAtaque <= 0f)
                {
                    if (aDistancia)
                    {
                        IniciarPreparacionDisparo();
                    }
                    else
                    {
                        Atacar();
                        _timerAtaque = cooldownAtaque;
                    }
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

        public override void Mover(Vector2 direccion)
        {
            base.Mover(direccion);

            if (_spriteRenderer != null && direccion.x != 0f)
            {
                _spriteRenderer.flipX = direccion.x < 0f;
            }
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Jugador jugador = collision.gameObject.GetComponent<Jugador>();
            if (jugador == null)
                jugador = collision.gameObject.GetComponentInParent<Jugador>();

            if (jugador == null) return;

            _objetivoIA = jugador.transform;
            _jugadorDetectado = true;
            _timerAtaque = 0f;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            Jugador jugador = collision.gameObject.GetComponent<Jugador>();
            if (jugador == null)
                jugador = collision.gameObject.GetComponentInParent<Jugador>();

            if (jugador == null) return;

            if (_objetivoIA == null)
                _objetivoIA = jugador.transform;

            _jugadorDetectado = true;

            bool aDistancia = tipoAcoso == TipoAcoso.Ciberacoso && balaPrefab != null;
            float rangoEfectivo = aDistancia ? rangoAtaqueDistancia : rangoAtaque;
            float dist = Vector2.Distance(transform.position, jugador.transform.position);

            if (dist <= rangoEfectivo)
                return;

            _timerAtaque -= Time.deltaTime;
            if (_timerAtaque <= 0f)
            {
                if (aDistancia)
                    IniciarPreparacionDisparo();
                else
                    Atacar();

                _timerAtaque = cooldownAtaque;
            }
        }


        public virtual void LanzarMensaje()
        {
            Debug.Log($"[SafeRun] {nombre} lanza mensaje de tipo {tipoAcoso}");
        }

        protected virtual void IniciarPreparacionDisparo()
        {
            if (_objetivoIA == null) return;

            Vector2 origen = puntoDisparo != null ? (Vector2)puntoDisparo.position : (Vector2)transform.position;
            Vector2 posObjetivo = (Vector2)_objetivoIA.position;

            Vector2 velObjetivo = Vector2.zero;
            var rbObj = _objetivoIA.GetComponent<Rigidbody2D>();
            if (rbObj != null) velObjetivo = rbObj.linearVelocity;

            float velBala = balaPrefab != null ? balaPrefab.Velocidad : 8f;

            _direccionDisparo = CalcularInterseccion(origen, posObjetivo, velObjetivo, velBala);

            if (_direccionDisparo.sqrMagnitude < 0.0001f)
                _direccionDisparo = _ultimaDireccion;

            if (errorPunteria > 0f)
            {
                float jitter = Random.Range(-errorPunteria, errorPunteria);
                _direccionDisparo = (Vector2)(Quaternion.Euler(0f, 0f, jitter) * _direccionDisparo);
            }

            _preparandoDisparo = true;
            _timerPreparacion = tiempoPreparacion;

            if (_spriteRenderer != null && Mathf.Abs(_direccionDisparo.x) > 0.01f)
                _spriteRenderer.flipX = _direccionDisparo.x < 0f;

            if (_animator != null)
            {
                if (HasParameter("attack"))
                    _animator.SetTrigger("attack");
                else if (HasParameter("Attack"))
                    _animator.SetTrigger("Attack");
            }

            Debug.Log($"[SafeRun] {nombre} prepara ataque a distancia ({tiempoPreparacion}s)");
        }

        protected static Vector2 CalcularInterseccion(Vector2 origen, Vector2 posObj, Vector2 velObj, float balaSpeed)
        {
            Vector2 delta = posObj - origen;
            float a = Vector2.Dot(velObj, velObj) - balaSpeed * balaSpeed;
            float b = 2f * Vector2.Dot(delta, velObj);
            float c = Vector2.Dot(delta, delta);

            float t;
            if (Mathf.Abs(a) < 0.0001f)
            {
                if (Mathf.Abs(b) < 0.0001f) return delta.normalized;
                t = -c / b;
            }
            else
            {
                float disc = b * b - 4f * a * c;
                if (disc < 0f) return delta.normalized; 
                float sqrt = Mathf.Sqrt(disc);
                float t1 = (-b - sqrt) / (2f * a);
                float t2 = (-b + sqrt) / (2f * a);
                t = Mathf.Min(t1, t2);
                if (t < 0f) t = Mathf.Max(t1, t2);
            }

            if (t <= 0f) return delta.normalized;

            Vector2 puntoImpacto = posObj + velObj * t;
            Vector2 dir = puntoImpacto - origen;
            return dir.sqrMagnitude > 0.0001f ? dir.normalized : delta.normalized;
        }

        protected virtual void DispararBala()
        {
            if (balaPrefab == null) return;

            Vector2 origen = puntoDisparo != null ? (Vector2)puntoDisparo.position : (Vector2)transform.position;
            float dano = danioBala > 0f ? danioBala : danioAtaque;

            BalaEnemigo bala = Instantiate(balaPrefab, origen, Quaternion.identity);
            bala.Configurar(_direccionDisparo, dano, gameObject);

            LanzarMensaje();
        }

        public virtual void ActivarConfusion(float duracion, float danoPorSegundo)
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null && !_colorOriginalGuardado)
            {
                _colorOriginal = _spriteRenderer.color;
                _colorOriginalGuardado = true;
            }

            _confundido = true;
            _tiempoConfusion = duracion;
            _danioConfusionPorSegundo = danoPorSegundo;
            _preparandoDisparo = false;
            _timerPreparacion = 0f;
            Debug.Log($"[SafeRun] {nombre} queda confundido por {duracion}s");
        }

        public bool EstaConfundido => _confundido;

        public void SetObjetivo(Transform t) => _objetivoIA = t;

        protected override void Morir()
        {
            RegistrarMuerteEnRun();
            base.Morir();
        }

        protected virtual void RegistrarMuerteEnRun()
        {
            if (_conteoMuerteRegistrado) return;
            _conteoMuerteRegistrado = true;
            var stats = RunStatsManager.Instancia;
            if (stats != null)
                stats.RegistrarEnemigoDerrotado();
        }
    }
}

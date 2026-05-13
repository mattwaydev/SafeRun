// BalaEnemigo.cs — Proyectil disparado por EnemigoRango
// POO: composicion (se anexa al prefab Bala_enemigo)
using UnityEngine;

namespace SafeRun.Entities
{
    [RequireComponent(typeof(Collider2D))]
    public class BalaEnemigo : MonoBehaviour
    {
        [SerializeField] private float danio = 10f;
        [SerializeField] private float velocidad = 8f;
        [SerializeField] private float tiempoVida = 4f;

        public float Velocidad => velocidad;

        [SerializeField] private bool rotarHaciaDireccion = true;
        [SerializeField] private float offsetAngulo = 0f;

        private Vector2 _direccion = Vector2.right;
        private GameObject _propietario;       
        private bool _configurado;

        private void Awake()
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = 120;
        }

        public void Configurar(Vector2 direccion, float danioPersonalizado = -1f, GameObject propietario = null)
        {
            _direccion = direccion.sqrMagnitude > 0.0001f ? direccion.normalized : Vector2.right;

            if (danioPersonalizado > 0f)
                danio = danioPersonalizado;

            _propietario = propietario;

            if (_propietario != null)
            {
                var miCol = GetComponent<Collider2D>();
                foreach (var colOwner in _propietario.GetComponentsInChildren<Collider2D>())
                {
                    if (miCol != null && colOwner != null)
                        Physics2D.IgnoreCollision(miCol, colOwner, true);
                }
            }

            if (rotarHaciaDireccion)
            {
                float ang = Mathf.Atan2(_direccion.y, _direccion.x) * Mathf.Rad2Deg + offsetAngulo;
                transform.rotation = Quaternion.Euler(0f, 0f, ang);
            }

            _configurado = true;
            Destroy(gameObject, tiempoVida);
        }

        private void Update()
        {
            if (!_configurado) return;
            transform.Translate(_direccion * velocidad * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Jugador jugador = other.GetComponentInParent<Jugador>();
            if (jugador != null)
            {
                jugador.RecibirDanio(danio);
                Destroy(gameObject);
                return;
            }

            if (other.GetComponentInParent<Enemigo>() != null)
                return;

            if (!other.isTrigger)
                Destroy(gameObject);
        }
    }
}

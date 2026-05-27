using UnityEngine;

namespace SafeRun.Entities
{
    public class ProyectilPapel : MonoBehaviour
    {
        [SerializeField] private float danio = 25f;
        [SerializeField] private float velocidad = 10f;
        [SerializeField] private float tiempoVida = 3f;
        [SerializeField] private bool rotarHaciaDireccion = true;
        [SerializeField] private float offsetAngulo = 0f;
        [SerializeField] private float shakeDuracion = 0.12f;
        [SerializeField] private float shakeMagnitud = 0.18f;

        private Vector2 _direccion;

        private void Awake()
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = 120;
        }

        public void Configurar(Vector2 direccion, float danioPersonalizado)
        {
            _direccion = direccion.normalized;
            if (_direccion == Vector2.zero)
                _direccion = Vector2.up;
            danio = danioPersonalizado > 0f ? danioPersonalizado : danio;

            if (rotarHaciaDireccion)
            {
                float ang = Mathf.Atan2(_direccion.y, _direccion.x) * Mathf.Rad2Deg + offsetAngulo;
                transform.rotation = Quaternion.Euler(0f, 0f, ang);
            }

            Destroy(gameObject, tiempoVida);
        }

        private void Update()
        {
            transform.Translate(_direccion * velocidad * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Enemigo enemigo = other.GetComponentInParent<Enemigo>();
            if (enemigo != null)
            {
                enemigo.RecibirDanio(danio);
                SafeRun.Core.CamaraSeguidora.SacudirCamara(shakeDuracion, shakeMagnitud);
                Destroy(gameObject);
            }
        }
    }
}

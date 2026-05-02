using UnityEngine;

namespace SafeRun.Entities
{
    public class ProyectilPapel : MonoBehaviour
    {
        [SerializeField] private float danio = 25f;
        [SerializeField] private float velocidad = 10f;
        [SerializeField] private float tiempoVida = 3f;

        private Vector2 _direccion;

        public void Configurar(Vector2 direccion, float danioPersonalizado)
        {
            _direccion = direccion.normalized;
            if (_direccion == Vector2.zero)
                _direccion = Vector2.up;
            danio = danioPersonalizado > 0f ? danioPersonalizado : danio;
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
            Destroy(gameObject);
        }
        }
    }
}

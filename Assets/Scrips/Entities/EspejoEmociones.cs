using UnityEngine;

namespace SafeRun.Entities
{
    public class EspejoEmociones : MonoBehaviour
    {
        [SerializeField] private float radio = 5f;
        [SerializeField] private float duracion = 5f;
        [SerializeField] private float danioPorSegundo = 15f;

        private readonly Collider2D[] _resultados = new Collider2D[32];

        private void Awake()
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = 120;
        }

        public void Configurar(float radioPersonalizado, float danioPersonalizado)
        {
            if (radioPersonalizado > 0f)
                radio = radioPersonalizado;

            if (danioPersonalizado > 0f)
                danioPorSegundo = danioPersonalizado;
        }

        private void Start()
        {
            Collider2D[] resultados = Physics2D.OverlapCircleAll(transform.position, radio);
            int cantidad = resultados.Length;
            for (int i = 0; i < cantidad; i++)
            {
                Collider2D col = resultados[i];
                if (col == null) continue;

                Enemigo enemigo = col.GetComponentInParent<Enemigo>();
                if (enemigo != null && enemigo.EstaVivo)
                {
                    enemigo.ActivarConfusion(duracion, danioPorSegundo);
                }
            }

            Destroy(gameObject, duracion + 0.5f);
        }
    }
}

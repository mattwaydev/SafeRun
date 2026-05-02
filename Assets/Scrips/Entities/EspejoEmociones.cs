using UnityEngine;

namespace SafeRun.Entities
{
    public class EspejoEmociones : MonoBehaviour
    {
        [SerializeField] private float radio = 5f;
        [SerializeField] private float duracion = 5f;
        [SerializeField] private float danioPorSegundo = 15f;

        private void Start()
        {
            Collider2D[] colisiones = Physics2D.OverlapCircleAll(transform.position, radio);
            foreach (var col in colisiones)
            {
                Enemigo enemigo = col.GetComponent<Enemigo>();
                if (enemigo != null && enemigo.EstaVivo)
                {
                    enemigo.ActivarConfusion(duracion, danioPorSegundo);
                }
            }

            Destroy(gameObject, duracion + 0.5f);
        }
    }
}

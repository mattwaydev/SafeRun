// CamaraSeguidora.cs — la camara sigue al jugador
using UnityEngine;

namespace SafeRun.Core
{
    public class CamaraSeguidora : MonoBehaviour
    {
        [SerializeField] private Transform objetivo;
        [SerializeField] private float suavizado = 5f;

        private void LateUpdate()
        {
            if (objetivo == null) return;
            Vector3 posObjetivo = new Vector3(objetivo.position.x, objetivo.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, posObjetivo, suavizado * Time.deltaTime);
        }
    }
}
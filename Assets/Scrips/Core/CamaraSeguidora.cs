// CamaraSeguidora.cs — la camara sigue al jugador
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SafeRun.Core
{
    public class CamaraSeguidora : MonoBehaviour
    {
        private static CamaraSeguidora _instancia;
        [SerializeField] private Transform objetivo;
        [SerializeField] private float suavizado = 5f;

        private void Awake()
        {
            if (_instancia != null && _instancia != this)
            {
                Destroy(gameObject);
                return;
            }

            _instancia = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            ReasignarObjetivo();
        }

        private void OnDestroy()
        {
            if (_instancia == this)
                _instancia = null;

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => ReasignarObjetivo();

        private void ReasignarObjetivo()
        {
            var jugador = FindAnyObjectByType<SafeRun.Entities.Jugador>();
            if (jugador != null)
                objetivo = jugador.transform;
        }

        public void Seguir(Transform nuevoObjetivo)
        {
            objetivo = nuevoObjetivo;
        }

        private void LateUpdate()
        {
            if (objetivo == null)
                ReasignarObjetivo();

            if (objetivo == null) return;
            Vector3 posObjetivo = new Vector3(objetivo.position.x, objetivo.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, posObjetivo, suavizado * Time.deltaTime);
        }
    }
}

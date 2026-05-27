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
        [SerializeField] private float decaimientoShake = 1.4f;

        private float _shakeTiempoRestante;
        private float _shakeDuracionTotal;
        private float _shakeMagnitud;
        private Vector3 _shakeOffset;

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

        public static void SacudirCamara(float duracion, float magnitud)
        {
            if (_instancia != null)
                _instancia.Shake(duracion, magnitud);
        }

        public void Shake(float duracion, float magnitud)
        {
            if (duracion <= 0f || magnitud <= 0f) return;
            if (duracion > _shakeTiempoRestante)
            {
                _shakeTiempoRestante = duracion;
                _shakeDuracionTotal = duracion;
            }
            if (magnitud > _shakeMagnitud)
                _shakeMagnitud = magnitud;
        }

        private void LateUpdate()
        {
            if (objetivo == null)
                ReasignarObjetivo();

            if (objetivo == null) return;
            Vector3 posObjetivo = new Vector3(objetivo.position.x, objetivo.position.y, transform.position.z);
            Vector3 baseActual = transform.position - _shakeOffset;
            Vector3 baseNueva = Vector3.Lerp(baseActual, posObjetivo, suavizado * Time.deltaTime);

            Vector3 nuevoOffset = Vector3.zero;
            if (_shakeTiempoRestante > 0f)
            {
                _shakeTiempoRestante -= Time.deltaTime;
                float t = Mathf.Clamp01(_shakeTiempoRestante / Mathf.Max(_shakeDuracionTotal, 0.0001f));
                float fuerza = _shakeMagnitud * Mathf.Pow(t, decaimientoShake);
                nuevoOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f) * fuerza;
                if (_shakeTiempoRestante <= 0f)
                {
                    _shakeTiempoRestante = 0f;
                    _shakeMagnitud = 0f;
                }
            }

            _shakeOffset = nuevoOffset;
            transform.position = baseNueva + nuevoOffset;
        }
    }
}

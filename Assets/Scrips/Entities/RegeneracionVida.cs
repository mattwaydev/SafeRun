using UnityEngine;

namespace SafeRun.Entities
{
    [RequireComponent(typeof(Entidad))]
    public class RegeneracionVida : MonoBehaviour
    {
        [Header("Configuracion")]
        [Tooltip("Vida regenerada por segundo cuando esta activa.")]
        [SerializeField] private float tasaPorSegundo = 2f;

        [Tooltip("Segundos sin recibir danio antes de empezar a regenerar.")]
        [SerializeField] private float delayTrasDanio = 5f;

        [Tooltip("Si esta activado, regenera. Util para desactivarlo en zonas de combate scriptadas.")]
        [SerializeField] private bool activo = true;

        private Entidad _entidad;
        private float _timerSinDanio;
        private float _vidaPrevia;

        private void Awake()
        {
            _entidad = GetComponent<Entidad>();
        }

        private void OnEnable()
        {
            _entidad.VidaCambiada += OnVidaCambiada;
            _vidaPrevia = _entidad.VidaActual;
            _timerSinDanio = delayTrasDanio;
        }

        private void OnDisable()
        {
            _entidad.VidaCambiada -= OnVidaCambiada;
        }

        private void Update()
        {
            if (!activo || !_entidad.EstaVivo) return;

            _timerSinDanio += Time.deltaTime;

            if (_timerSinDanio >= delayTrasDanio &&
                _entidad.VidaActual < _entidad.VidaMaxima)
            {
                _entidad.Sanar(tasaPorSegundo * Time.deltaTime);
            }
        }

        private void OnVidaCambiada(float vidaActual, float vidaMaxima)
        {
            if (vidaActual < _vidaPrevia)
                _timerSinDanio = 0f;
            _vidaPrevia = vidaActual;
        }

        public void SetActivo(bool valor) => activo = valor;
    }
}

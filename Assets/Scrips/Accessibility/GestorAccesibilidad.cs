// GestorAccesibilidad.cs — Componente inclusivo
// Cumple requisito funcional b) de ambas materias
using UnityEngine;
using SafeRun.Core;
using SafeRun.Audio;

namespace SafeRun.Accessibility
{
    public class GestorAccesibilidad : MonoBehaviour
    {
        public enum DaltonismoModo { Normal = 0, Protanopia = 1, Deuteranopia = 2, Tritanopia = 3 }

        [Header("Opciones de accesibilidad")]
        [SerializeField] private bool modoDaltonico     = false;
        [SerializeField] private bool subtitulosActivos = true;
        [SerializeField] private bool narracionVoz      = false;
        [SerializeField] private string skinPersonaje   = "default";

        [Header("Referencias")]
        [SerializeField] private Camera camaraPrincipal;
        [SerializeField] private Material materialDaltonismo;
        [SerializeField] private DaltonismoModo modoDaltonismo = DaltonismoModo.Normal;
        [SerializeField] private SubtitleManager subtitleManager;
        [SerializeField] private NarrationManager narrationManager;

        private GestorJuego _gestorJuego;

        private void Awake()
        {
            if (camaraPrincipal == null)
                camaraPrincipal = Camera.main;

            if (materialDaltonismo != null)
                materialDaltonismo.SetInt("_Mode", (int)modoDaltonismo);
        }

        private void Start()
        {
            _gestorJuego = FindAnyObjectByType<GestorJuego>();
            if (_gestorJuego != null)
            {
                _gestorJuego.SubtituloSolicitado += MostrarSubtitulo;
                _gestorJuego.NarracionSolicitada += ReproducirNarracion;
            }
        }

        private void OnDestroy()
        {
            if (_gestorJuego != null)
            {
                _gestorJuego.SubtituloSolicitado -= MostrarSubtitulo;
                _gestorJuego.NarracionSolicitada -= ReproducirNarracion;
            }
        }

        public void ActivarModoDaltonico(bool activo)
        {
            modoDaltonico = activo;
            Debug.Log($"[Accesibilidad] Modo daltonico: {activo}");
             AplicarDaltonismo(activo);
        }

        public void CambiarModoDaltonismo(DaltonismoModo modo)
        {
            modoDaltonismo = modo;
            if (materialDaltonismo != null)
                materialDaltonismo.SetInt("_Mode", (int)modoDaltonismo);

            if (modoDaltonismo == DaltonismoModo.Normal)
                ActivarModoDaltonico(false);
            else
                ActivarModoDaltonico(true);
        }

        public void ActivarSubtitulos(bool activo)
        {
            subtitulosActivos = activo;
            Debug.Log($"[Accesibilidad] Subtitulos: {activo}");
        }

        public void ActivarNarracion(bool activo)
        {
            narracionVoz = activo;
            Debug.Log($"[Accesibilidad] Narracion de voz: {activo}");
        }

        public void CambiarSkin(string skin)
        {
            skinPersonaje = skin;
            Debug.Log($"[Accesibilidad] Skin cambiada a: {skin}");
        }

        public bool ModoDaltonico    => modoDaltonico;
        public bool Subtitulos       => subtitulosActivos;
        public string SkinPersonaje  => skinPersonaje;
        public DaltonismoModo ModoActual => modoDaltonismo;

        private void AplicarDaltonismo(bool activo)
        {
            if (camaraPrincipal == null || materialDaltonismo == null) return;
            if (activo)
            {
                var efecto = camaraPrincipal.GetComponent<ColorBlindnessEffect>();
                if (efecto == null)
                    efecto = camaraPrincipal.gameObject.AddComponent<ColorBlindnessEffect>();

                efecto.Configurar(materialDaltonismo);
            }
            else
            {
                var efecto = camaraPrincipal.GetComponent<ColorBlindnessEffect>();
                if (efecto != null) Destroy(efecto);
            }
        }

        private void MostrarSubtitulo(string texto)
        {
            if (!subtitulosActivos || subtitleManager == null) return;
            subtitleManager.Mostrar(texto, 3f);
        }

        private void ReproducirNarracion(string clave)
        {
            if (!narracionVoz || narrationManager == null) return;
            narrationManager.Reproducir(clave);
        }
    }
}

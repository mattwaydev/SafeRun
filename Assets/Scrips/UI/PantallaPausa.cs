using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SafeRun.UI
{
    public class PantallaPausa : MonoBehaviour
    {
        [Header("Configuracion")]
        [SerializeField] private string escenaMenuPrincipal = "Menu";
        [SerializeField] private bool pausarAudio = true;
        [SerializeField] private bool bloquearSiHayOtraPantallaAbierta = true;

        [Header("Paneles")]
        [SerializeField] private GameObject panelPausa;
        [SerializeField] private GameObject panelOpciones;

        [Header("Botones del menu de pausa")]
        [SerializeField] private Button botonReanudar;
        [SerializeField] private Button botonOpciones;
        [SerializeField] private Button botonVolverMenu;
        [SerializeField] private Button botonSalir;

        [Header("Boton para cerrar opciones (opcional)")]
        [SerializeField] private Button botonCerrarOpciones;

        public bool EstaPausado { get; private set; }

        private void Awake()
        {
            if (panelPausa != null)
                panelPausa.SetActive(false);

            if (panelOpciones != null)
                panelOpciones.SetActive(false);

            EnlazarBoton(botonReanudar, Reanudar);
            EnlazarBoton(botonOpciones, AbrirOpciones);
            EnlazarBoton(botonVolverMenu, VolverAlMenu);
            EnlazarBoton(botonSalir, SalirJuego);
            EnlazarBoton(botonCerrarOpciones, CerrarOpciones);
        }

        private void OnDestroy()
        {
            if (EstaPausado)
            {
                Time.timeScale = 1f;
                AudioListener.pause = false;
            }
        }

        private void Update()
        {
            var teclado = Keyboard.current;
            if (teclado == null) return;

            if (!teclado.escapeKey.wasPressedThisFrame) return;

            if (panelOpciones != null && panelOpciones.activeSelf)
            {
                CerrarOpciones();
                return;
            }

            Alternar();
        }

        public void Alternar()
        {
            if (EstaPausado) Reanudar();
            else Pausar();
        }

        public void Pausar()
        {
            if (EstaPausado) return;

            if (bloquearSiHayOtraPantallaAbierta && HayOtraPantallaModalAbierta())
                return;

            Time.timeScale = 0f;
            if (pausarAudio) AudioListener.pause = true;

            if (panelPausa != null)
                panelPausa.SetActive(true);

            EstaPausado = true;
        }

        public void Reanudar()
        {
            if (panelOpciones != null && panelOpciones.activeSelf)
                panelOpciones.SetActive(false);

            if (panelPausa != null)
                panelPausa.SetActive(false);

            Time.timeScale = 1f;
            if (pausarAudio) AudioListener.pause = false;
            EstaPausado = false;
        }

        public void AbrirOpciones()
        {
            if (panelOpciones == null) return;

            panelOpciones.SetActive(true);

            if (panelPausa != null)
                panelPausa.SetActive(false);
        }

        public void CerrarOpciones()
        {
            if (panelOpciones != null)
                panelOpciones.SetActive(false);

            if (panelPausa != null)
                panelPausa.SetActive(true);
        }

        public void VolverAlMenu()
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            EstaPausado = false;

            if (string.IsNullOrWhiteSpace(escenaMenuPrincipal))
            {
                Debug.LogWarning("[PantallaPausa] No se definio la escena del menu principal.");
                return;
            }

            SceneManager.LoadScene(escenaMenuPrincipal);
        }

        public void SalirJuego()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private bool HayOtraPantallaModalAbierta()
        {
            return panelOpciones != null && panelOpciones.activeSelf;
        }

        private static void EnlazarBoton(Button boton, UnityEngine.Events.UnityAction accion)
        {
            if (boton == null || accion == null) return;
            boton.onClick.RemoveListener(accion);
            boton.onClick.AddListener(accion);
        }
    }
}

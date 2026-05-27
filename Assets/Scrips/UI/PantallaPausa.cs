using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
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

        private static PantallaPausa _instancia;
        public static PantallaPausa Instancia => _instancia;

        public bool EstaPausado { get; private set; }

        private void Awake()
        {
            var raiz = transform.root.gameObject;

            if (_instancia != null && _instancia != this)
            {
                Destroy(raiz);
                return;
            }

            _instancia = this;
            DontDestroyOnLoad(raiz);

            GarantizarEventSystem(raiz.transform);

            if (panelPausa != null)
                panelPausa.SetActive(false);

            if (panelOpciones != null)
                panelOpciones.SetActive(false);

            EnlazarBoton(botonReanudar, Reanudar);
            EnlazarBoton(botonOpciones, AbrirOpciones);
            EnlazarBoton(botonVolverMenu, VolverAlMenu);
            EnlazarBoton(botonSalir, SalirJuego);
            EnlazarBoton(botonCerrarOpciones, CerrarOpciones);

            SceneManager.sceneLoaded += AlCargarEscena;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= AlCargarEscena;

            if (_instancia == this)
                _instancia = null;

            if (EstaPausado)
            {
                Time.timeScale = 1f;
                AudioListener.pause = false;
            }
        }

        private void AlCargarEscena(Scene escena, LoadSceneMode modo)
        {
            var raiz = transform.root;
            var todos = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
            foreach (var es in todos)
            {
                if (es == null) continue;
                if (es.transform.root != raiz)
                    Destroy(es.gameObject);
            }
        }

        private static void GarantizarEventSystem(Transform padre)
        {
            var existente = FindFirstObjectByType<EventSystem>();

            if (existente != null)
            {
                if (existente.transform.root != padre)
                    existente.transform.SetParent(padre, false);

                if (existente.GetComponent<InputSystemUIInputModule>() == null
                    && existente.GetComponent<BaseInputModule>() == null)
                {
                    existente.gameObject.AddComponent<InputSystemUIInputModule>();
                }
                return;
            }

            var go = new GameObject("EventSystem");
            go.transform.SetParent(padre, false);
            go.AddComponent<EventSystem>();
            go.AddComponent<InputSystemUIInputModule>();
        }

        private void Update()
        {
            if (PopupHabilidad.EstaActivo) return;

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

            if (panelOpciones != null)
                panelOpciones.SetActive(false);

            if (panelPausa != null)
                panelPausa.SetActive(false);

            if (string.IsNullOrWhiteSpace(escenaMenuPrincipal))
            {
                Debug.LogWarning("[PantallaPausa] No se definio la escena del menu principal.");
                return;
            }

            DestruirSingletonesDePartida();

            var cargador = new GameObject("CargadorMenu");
            DontDestroyOnLoad(cargador);
            cargador.AddComponent<CargadorMenu>().Iniciar(escenaMenuPrincipal);

            _instancia = null;
            Destroy(transform.root.gameObject);
        }

        private static void DestruirSingletonesDePartida()
        {
            var jugador = FindAnyObjectByType<SafeRun.Entities.Jugador>();
            if (jugador != null)
                Destroy(jugador.gameObject);

            var gestorJuego = FindAnyObjectByType<SafeRun.Core.GestorJuego>();
            if (gestorJuego != null)
                Destroy(gestorJuego.gameObject);

            var gestorEscenas = SafeRun.Core.GestorEscenas.Instancia;
            if (gestorEscenas != null)
                Destroy(gestorEscenas.gameObject);
        }

        internal static void DestruirPersistentesDePartida()
        {
            var camara = FindAnyObjectByType<SafeRun.Core.CamaraSeguidora>();
            if (camara != null)
                Destroy(camara.gameObject);

            var barraVida = FindAnyObjectByType<BarraDeVida>();
            if (barraVida != null)
                Destroy(barraVida.gameObject);

            var spawner = FindAnyObjectByType<SafeRun.Core.SpawnerEnemigos>();
            if (spawner != null)
                Destroy(spawner.gameObject);
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

    public class CargadorMenu : MonoBehaviour
    {
        public void Iniciar(string escena)
        {
            StartCoroutine(EjecutarCarga(escena));
        }

        private System.Collections.IEnumerator EjecutarCarga(string escena)
        {
            yield return null;

            var op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(escena);
            if (op == null)
                yield break;

            while (!op.isDone)
                yield return null;

            PantallaPausa.DestruirPersistentesDePartida();
            Destroy(gameObject);
        }
    }
}

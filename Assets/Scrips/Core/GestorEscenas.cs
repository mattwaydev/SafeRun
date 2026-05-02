using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

namespace SafeRun.Core
{
    public class GestorEscenas : MonoBehaviour
    {
        private static GestorEscenas _instancia;

        [SerializeField] private float duracionFade = 0.2f;
        [SerializeField] private CanvasGroup overlayFade;

        private readonly Stack<string> _historial = new();
        private bool _cargando;

        public static GestorEscenas Instancia => _instancia;

        private void Awake()
        {
            if (_instancia != null && _instancia != this)
            {
                Destroy(gameObject);
                return;
            }

            _instancia = this;
            DontDestroyOnLoad(gameObject);

            if (overlayFade == null)
                CrearOverlayFade();

            if (overlayFade != null)
            {
                overlayFade.alpha = 0f;
                overlayFade.blocksRaycasts = false;
            }

            AjustarFisicaEscena();
        }

        private void Start()
        {
            AjustarFisicaEscena();
        }

        private void CrearOverlayFade()
        {
            var canvasObj = new GameObject("FadeOverlay");
            canvasObj.transform.SetParent(transform, false);

            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32767;

            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            var panel = new GameObject("FadePanel");
            panel.transform.SetParent(canvasObj.transform, false);

            var image = panel.AddComponent<Image>();
            image.color = Color.black;

            var rect = image.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            overlayFade = panel.AddComponent<CanvasGroup>();
        }

        public void IrASala(string escena)
        {
            if (_cargando || string.IsNullOrWhiteSpace(escena))
                return;

            StartCoroutine(CargarConFade(escena, guardarActual: true));
        }

        public void VolverASalaAnterior()
        {
            if (_cargando || _historial.Count == 0)
                return;

            StartCoroutine(CargarConFade(_historial.Pop(), guardarActual: false));
        }

        private IEnumerator CargarConFade(string escena, bool guardarActual)
        {
            _cargando = true;

            if (guardarActual)
                _historial.Push(SceneManager.GetActiveScene().name);

            yield return Fade(0f, 1f);
            yield return SceneManager.LoadSceneAsync(escena);
            yield return null;
            AjustarFisicaEscena();
            Physics2D.SyncTransforms();
            yield return Fade(1f, 0f);

            _cargando = false;
        }

        private void AjustarFisicaEscena()
        {
            DesactivarAudioListenersDuplicados();

            var tilemaps = FindObjectsByType<Tilemap>(FindObjectsInactive.Exclude);
            foreach (var tilemap in tilemaps)
            {
                string nombre = tilemap.gameObject.name.ToLowerInvariant();
                bool esPiso = nombre.Contains("piso") || nombre.Contains("suelo") || nombre.Contains("floor") || nombre.Contains("ground") || nombre.Contains("stairs") || nombre.Contains("stair");

                if (esPiso)
                    continue;

                var collider = tilemap.GetComponent<TilemapCollider2D>();
                if (collider == null)
                    collider = tilemap.gameObject.AddComponent<TilemapCollider2D>();

                collider.usedByComposite = true;
                collider.extrusionFactor = 0.02f;

                var composite = tilemap.GetComponent<CompositeCollider2D>();
                if (composite == null)
                    composite = tilemap.gameObject.AddComponent<CompositeCollider2D>();

                var rb = tilemap.GetComponent<Rigidbody2D>();
                if (rb == null)
                    rb = tilemap.gameObject.AddComponent<Rigidbody2D>();

                rb.bodyType = RigidbodyType2D.Static;
                rb.simulated = true;
                collider.compositeOperation = Collider2D.CompositeOperation.Merge;
                composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
            }
        }

        private void DesactivarAudioListenersDuplicados()
        {
            var listeners = FindObjectsByType<AudioListener>(FindObjectsInactive.Exclude);
            bool encontrado = false;

            foreach (var listener in listeners)
            {
                if (!encontrado)
                {
                    encontrado = true;
                    continue;
                }

                listener.enabled = false;
            }
        }

        private IEnumerator Fade(float desde, float hasta)
        {
            if (overlayFade == null)
                yield break;

            overlayFade.blocksRaycasts = true;
            float tiempo = 0f;

            while (tiempo < duracionFade)
            {
                tiempo += Time.unscaledDeltaTime;
                overlayFade.alpha = Mathf.Lerp(desde, hasta, tiempo / duracionFade);
                yield return null;
            }

            overlayFade.alpha = hasta;
            overlayFade.blocksRaycasts = hasta > 0f;
        }
    }
}

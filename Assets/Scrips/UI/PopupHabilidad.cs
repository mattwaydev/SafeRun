using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SafeRun.UI
{
    public class PopupHabilidad : MonoBehaviour
    {
        private static PopupHabilidad _instancia;

        [SerializeField] private float duracionFadeIn = 0.25f;
        [SerializeField] private float duracionPermanencia = 2.2f;
        [SerializeField] private float duracionFadeOut = 0.35f;
        [SerializeField] private float escalaInicial = 0.5f;
        [SerializeField] private int sortingOrder = 5000;

        [Header("Layout")]
        [SerializeField] private Vector2 tamanoPopup = new Vector2(360f, 440f);
        [SerializeField] private Vector2 tamanoIcono = new Vector2(200f, 156f);
        [SerializeField] private Vector2 posicionIcono = new Vector2(0f, 50f);
        [SerializeField] private Vector2 tamanoDescripcion = new Vector2(300f, 120f);
        [SerializeField] private Vector2 posicionDescripcion = new Vector2(0f, -110f);
        [SerializeField] private float tamanoFuenteDescripcion = 18f;
        [SerializeField] private Color colorTextoDescripcion = Color.white;
        [SerializeField] private Vector2 resolucionReferencia = new Vector2(800f, 600f);

        private PopupHabilidadConfig _config;
        private Coroutine _enCurso;
        private GameObject _overlayActivo;

        public static void Mostrar(string nombreItem)
        {
            Debug.Log($"[SafeRun] PopupHabilidad.Mostrar('{nombreItem}')");
            GarantizarInstancia();
            _instancia.LanzarPopup(nombreItem);
        }

        private static void GarantizarInstancia()
        {
            if (_instancia != null) return;
            var go = new GameObject("PopupHabilidad");
            DontDestroyOnLoad(go);
            _instancia = go.AddComponent<PopupHabilidad>();
        }

        private PopupHabilidadConfig ConfigCargado()
        {
            if (_config != null) return _config;
            _config = Resources.Load<PopupHabilidadConfig>("PopupHabilidadConfig");
            if (_config == null)
                Debug.LogWarning("[SafeRun] PopupHabilidad: no se encontro 'Resources/PopupHabilidadConfig.asset'.");
            return _config;
        }

        private void LanzarPopup(string nombreItem)
        {
            var cfg = ConfigCargado();
            if (cfg == null)
            {
                Debug.LogError("[SafeRun] PopupHabilidad: config nulo.");
                return;
            }

            var entrada = cfg.Buscar(nombreItem);
            if (entrada == null)
            {
                Debug.LogWarning($"[SafeRun] PopupHabilidad: no hay entrada para '{nombreItem}'. Entradas: {(cfg.entradas == null ? 0 : cfg.entradas.Length)}");
                return;
            }

            if (_enCurso != null)
            {
                StopCoroutine(_enCurso);
                if (_overlayActivo != null) Destroy(_overlayActivo);
                _overlayActivo = null;
            }
            _enCurso = StartCoroutine(Ejecutar(entrada));
        }

        private IEnumerator Ejecutar(PopupHabilidadConfig.Entrada entrada)
        {
            var contenedor = new GameObject("PopupHabilidadOverlay");
            _overlayActivo = contenedor;
            DontDestroyOnLoad(contenedor);

            var canvas = contenedor.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            var scaler = contenedor.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = resolucionReferencia;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            contenedor.AddComponent<GraphicRaycaster>();
            var grupo = contenedor.AddComponent<CanvasGroup>();
            grupo.alpha = 0f;
            grupo.interactable = false;
            grupo.blocksRaycasts = false;

            var popGO = new GameObject("Pop");
            var popRect = popGO.AddComponent<RectTransform>();
            popRect.SetParent(contenedor.transform, false);
            popRect.anchorMin = new Vector2(0.5f, 0.5f);
            popRect.anchorMax = new Vector2(0.5f, 0.5f);
            popRect.pivot = new Vector2(0.5f, 0.5f);
            popRect.sizeDelta = tamanoPopup;
            popRect.anchoredPosition = Vector2.zero;

            if (entrada.panel != null)
            {
                var panelGO = new GameObject("Panel");
                var panelRect = panelGO.AddComponent<RectTransform>();
                panelRect.SetParent(popRect, false);
                panelRect.anchorMin = Vector2.zero;
                panelRect.anchorMax = Vector2.one;
                panelRect.pivot = new Vector2(0.5f, 0.5f);
                panelRect.offsetMin = Vector2.zero;
                panelRect.offsetMax = Vector2.zero;
                var panelImg = panelGO.AddComponent<Image>();
                panelImg.sprite = entrada.panel;
                panelImg.preserveAspect = true;
                panelImg.raycastTarget = false;
            }

            if (entrada.icono != null)
            {
                var iconGO = new GameObject("Icono");
                var iconRect = iconGO.AddComponent<RectTransform>();
                iconRect.SetParent(popRect, false);
                iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                iconRect.pivot = new Vector2(0.5f, 0.5f);
                iconRect.sizeDelta = tamanoIcono;
                iconRect.anchoredPosition = posicionIcono;
                var iconImg = iconGO.AddComponent<RawImage>();
                iconImg.texture = entrada.icono;
                iconImg.raycastTarget = false;
            }

            if (!string.IsNullOrEmpty(entrada.descripcion))
            {
                var descGO = new GameObject("Descripcion");
                var descRect = descGO.AddComponent<RectTransform>();
                descRect.SetParent(popRect, false);
                descRect.anchorMin = new Vector2(0.5f, 0.5f);
                descRect.anchorMax = new Vector2(0.5f, 0.5f);
                descRect.pivot = new Vector2(0.5f, 0.5f);
                descRect.sizeDelta = tamanoDescripcion;
                descRect.anchoredPosition = posicionDescripcion;
                var texto = descGO.AddComponent<TextMeshProUGUI>();
                texto.text = entrada.descripcion;
                texto.fontSize = tamanoFuenteDescripcion;
                texto.color = colorTextoDescripcion;
                texto.alignment = TextAlignmentOptions.Center;
                texto.enableWordWrapping = true;
                texto.raycastTarget = false;
            }

            popRect.localScale = Vector3.one * escalaInicial;

            float t = 0f;
            while (t < duracionFadeIn)
            {
                if (grupo == null) yield break;
                t += Time.unscaledDeltaTime;
                float p = Mathf.Clamp01(t / duracionFadeIn);
                grupo.alpha = p;
                float curva = EaseOutBack(p);
                if (popRect != null)
                    popRect.localScale = Vector3.one * Mathf.LerpUnclamped(escalaInicial, 1f, curva);
                yield return null;
            }
            if (grupo != null) grupo.alpha = 1f;
            if (popRect != null) popRect.localScale = Vector3.one;

            yield return new WaitForSecondsRealtime(duracionPermanencia);

            t = 0f;
            while (t < duracionFadeOut)
            {
                if (grupo == null) yield break;
                t += Time.unscaledDeltaTime;
                grupo.alpha = 1f - Mathf.Clamp01(t / duracionFadeOut);
                yield return null;
            }

            if (contenedor != null) Destroy(contenedor);
            if (_overlayActivo == contenedor) _overlayActivo = null;
            _enCurso = null;
        }

        private static float EaseOutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            float t = x - 1f;
            return 1f + c3 * t * t * t + c1 * t * t;
        }
    }
}

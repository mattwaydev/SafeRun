using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SafeRun.UI
{
    public class GameWinnerEstadisticaButton : MonoBehaviour
    {
        private const string EscenaDestino = "ESTADISTICA";
        private const string NombreBoton = "estadistica Button";

        [SerializeField] private Sprite spriteFondo;
        [SerializeField] private string textoBoton = "ESTADISTICAS";

        private TMP_FontAsset _fuente;

        private void Start()
        {
            _fuente = ResolveFontAsset();

            RectTransform panel = LocalizarWinnerPanel();
            if (panel == null) return;

            if (panel.Find(NombreBoton) != null) return;

            ReubicarBotonesExistentes(panel);
            CrearBoton(panel);
        }

        private RectTransform LocalizarWinnerPanel()
        {
            var winnerPanel = GameObject.Find("Winner Panel");
            if (winnerPanel != null) return winnerPanel.GetComponent<RectTransform>();

            var canvas = FindAnyObjectByType<Canvas>();
            return canvas != null ? canvas.GetComponent<RectTransform>() : null;
        }

        private void ReubicarBotonesExistentes(RectTransform panel)
        {
            for (int i = 0; i < panel.childCount; i++)
            {
                var hijo = panel.GetChild(i) as RectTransform;
                if (hijo == null) continue;
                string nombre = hijo.gameObject.name;
                if (nombre.Contains("volver") || nombre.Contains("salir"))
                {
                    var pos = hijo.anchoredPosition;
                    if (pos.y < -100f)
                    {
                        pos.y = -185f;
                        hijo.anchoredPosition = pos;
                    }
                }
            }
        }

        private void CrearBoton(RectTransform panel)
        {
            var go = new GameObject(NombreBoton);
            go.transform.SetParent(panel, false);
            int uiLayer = LayerMask.NameToLayer("UI");
            if (uiLayer >= 0) go.layer = uiLayer;

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(280f, 50f);
            rect.anchoredPosition = new Vector2(14.27f, -115f);

            var img = go.AddComponent<Image>();
            if (spriteFondo != null)
            {
                img.sprite = spriteFondo;
                img.preserveAspect = false;
                img.color = new Color(1f, 1f, 1f, 0.95f);
            }
            else
            {
                img.color = new Color(0.18f, 0.07f, 0.32f, 0.92f);
            }

            var contorno = go.AddComponent<Outline>();
            contorno.effectColor = new Color(1f, 0.85f, 0.35f, 1f);
            contorno.effectDistance = new Vector2(2f, -2f);

            var boton = go.AddComponent<Button>();
            boton.targetGraphic = img;
            var colores = boton.colors;
            colores.normalColor = new Color(1f, 1f, 1f, 1f);
            colores.highlightedColor = new Color(1f, 0.9f, 0.55f, 1f);
            colores.pressedColor = new Color(0.85f, 0.7f, 0.3f, 1f);
            colores.selectedColor = new Color(1f, 0.95f, 0.6f, 1f);
            colores.fadeDuration = 0.12f;
            boton.colors = colores;
            boton.onClick.AddListener(IrAEstadistica);

            var textoGO = new GameObject("Texto");
            textoGO.transform.SetParent(rect, false);
            var tmp = textoGO.AddComponent<TextMeshProUGUI>();
            tmp.font = _fuente;
            tmp.text = textoBoton;
            tmp.fontSize = 14f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(1f, 0.95f, 0.55f, 1f);
            tmp.raycastTarget = false;
            var sombra = textoGO.AddComponent<Shadow>();
            sombra.effectColor = new Color(0f, 0f, 0f, 0.85f);
            sombra.effectDistance = new Vector2(1.2f, -1.2f);
            var textoRect = tmp.rectTransform;
            textoRect.anchorMin = Vector2.zero;
            textoRect.anchorMax = Vector2.one;
            textoRect.offsetMin = Vector2.zero;
            textoRect.offsetMax = Vector2.zero;
        }

        private static void IrAEstadistica()
        {
            SceneManager.LoadScene(EscenaDestino);
        }

        private static TMP_FontAsset ResolveFontAsset()
        {
            var tmps = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var t in tmps)
            {
                if (t != null && t.font != null && t.font.name.Contains("PressStart2P"))
                    return t.font;
            }
            foreach (var t in tmps)
            {
                if (t != null && t.font != null) return t.font;
            }
            foreach (var f in Resources.FindObjectsOfTypeAll<TMP_FontAsset>())
            {
                if (f != null && f.name.Contains("PressStart2P"))
                    return f;
            }
            return TMP_Settings.defaultFontAsset;
        }
    }
}

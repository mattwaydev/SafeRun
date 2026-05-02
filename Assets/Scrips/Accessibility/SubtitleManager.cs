using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SafeRun.Accessibility
{
    public class SubtitleManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textoSubtitulo;
        [SerializeField] private CanvasGroup panelSubtitulo;
        [SerializeField] private Image fondoPanel;
        [SerializeField] private bool ocultarFondo = true;
        [SerializeField] private bool ocultarAlInicio = true;
        [SerializeField] private float duracionFade = 0.2f;

        private void Awake()
        {
            if (ocultarAlInicio)
            {
                if (panelSubtitulo != null)
                {
                    panelSubtitulo.alpha = 0f;
                    panelSubtitulo.interactable = false;
                    panelSubtitulo.blocksRaycasts = false;
                }

                if (textoSubtitulo != null)
                    textoSubtitulo.text = string.Empty;
            }

            if (ocultarFondo)
            {
                Image img = fondoPanel;
                if (img == null && panelSubtitulo != null)
                    img = panelSubtitulo.GetComponent<Image>();

                if (img != null)
                {
                    Color c = img.color;
                    c.a = 0f;
                    img.color = c;
                }
            }
        }

        public void Mostrar(string texto, float duracion)
        {
            if (textoSubtitulo == null || panelSubtitulo == null) return;
            StopAllCoroutines();
            StartCoroutine(MostrarRutina(texto, duracion));
        }

        private IEnumerator MostrarRutina(string texto, float duracion)
        {
            textoSubtitulo.text = texto;
            yield return StartCoroutine(Fade(0f, 1f));

            yield return new WaitForSeconds(duracion);

            yield return StartCoroutine(Fade(1f, 0f));
            textoSubtitulo.text = string.Empty;
        }

        private IEnumerator Fade(float desde, float hasta)
        {
            float tiempo = 0f;
            while (tiempo < duracionFade)
            {
                tiempo += Time.deltaTime;
                panelSubtitulo.alpha = Mathf.Lerp(desde, hasta, tiempo / duracionFade);
                yield return null;
            }

            panelSubtitulo.alpha = hasta;
        }
    }
}

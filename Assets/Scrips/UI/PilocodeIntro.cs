using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SafeRun.UI
{
    public class PilocodeIntro : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private string escenaDestino = "Menu Principal";
        [SerializeField] private float duracion = 3f;
        [SerializeField] private float esperaFinal = 0.3f;

        private void Start()
        {
            if (slider != null)
            {
                slider.minValue = 0f;
                slider.maxValue = 1f;
                slider.value = 0f;
                slider.interactable = false;
            }
            StartCoroutine(Cargar());
        }

        private IEnumerator Cargar()
        {
            float t = 0f;
            while (t < duracion)
            {
                t += Time.unscaledDeltaTime;
                if (slider != null)
                    slider.value = Mathf.Clamp01(t / duracion);
                yield return null;
            }
            if (slider != null) slider.value = 1f;

            if (esperaFinal > 0f)
                yield return new WaitForSecondsRealtime(esperaFinal);

            if (!string.IsNullOrWhiteSpace(escenaDestino))
                SceneManager.LoadScene(escenaDestino);
        }
    }
}

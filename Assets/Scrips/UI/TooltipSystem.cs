using TMPro;
using UnityEngine;

namespace SafeRun.UI
{
    public class TooltipSystem : MonoBehaviour
    {
        [SerializeField] private RectTransform panelTooltip;
        [SerializeField] private TextMeshProUGUI textoTooltip;
        [SerializeField] private float offsetY = 40f;

        public void Mostrar(string texto, Vector3 posicionMundo)
        {
            if (panelTooltip == null || textoTooltip == null) return;

            textoTooltip.text = texto;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(posicionMundo);
            panelTooltip.position = new Vector3(screenPos.x, screenPos.y + offsetY, screenPos.z);
            panelTooltip.gameObject.SetActive(true);
        }

        public void Ocultar()
        {
            if (panelTooltip == null) return;
            panelTooltip.gameObject.SetActive(false);
        }
    }
}

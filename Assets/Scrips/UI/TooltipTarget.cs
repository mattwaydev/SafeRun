using UnityEngine;

namespace SafeRun.UI
{
    public class TooltipTarget : MonoBehaviour
    {
        [SerializeField] private string mensaje = "Presiona E para interactuar";
        [SerializeField] private TooltipSystem tooltipSystem;

        private void Awake()
        {
            if (tooltipSystem == null)
                tooltipSystem = FindAnyObjectByType<TooltipSystem>();
        }

        private void OnMouseEnter()
        {
            if (tooltipSystem != null)
                tooltipSystem.Mostrar(mensaje, transform.position);
        }

        private void OnMouseExit()
        {
            if (tooltipSystem != null)
                tooltipSystem.Ocultar();
        }
    }
}

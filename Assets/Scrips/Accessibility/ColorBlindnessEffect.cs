using UnityEngine;

namespace SafeRun.Accessibility
{
    [RequireComponent(typeof(Camera))]
    public class ColorBlindnessEffect : MonoBehaviour
    {
        private Material _material;

        public void Configurar(Material material)
        {
            _material = material;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_material == null)
            {
                Graphics.Blit(source, destination);
                return;
            }

            Graphics.Blit(source, destination, _material);
        }
    }
}

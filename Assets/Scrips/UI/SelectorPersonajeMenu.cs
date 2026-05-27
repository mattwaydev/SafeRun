using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SafeRun.Entities;

namespace SafeRun.UI
{
    public class SelectorPersonajeMenu : MonoBehaviour
    {
        [System.Serializable]
        public class Opcion
        {
            public string nombre = "personaje";
            public Sprite preview;
        }

        [SerializeField] private Opcion[] opciones;
        [SerializeField] private Image imagenPreview;
        [SerializeField] private RawImage imagenPreviewRaw;
        [SerializeField] private Text textoNombre;
        [SerializeField] private TMPro.TMP_Text textoNombreTMP;
        [SerializeField] private string escenaDestino = "School Main";

        private int _index;

        private void Start()
        {
            if (opciones == null || opciones.Length == 0)
                Debug.LogWarning("[SelectorPersonajeMenu] Opciones vacío. Asigna al menos una opción en el inspector.", this);
            if (imagenPreview == null && imagenPreviewRaw == null)
                Debug.LogWarning("[SelectorPersonajeMenu] imagenPreview e imagenPreviewRaw nulos. Asigna una Image o RawImage.", this);

            _index = Mathf.Clamp(CharacterSwitcher.CargarSeleccion(0), 0, Mathf.Max(0, (opciones?.Length ?? 1) - 1));
            Refrescar();
        }

        public void Siguiente()
        {
            Debug.Log("[SelectorPersonajeMenu] Siguiente clic", this);
            if (opciones == null || opciones.Length == 0) return;
            _index = (_index + 1) % opciones.Length;
            Refrescar();
        }

        public void Atras()
        {
            Debug.Log("[SelectorPersonajeMenu] Atras clic", this);
            if (opciones == null || opciones.Length == 0) return;
            _index = (_index - 1 + opciones.Length) % opciones.Length;
            Refrescar();
        }

        public void Confirmar()
        {
            Debug.Log($"[SelectorPersonajeMenu] Confirmar index={_index}", this);
            CharacterSwitcher.GuardarSeleccion(_index);
        }

        public void Jugar()
        {
            Debug.Log($"[SelectorPersonajeMenu] Jugar index={_index} -> escena '{escenaDestino}'", this);
            CharacterSwitcher.GuardarSeleccion(_index);
            if (string.IsNullOrWhiteSpace(escenaDestino))
            {
                Debug.LogError("[SelectorPersonajeMenu] escenaDestino vacío.", this);
                return;
            }
            SceneManager.LoadScene(escenaDestino);
        }

        private void Refrescar()
        {
            if (opciones == null || opciones.Length == 0) return;
            var op = opciones[_index];
            if (imagenPreview != null) imagenPreview.sprite = op.preview;
            if (imagenPreviewRaw != null) AsignarRawImage(imagenPreviewRaw, op.preview);
            if (textoNombre != null) textoNombre.text = op.nombre;
            if (textoNombreTMP != null) textoNombreTMP.text = op.nombre;
        }

        private static void AsignarRawImage(RawImage ri, Sprite sprite)
        {
            if (sprite == null || sprite.texture == null)
            {
                ri.texture = null;
                ri.uvRect = new Rect(0, 0, 1, 1);
                return;
            }
            ri.texture = sprite.texture;
            var r = sprite.textureRect;
            float tw = sprite.texture.width;
            float th = sprite.texture.height;
            ri.uvRect = new Rect(r.x / tw, r.y / th, r.width / tw, r.height / th);
        }
    }
}

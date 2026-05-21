using UnityEngine;

namespace SafeRun.UI
{
    [CreateAssetMenu(fileName = "PopupHabilidadConfig", menuName = "SafeRun/Popup Habilidad Config")]
    public class PopupHabilidadConfig : ScriptableObject
    {
        [System.Serializable]
        public class Entrada
        {
            public string nombreItem;
            public Sprite panel;
            public Texture icono;
            [TextArea(2, 5)] public string descripcion;
        }

        public Entrada[] entradas;

        public Entrada Buscar(string nombreItem)
        {
            if (entradas == null || string.IsNullOrWhiteSpace(nombreItem)) return null;
            for (int i = 0; i < entradas.Length; i++)
            {
                var e = entradas[i];
                if (e != null && e.nombreItem == nombreItem) return e;
            }
            return null;
        }
    }
}

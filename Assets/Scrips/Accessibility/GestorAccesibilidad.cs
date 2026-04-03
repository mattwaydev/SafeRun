// GestorAccesibilidad.cs — Componente inclusivo
// Cumple requisito funcional b) de ambas materias
using UnityEngine;

namespace SafeRun.Accessibility
{
    public class GestorAccesibilidad : MonoBehaviour
    {
        [Header("Opciones de accesibilidad")]
        [SerializeField] private bool modoDaltonico     = false;
        [SerializeField] private bool subtitulosActivos = true;
        [SerializeField] private bool narracionVoz      = false;
        [SerializeField] private string skinPersonaje   = "default";

        public void ActivarModoDaltonico(bool activo)
        {
            modoDaltonico = activo;
            Debug.Log($"[Accesibilidad] Modo daltonico: {activo}");
            // Aqui se aplica el shader/paleta de colores alternativa
        }

        public void ActivarSubtitulos(bool activo)
        {
            subtitulosActivos = activo;
            Debug.Log($"[Accesibilidad] Subtitulos: {activo}");
        }

        public void ActivarNarracion(bool activo)
        {
            narracionVoz = activo;
            Debug.Log($"[Accesibilidad] Narracion de voz: {activo}");
        }

        public void CambiarSkin(string skin)
        {
            skinPersonaje = skin;
            Debug.Log($"[Accesibilidad] Skin cambiada a: {skin}");
        }

        public bool ModoDaltonico    => modoDaltonico;
        public bool Subtitulos       => subtitulosActivos;
        public string SkinPersonaje  => skinPersonaje;
    }
}

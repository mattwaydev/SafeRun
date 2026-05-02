using UnityEngine;
using SafeRun.Core;

namespace SafeRun.Audio
{
    public class AudioBridge : MonoBehaviour
    {
        [SerializeField] private SoundEffectsManager sfxManager;
        [SerializeField] private GestorJuego gestorJuego;

        private void Awake()
        {
            if (gestorJuego == null)
                gestorJuego = FindAnyObjectByType<GestorJuego>();
        }

        private void OnEnable()
        {
            if (gestorJuego != null)
                gestorJuego.SonidoSolicitado += OnSonidoSolicitado;
        }

        private void OnDisable()
        {
            if (gestorJuego != null)
                gestorJuego.SonidoSolicitado -= OnSonidoSolicitado;
        }

        private void OnSonidoSolicitado(string clave)
        {
            if (sfxManager == null) return;
            sfxManager.Reproducir(clave);
        }
    }
}

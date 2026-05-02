using System;
using UnityEngine;

namespace SafeRun.Audio
{
    public class NarrationManager : MonoBehaviour
    {
        [Serializable]
        public struct NarracionClip
        {
            public string clave;
            public AudioClip audioClip;
            public string textoFallback;
        }

        [SerializeField] private AudioSource fuenteNarracion;
        [SerializeField] private NarracionClip[] clips;
        [SerializeField] private bool usarTtsSiNoHayClip = true;

        public void Reproducir(string clave)
        {
            var clip = Array.Find(clips, c => c.clave == clave);

            if (clip.audioClip != null)
            {
                if (fuenteNarracion != null)
                    fuenteNarracion.PlayOneShot(clip.audioClip);
                return;
            }

            if (usarTtsSiNoHayClip && !string.IsNullOrWhiteSpace(clip.textoFallback))
            {
                Debug.Log($"[SafeRun][TTS] {clip.textoFallback}");
            }
        }
    }
}

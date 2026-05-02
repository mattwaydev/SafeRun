using System;
using UnityEngine;

namespace SafeRun.Audio
{
    public class SoundEffectsManager : MonoBehaviour
    {
        [Serializable]
        public struct Sfx
        {
            public string clave;
            public AudioClip clip;
            public float volumen;
        }

        [SerializeField] private AudioSource fuenteSfx;
        [SerializeField] private Sfx[] sonidos;

        public void Reproducir(string clave)
        {
            if (fuenteSfx == null) return;
            var sfx = Array.Find(sonidos, s => s.clave == clave);
            if (sfx.clip == null) return;
            float volumen = sfx.volumen <= 0f ? 1f : sfx.volumen;
            fuenteSfx.PlayOneShot(sfx.clip, volumen);
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

namespace SafeRun.Audio
{
    public class MusicManager : MonoBehaviour
    {
        private const string ClipResourcePath = "GameBaseMusic";
        private const string GameScenePrefix = "School";

        private static MusicManager _instancia;

        private AudioSource _fuente;
        private AudioClip _clip;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Inicializar()
        {
            if (_instancia != null) return;

            var go = new GameObject("MusicManager");
            _instancia = go.AddComponent<MusicManager>();
            DontDestroyOnLoad(go);
        }

        private void Awake()
        {
            if (_instancia != null && _instancia != this)
            {
                Destroy(gameObject);
                return;
            }

            _instancia = this;

            _fuente = gameObject.AddComponent<AudioSource>();
            _fuente.loop = true;
            _fuente.playOnAwake = false;
            _fuente.spatialBlend = 0f;
            _fuente.volume = 1f;

            _clip = Resources.Load<AudioClip>(ClipResourcePath);
            if (_clip == null)
            {
                Debug.LogWarning($"[MusicManager] No se encontro el clip en Resources/{ClipResourcePath}.");
                return;
            }

            _fuente.clip = _clip;

            SceneManager.sceneLoaded += AlCargarEscena;
            AjustarParaEscena(SceneManager.GetActiveScene());
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= AlCargarEscena;
            if (_instancia == this)
                _instancia = null;
        }

        private void AlCargarEscena(Scene escena, LoadSceneMode modo)
        {
            if (modo != LoadSceneMode.Single) return;
            AjustarParaEscena(escena);
        }

        private void AjustarParaEscena(Scene escena)
        {
            if (_fuente == null || _clip == null) return;

            bool esEscenaJuego = escena.IsValid()
                && escena.name != null
                && escena.name.StartsWith(GameScenePrefix);

            if (esEscenaJuego)
            {
                if (!_fuente.isPlaying)
                {
                    _fuente.time = 0f;
                    _fuente.Play();
                }
            }
            else if (_fuente.isPlaying)
            {
                _fuente.Stop();
            }
        }
    }
}

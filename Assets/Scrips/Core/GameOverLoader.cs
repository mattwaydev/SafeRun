using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SafeRun.Core
{
    public class GameOverLoader : MonoBehaviour
    {
        private static GameOverLoader _instancia;

        public static void Programar(string escena, float retardo, GestorJuego gestorJuego, GameObject jugador, bool esVictoria = false)
        {
            if (_instancia != null) return;

            var go = new GameObject("GameOverLoader");
            DontDestroyOnLoad(go);
            _instancia = go.AddComponent<GameOverLoader>();
            _instancia.StartCoroutine(_instancia.Ejecutar(escena, retardo, gestorJuego, jugador, esVictoria));
        }

        private IEnumerator Ejecutar(string escena, float retardo, GestorJuego gestorJuego, GameObject jugador, bool esVictoria)
        {
            Debug.Log($"[SafeRun] GameOverLoader esperando {retardo}s antes de cargar '{escena}'");

            if (retardo > 0f)
                yield return new WaitForSeconds(retardo);

            if (gestorJuego != null)
            {
                try
                {
                    if (esVictoria)
                        gestorJuego.Victoria();
                    else
                        gestorJuego.GameOver();
                }
                catch (System.Exception e) { Debug.LogWarning($"[SafeRun] GameOverLoader falla notificar gestor: {e.Message}"); }
            }

            if (jugador != null)
                jugador.SetActive(false);

            Debug.Log($"[SafeRun] GameOverLoader iniciando LoadSceneAsync '{escena}'");
            var op = SceneManager.LoadSceneAsync(escena);
            if (op == null)
            {
                Debug.LogError($"[SafeRun] GameOverLoader: LoadSceneAsync devolvio null. ¿La escena '{escena}' esta en Build Settings?");
                Destroy(gameObject);
                yield break;
            }
            while (!op.isDone)
                yield return null;

            Debug.Log("[SafeRun] GameOverLoader escena cargada, limpiando objetos persistentes");
            try { if (gestorJuego != null) Destroy(gestorJuego.gameObject); } catch (System.Exception e) { Debug.LogWarning(e.Message); }
            try
            {
                var gestorEscenas = GestorEscenas.Instancia;
                if (gestorEscenas != null) Destroy(gestorEscenas.gameObject);
            }
            catch (System.Exception e) { Debug.LogWarning(e.Message); }
            try { if (jugador != null) Destroy(jugador); } catch (System.Exception e) { Debug.LogWarning(e.Message); }
            try { Item.ReiniciarRecogidos(); } catch (System.Exception e) { Debug.LogWarning(e.Message); }

            _instancia = null;
            Destroy(gameObject);
        }
    }
}

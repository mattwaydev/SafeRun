using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SafeRun.Core
{
    public class GameOverLoader : MonoBehaviour
    {
        private static GameOverLoader _instancia;

        public static void Programar(string escena, float retardo, GestorJuego gestorJuego, GameObject jugador)
        {
            if (_instancia != null) return;

            var go = new GameObject("GameOverLoader");
            DontDestroyOnLoad(go);
            _instancia = go.AddComponent<GameOverLoader>();
            _instancia.StartCoroutine(_instancia.Ejecutar(escena, retardo, gestorJuego, jugador));
        }

        private IEnumerator Ejecutar(string escena, float retardo, GestorJuego gestorJuego, GameObject jugador)
        {
            Debug.Log($"[SafeRun] GameOverLoader esperando {retardo}s antes de cargar '{escena}'");

            if (retardo > 0f)
                yield return new WaitForSeconds(retardo);

            if (gestorJuego != null)
            {
                gestorJuego.GameOver();
                Destroy(gestorJuego.gameObject);
            }

            var gestorEscenas = GestorEscenas.Instancia;
            if (gestorEscenas != null)
                Destroy(gestorEscenas.gameObject);

            if (jugador != null)
                Destroy(jugador);

            Debug.Log($"[SafeRun] GameOverLoader cargando escena '{escena}'");
            SceneManager.LoadScene(escena);

            Destroy(gameObject);
        }
    }
}

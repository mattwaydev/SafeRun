using UnityEngine;
using SafeRun.Core;

namespace SafeRun.Core
{
    public class GameplayHints : MonoBehaviour
    {
        [SerializeField] private GestorJuego gestorJuego;
        [SerializeField] private float intervalo = 25f;
        [SerializeField] private string[] mensajes =
        {
            "Recuerda: puedes reportar situaciones de acoso.",
            "Busca apoyo en tus aliados, no estas solo.",
            "Evita a los agresores y protege tu empatia.",
            "Las pausas ayudan, respira y sigue.",
            "Cada ayuda cuenta, sigue avanzando."
        };

        private float _timer;

        private void Awake()
        {
            if (gestorJuego == null)
                gestorJuego = FindAnyObjectByType<GestorJuego>();
        }

        private void Update()
        {
            if (gestorJuego == null) return;
            _timer += Time.deltaTime;
            if (_timer >= intervalo)
            {
                _timer = 0f;
                string msg = mensajes[Random.Range(0, mensajes.Length)];
                gestorJuego.SolicitarSubtitulo(msg);
            }
        }
    }
}

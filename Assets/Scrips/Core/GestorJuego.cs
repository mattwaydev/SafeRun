// GestorJuego.cs
// POO: Patron Observer — notifica UI, audio y stats al cambiar GameState
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SafeRun.Core
{
    public class GestorJuego : MonoBehaviour
    {
        private static GestorJuego _instancia;

        private GameState _estado;
        private int _puntos;
        private int _nivel;
        private readonly List<Action<GameState>> _listeners = new();

        public event Action<GameState> EstadoCambiado;
        public event Action<int> PuntosCambiados;
        public event Action<string> SubtituloSolicitado;
        public event Action<string> NarracionSolicitada;
        public event Action<string> SonidoSolicitado;

        private void Awake()
        {
            if (_instancia != null && _instancia != this)
            {
                Destroy(gameObject);
                return;
            }

            _instancia = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (_instancia == this)
                _instancia = null;
        }

        

        public void Suscribir(Action<GameState> cb)   => _listeners.Add(cb);
        public void Desuscribir(Action<GameState> cb) => _listeners.Remove(cb);

        public void CambiarEstado(GameState nuevo)
        {
            _estado = nuevo;
            foreach (var cb in _listeners) cb?.Invoke(_estado);
            EstadoCambiado?.Invoke(_estado);
        }

        public void IniciarNivel(int n)
        {
            _nivel = n;
            CambiarEstado(GameState.Playing);
            NarracionSolicitada?.Invoke($"nivel_{n}");
        }

        public void AgregarPuntos(int p)
        {
            int anterior = _puntos;
            _puntos += p;
            PuntosCambiados?.Invoke(_puntos);

            if (p > 0 && _puntos / 100 > anterior / 100)
                SubtituloSolicitado?.Invoke(ObtenerMensajeMotivador());
        }

        public void SolicitarSubtitulo(string texto) => SubtituloSolicitado?.Invoke(texto);
        public void SolicitarNarracion(string clave) => NarracionSolicitada?.Invoke(clave);
        public void SolicitarSonido(string clave) => SonidoSolicitado?.Invoke(clave);

        public void GameOver()
        {
            CambiarEstado(GameState.GameOver);
            SonidoSolicitado?.Invoke("game_over");
        }

        public void Victoria()
        {
            CambiarEstado(GameState.Victory);
            SonidoSolicitado?.Invoke("victory");
        }

        private string ObtenerMensajeMotivador()
        {
            string[] mensajes =
            {
                "Buen trabajo, sigue adelante.",
                "Cada paso cuenta, tu puedes.",
                "No estas solo, sigue avanzando.",
                "Respira, estas mejorando.",
                "Gran esfuerzo, continua asi."
            };

            return mensajes[UnityEngine.Random.Range(0, mensajes.Length)];
        }

        public GameState Estado => _estado;
        public int Puntos => _puntos;
        public int Nivel  => _nivel;
    }
}

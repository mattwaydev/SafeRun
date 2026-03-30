// GestorJuego.cs
// POO: Patron Observer — notifica UI, audio y stats al cambiar GameState
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SafeRun.Core
{
    public class GestorJuego : MonoBehaviour
    {
        

        private GameState _estado;
        private int _puntos;
        private int _nivel;
        private readonly List<Action<GameState>> _listeners = new();

        

        public void Suscribir(Action<GameState> cb)   => _listeners.Add(cb);
        public void Desuscribir(Action<GameState> cb) => _listeners.Remove(cb);

        public void CambiarEstado(GameState nuevo)
        {
            _estado = nuevo;
            foreach (var cb in _listeners) cb?.Invoke(_estado);
        }

        public void IniciarNivel(int n) { _nivel = n; CambiarEstado(GameState.Playing); }
        public void AgregarPuntos(int p) => _puntos += p;
        public void GameOver() => CambiarEstado(GameState.GameOver);
        public void Victoria() => CambiarEstado(GameState.Victory);

        public GameState Estado => _estado;
        public int Puntos => _puntos;
        public int Nivel  => _nivel;
    }
}

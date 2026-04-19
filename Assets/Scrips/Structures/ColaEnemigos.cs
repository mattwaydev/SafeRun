// ColaEnemigos.cs — Cola (Queue) de oleadas
// ED: estructura Cola — enemigos esperan en orden para entrar a la sala
using System.Collections.Generic;
using UnityEngine;

namespace SafeRun.Structures
{
    public class ColaEnemigos
    {
        private readonly Queue<string> _cola = new();

        public void Encolar(string tipoEnemigo)
        {
            _cola.Enqueue(tipoEnemigo);
            Debug.Log($"[Cola] Enemigo encolado: {tipoEnemigo} | Total: {_cola.Count}");
        }

        public string Desencolar()
        {
            if (_cola.Count == 0) return null;
            var e = _cola.Dequeue();
            Debug.Log($"[Cola] Enemigo despachado: {e}");
            return e;
        }

        public bool HayEnemigos => _cola.Count > 0;
        public int CantidadPendiente => _cola.Count;
        public string VerSiguiente() => _cola.Count > 0 ? _cola.Peek() : null;
    }
}

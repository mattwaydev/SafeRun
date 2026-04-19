// PilaHistorialSalas.cs — Pila (Stack) de salas visitadas
// ED: estructura Pila — permite al jugador ver su recorrido
using System.Collections.Generic;
using UnityEngine;

namespace SafeRun.Structures
{
    public class PilaHistorialSalas
    {
        private readonly Stack<string> _pila = new();

        public void Push(string nombreSala)
        {
            _pila.Push(nombreSala);
            Debug.Log($"[Historial] Sala guardada: {nombreSala}");
        }

        public string Pop()
        {
            if (_pila.Count == 0) return null;
            return _pila.Pop();
        }

        public string VerUltima() => _pila.Count > 0 ? _pila.Peek() : null;
        public int Profundidad => _pila.Count;
        public bool EstaVacia  => _pila.Count == 0;
    }
}

// InventarioArmas.cs — Lista enlazada simple (TAD ED)
// ED: estructura Lista Enlazada — inventario del jugador
// Permite agregar/eliminar armas dinamicamente durante el run
using UnityEngine;

namespace SafeRun.Structures
{
    public class InventarioArmas
    {
        private NodoArma _cabeza;
        private int _cantidad;

        public void Agregar(string nombre, float danio, int municion)
        {
            var nuevo = new NodoArma(nombre, danio, municion);
            if (_cabeza == null) { _cabeza = nuevo; }
            else
            {
                var actual = _cabeza;
                while (actual.Siguiente != null) actual = actual.Siguiente;
                actual.Siguiente = nuevo;
            }
            _cantidad++;
            Debug.Log($"[Inventario] Arma agregada: {nombre}");
        }

        public bool Eliminar(string nombre)
        {
            if (_cabeza == null) return false;
            if (_cabeza.NombreArma == nombre) { _cabeza = _cabeza.Siguiente; _cantidad--; return true; }
            var actual = _cabeza;
            while (actual.Siguiente != null)
            {
                if (actual.Siguiente.NombreArma == nombre)
                {
                    actual.Siguiente = actual.Siguiente.Siguiente;
                    _cantidad--;
                    return true;
                }
                actual = actual.Siguiente;
            }
            return false;
        }

        public NodoArma ObtenerPrimera() => _cabeza;
        public int Cantidad => _cantidad;
        public bool EstaVacio => _cabeza == null;

        public void Imprimir()
        {
            var actual = _cabeza;
            while (actual != null)
            {
                Debug.Log($"  - {actual.NombreArma} | Daño:{actual.Danio} | Mun:{actual.Municion}");
                actual = actual.Siguiente;
            }
        }
    }
}

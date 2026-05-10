// NodoArma.cs — Nodo para lista enlazada de inventario
// ED: estructura de datos nodo generico
namespace SafeRun.Structures
{
    public class NodoArma
    {
        public string NombreArma;
        public NodoArma Siguiente;

        public NodoArma(string nombre)
        {
            NombreArma = nombre;
            Siguiente  = null;
        }
    }
}

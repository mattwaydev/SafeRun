// NodoArma.cs — Nodo para lista enlazada de inventario
// ED: estructura de datos nodo generico
namespace SafeRun.Structures
{
    public class NodoArma
    {
        public string NombreArma;
        public float Danio;
        public int Municion;
        public NodoArma Siguiente;

        public NodoArma(string nombre, float danio, int municion)
        {
            NombreArma = nombre;
            Danio      = danio;
            Municion   = municion;
            Siguiente  = null;
        }
    }
}

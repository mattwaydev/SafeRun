// FabricaSalas.cs — Patron Factory Method
// POO: patron de disenio Factory Method — crea SalaEnemigos o SalaJefe segun semilla
using UnityEngine;

namespace SafeRun.Patterns
{
    public enum TipoSala { Enemigos, Jefe, Descanso }

    public abstract class Sala
    {
        public string Id;
        public TipoSala Tipo;
        public abstract void Inicializar();
    }

    public class SalaEnemigos : Sala
    {
        public int CantidadOleadas;
        public SalaEnemigos(string id, int oleadas)
        {
            Id = id; Tipo = TipoSala.Enemigos; CantidadOleadas = oleadas;
        }
        public override void Inicializar() =>
            Debug.Log($"[Sala] Sala de enemigos {Id} iniciada — {CantidadOleadas} oleadas");
    }

    public class SalaJefe : Sala
    {
        public string NombreJefe;
        public SalaJefe(string id, string jefe)
        {
            Id = id; Tipo = TipoSala.Jefe; NombreJefe = jefe;
        }
        public override void Inicializar() =>
            Debug.Log($"[Sala] Sala del jefe {NombreJefe} iniciada");
    }

    public class SalaDescanso : Sala
    {
        public SalaDescanso(string id) { Id = id; Tipo = TipoSala.Descanso; }
        public override void Inicializar() =>
            Debug.Log($"[Sala] Sala de descanso {Id} iniciada");
    }

    // Factory: decide qué tipo de sala crear
    public static class FabricaSalas
    {
        public static Sala Crear(TipoSala tipo, string id, int parametro = 1)
        {
            return tipo switch
            {
                TipoSala.Enemigos  => new SalaEnemigos(id, parametro),
                TipoSala.Jefe      => new SalaJefe(id, $"Jefe_Nivel{parametro}"),
                TipoSala.Descanso  => new SalaDescanso(id),
                _                  => new SalaDescanso(id)
            };
        }
    }
}

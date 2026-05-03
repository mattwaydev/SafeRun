using UnityEngine;

namespace SafeRun.Core
{
    public class PuertaSala : MonoBehaviour
    {
        [SerializeField] private string salaDestino;
        [SerializeField] private string spawnDestino;
        [SerializeField] private bool usarSalaAnterior = false;

        private bool _activada;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_activada) return;
            if (other.GetComponentInParent<SafeRun.Entities.Jugador>() == null) return;

            if (GestorEscenas.Instancia == null)
                return;

            _activada = true;

            if (!string.IsNullOrWhiteSpace(spawnDestino))
                GestorEscenas.Instancia.DefinirSpawnDestino(spawnDestino);

            if (usarSalaAnterior)
                GestorEscenas.Instancia.VolverASalaAnterior();
            else if (!string.IsNullOrWhiteSpace(salaDestino))
                GestorEscenas.Instancia.IrASala(salaDestino);
        }
    }
}

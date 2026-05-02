using UnityEngine;

namespace SafeRun.Core
{
    public class PuertaSala : MonoBehaviour
    {
        [SerializeField] private string salaDestino;
        [SerializeField] private bool volverASalaAnterior = false;

        private bool _activada;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_activada) return;
            if (other.GetComponentInParent<SafeRun.Entities.Jugador>() == null) return;

            _activada = true;

            if (GestorEscenas.Instancia == null)
                return;

            if (volverASalaAnterior)
                GestorEscenas.Instancia.IrASala("School Main");
            else if (!string.IsNullOrWhiteSpace(salaDestino))
                GestorEscenas.Instancia.IrASala(salaDestino);
        }
    }
}

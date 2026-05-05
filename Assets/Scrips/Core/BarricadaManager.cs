using UnityEngine;
using SafeRun.Entities;

namespace SafeRun.Core
{
    public class BarricadaManager : MonoBehaviour
    {
        [SerializeField] private Jugador jugador;
        [SerializeField] private GameObject parte1;
        [SerializeField] private GameObject parte2;
        [SerializeField] private GameObject parte3;

        private int _itemsRemovidos;

        private void Start()
        {
            if (jugador == null)
                jugador = FindAnyObjectByType<Jugador>();

            if (jugador != null && jugador.Inventario != null)
            {
                jugador.Inventario.InventarioCambiado += OnInventarioCambiado;
                _itemsRemovidos = Mathf.Min(jugador.Inventario.Cantidad, 3);
                ActualizarBarricada();
            }
        }

        private void OnDestroy()
        {
            if (jugador != null && jugador.Inventario != null)
                jugador.Inventario.InventarioCambiado -= OnInventarioCambiado;
        }

        private void OnInventarioCambiado(int cantidad)
        {
            _itemsRemovidos = Mathf.Min(cantidad, 3);
            ActualizarBarricada();
        }

        private void ActualizarBarricada()
        {
            if (parte1 != null) parte1.SetActive(_itemsRemovidos < 1);
            if (parte2 != null) parte2.SetActive(_itemsRemovidos < 2);
            if (parte3 != null) parte3.SetActive(_itemsRemovidos < 3);
        }
    }
}

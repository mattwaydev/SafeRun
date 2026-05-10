
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using SafeRun.Entities;
using SafeRun.Structures;

namespace SafeRun.Core
{
    public class BarricadaManager : MonoBehaviour
    {
        [SerializeField] private int maxItems = 3;
        [SerializeField] private float reintentoSegundos = 0.2f;

        private InventarioArmas _inventario;
        private Coroutine _buscarJugadorRutina;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            IniciarBusquedaJugador();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            DesuscribirInventario();
            if (_buscarJugadorRutina != null)
            {
                StopCoroutine(_buscarJugadorRutina);
                _buscarJugadorRutina = null;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            IniciarBusquedaJugador();
        }

        private void IniciarBusquedaJugador()
        {
            if (_buscarJugadorRutina != null)
            {
                StopCoroutine(_buscarJugadorRutina);
            }
            _buscarJugadorRutina = StartCoroutine(BuscarJugadorCuandoExista());
        }

        private IEnumerator BuscarJugadorCuandoExista()
        {
            DesuscribirInventario();

            Jugador jugador = null;
            while (jugador == null)
            {
                jugador = FindObjectOfType<Jugador>();
                if (jugador == null)
                    yield return new WaitForSeconds(reintentoSegundos);
            }

            _inventario = jugador.Inventario;
            if (_inventario != null)
                _inventario.InventarioCambiado += OnInventarioCambiado;

            RefrescarBarricada();
        }

        private void DesuscribirInventario()
        {
            if (_inventario != null)
                _inventario.InventarioCambiado -= OnInventarioCambiado;
            _inventario = null;
        }

        private void OnInventarioCambiado(int cantidad)
        {
            RefrescarBarricada();
        }

        private void RefrescarBarricada()
        {
            if (_inventario == null)
                return;

            int cantidad = Mathf.Clamp(_inventario.Cantidad, 0, maxItems);
            int indiceActivo = Mathf.Clamp(cantidad, 0, 3);

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform hijo = transform.GetChild(i);
                bool activo = i == indiceActivo;
                if (hijo.gameObject.activeSelf != activo)
                    hijo.gameObject.SetActive(activo);
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SafeRun.Structures;


public class Item : MonoBehaviour
{   
    private static readonly HashSet<string> _itemsRecogidos = new HashSet<string>();

    [SerializeField] private string nombreItem = "Item";

    private string _claveItem;

    private void Awake()
    {
        _claveItem = ConstruirClave();
        if (_itemsRecogidos.Contains(_claveItem))
        {
            Destroy(gameObject);
        }
    }

    public void Interactuar(InventarioArmas inventario)
    {
        inventario.Agregar(nombreItem);
        _itemsRecogidos.Add(_claveItem);
        Destroy(gameObject);
    }

    public static void ReiniciarRecogidos()
    {
        _itemsRecogidos.Clear();
    }

    private string ConstruirClave()
    {
        string escena = SceneManager.GetActiveScene().name;
        return escena + "|" + nombreItem + "|" + gameObject.name;
    }
}

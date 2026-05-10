using UnityEngine;
using UnityEngine.InputSystem;
using SafeRun.Structures;

public class Interactuar : MonoBehaviour
{
    [SerializeField] private Transform controlador;
    [SerializeField] private Vector2 dimensiones;
    [SerializeField] private LayerMask capaInteractuable;
    [SerializeField] private InputActionReference interactuar;


    private InventarioArmas _inventario;

    private void Start()
    {
        var jugador = GetComponent<SafeRun.Entities.Jugador>();
        _inventario = jugador.Inventario;
    }
    private void OnEnable()
    {
        interactuar.action.Enable();
        
        interactuar.action.performed += OnInteractuar;

    }

    private void OnDisable()
    {
        interactuar.action.performed -= OnInteractuar;
        interactuar.action.Disable();

    }
    private void OnInteractuar(InputAction.CallbackContext ctx)
    {
        EInteractuar();
    }
    private void EInteractuar()
    {
        Collider2D[] objetos = Physics2D.OverlapBoxAll(controlador.position, dimensiones, 0f, capaInteractuable);
        foreach (Collider2D objeto in objetos)
        {
            if (objeto.TryGetComponent(out Item item))
            {
                item.Interactuar(_inventario);
            }
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(controlador.position, dimensiones);
    }
}

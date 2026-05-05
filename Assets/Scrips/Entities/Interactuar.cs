using UnityEngine;
using UnityEngine.InputSystem;
public class Interactuar : MonoBehaviour
{
    [SerializeField] private Transform controlador;
    [SerializeField] private Vector2 dimensiones;
    [SerializeField] private LayerMask capaInteractuable;
    [SerializeField] private InputActionReference interactuar;


    public void Update()
    {
        if (Input.GetButtonDown("Interactuar"))
        {
            EInteractuar();
        }
    }


    private void EInteractuar()
    {
        Collider2D[] objetos = Physics2D.OverlapBoxAll(controlador.position, dimensiones, 0f, capaInteractuable);
        foreach (Collider2D objeto in objetos)
        {
            if (objeto.TryGetComponent(out Item item))
            {
                item.Interactuar();
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(controlador.position, dimensiones);
    }
}

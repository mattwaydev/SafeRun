using UnityEngine;

public class Interactuar : MonoBehaviour
{
    [SerializeField] private Transform controlador;
    [SerializeField] private Vector2 dimensiones;

    public void Update()
    {
        if (Input.GetButtonDown("Interactuar"))
        {
            Interactuar();
        }
    }


    private void Interactuar()
    {
        Collider2D[] objetos = Physics2D.OverlapBoxAll(controlador.position, dimensiones, 0f);
        foreach (Collider2D objeto in objetos)
        {
            if (Item1.TryGetComponent(out Item item))
            {
                Item1.Interactuar();
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(controlador.position, dimensiones);
    }
}

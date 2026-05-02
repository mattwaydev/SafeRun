using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    [SerializeField] private string Escena;

    public void CambioEscena()
    {
        SceneManager.LoadScene(Escena);
    }
}

using UnityEngine;

public class CambioPorColider : MonoBehaviour
{
    [SerializeField] private GameObject ObjetoDestruir;
    [SerializeField] private GameObject ObjetoActivar;
    public string tagPlayer = "Player";
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag(tagPlayer)) return;
        if (ObjetoDestruir != null)
        {
            Destroy(ObjetoDestruir);
        }
        if (ObjetoActivar != null)
        {
            ObjetoActivar.SetActive(true);
        }

        gameObject.SetActive(false);
        
        //me pashe me pashe
    }

}

using UnityEngine;
using UnityEngine.UI;
using SafeRun.Entities;
public class BarraDeVida : MonoBehaviour
{
    [SerializeField] private Image barraDeVida;
    [SerializeField] private Jugador jugador;
    private void Update()
    {
        if (jugador == null || barraDeVida == null) return;
        barraDeVida.fillAmount = (jugador.VidaActual / 100f);
    }
}

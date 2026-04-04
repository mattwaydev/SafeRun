using UnityEngine;
using UnityEngine.UI;
using SafeRun.Entities;

public class BarraDeVidaUI : MonoBehaviour
{
   [SerializeField] private Slider SliderBarra;
   [SerializeField] private Jugador jugador;

    private void Update()
    {
        if (jugador == null || SliderBarra == null) return;
        SliderBarra.value = jugador.VidaActual / 100f;
    }
}

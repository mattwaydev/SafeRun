using UnityEngine;
using UnityEngine.UI;
using SafeRun.Entities;
public class BarraDeVida : MonoBehaviour
{
    [SerializeField] private Image barraDeVida;
    [SerializeField] private Jugador jugador;

    [Header("Ajuste UI")]
    [SerializeField] private RectTransform barraRect;
    [SerializeField] private bool forzarEscalaUno = true;
    [SerializeField] private bool ajustarAnclaje = true;
    [SerializeField] private Vector2 anclaMin = new Vector2(0f, 1f);
    [SerializeField] private Vector2 anclaMax = new Vector2(0f, 1f);
    [SerializeField] private Vector2 pivote = new Vector2(0f, 1f);
    [SerializeField] private Vector2 posicion = new Vector2(16f, -16f);

    private void Awake()
    {
        if (barraRect == null)
            barraRect = GetComponent<RectTransform>();

        if (barraRect == null) return;

        if (forzarEscalaUno)
            barraRect.localScale = Vector3.one;

        if (ajustarAnclaje)
        {
            barraRect.anchorMin = anclaMin;
            barraRect.anchorMax = anclaMax;
            barraRect.pivot = pivote;
            barraRect.anchoredPosition = posicion;
        }
    }
    private void OnEnable()
    {
        if (jugador != null)
            jugador.VidaCambiada += ActualizarBarra;
    }

    private void OnDisable()
    {
        if (jugador != null)
            jugador.VidaCambiada -= ActualizarBarra;
    }

    private void Start()
    {
        if (jugador != null)
            ActualizarBarra(jugador.VidaActual, jugador.VidaMaxima);
    }

    private void ActualizarBarra(float vidaActual, float vidaMaxima)
    {
        if (barraDeVida == null) return;
        if (vidaMaxima <= 0f) return;
        barraDeVida.fillAmount = vidaActual / vidaMaxima;
    }
}

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CodigoDaltonismo : MonoBehaviour
{
    public Toggle toggle;
    public Volume volumen;

    void Start()
    {
        bool activo = PlayerPrefs.GetInt("daltonismo", 0) == 1;

        if (volumen != null)
            volumen.weight = activo ? 1f : 0f;

        if (toggle != null)
            toggle.isOn = activo;
    }

    public void ActivarDaltonismo(bool activo)
    {
        if (volumen != null)
            volumen.weight = activo ? 1f : 0f;

        PlayerPrefs.SetInt("daltonismo", activo ? 1 : 0);
    }
}

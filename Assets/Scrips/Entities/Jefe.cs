// Jefe.cs — Enemigo con fases (TAD 5)
// POO: herencia de Enemigo, override de comportamiento
using UnityEngine;

namespace SafeRun.Entities
{
    public class Jefe : Enemigo
    {
        [SerializeField] private int fases = 3;
        private int _faseActual = 1;
        private float _umbralCambio;

        protected override void Start()
        {
            base.Start();
            _umbralCambio = vidaMaxima / fases;
        }

        public override void RecibirDanio(float cantidad)
        {
            base.RecibirDanio(cantidad);
            int faseNueva = Mathf.CeilToInt(_vidaActual / _umbralCambio);
            if (faseNueva < _faseActual) CambiarFase(faseNueva);
        }

        private void CambiarFase(int nueva)
        {
            _faseActual = nueva;
            agresividad *= 1.5f;
            Debug.Log($"[SafeRun] Jefe cambia a fase {_faseActual} — agresividad: {agresividad}");
        }

        public override void LanzarMensaje()
        {
            Debug.Log($"[SafeRun] Jefe (fase {_faseActual}) ataca con patron especial");
        }

        public int FaseActual => _faseActual;
    }
}

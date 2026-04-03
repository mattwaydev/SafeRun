// Jugador.cs — Controlado por el usuario (TAD 3)
// POO: herencia de Personaje -> Entidad
using UnityEngine;
using SafeRun.Core;

namespace SafeRun.Entities
{
    public class Jugador : Personaje
    {
        [SerializeField] private float empatia = 100f;
        [SerializeField] private string skinActual = "default";

        [SerializeField] private GestorJuego gestorJuego;

        protected override void Start()
        {
            base.Start();
            if (gestorJuego == null)
                gestorJuego = FindAnyObjectByType<GestorJuego>();
        }

        private void FixedUpdate()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            if (h != 0 || v != 0)
                Mover(new Vector2(h, v).normalized);
            else
                Mover(Vector2.zero);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) Atacar();
            if (Input.GetKeyDown(KeyCode.R)) Reportar();
        }

        public override void Atacar()
        {
            Debug.Log("[SafeRun] Jugador lanza respuesta positiva");
        }

        // Habilidad especial del jugador
        public void Reportar()
        {
            Debug.Log("[SafeRun] Jugador reporta al acosador");
            gestorJuego?.AgregarPuntos(50);
        }

        public void CambiarSkin(string nuevaSkin) => skinActual = nuevaSkin;
        public float Empatia => empatia;

        private void OnCollisionStay2D(Collision2D col)
        {
            if (col.gameObject.GetComponent<Enemigo>() != null)
            {
                RecibirDanio(10f * Time.deltaTime);
            }
        }
    }
}

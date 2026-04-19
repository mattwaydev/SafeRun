using UnityEngine;
using UnityEngine.InputSystem;
using SafeRun.Core;


namespace SafeRun.Entities
{
    public class Jugador : Personaje
    {

        [SerializeField] private float empatia = 100f;
        [SerializeField] private string skinActual = "default";
        [SerializeField] private GestorJuego gestorJuego;

        [SerializeField] private JugadorInputs _inputs;
        private Vector2 _movimiento;

        protected override void Start()
        {
            base.Start();

            if (gestorJuego == null)
                gestorJuego = FindAnyObjectByType<GestorJuego>();

            // Inicializa el nuevo input system
            _inputs = new JugadorInputs();
            _inputs.Gameplay.Movimiento.performed += ctx => _movimiento = ctx.ReadValue<Vector2>();
            _inputs.Gameplay.Movimiento.canceled += ctx => _movimiento = Vector2.zero;
            _inputs.Enable();
        }

        private void OnDestroy()
        {
            _inputs.Disable();
        }

        private void FixedUpdate()
        {
            if (_movimiento != Vector2.zero)
            {
                Mover(_movimiento.normalized);
                _animator.SetBool("isWalking", true);
                // Si tienes animaciones por dirección:
                _animator.SetFloat("moveX", _movimiento.x);
                _animator.SetFloat("moveY", _movimiento.y);
            }
            else
            {
                Mover(Vector2.zero);
                _animator.SetBool("isWalking", false);
            }
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Space)) Atacar();
            //if (Input.GetKeyDown(KeyCode.R)) Reportar();
            //cambiar a nuevo input system en el futuro para estas acciones
        }

        public override void Atacar()
        {
            Debug.Log("[SafeRun] Jugador lanza respuesta positiva");
        }

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
                RecibirDanio(10f * Time.deltaTime);
        }
    }
}
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


        //dash----------------------
        [Header("Dash")]
        [SerializeField] private float dashSpeed = 18f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashCooldown = 3f;

        private bool _isDashing = false;
        private float _dashTimeLeft = 0f;
        private float _dashTimer = 0f;
        private float _dashCooldownTimer = 0f;
        private Vector2 _dashDirection;
        //--------------------------------

        //combate----------------------
        [Header("Combate")]
        [SerializeField] private ProyectilPapel proyectilPapelPrefab;
        [SerializeField] private Transform puntoDisparo;
        [SerializeField] private float danioAtaque = 25f;
        [SerializeField] private float cooldownAtaque = 0.8f;
        private float _cooldownTimerAtaque;

        [Header("Espejo de las Emociones")]
        [SerializeField] private EspejoEmociones espejoEmocionesPrefab;
        [SerializeField] private float radioEspejo = 5f;
        [SerializeField] private float danioEspejoPorSegundo = 15f;
        [SerializeField] private float cooldownEspejo = 15f;
        private float _cooldownTimerEspejo;
        //--------------------------------

        protected override void Start()
        {
            base.Start();

            if (gestorJuego == null)
                Debug.LogWarning("[SafeRun] GestorJuego no asignado en Jugador. Asignalo en el inspector.");

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
            if(_estaMuriendo) return;

            if(_isDashing)
            {
                _rb.linearVelocity = _dashDirection * dashSpeed;
            }
            else if(_movimiento != Vector2.zero)
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
            }
        }

        private void Update()
        {
            if(_estaMuriendo) return;

            if(_dashCooldownTimer > 0f)
                _dashCooldownTimer -= Time.deltaTime;

            if(_cooldownTimerAtaque > 0f)
                _cooldownTimerAtaque -= Time.deltaTime;

            if(_cooldownTimerEspejo > 0f)
                _cooldownTimerEspejo -= Time.deltaTime;
            
            if(_isDashing)
            {
                _dashTimer -= Time.deltaTime;
                if(_dashTimer <= 0f)
                {
                    _isDashing = false;
                    _animator.SetBool("isDashing", false);
                }
            }

            if(!_isDashing)
            {
                bool caminando = _movimiento != Vector2.zero;
                _animator.SetBool("isWalking", caminando);

                if (caminando)
                {
                    _animator.SetFloat("moveX", _movimiento.x);
                    _animator.SetFloat("moveY", _movimiento.y);
                }
            }

            if (Keyboard.current.leftShiftKey.wasPressedThisFrame && !_isDashing && _dashCooldownTimer <= 0f)
            {
                IniciarDash();
            }

            if (Keyboard.current.spaceKey.wasPressedThisFrame && _cooldownTimerAtaque <= 0f)
            {
                Atacar();
            }

            if (Keyboard.current.eKey.wasPressedThisFrame && _cooldownTimerEspejo <= 0f)
            {
                ActivarEspejo();
            }
        }


        private void IniciarDash()
        {
            _isDashing = true;
            _dashTimer = dashDuration;
            _dashCooldownTimer = dashCooldown;

            _dashDirection = _movimiento.normalized;
            if(_dashDirection == Vector2.zero)
                _dashDirection = new Vector2(_animator.GetFloat("moveX"), _animator.GetFloat("moveY")).normalized;
            
            _animator.SetBool("isDashing", true);
        }

        public override void Atacar()
        {
            if (proyectilPapelPrefab == null)
            {
                Debug.Log("[SafeRun] Jugador lanza respuesta positiva");
                _cooldownTimerAtaque = cooldownAtaque;
                return;
            }

            Vector2 direccion = _movimiento != Vector2.zero ? _movimiento : _ultimaDireccion;
            Transform origen = puntoDisparo != null ? puntoDisparo : transform;

            ProyectilPapel proyectil = Instantiate(proyectilPapelPrefab, origen.position, Quaternion.identity);
            proyectil.Configurar(direccion, danioAtaque);

            _cooldownTimerAtaque = cooldownAtaque;
            gestorJuego?.SolicitarSubtitulo("Respuesta empatica lanzada");
        }

        private void ActivarEspejo()
        {
            if (espejoEmocionesPrefab == null)
            {
                Debug.Log("[SafeRun] Espejo de las Emociones no configurado");
                return;
            }

            EspejoEmociones espejo = Instantiate(espejoEmocionesPrefab, transform.position, Quaternion.identity);
            _cooldownTimerEspejo = cooldownEspejo;

            gestorJuego?.SolicitarSubtitulo("Espejo de las Emociones activado — los agresores se enfrentan a si mismos");
            gestorJuego?.SolicitarSonido("espejo_emociones");
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

        public void OnMuerteTerminada()
        {
            Debug.Log("[SafeRun] Jugador ha muerto.");
            Destroy(gameObject);
        }
    }
}

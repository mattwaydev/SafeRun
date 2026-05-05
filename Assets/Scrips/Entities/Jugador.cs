using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using SafeRun.Core;
using SafeRun.Structures;


namespace SafeRun.Entities
{
    public class Jugador : Personaje
    {
        private static Jugador _instancia;

        [SerializeField] private float empatia = 100f;
        [SerializeField] private string skinActual = "default";
        [SerializeField] private GestorJuego gestorJuego;
        [SerializeField] private InventarioArmas inventario;

        public InventarioArmas Inventario => inventario;

        [SerializeField] private JugadorInputs _inputs;
        private Vector2 _movimiento;


        //dash----------------------
        [Header("Dash")]
        [SerializeField] private float dashSpeed = 18f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashCooldown = 3f;

        private bool _isDashing = false;
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

            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = 100;

            var rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            }

            var collider = GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.size = new Vector2(0.11f, 0.18f);
                collider.offset = new Vector2(0f, -0.01f);

                var material = new PhysicsMaterial2D("JugadorSinFriccion")
                {
                    friction = 0f,
                    bounciness = 0f
                };
                collider.sharedMaterial = material;
            }

            if (gestorJuego == null)
                Debug.LogWarning("[SafeRun] GestorJuego no asignado en Jugador. Asignalo en el inspector.");

            if (inventario == null)
                inventario = new InventarioArmas();

            // Inicializa el nuevo input system
            _inputs = new JugadorInputs();
            _inputs.Gameplay.Movimiento.performed += ctx => _movimiento = ctx.ReadValue<Vector2>();
            _inputs.Gameplay.Movimiento.canceled += ctx => _movimiento = Vector2.zero;
            _inputs.Enable();

        }

        private void Awake()
        {
            if (_instancia != null && _instancia != this)
            {
                Destroy(gameObject);
                return;
            }

            _instancia = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ReubicarEnEscena(scene.name);
        }

        private void ReubicarEnEscena(string nombreEscena)
        {
            Transform spawn = null;

            var spawnObj = GameObject.Find("PlayerSpawn");
            if (spawnObj != null)
                spawn = spawnObj.transform;

            if (spawn == null)
                spawnObj = GameObject.Find("JugadorSpawn");

            if (spawnObj != null)
                spawn = spawnObj.transform;

            if (spawn == null)
            {
                if (nombreEscena == "School Main")
                    spawn = null;
                else if (nombreEscena == "School Bathroom")
                    spawn = null;
                else if (nombreEscena == "School Salon 1")
                    spawn = null;
            }

            Vector3 destino;
            if (spawn != null)
                destino = spawn.position;
            else if (nombreEscena == "School Main")
                destino = new Vector3(0.06f, -2.04f, transform.position.z);
            else if (nombreEscena == "School Bathroom")
                destino = new Vector3(0f, 0f, transform.position.z);
            else if (nombreEscena == "School Salon 1")
                destino = new Vector3(2.36f, -1.27f, transform.position.z);
            else
                return;

            var rb = GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            transform.position = destino;
        }

        private void OnDestroy()
        {
            if (_instancia == this)
                _instancia = null;

            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (_inputs != null)
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
            espejo.Configurar(radioEspejo, danioEspejoPorSegundo);
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

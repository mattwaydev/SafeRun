using UnityEngine;
using SafeRun.Entities;

namespace SafeRun.Entities
{
    public abstract class Personaje : Entidad
    {
        [SerializeField] protected float velocidad = 5f;
        [SerializeField] protected float escudo = 0f;

        protected Rigidbody2D _rb;
        protected Animator _animator;

        // Guarda la última dirección para el Idle
        protected Vector2 _ultimaDireccion = Vector2.down;
        protected bool _estaMuriendo = false;

        protected override void Start()
        {
            base.Start();
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        public override void Mover(Vector2 direccion)
        {
            if (_rb != null)
                _rb.linearVelocity = direccion * velocidad;
            else
                transform.Translate(direccion * velocidad * Time.deltaTime);

            // Actualizar animación
            if (_animator != null)
            {
                bool isWalking = direccion.magnitude > 0.1f;
                if (HasParameter("isWalking"))
                    _animator.SetBool("isWalking", isWalking);
                else if (HasParameter("IsWalking"))
                    _animator.SetBool("IsWalking", isWalking);

                if (isWalking)
                {
                    // Guardar última dirección para el Idle
                    _ultimaDireccion = direccion;
                    if (HasParameter("moveX"))
                        _animator.SetFloat("moveX", direccion.x);
                    else if (HasParameter("MoveX"))
                        _animator.SetFloat("MoveX", direccion.x);

                    if (HasParameter("moveY"))
                        _animator.SetFloat("moveY", direccion.y);
                    else if (HasParameter("MoveY"))
                        _animator.SetFloat("MoveY", direccion.y);
                }
                else
                {
                    // Idle apunta a la última dirección que caminó
                    if (HasParameter("moveX"))
                        _animator.SetFloat("moveX", _ultimaDireccion.x);
                    else if (HasParameter("MoveX"))
                        _animator.SetFloat("MoveX", _ultimaDireccion.x);

                    if (HasParameter("moveY"))
                        _animator.SetFloat("moveY", _ultimaDireccion.y);
                    else if (HasParameter("MoveY"))
                        _animator.SetFloat("MoveY", _ultimaDireccion.y);
                }
            }
        }

        public override void RecibirDanio(float cantidad)
        {
            if (_estaMuriendo) return;

            float danioReal = Mathf.Max(cantidad - escudo, 0);
            _vidaActual = Mathf.Max(_vidaActual - danioReal, 0f);
            NotificarVida();

            if(_vidaActual <= 0)
                Morir();

        }

        public abstract void Atacar();

        protected override void Morir()
        {
            if (_estaMuriendo) return;
            _estaMuriendo = true;   

            if (_rb != null)
            {
                _rb.linearVelocity = Vector2.zero;
                _rb.simulated = false;
            }

            if (_animator != null)
            {
                if (HasParameter("moveX"))
                    _animator.SetFloat("moveX", _ultimaDireccion.x);
                else if (HasParameter("MoveX"))
                    _animator.SetFloat("MoveX", _ultimaDireccion.x);

                if (HasParameter("moveY"))
                    _animator.SetFloat("moveY", _ultimaDireccion.y);
                else if (HasParameter("MoveY"))
                    _animator.SetFloat("MoveY", _ultimaDireccion.y);

                if (HasParameter("isDead"))
                    _animator.SetBool("isDead", true);
                else if (HasParameter("IsDead"))
                    _animator.SetBool("IsDead", true);
            }

            Destroy(gameObject, 1f);
        }

        protected bool HasParameter(string name)
        {
            if (_animator == null) return false;
            var parameters = _animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].name == name) return true;
            }
            return false;
        }
    }
}

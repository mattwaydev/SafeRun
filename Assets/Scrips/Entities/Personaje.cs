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
        private Vector2 _ultimaDireccion = Vector2.down;

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
                _animator.SetBool("isWalking", isWalking);

                if (isWalking)
                {
                    // Guardar última dirección para el Idle
                    _ultimaDireccion = direccion;
                    _animator.SetFloat("moveX", direccion.x);
                    _animator.SetFloat("moveY", direccion.y);
                }
                else
                {
                    // Idle apunta a la última dirección que caminó
                    _animator.SetFloat("moveX", _ultimaDireccion.x);
                    _animator.SetFloat("moveY", _ultimaDireccion.y);
                }
            }
        }

        public override void RecibirDanio(float cantidad)
        {
            float danioReal = Mathf.Max(0, cantidad - escudo);
            base.RecibirDanio(danioReal);
        }

        public abstract void Atacar();
    }
}
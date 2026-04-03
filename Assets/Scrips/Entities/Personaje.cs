// Personaje.cs — Hereda de Entidad (TAD 2)
// POO: herencia, encapsulamiento
using UnityEngine;
using SafeRun.Entities;

namespace SafeRun.Entities
{
    public abstract class Personaje : Entidad
    {
        [SerializeField] protected float velocidad = 5f;
        [SerializeField] protected float escudo = 0f;

        protected Rigidbody2D _rb;

        protected override void Start()
        {
            base.Start();
            _rb = GetComponent<Rigidbody2D>();
        }

        public override void Mover(Vector2 direccion)
        {
            if (_rb != null)
                _rb.linearVelocity = direccion * velocidad;
            else
                transform.Translate(direccion * velocidad * Time.deltaTime);
        }

        public override void RecibirDanio(float cantidad)
        {
            float danioReal = Mathf.Max(0, cantidad - escudo);
            base.RecibirDanio(danioReal);
        }

        public abstract void Atacar();
    }
}

// Entidad.cs — Clase base abstracta (TAD 1)
// POO: herencia, abstraccion, polimorfismo
using System;
using UnityEngine;

namespace SafeRun.Entities
{
    public abstract class Entidad : MonoBehaviour
    {
        [SerializeField] protected string nombre;
        [SerializeField] protected float vidaMaxima = 100f;
        protected float _vidaActual;
        protected Vector2 _posicion;

        public event Action<float, float> VidaCambiada;

        protected virtual void Start()
        {
            _vidaActual = vidaMaxima;
            NotificarVida();
        }

        public abstract void Mover(Vector2 direccion);

        public virtual void RecibirDanio(float cantidad)
        {
            _vidaActual = Mathf.Max(_vidaActual - cantidad, 0f);
            NotificarVida();
            if (_vidaActual <= 0) Morir();
        }

        protected virtual void Morir()
        {
            Debug.Log($"[SafeRun] {nombre} fue derrotado.");
            Destroy(gameObject);
        }

        protected void NotificarVida() => VidaCambiada?.Invoke(_vidaActual, vidaMaxima);

        public bool EstaVivo => _vidaActual > 0;
        public float VidaActual => _vidaActual;
        public float VidaMaxima => vidaMaxima;
        public string Nombre => nombre;
    }
}

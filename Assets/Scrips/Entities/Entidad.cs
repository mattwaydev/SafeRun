// Entidad.cs — Clase base abstracta (TAD 1)
// POO: herencia, abstraccion, polimorfismo
using UnityEngine;

namespace SafeRun.Entities
{
    public abstract class Entidad : MonoBehaviour
    {
        [SerializeField] protected string nombre;
        [SerializeField] protected float vidaMaxima = 100f;
        protected float _vidaActual;
        protected Vector2 _posicion;

        protected virtual void Start() => _vidaActual = vidaMaxima;

        public abstract void Mover(Vector2 direccion);

        public virtual void RecibirDanio(float cantidad)
        {
            _vidaActual -= cantidad;
            if (_vidaActual <= 0) Morir();
        }

        protected virtual void Morir()
        {
            Debug.Log($"[SafeRun] {nombre} fue derrotado.");
            Destroy(gameObject);
        }

        public bool EstaVivo => _vidaActual > 0;
        public float VidaActual => _vidaActual;
        public string Nombre => nombre;
    }
}

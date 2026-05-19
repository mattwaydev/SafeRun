using UnityEngine;

namespace SafeRun.Entities
{
    public class EspejoEmociones : MonoBehaviour
    {
        [SerializeField] private float radio = 5f;
        [SerializeField] private float duracion = 5f;
        [SerializeField] private float danioPorSegundo = 15f;

        [Header("Efecto visual")]
        [SerializeField] private Color colorOnda = new Color(0.55f, 0.85f, 1f, 1f);
        [SerializeField] private int segmentosCirculo = 64;
        [SerializeField] private float anchoLinea = 0.15f;
        [SerializeField] private float tiempoExpansion = 0.35f;

        private LineRenderer _onda;
        private LineRenderer _anillo;
        private float _tiempoVida;

        private void Awake()
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = 120;

            _onda = CrearAnillo("OndaExpansiva", anchoLinea);
            _anillo = CrearAnillo("AnilloPulsante", anchoLinea * 0.55f);
        }

        public void Configurar(float radioPersonalizado, float danioPersonalizado)
        {
            if (radioPersonalizado > 0f)
                radio = radioPersonalizado;

            if (danioPersonalizado > 0f)
                danioPorSegundo = danioPersonalizado;
        }

        private void Start()
        {
            Collider2D[] resultados = Physics2D.OverlapCircleAll(transform.position, radio);
            int cantidad = resultados.Length;
            for (int i = 0; i < cantidad; i++)
            {
                Collider2D col = resultados[i];
                if (col == null) continue;

                Enemigo enemigo = col.GetComponentInParent<Enemigo>();
                if (enemigo != null && enemigo.EstaVivo)
                {
                    enemigo.ActivarConfusion(duracion, danioPorSegundo);
                }
            }

            Destroy(gameObject, duracion + 0.5f);
        }

        private void Update()
        {
            _tiempoVida += Time.deltaTime;
            ActualizarOnda();
            ActualizarAnillo();
        }

        private void ActualizarOnda()
        {
            if (_onda == null) return;

            float t = Mathf.Clamp01(_tiempoVida / Mathf.Max(0.01f, tiempoExpansion));
            float r = Mathf.SmoothStep(0f, radio, t);
            DibujarCirculo(_onda, r);

            Color c = colorOnda;
            c.a = (1f - t) * 0.9f;
            _onda.startColor = c;
            _onda.endColor = c;
        }

        private void ActualizarAnillo()
        {
            if (_anillo == null) return;

            float vida = Mathf.Clamp01(_tiempoVida / Mathf.Max(0.01f, duracion));
            float pulso = 0.5f + 0.5f * Mathf.Sin(_tiempoVida * 6f);
            float r = radio * (0.9f + 0.06f * pulso);
            DibujarCirculo(_anillo, r);

            Color c = colorOnda;
            c.a = (1f - vida) * (0.35f + 0.35f * pulso);
            _anillo.startColor = c;
            _anillo.endColor = c;
        }

        private LineRenderer CrearAnillo(string nombre, float ancho)
        {
            var hijo = new GameObject(nombre);
            hijo.transform.SetParent(transform, false);

            var lr = hijo.AddComponent<LineRenderer>();
            lr.useWorldSpace = false;
            lr.loop = true;
            lr.positionCount = segmentosCirculo;
            lr.startWidth = ancho;
            lr.endWidth = ancho;
            lr.numCapVertices = 2;
            lr.numCornerVertices = 2;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.sortingOrder = 120;
            lr.startColor = colorOnda;
            lr.endColor = colorOnda;
            return lr;
        }

        private void DibujarCirculo(LineRenderer lr, float r)
        {
            int n = lr.positionCount;
            float paso = Mathf.PI * 2f / n;
            for (int i = 0; i < n; i++)
            {
                float a = i * paso;
                lr.SetPosition(i, new Vector3(Mathf.Cos(a) * r, Mathf.Sin(a) * r, 0f));
            }
        }
    }
}

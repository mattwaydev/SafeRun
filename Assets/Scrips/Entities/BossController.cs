using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SafeRun.Entities
{
    public class BossController : Enemigo
    {
        //fases del boss hola hola hola
        [Header("Fases")]
        [SerializeField] private float vidaFase1 = 150f;
        [SerializeField] private float vidaFase2 = 150f;
        private int _faseActual = 1;
        private bool _transicionando = false;

        //Cooldowns fase 1
        [Header("Cooldowns Fase 1")]
        [SerializeField] private float cooldownLluvia = 6f;
        [SerializeField] private float cooldownCirculo = 8f;
        [SerializeField] private float cooldownSpawn = 10f;

        [Header("Cooldowns Fase 2")]
        [SerializeField] private float cooldownLluviaF2 = 3.5f;
        [SerializeField] private float cooldownCirculoF2 = 5f;
        [SerializeField] private float cooldownSpawnF2 = 7f;
        private float _timerLluvia;
        private float _timerCirculo;
        private float _timerSpawn;

        //Datos de lluvia de mensajes
        [Header("Lluvia De Mensajes")]
        [SerializeField] private int cantidadLluvia = 8;
        [SerializeField] private float intervaloLluvia = 0.15f;
        [SerializeField] private float danioLluvia = 10f;
        [SerializeField] private float danioLluviaF2 = 18f;
        [SerializeField] private float alturaSpawnLluvia = 6f;
        [SerializeField] private float radioDispersionLluvia = 5f;

        //Círculo de mensajes datos
        [Header("Circulo De Mensajes")]
        [SerializeField] private int cantidadCirculo = 10;
        [SerializeField] private float radioCirculo = 1.2f;
        [SerializeField] private float pausaCirculo = 1f;
        [SerializeField] private float danioCirculo = 12f;
        [SerializeField] private float danioCirculoF2 = 20f;



        [Header("Spawn de Enemigos")]
        [SerializeField] private Enemigo[] prefabsEnemigos;
        [SerializeField] private Transform[] puntosSpawn;
        [SerializeField] private int minEnemigosF1 = 1;
        [SerializeField] private int maxEnemigosF1 = 3;
        [SerializeField] private int cantidadEnemigosF2 = 3;


        [Header("UI Boss")]
        [SerializeField] private UnityEngine.UI.Image barraVidaF1;
        [SerializeField] private UnityEngine.UI.Image barraVidaF2;
        [SerializeField] private GameObject bossHealthUIPrefab;
        [SerializeField] private string nombreBarraF1 = "BossHealthBarF1";
        [SerializeField] private string nombreBarraF2 = "BossHealthBarF2";
        private GameObject _uiInstanciada;
        [Header("Transicion Fase 2")]
        [SerializeField] private float duracionTransicion = 1.2f;
        [SerializeField] private float multiplicadorAgresividadF2 = 1.4f;

        protected override void Start()
        {
            vidaMaxima = vidaFase1;
            base.Start();
            InstanciarUIBoss();
            DespegarPuntosSpawn();
            _timerLluvia = cooldownLluvia * 0.3f;
            _timerCirculo = cooldownCirculo * 0.6f;
            _timerSpawn = cooldownSpawn;
            if (barraVidaF1 != null)
            {
                barraVidaF1.fillAmount = 1f;
            }
            if (barraVidaF2 != null)
            {
                barraVidaF2.gameObject.SetActive(false);
            }
            VidaCambiada += ActualizarBarraUI;
        }


        private void DespegarPuntosSpawn()
        {
            if (puntosSpawn == null)
            {
                return;
            }
            for (int i = 0; i < puntosSpawn.Length; i++)
            {
                if (puntosSpawn[i] != null)
                {
                    puntosSpawn[i].SetParent(null, true);
                }
            }
        }


        private void InstanciarUIBoss()
        {
            if (barraVidaF1 != null && barraVidaF2 != null)
            {
                return;
            }
            if (bossHealthUIPrefab == null)
            {
                Debug.LogWarning("[SafeRun] BossController: ni las barras ni el prefab de UI estan asignados.");
                return;
            }
            Canvas canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("[SafeRun] BossController: no hay Canvas en la escena para instanciar la UI del Boss.");
                return;
            }
            GameObject uiInstance = Instantiate(bossHealthUIPrefab, canvas.transform, false);
            _uiInstanciada = uiInstance;
            UnityEngine.UI.Image[] images = uiInstance.GetComponentsInChildren<UnityEngine.UI.Image>(true);
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i].gameObject.name == nombreBarraF1)
                {
                    barraVidaF1 = images[i];
                }
                else if (images[i].gameObject.name == nombreBarraF2)
                {
                    barraVidaF2 = images[i];
                }
            }

            if (barraVidaF1 == null || barraVidaF2 == null)
            {
                Debug.LogWarning("[SafeRun] BossController: no se encontraron las barras dentro del prefab de UI. Revisa los nombres.");
            }
        }
       
       protected override void Update()
        {
            if (_transicionando)
            {
                return;
            }
            if (!EstaVivo)
            {
                return;
            }
            base.Update();
            if (_objetivoIA == null)
            {
                return;
            }
            _timerLluvia -= Time.deltaTime;
            _timerCirculo -= Time.deltaTime;
            _timerSpawn -= Time.deltaTime;

            if (_timerLluvia <= 0f)
            {
                StartCoroutine(AtaqueLluvia());
                if (_faseActual == 1)
                {
                    _timerLluvia = cooldownLluvia;
                }
                else
                {
                    _timerLluvia = cooldownLluviaF2;
                }
            }

            if (_timerCirculo <= 0f)
            {
                StartCoroutine(AtaqueCirculo());
                if (_faseActual == 1)
                {
                    _timerCirculo = cooldownCirculo;
                }
                else
                {
                    _timerCirculo = cooldownCirculoF2;
                }
            }

            if (_timerSpawn <= 0f)
            {
                SpawnearEnemigos();
                if (_faseActual == 1)
                {
                    _timerSpawn = cooldownSpawn;
                }
                else
                {
                    _timerSpawn = cooldownSpawnF2;
                }
            }
        }


        public override void RecibirDanio(float cantidad)
        {
            if (_transicionando)
            {
                return;
            }
            base.RecibirDanio(cantidad);
            if (_vidaActual <= 0f && _faseActual == 1)
            {
                StartCoroutine(TransicionarFase2());
            }
        }


        protected override void Morir()
        {
            if (_faseActual == 1)
            {
                return;
            }
            base.Morir();
        }


        private IEnumerator TransicionarFase2()
        {
            _transicionando = true;
            _faseActual = 2;
            if (_rb != null)
            {
                _rb.linearVelocity = Vector2.zero;
            }

            //Meter animación o algo por acá
            yield return new WaitForSeconds(duracionTransicion);
            _vidaActual = vidaFase2;
            vidaMaxima = vidaFase2;
            NotificarVida();
            if (barraVidaF1 != null)
            {
                barraVidaF1.gameObject.SetActive(false);
            }
            if (barraVidaF2 != null)
            {
                barraVidaF2.gameObject.SetActive(true);
                barraVidaF2.fillAmount = 1f;
            }
            agresividad *= multiplicadorAgresividadF2;
            _transicionando = false;
            Debug.Log("[SafeRun] Boss entra en FASE 2");
        }


        private IEnumerator AtaqueLluvia()
        {
            if (balaPrefab == null)
            {
                yield break;
            }
            float danio;
            if (_faseActual == 1)
            {
                danio = danioLluvia;
            }
            else
            {
                danio = danioLluviaF2;
            }
            for (int i = 0; i < cantidadLluvia; i++)
            {
                float offsetX = Random.Range(-radioDispersionLluvia, radioDispersionLluvia);
                Vector2 spawnPos = (Vector2)transform.position + new Vector2(offsetX, alturaSpawnLluvia);

                BalaEnemigo bala = Instantiate(balaPrefab, spawnPos, Quaternion.identity);
                bala.Configurar(Vector2.down, danio, gameObject);

                yield return new WaitForSeconds(intervaloLluvia);
            }
        }


        private IEnumerator AtaqueCirculo()
        {
            if (balaPrefab == null)
            {
                yield break;
            }
            float danio;
            if (_faseActual == 1)
            {
                danio = danioCirculo;
            }
            else
            {
                danio = danioCirculoF2;
            }
            List<BalaEnemigo> balas = new List<BalaEnemigo>();
            List<Vector2> direcciones = new List<Vector2>();
            for (int i = 0; i < cantidadCirculo; i++)
            {
                float angulo = (360f / cantidadCirculo) * i;
                float rad = angulo * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radioCirculo;
                Vector2 spawnPos = (Vector2)transform.position + offset;
                BalaEnemigo bala = Instantiate(balaPrefab, spawnPos, Quaternion.identity);
                balas.Add(bala);
                direcciones.Add(offset.normalized);
            }
            yield return new WaitForSeconds(pausaCirculo);
            for (int i = 0; i < balas.Count; i++)
            {
                if (balas[i] != null)
                {
                    balas[i].Configurar(direcciones[i], danio, gameObject);
                }
            }
        }


        private void SpawnearEnemigos()
        {
            if (prefabsEnemigos == null || prefabsEnemigos.Length == 0)
            {
                return;
            }
            if (puntosSpawn == null || puntosSpawn.Length == 0)
            {
                return;
            }
            int cantidad;
            if (_faseActual == 1)
            {
                cantidad = Random.Range(minEnemigosF1, maxEnemigosF1 + 1);
            }
            else
            {
                cantidad = cantidadEnemigosF2;
            }
            List<Transform> disponibles = new List<Transform>(puntosSpawn);

            for (int i = 0; i < cantidad; i++)
            {
                if (disponibles.Count == 0)
                {
                    break;
                }

                int idxSpawn = Random.Range(0, disponibles.Count);
                int idxPrefab = Random.Range(0, prefabsEnemigos.Length);

                if (prefabsEnemigos[idxPrefab] == null)
                {
                    disponibles.RemoveAt(idxSpawn);
                    continue;
                }

                Enemigo enemigo = Instantiate(
                    prefabsEnemigos[idxPrefab],
                    disponibles[idxSpawn].position,
                    Quaternion.identity
                );

                if (_objetivoIA != null)
                {
                    enemigo.SetObjetivo(_objetivoIA);
                }

                disponibles.RemoveAt(idxSpawn);
            }
        }


        private void ActualizarBarraUI(float vidaActual, float vidaMax)
        {
            float fill;
            if (vidaMax > 0f)
            {
                fill = vidaActual / vidaMax;
            }
            else
            {
                fill = 0f;
            }
            if (_faseActual == 1 && barraVidaF1 != null)
            {
                barraVidaF1.fillAmount = fill;
            }
            else if (_faseActual == 2 && barraVidaF2 != null)
            {
                barraVidaF2.fillAmount = fill;
            }
        }


        private void OnDestroy()
        {
            VidaCambiada -= ActualizarBarraUI;
            if (_uiInstanciada != null)
            {
                Destroy(_uiInstanciada);
                _uiInstanciada = null;
            }
        }


        public int FaseActualBoss
        {
            get { return _faseActual; }
        }
    }
}

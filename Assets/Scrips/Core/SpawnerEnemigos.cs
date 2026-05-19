using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using SafeRun.Entities;

namespace SafeRun.Core
{
    // Spawner procedural ligero. Persistente entre escenas: colocar uno en la primera escena (ej. Menu Principal o School Main).
    public class SpawnerEnemigos : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private Enemigo prefabRango;
        [SerializeField] private Enemigo prefabStatic;

        [Header("Cantidad procedural")]
        [SerializeField, Min(0)] private int minRango = 1;
        [SerializeField, Min(0)] private int maxRango = 2;
        [SerializeField, Min(0)] private int minStatic = 0;
        [SerializeField, Min(0)] private int maxStatic = 1;

        [Header("Suelo (Tilemap)")]
        [SerializeField] private bool usarSueloTilemap = true;
        [SerializeField] private string[] nombresSuelo = { "piso", "suelo", "Piso", "Suelo" };
        [SerializeField, Min(0f)] private float distanciaMinJugador = 8f;

        [Header("Separacion entre enemigos")]
        [SerializeField, Min(0f)] private float distanciaMinEntreEnemigos = 3f;
        [Tooltip("Si tras agotar los intentos no se cumple la separacion, se acepta la mejor posicion encontrada.")]
        [SerializeField] private bool relajarSiNoHayHueco = true;

        [Header("Fallback (sin tilemap)")]
        [SerializeField] private Transform centro;
        [SerializeField] private bool usarJugadorComoCentro = true;
        [SerializeField, Min(0.1f)] private float radio = 12f;
        [SerializeField] private bool evitarMuros = false;
        [SerializeField] private LayerMask mascaraMuros = 0;
        [SerializeField, Min(0.05f)] private float radioChequeoMuro = 0.35f;
        [SerializeField, Range(1, 100)] private int intentosPorEnemigo = 30;

        [Header("Comportamiento")]
        [SerializeField] private bool persistirEntreEscenas = true;
        [SerializeField] private bool spawnearAlIniciar = true;
        [Tooltip("Si esta activo, cada partida da spawns diferentes. Si esta desactivo, usa 'semilla' + visita para reproducibilidad.")]
        [SerializeField] private bool semillaAleatoria = true;
        [Tooltip("Solo se usa si 'semillaAleatoria' esta desactivo. 0 = se deriva del nombre de escena + numero de visita")]
        [SerializeField] private int semilla = 0;
        [SerializeField] private string[] escenasExcluidas =
        {
            "School High Rank",
            "Menu Principal",
            "Menu Jugador",
            "Menu Pausa",
            "GameOver",
            "GameWinner"
        };
        [SerializeField, Min(0)] private int framesEsperaTrasCarga = 2;

        private static SpawnerEnemigos _instancia;
        private static readonly Dictionary<string, int> _visitas = new();
        private int[] _indicesBaraja;

        private void Awake()
        {
            if (!persistirEntreEscenas) return;

            if (_instancia != null && _instancia != this)
            {
                Destroy(gameObject);
                return;
            }

            _instancia = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += AlCargarEscena;
        }

        private void OnDestroy()
        {
            if (_instancia == this)
            {
                SceneManager.sceneLoaded -= AlCargarEscena;
                _instancia = null;
            }
        }

        private void Start()
        {
            if (!spawnearAlIniciar) return;
            StartCoroutine(SpawnearEnEscena(SceneManager.GetActiveScene().name));
        }

        private void AlCargarEscena(Scene scene, LoadSceneMode mode)
        {
            if (!spawnearAlIniciar) return;
            StartCoroutine(SpawnearEnEscena(scene.name));
        }

        private IEnumerator SpawnearEnEscena(string escena)
        {
            for (int i = 0; i < framesEsperaTrasCarga; i++) yield return null;

            foreach (var ex in escenasExcluidas)
            {
                if (escena == ex) yield break;
            }

            if (!_visitas.TryGetValue(escena, out int visita)) visita = 0;
            _visitas[escena] = visita + 1;

            System.Random rnd;
            if (semillaAleatoria)
            {
                rnd = new System.Random();
            }
            else
            {
                int seed = semilla != 0 ? semilla + visita : (escena.GetHashCode() ^ (visita * 73856093));
                rnd = new System.Random(seed);
            }

            int nRango = NextInclusive(rnd, minRango, maxRango);
            int nStatic = NextInclusive(rnd, minStatic, maxStatic);

            List<Vector3> celdasSuelo = usarSueloTilemap ? RecolectarCeldasSuelo() : null;
            List<Vector3> ocupadas = RecolectarPosicionesEnemigosExistentes();

            for (int i = 0; i < nRango; i++) SpawnearUno(prefabRango, rnd, celdasSuelo, ocupadas);
            for (int i = 0; i < nStatic; i++) SpawnearUno(prefabStatic, rnd, celdasSuelo, ocupadas);
        }

        private List<Vector3> RecolectarPosicionesEnemigosExistentes()
        {
            var lista = new List<Vector3>();
            var enemigos = FindObjectsByType<Enemigo>(FindObjectsSortMode.None);
            for (int i = 0; i < enemigos.Length; i++)
            {
                if (enemigos[i] != null) lista.Add(enemigos[i].transform.position);
            }
            return lista;
        }

        private List<Vector3> RecolectarCeldasSuelo()
        {
            var tilemap = BuscarTilemapSuelo();
            if (tilemap == null) return null;

            var resultado = new List<Vector3>();
            var bounds = tilemap.cellBounds;
            var pos = new Vector3Int();
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    pos.x = x; pos.y = y; pos.z = 0;
                    if (tilemap.HasTile(pos))
                        resultado.Add(tilemap.GetCellCenterWorld(pos));
                }
            }
            return resultado.Count > 0 ? resultado : null;
        }

        private Tilemap BuscarTilemapSuelo()
        {
            var todos = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
            foreach (var tm in todos)
            {
                foreach (var nombre in nombresSuelo)
                {
                    if (tm.gameObject.name == nombre) return tm;
                }
            }
            return null;
        }

        private void SpawnearUno(Enemigo prefab, System.Random rnd, List<Vector3> celdasSuelo, List<Vector3> ocupadas)
        {
            if (prefab == null)
            {
                Debug.LogWarning("[SpawnerEnemigos] Prefab no asignado", this);
                return;
            }

            Transform jugador = null;
            var jug = FindAnyObjectByType<Jugador>();
            if (jug != null) jugador = jug.transform;

            Vector3 mejorPos = Vector3.zero;
            float mejorSeparacion = -1f;
            bool hayMejor = false;

            if (celdasSuelo != null && celdasSuelo.Count > 0)
            {
                int total = celdasSuelo.Count;
                if (_indicesBaraja == null || _indicesBaraja.Length < total)
                    _indicesBaraja = new int[total];
                for (int i = 0; i < total; i++) _indicesBaraja[i] = i;
                BarajarFisherYates(_indicesBaraja, total, rnd);

                int limite = Mathf.Min(total, Mathf.Max(intentosPorEnemigo, total));
                for (int k = 0; k < limite; k++)
                {
                    Vector3 pos = celdasSuelo[_indicesBaraja[k]];
                    if (jugador != null && Vector2.Distance(pos, jugador.position) < distanciaMinJugador)
                        continue;

                    float sep = SeparacionMinima(pos, ocupadas);
                    if (sep >= distanciaMinEntreEnemigos)
                    {
                        InstanciarYRegistrar(prefab, pos, ocupadas);
                        return;
                    }
                    if (sep > mejorSeparacion)
                    {
                        mejorSeparacion = sep;
                        mejorPos = pos;
                        hayMejor = true;
                    }
                }

                if (relajarSiNoHayHueco && hayMejor)
                {
                    Debug.LogWarning($"[SpawnerEnemigos] Sin celda con separacion >= {distanciaMinEntreEnemigos:F2} para {prefab.name}. Acepto mejor ({mejorSeparacion:F2}).", this);
                    InstanciarYRegistrar(prefab, mejorPos, ocupadas);
                    return;
                }
                Debug.LogWarning($"[SpawnerEnemigos] No se hallo celda de suelo valida para {prefab.name} en {SceneManager.GetActiveScene().name}", this);
                return;
            }

            Vector3 origen;
            if (usarJugadorComoCentro && jugador != null)
                origen = jugador.position;
            else
                origen = (centro != null ? centro : transform).position;

            for (int intento = 0; intento < intentosPorEnemigo; intento++)
            {
                float ang = (float)(rnd.NextDouble() * Mathf.PI * 2.0);
                float t = (float)rnd.NextDouble();
                float dist = Mathf.Lerp(distanciaMinJugador, radio, Mathf.Sqrt(t));
                Vector3 pos = origen + new Vector3(Mathf.Cos(ang) * dist, Mathf.Sin(ang) * dist, 0f);

                if (jugador != null && Vector2.Distance(pos, jugador.position) < distanciaMinJugador)
                    continue;
                if (evitarMuros && Physics2D.OverlapCircle(pos, radioChequeoMuro, mascaraMuros) != null)
                    continue;

                float sep = SeparacionMinima(pos, ocupadas);
                if (sep >= distanciaMinEntreEnemigos)
                {
                    InstanciarYRegistrar(prefab, pos, ocupadas);
                    return;
                }
                if (sep > mejorSeparacion)
                {
                    mejorSeparacion = sep;
                    mejorPos = pos;
                    hayMejor = true;
                }
            }

            if (relajarSiNoHayHueco && hayMejor)
            {
                InstanciarYRegistrar(prefab, mejorPos, ocupadas);
                return;
            }

            Debug.LogWarning($"[SpawnerEnemigos] No se encontro posicion valida para {prefab.name} en {SceneManager.GetActiveScene().name}", this);
        }

        private static void BarajarFisherYates(int[] arr, int n, System.Random rnd)
        {
            for (int i = n - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                int tmp = arr[i]; arr[i] = arr[j]; arr[j] = tmp;
            }
        }

        private static float SeparacionMinima(Vector3 pos, List<Vector3> ocupadas)
        {
            if (ocupadas == null || ocupadas.Count == 0) return float.PositiveInfinity;
            float minSqr = float.PositiveInfinity;
            for (int i = 0; i < ocupadas.Count; i++)
            {
                float d = ((Vector2)(pos - ocupadas[i])).sqrMagnitude;
                if (d < minSqr) minSqr = d;
            }
            return Mathf.Sqrt(minSqr);
        }

        private static void InstanciarYRegistrar(Enemigo prefab, Vector3 pos, List<Vector3> ocupadas)
        {
            Instantiate(prefab, pos, Quaternion.identity);
            ocupadas?.Add(pos);
        }

        private static int NextInclusive(System.Random rnd, int min, int max)
        {
            if (max < min) max = min;
            return rnd.Next(min, max + 1);
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 c = (centro != null ? centro : transform).position;
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.35f);
            Gizmos.DrawWireSphere(c, radio);
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawWireSphere(c, distanciaMinJugador);
        }
    }
}

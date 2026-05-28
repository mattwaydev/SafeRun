using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SafeRun.UI;

namespace SafeRun.Core
{
    [Serializable]
    public struct RunEntry
    {
        public string nombre;
        public float tiempo;
        public int enemigos;
        public int puntos;
        public long fechaUnix;
    }

    public class RunStatsManager : MonoBehaviour
    {
        private const string PlayerPrefsKey = "SafeRun_Leaderboard_v1";
        private const string EscenaInicioRun = "School Main";
        private const int MaxEntradasGuardadas = 50;

        private static RunStatsManager _instancia;

        public static RunStatsManager Instancia
        {
            get
            {
                EnsureExists();
                return _instancia;
            }
        }

        public static void EnsureExists()
        {
            if (_instancia != null) return;
            var go = new GameObject("RunStatsManager");
            DontDestroyOnLoad(go);
            _instancia = go.AddComponent<RunStatsManager>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            EnsureExists();
        }

        public bool RunActiva { get; private set; }
        public string JugadorActual { get; private set; } = "JUGADOR";
        public int EnemigosDerrotados { get; private set; }
        public float TiempoUltimaRun { get; private set; }

        public RunEntry UltimaRun { get; private set; }
        public bool HayUltimaRun { get; private set; }
        public List<RunEntry> Leaderboard { get; private set; } = new List<RunEntry>();

        private float _tiempoInicio;
        private bool _esperandoNombre;
        private bool _runConsumida;

        public float TiempoActual
        {
            get
            {
                if (RunActiva) return Time.unscaledTime - _tiempoInicio;
                return TiempoUltimaRun;
            }
        }

        private void Awake()
        {
            if (_instancia != null && _instancia != this)
            {
                Destroy(gameObject);
                return;
            }
            _instancia = this;
            DontDestroyOnLoad(gameObject);
            CargarLeaderboard();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            if (_instancia == this)
                _instancia = null;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            string nombre = scene.name;

            if (nombre == "Menu Principal"
                || nombre == "CONTEXTO"
                || nombre == "PILOCODE INTRO"
                || nombre == "GameOver"
                || nombre == "GameWinner"
                || nombre == "ESTADISTICA")
            {
                ReiniciarParaNuevaRun();
                return;
            }

            if (nombre == EscenaInicioRun && !RunActiva && !_runConsumida && !_esperandoNombre)
            {
                PedirNombreEIniciar();
            }
        }

        private void PedirNombreEIniciar()
        {
            _esperandoNombre = true;
            string sugerido = string.IsNullOrWhiteSpace(JugadorActual) ? "" : JugadorActual;
            NameEntryOverlay.Mostrar(sugerido, OnNombreConfirmado);
        }

        private void OnNombreConfirmado(string nombre)
        {
            _esperandoNombre = false;
            IniciarRun(nombre);
        }

        public void IniciarRun(string nombre)
        {
            JugadorActual = string.IsNullOrWhiteSpace(nombre)
                ? "JUGADOR"
                : nombre.Trim().ToUpperInvariant();
            EnemigosDerrotados = 0;
            _tiempoInicio = Time.unscaledTime;
            RunActiva = true;
            _runConsumida = false;
            Debug.Log($"[SafeRun] Run iniciada para '{JugadorActual}'");
        }

        public void RegistrarEnemigoDerrotado()
        {
            if (!RunActiva) return;
            EnemigosDerrotados++;
        }

        public void TerminarRunVictoria()
        {
            if (!RunActiva) return;

            TiempoUltimaRun = Time.unscaledTime - _tiempoInicio;
            int puntos = CalcularPuntos(TiempoUltimaRun, EnemigosDerrotados);

            var entrada = new RunEntry
            {
                nombre = JugadorActual,
                tiempo = TiempoUltimaRun,
                enemigos = EnemigosDerrotados,
                puntos = puntos,
                fechaUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            UltimaRun = entrada;
            HayUltimaRun = true;
            RunActiva = false;
            _runConsumida = true;

            Leaderboard.Add(entrada);
            Leaderboard.Sort((a, b) => b.puntos.CompareTo(a.puntos));
            if (Leaderboard.Count > MaxEntradasGuardadas)
                Leaderboard.RemoveRange(MaxEntradasGuardadas, Leaderboard.Count - MaxEntradasGuardadas);

            GuardarLeaderboard();
            Debug.Log($"[SafeRun] Run terminada: {entrada.nombre} {entrada.puntos}p en {entrada.tiempo:0.0}s ({entrada.enemigos} enemigos)");
        }

        public void AbandonarRun()
        {
            if (!RunActiva) return;
            RunActiva = false;
            _runConsumida = true;
            TiempoUltimaRun = Time.unscaledTime - _tiempoInicio;
        }

        public void ReiniciarParaNuevaRun()
        {
            RunActiva = false;
            _runConsumida = false;
            _esperandoNombre = false;
            EnemigosDerrotados = 0;
        }

        public int PosicionUltimaRun
        {
            get
            {
                if (!HayUltimaRun) return -1;
                long firma = UltimaRun.fechaUnix;
                for (int i = 0; i < Leaderboard.Count; i++)
                {
                    if (Leaderboard[i].fechaUnix == firma && Leaderboard[i].nombre == UltimaRun.nombre)
                        return i + 1;
                }
                return -1;
            }
        }

        public static int CalcularPuntos(float tiempoSegundos, int enemigos)
        {
            const int basePuntos = 4000;
            const int puntosPorEnemigo = 150;
            const float penalizacionTiempo = 4.5f;
            const float tiempoLimitePenalizacion = 720f;
            const int bonusVelocidad = 1800;
            const float umbralBonusSegundos = 240f;

            float tiempoEfectivo = Mathf.Min(tiempoSegundos, tiempoLimitePenalizacion);
            int puntosEnemigos = enemigos * puntosPorEnemigo;
            int penal = Mathf.RoundToInt(tiempoEfectivo * penalizacionTiempo);
            int extra = tiempoSegundos < umbralBonusSegundos ? bonusVelocidad : 0;
            int total = basePuntos + puntosEnemigos - penal + extra;
            return Mathf.Max(0, total);
        }

        public static string FormatearTiempo(float segundos)
        {
            if (segundos < 0f) segundos = 0f;
            int m = Mathf.FloorToInt(segundos / 60f);
            int s = Mathf.FloorToInt(segundos - m * 60f);
            return string.Format("{0:00}:{1:00}", m, s);
        }

        private void CargarLeaderboard()
        {
            try
            {
                if (!PlayerPrefs.HasKey(PlayerPrefsKey))
                {
                    Leaderboard = new List<RunEntry>();
                    return;
                }
                string json = PlayerPrefs.GetString(PlayerPrefsKey, "");
                if (string.IsNullOrWhiteSpace(json))
                {
                    Leaderboard = new List<RunEntry>();
                    return;
                }
                var wrapper = JsonUtility.FromJson<LeaderboardWrapper>(json);
                Leaderboard = (wrapper != null && wrapper.entradas != null)
                    ? new List<RunEntry>(wrapper.entradas)
                    : new List<RunEntry>();
                Leaderboard.Sort((a, b) => b.puntos.CompareTo(a.puntos));
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[SafeRun] No se pudo cargar leaderboard: " + ex.Message);
                Leaderboard = new List<RunEntry>();
            }
        }

        private void GuardarLeaderboard()
        {
            try
            {
                var wrapper = new LeaderboardWrapper { entradas = Leaderboard.ToArray() };
                string json = JsonUtility.ToJson(wrapper);
                PlayerPrefs.SetString(PlayerPrefsKey, json);
                PlayerPrefs.Save();
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[SafeRun] No se pudo guardar leaderboard: " + ex.Message);
            }
        }

        [Serializable]
        private class LeaderboardWrapper
        {
            public RunEntry[] entradas;
        }
    }
}

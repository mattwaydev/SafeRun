using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SafeRun.Core;

namespace SafeRun.UI
{
    public class EstadisticaUI : MonoBehaviour
    {
        private const string GuidFuente = "d9e803524d7ce5b43a1c0b7b88c0ba26";
        private const int MaxFilasMostradas = 20;

        private TMP_FontAsset _fuente;

        private TextMeshProUGUI _txtContador;
        private TextMeshProUGUI _txtMensaje;
        private RectTransform _contentRanking;

        private void Awake()
        {
            _fuente = ResolveFontAsset();
        }

        private TMP_FontAsset ResolveFontAsset()
        {
            var tmps = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var t in tmps)
            {
                if (t != null && t.font != null && t.font.name.Contains("PressStart2P"))
                    return t.font;
            }
            foreach (var t in tmps)
            {
                if (t != null && t.font != null) return t.font;
            }
            var asset = Resources.Load<TMP_FontAsset>("Fonts & Materials/PressStart2P-Regular SDF");
            if (asset != null) return asset;
            foreach (var f in Resources.FindObjectsOfTypeAll<TMP_FontAsset>())
            {
                if (f != null && f.name.Contains("PressStart2P"))
                    return f;
            }
            return TMP_Settings.defaultFontAsset;
        }

        private void Start()
        {
            LocalizarComponentes();
            RellenarStatsActuales();
            EnriquecerPanel();
            ConstruirRanking();
        }

        private void LocalizarComponentes()
        {
            var tmps = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var t in tmps)
            {
                if (t == null) continue;
                if (t.gameObject.name == "CONTADOR Text (TMP)")
                    _txtContador = t;
                else if (t.gameObject.name == "mensaje Text (TMP)")
                    _txtMensaje = t;
            }

            var scrolls = FindObjectsByType<ScrollRect>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            ScrollRect ranking = null;
            foreach (var s in scrolls)
            {
                if (s == null) continue;
                if (s.gameObject.name.Contains("Ranking"))
                {
                    ranking = s;
                    break;
                }
            }
            if (ranking == null && scrolls.Length > 0)
                ranking = scrolls[0];

            if (ranking != null)
            {
                ArreglarScrollRect(ranking);
                _contentRanking = ranking.content;
            }

            if (_contentRanking == null)
                _contentRanking = CrearContenedorRankingFallback();

            Debug.Log($"[SafeRun] EstadisticaUI: contentRanking={(_contentRanking != null ? _contentRanking.name : "NULL")}, contador={(_txtContador != null)}, mensaje={(_txtMensaje != null)}");
        }

        private RectTransform CrearContenedorRankingFallback()
        {
            var panel = GameObject.Find("Panel");
            if (panel == null) return null;

            var fondoGO = new GameObject("RankingFallback", typeof(RectTransform), typeof(Image));
            var fondoRect = (RectTransform)fondoGO.transform;
            fondoRect.SetParent(panel.transform, false);
            fondoRect.anchorMin = new Vector2(0.5f, 0.5f);
            fondoRect.anchorMax = new Vector2(0.5f, 0.5f);
            fondoRect.pivot = new Vector2(0.5f, 0.5f);
            fondoRect.sizeDelta = new Vector2(340f, 200f);
            fondoRect.anchoredPosition = new Vector2(120f, -50f);
            var fondoImg = fondoGO.GetComponent<Image>();
            fondoImg.color = new Color(0.05f, 0.03f, 0.15f, 0.55f);

            var contentGO = new GameObject("RankingContent", typeof(RectTransform));
            var contentRect = (RectTransform)contentGO.transform;
            contentRect.SetParent(fondoRect, false);
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(-8f, 0f);

            return contentRect;
        }

        private void ArreglarScrollRect(ScrollRect ranking)
        {
            var rect = ranking.transform as RectTransform;
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(340f, 200f);
                rect.anchoredPosition = new Vector2(120f, -50f);
            }

            if (ranking.viewport != null)
            {
                var vp = ranking.viewport;
                vp.anchorMin = new Vector2(0f, 0f);
                vp.anchorMax = new Vector2(1f, 1f);
                vp.pivot = new Vector2(0f, 1f);
                vp.anchoredPosition = Vector2.zero;
                vp.offsetMin = new Vector2(0f, 0f);
                vp.offsetMax = new Vector2(-18f, 0f);
                var maskImg = vp.GetComponent<Image>();
                if (maskImg != null)
                    maskImg.color = new Color(maskImg.color.r, maskImg.color.g, maskImg.color.b, 0f);
            }
            ranking.horizontal = false;
            ranking.vertical = true;
            ranking.movementType = ScrollRect.MovementType.Clamped;
        }

        private void RellenarStatsActuales()
        {
            var stats = RunStatsManager.Instancia;

            float tiempo;
            int enemigos;
            int puntos;
            string nombre;
            bool hayDatos = stats.HayUltimaRun;

            if (hayDatos)
            {
                tiempo = stats.UltimaRun.tiempo;
                enemigos = stats.UltimaRun.enemigos;
                puntos = stats.UltimaRun.puntos;
                nombre = stats.UltimaRun.nombre;
            }
            else
            {
                tiempo = 0f;
                enemigos = 0;
                puntos = 0;
                nombre = stats.JugadorActual;
            }

            if (_txtContador != null)
                _txtContador.text = RunStatsManager.FormatearTiempo(tiempo);

            if (_txtMensaje != null)
            {
                string mensaje;
                if (hayDatos)
                {
                    int posicion = stats.PosicionUltimaRun;
                    string posStr = posicion > 0 ? $"#{posicion}" : "-";
                    mensaje = $"{nombre}\nPUESTO {posStr}\n{ObtenerFraseAnimo(posicion)}";
                }
                else
                {
                    mensaje = "TERMINA UNA RUN PARA REGISTRARTE EN EL RANKING";
                }
                _txtMensaje.text = mensaje;
                _txtMensaje.fontSize = 9f;
                _txtMensaje.alignment = TextAlignmentOptions.Center;
            }
        }

        private void EnriquecerPanel()
        {
            var stats = RunStatsManager.Instancia;
            var panel = GameObject.Find("Panel");
            if (panel == null) return;
            var panelRect = panel.GetComponent<RectTransform>();
            if (panelRect == null) return;

            int enemigos = stats.HayUltimaRun ? stats.UltimaRun.enemigos : 0;
            int puntos = stats.HayUltimaRun ? stats.UltimaRun.puntos : 0;

            ReubicarBotones(panelRect);
            ReubicarMensaje();

            CrearFilaInfo(panelRect, "enemiesRow", "ENEMIGOS",
                enemigos.ToString(),
                new Vector2(-134f, -110f),
                new Color(0.95f, 0.55f, 0.55f, 1f));

            CrearFilaInfo(panelRect, "scoreRow", "PUNTOS",
                puntos.ToString("N0"),
                new Vector2(-134f, -148f),
                new Color(1f, 0.95f, 0.55f, 1f),
                tamLabel: 9f,
                tamValor: 14f,
                alturaFila: 32f);
        }

        private void ReubicarBotones(RectTransform panelRect)
        {
            foreach (Transform child in panelRect)
            {
                string nombre = child.gameObject.name;
                if (nombre.Contains("Intentar") || nombre.Contains("Menu Button"))
                {
                    var rect = child as RectTransform;
                    if (rect == null) continue;
                    var pos = rect.anchoredPosition;
                    if (pos.y > -200f && pos.y < -100f)
                    {
                        pos.y = -195f;
                        rect.anchoredPosition = pos;
                    }
                }
            }
        }

        private void ReubicarMensaje()
        {
            if (_txtMensaje == null) return;
            var rect = _txtMensaje.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(250f, 60f);
            rect.anchoredPosition = new Vector2(-103f, -64f);
        }

        private void CrearFilaInfo(RectTransform padre, string nombre, string label, string valor,
            Vector2 anchoredPos, Color colorValor, float tamLabel = 8f, float tamValor = 11f, float alturaFila = 26f)
        {
            var existente = padre.Find(nombre);
            if (existente != null) Destroy(existente.gameObject);

            var go = new GameObject(nombre);
            go.transform.SetParent(padre, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(240f, alturaFila);
            rect.anchoredPosition = anchoredPos;

            var fondo = go.AddComponent<Image>();
            fondo.color = new Color(0.12f, 0.08f, 0.22f, 0.6f);

            var contorno = go.AddComponent<Outline>();
            contorno.effectColor = new Color(0.55f, 0.35f, 0.8f, 0.85f);
            contorno.effectDistance = new Vector2(1f, -1f);

            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(rect, false);
            var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
            labelTMP.font = _fuente;
            labelTMP.text = label;
            labelTMP.fontSize = tamLabel;
            labelTMP.alignment = TextAlignmentOptions.MidlineLeft;
            labelTMP.color = new Color(0.85f, 0.7f, 1f, 1f);
            labelTMP.raycastTarget = false;
            var labelRect = labelTMP.rectTransform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = new Vector2(0.5f, 1f);
            labelRect.offsetMin = new Vector2(8f, 0f);
            labelRect.offsetMax = Vector2.zero;

            var valorGO = new GameObject("Valor");
            valorGO.transform.SetParent(rect, false);
            var valorTMP = valorGO.AddComponent<TextMeshProUGUI>();
            valorTMP.font = _fuente;
            valorTMP.text = valor;
            valorTMP.fontSize = tamValor;
            valorTMP.alignment = TextAlignmentOptions.MidlineRight;
            valorTMP.color = colorValor;
            valorTMP.raycastTarget = false;
            var valorRect = valorTMP.rectTransform;
            valorRect.anchorMin = new Vector2(0.5f, 0f);
            valorRect.anchorMax = Vector2.one;
            valorRect.offsetMin = Vector2.zero;
            valorRect.offsetMax = new Vector2(-8f, 0f);
        }

        private void ConstruirRanking()
        {
            if (_contentRanking == null) return;

            for (int i = _contentRanking.childCount - 1; i >= 0; i--)
                Destroy(_contentRanking.GetChild(i).gameObject);

            var stats = RunStatsManager.Instancia;
            var lista = stats.Leaderboard;
            if (lista == null) lista = new List<RunEntry>();

            const float alturaFila = 38f;
            float anchoContenedor = _contentRanking.rect.width;
            int total = Mathf.Min(lista.Count, MaxFilasMostradas);

            _contentRanking.sizeDelta = new Vector2(_contentRanking.sizeDelta.x, Mathf.Max(alturaFila * Mathf.Max(1, total) + 8f, 60f));

            if (total == 0)
            {
                CrearFilaVacia();
                return;
            }

            long firmaActual = stats.HayUltimaRun ? stats.UltimaRun.fechaUnix : long.MinValue;
            for (int i = 0; i < total; i++)
            {
                var entrada = lista[i];
                bool esActual = stats.HayUltimaRun && entrada.fechaUnix == firmaActual && entrada.nombre == stats.UltimaRun.nombre;
                CrearFilaRanking(i + 1, entrada, esActual, alturaFila);
            }
        }

        private void CrearFilaVacia()
        {
            var go = new GameObject("RankingVacio");
            go.transform.SetParent(_contentRanking, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, -6f);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.font = _fuente;
            tmp.text = "SE EL PRIMERO\nEN VENCER AL BOSS";
            tmp.fontSize = 9f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(0.8f, 0.7f, 1f, 1f);
            tmp.raycastTarget = false;
        }

        private void CrearFilaRanking(int posicion, RunEntry entrada, bool resaltar, float alturaFila)
        {
            var go = new GameObject($"Fila_{posicion}");
            go.transform.SetParent(_contentRanking, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(-6f, alturaFila - 4f);
            rect.anchoredPosition = new Vector2(0f, -(posicion - 1) * alturaFila - 4f);

            var fondo = go.AddComponent<Image>();
            if (resaltar)
                fondo.color = new Color(0.9f, 0.45f, 0.95f, 0.55f);
            else if (posicion == 1)
                fondo.color = new Color(1f, 0.85f, 0.2f, 0.45f);
            else if (posicion == 2)
                fondo.color = new Color(0.75f, 0.75f, 0.85f, 0.4f);
            else if (posicion == 3)
                fondo.color = new Color(0.8f, 0.55f, 0.25f, 0.4f);
            else
                fondo.color = new Color(0.15f, 0.1f, 0.25f, 0.55f);

            var contorno = go.AddComponent<Outline>();
            contorno.effectColor = resaltar
                ? new Color(1f, 1f, 1f, 0.9f)
                : new Color(0.4f, 0.3f, 0.55f, 0.8f);
            contorno.effectDistance = new Vector2(1f, -1f);

            CrearTextoEnFila(rect, "Pos", $"#{posicion}", 11f, TextAlignmentOptions.MidlineLeft,
                new Vector2(0f, 0f), new Vector2(0.18f, 1f), new Vector2(4f, 0f), Vector2.zero, ColorPos(posicion));

            CrearTextoEnFila(rect, "Nombre", entrada.nombre, 10f, TextAlignmentOptions.MidlineLeft,
                new Vector2(0.18f, 0f), new Vector2(0.55f, 1f), Vector2.zero, Vector2.zero, Color.white);

            CrearTextoEnFila(rect, "Tiempo", RunStatsManager.FormatearTiempo(entrada.tiempo), 9f, TextAlignmentOptions.Midline,
                new Vector2(0.55f, 0f), new Vector2(0.74f, 1f), Vector2.zero, Vector2.zero, new Color(0.9f, 0.85f, 1f, 1f));

            CrearTextoEnFila(rect, "Enemigos", entrada.enemigos.ToString(), 9f, TextAlignmentOptions.Midline,
                new Vector2(0.74f, 0f), new Vector2(0.87f, 1f), Vector2.zero, Vector2.zero, new Color(0.95f, 0.6f, 0.6f, 1f));

            CrearTextoEnFila(rect, "Puntos", entrada.puntos.ToString("N0"), 11f, TextAlignmentOptions.MidlineRight,
                new Vector2(0.87f, 0f), new Vector2(1f, 1f), Vector2.zero, new Vector2(-4f, 0f), new Color(1f, 0.95f, 0.55f, 1f));
        }

        private Color ColorPos(int pos)
        {
            if (pos == 1) return new Color(1f, 0.85f, 0.2f, 1f);
            if (pos == 2) return new Color(0.85f, 0.85f, 0.95f, 1f);
            if (pos == 3) return new Color(1f, 0.7f, 0.35f, 1f);
            return new Color(0.85f, 0.75f, 1f, 1f);
        }

        private void CrearTextoEnFila(RectTransform padre, string nombre, string texto, float tam,
            TextAlignmentOptions align, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, Color color)
        {
            var go = new GameObject(nombre);
            go.transform.SetParent(padre, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.font = _fuente;
            tmp.text = texto;
            tmp.fontSize = tam;
            tmp.alignment = align;
            tmp.color = color;
            tmp.raycastTarget = false;
            tmp.overflowMode = TextOverflowModes.Truncate;
            var rect = tmp.rectTransform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static string ObtenerFraseAnimo(int posicion)
        {
            if (posicion == 1) return "ERES LEYENDA";
            if (posicion == 2) return "CASI PERFECTO";
            if (posicion == 3) return "PODIO LOGRADO";
            if (posicion > 0 && posicion <= 10) return "TOP 10 BIEN AHI";
            return "OTRA RUN MAS RAPIDA?";
        }
    }
}

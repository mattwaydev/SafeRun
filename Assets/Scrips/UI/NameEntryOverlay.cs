using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SafeRun.UI
{
    public class NameEntryOverlay : MonoBehaviour
    {
        private const string GuidFuente = "d9e803524d7ce5b43a1c0b7b88c0ba26";
        private const int MaxCaracteres = 12;
        private const float DuracionParpadeo = 0.55f;

        private static NameEntryOverlay _instancia;

        private GameObject _overlay;
        private TextMeshProUGUI _textoNombre;
        private TextMeshProUGUI _hint;
        private CanvasGroup _grupo;
        private Action<string> _callback;
        private StringBuilder _buffer = new StringBuilder();
        private float _timeScaleAnterior = 1f;
        private bool _pausaAplicada;
        private bool _aceptandoInput;
        private float _timerParpadeo;
        private bool _cursorVisible;
        private TMP_FontAsset _fuente;

        public static bool EstaActivo { get; private set; }

        public static void Mostrar(string sugerido, Action<string> callback)
        {
            GarantizarInstancia();
            _instancia.Lanzar(sugerido, callback);
        }

        private static void GarantizarInstancia()
        {
            if (_instancia != null) return;
            var go = new GameObject("NameEntryOverlay");
            DontDestroyOnLoad(go);
            _instancia = go.AddComponent<NameEntryOverlay>();
        }

        private void Lanzar(string sugerido, Action<string> callback)
        {
            _callback = callback;
            _buffer.Clear();
            if (!string.IsNullOrEmpty(sugerido))
            {
                foreach (char c in sugerido.ToUpperInvariant())
                {
                    if (_buffer.Length >= MaxCaracteres) break;
                    if (EsCaracterPermitido(c))
                        _buffer.Append(c);
                }
            }

            if (_overlay != null)
            {
                Destroy(_overlay);
                _overlay = null;
            }

            StartCoroutine(Construir());
        }

        private IEnumerator Construir()
        {
            AplicarPausa();
            yield return null;

            _fuente = ResolveFontAsset();

            _overlay = new GameObject("NameEntryCanvas");
            DontDestroyOnLoad(_overlay);

            var canvas = _overlay.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 6000;

            var scaler = _overlay.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(800f, 600f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            _overlay.AddComponent<GraphicRaycaster>();
            _grupo = _overlay.AddComponent<CanvasGroup>();
            _grupo.alpha = 0f;
            _grupo.blocksRaycasts = true;

            CrearFondo();
            var panel = CrearPanel();
            CrearTitulo(panel);
            CrearCajaNombre(panel);
            _hint = CrearHint(panel);

            yield return AnimarEntrada();

            _aceptandoInput = true;
            SuscribirInput(true);
            ActualizarTexto();
        }

        private void CrearFondo()
        {
            var go = new GameObject("Fondo");
            go.transform.SetParent(_overlay.transform, false);
            var img = go.AddComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0.78f);
            var rect = img.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private RectTransform CrearPanel()
        {
            var go = new GameObject("Panel");
            go.transform.SetParent(_overlay.transform, false);
            var img = go.AddComponent<Image>();
            img.color = new Color(0.07f, 0.05f, 0.18f, 0.96f);

            var contorno = go.AddComponent<Outline>();
            contorno.effectColor = new Color(0.65f, 0.42f, 1f, 1f);
            contorno.effectDistance = new Vector2(2f, -2f);

            var rect = img.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(460f, 260f);
            rect.anchoredPosition = Vector2.zero;
            return rect;
        }

        private void CrearTitulo(RectTransform padre)
        {
            var go = new GameObject("Titulo");
            go.transform.SetParent(padre, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.font = _fuente;
            tmp.text = "INGRESA TU NOMBRE";
            tmp.fontSize = 20f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(1f, 1f, 1f, 1f);
            tmp.raycastTarget = false;
            var rect = tmp.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(420f, 50f);
            rect.anchoredPosition = new Vector2(0f, 85f);
        }

        private void CrearCajaNombre(RectTransform padre)
        {
            var cajaGO = new GameObject("Caja");
            cajaGO.transform.SetParent(padre, false);
            var cajaImg = cajaGO.AddComponent<Image>();
            cajaImg.color = new Color(0f, 0f, 0f, 0.55f);
            var cajaContorno = cajaGO.AddComponent<Outline>();
            cajaContorno.effectColor = new Color(0.9f, 0.8f, 1f, 1f);
            cajaContorno.effectDistance = new Vector2(1.5f, -1.5f);
            var cajaRect = cajaImg.rectTransform;
            cajaRect.anchorMin = new Vector2(0.5f, 0.5f);
            cajaRect.anchorMax = new Vector2(0.5f, 0.5f);
            cajaRect.pivot = new Vector2(0.5f, 0.5f);
            cajaRect.sizeDelta = new Vector2(360f, 60f);
            cajaRect.anchoredPosition = new Vector2(0f, 5f);

            var textoGO = new GameObject("Texto");
            textoGO.transform.SetParent(cajaRect, false);
            _textoNombre = textoGO.AddComponent<TextMeshProUGUI>();
            _textoNombre.font = _fuente;
            _textoNombre.text = "";
            _textoNombre.fontSize = 24f;
            _textoNombre.alignment = TextAlignmentOptions.Center;
            _textoNombre.color = new Color(1f, 0.95f, 0.55f, 1f);
            _textoNombre.raycastTarget = false;
            var textoRect = _textoNombre.rectTransform;
            textoRect.anchorMin = Vector2.zero;
            textoRect.anchorMax = Vector2.one;
            textoRect.offsetMin = new Vector2(8f, 4f);
            textoRect.offsetMax = new Vector2(-8f, -4f);
        }

        private TextMeshProUGUI CrearHint(RectTransform padre)
        {
            var go = new GameObject("Hint");
            go.transform.SetParent(padre, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.font = _fuente;
            tmp.text = "ESCRIBE Y PULSA ENTER";
            tmp.fontSize = 10f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(0.8f, 0.65f, 1f, 1f);
            tmp.raycastTarget = false;
            var rect = tmp.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(420f, 30f);
            rect.anchoredPosition = new Vector2(0f, -95f);
            return tmp;
        }

        private IEnumerator AnimarEntrada()
        {
            const float duracion = 0.25f;
            float t = 0f;
            while (t < duracion)
            {
                if (_grupo == null) yield break;
                t += Time.unscaledDeltaTime;
                _grupo.alpha = Mathf.Clamp01(t / duracion);
                yield return null;
            }
            if (_grupo != null) _grupo.alpha = 1f;
        }

        private void Update()
        {
            if (!_aceptandoInput) return;

            _timerParpadeo += Time.unscaledDeltaTime;
            if (_timerParpadeo >= DuracionParpadeo)
            {
                _timerParpadeo = 0f;
                _cursorVisible = !_cursorVisible;
                ActualizarTexto();
            }

            var kb = Keyboard.current;
            if (kb == null) return;

            if (kb.backspaceKey.wasPressedThisFrame && _buffer.Length > 0)
            {
                _buffer.Length -= 1;
                ActualizarTexto();
            }

            if (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame)
            {
                Confirmar();
            }

            var gamepad = Gamepad.current;
            if (gamepad != null && gamepad.startButton.wasPressedThisFrame)
            {
                Confirmar();
            }
        }

        private void OnEnable()
        {
            if (_aceptandoInput)
                SuscribirInput(true);
        }

        private void OnDisable()
        {
            SuscribirInput(false);
        }

        private void SuscribirInput(bool sub)
        {
            if (Keyboard.current == null) return;
            Keyboard.current.onTextInput -= EnTextInput;
            if (sub)
                Keyboard.current.onTextInput += EnTextInput;
        }

        private void EnTextInput(char c)
        {
            if (!_aceptandoInput) return;
            if (_buffer.Length >= MaxCaracteres) return;
            if (!EsCaracterPermitido(c)) return;
            char upper = char.ToUpperInvariant(c);
            _buffer.Append(upper);
            ActualizarTexto();
        }

        private static TMP_FontAsset ResolveFontAsset()
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
            foreach (var f in Resources.FindObjectsOfTypeAll<TMP_FontAsset>())
            {
                if (f != null && f.name.Contains("PressStart2P"))
                    return f;
            }
            return TMP_Settings.defaultFontAsset;
        }

        private static bool EsCaracterPermitido(char c)
        {
            if (c >= 'A' && c <= 'Z') return true;
            if (c >= 'a' && c <= 'z') return true;
            if (c >= '0' && c <= '9') return true;
            if (c == ' ' || c == '-' || c == '_') return true;
            return false;
        }

        private void ActualizarTexto()
        {
            if (_textoNombre == null) return;
            string cursor = _cursorVisible ? "_" : " ";
            _textoNombre.text = _buffer.ToString() + cursor;
        }

        private void Confirmar()
        {
            if (!_aceptandoInput) return;
            string resultado = _buffer.ToString().Trim();
            if (resultado.Length == 0)
                resultado = "JUGADOR";

            _aceptandoInput = false;
            SuscribirInput(false);
            StartCoroutine(AnimarSalida(resultado));
        }

        private IEnumerator AnimarSalida(string resultado)
        {
            const float duracion = 0.2f;
            float t = 0f;
            while (t < duracion)
            {
                if (_grupo == null) break;
                t += Time.unscaledDeltaTime;
                _grupo.alpha = 1f - Mathf.Clamp01(t / duracion);
                yield return null;
            }

            RestaurarTiempo();
            EstaActivo = false;

            if (_overlay != null)
            {
                Destroy(_overlay);
                _overlay = null;
            }

            var cb = _callback;
            _callback = null;
            cb?.Invoke(resultado);
        }

        private void AplicarPausa()
        {
            if (_pausaAplicada) return;
            _timeScaleAnterior = Time.timeScale;
            Time.timeScale = 0f;
            _pausaAplicada = true;
            EstaActivo = true;
        }

        private void RestaurarTiempo()
        {
            if (!_pausaAplicada) return;
            Time.timeScale = _timeScaleAnterior > 0f ? _timeScaleAnterior : 1f;
            _pausaAplicada = false;
        }

        private void OnDestroy()
        {
            SuscribirInput(false);
            RestaurarTiempo();
            if (_instancia == this)
                _instancia = null;
        }
    }
}

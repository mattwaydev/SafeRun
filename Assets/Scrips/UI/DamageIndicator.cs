using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private Sprite vignetteSprite;
    [SerializeField] private Color tint = new Color(1f, 0f, 0f, 1f);
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 0.6f;

    [SerializeField] private float fadeInSeconds = 0.08f;
    [SerializeField] private float fadeOutSeconds = 0.35f;
    [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    [SerializeField] private bool useUnscaledTime = true;

    [SerializeField] private int sortingOrder = 1000;

    private Canvas canvas;
    private Image image;
    private Coroutine flashRoutine;

    private void Awake()
    {
        EnsureUI();
        SetAlpha(0f);
    }

    public void Trigger()
    {
        Trigger(1f);
    }

    public void Trigger(float intensityMultiplier)
    {
        EnsureUI();

        float peakAlpha = Mathf.Clamp01(maxAlpha * Mathf.Max(0f, intensityMultiplier));
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine(peakAlpha));
    }

    public void SetSprite(Sprite sprite)
    {
        vignetteSprite = sprite;
        if (image != null)
        {
            image.sprite = vignetteSprite;
        }
    }

    private void EnsureUI()
    {
        if (image != null)
        {
            return;
        }

        Transform canvasTransform = transform.Find("DamageIndicatorCanvas");
        if (canvasTransform != null)
        {
            canvas = canvasTransform.GetComponent<Canvas>();
        }

        if (canvas == null)
        {
            var canvasObject = new GameObject("DamageIndicatorCanvas");
            canvasObject.transform.SetParent(transform, false);
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();

            int uiLayer = LayerMask.NameToLayer("UI");
            if (uiLayer != -1)
            {
                canvasObject.layer = uiLayer;
            }
        }

        var imageObject = new GameObject("DamageIndicator");
        imageObject.transform.SetParent(canvas.transform, false);
        image = imageObject.AddComponent<Image>();
        image.sprite = vignetteSprite;
        image.raycastTarget = false;
        image.preserveAspect = false;

        int imageLayer = LayerMask.NameToLayer("UI");
        if (imageLayer != -1)
        {
            imageObject.layer = imageLayer;
        }

        var rectTransform = image.rectTransform;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private IEnumerator FlashRoutine(float peakAlpha)
    {
        float time = 0f;
        if (fadeInSeconds > 0f)
        {
            while (time < fadeInSeconds)
            {
                float t = time / fadeInSeconds;
                float curveValue = fadeInCurve != null ? fadeInCurve.Evaluate(t) : t;
                SetAlpha(Mathf.Lerp(0f, peakAlpha, curveValue));
                time += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
        }

        SetAlpha(peakAlpha);

        time = 0f;
        if (fadeOutSeconds > 0f)
        {
            while (time < fadeOutSeconds)
            {
                float t = time / fadeOutSeconds;
                float curveValue = fadeOutCurve != null ? fadeOutCurve.Evaluate(t) : t;
                SetAlpha(Mathf.Lerp(peakAlpha, 0f, curveValue));
                time += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
        }

        SetAlpha(0f);
        flashRoutine = null;
    }

    private void SetAlpha(float alpha)
    {
        if (image == null)
        {
            return;
        }
        Color color = tint;
        color.a = Mathf.Clamp01(alpha);
        image.color = color;
    }
}

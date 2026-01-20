using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class FpsDisplayTMP : MonoBehaviour
{
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private float updateInterval = 0.5f;
    [SerializeField] private bool useUnscaledTime = true;
    [SerializeField] private bool includeFrameTime = false;
    [SerializeField] private string fpsFormat = "FPS: {0:0}";
    [SerializeField] private string fpsWithMsFormat = "FPS: {0:0} ({1:0.0} ms)";

    private float timeLeft;
    private float timeAccum;
    private int frameCount;

    private void Awake()
    {
        if (targetText == null)
            targetText = GetComponent<TMP_Text>();

        if (targetText == null)
            Debug.LogError("[FpsDisplayTMP] TMP_Text missing.");

        timeLeft = updateInterval;
    }

    private void Update()
    {
        var dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        timeLeft -= dt;
        timeAccum += dt;
        frameCount++;

        if (timeLeft > 0f)
            return;

        var fps = timeAccum > 0f ? frameCount / timeAccum : 0f;
        var ms = frameCount > 0 ? (timeAccum / frameCount) * 1000f : 0f;

        if (targetText != null)
        {
            var format = includeFrameTime ? fpsWithMsFormat : fpsFormat;
            targetText.text = string.Format(format, fps, ms);
        }

        timeLeft = updateInterval <= 0f ? 0f : updateInterval;
        timeAccum = 0f;
        frameCount = 0;
    }
}

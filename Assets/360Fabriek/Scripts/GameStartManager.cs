using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStartManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject startUiRoot;
    public Button startButton;
    public TMP_Text startStatusText;
    public string preStartText = "Start";
    public string startedText = "Happy Killing!";

    [Header("Systems to enable on start")]
    public MonoBehaviour[] systemsToEnable;
    public WaveSystem[] waveSystemsToStart;
    public Barracks[] barracksToStart;

    [Header("Auto Find")]
    public bool autoFindSystems = true;

    private bool hasStarted;

    private void Awake()
    {
        if (startUiRoot != null)
            startUiRoot.SetActive(true);

        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (startStatusText != null)
            startStatusText.text = preStartText;

        if (autoFindSystems)
        {
            if (waveSystemsToStart == null || waveSystemsToStart.Length == 0)
                waveSystemsToStart = FindObjectsByType<WaveSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (barracksToStart == null || barracksToStart.Length == 0)
                barracksToStart = FindObjectsByType<Barracks>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }

        if (waveSystemsToStart != null)
        {
            foreach (var waveSystem in waveSystemsToStart)
            {
                if (waveSystem != null)
                    waveSystem.enabled = false;
            }
        }

        if (barracksToStart != null)
        {
            foreach (var barracks in barracksToStart)
            {
                if (barracks != null)
                    barracks.enabled = false;
            }
        }

        if (systemsToEnable != null)
        {
            foreach (var system in systemsToEnable)
            {
                if (system != null)
                    system.enabled = false;
            }
        }

    }

    public void StartGame()
    {
        if (hasStarted) return;
        hasStarted = true;

        if (systemsToEnable != null)
        {
            foreach (var system in systemsToEnable)
            {
                if (system != null)
                    system.enabled = true;
            }
        }

        if (waveSystemsToStart != null)
        {
            foreach (var waveSystem in waveSystemsToStart)
            {
                if (waveSystem != null)
                {
                    waveSystem.enabled = true;
                    waveSystem.StartWaves();
                }
            }
        }

        if (barracksToStart != null)
        {
            foreach (var barracks in barracksToStart)
            {
                if (barracks != null)
                {
                    barracks.enabled = true;
                    barracks.BeginSpawning();
                }
            }
        }

        if (startUiRoot != null)
            startUiRoot.SetActive(false);

        if (startStatusText != null)
            startStatusText.text = startedText;
    }
}

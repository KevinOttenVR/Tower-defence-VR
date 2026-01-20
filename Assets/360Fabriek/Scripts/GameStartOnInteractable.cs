using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class GameStartOnInteractable : MonoBehaviour
{
    public GameStartManager gameStartManager;

    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        if (gameStartManager == null)
            gameStartManager = FindFirstObjectByType<GameStartManager>();
    }

    private void OnEnable()
    {
        if (interactable != null)
            interactable.selectEntered.AddListener(OnSelectEntered);
        if (interactable != null)
            interactable.activated.AddListener(OnActivated);
    }

    private void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnSelectEntered);
        if (interactable != null)
            interactable.activated.RemoveListener(OnActivated);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        TryStartGame();
    }

    private void OnActivated(ActivateEventArgs args)
    {
        TryStartGame();
    }

    private void TryStartGame()
    {
        if (gameStartManager != null)
            gameStartManager.StartGame();
    }
}

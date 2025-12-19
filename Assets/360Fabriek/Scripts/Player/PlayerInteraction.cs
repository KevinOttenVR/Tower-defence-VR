using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public TowerController towerController;
    public TowerData towerToPlace;
    public LayerMask groundLayer;
    public LayerMask towerLayer;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, towerLayer, QueryTriggerInteraction.Ignore))
        {
            Tower existingTower = hit.collider.GetComponentInParent<Tower>();
            if (existingTower != null)
            {
                towerController.UpgradeTower(existingTower.Data, existingTower.gameObject, existingTower.currentLevel);
            }
        }
        else if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            towerController.PlaceTower(towerToPlace, hit.point);
        }
    }
}

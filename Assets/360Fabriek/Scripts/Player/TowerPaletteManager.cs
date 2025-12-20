using UnityEngine;
using System.Collections.Generic;

public class TowerPaletteManager : MonoBehaviour
{
    [Header("Configuration")]
    public List<GameObject> towerPrefabs; // Your original Tower Prefabs
    public List<Transform> spawnSlots;    // Empty GameObjects on the menu where towers sit
    public float menuScale = 0.1f;        // How tiny the towers look on your wrist

    // Call this from HandMenuController -> OnMenuOpen
    public void RefillPalette()
    {
        // Loop through all slots
        for (int i = 0; i < spawnSlots.Count; i++)
        {
            if (i >= towerPrefabs.Count) break;

            // If the slot is empty (because the player grabbed the previous one), fill it
            if (spawnSlots[i].childCount == 0)
            {
                SpawnTowerInSlot(i);
            }
        }
    }

    private void SpawnTowerInSlot(int index)
    {
        GameObject newTower = Instantiate(towerPrefabs[index], spawnSlots[index]);

        // Reset position to zero relative to the slot
        newTower.transform.localPosition = Vector3.zero;
        newTower.transform.localRotation = Quaternion.identity;

        // Apply the special "Menu Logic" script
        TowerMenuBehavior behavior = newTower.GetComponent<TowerMenuBehavior>();
        if (behavior == null) behavior = newTower.AddComponent<TowerMenuBehavior>();

        behavior.Initialize(menuScale);
    }
}
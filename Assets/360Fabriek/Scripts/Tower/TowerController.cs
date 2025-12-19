using UnityEngine;

public class TowerController : MonoBehaviour
{
    int money = 0;

    void PlaceTower(TowerData towerData)
    {
        int towerPrice = towerData.price;

        if (money <= towerPrice)
        {
            money -= towerPrice;

            Instantiate(towerData.towerPrefab);
        }
    }

    void UpgradeTower(TowerData towerData, GameObject tower, int currentLevel)
    {
        // Check if the tower can be upgraded
        if (towerData.levels[currentLevel] == null)
        {
            return;
        }

        // Check if the user has enough money to upgrade the tower
        int upgradePrice = towerData.levels[currentLevel-1].upgradePrice;

        if (money <= upgradePrice)
        {
            money -= upgradePrice;
            tower.GetComponent<Tower>().UpdateTowerStats();
        }
    }

    void SellTower(TowerData towerData, GameObject tower, int currentLevel)
    {
        int sellPrice = towerData.levels[currentLevel - 1].destroyOrKillPrice;

        Destroy(tower);
        money += sellPrice;
    }

    void TakeDamage(int amount, GameObject tower)
    {
        Tower towerInstance = tower.GetComponent<Tower>();
        
        towerInstance.TakeDamage(amount);
        if(towerInstance.currentHP <= 0)
        {
            Destroy(tower);
        }

    }
}

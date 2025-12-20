using Unity.Mathematics;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    void SellTower(TowerData towerData, GameObject tower, int currentLevel)
    {
        int sellPrice = towerData.levels[currentLevel - 1].destroyOrKillPrice;

        Destroy(tower);
        ScoreManager.score += sellPrice;
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

    public void PlaceTower(TowerData towerData, Vector3 worldPosition, Quaternion worldRotation, Transform parentTransform)
    {
        int towerPrice = towerData.price;

        if (ScoreManager.score >= towerPrice)
        {
            ScoreManager.score -= towerPrice;
            GameObject newTower = Instantiate(towerData.towerPrefab, worldPosition, worldRotation, parentTransform);
        }
        else
        {
            Debug.Log("Not enough money to place!");
        }
    }

    public void UpgradeTower(TowerData towerData, GameObject tower, int currentLevel)
    {
        if (currentLevel >= towerData.levels.Length)
        {
            Debug.Log("Tower at max level!");
            return;
        }

        int upgradePrice = towerData.levels[currentLevel -1].upgradePrice;

        if (ScoreManager.score >= upgradePrice)
        {
            ScoreManager.score -= upgradePrice;
            tower.GetComponent<Tower>().UpdateTowerStats();
        }
        else
        {
            Debug.Log("Not enough money to upgrade!");
        }
    }
}

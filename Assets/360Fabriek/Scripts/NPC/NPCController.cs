using UnityEngine;

public class NPCController : MonoBehaviour
{
    void PlaceNPC(NPCData NPCData)
    {
        int npcPrice = NPCData.price;

        if (ScoreManager.score <= npcPrice)
        {
            ScoreManager.score -= npcPrice;

            Instantiate(NPCData.NPCPrefab);
        }
    }

    void TakeDamage(int amount, GameObject npc)
    {
        NPC npcInstance = npc.GetComponent<NPC>();

        npcInstance.TakeDamage(amount);
        if (npcInstance.currentHP <= 0)
        {
            Destroy(npc);
        }
    }
}

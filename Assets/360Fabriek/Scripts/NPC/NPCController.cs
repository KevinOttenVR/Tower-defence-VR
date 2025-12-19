using UnityEngine;

public class NPCController : MonoBehaviour
{
    int money = 0;

    void PlaceNPC(NPCData NPCData)
    {
        int npcPrice = NPCData.price;

        if (money <= npcPrice)
        {
            money -= npcPrice;

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

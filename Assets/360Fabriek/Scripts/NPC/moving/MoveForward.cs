using UnityEngine;

public class MoveForward : MonoBehaviour
{
    private NPC npc;

    void Start()
    {
        npc = GetComponent<NPC>();
        if (npc == null)
        {
            Debug.LogWarning("MoveForward requires an NPC component!");
        }
    }

    void Update()
    {
        if (npc != null)
        {
            transform.Translate(Vector3.right * npc.movementSpeed * Time.deltaTime);
        }
    }
}

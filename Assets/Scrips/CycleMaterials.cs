using System.Collections.Generic;
using UnityEngine;

public class CycleMaterials : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Renderer targetRenderer;

    [Header("Materials (cycled in order)")]
    [SerializeField] private List<Material> materials = new();

    [Header("Options")]
    [SerializeField] private int materialSlotIndex = 0;     // 0 = first submesh/material slot
    [SerializeField] private bool useSharedMaterials = true; // true = no instancing; false = instance at runtime

    private int currentIndex = -1;

    public void NextMaterial()
    {
        if (targetRenderer == null)
        {
            Debug.LogWarning("[CycleMaterials] Target Renderer is not assigned.");
            return;
        }

        if (materials == null || materials.Count == 0)
        {
            Debug.LogWarning("[CycleMaterials] No materials assigned.");
            return;
        }

        currentIndex = (currentIndex + 1) % materials.Count;
        var nextMat = materials[currentIndex];

        if (nextMat == null)
        {
            Debug.LogWarning("[CycleMaterials] Material in list is null.");
            return;
        }

        if (useSharedMaterials)
        {
            var shared = targetRenderer.sharedMaterials;
            if (shared == null || shared.Length == 0)
            {
                Debug.LogWarning("[CycleMaterials] Renderer has no shared materials.");
                return;
            }

            if (materialSlotIndex < 0 || materialSlotIndex >= shared.Length)
            {
                Debug.LogWarning($"[CycleMaterials] materialSlotIndex {materialSlotIndex} out of range (0..{shared.Length - 1}).");
                return;
            }

            shared[materialSlotIndex] = nextMat;
            targetRenderer.sharedMaterials = shared;
        }
        else
        {
            // Creates/uses instanced materials at runtime (fine for demos, can increase memory)
            var mats = targetRenderer.materials;
            if (mats == null || mats.Length == 0)
            {
                Debug.LogWarning("[CycleMaterials] Renderer has no materials.");
                return;
            }

            if (materialSlotIndex < 0 || materialSlotIndex >= mats.Length)
            {
                Debug.LogWarning($"[CycleMaterials] materialSlotIndex {materialSlotIndex} out of range (0..{mats.Length - 1}).");
                return;
            }

            mats[materialSlotIndex] = nextMat;
            targetRenderer.materials = mats;
        }
    }

    public void ResetCycle()
    {
        currentIndex = -1;
    }
}
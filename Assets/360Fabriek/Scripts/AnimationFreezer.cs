using UnityEngine;

[ExecuteInEditMode]
public class AnimationFreezer : MonoBehaviour
{
    [Header("Settings")]
    public AnimationClip clip;
    [Range(0f, 1f)]
    public float normalizedTime;

    public bool freezeInPlace = false;

    // This runs whenever a value is changed in the Inspector
    void OnValidate()
    {
        if (clip != null && freezeInPlace)
        {
            // Samples the animation at the specific time defined by the slider
            clip.SampleAnimation(gameObject, normalizedTime * clip.length);
        }
    }

    void Update()
    {
        // Keeps the pose updated even if you move the object in the editor
        if (!Application.isPlaying && freezeInPlace && clip != null)
        {
            clip.SampleAnimation(gameObject, normalizedTime * clip.length);
        }
    }
}
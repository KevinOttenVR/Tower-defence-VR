using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    [SerializeField] private GameObject target;

    public void Toggle()
    {
        if (target == null) return;
        target.SetActive(!target.activeSelf);
    }
}
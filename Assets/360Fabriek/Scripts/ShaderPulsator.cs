using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderPulsator : MonoBehaviour
{
    [System.Serializable]
    public class ShaderProperty
    {
        public string propertyName;
        public float minValue;
        public float maxValue;
    }

    [SerializeField] private List<ShaderProperty> shaderProperties = new List<ShaderProperty>();
    [SerializeField] private float duration = 1.0f;

    private Material material;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
        }
        else
        {
            Debug.LogError("No Renderer component found on the GameObject.");
            enabled = false;
        }

        StartCoroutine(PulsateProperties());
    }

    IEnumerator PulsateProperties()
    {
        while (true)
        {
            foreach (ShaderProperty property in shaderProperties)
            {
                float startTime = Time.time;
                while (Time.time - startTime < duration)
                {
                    float t = (Time.time - startTime) / duration;
                    float value = Mathf.Lerp(property.minValue, property.maxValue, Mathf.SmoothStep(0f, 1f, t));
                    material.SetFloat(property.propertyName, value);
                    yield return null;
                }

                // Ensure property is set to max value at the end
                material.SetFloat(property.propertyName, property.maxValue);
            }

            yield return null;
        }
    }
}

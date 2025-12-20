using UnityEngine;
using System.Collections;

namespace Fabriek.VisualFX
{
    public class ObjectRotatorFX : MonoBehaviour
    {
        public enum RotationAxis { X, Y, Z }

        [Header("Configuration")]
        public RotationAxis axisToRotate = RotationAxis.X;
        public float rotationAngle = 45f;
        public float duration = 0.5f;

        [Header("Trigger")]
        public bool _Fire = false;

        private bool isRotating = false;
        private Quaternion originalRotation;

        void Start()
        {
            originalRotation = transform.localRotation;
        }

        void Update()
        {
            if (_Fire && !isRotating)
            {
                StartCoroutine(RotateAndBack());
            }
        }

        [ContextMenu("Debug Rotate")]
        public void TriggerRotation()
        {
            if (Application.isPlaying && !isRotating)
            {
                StartCoroutine(RotateAndBack());
            }
        }

        IEnumerator RotateAndBack()
        {
            isRotating = true;
            _Fire = false;

            Vector3 axisVector = Vector3.right;
            if (axisToRotate == RotationAxis.Y) axisVector = Vector3.up;
            if (axisToRotate == RotationAxis.Z) axisVector = Vector3.forward;

            Quaternion targetRotation = originalRotation * Quaternion.Euler(axisVector * rotationAngle);

            // Forward
            yield return StartCoroutine(MoveRotation(originalRotation, targetRotation));
            // Back
            yield return StartCoroutine(MoveRotation(targetRotation, originalRotation));

            isRotating = false;
        }

        IEnumerator MoveRotation(Quaternion from, Quaternion to)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                transform.localRotation = Quaternion.Slerp(from, to, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.localRotation = to;
        }
    }
}
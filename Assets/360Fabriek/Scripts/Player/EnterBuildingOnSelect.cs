using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

namespace _360Fabriek.Player
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
    public class EnterBuildingOnSelect : MonoBehaviour
    {
        [Header("Viewpoint binnen het gebouw")]
        public Transform interiorViewPoint;

        [Header("XR Origin / Rig")]
        public XROrigin xrOrigin;

        [Header("Instellingen")]
        public bool toggleOnSelect = true;

        private bool _inBuilding;
        private Vector3 _savedPosition;
        private Quaternion _savedRotation;
        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable _interactable;

        private void Awake()
        {
            _interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

            if (xrOrigin == null)
                xrOrigin = FindFirstObjectByType<XROrigin>();
        }

        private void OnEnable()
        {
            if (_interactable != null)
                _interactable.selectEntered.AddListener(OnSelectEntered);
            if (_interactable != null)
                _interactable.activated.AddListener(OnActivated);
        }

        private void OnDisable()
        {
            if (_interactable != null)
                _interactable.selectEntered.RemoveListener(OnSelectEntered);
            if (_interactable != null)
                _interactable.activated.RemoveListener(OnActivated);
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            TryToggleView();
        }

        private void OnActivated(ActivateEventArgs args)
        {
            TryToggleView();
        }

        private void TryToggleView()
        {
            if (xrOrigin == null || interiorViewPoint == null)
            {
                Debug.LogWarning("[EnterBuildingOnSelect] xrOrigin of interiorViewPoint is niet ingesteld.");
                return;
            }

            if (!toggleOnSelect || !_inBuilding)
                EnterBuilding();
            else
                ExitBuilding();
        }

        private void EnterBuilding()
        {
            _savedPosition = xrOrigin.transform.position;
            _savedRotation = xrOrigin.transform.rotation;

            xrOrigin.MoveCameraToWorldLocation(interiorViewPoint.position);
            xrOrigin.MatchOriginUpCameraForward(interiorViewPoint.up, interiorViewPoint.forward);

            _inBuilding = true;
        }

        private void ExitBuilding()
        {
            xrOrigin.MoveCameraToWorldLocation(_savedPosition);
            xrOrigin.MatchOriginUpCameraForward(Vector3.up, _savedRotation * Vector3.forward);

            _inBuilding = false;
        }
    }
}

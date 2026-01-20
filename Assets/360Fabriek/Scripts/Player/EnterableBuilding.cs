using UnityEngine;

namespace _360Fabriek.Player
{
    /// <summary>
    /// Markeert een gebouw als "betreedbaar" en definieert het viewpoint
    /// waar de XR-rig wordt neergezet wanneer de speler het gebouw binnengaat.
    /// </summary>
    public class EnterableBuilding : MonoBehaviour
    {
        [Header("Viewpoint inside this building")]
        [Tooltip("Transform waar de camera/rich naartoe wordt verplaatst als je dit gebouw 'binnengaat'.")]
        public Transform interiorViewPoint;
    }
}


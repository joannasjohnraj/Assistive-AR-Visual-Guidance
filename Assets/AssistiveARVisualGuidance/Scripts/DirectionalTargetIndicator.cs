using UnityEngine;
using UnityEngine.UI;

namespace AssistiveARVisualGuidance
{
    // Rotates a screen-space arrow so it behaves like an AR navigation cue toward the target.
    public class DirectionalTargetIndicator : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform target;
        [SerializeField] private RectTransform arrow;
        [SerializeField] private Text distanceText;
        [SerializeField] private float arrivalRadius = 1.2f;

        public void Initialize(Camera sourceCamera, Transform targetTransform, RectTransform arrowTransform, Text label)
        {
            playerCamera = sourceCamera;
            target = targetTransform;
            arrow = arrowTransform;
            distanceText = label;
        }

        private void Update()
        {
            if (playerCamera == null || target == null || arrow == null)
            {
                return;
            }

            Vector3 cameraToTarget = target.position - playerCamera.transform.position;
            Vector3 localDirection = playerCamera.transform.InverseTransformDirection(cameraToTarget.normalized);

            // Local X/Z gives a stable left-right bearing even when the camera is looking up or down.
            float angle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
            arrow.localRotation = Quaternion.Euler(0f, 0f, -angle);

            if (distanceText != null)
            {
                Vector3 cameraFloorPosition = playerCamera.transform.position;
                Vector3 targetFloorPosition = target.position;
                cameraFloorPosition.y = 0f;
                targetFloorPosition.y = 0f;

                // Show useful navigation distance: floor distance to the target area, not eye-to-center distance.
                float floorDistance = Vector3.Distance(cameraFloorPosition, targetFloorPosition);
                float remainingDistance = Mathf.Max(0f, floorDistance - arrivalRadius);
                distanceText.text = remainingDistance <= 0.05f ? "TARGET ARRIVED" : "TARGET " + remainingDistance.ToString("0.0") + " m";
            }
        }
    }
}

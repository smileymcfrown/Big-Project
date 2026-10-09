using DumplingKitchen.Core;
using UnityEngine;

namespace DumplingKitchen.Dumplings
{
    /// <summary>
    /// A simple third-person orbit camera. Lives on a child "CameraRig" object inside
    /// the dumpling prefab, with the Camera as its child. It positions itself in world
    /// space every LateUpdate, so the dumpling's own rotation does not drag it around.
    /// </summary>
    public sealed class DumplingCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 pivotOffset = new(0f, 0.5f, 0f);
        [SerializeField, Min(0.5f)] private float distance = 3f;
        [SerializeField] private Vector2 pitchLimits = new(-10f, 60f);

        [Header("Sensitivity")]
        [SerializeField] private float mouseDegreesPerPixel = 0.12f;
        [SerializeField] private float stickDegreesPerSecond = 180f;

        [Header("Collision")]
        [Tooltip("What the camera should not clip through (walls, counters).")]
        [SerializeField] private LayerMask obstructionLayers = 1; // Default layer
        [SerializeField] private float collisionRadius = 0.2f;

        private IDumplingInput _input;
        private float _yaw;
        private float _pitch = 15f;

        public Camera ViewCamera { get; private set; }

        private void Awake()
        {
            _input = GetComponentInParent<IDumplingInput>();
            ViewCamera = GetComponentInChildren<Camera>();
            CameraUtility.MakeDesktopOnly(ViewCamera);

            if (target == null)
                target = transform.parent;

            _yaw = target.eulerAngles.y;
        }

        private void LateUpdate()
        {
            Vector2 look = _input.Look;
            Vector2 delta = _input.LookIsPointerDelta
                ? look * mouseDegreesPerPixel
                : look * (stickDegreesPerSecond * Time.deltaTime);

            _yaw += delta.x;
            _pitch = Mathf.Clamp(_pitch - delta.y, pitchLimits.x, pitchLimits.y);

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 pivot = target.position + pivotOffset;
            Vector3 back = rotation * Vector3.back;

            // Pull the camera in if something is between it and the dumpling.
            float allowed = distance;
            if (Physics.SphereCast(pivot, collisionRadius, back, out RaycastHit hit, distance,
                    obstructionLayers, QueryTriggerInteraction.Ignore))
            {
                allowed = hit.distance;
            }

            transform.SetPositionAndRotation(pivot + back * allowed, rotation);
        }
    }
}

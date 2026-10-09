using UnityEngine;

namespace DumplingKitchen.Dumplings
{
    /// <summary>
    /// Moves a physics-based dumpling. Reads WHAT to do from IDumplingInput, decides
    /// HOW in FixedUpdate. Physics (Rigidbody) code belongs in FixedUpdate; input
    /// events can arrive any frame, so we buffer the jump request until then.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class DumplingMotor : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField, Min(0f)] private float moveSpeed = 4f;
        [SerializeField, Min(0f)] private float acceleration = 25f;
        [SerializeField, Min(0f)] private float turnSpeedDegrees = 720f;

        [Header("Jumping")]
        [SerializeField, Min(0f)] private float jumpVelocity = 5f;
        [Tooltip("Layers that count as ground. Do NOT include the Dumpling layer.")]
        [SerializeField] private LayerMask groundLayers = ~0;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [Tooltip("Ground check centre, relative to the dumpling's feet (its pivot).")]
        [SerializeField] private Vector3 groundCheckOffset = new(0f, 0.1f, 0f);

        [Header("References")]
        [Tooltip("Movement is relative to this (the dumpling's camera rig).")]
        [SerializeField] private Transform viewTransform;

        private Rigidbody _body;
        private IDumplingInput _input;
        private bool _jumpRequested;

        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _body.interpolation = RigidbodyInterpolation.Interpolate; // smooth camera follow
            _body.constraints = RigidbodyConstraints.FreezeRotation;  // we steer rotation ourselves

            _input = GetComponent<IDumplingInput>();
        }

        private void OnEnable() => _input.JumpPressed += HandleJumpPressed;
        private void OnDisable() => _input.JumpPressed -= HandleJumpPressed;

        private void HandleJumpPressed() => _jumpRequested = true;

        private void FixedUpdate()
        {
            IsGrounded = Physics.CheckSphere(
                transform.position + groundCheckOffset, groundCheckRadius,
                groundLayers, QueryTriggerInteraction.Ignore);

            Vector3 wish = WishDirection(_input.Move);
            Vector3 velocity = _body.linearVelocity; // Unity 6: "velocity" was renamed "linearVelocity"

            // Accelerate the horizontal part towards the target; leave gravity alone.
            Vector3 horizontal = new(velocity.x, 0f, velocity.z);
            horizontal = Vector3.MoveTowards(horizontal, wish * moveSpeed, acceleration * Time.fixedDeltaTime);
            velocity = new Vector3(horizontal.x, velocity.y, horizontal.z);

            if (_jumpRequested && IsGrounded)
                velocity.y = jumpVelocity;
            _jumpRequested = false;

            _body.linearVelocity = velocity;

            if (wish.sqrMagnitude > 0.001f)
            {
                Quaternion target = Quaternion.LookRotation(wish, Vector3.up);
                _body.MoveRotation(Quaternion.RotateTowards(_body.rotation, target, turnSpeedDegrees * Time.fixedDeltaTime));
            }
        }

        private Vector3 WishDirection(Vector2 move)
        {
            Transform view = viewTransform != null ? viewTransform : transform;
            Vector3 forward = Vector3.ProjectOnPlane(view.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(view.right, Vector3.up).normalized;

            Vector3 wish = forward * move.y + right * move.x;
            return wish.sqrMagnitude > 1f ? wish.normalized : wish;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
        }
    }
}

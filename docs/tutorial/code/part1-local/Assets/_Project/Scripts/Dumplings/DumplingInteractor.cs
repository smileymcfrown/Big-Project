using System;
using UnityEngine;

namespace DumplingKitchen.Dumplings
{
    /// <summary>
    /// Finds the nearest thing this dumpling can interact with and triggers it when the
    /// player presses Interact. Knows only IDumplingInteractable, never concrete classes.
    /// </summary>
    public sealed class DumplingInteractor : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float reach = 1.2f;
        [Tooltip("Put every sabotage target's collider on this layer.")]
        [SerializeField] private LayerMask interactableLayers;
        [SerializeField] private Transform reachOrigin;

        // Allocated once and reused: OverlapSphereNonAlloc writes into it, so scanning
        // every frame creates zero garbage for the garbage collector to clean up.
        private readonly Collider[] _hits = new Collider[16];

        private IDumplingInput _input;

        public IDumplingInteractable Current { get; private set; }
        public float Reach => reach;
        public Vector3 ReachOrigin => reachOrigin != null ? reachOrigin.position : transform.position;

        public event Action<IDumplingInteractable> CurrentChanged;
        public event Action<IDumplingInteractable> Interacted;

        private void Awake() => _input = GetComponent<IDumplingInput>();

        private void OnEnable()
        {
            if (_input != null)
                _input.InteractPressed += TryInteract;
        }

        private void OnDisable()
        {
            if (_input != null)
                _input.InteractPressed -= TryInteract;
            SetCurrent(null);
        }

        private void Update() => SetCurrent(FindBest());

        /// <summary>Public so other code (an AI, or the server in Part 2) can trigger it.</summary>
        public void TryInteract()
        {
            if (!IsAlive(Current) || !Current.CanInteract(this))
                return;

            Current.Interact(this);
            Interacted?.Invoke(Current);
        }

        /// <summary>
        /// Range check with some slack. In Part 2 the server uses this to reject
        /// cheating or laggy requests.
        /// </summary>
        public bool IsInReach(IDumplingInteractable interactable, float tolerance = 0.75f)
        {
            Vector3 point = interactable.InteractionPoint.position;
            return (point - ReachOrigin).sqrMagnitude <= (reach + tolerance) * (reach + tolerance);
        }

        private IDumplingInteractable FindBest()
        {
            Vector3 origin = ReachOrigin;
            int count = Physics.OverlapSphereNonAlloc(origin, reach, _hits, interactableLayers, QueryTriggerInteraction.Collide);

            IDumplingInteractable best = null;
            float bestDistance = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                IDumplingInteractable candidate = _hits[i].GetComponentInParent<IDumplingInteractable>();
                if (candidate == null || !candidate.CanInteract(this))
                    continue;

                float distance = (candidate.InteractionPoint.position - origin).sqrMagnitude;
                if (distance < bestDistance)
                {
                    best = candidate;
                    bestDistance = distance;
                }
            }

            return best;
        }

        private void SetCurrent(IDumplingInteractable next)
        {
            if (ReferenceEquals(next, Current))
                return;

            Current = next;
            CurrentChanged?.Invoke(next);
        }

        /// <summary>
        /// Gotcha: an interface reference to a DESTROYED MonoBehaviour is not "== null",
        /// because Unity's fake-null check only works on UnityEngine.Object variables.
        /// </summary>
        private static bool IsAlive(IDumplingInteractable interactable) =>
            interactable is UnityEngine.Object unityObject ? unityObject != null : interactable != null;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ReachOrigin, reach);
        }
    }
}

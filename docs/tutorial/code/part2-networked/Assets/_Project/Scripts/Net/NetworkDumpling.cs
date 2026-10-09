using DumplingKitchen.Dumplings;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DumplingKitchen.Net
{
    /// <summary>
    /// Network glue for the dumpling prefab (Dumpling_Net, a prefab VARIANT of the local
    /// Dumpling prefab). Every machine has a copy of every dumpling, but only the
    /// owner's copy reads input, runs the motor and renders a camera.
    ///
    /// Prefab setup: NetworkObject, NetworkTransform (Authority Mode = Owner),
    /// NetworkRigidbody, this component. In the prefab, DISABLE the PlayerInput and
    /// every component listed in ownerOnlyBehaviours, and the objects in ownerOnlyObjects.
    /// </summary>
    [RequireComponent(typeof(DumplingPlayer))]
    public sealed class NetworkDumpling : NetworkBehaviour
    {
        private const string KeyboardMouseScheme = "Keyboard&Mouse";

        // Server writes, everyone reads (the default permissions).
        public readonly NetworkVariable<int> PlayerIndex = new(-1);

        [SerializeField] private PlayerInput playerInput;
        [Tooltip("PlayerDumplingInput, DumplingMotor, DumplingInteractor, ...")]
        [SerializeField] private Behaviour[] ownerOnlyBehaviours;
        [Tooltip("Camera rig, prompt canvas, audio listener...")]
        [SerializeField] private GameObject[] ownerOnlyObjects;
        [SerializeField] private Color[] dumplingColours =
        {
            new(1.00f, 0.86f, 0.62f),
            new(0.62f, 0.85f, 0.55f),
            new(0.95f, 0.55f, 0.50f),
            new(0.70f, 0.62f, 0.90f)
        };

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                playerInput.enabled = true; // grab this PC's keyboard/mouse or gamepad
                if (playerInput.currentControlScheme == KeyboardMouseScheme)
                    Cursor.lockState = CursorLockMode.Locked;
            }

            foreach (Behaviour behaviour in ownerOnlyBehaviours)
                behaviour.enabled = IsOwner;

            foreach (GameObject ownerObject in ownerOnlyObjects)
                ownerObject.SetActive(IsOwner);

            PlayerIndex.OnValueChanged += HandlePlayerIndexChanged;
            HandlePlayerIndexChanged(-1, PlayerIndex.Value);
        }

        public override void OnNetworkDespawn()
        {
            PlayerIndex.OnValueChanged -= HandlePlayerIndexChanged;
            if (IsOwner)
                Cursor.lockState = CursorLockMode.None;
        }

        private void HandlePlayerIndexChanged(int previous, int index)
        {
            if (index >= 0)
                GetComponent<DumplingPlayer>().Initialise(index, dumplingColours[index % dumplingColours.Length]);
        }
    }
}

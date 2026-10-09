using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DumplingKitchen.Dumplings
{
    /// <summary>
    /// Adapts Unity's PlayerInput component to IDumplingInput. PlayerInput gives each
    /// local player their own copy of the actions and their own devices, which is
    /// exactly what split-screen needs.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public sealed class PlayerDumplingInput : MonoBehaviour, IDumplingInput
    {
        // Must match the names in Dumpling.inputactions.
        private const string KeyboardMouseScheme = "Keyboard&Mouse";

        private PlayerInput _playerInput;
        private InputAction _move;
        private InputAction _look;
        private InputAction _jump;
        private InputAction _interact;

        public Vector2 Move => _move.ReadValue<Vector2>();
        public Vector2 Look => _look.ReadValue<Vector2>();
        public bool LookIsPointerDelta => _playerInput.currentControlScheme == KeyboardMouseScheme;
        public string InteractHint => _interact.GetBindingDisplayString(group: _playerInput.currentControlScheme);

        public event Action JumpPressed;
        public event Action InteractPressed;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            InputActionAsset actions = _playerInput.actions;

            // throwIfNotFound: a typo in an action name fails loudly on the first frame
            // instead of silently doing nothing.
            _move = actions.FindAction("Dumpling/Move", throwIfNotFound: true);
            _look = actions.FindAction("Dumpling/Look", throwIfNotFound: true);
            _jump = actions.FindAction("Dumpling/Jump", throwIfNotFound: true);
            _interact = actions.FindAction("Dumpling/Interact", throwIfNotFound: true);
        }

        private void OnEnable()
        {
            _jump.performed += HandleJump;
            _interact.performed += HandleInteract;
        }

        private void OnDisable()
        {
            // Always unsubscribe what you subscribe. Forgetting this is the #1 source of
            // "MissingReferenceException" after a scene reload.
            _jump.performed -= HandleJump;
            _interact.performed -= HandleInteract;
        }

        private void HandleJump(InputAction.CallbackContext _) => JumpPressed?.Invoke();
        private void HandleInteract(InputAction.CallbackContext _) => InteractPressed?.Invoke();
    }
}

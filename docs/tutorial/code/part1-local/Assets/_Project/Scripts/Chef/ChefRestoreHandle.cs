using DumplingKitchen.Sabotage;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace DumplingKitchen.Chef
{
    /// <summary>
    /// Glue between the XR Interaction Toolkit and our game code: when the chef grabs
    /// (selects) this interactable, restore the sabotage target.
    ///
    /// Works with XR Simple Interactable (gas knob, light switch, plug, hat) and with
    /// XR Grab Interactable (a knocked-over pot). The sabotage classes never mention
    /// XR, so they stay usable by non-VR code and tests.
    /// </summary>
    [RequireComponent(typeof(XRBaseInteractable))]
    public sealed class ChefRestoreHandle : MonoBehaviour
    {
        [Tooltip("Leave empty to use the SabotageTarget on this object or a parent.")]
        [SerializeField] private SabotageTarget target;

        private XRBaseInteractable _interactable;

        private void Awake()
        {
            _interactable = GetComponent<XRBaseInteractable>();
            if (target == null)
                target = GetComponentInParent<SabotageTarget>();
        }

        private void OnEnable() => _interactable.selectEntered.AddListener(HandleSelectEntered);
        private void OnDisable() => _interactable.selectEntered.RemoveListener(HandleSelectEntered);

        private void HandleSelectEntered(SelectEnterEventArgs args)
        {
            if (target != null)
                target.Restore();
        }
    }
}

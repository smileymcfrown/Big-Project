using DumplingKitchen.Sabotage;
using UnityEngine;

namespace DumplingKitchen.Chef
{
    /// <summary>
    /// Sits on the chef's XR camera. When ANY ChefHat is knocked down, it shows the
    /// blindfold (a dark quad just in front of the eyes). It finds out through the
    /// static SabotageTarget events, so it doesn't need a reference to the hat. That
    /// matters in Part 2, where the hat lives on a networked avatar spawned at runtime.
    /// </summary>
    public sealed class ChefVisionBlocker : MonoBehaviour
    {
        [Tooltip("A child of the XR camera, on a layer the dumpling cameras do not render.")]
        [SerializeField] private GameObject blindfold;

        private void Awake() => blindfold.SetActive(false);

        private void OnEnable()
        {
            SabotageTarget.AnySabotaged += HandleSabotaged;
            SabotageTarget.AnyRestored += HandleRestored;
        }

        private void OnDisable()
        {
            SabotageTarget.AnySabotaged -= HandleSabotaged;
            SabotageTarget.AnyRestored -= HandleRestored;
        }

        private void HandleSabotaged(SabotageTarget target)
        {
            if (target is ChefHat)
                blindfold.SetActive(true);
        }

        private void HandleRestored(SabotageTarget target)
        {
            if (target is ChefHat)
                blindfold.SetActive(false);
        }
    }
}

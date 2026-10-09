using DumplingKitchen.Dumplings;
using TMPro;
using UnityEngine;

namespace DumplingKitchen.UI
{
    /// <summary>
    /// "[E] Turn off Gas" prompt for ONE dumpling. Lives on that dumpling's own canvas
    /// (Screen Space - Camera, using the dumpling's camera) so it appears in the right
    /// split-screen slice.
    /// </summary>
    public sealed class InteractionPromptDisplay : MonoBehaviour
    {
        [SerializeField] private DumplingInteractor interactor;
        [SerializeField] private GameObject promptRoot;
        [SerializeField] private TMP_Text promptText;

        private IDumplingInput _input;

        private void Awake() => _input = interactor.GetComponent<IDumplingInput>();

        private void OnEnable()
        {
            interactor.CurrentChanged += Show;
            Show(interactor.Current);
        }

        private void OnDisable() => interactor.CurrentChanged -= Show;

        private void Show(IDumplingInteractable interactable)
        {
            bool visible = interactable != null;
            promptRoot.SetActive(visible);
            if (visible)
                promptText.text = $"[{_input.InteractHint}] {interactable.Prompt}";
        }
    }
}

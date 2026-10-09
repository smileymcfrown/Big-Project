using UnityEngine;

namespace DumplingKitchen.Dumplings
{
    /// <summary>
    /// Anything a dumpling can walk up to and press "Interact" on. The interactor only
    /// knows this interface, so the Dumplings code never depends on the sabotage code.
    /// </summary>
    public interface IDumplingInteractable
    {
        /// <summary>Text for the on-screen prompt, e.g. "Turn off Gas".</summary>
        string Prompt { get; }

        /// <summary>Where "near enough" is measured to.</summary>
        Transform InteractionPoint { get; }

        bool CanInteract(DumplingInteractor dumpling);
        void Interact(DumplingInteractor dumpling);
    }
}

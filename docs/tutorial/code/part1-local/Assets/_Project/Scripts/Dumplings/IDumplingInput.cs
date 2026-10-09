using System;
using UnityEngine;

namespace DumplingKitchen.Dumplings
{
    /// <summary>
    /// What a dumpling "wants" to do this frame, with no idea WHERE that came from.
    /// Today it is a keyboard or gamepad (PlayerDumplingInput). Tomorrow it could be
    /// an AI dumpling, a replay, or a network message, and the motor, camera and
    /// interactor will not need to change.
    /// </summary>
    public interface IDumplingInput
    {
        Vector2 Move { get; }
        Vector2 Look { get; }

        /// <summary>True when Look is a mouse delta (pixels) rather than a stick rate.</summary>
        bool LookIsPointerDelta { get; }

        /// <summary>Human-readable key/button for interacting, e.g. "E" or "Button West".</summary>
        string InteractHint { get; }

        event Action JumpPressed;
        event Action InteractPressed;
    }
}

using UnityEngine;

namespace DumplingKitchen.Dumplings
{
    /// <summary>
    /// The dumpling prefab's "front door". Spawners talk to this one component instead
    /// of poking at the motor, camera and renderer separately.
    /// </summary>
    public sealed class DumplingPlayer : MonoBehaviour
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor"); // URP Lit colour

        [SerializeField] private Renderer bodyRenderer;
        [SerializeField] private DumplingCamera cameraRig;

        private MaterialPropertyBlock _propertyBlock;

        public int PlayerIndex { get; private set; }
        public Color Colour { get; private set; }
        public Camera ViewCamera => cameraRig != null ? cameraRig.ViewCamera : null;

        public void Initialise(int playerIndex, Color colour)
        {
            PlayerIndex = playerIndex;
            Colour = colour;
            name = $"Dumpling {playerIndex + 1}";

            if (bodyRenderer == null)
                return;

            // A property block tints this one renderer without creating a new material.
            _propertyBlock ??= new MaterialPropertyBlock();
            bodyRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(BaseColorId, colour);
            bodyRenderer.SetPropertyBlock(_propertyBlock);
        }
    }
}

using DumplingKitchen.Chef;
using Unity.Netcode;
using UnityEngine;

namespace DumplingKitchen.Net
{
    /// <summary>
    /// What the dumplings SEE of the chef: a head (with the ChefHat on it) and two hands.
    /// On the host it copies the real tracked XR rig every frame; NetworkTransforms on
    /// the head and hands (Authority Mode = Server) send that to every dumpling.
    /// </summary>
    public sealed class ChefAvatar : NetworkBehaviour
    {
        [SerializeField] private Transform head;
        [SerializeField] private Transform leftHand;
        [SerializeField] private Transform rightHand;

        [Tooltip("Meshes the chef shouldn't see from inside their own head (face, hat).")]
        [SerializeField] private Renderer[] hiddenFromChef;

        private ChefRigAnchors _rig;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;

            _rig = ChefRigAnchors.Instance;
            if (_rig == null)
                Debug.LogError("ChefAvatar: no ChefRigAnchors on the XR Origin in this scene.", this);

            foreach (Renderer meshRenderer in hiddenFromChef)
                meshRenderer.enabled = false;
        }

        // LateUpdate: after XR tracking and XRI have moved the rig this frame.
        private void LateUpdate()
        {
            if (!IsServer || _rig == null)
                return;

            Follow(head, _rig.Head);
            Follow(leftHand, _rig.LeftHand);
            Follow(rightHand, _rig.RightHand);
        }

        private static void Follow(Transform avatarPart, Transform tracked)
        {
            if (avatarPart != null && tracked != null)
                avatarPart.SetPositionAndRotation(tracked.position, tracked.rotation);
        }
    }
}

using DumplingKitchen.Net;
using UnityEngine;

namespace DumplingKitchen.Chef
{
    /// <summary>
    /// Sits on the XR Origin in the Kitchen scene and points at the tracked head and
    /// hands, so the networked ChefAvatar can copy them. Read-only from outside.
    ///
    /// On dumpling PCs (not the authority) the whole XR rig switches itself off.
    /// </summary>
    public sealed class ChefRigAnchors : MonoBehaviour
    {
        [SerializeField] private Transform head;
        [SerializeField] private Transform leftHand;
        [SerializeField] private Transform rightHand;

        public static ChefRigAnchors Instance { get; private set; }

        public Transform Head => head;
        public Transform LeftHand => leftHand;
        public Transform RightHand => rightHand;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => Instance = null;

        private void Awake()
        {
            if (!Authority.IsAuthority)
            {
                gameObject.SetActive(false); // dumpling PC: no VR rig here
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}

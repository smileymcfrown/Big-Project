using DumplingKitchen.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace DumplingKitchen.Local
{
    /// <summary>
    /// One PC, two audiences. The headset shows the chef's XR camera; the monitor shows
    /// a lobby camera until dumplings join, then their split-screen cameras.
    /// </summary>
    public sealed class DesktopViewSetup : MonoBehaviour
    {
        [SerializeField] private Camera lobbyCamera;
        [SerializeField] private PlayerInputManager playerInputManager;
        [Tooltip("Stop Unity copying the headset image to the monitor (saves GPU time).")]
        [SerializeField] private bool hideHeadsetMirror = true;

        private void OnEnable()
        {
            playerInputManager.onPlayerJoined += HandlePlayersChanged;
            playerInputManager.onPlayerLeft += HandlePlayersChanged;
        }

        private void OnDisable()
        {
            playerInputManager.onPlayerJoined -= HandlePlayersChanged;
            playerInputManager.onPlayerLeft -= HandlePlayersChanged;
        }

        private void Start()
        {
            CameraUtility.MakeDesktopOnly(lobbyCamera);

            // Done in Start so XR has finished initialising.
            if (hideHeadsetMirror)
                XRSettings.gameViewRenderMode = GameViewRenderMode.None;

            Refresh();
        }

        private void HandlePlayersChanged(PlayerInput _) => Refresh();

        private void Refresh() => lobbyCamera.enabled = playerInputManager.playerCount == 0;
    }
}

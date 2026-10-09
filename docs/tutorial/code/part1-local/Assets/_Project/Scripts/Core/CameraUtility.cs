using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace DumplingKitchen.Core
{
    public static class CameraUtility
    {
        /// <summary>
        /// Makes a camera render to the desktop monitor only, never into the headset.
        /// Used for dumpling cameras and the lobby camera so the VR chef and the PC
        /// players can share one computer.
        /// </summary>
        public static void MakeDesktopOnly(Camera camera)
        {
            camera.stereoTargetEye = StereoTargetEyeMask.None;

            if (camera.TryGetComponent(out UniversalAdditionalCameraData urpData))
                urpData.allowXRRendering = false;
        }
    }
}

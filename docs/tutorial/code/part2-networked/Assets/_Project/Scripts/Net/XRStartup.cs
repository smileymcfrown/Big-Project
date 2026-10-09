using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

namespace DumplingKitchen.Net
{
    /// <summary>
    /// Starts VR only for the player who picks "Chef". Dumpling PCs never start XR, so
    /// they don't launch SteamVR or complain about a missing headset.
    /// Requires: Project Settings > XR Plug-in Management > "Initialize XR on Startup" OFF.
    /// </summary>
    public static class XRStartup
    {
        private static XRManagerSettings Manager =>
            XRGeneralSettings.Instance != null ? XRGeneralSettings.Instance.Manager : null;

        public static bool IsRunning =>
            Manager != null && Manager.isInitializationComplete && Manager.activeLoader != null;

        public static IEnumerator StartXR()
        {
            if (Manager == null)
            {
                Debug.LogError("XR Plug-in Management is not configured for this platform.");
                yield break;
            }

            if (IsRunning)
                yield break;

            yield return Manager.InitializeLoader();

            if (Manager.activeLoader == null)
            {
                Debug.LogWarning("XR did not start. Is the Quest connected and Meta Horizon Link or SteamVR running?");
                yield break;
            }

            Manager.StartSubsystems();
        }

        public static void StopXR()
        {
            if (Manager == null || !Manager.isInitializationComplete)
                return;

            Manager.StopSubsystems();
            Manager.DeinitializeLoader();
        }
    }
}

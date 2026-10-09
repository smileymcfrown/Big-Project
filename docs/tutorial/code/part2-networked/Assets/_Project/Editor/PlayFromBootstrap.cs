using UnityEditor;
using UnityEditor.SceneManagement;

namespace DumplingKitchen.EditorTools
{
    /// <summary>
    /// Networked games break if you press Play in the Kitchen scene directly (there is
    /// no NetworkManager yet). This makes Play always start from Bootstrap, whichever
    /// scene you are editing. Toggle it from the menu: Dumpling Kitchen > Play From Bootstrap.
    /// </summary>
    [InitializeOnLoad]
    public static class PlayFromBootstrap
    {
        private const string BootstrapPath = "Assets/_Project/Scenes/Bootstrap.unity";
        private const string MenuPath = "Dumpling Kitchen/Play From Bootstrap";
        private const string PrefKey = "DumplingKitchen.PlayFromBootstrap";

        static PlayFromBootstrap() => Apply();

        private static bool Enabled
        {
            get => EditorPrefs.GetBool(PrefKey, true);
            set => EditorPrefs.SetBool(PrefKey, value);
        }

        [MenuItem(MenuPath)]
        private static void Toggle()
        {
            Enabled = !Enabled;
            Apply();
        }

        [MenuItem(MenuPath, true)]
        private static bool ToggleValidate()
        {
            Menu.SetChecked(MenuPath, Enabled);
            return true;
        }

        private static void Apply()
        {
            EditorSceneManager.playModeStartScene = Enabled
                ? AssetDatabase.LoadAssetAtPath<SceneAsset>(BootstrapPath)
                : null;
        }
    }
}

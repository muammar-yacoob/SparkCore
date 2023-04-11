using SparkCore.Editor.Setup;
using UnityEditor;
using UnityEngine;

namespace SparkCore.Editor.Menus
{
    public static class ToolsMenu
    {
        [MenuItem("Tools/Setup/Create Default Folders", false, 11)]
        public static void CreateDefaultFolders() => Folders.CreateDefaultFolders();

        [MenuItem("Tools/Setup/Install Core Dependencies", false, 12)]
        public static void InstallCoreDependencies() => Packages.InstallCoreDependencies();

        [MenuItem("Tools/Setup/Unity Packages/Input System", false, 31)]
        public static void InstallUnity_InputSystem() => Packages.InstallUnityPackage("inputsystem");

        [MenuItem("Tools/Setup/Unity Packages/Post Processing", false, 32)]
        public static void InstallUnity_PostProcessing() => Packages.InstallUnityPackage("postprocessing");

        [MenuItem("Tools/Setup/Unity Packages/Cinemachine", false, 33)]
        public static void InstallUnity_Cinemachine() => Packages.InstallUnityPackage("cinemachine");

        [MenuItem("Tools/Utils/Find Missing Scripts in project", false, 51)]
        public static void Find_Prefabs_Missing_Scripts_In_Project() => Utils.PrefabsWithMissingScripts.FindInProject();
        [MenuItem("Tools/Utils/Find Missing Scripts in Hierarchy", false, 52)]
        public static void Find_Objects_Missing_Scripts_In_Hierarchy() => Utils.PrefabsWithMissingScripts.FindInHierarchy();

        [MenuItem("Tools/About", false, 1011)]
        public static void MenuHelp() => Application.OpenURL($"https://github.com/muammar-yacoob/SparkCore");
    }
}
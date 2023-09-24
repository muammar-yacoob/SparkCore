using SparkCore.Editor.Setup;
using SparkCore.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace SparkCore.Editor.Menus
{
    public static class ToolsMenu
    {
        [MenuItem("Spark Core/Setup/Create Default Folders",false,11)]
        public static void CreateDefaultFolders()=> Folders.CreateDefaultFolders();

        [MenuItem("Spark Core/Setup/Install Core Dependencies",false,12)]
        public static void InstallCoreDependencies() => Packages.InstallCoreDependencies();
        
        [MenuItem("Spark Core/Setup/Unity Packages/Input System",false,31)]
        public static void InstallUnity_InputSystem() => Packages.Install("com.unity.inputsystem");
        
        [MenuItem("Spark Core/Setup/Unity Packages/XR",false,32)]
        public static void InstallUnity_XR() => Packages.InstallXR();
        
        [MenuItem("Spark Core/Setup/Unity Packages/Cinemachine",false,33)]
        public static void InstallUnity_Cinemachine() => Packages.Install("com.unity.cinemachine");

        [MenuItem("Tools/Find missing scripts in Hierarchy", false, 41)]
        public static void FindMissingInHierarchy() => PrefabsWithMissingScripts.FindInHierarchy();

        [MenuItem("Tools/Find missing scripts in Project", false, 42)]
        public static void FindMissingInProject() => PrefabsWithMissingScripts.FindInProject();

        [MenuItem("Spark Core/About", false, 1011)]
        public static void MenuHelp() => Application.OpenURL($"https://github.com/Born-Studios/BornCore");
    }
}
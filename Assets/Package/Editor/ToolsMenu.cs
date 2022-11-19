using UnityEditor;
using UnityEngine;
using static System.IO.Path;
using static System.IO.Directory;
using static UnityEditor.AssetDatabase;

namespace SparkCore.Editor
{
    public static class ToolsMenu
    {
        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            string[] dirs = { "Artwork", "Scenes", "Scripts", "Animations", "Prefabs", "StreamingAssets", "Audio" };
            CreateDirs("_Project", dirs);
            Refresh();
        }

        private static void CreateDirs(string root, string[] dirs)
        {
            var fullPath = Combine(Application.dataPath, root);
            foreach (var newDir in dirs)
            {
                CreateDirectory(Combine(fullPath, newDir));
            }
        }
    }
}
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SparkCore.Editor.Setup
{
    public static class Folders
    {
        public static void CreateDefaultFolders()
        {
            string root = "_Project";
            string[] dirs = { "Artwork", "Scenes", "Scripts", "Animations", "Prefabs", "StreamingAssets", "Audio" };
            
            var fullPath = Path.Combine(Application.dataPath, root);
            foreach (var newDir in dirs)
            {
                Directory.CreateDirectory(Path.Combine(fullPath, newDir));
            }
            AssetDatabase.Refresh();
        }
    }
}
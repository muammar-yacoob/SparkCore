using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SparkCore.Editor.Utils
{
    public static class PrefabsWithMissingScripts 
    {
        public static void FindInHierarchy()
        {
            var gameObjects = GameObject.FindObjectsOfType<GameObject>(true);
            foreach (var gameObject in gameObjects)
            {
                var comps = gameObject.GetComponentsInChildren<Component>();
                
                foreach (var comp in comps)
                {
                    if (comp == null)
                    {
                        Debug.Log($"{gameObject.name} has a missing script.",gameObject);
                    }
                }
            }
        }

        public static void FindInProject()
        {
            var prefabPaths = AssetDatabase.GetAllAssetPaths()
                .Where(path => path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase));
            foreach (var path in prefabPaths)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var comps = prefab.GetComponentsInChildren<Component>();
                
                foreach (var comp in comps)
                {
                    if (comp == null)
                    {
                        Debug.Log($"{prefab.name} has a missing script.",prefab);
                    }
                }
            }
        }
    }
}
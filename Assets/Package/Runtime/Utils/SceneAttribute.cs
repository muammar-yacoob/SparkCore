
using UnityEditor;
using UnityEngine;

namespace SparkGames.SparkCore.Utils
{
    public class SceneAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class SceneDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "[Scene] attribute can only be used with string fields");
                return;
            }

            var sceneObject = string.IsNullOrWhiteSpace(property.stringValue) 
                ? null 
                : AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue) 
                  ?? GetBuildSettingsSceneObject(property.stringValue);

            if (sceneObject == null && !string.IsNullOrWhiteSpace(property.stringValue))
            {
                Debug.LogError($"Scene not found: {property.stringValue} in {property.propertyPath}");
            }

            var scene = EditorGUI.ObjectField(position, label, sceneObject, typeof(SceneAsset), true) as SceneAsset;
            property.stringValue = scene != null ? AssetDatabase.GetAssetPath(scene) : "";
        }

        private SceneAsset GetBuildSettingsSceneObject(string sceneName)
        {
            var buildScene = System.Array.Find(EditorBuildSettings.scenes, 
                scene => AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path)?.name == sceneName);
            return buildScene != null ? AssetDatabase.LoadAssetAtPath<SceneAsset>(buildScene.path) : null;
        }
    }
#endif
} 

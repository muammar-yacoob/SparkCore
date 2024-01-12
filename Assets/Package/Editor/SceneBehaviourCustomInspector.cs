using SparkCore.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace SparkCore.Editor
{
    [CustomEditor(typeof(InjectableMonoBehaviour), true)]
    public class SceneBehaviourCustomInspector : UnityEditor.Editor
    {
        private GUISkin skin;
        private Texture2D iconHeader;
        private InjectableMonoBehaviour _injectableMono;
        private GameObject activeObject;
        private string headerText;
        private Rect headerRect;
        private Texture2D headerTexture;
        private float headerTexScale = 0.09f;
        private bool showListeners;
        private bool showPublishers;

        void OnEnable()
        {
            skin = Resources.Load<GUISkin>("guiStyles/Default");
            headerTexture = Resources.Load<Texture2D>("icons/spark");
            _injectableMono = target as InjectableMonoBehaviour;
        }

        public override void OnInspectorGUI()
        {
            //Header
            GUILayout.Label($"SparkCore", skin.GetStyle("CustomHeader"));
            GUILayout.Label($"Scene Behaviour", skin.GetStyle("CustomH1"));
            headerRect = new Rect(Screen.width - headerTexture.width * headerTexScale, 0,
                headerTexture.width * headerTexScale, headerTexture.height * headerTexScale);
            GUI.DrawTexture(headerRect, headerTexture);

            base.DrawDefaultInspector();
            DrawGUI();
        }

        private void DrawGUI()
        {
            EditorGUILayout.Space(20);
            if (!EditorApplication.isPlaying && _injectableMono is InjectableMonoBehaviour) return;

            showPublishers = EditorGUILayout.BeginFoldoutHeaderGroup(showPublishers, "Published Events");
            if (showPublishers)
            {
                for (int i = 1; i < 5; i++)
                {
                    if (GUILayout.Button($"Fire event {i}"))
                    {
                        Debug.Log("Firing Event...");
                    }
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.Space(5);
            showListeners = EditorGUILayout.BeginFoldoutHeaderGroup(showListeners, "Event Listeners");
            if (showListeners)
            {
                for (int i = 1; i < 5; i++)
                {
                    GUILayout.Label($"Event {i}",skin.GetStyle("CustomLink"));
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.Space(5);
        }
    }
}
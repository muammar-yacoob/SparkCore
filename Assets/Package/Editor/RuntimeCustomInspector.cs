using System;
using System.Linq;
using SparkCore.Runtime.Injection;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using static System.Threading.Thread;

namespace SparkCore.Editor
{
    [CustomEditor(typeof(RuntimeInjector), true)]
    public class RuntimeCustomInspector : UnityEditor.Editor
    {
        private GUISkin skin;
        private Texture2D iconHeader;
        private RuntimeInjector _runtimeInjector;
        private GameObject activeObject;
        private string headerText;
        private Rect headerRect;
        private Texture2D headerTexture;
        private readonly float headerTexScale = 0.09f;
        private bool showListeners;
        private bool showPublishers;
        private string sparkCoreVersion = string.Empty;
        private float headerXPadding=20;

        void OnEnable()
        {
            skin = Resources.Load<GUISkin>("guiStyles/Default");
            headerTexture = Resources.Load<Texture2D>("icons/spark");

            if (!String.IsNullOrEmpty(sparkCoreVersion)) return;
            // Fetch version from Unity package
            ListRequest listRequest = Client.List();
            while (!listRequest.IsCompleted) Sleep(100);
            var package = listRequest.Result.FirstOrDefault(p => p.name == "com.sparkgames.sparkcore");
            if (package != null)
            {
                sparkCoreVersion = package.version;
            }
        }

        public override void OnInspectorGUI()
        {
            activeObject = Selection.activeGameObject;

            //Header
            GUILayout.Label($"SparkCore", skin.GetStyle("CustomHeader"));
            GUILayout.Space(20);
            GUILayout.Label($"Runtime Injector", skin.GetStyle("CustomH1"));
            headerRect = new Rect(Screen.width - headerTexture.width * headerTexScale-headerXPadding, 0,
                headerTexture.width * headerTexScale, headerTexture.height * headerTexScale);
            GUI.DrawTexture(headerRect, headerTexture);

            base.DrawDefaultInspector();
            DrawGUI();
        }

        private void DrawGUI()
        {
            GUILayout.Space(5);
            if (activeObject != null && !String.IsNullOrEmpty(sparkCoreVersion)) GUILayout.Label($"{activeObject.name} Ver: {sparkCoreVersion}", skin.GetStyle("CustomLabel"));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using SparkCore.Runtime.Core;
using SparkCore.Runtime.Injection;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SparkCore.Editor
{
    public class SparkCoreWindow : EditorWindow
    {
        private Vector2 scrollPos;
        private bool showFields;
        private GUIStyle headerStyle;
        private GUISkin skin;
        private static Texture2D iconTexture;
        private static Rect iconRect;
        private static Rect headerRect;
        private static Rect bodyRect;
        private static float headerScale = 0.09f;
        private GUIStyle linkStyle;
        private GUIStyle h1Style;
        private GUIStyle labelStyle;

        private static List<FieldDescriptor> injectedFieldsList = new();
        private static List<SceneEventDescriptor> sceneEventsList = new();

        private static SparkCoreWindow window;
        private bool showTypes;
        private static Rect hoverLayer;
        private static int currentTab;
        private string[] tabNames = { "Injection View", "Scene Events" };
        private static Rect bottomRect;
        private string helpMsg;
        private bool showEvents;
        private List<Delegate> sceneEvents;
        private bool showFullEventName;

        #region Menus

        [MenuItem("Spark Core/Injection Manager &B", false, 11)]
        public static void OpenInjectionWindow() => OpenMainWindow(0);

        [MenuItem("Spark Core/Injection Manager &I", true, 11)]
        private static bool Validate_OpenInjectionWindow() => !Application.isPlaying;

        [MenuItem("Spark Core/Scene Events", false, 12)]
        public static void OpenSceneWindow() => OpenMainWindow(1);

        [MenuItem("Spark Core/Scene Events", true, 12)]
        private static bool Validate_OpenSceneWindow() => Application.isPlaying;

        [MenuItem("Spark Core/Help", false, 30)]
        public static void MenuHelp() => Application.OpenURL($"https://github.com/Spark-Studios/SparkCore#readme");

        #endregion

        private static void OpenMainWindow(int tabIndex)
        {
            string windowTitle = "Spark Core";
            if (window == null)
            {
                window = GetWindow<SparkCoreWindow>(windowTitle, typeof(SceneView));
            }

            //Window Setup
            window.titleContent = new GUIContent(windowTitle,
                EditorGUIUtility.ObjectContent(CreateInstance<SparkCoreWindow>(), typeof(SparkCoreWindow)).image);

            window.minSize = new Vector2(350, 350);
            window.maxSize = new Vector2(1200, 500);
            window.Show();
            currentTab = tabIndex;
            window.Repaint();
        }


        private void OnEnable()
        {
            LoadTextures();
            Repaint();
        }

        private void LoadTextures()
        {
            iconTexture = Resources.Load<Texture2D>("icons/spark");
            skin = Resources.Load<GUISkin>("guiStyles/Default");
            headerStyle = skin.GetStyle("CustomHeader");
            linkStyle = skin.GetStyle("CustomLink");
            h1Style = skin.GetStyle("CustomH1");
            labelStyle = skin.GetStyle("CustomLabel");
        }

        private static void DrawRects()
        {
            float pad = 5;
            iconRect = new Rect((Screen.width - iconTexture.width * headerScale) - pad, 0,
                iconTexture.width * headerScale, iconTexture.height * headerScale);
            headerRect = new Rect(0, 0, Screen.width, 40);
            //iconRect = new Rect( pad, pad, iconTexture.width * headerScale, iconTexture.height * headerScale);

            float bottomHeight = 60;
            bottomRect = new Rect(0, Screen.height - bottomHeight, Screen.width, bottomHeight);
            float screenOffset = 2f;
            hoverLayer = new Rect(Screen.width - (Screen.width / screenOffset), 50, (Screen.width / screenOffset),
                Screen.height / 2);
        }

        private void DrawHeader()
        {
            DrawBox(headerRect, new Color(0.1f, 0.1f, 0.1f));
            GUI.DrawTexture(iconRect, iconTexture);
            GUILayout.Space(5);
            GUILayout.Label("Spark Core", headerStyle);
        }

        void DrawBox(Rect rect, Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            GUI.skin.box.normal.background = texture;
            GUI.Box(rect, GUIContent.none);
        }

        private void OnGUI()
        {
            DrawRects();
            DrawHeader();
            GUILayout.Space(10);

            DrawTabs();

            if (Application.isPlaying) ShowNotification(new GUIContent($"Running..."), 0.3f);

            GUILayout.BeginArea(bottomRect);
            EditorGUILayout.HelpBox(helpMsg, MessageType.Info);
            GUILayout.EndArea();
        }


        private void DrawTabs()
        {
            currentTab = GUILayout.Toolbar(currentTab, tabNames);
            switch (currentTab)
            {
                case 0:
                    helpMsg = $"A total of {injectedFieldsList.Count} fields were detected.";
                    DrawInjectionTab();
                    break;
                case 1:
                    helpMsg = sceneEventsList.Count > 0
                        ? $"A total of {sceneEventsList.Count} events were detected."
                        : $"Click Load Scene Events to refresh.";
                    DrawSceneEventsTab();
                    break;
                default:
                    //Debug.Log(currentTab);
                    break;
            }
        }

        private void DrawSceneEventsTab()
        {
            GUILayout.Space(10);
            if (!EditorApplication.isPlaying)
            {
                helpMsg = "List can only be loaded while playing.";
                return;
            }

            DrawSceneEvents();
            if (GUILayout.Button("Load Scene Events"))
            {
                LoadSceneEvents();
            }

            GUILayout.Space(20);
            GUILayout.Label("Settings", h1Style);
            showFullEventName = GUILayout.Toggle(showFullEventName, "Show Full Path");
        }

        private void DrawInjectionTab()
        {
            DrawFields();
            GUILayout.Space(10);

            if (EditorApplication.isPlaying)
            {
                helpMsg = "Refreshing is disabled while playing.";
                return;
            }

            if (GUILayout.Button("Rescan Assembly"))
            {
                LoadFields();
                DrawFields();
                Repaint();
            }

            GUILayout.Space(20);
            GUILayout.Label("Settings", h1Style);
            showTypes = GUILayout.Toggle(showTypes, "Show Types");
            ConditionalRepaint();
        }

        private void ConditionalRepaint()
        {
            if (!showFields) return;
            if (mouseOverWindow == null) return;
            if (mouseOverWindow.GetType() != (typeof(SparkCoreWindow))) return;
            if (currentTab != 0) return;
            //if (!hoverLayer.Contains(Event.current.mousePosition))return; //Doesn't work well

            Repaint(); //expensive but required for responsive hover style to work

            //debug
            //DrawBox(hoverLayer,new Color(0,0,0,0.3f));
        }

        private void DrawFields()
        {
            GUILayout.Space(20);
            showFields =
                EditorGUILayout.BeginFoldoutHeaderGroup(showFields, $"{(showFields ? "-" : "+")} Fields", h1Style);
            if (showFields)
            {
                if (injectedFieldsList.Count < 1) LoadFields();

                var maxSectionHeight = Mathf.Min(injectedFieldsList.Count * 30, Screen.height / 3);
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true),
                    GUILayout.Height(maxSectionHeight));
                foreach (var item in injectedFieldsList)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Space(10);
                    GUILayout.Label($"{(showTypes ? item.Type.Name : "")} {item.Name} ➜", labelStyle);

                    GUILayout.Space(10);
                    string caption = $"{item.ParentType.Name}";
                    if (GUILayout.Button(caption, linkStyle))
                    {
                        var path = Application.dataPath + "/" + item.FileName;
                        path = path.Replace("Assets/Assets/", "Assets/");
                        Debug.Log($"Opening {path}");
                        try
                        {
                            // string cmd = "code";
                            // string args = $"--goto \"{path}\":{3}";
                            // Process.Start(cmd, args);
                            
                            Process.Start($"\"{path}\"");
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning($"Failed to open {path}. Error: {e.Message}");
                        }
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }


        private void LoadFields()
        {
            var ass = GetDefualtAssembly();
            IEnumerable<Type> types = ass.GetTypes();
            var myTypes =  types.Where(type => type.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

            injectedFieldsList.Clear();


            foreach (Type type in myTypes)
            {
                foreach (FieldInfo field in type.GetFields(flags))
                {
                    if (field.GetCustomAttribute<Inject>() != null)
                    {
                        // if (field.FieldType.IsAssignableFrom(typeof(ISceneEventHistory)) ||
                        //     field.FieldType.IsAssignableFrom(typeof(ISceneEventProvider)))
                        //     continue;

                        var results = AssetDatabase.FindAssets((type.Name));
                        var g = results.FirstOrDefault();
                        var filePath = AssetDatabase.GUIDToAssetPath(g);
                        var fld = new FieldDescriptor(field.Name, field.FieldType, filePath, type);
                        injectedFieldsList.Add(fld);
                        //Debug.Log(fld.Name);
                    }
                }
            }
        }

        public void LoadSceneEvents()
        {
                sceneEvents = EventManager.Instance.GetSubscribers<SceneEvent>();
            foreach (var sceneEvent in sceneEvents)
            {
                Debug.Log(sceneEvent.Method.DeclaringType.Name);
            }
            sceneEventsList.Clear();
            foreach (var sceneEvent in sceneEvents)
            {
                foreach (Action<SceneEvent> handler in sceneEvent.GetInvocationList())
                {
                    Type eventName = sceneEvent.Method.DeclaringType;
                    string eventSubscriberNamespace = handler.Target.GetType().Namespace;
                    string eventSubscriberClass = handler.Target.GetType().Name;
                    string eventSubscriberHandler = handler.Method.Name;

                    //Debug.Log($"{eventName}: {eventSubscriberNamespace}.{eventSubscriberClass}.{eventSubscriberHandler}");

                    var eventDesc = new SceneEventDescriptor(sceneEvent, handler);
                    sceneEventsList.Add(eventDesc);
                }
            }
        }

        private void DrawSceneEvents()
        {
            GUILayout.Space(20);
            showEvents =
                EditorGUILayout.BeginFoldoutHeaderGroup(showEvents, $"{(showEvents ? "-" : "+")} Events", h1Style);
            if (showEvents)
            {
                if (sceneEvents == null) return;
                if (sceneEvents.Count < 1) LoadSceneEvents();

                var maxSectionHeight = Mathf.Min(sceneEvents.Count * 30, Screen.height / 1.1f);
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true),
                    GUILayout.Height(maxSectionHeight));
                foreach (var item in sceneEventsList)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Space(10);


                    string eventName = item.SceneEvent.Method.DeclaringType.Name;
                    string eventSubscriberNamespace = item.Handler.Target.GetType().Namespace;
                    string eventSubscriberClass = item.Handler.Target.GetType().Name;
                    string eventSubscriberHandler = item.Handler.Method.Name;

                    GUILayout.Label($"{(showFullEventName ? $"{eventSubscriberNamespace}." : "")}" +
                                    $"{eventSubscriberClass}{(showFullEventName ? $".{eventSubscriberHandler}" : "")} ➜",
                        labelStyle);

                    GUILayout.Space(10);
                    string caption = $"{eventName}";

                    var results = AssetDatabase.FindAssets((eventSubscriberClass));
                    var g = results.FirstOrDefault();
                    var filePath = AssetDatabase.GUIDToAssetPath(g);

                    if (GUILayout.Button(caption, linkStyle))
                    {
                        var path = Application.dataPath + "/" + filePath;
                        path = path.Replace("Assets/Assets/", "Assets/");
                        Debug.Log($"Opening {path}");
                        try
                        {
                            Process.Start($"{path}");
                        }
                        catch (Exception e)
                        {
                            if (e.Message.Contains("cancelled"))
                            {
                                Debug.LogWarning("Operation was cancelled by the user.");
                            }
                            else
                            {
                                Debug.LogWarning($"Failed to open {path}. Error: {e.Message}");
                            }
                        }
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        //Helpers
        static Assembly GetDefualtAssembly() => GetAssemblyByName("Assembly-CSharp");

        static Assembly GetAssemblyByName(string name) => AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(assembly => assembly.GetName().Name == name);

        static IEnumerable<Type> GetTypesInDefaultAssembly<T>() => GetDefualtAssembly().GetTypes()
            .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
    }


    public class FieldDescriptor
    {
        public string Name;
        public Type Type;
        public Type ParentType;
        public string FileName;

        public FieldDescriptor(string name, Type type, string fileName, Type parentType)
        {
            Name = name;
            Type = type;
            ParentType = parentType;
            FileName = fileName;
        }
    }

    public class SceneEventDescriptor
    {
        public Delegate SceneEvent;
        public Action<SceneEvent> Handler;

        public SceneEventDescriptor(Delegate sceneEvent,
            Action<SceneEvent> handler)
        {
            SceneEvent = sceneEvent;
            Handler = handler;
        }
    }
}
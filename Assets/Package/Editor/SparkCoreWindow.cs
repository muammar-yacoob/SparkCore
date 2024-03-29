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
        private bool showProps = true;
        private bool showMethods;
        
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
        private static List<PropertyDescriptor> injectedPropertiesList = new();
        private static List<MethodDescriptor> injectedMethodsList = new();
        
        private static List<MonoEventsDescriptor> monoEventsList = new();

        private static SparkCoreWindow window;
        private bool showTypes;
        private static Rect hoverLayer;
        private static int currentTab;
        private string[] tabNames = { "Injection View", "Mono Events" };
        private static Rect bottomRect;
        private string helpMsg;
        private bool showEvents;
        private List<Delegate> MonoEventss;
        private bool showFullEventName;
        private static Assembly defaultAssembly;

        #region Menus

        [MenuItem("Spark Core/Injection Manager &B", false, 11)]
        public static void OpenInjectionWindow() => OpenMainWindow(0);

        [MenuItem("Spark Core/Injection Manager &I", true, 11)]
        private static bool Validate_OpenInjectionWindow() => !Application.isPlaying;

        [MenuItem("Spark Core/Mono Events", false, 12)]
        public static void OpenSceneWindow() => OpenMainWindow(1);

        [MenuItem("Spark Core/Mono Events", true, 12)]
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
            defaultAssembly = GetDefualtAssembly();
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
                    int total = injectedFieldsList.Count + injectedPropertiesList.Count + injectedMethodsList.Count;
                    helpMsg = total > 0
                        ? $"A total of {total} injections were detected."
                        : $"Click Rescan Assembly to refresh.";
                    DrawInjectionTab();
                    break;
                case 1:
                    helpMsg = monoEventsList.Count > 0
                        ? $"A total of {monoEventsList.Count} events were detected."
                        : $"Click Load Mono Events to refresh.";
                    DrawMonoEventsTab();
                    break;
                default:
                    //Debug.Log(currentTab);
                    break;
            }
        }

        private void DrawMonoEventsTab()
        {
            GUILayout.Space(10);
            if (!EditorApplication.isPlaying)
            {
                helpMsg = "List can only be loaded while playing.";
                return;
            }

            
            DrawMonoEvents();
            if (GUILayout.Button("Load Mono Events"))
            {
                LoadMonoEvents();
            }

            GUILayout.Space(5);
            GUILayout.Label("Settings", h1Style);
            showFullEventName = GUILayout.Toggle(showFullEventName, "Show Full Path");
        }

        private void DrawInjectionTab()
        {
            DrawFields();
            DrawProperties();
            DrawMethods();

            if (EditorApplication.isPlaying)
            {
                helpMsg = "Refreshing is disabled while playing.";
                return;
            }

            if (GUILayout.Button("Rescan Assembly"))
            {
                LoadFields();
                LoadProperties();
                LoadMethods();
                
                DrawFields();
                DrawProperties();
                DrawMethods();
                
                Repaint();
            }

            GUILayout.Space(5);
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

        #region Injections
        #region Fields
        private void DrawFields()
        {
            GUILayout.Space(5);
            showFields = EditorGUILayout.BeginFoldoutHeaderGroup(showFields, $"{(showFields ? "-" : "+")} Fields", h1Style);
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
            IEnumerable<Type> types = defaultAssembly.GetTypes();
            var myTypes =  types.Where(type => type.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

            injectedFieldsList.Clear();


            foreach (Type type in myTypes)
            {
                foreach (FieldInfo field in type.GetFields(flags))
                {
                    if (field.GetCustomAttribute<Inject>() != null)
                    {
                        // if (field.FieldType.IsAssignableFrom(typeof(IMonoEventsHistory)) ||
                        //     field.FieldType.IsAssignableFrom(typeof(IMonoEventsProvider)))
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
        #endregion

        #region Poperties

        private void DrawProperties()
        {
            GUILayout.Space(5);

            showProps = EditorGUILayout.BeginFoldoutHeaderGroup(showProps, $"{(showProps ? "-" : "+")} Properties", h1Style);
            if (showProps)
            {
                if (injectedPropertiesList.Count < 1) LoadProperties();

                var maxSectionHeight = Mathf.Min(injectedPropertiesList.Count * 30, Screen.height / 3);
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true),
                    GUILayout.Height(maxSectionHeight));
                foreach (var item in injectedPropertiesList)
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

        private void LoadProperties()
        {
            IEnumerable<Type> types = defaultAssembly.GetTypes();
            var myTypes =  types.Where(type => type.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

            injectedPropertiesList.Clear();
            
            foreach (Type type in myTypes)
            {
                foreach (PropertyInfo property in type.GetProperties(flags))
                {
                    if (property.GetCustomAttribute<Inject>() != null)
                    {
                        // if (property.PropertyType.IsAssignableFrom(typeof(IMonoEventsHistory)) ||
                        //     property.PropertyType.IsAssignableFrom(typeof(IMonoEventsProvider)))
                        //     continue;

                        var results = AssetDatabase.FindAssets((type.Name));
                        var g = results.FirstOrDefault();
                        var filePath = AssetDatabase.GUIDToAssetPath(g);
                        var prop = new PropertyDescriptor(property.Name, property.PropertyType, filePath, type);
                        injectedPropertiesList.Add(prop);
                        // Debug.Log(prop.Name);
                    }
                }
            }
        }

        #endregion
        
        #region Methods
        private void DrawMethods()
        {
            GUILayout.Space(5);

            showMethods = EditorGUILayout.BeginFoldoutHeaderGroup(showMethods, $"{(showMethods ? "-" : "+")} Methods", h1Style);
            if (showMethods)
            {
                if (injectedMethodsList.Count < 1) LoadMethods();

                var maxSectionHeight = Mathf.Min(injectedMethodsList.Count * 30, Screen.height / 3);
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true),
                    GUILayout.Height(maxSectionHeight));
                foreach (var item in injectedMethodsList)
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
        
        private void LoadMethods()
        {
            IEnumerable<Type> types = defaultAssembly.GetTypes();
            var myTypes =  types.Where(type => type.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

            injectedMethodsList.Clear();
            
            foreach (Type type in myTypes)
            {
                foreach (MethodInfo method in type.GetMethods(flags))
                {
                    if (method.GetCustomAttribute<Inject>() != null)
                    {
                        // if (method.ReturnType.IsAssignableFrom(typeof(IMonoEventsHistory)) ||
                        //     method.ReturnType.IsAssignableFrom(typeof(IMonoEventsProvider)))
                        //     continue;

                        var results = AssetDatabase.FindAssets((type.Name));
                        var g = results.FirstOrDefault();
                        var filePath = AssetDatabase.GUIDToAssetPath(g);
                        var mthd = new MethodDescriptor(method.Name, method.ReturnType, filePath, type);
                        injectedMethodsList.Add(mthd);
                        // Debug.Log(mthd.Name);
                    }
                }
            }
        }
        #endregion
        #endregion

        private void LoadMonoEvents()
        {
            monoEventsList.Clear();
            var MonoEventss = EventManager.Instance.GetSubscribers<MonoEvent>();
            foreach (var MonoEvents in MonoEventss)
            {
                monoEventsList.Add(new MonoEventsDescriptor(MonoEvents, MonoEvents.Method.GetParameters()[0].ParameterType));
            }
        }
        
        private void DrawMonoEvents()
        {
            GUILayout.Space(5);
            showEvents =
                EditorGUILayout.BeginFoldoutHeaderGroup(showEvents, $"{(showEvents ? "-" : "+")} Events", h1Style);
            
            if (showEvents)
            {
                if (monoEventsList == null) return;
                if (monoEventsList.Count < 1) LoadMonoEvents();

                var maxSectionHeight = Mathf.Min(monoEventsList.Count * 30, Screen.height / 1.1f);
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true),
                    GUILayout.Height(maxSectionHeight));
                foreach (var item in monoEventsList)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);

                    string eventSubscriberNamespace = item.MonoEvents.Method.DeclaringType.Namespace;
                    string eventSubscriberClass = item.MonoEvents.Method.DeclaringType.Name;
                    string eventSubscriberHandler = item.MonoEvents.Method.Name;
                    string eventSubscriberType = item.EventType.Name;

                    string caption = ($"{(showFullEventName ? $"{eventSubscriberNamespace}." : "")}" +
                                      $"{eventSubscriberClass}{(showFullEventName ? $".{eventSubscriberHandler}" : "")}");

                    GUILayout.Space(10);
                    GUILayout.Label($"{eventSubscriberType} \u279c ", labelStyle);

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
    
    public class PropertyDescriptor
    {
        public string Name;
        public Type Type;
        public Type ParentType;
        public string FileName;

        public PropertyDescriptor(string name, Type type, string fileName, Type parentType)
        {
            Name = name;
            Type = type;
            ParentType = parentType;
            FileName = fileName;
        }
    }
    
    public class MethodDescriptor
    {
        public string Name;
        public Type Type;
        public Type ParentType;
        public string FileName;

        public MethodDescriptor(string name, Type type, string fileName, Type parentType)
        {
            Name = name;
            Type = type;
            ParentType = parentType;
            FileName = fileName;
        }
    }

    public class MonoEventsDescriptor
    {
        public Delegate MonoEvents { get; private set; }
        public Type EventType { get; private set; }

        public MonoEventsDescriptor(Delegate monoEvents, Type eventType)
        {
            MonoEvents = monoEvents;
            EventType = eventType;
        }
    }

}
using SparkCore.Runtime.Core;
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace SparkCore.Editor
{
    [CustomEditor(typeof(InjectableMonoBehaviour), true)]
    public class SceneBehaviourCustomInspector : UnityEditor.Editor
    {
        private GUISkin skin;
        private InjectableMonoBehaviour injectableMono;
        private Rect headerRect;
        private Texture2D headerTexture;
        private readonly float headerTexScale = 0.09f;
        private bool showPublishers;
        private bool showListeners;

        private List<Type> publishedEventTypes;
        private List<Type> subscribedEventTypes;

        void OnEnable()
        {
            skin = Resources.Load<GUISkin>("guiStyles/Default");
            headerTexture = Resources.Load<Texture2D>("icons/spark");
            injectableMono = target as InjectableMonoBehaviour;

            if (injectableMono != null)
            {
                FindEventTypes();
            }
        }

        public override void OnInspectorGUI()
        {
            // Header
            GUILayout.Label($"SparkCore", skin.GetStyle("CustomHeader"));
            GUILayout.Label($"Injectable MonoBehaviour", skin.GetStyle("CustomH1"));
            headerRect = new Rect(Screen.width - headerTexture.width * headerTexScale, 0,
                headerTexture.width * headerTexScale, headerTexture.height * headerTexScale);
            GUI.DrawTexture(headerRect, headerTexture);

            DrawDefaultInspector();
            DrawGUI();
        }

        private void DrawGUI()
        {
            EditorGUILayout.Space(20);
            if (!EditorApplication.isPlaying || injectableMono == null) return;

            if (publishedEventTypes.Any())
            {
                showPublishers = EditorGUILayout.BeginFoldoutHeaderGroup(showPublishers, "Published Events");
                if (showPublishers)
                {
                    foreach (var eventType in publishedEventTypes)
                    {
                        if (GUILayout.Button($"Fire {eventType.Name}"))
                        {
                            FireEvent(eventType);
                        }
                    }
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
                GUILayout.Space(5);
            }

            if (subscribedEventTypes.Any())
            {
                showListeners = EditorGUILayout.BeginFoldoutHeaderGroup(showListeners, "Event Listeners");
                if (showListeners)
                {
                    foreach (var eventType in subscribedEventTypes)
                    {
                        GUILayout.Label($"Listening to {eventType.Name}", skin.GetStyle("CustomLink"));
                    }
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
                GUILayout.Space(5);
            }
        }

        private void FireEvent(Type eventType)
        {
            try
            {
                object eventInstance = CreateEventInstance(eventType);

                if (eventInstance == null)
                {
                    Debug.LogError($"Failed to create an instance of {eventType.Name}. No suitable constructor found.");
                    return;
                }

                var publishMethod = typeof(InjectableMonoBehaviour).GetMethod("PublishEvent",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                // Create a generic method with the correct event type
                var genericPublishMethod = publishMethod?.MakeGenericMethod(eventType);

                // Invoke the generic method
                genericPublishMethod?.Invoke(injectableMono, new[] { eventInstance });

                Debug.Log($"Fired event: {eventType.Name}", injectableMono);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error firing event of type {eventType.Name}: {ex.Message}");
            }
        }

        private object CreateEventInstance(Type eventType)
        {
            var constructors = eventType.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(c => c.GetParameters().Length);

            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                var paramValues = new object[parameters.Length];

                bool canUseConstructor = true;
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType == typeof(string))
                    {
                        paramValues[i] = "Event fired from Inspector";
                    }
                    else if (parameters[i].HasDefaultValue)
                    {
                        paramValues[i] = parameters[i].DefaultValue;
                    }
                    else
                    {
                        canUseConstructor = false;
                        break;
                    }
                }

                if (canUseConstructor)
                {
                    return constructor.Invoke(paramValues);
                }
            }

            return null;
        }

        private void FindEventTypes()
        {
            publishedEventTypes = new List<Type>();
            subscribedEventTypes = new List<Type>();
            var methods = injectableMono.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var method in methods)
            {
                var body = method.GetMethodBody();
                if (body == null) continue;

                foreach (var localVar in body.LocalVariables)
                {
                    if (typeof(MonoEvent).IsAssignableFrom(localVar.LocalType) &&
                        localVar.LocalType != typeof(MonoEvent))
                    {
                        publishedEventTypes.Add(localVar.LocalType);
                    }
                }

                var instructions = body.GetILAsByteArray();

                for (int i = 0; i < instructions.Length - 4; i++)
                {
                    if (instructions[i] == 0x28) // call opcode
                    {
                        var methodToken = BitConverter.ToUInt32(instructions, i + 1);
                        try
                        {
                            var calledMethod = method.Module.ResolveMethod((int)methodToken);
                            if (calledMethod.Name == "PublishEvent")
                            {
                                var eventType = calledMethod.GetParameters()[0].ParameterType;
                                if (typeof(MonoEvent).IsAssignableFrom(eventType) && eventType != typeof(MonoEvent))
                                {
                                    publishedEventTypes.Add(eventType);
                                }
                            }
                            else if (calledMethod.Name == "SubscribeEvent")
                            {
                                var eventType = calledMethod.GetParameters()[0].ParameterType.GetGenericArguments()[0];
                                if (typeof(MonoEvent).IsAssignableFrom(eventType))
                                {
                                    subscribedEventTypes.Add(eventType);
                                }
                            }
                        }
                        catch (ArgumentException)
                        {
                            // Ignore if we can't resolve the method
                        }
                    }
                }
            }

            publishedEventTypes = publishedEventTypes.Distinct().ToList();
            subscribedEventTypes = subscribedEventTypes.Distinct().ToList();
        }
    }
}
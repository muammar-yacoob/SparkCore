using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace SparkCore.Editor.Setup
{
    public static class Packages
    {
        private static Dictionary<AddRequest, Action> updateCallbacks = new Dictionary<AddRequest, Action>();

        public static void InstallCoreDependencies()
        {
            List<string> packageIds = new List<string>()
            {
                "com.unity.inputsystem"
                // ,"com.cysharp.unitask"
                // ,"com.verraes.vcontainer"
            };
            packageIds.ForEach(packageId => Install(packageId));
        }

        public static void InstallXR()
        {
            List<string> packageIds = new List<string>()
            {
                "com.unity.xr.management",
                "com.unity.xr.interaction.toolkit"
            };
            packageIds.ForEach(packageId => Install(packageId));
        }

        public static void Install(string packageId)
        {
            var request = Client.Add(packageId);
            var updateCallback = new Action(() => CheckAndInstall(packageId, request));
            updateCallbacks[request] = updateCallback;
            EditorApplication.update += updateCallback.Invoke;
        }

        private static void CheckAndInstall(string packageId, AddRequest request)
        {
            if (request.IsCompleted)
            {
                if (request.Status == StatusCode.Success)
                {
                    Debug.Log($"{packageId.PackageName()} installed successfully!");
                }
                else if (request.Status >= StatusCode.Failure)
                {
                    Debug.LogError($"Failed to install {packageId.PackageName()}: {request.Error.message}");
                }

                var updateCallback = updateCallbacks[request];
                EditorApplication.update -= updateCallback.Invoke;
                updateCallbacks.Remove(request);
            }
        }

        private static string PackageName(this string packageId)
        {
            int lastDotIndex = packageId.LastIndexOf('.');
            if (lastDotIndex >= 0 && lastDotIndex < packageId.Length - 1)
            {
                return packageId.Substring(lastDotIndex + 1);
            }

            return packageId;
        }
    }
}

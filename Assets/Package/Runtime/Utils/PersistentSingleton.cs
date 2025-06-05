﻿using UnityEngine;

namespace SparkGames.SparkCore.Utils
{
    public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }

    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if(Instance != null && Instance != this) Destroy(gameObject);
            Instance = this as T;
        }
    }
}
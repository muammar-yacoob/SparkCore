﻿using System;
using SparkCore.Runtime.Injection;
using UnityEngine;

namespace SparkDev.Demo.InjectionTest
{
    [ServiceProvider]
    public class ConsoleLogger : ILogger
    {
        private readonly Guid guid;
        public ConsoleLogger() => this.guid = Guid.NewGuid();

        public void Log(string message) => Debug.Log($"<color=yellow>ConsoleLogger: {guid}</color> {message}");
    }
    
    [ServiceProvider(ServiceLifetime.Transient)]
    public class AnotherLogger : ILogger
    {
        private readonly Guid guid;
        public AnotherLogger() => this.guid = Guid.NewGuid();
        public void Log(string message) => Debug.Log($"<color=cyan>AnotherLogger: {guid}</color> {message}");
    }

}
using Flux.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FluxEditor
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class FluxMonoBehaviourEditor : Editor
    {
        private const BindingFlags TargetFlags = BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        private readonly Dictionary<MethodInfo, List<object>> parametersCache = new();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Get the methods
            var target = (MonoBehaviour)this.target;
            var methods = target.GetType().GetMethods(TargetFlags);

            DrawButtons(methods);
            DrawWithConfirmationButtons(methods);
            DrawBenchmarks(methods);
        }

        private void DrawWithConfirmationButtons(MethodInfo[] allMethods)
        {
            List<(MethodInfo, ButtonWithConfirmationAttribute)> buttonAttributes = new();
            var type = typeof(ButtonWithConfirmationAttribute);

            for (int i = 0; i < allMethods.Length; i++)
            {
                var element = allMethods[i];
                var attribute = (ButtonWithConfirmationAttribute)Attribute.GetCustomAttribute(element, type);
                if (attribute != null)
                {
                    buttonAttributes.Add((allMethods[i], attribute));
                }
            }

            foreach (var (method, buttonAttribute) in buttonAttributes)
            {
                // Get the parameter cache
                if (!parametersCache.TryGetValue(method, out var parameterCache))
                {
                    parameterCache = new();
                    parametersCache[method] = parameterCache;
                }

                string label = buttonAttribute.label;
                string message = buttonAttribute.message;

                // Default name to the method name, if none given
                if (label == string.Empty)
                    label = method.Name;

                if (GUILayout.Button(label))
                {
                    // Get the params
                    var parametersInfo = method.GetParameters();

                    object[] parameters = new object[parametersInfo.Length];

                    for (int i = 0; i < parametersInfo.Length; i++)
                    {
                        Debug.Log(parametersInfo[i].ParameterType);
                    }

                    if (method.ReturnType == typeof(void))
                    {
                        if (EditorUtility.DisplayDialog("Confirmation Required", message, "Confirm", "Decline"))
                        {
                            method.Invoke(target, parameters);
                        }
                    }
                    else
                    {
                        if (EditorUtility.DisplayDialog("Confirmation Required", message, "Confirm", "Decline"))
                        {
                            Debug.Log(method.Invoke(target, parameters));
                        }
                    }
                }
            }
        }

        private void DrawBenchmarks(MethodInfo[] allMethods)
        {
            // get the groups
            var benchmarkGroups = GetGroup<BenchmarkAttribute>();
            var setupGroups = GetGroup<BenchmarkSetupAttribute>();
            var cleanupGroups = GetGroup<BenchmarkCleanupAttribute>();

            foreach (var (groupName, benchmarkAttributes) in benchmarkGroups)
            {
                if (GUILayout.Button($"Benchmark Group: {groupName}"))
                {
                    System.Diagnostics.Stopwatch timer = new();
                    Dictionary<int, Dictionary<MethodInfo, (float, long)>> times = new();

                    TryRunSetup(groupName, BenchmarkSetupAttribute.E_RunType.OnceAtStart);

                    foreach (var (method, attribute) in benchmarkAttributes)
                    {
                        var itterations = attribute.Itterations;

                        var parametersInfo = method.GetParameters();
                        object[] parameters = new object[parametersInfo.Length];

                        TryRunSetup(groupName, BenchmarkSetupAttribute.E_RunType.BeforeEachUniqueMethod);

                        for (int i = 0; i < itterations.Length; i++)
                        {
                            var count = itterations[i];

                            if (count <= 0)
                            {
                                continue;
                            }

                            if (!times.TryGetValue(count, out var methodTime))
                            {
                                methodTime = new();
                                times[count] = methodTime;
                            }

                            for (int j = 0; j < count; j++)
                            {
                                TryRunSetup(groupName, BenchmarkSetupAttribute.E_RunType.BeforeEachItteration);

                                timer.Start();
                                method.Invoke(target, parameters);
                                timer.Stop();

                                TryRunCleanup(groupName, BenchmarkCleanupAttribute.E_RunType.AfterEachItteration);
                            }

                            if (!methodTime.ContainsKey(method))
                            {
                                methodTime[method] = (timer.ElapsedMilliseconds, timer.ElapsedTicks);
                            }
                            else
                            {
                                var current = methodTime[method];
                                methodTime[method] = (current.Item1 + timer.ElapsedMilliseconds, current.Item2 + timer.ElapsedTicks);
                            }

                            timer.Reset();
                        }

                        TryRunCleanup(groupName, BenchmarkCleanupAttribute.E_RunType.AfterEachUniqueMethod);
                    }

                    TryRunCleanup(groupName, BenchmarkCleanupAttribute.E_RunType.OnceAtEnd);

                    System.Text.StringBuilder sb = new();

                    long largest = 0;
                    foreach (var (numItterations, methodTimes) in times)
                    {
                        foreach (var (method, (ms, ticks)) in methodTimes)
                        {
                            float avgTicks = ticks / numItterations;
                            if (avgTicks > (float)largest)
                            {
                                largest = (long)avgTicks;
                            }
                        }
                    }

                    foreach (var (numItterations, methodTimes) in times)
                    {
                        sb.Append($" Itterations:{numItterations} = ");

                        foreach (var (method, (ms, ticks)) in methodTimes)
                        {
                            float avgTicks = ticks / numItterations;
                            float avgMs = ms / (float)numItterations;
                            double fraction = (double)avgTicks / (double)largest;
                            double percent = fraction * 100d;

                            Color color = Color.Lerp(Color.green, Color.red, (float)fraction);
                            string hexColor = ColorUtility.ToHtmlStringRGB(color);

                            sb.Append($"[<color=#{hexColor}>{method.Name}</color> : ({percent:F2}%) {ticks}t/{ms}ms - Avg.{avgTicks}t/{avgMs}ms]");
                        }
                    }

                    Debug.Log($"{sb}");
                }
            }

            Dictionary<string, List<(MethodInfo, T)>> GetGroup<T>() where T : A_BenchmarkAttribute
            {
                Dictionary<string, List<(MethodInfo, T)>> group = new();
                for (int i = 0; i < allMethods.Length; i++)
                {
                    var element = allMethods[i];
                    var attribute = (T)Attribute.GetCustomAttribute(element, typeof(T));
                    if (attribute != null)
                    {
                        var groupName = attribute.GroupName;

                        if (!group.TryGetValue(groupName, out var list))
                        {
                            list = new();
                            group[groupName] = list;
                        }

                        list.Add((element, attribute));
                    }
                }

                return group;
            }

            void TryRunSetup(string groupName, BenchmarkSetupAttribute.E_RunType type)
            {
                if (setupGroups.TryGetValue(groupName, out var attributes))
                {
                    foreach (var (method, attribute) in attributes)
                    {
                        if (attribute.RunType == type)
                        {
                            var parametersInfo = method.GetParameters();
                            object[] parameters = new object[parametersInfo.Length];
                            method.Invoke(target, parameters);
                        }
                    }
                }
            }

            void TryRunCleanup(string groupName, BenchmarkCleanupAttribute.E_RunType type)
            {
                if (cleanupGroups.TryGetValue(groupName, out var attributes))
                {
                    foreach (var (method, attribute) in attributes)
                    {
                        if (attribute.RunType == type)
                        {
                            var parametersInfo = method.GetParameters();
                            object[] parameters = new object[parametersInfo.Length];
                            method.Invoke(target, parameters);
                        }
                    }
                }
            }
        }

        private void DrawButtons(MethodInfo[] allMethods)
        {
            List<(MethodInfo, ButtonAttribute)> buttonAttributes = new();
            var type = typeof(ButtonAttribute);

            for (int i = 0; i < allMethods.Length; i++)
            {
                var element = allMethods[i];
                var attribute = (ButtonAttribute)Attribute.GetCustomAttribute(element, type);
                if (attribute != null)
                {
                    buttonAttributes.Add((allMethods[i], attribute));
                }
            }

            foreach (var (method, buttonAttribute) in buttonAttributes)
            {
                // Get the parameter cache
                if (!parametersCache.TryGetValue(method, out var parameterCache))
                {
                    parameterCache = new();
                    parametersCache[method] = parameterCache;
                }

                string label = buttonAttribute.label;

                // Default name to the method name, if none given
                if (label == string.Empty)
                    label = method.Name;

                if (GUILayout.Button(label))
                {
                    // Get the params
                    var parametersInfo = method.GetParameters();

                    object[] parameters = new object[parametersInfo.Length];

                    for (int i = 0; i < parametersInfo.Length; i++)
                    {
                        Debug.Log(parametersInfo[i].ParameterType);
                    }

                    if (method.ReturnType == typeof(void))
                    {
                        method.Invoke(target, parameters);
                    }
                    else
                    {
                        Debug.Log(method.Invoke(target, parameters));
                    }
                }
            }
        }

        private void DrawField(Type type, object value, Action<object> valueSetter)
        {
            if (type == typeof(int))
            {
                value = EditorGUILayout.IntField("Int Field", (int)value);
            }
            else if (type == typeof(float))
            {
                value = EditorGUILayout.FloatField("Float Field", (float)value);
            }
            else if (type == typeof(string))
            {
                value = EditorGUILayout.TextField("String Field", (string)value);
            }
            // Add more type cases as needed for other types

            if (GUI.changed)
            {
                valueSetter(value);
            }
        }
    }
}
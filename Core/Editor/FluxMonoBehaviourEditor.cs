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
        private const string BUTTONS_LABEL = "Flux Buttons";

        private readonly Dictionary<MethodInfo, List<object>> parametersCache = new();
        private bool buttonsFoldout = false;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Get the methods
            var target = (MonoBehaviour)this.target;
            var methods = target.GetType().GetMethods(TargetFlags);

            // get + draw buttons
            DrawButtons(methods);

            DrawBenchmarks(methods);
        }

        private void DrawBenchmarks(MethodInfo[] allMethods)
        {
            Dictionary<string, List<(MethodInfo, BenchmarkAttribute)>> benchmarkGroups = new();
            var type = typeof(BenchmarkAttribute);

            for (int i = 0; i < allMethods.Length; i++)
            {
                var element = allMethods[i];
                var attribute = (BenchmarkAttribute)Attribute.GetCustomAttribute(element, type);
                if (attribute != null)
                {
                    var groupName = attribute.GroupName;

                    if (!benchmarkGroups.TryGetValue(groupName, out var list))
                    {
                        list = new();
                        benchmarkGroups[groupName] = list;
                    }

                    list.Add((element, attribute));
                }
            }

            foreach (var (groupName, benchmarkAttributes) in benchmarkGroups)
            {
                if (GUILayout.Button($"Benchmark Group: {groupName}"))
                {
                    System.Diagnostics.Stopwatch timer = new();
                    Dictionary<int, Dictionary<MethodInfo, (float, long)>> times = new();

                    foreach (var (method, attribute) in benchmarkAttributes)
                    {
                        var itterations = attribute.Itterations;

                        var parametersInfo = method.GetParameters();
                        object[] parameters = new object[parametersInfo.Length];

                        for (int i = 0; i < itterations.Length; i++)
                        {
                            var count = itterations[i];

                            if (!times.TryGetValue(count, out var methodTime))
                            {
                                methodTime = new();
                                times[count] = methodTime;
                            }

                            for (int j = 0; j < count; j++)
                            {
                                timer.Start();
                                method.Invoke(target, parameters);
                                timer.Stop();
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
                    }

                    System.Text.StringBuilder sb = new();

                    foreach (var (numItterations, methodTimes) in times)
                    {
                        sb.Append($" Itterations:{numItterations} = ");

                        long largest = long.MinValue;
                        foreach (var (method, (ms, ticks)) in methodTimes)
                        {
                            if (ticks > largest)
                                largest = ticks;
                        }

                        foreach (var (method, (ms, ticks)) in methodTimes)
                        {
                            float avgTicks = ticks / numItterations;
                            float avgMs = ms / (float)numItterations;
                            double percent = ((double)ticks / (double)largest) * 100d;

                            sb.Append($"[{method.Name} : {ticks}t/{ms}ms - Avg.{avgTicks}t/{avgMs}ms ({percent:F2}%)]");
                        }
                    }

                    Debug.Log(sb.ToString());
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
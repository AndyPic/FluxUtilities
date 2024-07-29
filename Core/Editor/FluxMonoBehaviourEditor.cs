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
        private const string ButtonsLabel = "Flux Buttons";

        private readonly Dictionary<MethodInfo, List<object>> parametersCache = new();
        private bool buttonsFoldout = false;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawButtons(ButtonsLabel);
        }

        private Dictionary<MethodInfo, T> GetMethodAttributes<T>(MethodInfo[] methods) where T : Attribute
        {
            Dictionary<MethodInfo, T> test = new();

            for (int i = 0; i < methods.Length; i++)
            {
                var btn = (T)Attribute.GetCustomAttribute(methods[i], typeof(T));
                if (btn != null)
                    test[methods[i]] = btn;
            }

            return test;
        }

        private void DrawButtons(string foldoutLabel)
        {
            // Get the methods
            var target = (MonoBehaviour)this.target;
            var methods = target.GetType().GetMethods(TargetFlags);

            // Filter for button attribute
            var methodAttributes = GetMethodAttributes<ButtonAttribute>(methods);

            // Guard if there are no buttons to draw
            if (methodAttributes.Count == 0)
                return;

            // Draw foldout for buttons
            buttonsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(buttonsFoldout, foldoutLabel);
            if (buttonsFoldout)
            {
                foreach (var (method, buttonAttribute) in methodAttributes)
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
            EditorGUILayout.EndFoldoutHeaderGroup();
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
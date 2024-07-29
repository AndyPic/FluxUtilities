using Flux.Core.Attributes;
using UnityEditor;
using UnityEngine;

namespace FluxEditor
{
    [CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
    public class RequireInterfaceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RequireInterfaceAttribute requireInterface = (RequireInterfaceAttribute)attribute;

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                EditorGUI.BeginProperty(position, label, property);

                UnityEngine.Object newObject = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(UnityEngine.Object), true);

                if (newObject != null)
                {
                    MonoBehaviour validComponent = null;

                    if (newObject is GameObject)
                    {
                        GameObject gameObject = newObject as GameObject;
                        MonoBehaviour[] components = gameObject.GetComponents<MonoBehaviour>();

                        foreach (MonoBehaviour component in components)
                        {
                            if (requireInterface.RequiredType.IsAssignableFrom(component.GetType()))
                            {
                                validComponent = component;
                                break;
                            }
                        }
                    }
                    else if (newObject is MonoBehaviour)
                    {
                        MonoBehaviour monoBehaviour = newObject as MonoBehaviour;
                        if (requireInterface.RequiredType.IsAssignableFrom(monoBehaviour.GetType()))
                        {
                            validComponent = monoBehaviour;
                        }
                    }

                    if (validComponent != null)
                    {
                        property.objectReferenceValue = validComponent;
                    }
                    else
                    {
                        property.objectReferenceValue = null;
                        Debug.LogError($"The assigned object must have a component that implements {requireInterface.RequiredType.Name}");
                    }
                }
                else
                {
                    property.objectReferenceValue = null;
                }

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use RequireInterface with MonoBehaviour.");
            }
        }
    }
}
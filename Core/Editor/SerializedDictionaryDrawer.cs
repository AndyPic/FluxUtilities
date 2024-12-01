using Flux.Core.Collections;
using UnityEditor;
using UnityEngine;

namespace FluxEditor
{
    [CustomPropertyDrawer(typeof(SerializedDictionary<,>))]
    public class SerializedDictionaryDrawer : PropertyDrawer
    {
        private const string KeysName = "<Keys>k__BackingField";
        private const string ValuesName = "<Values>k__BackingField";

        private const float Spacing = 5f;
        private const float KeyWidthPercentage = 0.4f;
        private const float ValueWidthPercentage = 0.6f;
        private const float DeleteWidth = 24f;
        private const float HalfDeleteWidth = DeleteWidth / 2f;

        private bool foldout = true;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty keysProperty = property.FindPropertyRelative(KeysName);

            if (foldout)
            {
                return (EditorGUIUtility.singleLineHeight + Spacing) * (keysProperty.arraySize + 2);
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect foldoutRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            foldout = EditorGUI.Foldout(foldoutRect, foldout, label);
            if (foldout)
            {
                EditorGUI.indentLevel++;

                SerializedProperty keysProperty = property.FindPropertyRelative(KeysName);
                SerializedProperty valuesProperty = property.FindPropertyRelative(ValuesName);

                float lineHeight = EditorGUIUtility.singleLineHeight;
                Rect linePosition = new(position.x, position.y, position.width, lineHeight);
                linePosition.y += lineHeight + Spacing;

                float keyWidth = (position.width * KeyWidthPercentage) - Spacing - HalfDeleteWidth;
                float valueWidth = (position.width * ValueWidthPercentage) - Spacing - HalfDeleteWidth;

                for (int i = 0; i < keysProperty.arraySize; i++)
                {
                    SerializedProperty keyProperty = keysProperty.GetArrayElementAtIndex(i);
                    SerializedProperty valueProperty = valuesProperty.GetArrayElementAtIndex(i);

                    Rect keyPosition = new(linePosition.x, linePosition.y, keyWidth, lineHeight);
                    Rect valuePosition = new(linePosition.x + keyWidth, linePosition.y, valueWidth, lineHeight);
                    Rect deletePosition = new(linePosition.x + keyWidth + valueWidth, linePosition.y, DeleteWidth, lineHeight);

                    EditorGUI.PropertyField(keyPosition, keyProperty, GUIContent.none);
                    EditorGUI.PropertyField(valuePosition, valueProperty, GUIContent.none);

                    if (GUI.Button(deletePosition, "-"))
                    {
                        keysProperty.DeleteArrayElementAtIndex(i);
                        valuesProperty.DeleteArrayElementAtIndex(i);
                    }

                    linePosition.y += lineHeight + Spacing;
                }

                if (GUI.Button(new Rect(linePosition.x + keyWidth + valueWidth, linePosition.y, DeleteWidth, lineHeight), "+"))
                {
                    keysProperty.arraySize++;
                    valuesProperty.arraySize++;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }
    }
}

using UnityEditor;
using UnityEngine;

namespace Popcron.Extras.Editor
{
    [CustomPropertyDrawer(typeof(PopType))]
    public class TypeDrawer : PropertyDrawer
    {
        private static GUIStyle customStyle;
        private bool? isValid;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect checkmarkPosition = new Rect(position.xMax - 18, position.y, 18, position.height);
            SerializedProperty fullName = property.FindPropertyRelative("fullName");
            string oldName = fullName.stringValue;
            EditorGUI.PropertyField(position, fullName, label);
            if (oldName != fullName.stringValue || isValid == null)
            {
                isValid = PopType.GetType(fullName.stringValue);
            }

            if (customStyle is null)
            {
                customStyle = new GUIStyle(EditorStyles.label);
                customStyle.richText = true;
            }

            EditorGUI.LabelField(checkmarkPosition, isValid == true ? "<color=green>✔</color>" : "<color=red>✖</color>", customStyle);
        }
    }
}

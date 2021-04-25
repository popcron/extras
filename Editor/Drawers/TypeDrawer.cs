using UnityEditor;
using UnityEngine;

namespace Popcron.Extras.Editor
{
    [CustomPropertyDrawer(typeof(global::Type))]
    public class TypeDrawer : PropertyDrawer
    {
        private static GUIStyle customStyle;
        private bool? isValid;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect valuePosition = EditorGUI.PrefixLabel(position, label);
            Rect checkmarkPosition = new Rect(valuePosition.xMax - 18, valuePosition.y, 18, valuePosition.height);
            SerializedProperty fullName = property.FindPropertyRelative("fullName");
            string newName = EditorGUI.TextField(valuePosition, fullName.stringValue);
            if (newName != fullName.stringValue || isValid is null)
            {
                isValid = Type.GetType(newName) != null;
                fullName.stringValue = newName;
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

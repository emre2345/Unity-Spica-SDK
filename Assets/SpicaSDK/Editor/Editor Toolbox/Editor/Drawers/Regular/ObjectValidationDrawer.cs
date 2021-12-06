using UnityEditor;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Regular
{
    public abstract class ObjectValidationDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);
            if (EditorGUI.EndChangeCheck())
            {
                var objectValue = property.objectReferenceValue;
                if (objectValue == null || IsObjectValid(objectValue, property))
                {
                    return;
                }

                property.objectReferenceValue = null;
                ToolboxEditorLog.AttributeUsageWarning(attribute, property, GetWarningMessage());
            }
        }

        protected virtual string GetWarningMessage()
        {
            return "Assigned value was rejected.";
        }

        protected abstract bool IsObjectValid(Object objectValue, SerializedProperty property);


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }
    }
}
using SpicaSDK.Editor.Editor_Toolbox.Editor.Utilities;
using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.RegularAttributes;
using UnityEditor;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Regular
{
    [CustomPropertyDrawer(typeof(OnValueChangedAttribute))]
    public class OnValueChangedAttributeDrawer : PropertyDrawerBase
    {
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label, property.isExpanded);
            if (EditorGUI.EndChangeCheck())
            {
                var methodName = Attribute.CallbackMethodName;
                if (ReflectionUtility.TryInvokeMethod(methodName, property.serializedObject))
                {
                    return;
                }

                ToolboxEditorLog.AttributeUsageWarning(attribute, string.Format("{0} method is invalid.", methodName));
            }
        }


        private OnValueChangedAttribute Attribute => attribute as OnValueChangedAttribute;
    }
}
using SpicaSDK.Editor.Editor_Toolbox.Editor.Utilities;
using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes;
using UnityEditor;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox
{
    public abstract class ToolboxPropertyDrawer<T> : ToolboxPropertyDrawerBase where T : ToolboxPropertyAttribute
    {
        protected virtual void OnGuiSafe(SerializedProperty property, GUIContent label, T attribute)
        {
            ToolboxEditorGui.DrawDefaultProperty(property, label);
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return true;
        }

        public override sealed void OnGui(SerializedProperty property, GUIContent label)
        {
            OnGui(property, label, PropertyUtility.GetAttribute<T>(property));
        }

        public override sealed void OnGui(SerializedProperty property, GUIContent label, ToolboxAttribute attribute)
        {
            OnGui(property, label, attribute as T);
        }

        public void OnGui(SerializedProperty property, GUIContent label, T attribute)
        {
            if (attribute == null)
            {
                return;
            }

            if (IsPropertyValid(property))
            {
                OnGuiSafe(property, label, attribute);
                return;
            }

            var warningContent = new GUIContent(string.Format("{0} has invalid property drawer", property.displayName));
            //create additional warning log to the Console window
            ToolboxEditorLog.WrongAttributeUsageWarning(attribute, property);
            //create additional warning label based on the property name
            ToolboxEditorGui.DrawEmptyProperty(property, warningContent);
        }
    }
}
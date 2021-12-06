using SpicaSDK.Editor.Editor_Toolbox.Editor.Utilities;
using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes;
using UnityEditor;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox
{
    public abstract class ToolboxConditionDrawer<T> : ToolboxConditionDrawerBase where T : ToolboxConditionAttribute
    {
        protected virtual PropertyCondition OnGuiValidateSafe(SerializedProperty property, T attribute)
        {
            return PropertyCondition.Valid;
        }


        public override sealed PropertyCondition OnGuiValidate(SerializedProperty property)
        { 
            return OnGuiValidate(property, PropertyUtility.GetAttribute<T>(property));
        }

        public override sealed PropertyCondition OnGuiValidate(SerializedProperty property, ToolboxAttribute attribute)
        {           
            return OnGuiValidate(property, attribute as T);         
        }

        public PropertyCondition OnGuiValidate(SerializedProperty property, T attribute)
        {
            if (attribute == null)
            {
                return PropertyCondition.Valid;
            }

            return OnGuiValidateSafe(property, attribute);
        }
    }
}
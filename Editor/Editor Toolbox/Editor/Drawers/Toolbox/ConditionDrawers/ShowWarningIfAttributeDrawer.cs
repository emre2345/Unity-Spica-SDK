using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.ConditionAttributes;
using UnityEditor;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.ConditionDrawers
{
    public class ShowWarningIfAttributeDrawer : ComparisonAttributeDrawer<ShowWarningIfAttribute>
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, ShowWarningIfAttribute attribute)
        {
            var result = base.OnGuiValidateSafe(property, attribute);
            if (result == PropertyCondition.Disabled)
            {
                EditorGUILayout.HelpBox(attribute.Message, MessageType.Warning);
                result = attribute.DisableField 
                    ? PropertyCondition.Disabled 
                    : PropertyCondition.Valid;
            }

            return result;
        }

        protected override PropertyCondition OnComparisonResult(bool result)
        {
            return result ? PropertyCondition.Disabled : PropertyCondition.Valid;
        }
    }
}
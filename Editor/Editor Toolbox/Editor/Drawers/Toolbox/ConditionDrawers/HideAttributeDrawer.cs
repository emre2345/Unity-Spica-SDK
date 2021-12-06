using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.ConditionAttributes;
using UnityEditor;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.ConditionDrawers
{
    public class HideAttributeDrawer : ToolboxConditionDrawer<HideAttribute>
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, HideAttribute attribute)
        {
            return PropertyCondition.NonValid;
        }
    }
}
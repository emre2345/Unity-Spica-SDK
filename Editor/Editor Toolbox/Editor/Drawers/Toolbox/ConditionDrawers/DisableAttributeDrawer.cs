using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.ConditionAttributes;
using UnityEditor;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.ConditionDrawers
{
    public class DisableAttributeDrawer : ToolboxConditionDrawer<DisableAttribute>
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, DisableAttribute attribute)
        {
            return PropertyCondition.Disabled;
        }
    }
}
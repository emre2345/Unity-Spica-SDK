using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.ConditionAttributes;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.ConditionDrawers
{
    public class HideDisabledIfAttributeDrawer : ComparisonAttributeDrawer<HideDisabledIfAttribute>
    {
        protected override PropertyCondition OnComparisonResult(bool result)
        {
            return result ? PropertyCondition.NonValid : PropertyCondition.Disabled;
        }
    }
}
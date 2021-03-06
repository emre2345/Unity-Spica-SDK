using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.ConditionAttributes;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.ConditionDrawers
{
    public class ShowDisabledIfAttributeDrawer : ComparisonAttributeDrawer<ShowDisabledIfAttribute>
    {
        protected override PropertyCondition OnComparisonResult(bool result)
        {
            return result ? PropertyCondition.Disabled : PropertyCondition.NonValid;
        }
    }
}
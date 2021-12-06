using SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Helpers.Comparison;
using SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Helpers.Extraction;
using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.ConditionAttributes;
using UnityEditor;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.ConditionDrawers
{
    public abstract class ComparisonAttributeDrawer<T> : ToolboxConditionDrawer<T> where T : ComparisonAttribute
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, T attribute)
        {
            var sourceHandle = attribute.SourceHandle;
            if (!ValueExtractionHelper.TryGetValue(sourceHandle, property, out var value, out var hasMixedValues))
            {
                ToolboxEditorLog.MemberNotFoundWarning(attribute, property, sourceHandle);
                return PropertyCondition.Valid;
            }

            var comparison = (ValueComparisonMethod)attribute.Comparison;
            var targetValue = attribute.ValueToMatch;
            if (!ValueComparisonHelper.TryCompare(value, targetValue, comparison, out var result))
            {
                ToolboxEditorLog.AttributeUsageWarning(attribute, property,
                    string.Format("Invalid comparison input: source:{0}, target:{1}, method:{2}.",
                    value?.GetType(), targetValue?.GetType(), comparison));
                return PropertyCondition.Valid;
            }

            return OnComparisonResult(hasMixedValues ? false : result);
        }

        protected abstract PropertyCondition OnComparisonResult(bool result);
    }
}
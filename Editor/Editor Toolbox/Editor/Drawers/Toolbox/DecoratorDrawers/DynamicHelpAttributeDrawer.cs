using SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Helpers.Extraction;
using SpicaSDK.Editor.Editor_Toolbox.Editor.Utilities;
using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes;
using UnityEditor;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.DecoratorDrawers
{
    public class DynamicHelpAttributeDrawer : ToolboxDecoratorDrawer<DynamicHelpAttribute>
    {
        protected override void OnGuiBeginSafe(DynamicHelpAttribute attribute)
        {
            var sourceHandle = attribute.SourceHandle;
            var targetObjects = InspectorUtility.CurrentTargetObjects;
            if (ValueExtractionHelper.TryGetValue(sourceHandle, targetObjects, out var value, out var hasMixedValues))
            {
                var messageText = hasMixedValues ? "-" : value?.ToString();
                var messageType = (MessageType)attribute.Type;
                EditorGUILayout.HelpBox(messageText, messageType);
                return;
            }

            var targetType = targetObjects[0].GetType();
            ToolboxEditorLog.MemberNotFoundWarning(attribute, targetType, sourceHandle);
        }
    }
}
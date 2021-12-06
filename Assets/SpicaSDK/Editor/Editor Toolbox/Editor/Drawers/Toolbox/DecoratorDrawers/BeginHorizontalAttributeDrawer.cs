using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes;
using UnityEditor;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.DecoratorDrawers
{
    public class BeginHorizontalAttributeDrawer : ToolboxDecoratorDrawer<BeginHorizontalAttribute>
    {
        protected override void OnGuiBeginSafe(BeginHorizontalAttribute attribute)
        {
            var width = EditorGUIUtility.currentViewWidth;
            //set a new width value for label/field controls
            EditorGUIUtility.labelWidth = width * attribute.LabelToWidthRatio;
            EditorGUIUtility.fieldWidth = width * attribute.FieldToWidthRatio;

            //begin horizontal group using internal utility
            ToolboxLayoutHelper.BeginHorizontal();
        }
    }
}
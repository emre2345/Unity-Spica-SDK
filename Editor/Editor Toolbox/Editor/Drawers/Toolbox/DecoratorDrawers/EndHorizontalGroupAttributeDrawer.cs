using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes;
using UnityEditor;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.DecoratorDrawers
{
    public class EndHorizontalGroupAttributeDrawer : ToolboxDecoratorDrawer<EndHorizontalGroupAttribute>
    {
        protected override void OnGuiCloseSafe(EndHorizontalGroupAttribute attribute)
        {
            ToolboxLayoutHelper.CloseHorizontal();
            EditorGUILayout.EndScrollView();
            ToolboxLayoutHelper.CloseVertical();

            EditorGUIUtility.labelWidth = 0.0f;
            EditorGUIUtility.fieldWidth = 0.0f;
        }
    }
}
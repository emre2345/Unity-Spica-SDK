using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes;
using UnityEditor;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.DecoratorDrawers
{
    public class BeginIndentAttributeDrawer : ToolboxDecoratorDrawer<BeginIndentAttribute>
    {
        protected override void OnGuiBeginSafe(BeginIndentAttribute attribute)
        {
            EditorGUI.indentLevel += attribute.IndentToAdd;
        }
    }
}
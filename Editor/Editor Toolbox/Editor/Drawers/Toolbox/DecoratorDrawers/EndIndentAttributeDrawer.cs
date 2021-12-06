using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes;
using UnityEditor;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.DecoratorDrawers
{
    public class EndIndentAttributeDrawer : ToolboxDecoratorDrawer<EndIndentAttribute>
    {
        protected override void OnGuiCloseSafe(EndIndentAttribute attribute)
        {
            EditorGUI.indentLevel -= attribute.IndentToSubtract;
        }
    }
}
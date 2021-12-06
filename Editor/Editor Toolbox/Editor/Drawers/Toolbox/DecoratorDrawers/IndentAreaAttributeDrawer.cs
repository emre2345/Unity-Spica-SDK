using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes;
using UnityEditor;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.DecoratorDrawers
{
    public class IndentAreaAttributeDrawer : ToolboxDecoratorDrawer<IndentAreaAttribute>
    {
        protected override void OnGuiBeginSafe(IndentAreaAttribute attribute)
        {
            EditorGUI.indentLevel += attribute.IndentLevelChange;
        }

        protected override void OnGuiCloseSafe(IndentAreaAttribute attribute)
        {
            EditorGUI.indentLevel -= attribute.IndentLevelChange;
        }
    }
}
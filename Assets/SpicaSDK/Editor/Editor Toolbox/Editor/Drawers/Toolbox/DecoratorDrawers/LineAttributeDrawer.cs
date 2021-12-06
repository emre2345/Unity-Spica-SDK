using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.DecoratorDrawers
{
    public class LineAttributeDrawer : ToolboxDecoratorDrawer<LineAttribute>
    {
        protected override void OnGuiBeginSafe(LineAttribute attribute)
        {
            ToolboxEditorGui.DrawLine(attribute.Thickness,
                                      attribute.Padding,
                                      attribute.GuiColor,
                                      attribute.IsHorizontal,
                                      attribute.ApplyIndent);
        }
    }
}
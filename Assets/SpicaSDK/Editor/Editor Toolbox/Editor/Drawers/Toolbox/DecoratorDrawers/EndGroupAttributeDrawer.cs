using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.DecoratorDrawers
{
    public class EndGroupAttributeDrawer : ToolboxDecoratorDrawer<EndGroupAttribute>
    {
        protected override void OnGuiCloseSafe(EndGroupAttribute attribute)
        {
            ToolboxLayoutHelper.CloseVertical();
        }
    }
}
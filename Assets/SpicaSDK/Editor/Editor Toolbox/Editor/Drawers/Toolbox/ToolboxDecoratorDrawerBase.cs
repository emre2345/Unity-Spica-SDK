using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox
{
    public abstract  class ToolboxDecoratorDrawerBase : ToolboxAttributeDrawer
    {
        public abstract void OnGuiBegin(ToolboxAttribute attribute);

        public abstract void OnGuiClose(ToolboxAttribute attribute);
    }
}
using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox
{
    public abstract class ToolboxSelfPropertyDrawer<T> : ToolboxPropertyDrawer<T> where T : ToolboxSelfPropertyAttribute
    { }
}
using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.DecoratorDrawers
{
    public class GuiColorAttributeDrawer : ToolboxDecoratorDrawer<GuiColorAttribute>
    {
        private Color formerGuiColor;


        protected override void OnGuiBeginSafe(GuiColorAttribute attribute)
        {
            formerGuiColor = GUI.color;
            GUI.color = attribute.Color;
        }

        protected override void OnGuiCloseSafe(GuiColorAttribute attribute)
        {
            GUI.color = formerGuiColor;
        }
    }
}
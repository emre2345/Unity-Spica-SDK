using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.DecoratorDrawers
{
    public class SpaceAreaAttributeDrawer : ToolboxDecoratorDrawer<SpaceAreaAttribute>
    {
        protected override void OnGuiBeginSafe(SpaceAreaAttribute attribute)
        {
            GUILayout.Space(attribute.SpaceBefore);         
        }

        protected override void OnGuiCloseSafe(SpaceAreaAttribute attribute)
        {
            GUILayout.Space(attribute.SpaceAfter);
        }
    }
}
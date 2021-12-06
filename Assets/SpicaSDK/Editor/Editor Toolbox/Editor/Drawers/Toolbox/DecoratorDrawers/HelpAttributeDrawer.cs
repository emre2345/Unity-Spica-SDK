using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes;
using UnityEditor;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.DecoratorDrawers
{
    public class HelpAttributeDrawer : ToolboxDecoratorDrawer<HelpAttribute>
    {
        protected override void OnGuiBeginSafe(HelpAttribute attribute)
        {
            EditorGUILayout.HelpBox(attribute.Text, (MessageType)attribute.Type);
        }
    }
}
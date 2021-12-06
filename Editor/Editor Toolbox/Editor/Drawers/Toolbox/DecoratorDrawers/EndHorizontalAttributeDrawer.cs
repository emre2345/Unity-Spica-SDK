using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes;
using UnityEditor;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.DecoratorDrawers
{
    public class EndHorizontalAttributeDrawer : ToolboxDecoratorDrawer<EndHorizontalAttribute>
    {
        protected override void OnGuiCloseSafe(EndHorizontalAttribute attribute)
        {
            //end horizontal group
            ToolboxLayoutHelper.CloseHorizontal();

            //restore label & field 
            EditorGUIUtility.labelWidth = 0.0f;
            EditorGUIUtility.fieldWidth = 0.0f;
        }
    }
}
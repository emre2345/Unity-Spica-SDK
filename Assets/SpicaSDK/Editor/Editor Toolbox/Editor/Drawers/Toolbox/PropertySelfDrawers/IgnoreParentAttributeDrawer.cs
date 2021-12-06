using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.PropertySelfAttributes;
using UnityEditor;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.PropertySelfDrawers
{
    public class IgnoreParentAttributeDrawer : ToolboxSelfPropertyDrawer<IgnoreParentAttribute>
    {
        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, IgnoreParentAttribute attribute)
        {
            if (!property.hasVisibleChildren)
            {
                ToolboxEditorGui.DrawNativeProperty(property, label);
                return;
            }

            ToolboxEditorGui.DrawPropertyChildren(property);
        }
    }
}
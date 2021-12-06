using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes;
using UnityEditor;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox
{
    public abstract class ToolboxListPropertyDrawer<T> : ToolboxPropertyDrawer<T> where T : ToolboxListPropertyAttribute
    {
        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.isArray;
        } 
    }
}
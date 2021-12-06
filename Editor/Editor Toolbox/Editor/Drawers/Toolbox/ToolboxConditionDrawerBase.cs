using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes;
using UnityEditor;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox
{
    public abstract class ToolboxConditionDrawerBase : ToolboxAttributeDrawer
    {
        public abstract PropertyCondition OnGuiValidate(SerializedProperty property);

        public abstract PropertyCondition OnGuiValidate(SerializedProperty property, ToolboxAttribute attribute);
    }
}
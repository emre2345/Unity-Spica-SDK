using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.ConditionAttributes;
using UnityEditor;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox.ConditionDrawers
{
    public class DisableInPlayModeAttributeDrawer : ToolboxConditionDrawer<DisableInPlayModeAttribute>
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, DisableInPlayModeAttribute attribute)
        {
            return EditorApplication.isPlayingOrWillChangePlaymode ? PropertyCondition.Disabled : PropertyCondition.Valid;
        }
    }
}
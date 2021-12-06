using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes;
using UnityEditor;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox
{
    public abstract class ToolboxPropertyDrawerBase : ToolboxAttributeDrawer
    {
        public abstract bool IsPropertyValid(SerializedProperty property);

        public abstract void OnGui(SerializedProperty property, GUIContent label);

        public abstract void OnGui(SerializedProperty property, GUIContent label, ToolboxAttribute attribute);

        public virtual void OnGuiReload()
        { }
    }
}
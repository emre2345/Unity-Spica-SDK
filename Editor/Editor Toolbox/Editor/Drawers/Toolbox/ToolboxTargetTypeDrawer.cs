using System;
using UnityEditor;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Toolbox
{
    public abstract class ToolboxTargetTypeDrawer : ToolboxDrawer
    {
        public abstract void OnGui(SerializedProperty property, GUIContent label);

        public abstract Type GetTargetType();
        public abstract bool UseForChildren();
    }
}
using System;
using SpicaSDK.Editor.Editor_Toolbox.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Internal
{
    /// <summary>
    /// Mirrors functionality of the <see cref="EditorGUI.PropertyScope"/> for layout-based controls.
    /// Additionally creates foldout label for the given property.
    /// </summary>
    internal class PropertyScope : IDisposable
    {
        private readonly SerializedProperty property;


        public PropertyScope(SerializedProperty property, GUIContent label)
        {
            this.property = property;
            ToolboxEditorGui.BeginProperty(property, ref label, out var labelRect);
            HandleEvents(labelRect);
            TryDrawLabel(labelRect, label);
        }


        private void HandleEvents(Rect rect)
        {
            if (property.isArray)
            {
                DraggingUtility.DoDragAndDropForProperty(rect, property);
            }
        }

        private void TryDrawLabel(Rect rect, GUIContent label)
        {
            if (property.hasChildren)
            {
                property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label, true);
            }
            else
            {
                EditorGUI.LabelField(rect, label);
            }
        }


        public void Dispose()
        {
            ToolboxEditorGui.CloseProperty();
        }


        public bool IsVisible => property.isExpanded;
    }
}

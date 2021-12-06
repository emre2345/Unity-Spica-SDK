using System;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes
{
    /// <summary>
    /// Changes indent level of the property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class IndentAreaAttribute : ToolboxDecoratorAttribute
    {
        public IndentAreaAttribute()
        {
            IndentLevelChange = 1;
        }

        public IndentAreaAttribute(int indentLevelChange)
        {
            IndentLevelChange = indentLevelChange;
        }

        public int IndentLevelChange { get; private set; }
    }
}
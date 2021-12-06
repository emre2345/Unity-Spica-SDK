using System;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes
{
    /// <summary>
    /// Begins indentation group. Has to be closed by the <see cref="EndIndentAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class BeginIndentAttribute : ToolboxDecoratorAttribute
    {
        public BeginIndentAttribute(int indentToAdd = 1)
        {
            IndentToAdd = indentToAdd;
        }

        public int IndentToAdd { get; private set; }
    }
}
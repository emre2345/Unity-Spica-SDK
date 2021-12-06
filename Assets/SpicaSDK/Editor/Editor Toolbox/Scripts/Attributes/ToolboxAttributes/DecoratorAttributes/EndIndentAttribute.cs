using System;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EndIndentAttribute : ToolboxDecoratorAttribute
    {
        public EndIndentAttribute(int indentToSubtract = 1)
        {
            IndentToSubtract = indentToSubtract;
        }

        public int IndentToSubtract { get; private set; }
    }
}
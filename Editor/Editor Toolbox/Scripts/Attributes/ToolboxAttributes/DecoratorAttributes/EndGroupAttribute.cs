using System;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EndGroupAttribute : ToolboxDecoratorAttribute
    {
        public EndGroupAttribute()
        {
            Order = -1000;
        }
    }
}
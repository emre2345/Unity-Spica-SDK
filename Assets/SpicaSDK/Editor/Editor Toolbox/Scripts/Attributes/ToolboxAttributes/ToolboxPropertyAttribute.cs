using System;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes
{    
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public abstract class ToolboxPropertyAttribute : ToolboxAttribute
    { }
}
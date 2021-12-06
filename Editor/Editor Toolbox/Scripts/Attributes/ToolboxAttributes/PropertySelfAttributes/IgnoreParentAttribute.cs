using System;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.PropertySelfAttributes
{
    /// <summary>
    /// Ignores parent label and default foldout for children-based properties.
    /// 
    /// <para>Supported types: any.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class IgnoreParentAttribute : ToolboxSelfPropertyAttribute
    { }
}
using System;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.ConditionAttributes
{
    /// <summary>
    /// Hides serialized field.
    /// 
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HideAttribute : ToolboxConditionAttribute
    { }
}
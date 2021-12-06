using System;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.ConditionAttributes
{
    /// <summary>
    /// Marks serialized field as read-only but only in the PlayMode.
    /// 
    /// <para>Supported types: all.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DisableInPlayModeAttribute : ToolboxConditionAttribute
    { }
}
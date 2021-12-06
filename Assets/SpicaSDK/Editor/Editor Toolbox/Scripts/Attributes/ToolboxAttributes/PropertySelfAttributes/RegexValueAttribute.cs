using System;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.PropertySelfAttributes
{
    /// <summary>
    /// Validates target's value using regular expression and given pattern.
    /// 
    /// <para>Supported types: <see cref="string"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class RegexValueAttribute : ToolboxSelfPropertyAttribute
    {
        public RegexValueAttribute(string pattern) : this(pattern, null)
        { }

        public RegexValueAttribute(string pattern, string message)
        {
            Pattern = pattern;
            Message = message;
        }

        public string Pattern { get; private set; }
        public string Message { get; private set; }

        public bool HasMessage => !string.IsNullOrEmpty(Message);
    }
}
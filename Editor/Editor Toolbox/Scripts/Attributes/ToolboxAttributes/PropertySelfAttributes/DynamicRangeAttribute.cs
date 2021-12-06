using System;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.PropertySelfAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DynamicRangeAttribute : ToolboxSelfPropertyAttribute
    {
        public DynamicRangeAttribute(string minValueSource, string maxValueSource)
        {
            MinValueSource = minValueSource;
            MaxValueSource = maxValueSource;
        }

        public string MinValueSource { get; private set; }

        public string MaxValueSource { get; private set; }
    }
}
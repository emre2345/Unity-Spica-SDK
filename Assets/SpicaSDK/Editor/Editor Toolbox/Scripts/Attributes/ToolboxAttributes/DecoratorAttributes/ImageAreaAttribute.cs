using System;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes
{
    /// <summary>
    /// Creates and displays texture as decorator, based on URL/URI.
    /// Target texture is downloaded using a standard HTTP web request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ImageAreaAttribute : ToolboxDecoratorAttribute
    {
        public ImageAreaAttribute(string url) : this(url, 100.0f)
        { }

        public ImageAreaAttribute(string url, float height)
        {
            Url = url;
            Height = height;
        }

        public string Url { get; private set; }

        public float Height { get; private set; }
    }
}
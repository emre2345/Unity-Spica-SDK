namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Helpers.Extraction
{
    internal interface IValueExtractor
    {
        bool TryGetValue(string source, object declaringObject, out object value);
    }
}
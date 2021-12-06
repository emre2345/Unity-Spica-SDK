using System;
using System.Reflection;
using SpicaSDK.Editor.Editor_Toolbox.Editor.Utilities;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Helpers.Extraction
{
    public class MethodValueExtractor : IValueExtractor
    {
        public bool TryGetValue(string source, object declaringObject, out object value)
        {
            value = default;
            if (string.IsNullOrEmpty(source))
            {
                return false;
            }

            var type = declaringObject.GetType();
            var info = type.GetMethod(source, ReflectionUtility.allBindings, null, CallingConventions.Any, new Type[0], null);
            if (info == null)
            {
                return false;
            }

            value = info.Invoke(declaringObject, null);
            return true;
        }
    }
}
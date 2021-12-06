using System;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.RegularAttributes
{
    /// <summary>
    /// Creates a popup window with an input field. Allows to search for enum values by their name.
    /// 
    /// <para>Supported types: any <see cref="Enum"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SearchableEnumAttribute : PropertyAttribute
    { }
}
using System;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.RegularAttributes
{
    /// <summary>
    /// Draws the left-ordered toggle.
    /// 
    /// <para>Supported types: <see cref="bool"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class LeftToggleAttribute : PropertyAttribute
    { }
}
using System;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.RegularAttributes
{
    /// <summary>
    /// Validates if current value is a proper Scene name.
    /// 
    /// <para>Supported types: <see cref="string"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SceneNameAttribute : PropertyAttribute
    { }
}
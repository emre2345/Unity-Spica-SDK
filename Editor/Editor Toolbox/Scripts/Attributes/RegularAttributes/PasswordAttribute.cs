using System;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.RegularAttributes
{
    /// <summary>
    /// Draws a password field.
    /// 
    /// <para>Supported types: <see cref="string"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PasswordAttribute : PropertyAttribute
    { }
}
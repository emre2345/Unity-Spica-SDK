using System;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.RegularAttributes
{
    /// <summary>
    /// Creates layer-based popup menu.
    /// 
    /// <para>Supported types: <see cref="int"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class LayerAttribute : PropertyAttribute
    { }
}
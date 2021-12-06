using System;
using SpicaSDK.Editor.Editor_Toolbox.Scripts.Serialization;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.RegularAttributes
{
    /// <summary>
    /// Indicates if <see cref="SerializedScene"/> drawer should show additional metadata about the picked Scene.
    /// 
    /// <para>Supported types: <see cref="SerializedScene"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SceneDetailsAttribute : PropertyAttribute
    { }
}
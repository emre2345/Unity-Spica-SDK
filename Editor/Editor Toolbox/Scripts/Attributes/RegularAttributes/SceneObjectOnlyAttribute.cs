using System;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.RegularAttributes
{
    /// <summary>
    /// Validates input values and accepts only objects instantiated on the Scene.
    /// 
    /// <para>Supported types: <see cref="GameObject"/> and any <see cref="Component"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SceneObjectOnlyAttribute : PropertyAttribute
    { }
}
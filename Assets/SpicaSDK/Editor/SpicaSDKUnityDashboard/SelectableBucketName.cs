using System;
using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes;
using SpicaSDK.Services.Models;
using UnityEngine;

namespace SpicaSDK.Editor.SpicaSDKUnityDashboard
{
    [Serializable]
    public class SelectableBucketName
    {
        public bool Selected;

        public string Title;

        [HideInInspector] public Bucket bucket;
    }
}
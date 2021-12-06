﻿using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.RegularAttributes;
using UnityEditor;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Regular
{
    [CustomPropertyDrawer(typeof(SceneObjectOnlyAttribute))]
    public class SceneObjectOnlyAttributeDrawer : ObjectValidationDrawer
    {
        private GameObject GetGameObject(Object reference)
        {
            switch (reference)
            {
                case GameObject gameObject:
                    return gameObject;
                case Component component:
                    return component.gameObject;
            }

            return null;
        }


        protected override string GetWarningMessage()
        {
            return "Assigned object has to be instantiated in the Scene.";
        }

        protected override bool IsObjectValid(Object objectValue, SerializedProperty property)
        {
            var gameObject = GetGameObject(objectValue);
            return gameObject && !string.IsNullOrEmpty(gameObject.scene.name);
        }
    }
}
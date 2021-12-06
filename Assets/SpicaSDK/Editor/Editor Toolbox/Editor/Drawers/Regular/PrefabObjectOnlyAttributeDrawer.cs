using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.RegularAttributes;
using UnityEditor;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Drawers.Regular
{
    [CustomPropertyDrawer(typeof(PrefabObjectOnlyAttribute))]
    public class PrefabObjectOnlyAttributeDrawer : ObjectValidationDrawer
    {
        protected override string GetWarningMessage()
        {
            return "Assigned object has to be a prefab.";
        }

        protected override bool IsObjectValid(Object objectValue, SerializedProperty property)
        {
            return PrefabUtility.GetPrefabAssetType(objectValue) != PrefabAssetType.NotAPrefab;
        }
    }
}
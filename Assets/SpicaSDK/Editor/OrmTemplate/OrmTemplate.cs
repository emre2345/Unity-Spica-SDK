using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SpicaSDK.Editor.OrmTemplate
{
    public static class OrmTemplate
    {
        private static string Template =>
            File.ReadAllText($"{Application.dataPath}/Plugins/SpicaSDK/Editor/OrmTemplate/OrmClassTemplate.txt");

        private const string ClassName = "{{ORM_CLASS_NAME}}";
        private const string DataClassName = "{{ORM_DATA_CLASS_NAME}}";
        private const string BucketId = "{{BUCKET_ID}}";

        public static string Create(string className, string dataClassName, string id) =>
            Template.Replace(ClassName, className).Replace(DataClassName, dataClassName).Replace(BucketId, $"\"{id}\"");
    }
}
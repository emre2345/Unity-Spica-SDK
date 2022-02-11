using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsCodeGenerator;
using CsCodeGenerator.Enums;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.ArchetypeAttributes;
using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.ConditionAttributes;
using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.DecoratorAttributes;
using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.PropertyListAttributes;
using SpicaSDK.Editor.Editor_Toolbox.Scripts.Attributes.ToolboxAttributes.PropertySelfAttributes;
using SpicaSDK.Editor.OrmTemplate;
using SpicaSDK.Editor.SpicaSDKUnityDashboard;
using SpicaSDK.Runtime.Utils;
using SpicaSDK.Services;
using SpicaSDK.Services.Models;
using SpicaSDK.Services.Services.Identity.Models;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

[CreateAssetMenu]
public class Dashboard : ScriptableObject
{
    [Title("Login")] [DisableIf(nameof(LoginButtonEnabled), false)]
    public string UserName;

    [DisableIf(nameof(LoginButtonEnabled), false)]
    public string Password;

    [SerializeField, Hide] [EditorButton(nameof(Login), nameof(Login), nameof(LoginButtonEnabled))]
    public bool dummyBool_LoginButton;

    private void OnEnable()
    {
        if (SpicaServerConfiguration.Instance == null)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            AssetDatabase.Refresh();

            SpicaServerConfiguration config = CreateInstance<SpicaServerConfiguration>();
            AssetDatabase.CreateAsset(config, $"Assets/Resources/{nameof(SpicaServerConfiguration)}.asset");
            SpicaLogger.Instance.Log("Created a config file in the Resources Folder");
        }
    }

    async UniTask Login()
    {
        logInProgress = true;

        try
        {
            Identity identity = await SpicaSDK.Services.SpicaSDK.LogIn(UserName, Password);
            SpicaSDK.Services.SpicaSDK.SetIdentity(identity);
            SpicaLogger.Instance.Log("Logged in successfully");
        }
        finally
        {
            logInProgress = false;
        }
    }

    private bool logInProgress = false;
    private bool LoginButtonEnabled() => !logInProgress && !SpicaSDK.Services.SpicaSDK.LoggedIn;

    [Title("ORM")]
    [SerializeField, Hide]
    [EditorButton(nameof(FetchBuckets), nameof(FetchBuckets), nameof(FetchBucketsButtonEnabled))]
    public bool dummyBool_FetchButton;

    [ReorderableList(ListStyle.Round, null, false, false)]
    public SelectableBucketName[] Buckets;

    private Bucket[] allBuckets;

    async UniTask FetchBuckets()
    {
        fetchInProgress = true;
        allBuckets = await SpicaSDK.Services.SpicaSDK.Bucket.Get();
        Buckets = allBuckets.Select(bucket => new SelectableBucketName()
                { bucket = bucket, Selected = false, Title = bucket.Title })
            .ToArray();
        fetchInProgress = false;
    }

    private bool fetchInProgress;
    private bool FetchBucketsButtonEnabled => !fetchInProgress;

    [EditorButton(nameof(CreateBucketClasses), "Create Bucket Classes", nameof(CreateBucketClassesButtonEnabled))]
    public string parentFolder;

    private bool CreateBucketClassesButtonEnabled => true;

    void CreateBucketClasses()
    {
        var buckets = Buckets.Where(bucketName => bucketName.Selected).Select(bucketName => bucketName.bucket)
            .ToArray();

        var files = new List<FileModel>();
        foreach (var bucket in buckets)
        {
            var classModel = new ClassModel($"{MakeSafeForCode(bucket.Title)}Data");
            var usingStatements = new[] { "System;" };
            var publicFields = new List<Field>();

            foreach (var bucketProperty in bucket.Properties)
            {
                var field = new Field(GetDataTypeOfProperty(allBuckets, bucketProperty), bucketProperty.Key);
                field.AccessModifier = AccessModifier.Public;

                publicFields.Add(field);
            }

            var idField = new Field(BuiltInDataType.String, "_id");
            idField.AccessModifier = AccessModifier.Public;
            publicFields.Insert(0, idField);

            classModel.Fields = publicFields;
            FileModel fileModel = new FileModel(classModel.Name);
            fileModel.LoadUsingDirectives(usingStatements.ToList());
            fileModel.Classes.Add(classModel);
            fileModel.Namespace = "SpicaSDK.Orm";
            files.Add(fileModel);

            string ormClass = OrmTemplate.Create(MakeSafeForCode(bucket.Title), MakeSafeForCode($"{bucket.Title}Data"),
                bucket.Id);
            File.WriteAllText($"{Application.dataPath}/{parentFolder}/{MakeSafeForCode(bucket.Title)}.cs", ormClass);
        }

        CsGenerator generator = new CsGenerator();
        generator.Path = $"{Application.dataPath}/{parentFolder}";
        generator.Files = files;
        generator.CreateFiles();

        AssetDatabase.Refresh();
    }

    string GetDataTypeOfProperty(Bucket[] buckets, KeyValuePair<string, JToken> bucketProperty)
    {
        var propertyType = bucketProperty.Value.First.ToObject<string>();

        if (propertyType.Equals("relation"))
        {
            var relationType = bucketProperty.Value.Value<string>("relationType");
            var relationBucketId = bucketProperty.Value.Value<string>("bucketId");
            switch (relationType)
            {
                case "onetoone":
                    return Array.Find(buckets, b => b.Id.Equals(relationBucketId)).Title +
                           "Data";
                case "onetomany":
                    return Array.Find(buckets, b => b.Id.Equals(relationBucketId)).Title +
                           "Data[]";
            }
        }

        switch (propertyType)
        {
            case "number":
                return BuiltInDataType.Double.ToTextLower();
            case "textarea":
            case "richtext":
            case "storage":
            case "string":
                return BuiltInDataType.String.ToTextLower();
            case "boolean":
                return BuiltInDataType.Bool.ToTextLower();
            case "date":
                return nameof(DateTime);
            case "location":
                return nameof(Point);
            default:
                throw new Exception($"Could not parse dataType: {propertyType}");
        }
    }

    private string MakeSafeForCode(string str)
    {
        str = Regex.Replace(str, "[^a-zA-Z0-9_]", "_", RegexOptions.Compiled);
        if (char.IsDigit(str[0]))
        {
            str = "_" + str;
        }

        return str;
    }
}
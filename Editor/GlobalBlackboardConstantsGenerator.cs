// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Text.RegularExpressions;
// using Newtonsoft.Json;
// using UnityEditor;
// using UnityEngine;
//
// namespace TemplateAssets.Scripts.Utilities
// {
//     public class GlobalBlackboardConstantsGenerator
//     {
//         [MenuItem("DHFramework/Generate GlobalBlackboardConstants.cs")]
//         public static void Generate()
//         {
//             // Try to find an existing file in the project called "GlobalBlackboardConstants.cs"
//             string filePath = string.Empty;
//             foreach (var file in Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories))
//             {
//                 if (Path.GetFileNameWithoutExtension(file) == "GlobalBlackboardConstants")
//                 {
//                     filePath = file;
//                     break;
//                 }
//             }
//
//             // If no such file exists already, use the save panel to get a folder in which the file will be placed.
//             if (string.IsNullOrEmpty(filePath))
//             {
//                 string directory = EditorUtility.OpenFolderPanel("Choose location for GlobalBlackboardConstants.cs",
//                     Application.dataPath, "");
//
//                 // Canceled choose? Do nothing.
//                 if (string.IsNullOrEmpty(directory))
//                 {
//                     return;
//                 }
//
//                 filePath = Path.Combine(directory, "GlobalBlackboardConstants.cs");
//             }
//
//             // Write out our file
//             using (var writer = new StreamWriter(filePath))
//             {
//                 WriteHeader(writer);
//
//                 var blackboards = GlobalBlackboard.GetAll();
//                 foreach (var globalBlackboard in blackboards)
//                 {
//                     string json = globalBlackboard.GetVariablesJson();
//
//                     var variables = JsonConvert.DeserializeObject<List<GlobalBlackboardVariable>>(json);
//
//                     writer.WriteLine($"    public static class {globalBlackboard.identifier}");
//                     writer.WriteLine("    {");
//
//                     foreach (GlobalBlackboardVariable globalBlackboardVariable in variables)
//                     {
//                         try
//                         {
//                             writer.WriteLine("        public const string {0} = \"{1}\";",
//                                 MakeSafeForCode(globalBlackboardVariable.Name),
//                                 globalBlackboardVariable.Name);
//                         }
//                         catch (Exception e)
//                         {
//                             Debug.Log(
//                                 $"[{nameof(GlobalBlackboardConstantsGenerator)}] - Exception while adding GlobalBlackboard: {globalBlackboard.identifier} - Variable Name: {globalBlackboardVariable.Name} \n\n Exception:\n {e.Message}");
//                         }
//                     }
//
//                     writer.WriteLine("    }");
//                     writer.WriteLine();
//                 }
//
//                 writer.WriteLine("    }");
//                 writer.WriteLine();
//             }
//
//             // Refresh
//             AssetDatabase.Refresh();
//         }
//
//         private static void WriteHeader(StreamWriter writer)
//         {
//             writer.WriteLine("// This file is auto-generated. Modifications are not saved.");
//             writer.WriteLine();
//             writer.WriteLine("namespace GlobalBlackboardConstants");
//             writer.WriteLine("{");
//         }
//
//
//         private static string MakeSafeForCode(string str)
//         {
//             str = Regex.Replace(str, "[^a-zA-Z0-9_]", "_", RegexOptions.Compiled);
//             if (char.IsDigit(str[0]))
//             {
//                 str = "_" + str;
//             }
//
//             return str;
//         }
//     }
// }
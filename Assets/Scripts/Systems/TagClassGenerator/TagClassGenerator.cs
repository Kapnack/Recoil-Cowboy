#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditorInternal;

namespace Systems.TagClassGenerator
{
    [InitializeOnLoad]
    public static class TagsClassGenerator
    {
        private const string FilePath = "Assets/Scripts/Systems/TagClassGenerator/Tags.cs";

        static TagsClassGenerator()
        {
            GenerateTagsClass();
        }

        [MenuItem("Tools/GenerateTags.cs")]
        public static void GenerateTagsClass()
        {
            string directory = Path.GetDirectoryName(FilePath);

            if (directory != null && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string[] tags = InternalEditorUtility.tags;

            using (StreamWriter writer = new(FilePath, false))
            {
                Type space = typeof(TagsClassGenerator);

                writer.WriteLine("// DO NOT MODIFY THIS FILE! It Auto-Generates.");
                writer.WriteLine("// Any change made to this file will be deleted.");
                writer.WriteLine("");
                writer.WriteLine($"namespace {space.Namespace}");
                writer.WriteLine("{");
                writer.WriteLine("  public static class Tags");
                writer.WriteLine("  {");

                foreach (string tag in tags)
                {
                    string safeName = MakeSafeName(tag);
                    writer.WriteLine($"    public const string {safeName} = \"{tag}\";");
                }

                writer.WriteLine("  }");
                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();
            UnityEngine.Debug.Log(
                $"Tags.cs was generated Successfully. There are now {tags.Length} Tags in {FilePath}");
        }

        private static string MakeSafeName(string tag)
        {
            string safe = tag.Replace(" ", "_");

            safe = safe.Replace("-", "_");

            if (char.IsDigit(safe[0]))
                safe = "_" + safe;

            return safe;
        }
    }
}
#endif
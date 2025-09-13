#if UNITY_EDITOR
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
            var directory = Path.GetDirectoryName(FilePath);
            
            if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

            var tags = InternalEditorUtility.tags;

            using (var writer = new StreamWriter(FilePath, false))
            {
                var space = typeof(TagsClassGenerator);
                
                writer.WriteLine("// DO NOT MODIFY THIS FILE! It Auto-Generates.");
                writer.WriteLine($"namespace {space.Namespace}");
                writer.WriteLine("{");
                writer.WriteLine("  public static class Tags");
                writer.WriteLine("  {");

                foreach (var tag in tags)
                {
                    var safeName = MakeSafeName(tag);
                    writer.WriteLine($"    public const string {safeName} = \"{tag}\";");
                }

                writer.WriteLine("  }");
                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();
            UnityEngine.Debug.Log($"Tags.cs was generated Successfully. There are now {tags.Length} Tags in {FilePath}");
        }

        private static string MakeSafeName(string tag)
        {
            var safe = tag.Replace(" ", "_");
            
            safe = safe.Replace("-", "_");
            
            if (char.IsDigit(safe[0]))
                safe = "_" + safe;
            
            return safe;
        }
    }
}
#endif
#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditorInternal;


namespace Systems.LayerClassGenerator
{
    [InitializeOnLoad]
    public class LayersGenerator
    {
        private const string FilePath = "Assets/Scripts/Systems/LayerClassGenerator/Layers.cs";

        static LayersGenerator()
        {
            GenerateTagsClass();
        }

        [MenuItem("Tools/GenerateLayers.cs")]
        public static void GenerateTagsClass()
        {
            string directory = Path.GetDirectoryName(FilePath);

            if (directory != null && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string[] layers = InternalEditorUtility.layers;

            using (StreamWriter writer = new(FilePath, false))
            {
                Type space = typeof(LayersGenerator);

                writer.WriteLine("// DO NOT MODIFY THIS FILE! It Auto-Generates.");
                writer.WriteLine("// Any change made to this file will be deleted.");
                writer.WriteLine("");
                writer.WriteLine($"namespace {space.Namespace}");
                writer.WriteLine("{");
                writer.WriteLine("  public static class Layers");
                writer.WriteLine("  {");

                foreach (string layer in layers)
                {
                    string safeName = MakeSafeName(layer);
                    writer.WriteLine($"    public const string {safeName} = \"{layer}\";");
                }

                writer.WriteLine("  }");
                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();
            UnityEngine.Debug.Log(
                $"Layers.cs was generated Successfully. There are now {layers.Length} Tags in {FilePath}");
        }

        private static string MakeSafeName(string layer)
        {
            string safe = layer.Replace(" ", "_");

            safe = safe.Replace("-", "_");

            if (char.IsDigit(safe[0]))
                safe = "_" + safe;

            return safe;
        }
    }
}
#endif
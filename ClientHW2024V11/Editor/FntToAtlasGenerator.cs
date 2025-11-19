using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FntToAtlasGenerator : EditorWindow
{
    [System.Serializable]
    public class FntCharInfo
    {
        public int id;
        public char character;
        public int x, y, width, height;
        public int xoffset, yoffset, xadvance;
        public Texture2D sourceImage;
    }
    
    private TextAsset fntFile;
    private List<FntCharInfo> characters = new List<FntCharInfo>();
    private Vector2 scrollPosition;
    private string outputPath = "";
    private int atlasWidth = 512;
    private int atlasHeight = 512;
    
    [MenuItem("Tools/FNT To Atlas Generator")]
    public static void ShowWindow()
    {
        GetWindow<FntToAtlasGenerator>("FNT To Atlas Generator");
    }
    
    void OnGUI()
    {
        GUILayout.Label("FNT + Images → Atlas Generator", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        // FNT File Selection
        GUILayout.Label("1. Select .fnt File:", EditorStyles.boldLabel);
        fntFile = (TextAsset)EditorGUILayout.ObjectField("FNT File", fntFile, typeof(TextAsset), false);
        
        if (fntFile != null)
        {
            string fntPath = AssetDatabase.GetAssetPath(fntFile);
            GUILayout.Label($"Selected: {fntPath}");
            
            if (fntPath.EndsWith(".fnt"))
            {
                if (GUILayout.Button("Parse .fnt File"))
                {
                    ParseFntFile();
                }
            }
            else
            {
                GUILayout.Label("⚠️ Please select a .fnt file!", EditorStyles.helpBox);
            }
        }
        else
        {
            GUILayout.Label("Please select a .fnt file from your project");
        }
        
        if (characters.Count > 0)
        {
            GUILayout.Space(10);
            
            // Output Settings
            GUILayout.Label("2. Output Settings:", EditorStyles.boldLabel);
            outputPath = EditorGUILayout.TextField("Output Path", outputPath);
            if (GUILayout.Button("Set Output Folder"))
            {
                outputPath = EditorUtility.OpenFolderPanel("Select Output Folder", Application.dataPath, "");
                if (outputPath.StartsWith(Application.dataPath))
                {
                    outputPath = "Assets" + outputPath.Substring(Application.dataPath.Length);
                }
            }
            
            atlasWidth = EditorGUILayout.IntField("Atlas Width", atlasWidth);
            atlasHeight = EditorGUILayout.IntField("Atlas Height", atlasHeight);
            
            GUILayout.Space(10);
            
            // Character Assignment
            GUILayout.Label("3. Assign Character Images:", EditorStyles.boldLabel);
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
            
            for (int i = 0; i < characters.Count; i++)
            {
                var ch = characters[i];
                
                GUILayout.BeginHorizontal();
                
                // Character info
                GUILayout.Label($"'{ch.character}' (ID:{ch.id})", GUILayout.Width(80));
                GUILayout.Label($"Pos:({ch.x},{ch.y})", GUILayout.Width(80));
                GUILayout.Label($"Size:({ch.width}x{ch.height})", GUILayout.Width(100));
                
                // Image assignment
                ch.sourceImage = (Texture2D)EditorGUILayout.ObjectField(ch.sourceImage, typeof(Texture2D), false, GUILayout.Width(100));
                
                // Auto-find button
                if (GUILayout.Button("Auto-Find", GUILayout.Width(70)))
                {
                    AutoFindCharacterImage(ch);
                }
                
                GUILayout.EndHorizontal();
                
                // Image preview
                if (ch.sourceImage != null)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(100);
                    
                    float previewSize = 32f;
                    Rect previewRect = GUILayoutUtility.GetRect(previewSize, previewSize);
                    GUI.DrawTexture(previewRect, ch.sourceImage);
                    
                    GUILayout.EndHorizontal();
                }
                
                GUILayout.Space(5);
            }
            
            GUILayout.EndScrollView();
            
            GUILayout.Space(10);
            
            // Auto-find all button
            if (GUILayout.Button("Auto-Find All Character Images"))
            {
                foreach (var ch in characters)
                {
                    AutoFindCharacterImage(ch);
                }
            }
            
            // Generate atlas button
            if (GUILayout.Button("Generate Atlas PNG", GUILayout.Height(30)))
            {
                GenerateAtlas();
            }
        }
    }
    
    void ParseFntFile()
    {
        characters.Clear();
        
        string fntContent = fntFile.text;
        string[] lines = fntContent.Split('\n');
        
        Debug.Log($"Parsing .fnt file with {lines.Length} lines");
        Debug.Log($"First few lines:\n{string.Join("\n", System.Array.ConvertAll(System.Linq.Enumerable.Take(lines, 5).ToArray(), x => x))}");
        
        // Parse common line for atlas dimensions
        foreach (string line in lines)
        {
            if (line.IndexOf("common") != -1)
            {
                Regex commonReg = new Regex(@"scaleW=(?<scaleW>\d+)\s+scaleH=(?<scaleH>\d+)");
                var match = commonReg.Match(line);
                if (match.Success)
                {
                    atlasWidth = int.Parse(match.Groups["scaleW"].Value);
                    atlasHeight = int.Parse(match.Groups["scaleH"].Value);
                    Debug.Log($"Found atlas dimensions: {atlasWidth}x{atlasHeight}");
                }
            }
        }
        
        // Parse character lines
        Regex charReg = new Regex(@"char id=(?<id>\d+)\s+x=(?<x>\d+)\s+y=(?<y>\d+)\s+width=(?<width>\d+)\s+height=(?<height>\d+)\s+xoffset=(?<xoffset>(-|\d)+)\s+yoffset=(?<yoffset>(-|\d)+)\s+xadvance=(?<xadvance>\d+)");
        
        foreach (string line in lines)
        {
            if (line.IndexOf("char id=") != -1)
            {
                var match = charReg.Match(line);
                if (match.Success)
                {
                    FntCharInfo info = new FntCharInfo();
                    info.id = int.Parse(match.Groups["id"].Value);
                    info.character = (char)info.id;
                    info.x = int.Parse(match.Groups["x"].Value);
                    info.y = int.Parse(match.Groups["y"].Value);
                    info.width = int.Parse(match.Groups["width"].Value);
                    info.height = int.Parse(match.Groups["height"].Value);
                    info.xoffset = int.Parse(match.Groups["xoffset"].Value);
                    info.yoffset = int.Parse(match.Groups["yoffset"].Value);
                    info.xadvance = int.Parse(match.Groups["xadvance"].Value);
                    
                    characters.Add(info);
                    Debug.Log($"Parsed character: '{info.character}' at ({info.x},{info.y})");
                }
                else
                {
                    Debug.LogWarning($"Could not parse line: {line}");
                }
            }
        }
        
        Debug.Log($"Successfully parsed {characters.Count} characters from .fnt file");
        
        // Auto-set output path to same folder as .fnt file
        string fntPath = AssetDatabase.GetAssetPath(fntFile);
        outputPath = Path.GetDirectoryName(fntPath);
        
        if (characters.Count == 0)
        {
            Debug.LogError("No characters found! Please check if this is a valid .fnt file.");
        }
    }
    
    void AutoFindCharacterImage(FntCharInfo charInfo)
    {
        // Search patterns for character files
        string[] searchPatterns = {
            charInfo.character.ToString(),           // "0", "1", "2"...
            "num_" + charInfo.character,             // "num_0", "num_1"...
            charInfo.character + "_",                // "0_", "1_"...
            "char_" + charInfo.id,                   // "char_48", "char_49"...
            charInfo.id.ToString()                   // "48", "49"... (ASCII codes)
        };
        
        // Search in project
        string[] allTextures = AssetDatabase.FindAssets("t:Texture2D");
        
        foreach (string guid in allTextures)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string filename = Path.GetFileNameWithoutExtension(path);
            
            foreach (string pattern in searchPatterns)
            {
                if (filename.Equals(pattern, System.StringComparison.OrdinalIgnoreCase))
                {
                    charInfo.sourceImage = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    Debug.Log($"Found image for '{charInfo.character}': {path}");
                    return;
                }
            }
        }
        
        Debug.LogWarning($"Could not auto-find image for character '{charInfo.character}'");
    }
    
    void GenerateAtlas()
    {
        if (string.IsNullOrEmpty(outputPath))
        {
            Debug.LogError("Please set output path!");
            return;
        }
        
        // Check all characters have images
        foreach (var ch in characters)
        {
            if (ch.sourceImage == null)
            {
                Debug.LogError($"Character '{ch.character}' is missing source image!");
                return;
            }
        }
        
        // Create atlas texture
        Texture2D atlas = new Texture2D(atlasWidth, atlasHeight, TextureFormat.RGBA32, false);
        Color[] clearPixels = new Color[atlasWidth * atlasHeight];
        for (int i = 0; i < clearPixels.Length; i++)
            clearPixels[i] = Color.clear;
        atlas.SetPixels(clearPixels);
        
        // Copy each character to atlas
        foreach (var ch in characters)
        {
            if (ch.sourceImage == null) continue;
            
            // Make source texture readable
            string sourcePath = AssetDatabase.GetAssetPath(ch.sourceImage);
            TextureImporter importer = AssetImporter.GetAtPath(sourcePath) as TextureImporter;
            bool wasReadable = importer.isReadable;
            TextureImporterFormat originalFormat = importer.textureFormat;
            
            if (!wasReadable)
            {
                importer.isReadable = true;
                importer.textureFormat = TextureImporterFormat.RGBA32;
                AssetDatabase.ImportAsset(sourcePath);
            }
            
            // Get source pixels and resize if needed
            Color[] sourcePixels;
            if (ch.sourceImage.width == ch.width && ch.sourceImage.height == ch.height)
            {
                sourcePixels = ch.sourceImage.GetPixels();
            }
            else
            {
                // Resize source image to match fnt dimensions
                RenderTexture rt = RenderTexture.GetTemporary(ch.width, ch.height);
                Graphics.Blit(ch.sourceImage, rt);
                
                Texture2D resized = new Texture2D(ch.width, ch.height);
                RenderTexture.active = rt;
                resized.ReadPixels(new Rect(0, 0, ch.width, ch.height), 0, 0);
                resized.Apply();
                RenderTexture.active = null;
                RenderTexture.ReleaseTemporary(rt);
                
                sourcePixels = resized.GetPixels();
                DestroyImmediate(resized);
            }
            
            // Copy to atlas
            atlas.SetPixels(ch.x, atlasHeight - ch.y - ch.height, ch.width, ch.height, sourcePixels);
            
            // Restore original settings
            if (!wasReadable)
            {
                importer.isReadable = false;
                importer.textureFormat = originalFormat;
                AssetDatabase.ImportAsset(sourcePath);
            }
        }
        
        atlas.Apply();
        
        // Save atlas
        string atlasPath = Path.Combine(outputPath, fntFile.name + "_atlas.png");
        byte[] pngData = atlas.EncodeToPNG();
        File.WriteAllBytes(atlasPath, pngData);
        
        DestroyImmediate(atlas);
        
        // Update .fnt file to reference new atlas
        string newFntPath = Path.Combine(outputPath, fntFile.name + "_generated.fnt");
        UpdateFntFileForNewAtlas(newFntPath, fntFile.name + "_atlas.png");
        
        AssetDatabase.Refresh();
        
        Debug.Log($"Generated atlas: {atlasPath}");
        Debug.Log($"Generated .fnt: {newFntPath}");
        
        // Select generated files
        Texture2D atlasAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(atlasPath);
        Selection.activeObject = atlasAsset;
        EditorGUIUtility.PingObject(atlasAsset);
    }
    
    void UpdateFntFileForNewAtlas(string newFntPath, string atlasFileName)
    {
        string originalContent = fntFile.text;
        string[] lines = originalContent.Split('\n');
        
        using (StreamWriter writer = new StreamWriter(newFntPath))
        {
            foreach (string line in lines)
            {
                if (line.StartsWith("page id="))
                {
                    writer.WriteLine($"page id=0 file=\"{atlasFileName}\"");
                }
                else
                {
                    writer.WriteLine(line);
                }
            }
        }
    }
}
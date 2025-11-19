// using System.Collections.Generic;
// using System.IO;
// using System.Text.RegularExpressions;
// using System.Linq;
// using UnityEditor;
// using UnityEngine;
//
// public class AdvancedFontGenerator : EditorWindow
// {
//     [System.Serializable]
//     public class FontCharacter
//     {
//         public string character = "";
//         public Texture2D sourceImage;
//         public int xoffset = 0;
//         public int yoffset = 0;
//         public int xadvance = 0;
//         public bool useAutoAdvance = true;
//         
//         // Calculated values (auto-filled)
//         public Rect atlasRect = new Rect(0, 0, 0, 0);
//         public int actualWidth = 0;
//         public int actualHeight = 0;
//     }
//     
//     [System.Serializable] 
//     public class PackingSettings
//     {
//         public int maxAtlasSize = 512;
//         public int padding = 2;
//         public bool powerOfTwo = true;
//         public bool allowRotation = false;
//         public Color backgroundColor = Color.clear;
//     }
//     
//     private List<FontCharacter> characters = new List<FontCharacter>();
//     private PackingSettings packingSettings = new PackingSettings();
//     private Vector2 scrollPosition;
//     private Vector2 previewScroll;
//     
//     // Font settings
//     private string fontName = "CustomFont";
//     private int fontSize = 32;
//     private int lineHeight = 40;
//     private string outputFolder = "Assets/Fonts";
//     
//     // Preview
//     private Texture2D previewAtlas;
//     private bool showPreview = false;
//     
//     // Quick setup presets
//     private string quickCharacters = "0123456789";
//     
//     [MenuItem("Tools/Advanced Font Generator")]
//     public static void ShowWindow()
//     {
//         var window = GetWindow<AdvancedFontGenerator>("Advanced Font Generator");
//         window.minSize = new Vector2(850, 600);
//     }
//     
//     void OnGUI()
//     {
//         EditorGUILayout.BeginVertical(GUI.skin.box);
//         
//         // Header
//         GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
//         headerStyle.fontSize = 18;
//         headerStyle.alignment = TextAnchor.MiddleCenter;
//         GUILayout.Label("Advanced Font Generator", headerStyle);
//         GUILayout.Space(10);
//         
//         EditorGUILayout.EndVertical();
//         
//         scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
//         
//         // === FONT SETTINGS ===
//         EditorGUILayout.BeginVertical(GUI.skin.box);
//         GUILayout.Label("Font Settings", EditorStyles.boldLabel);
//         
//         EditorGUILayout.BeginHorizontal();
//         fontName = EditorGUILayout.TextField("Font Name", fontName);
//         fontSize = EditorGUILayout.IntField("Size", fontSize, GUILayout.Width(80));
//         lineHeight = EditorGUILayout.IntField("Line Height", lineHeight, GUILayout.Width(100));
//         EditorGUILayout.EndHorizontal();
//         
//         EditorGUILayout.BeginHorizontal();
//         EditorGUILayout.PrefixLabel("Output Folder");
//         outputFolder = EditorGUILayout.TextField(outputFolder);
//         if (GUILayout.Button("Browse", GUILayout.Width(60)))
//         {
//             string folder = EditorUtility.OpenFolderPanel("Select Output Folder", Application.dataPath, "");
//             if (!string.IsNullOrEmpty(folder) && folder.StartsWith(Application.dataPath))
//             {
//                 outputFolder = "Assets" + folder.Substring(Application.dataPath.Length);
//             }
//         }
//         EditorGUILayout.EndHorizontal();
//         
//         EditorGUILayout.EndVertical();
//         GUILayout.Space(5);
//         
//         // === PACKING SETTINGS ===
//         EditorGUILayout.BeginVertical(GUI.skin.box);
//         GUILayout.Label("Packing Settings", EditorStyles.boldLabel);
//         
//         EditorGUILayout.BeginHorizontal();
//         packingSettings.maxAtlasSize = EditorGUILayout.IntSlider("Max Atlas Size", packingSettings.maxAtlasSize, 128, 2048);
//         packingSettings.powerOfTwo = EditorGUILayout.Toggle("Power of 2", packingSettings.powerOfTwo, GUILayout.Width(100));
//         EditorGUILayout.EndHorizontal();
//         
//         EditorGUILayout.BeginHorizontal();
//         packingSettings.padding = EditorGUILayout.IntSlider("Padding", packingSettings.padding, 0, 10);
//         packingSettings.allowRotation = EditorGUILayout.Toggle("Allow Rotation", packingSettings.allowRotation, GUILayout.Width(120));
//         EditorGUILayout.EndHorizontal();
//         
//         packingSettings.backgroundColor = EditorGUILayout.ColorField("Background Color", packingSettings.backgroundColor);
//         
//         EditorGUILayout.EndVertical();
//         GUILayout.Space(5);
//         
//         // === QUICK SETUP ===
//         EditorGUILayout.BeginVertical(GUI.skin.box);
//         GUILayout.Label("Quick Setup", EditorStyles.boldLabel);
//         
//         EditorGUILayout.BeginHorizontal();
//         quickCharacters = EditorGUILayout.TextField("Characters", quickCharacters);
//         if (GUILayout.Button("Add All", GUILayout.Width(80)))
//         {
//             AddCharactersFromString(quickCharacters);
//         }
//         EditorGUILayout.EndHorizontal();
//         
//         EditorGUILayout.BeginHorizontal();
//         if (GUILayout.Button("Numbers (0-9)"))
//             AddCharactersFromString("0123456789");
//         if (GUILayout.Button("Currency"))
//             AddCharactersFromString("0123456789$.,");
//         if (GUILayout.Button("Game UI"))
//             AddCharactersFromString("0123456789+-×÷%:.,");
//         EditorGUILayout.EndHorizontal();
//         
//         EditorGUILayout.EndVertical();
//         GUILayout.Space(5);
//         
//         // === CHARACTER LIST ===
//         EditorGUILayout.BeginVertical(GUI.skin.box);
//         EditorGUILayout.BeginHorizontal();
//         GUILayout.Label($"Characters ({characters.Count})", EditorStyles.boldLabel);
//         GUILayout.FlexibleSpace();
//         if (GUILayout.Button("Add Character", GUILayout.Width(100)))
//         {
//             characters.Add(new FontCharacter());
//         }
//         if (GUILayout.Button("Clear All", GUILayout.Width(80)))
//         {
//             characters.Clear();
//             showPreview = false;
//             if (previewAtlas != null)
//             {
//                 DestroyImmediate(previewAtlas);
//                 previewAtlas = null;
//             }
//         }
//         EditorGUILayout.EndHorizontal();
//         
//         if (characters.Count > 0)
//         {
//             // Header
//             EditorGUILayout.BeginHorizontal();
//             GUILayout.Label("Char", EditorStyles.miniLabel, GUILayout.Width(40));
//             GUILayout.Label("Image", EditorStyles.miniLabel, GUILayout.Width(100));
//             GUILayout.Label("Preview", EditorStyles.miniLabel, GUILayout.Width(60));
//             GUILayout.Label("X Offset", EditorStyles.miniLabel, GUILayout.Width(60));
//             GUILayout.Label("Y Offset", EditorStyles.miniLabel, GUILayout.Width(60));
//             GUILayout.Label("X Advance", EditorStyles.miniLabel, GUILayout.Width(70));
//             GUILayout.Label("Auto", EditorStyles.miniLabel, GUILayout.Width(40));
//             GUILayout.Label("Del", EditorStyles.miniLabel, GUILayout.Width(25));
//             EditorGUILayout.EndHorizontal();
//             
//             // Character rows
//             for (int i = 0; i < characters.Count; i++)
//             {
//                 var ch = characters[i];
//                 
//                 EditorGUILayout.BeginHorizontal();
//                 
//                 // Character input
//                 string newChar = EditorGUILayout.TextField(ch.character, GUILayout.Width(40));
//                 if (newChar != ch.character)
//                 {
//                     ch.character = newChar;
//                     if (ch.useAutoAdvance && ch.sourceImage != null)
//                     {
//                         ch.xadvance = ch.sourceImage.width + packingSettings.padding;
//                     }
//                 }
//                 
//                 // Image assignment
//                 var newImage = (Texture2D)EditorGUILayout.ObjectField(ch.sourceImage, typeof(Texture2D), false, GUILayout.Width(100));
//                 if (newImage != ch.sourceImage)
//                 {
//                     ch.sourceImage = newImage;
//                     if (newImage != null && ch.useAutoAdvance)
//                     {
//                         ch.xadvance = newImage.width + packingSettings.padding;
//                     }
//                 }
//                 
//                 // Image preview
//                 if (ch.sourceImage != null)
//                 {
//                     Rect previewRect = GUILayoutUtility.GetRect(50, 50, GUILayout.Width(60));
//                     GUI.DrawTexture(previewRect, ch.sourceImage, ScaleMode.ScaleToFit);
//                 }
//                 else
//                 {
//                     GUILayout.Box("No Image", GUILayout.Width(60), GUILayout.Height(50));
//                 }
//                 
//                 // Offset and advance settings
//                 ch.xoffset = EditorGUILayout.IntField(ch.xoffset, GUILayout.Width(60));
//                 ch.yoffset = EditorGUILayout.IntField(ch.yoffset, GUILayout.Width(60));
//                 
//                 EditorGUI.BeginDisabledGroup(ch.useAutoAdvance);
//                 ch.xadvance = EditorGUILayout.IntField(ch.xadvance, GUILayout.Width(70));
//                 EditorGUI.EndDisabledGroup();
//                 
//                 ch.useAutoAdvance = EditorGUILayout.Toggle(ch.useAutoAdvance, GUILayout.Width(40));
//                 
//                 // Delete button
//                 if (GUILayout.Button("X", GUILayout.Width(25)))
//                 {
//                     characters.RemoveAt(i);
//                     i--;
//                     showPreview = false;
//                 }
//                 
//                 EditorGUILayout.EndHorizontal();
//             }
//         }
//         
//         EditorGUILayout.EndVertical();
//         GUILayout.Space(5);
//         
//         // === ACTIONS ===
//         EditorGUILayout.BeginVertical(GUI.skin.box);
//         GUILayout.Label("Actions", EditorStyles.boldLabel);
//         
//         // Validation info
//         var missingImages = characters.Where(c => string.IsNullOrEmpty(c.character) || c.sourceImage == null).ToList();
//         if (missingImages.Count > 0)
//         {
//             EditorGUILayout.HelpBox($"Warning: {missingImages.Count} character(s) missing image or character value", MessageType.Warning);
//         }
//         else if (characters.Count > 0)
//         {
//             EditorGUILayout.HelpBox($"Ready to generate! {characters.Count} characters with images", MessageType.Info);
//         }
//         
//         EditorGUI.BeginDisabledGroup(characters.Count == 0 || characters.Any(c => c.sourceImage == null || string.IsNullOrEmpty(c.character)));
//         
//         EditorGUILayout.BeginHorizontal();
//         if (GUILayout.Button("Generate Preview Atlas", GUILayout.Height(30)))
//         {
//             GeneratePreviewAtlas();
//         }
//         
//         if (GUILayout.Button("Generate Complete Font", GUILayout.Height(30)))
//         {
//             GenerateCompleteFont();
//         }
//         EditorGUILayout.EndHorizontal();
//         
//         // Trim final atlas button
//         if (showPreview && previewAtlas != null)
//         {
//             if (GUILayout.Button("Trim Final Atlas (Optimize)", GUILayout.Height(25)))
//             {
//                 TrimFinalAtlas();
//             }
//         }
//         
//         EditorGUI.EndDisabledGroup();
//         
//         // Auto-find and optimize buttons
//         EditorGUILayout.BeginHorizontal();
//         if (GUILayout.Button("Auto-Find Character Images"))
//         {
//             AutoFindAllCharacterImages();
//         }
//         
//         if (GUILayout.Button("Generate & Auto-Optimize"))
//         {
//             GenerateOptimizedFont();
//         }
//         EditorGUILayout.EndHorizontal();
//         
//         EditorGUILayout.EndVertical();
//         
//         // === PREVIEW ===
//         if (showPreview && previewAtlas != null)
//         {
//             GUILayout.Space(10);
//             EditorGUILayout.BeginVertical(GUI.skin.box);
//             GUILayout.Label($"Atlas Preview ({previewAtlas.width}x{previewAtlas.height})", EditorStyles.boldLabel);
//             
//             previewScroll = EditorGUILayout.BeginScrollView(previewScroll, GUILayout.Height(250));
//             
//             float scale = Mathf.Min(1f, 400f / previewAtlas.width);
//             float previewWidth = previewAtlas.width * scale;
//             float previewHeight = previewAtlas.height * scale;
//             
//             Rect previewRect = GUILayoutUtility.GetRect(previewWidth, previewHeight);
//             GUI.DrawTexture(previewRect, previewAtlas, ScaleMode.ScaleToFit);
//             
//             // Draw character bounds
//             foreach (var ch in characters)
//             {
//                 if (ch.atlasRect.width > 0)
//                 {
//                     Rect charRect = new Rect(
//                         previewRect.x + ch.atlasRect.x * scale,
//                         previewRect.y + ch.atlasRect.y * scale,
//                         ch.atlasRect.width * scale,
//                         ch.atlasRect.height * scale
//                     );
//                     
//                     // Draw border
//                     Color oldColor = GUI.color;
//                     GUI.color = Color.red;
//                     GUI.DrawTexture(charRect, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
//                     
//                     GUI.color = Color.white;
//                     charRect.x += 1; charRect.y += 1; charRect.width -= 2; charRect.height -= 2;
//                     if (ch.sourceImage != null)
//                     {
//                         GUI.DrawTexture(charRect, ch.sourceImage, ScaleMode.StretchToFill);
//                     }
//                     GUI.color = oldColor;
//                 }
//             }
//             
//             EditorGUILayout.EndScrollView();
//             EditorGUILayout.EndVertical();
//         }
//         
//         EditorGUILayout.EndScrollView();
//     }
//     
//     void AddCharactersFromString(string chars)
//     {
//         foreach (char c in chars)
//         {
//             if (!characters.Any(ch => ch.character == c.ToString()))
//             {
//                 var fontChar = new FontCharacter();
//                 fontChar.character = c.ToString();
//                 fontChar.xadvance = fontSize;
//                 characters.Add(fontChar);
//             }
//         }
//         
//         // Try auto-find images for new characters
//         AutoFindAllCharacterImages();
//     }
//     
//     void AutoFindAllCharacterImages()
//     {
//         string[] allTextures = AssetDatabase.FindAssets("t:Texture2D");
//         
//         foreach (var ch in characters)
//         {
//             if (ch.sourceImage != null || string.IsNullOrEmpty(ch.character)) continue;
//             
//             string[] searchPatterns = {
//                 ch.character,                                    // "0", "1", "+"
//                 "num_" + ch.character,                          // "num_0", "num_1"
//                 ch.character + "_",                             // "0_", "1_"
//                 "char_" + ((int)ch.character[0]).ToString(),    // "char_48" (ASCII)
//                 "icon_" + ch.character,                         // "icon_0"
//                 "number_" + ch.character,                       // "number_0"
//             };
//             
//             foreach (string guid in allTextures)
//             {
//                 string path = AssetDatabase.GUIDToAssetPath(guid);
//                 string filename = Path.GetFileNameWithoutExtension(path).ToLower();
//                 
//                 foreach (string pattern in searchPatterns)
//                 {
//                     if (filename == pattern.ToLower() || filename.EndsWith(pattern.ToLower()))
//                     {
//                         ch.sourceImage = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
//                         if (ch.useAutoAdvance)
//                         {
//                             ch.xadvance = ch.sourceImage.width + packingSettings.padding;
//                         }
//                         Debug.Log($"Found image for '{ch.character}': {Path.GetFileName(path)}");
//                         goto NextCharacter;
//                     }
//                 }
//             }
//             
//             Debug.LogWarning($"Could not find image for character '{ch.character}'");
//             NextCharacter:;
//         }
//     }
//     
//     void GenerateOptimizedFont()
//     {
//         var validChars = characters.Where(c => c.sourceImage != null && !string.IsNullOrEmpty(c.character)).ToList();
//         if (validChars.Count == 0)
//         {
//             EditorUtility.DisplayDialog("Error", "No valid characters with images found!", "OK");
//             return;
//         }
//         
//         Debug.Log($"Starting optimized font generation for {validChars.Count} characters...");
//         
//         // Calculate optimal tight-packed layout first
//         var (atlasSize, layout) = CalculateTightLayout(validChars);
//         
//         // Create atlas with minimal size
//         Texture2D atlas = CreateOptimizedAtlas(validChars, layout, atlasSize);
//         
//         if (atlas == null)
//         {
//             EditorUtility.DisplayDialog("Error", "Could not create optimized atlas!", "OK");
//             return;
//         }
//         
//         // Ensure output directory exists
//         if (!AssetDatabase.IsValidFolder(outputFolder))
//         {
//             string[] folders = outputFolder.Split('/');
//             string currentPath = folders[0];
//             for (int i = 1; i < folders.Length; i++)
//             {
//                 string newPath = currentPath + "/" + folders[i];
//                 if (!AssetDatabase.IsValidFolder(newPath))
//                 {
//                     AssetDatabase.CreateFolder(currentPath, folders[i]);
//                 }
//                 currentPath = newPath;
//             }
//         }
//         
//         // Save optimized atlas
//         string atlasPath = (outputFolder + "/" + fontName + ".png").Replace('\\', '/');
//         byte[] pngData = atlas.EncodeToPNG();
//         File.WriteAllBytes(atlasPath, pngData);
//         
//         // Generate .fnt file with correct coordinates
//         string fntPath = (outputFolder + "/" + fontName + ".fnt").Replace('\\', '/');
//         GenerateFntFile(validChars, fntPath, new Vector2Int(atlas.width, atlas.height), fontName + ".png");
//         
//         AssetDatabase.Refresh();
//         
//         // Import atlas with proper settings
//         TextureImporter atlasImporter = AssetImporter.GetAtPath(atlasPath) as TextureImporter;
//         if (atlasImporter != null)
//         {
//             atlasImporter.textureType = TextureImporterType.GUI;
//             atlasImporter.alphaIsTransparency = true;
//             atlasImporter.mipmapEnabled = false;
//             atlasImporter.filterMode = FilterMode.Point;
//             atlasImporter.wrapMode = TextureWrapMode.Clamp;
//             AssetDatabase.ImportAsset(atlasPath);
//         }
//         
//         // Create Unity Font components
//         Texture2D atlasTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(atlasPath);
//         
//         // Create material
//         string matPath = (outputFolder + "/" + fontName + ".mat").Replace('\\', '/');
//         Material mat = new Material(Shader.Find("GUI/Text Shader"));
//         mat.mainTexture = atlasTexture;
//         mat.name = fontName;
//         AssetDatabase.CreateAsset(mat, matPath);
//         
//         // Create font settings
//         string fontSettingsPath = (outputFolder + "/" + fontName + ".fontsettings").Replace('\\', '/');
//         Font customFont = new Font();
//         customFont.material = mat;
//         
//         TextAsset fntAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(fntPath);
//         if (fntAsset != null)
//         {
//             SetFontCharacterInfo(customFont, fntAsset.text, atlasTexture);
//         }
//         
//         AssetDatabase.CreateAsset(customFont, fontSettingsPath);
//         AssetDatabase.Refresh();
//         
//         DestroyImmediate(atlas);
//         
//         string resultMessage = $"Optimized font '{fontName}' generated!\n\n" +
//                               $"Files created:\n" +
//                               $"• {Path.GetFileName(atlasPath)}\n" +
//                               $"• {Path.GetFileName(fntPath)}\n" +
//                               $"• {fontName}.fontsettings\n" +
//                               $"• {fontName}.mat\n\n" +
//                               $"Atlas: {atlasTexture.width}x{atlasTexture.height} (minimized)\n" +
//                               $"Characters: {validChars.Count}";
//         
//         EditorUtility.DisplayDialog("Success!", resultMessage, "Awesome!");
//         
//         // Select generated font
//         Font generatedFont = AssetDatabase.LoadAssetAtPath<Font>(fontSettingsPath);
//         if (generatedFont != null)
//         {
//             Selection.activeObject = generatedFont;
//             EditorGUIUtility.PingObject(generatedFont);
//         }
//         
//         Debug.Log($"Optimized font generation finished for '{fontName}'!");
//     }
//     
//     (Vector2Int, List<Rect>) CalculateTightLayout(List<FontCharacter> chars)
//     {
//         // Calculate minimum required size
//         int totalArea = 0;
//         int maxWidth = 0;
//         int maxHeight = 0;
//         
//         foreach (var ch in chars)
//         {
//             if (ch.sourceImage != null)
//             {
//                 totalArea += (ch.sourceImage.width + packingSettings.padding) * (ch.sourceImage.height + packingSettings.padding);
//                 maxWidth = Mathf.Max(maxWidth, ch.sourceImage.width + packingSettings.padding);
//                 maxHeight = Mathf.Max(maxHeight, ch.sourceImage.height + packingSettings.padding);
//             }
//         }
//         
//         // Try to find the most square-like atlas that can fit everything
//         int minDimension = Mathf.Max(maxWidth, maxHeight, Mathf.CeilToInt(Mathf.Sqrt(totalArea)));
//         
//         // Start with minimum and grow if needed
//         for (int size = minDimension; size <= packingSettings.maxAtlasSize; size += (packingSettings.powerOfTwo ? size : 32))
//         {
//             if (packingSettings.powerOfTwo)
//             {
//                 size = GetNextPowerOfTwo(size);
//             }
//             
//             var layout = TryTightPackCharacters(chars, new Vector2Int(size, size));
//             if (layout != null)
//             {
//                 // Update character rects
//                 for (int i = 0; i < chars.Count; i++)
//                 {
//                     if (i < layout.Count && chars[i].sourceImage != null)
//                     {
//                         chars[i].atlasRect = layout[i];
//                     }
//                 }
//                 
//                 return (new Vector2Int(size, size), layout);
//             }
//         }
//         
//         // Fallback to simple grid layout with minimal size
//         return CalculateGridLayout(chars);
//     }
//     
//     int GetNextPowerOfTwo(int value)
//     {
//         int power = 32;
//         while (power < value && power < 2048)
//             power *= 2;
//         return power;
//     }
//     
//     List<Rect> TryTightPackCharacters(List<FontCharacter> chars, Vector2Int atlasSize)
//     {
//         List<Rect> layout = new List<Rect>();
//         
//         // Sort by height descending for better packing
//         var sortedChars = chars.Where(c => c.sourceImage != null).OrderByDescending(c => c.sourceImage.height).ThenByDescending(c => c.sourceImage.width).ToList();
//         
//         int currentY = packingSettings.padding;
//         int currentRowHeight = 0;
//         
//         while (sortedChars.Count > 0)
//         {
//             int currentX = packingSettings.padding;
//             List<FontCharacter> rowChars = new List<FontCharacter>();
//             
//             // Try to fit characters in current row
//             for (int i = sortedChars.Count - 1; i >= 0; i--)
//             {
//                 var ch = sortedChars[i];
//                 int charWidth = ch.sourceImage.width + packingSettings.padding;
//                 int charHeight = ch.sourceImage.height + packingSettings.padding;
//                 
//                 if (currentX + charWidth <= atlasSize.x && currentY + charHeight <= atlasSize.y)
//                 {
//                     layout.Add(new Rect(currentX, currentY, ch.sourceImage.width, ch.sourceImage.height));
//                     currentX += charWidth;
//                     currentRowHeight = Mathf.Max(currentRowHeight, charHeight);
//                     rowChars.Add(ch);
//                     sortedChars.RemoveAt(i);
//                 }
//             }
//             
//             if (rowChars.Count == 0)
//             {
//                 // Can't fit any more characters
//                 return null;
//             }
//             
//             currentY += currentRowHeight;
//             currentRowHeight = 0;
//         }
//         
//         return layout;
//     }
//     
//     (Vector2Int, List<Rect>) CalculateGridLayout(List<FontCharacter> chars)
//     {
//         var validChars = chars.Where(c => c.sourceImage != null).ToList();
//         if (validChars.Count == 0) return (Vector2Int.zero, new List<Rect>());
//         
//         // Calculate grid dimensions
//         int columns = Mathf.CeilToInt(Mathf.Sqrt(validChars.Count));
//         int rows = Mathf.CeilToInt((float)validChars.Count / columns);
//         
//         // Calculate cell size based on largest character
//         int maxCharWidth = validChars.Max(c => c.sourceImage.width);
//         int maxCharHeight = validChars.Max(c => c.sourceImage.height);
//         
//         int cellWidth = maxCharWidth + packingSettings.padding * 2;
//         int cellHeight = maxCharHeight + packingSettings.padding * 2;
//         
//         int atlasWidth = columns * cellWidth;
//         int atlasHeight = rows * cellHeight;
//         
//         // Adjust to power of 2 if needed
//         if (packingSettings.powerOfTwo)
//         {
//             atlasWidth = GetNextPowerOfTwo(atlasWidth);
//             atlasHeight = GetNextPowerOfTwo(atlasHeight);
//         }
//         
//         List<Rect> layout = new List<Rect>();
//         
//         for (int i = 0; i < validChars.Count; i++)
//         {
//             int row = i / columns;
//             int col = i % columns;
//             
//             int x = col * cellWidth + packingSettings.padding;
//             int y = row * cellHeight + packingSettings.padding;
//             
//             layout.Add(new Rect(x, y, validChars[i].sourceImage.width, validChars[i].sourceImage.height));
//             chars[chars.IndexOf(validChars[i])].atlasRect = layout[i];
//         }
//         
//         return (new Vector2Int(atlasWidth, atlasHeight), layout);
//     }
//     
//     Texture2D CreateOptimizedAtlas(List<FontCharacter> chars, List<Rect> layout, Vector2Int atlasSize)
//     {
//         var validChars = chars.Where(c => c.sourceImage != null && !string.IsNullOrEmpty(c.character)).ToList();
//         
//         Texture2D atlas = new Texture2D(atlasSize.x, atlasSize.y, TextureFormat.RGBA32, false);
//         
//         // Clear with background color
//         Color[] clearPixels = new Color[atlasSize.x * atlasSize.y];
//         for (int i = 0; i < clearPixels.Length; i++)
//             clearPixels[i] = packingSettings.backgroundColor;
//         atlas.SetPixels(clearPixels);
//         
//         // Copy each character to calculated position
//         for (int i = 0; i < validChars.Count && i < layout.Count; i++)
//         {
//             var ch = validChars[i];
//             var rect = layout[i];
//             
//             // Make texture readable
//             string path = AssetDatabase.GetAssetPath(ch.sourceImage);
//             TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
//             bool wasReadable = importer.isReadable;
//             
//             if (!wasReadable)
//             {
//                 importer.isReadable = true;
//                 AssetDatabase.ImportAsset(path);
//             }
//             
//             try
//             {
//                 Color[] sourcePixels = ch.sourceImage.GetPixels();
//                 int targetX = Mathf.RoundToInt(rect.x);
//                 int targetY = Mathf.RoundToInt(rect.y);
//                 
//                 atlas.SetPixels(targetX, targetY, ch.sourceImage.width, ch.sourceImage.height, sourcePixels);
//                 Debug.Log($"Placed '{ch.character}' at ({targetX},{targetY})");
//             }
//             catch (System.Exception ex)
//             {
//                 Debug.LogError($"Error placing character '{ch.character}': {ex.Message}");
//             }
//             
//             // Restore texture settings
//             if (!wasReadable)
//             {
//                 importer.isReadable = false;
//                 AssetDatabase.ImportAsset(path);
//             }
//         }
//         
//         atlas.Apply();
//         
//         // Now trim the final atlas to remove empty space
//         return TrimAtlasToContent(atlas, validChars, layout);
//     }
//     
//     Texture2D TrimAtlasToContent(Texture2D originalAtlas, List<FontCharacter> chars, List<Rect> layout)
//     {
//         // Find actual content bounds
//         int minX = originalAtlas.width, maxX = -1;
//         int minY = originalAtlas.height, maxY = -1;
//         
//         Color[] pixels = originalAtlas.GetPixels();
//         
//         for (int y = 0; y < originalAtlas.height; y++)
//         {
//             for (int x = 0; x < originalAtlas.width; x++)
//             {
//                 Color pixel = pixels[y * originalAtlas.width + x];
//                 
//                 if (pixel.a > 0.01f && !ColorsEqual(pixel, packingSettings.backgroundColor, 0.02f))
//                 {
//                     minX = Mathf.Min(minX, x);
//                     maxX = Mathf.Max(maxX, x);
//                     minY = Mathf.Min(minY, y);
//                     maxY = Mathf.Max(maxY, y);
//                 }
//             }
//         }
//         
//         if (maxX < 0)
//         {
//             Debug.LogError("No content found in atlas!");
//             return originalAtlas;
//         }
//         
//         // Add minimal padding
//         int trimPadding = 1;
//         minX = Mathf.Max(0, minX - trimPadding);
//         minY = Mathf.Max(0, minY - trimPadding);
//         maxX = Mathf.Min(originalAtlas.width - 1, maxX + trimPadding);
//         maxY = Mathf.Min(originalAtlas.height - 1, maxY + trimPadding);
//         
//         int trimmedWidth = maxX - minX + 1;
//         int trimmedHeight = maxY - minY + 1;
//         
//         // Create trimmed atlas
//         Texture2D trimmedAtlas = new Texture2D(trimmedWidth, trimmedHeight, TextureFormat.RGBA32, false);
//         Color[] trimmedPixels = new Color[trimmedWidth * trimmedHeight];
//         
//         for (int y = 0; y < trimmedHeight; y++)
//         {
//             for (int x = 0; x < trimmedWidth; x++)
//             {
//                 int sourceX = x + minX;
//                 int sourceY = y + minY;
//                 trimmedPixels[y * trimmedWidth + x] = pixels[sourceY * originalAtlas.width + sourceX];
//             }
//         }
//         
//         trimmedAtlas.SetPixels(trimmedPixels);
//         trimmedAtlas.Apply();
//         
//         // Update character positions
//         Vector2 offset = new Vector2(minX, minY);
//         for (int i = 0; i < chars.Count && i < layout.Count; i++)
//         {
//             chars[i].atlasRect = new Rect(
//                 layout[i].x - offset.x,
//                 layout[i].y - offset.y,
//                 layout[i].width,
//                 layout[i].height
//             );
//         }
//         
//         DestroyImmediate(originalAtlas);
//         
//         float savings = (1f - (float)(trimmedWidth * trimmedHeight) / (originalAtlas.width * originalAtlas.height)) * 100f;
//         Debug.Log($"Atlas optimized: {originalAtlas.width}x{originalAtlas.height} -> {trimmedWidth}x{trimmedHeight} (saved {savings:F1}%)");
//         
//         return trimmedAtlas;
//     }
//     
//     bool ColorsEqual(Color a, Color b, float tolerance)
//     {
//         return Mathf.Abs(a.r - b.r) < tolerance && 
//                Mathf.Abs(a.g - b.g) < tolerance && 
//                Mathf.Abs(a.b - b.b) < tolerance && 
//                Mathf.Abs(a.a - b.a) < tolerance;
//     }
//     
//     void TrimFinalAtlas()
//     {
//         if (previewAtlas == null)
//         {
//             Debug.LogError("No atlas to trim! Generate preview first.");
//             return;
//         }
//         
//         Debug.Log("Starting final atlas trimming...");
//         
//         // Find bounds of all character content
//         int minX = previewAtlas.width, maxX = -1;
//         int minY = previewAtlas.height, maxY = -1;
//         
//         Color[] atlasPixels = previewAtlas.GetPixels();
//         
//         for (int y = 0; y < previewAtlas.height; y++)
//         {
//             for (int x = 0; x < previewAtlas.width; x++)
//             {
//                 Color pixel = atlasPixels[y * previewAtlas.width + x];
//                 
//                 // Check if pixel has content (not transparent and not background color)
//                 bool hasContent = pixel.a > 0.01f;
//                 if (packingSettings.backgroundColor.a > 0.01f)
//                 {
//                     // If background has color, check if pixel is different from background
//                     hasContent = hasContent && !ColorsEqual(pixel, packingSettings.backgroundColor, 0.02f);
//                 }
//                 
//                 if (hasContent)
//                 {
//                     minX = Mathf.Min(minX, x);
//                     maxX = Mathf.Max(maxX, x);
//                     minY = Mathf.Min(minY, y);
//                     maxY = Mathf.Max(maxY, y);
//                 }
//             }
//         }
//         
//         if (maxX < 0 || maxY < 0)
//         {
//             Debug.LogError("No content found in atlas!");
//             return;
//         }
//         
//         // Add small padding to trimmed atlas
//         int trimPadding = Mathf.Max(1, packingSettings.padding);
//         minX = Mathf.Max(0, minX - trimPadding);
//         minY = Mathf.Max(0, minY - trimPadding);
//         maxX = Mathf.Min(previewAtlas.width - 1, maxX + trimPadding);
//         maxY = Mathf.Min(previewAtlas.height - 1, maxY + trimPadding);
//         
//         int trimmedWidth = maxX - minX + 1;
//         int trimmedHeight = maxY - minY + 1;
//         
//         // Check if trimming is beneficial
//         float originalSize = previewAtlas.width * previewAtlas.height;
//         float trimmedSize = trimmedWidth * trimmedHeight;
//         float savings = (originalSize - trimmedSize) / originalSize * 100;
//         
//         if (savings < 1f)
//         {
//             Debug.Log($"Atlas already well-optimized (only {savings:F1}% savings possible)");
//             return;
//         }
//         
//         // Create trimmed atlas
//         Texture2D trimmedAtlas = new Texture2D(trimmedWidth, trimmedHeight, TextureFormat.RGBA32, false);
//         Color[] trimmedPixels = new Color[trimmedWidth * trimmedHeight];
//         
//         for (int y = 0; y < trimmedHeight; y++)
//         {
//             for (int x = 0; x < trimmedWidth; x++)
//             {
//                 int sourceX = x + minX;
//                 int sourceY = y + minY;
//                 trimmedPixels[y * trimmedWidth + x] = atlasPixels[sourceY * previewAtlas.width + sourceX];
//             }
//         }
//         
//         trimmedAtlas.SetPixels(trimmedPixels);
//         trimmedAtlas.Apply();
//         
//         // Update character atlas rects with new coordinates
//         Vector2 offset = new Vector2(minX, minY);
//         foreach (var ch in characters)
//         {
//             if (ch.atlasRect.width > 0)
//             {
//                 ch.atlasRect.x -= offset.x;
//                 ch.atlasRect.y -= offset.y;
//             }
//         }
//         
//         // Replace preview atlas
//         DestroyImmediate(previewAtlas);
//         previewAtlas = trimmedAtlas;
//         
//         Debug.Log($"Atlas trimmed successfully: {previewAtlas.width + minX}x{previewAtlas.height + minY} -> {trimmedWidth}x{trimmedHeight} (saved {savings:F1}%)");
//     }
//     
//     void GeneratePreviewAtlas()
//     {
//         var validChars = characters.Where(c => c.sourceImage != null && !string.IsNullOrEmpty(c.character)).ToList();
//         if (validChars.Count == 0) 
//         {
//             Debug.LogError("No valid characters to preview!");
//             return;
//         }
//         
//         Debug.Log($"Generating preview for {validChars.Count} characters...");
//         
//         // Calculate optimal atlas size
//         var (atlasSize, layout) = CalculateOptimalLayout(validChars);
//         
//         if (layout == null || layout.Count == 0)
//         {
//             EditorUtility.DisplayDialog("Error", $"Could not pack characters into atlas of max size {packingSettings.maxAtlasSize}x{packingSettings.maxAtlasSize}. Try increasing max atlas size or reducing character image sizes.", "OK");
//             return;
//         }
//         
//         if (previewAtlas != null)
//         {
//             DestroyImmediate(previewAtlas);
//         }
//         
//         previewAtlas = CreateAtlasTexture(validChars, layout, atlasSize);
//         showPreview = true;
//         
//         Debug.Log($"Preview generated successfully: {atlasSize.x}x{atlasSize.y} atlas with {validChars.Count} characters");
//     }
//     
//     void GenerateCompleteFont()
//     {
//         var validChars = characters.Where(c => c.sourceImage != null && !string.IsNullOrEmpty(c.character)).ToList();
//         if (validChars.Count == 0)
//         {
//             EditorUtility.DisplayDialog("Error", "No valid characters with images found!", "OK");
//             return;
//         }
//         
//         Debug.Log($"Starting complete font generation for {validChars.Count} characters...");
//         
//         // Ensure output directory exists
//         if (!AssetDatabase.IsValidFolder(outputFolder))
//         {
//             string[] folders = outputFolder.Split('/');
//             string currentPath = folders[0];
//             for (int i = 1; i < folders.Length; i++)
//             {
//                 string newPath = currentPath + "/" + folders[i];
//                 if (!AssetDatabase.IsValidFolder(newPath))
//                 {
//                     AssetDatabase.CreateFolder(currentPath, folders[i]);
//                 }
//                 currentPath = newPath;
//             }
//         }
//         
//         // Calculate optimal layout
//         var (atlasSize, layout) = CalculateOptimalLayout(validChars);
//         
//         if (layout == null)
//         {
//             EditorUtility.DisplayDialog("Error", $"Could not pack characters! Try increasing max atlas size to {packingSettings.maxAtlasSize * 2}.", "OK");
//             return;
//         }
//         
//         // Generate atlas texture
//         Texture2D finalAtlas = CreateAtlasTexture(validChars, layout, atlasSize);
//         
//         // Save atlas PNG
//         string atlasPath = (outputFolder + "/" + fontName + ".png").Replace('\\', '/');
//         byte[] pngData = finalAtlas.EncodeToPNG();
//         File.WriteAllBytes(atlasPath, pngData);
//         
//         // Generate .fnt file
//         string fntPath = (outputFolder + "/" + fontName + ".fnt").Replace('\\', '/');
//         GenerateFntFile(validChars, fntPath, atlasSize, fontName + ".png");
//         
//         AssetDatabase.Refresh();
//         
//         // Import atlas with proper settings
//         TextureImporter atlasImporter = AssetImporter.GetAtPath(atlasPath) as TextureImporter;
//         if (atlasImporter != null)
//         {
//             atlasImporter.textureType = TextureImporterType.GUI;
//             atlasImporter.alphaIsTransparency = true;
//             atlasImporter.mipmapEnabled = false;
//             atlasImporter.filterMode = FilterMode.Point;
//             atlasImporter.wrapMode = TextureWrapMode.Clamp;
//             AssetDatabase.ImportAsset(atlasPath);
//             Debug.Log($"Atlas imported: {atlasPath}");
//         }
//         
//         // Wait for import to complete
//         AssetDatabase.Refresh();
//         
//         // Create material
//         Texture2D atlasTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(atlasPath);
//         string matPath = (outputFolder + "/" + fontName + ".mat").Replace('\\', '/');
//         
//         Material mat = new Material(Shader.Find("GUI/Text Shader"));
//         mat.mainTexture = atlasTexture;
//         mat.name = fontName;
//         AssetDatabase.CreateAsset(mat, matPath);
//         
//         // Create font settings
//         string fontSettingsPath = (outputFolder + "/" + fontName + ".fontsettings").Replace('\\', '/');
//         Font customFont = new Font();
//         customFont.material = mat;
//         
//         // Parse fnt and set character info
//         TextAsset fntAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(fntPath);
//         if (fntAsset != null)
//         {
//             SetFontCharacterInfo(customFont, fntAsset.text, atlasTexture);
//         }
//         
//         AssetDatabase.CreateAsset(customFont, fontSettingsPath);
//         AssetDatabase.Refresh();
//         
//         DestroyImmediate(finalAtlas);
//         
//         string resultMessage = $"Font '{fontName}' generated successfully!\n\n" +
//                               $"Files created:\n" +
//                               $"• {Path.GetFileName(atlasPath)} - Atlas texture\n" +
//                               $"• {Path.GetFileName(fntPath)} - BMFont data\n" +
//                               $"• {fontName}.fontsettings - Unity Font\n" +
//                               $"• {fontName}.mat - Font Material\n\n" +
//                               $"Atlas: {atlasSize.x}x{atlasSize.y} (optimized)\n" +
//                               $"Characters: {validChars.Count}";
//         
//         EditorUtility.DisplayDialog("Success!", resultMessage, "Awesome!");
//         
//         // Select generated font
//         Font generatedFont = AssetDatabase.LoadAssetAtPath<Font>(fontSettingsPath);
//         if (generatedFont != null)
//         {
//             Selection.activeObject = generatedFont;
//             EditorGUIUtility.PingObject(generatedFont);
//         }
//         
//         Debug.Log($"Complete font generation finished for '{fontName}'!");
//     }
//     
//     void SetFontCharacterInfo(Font font, string fntContent, Texture2D texture)
//     {
//         List<CharacterInfo> charList = new List<CharacterInfo>();
//         
//         string[] lines = fntContent.Split('\n');
//         int lineHeight = 65;
//         int texWidth = texture.width;
//         int texHeight = texture.height;
//         
//         Regex charReg = new Regex(@"char id=(?<id>\d+)\s+x=(?<x>\d+)\s+y=(?<y>\d+)\s+width=(?<width>\d+)\s+height=(?<height>\d+)\s+xoffset=(?<xoffset>(-|\d)+)\s+yoffset=(?<yoffset>(-|\d)+)\s+xadvance=(?<xadvance>\d+)");
//         Regex commonReg = new Regex(@"common lineHeight=(?<lineHeight>\d+)");
//         
//         foreach (string line in lines)
//         {
//             if (line.IndexOf("common lineHeight=") != -1)
//             {
//                 var commonMatch = commonReg.Match(line);
//                 if (commonMatch.Success)
//                 {
//                     lineHeight = int.Parse(commonMatch.Groups["lineHeight"].Value);
//                 }
//             }
//             else if (line.IndexOf("char id=") != -1)
//             {
//                 var match = charReg.Match(line);
//                 if (match.Success)
//                 {
//                     var id = int.Parse(match.Groups["id"].Value);
//                     var x = int.Parse(match.Groups["x"].Value);
//                     var y = int.Parse(match.Groups["y"].Value);
//                     var width = int.Parse(match.Groups["width"].Value);
//                     var height = int.Parse(match.Groups["height"].Value);
//                     var xoffset = int.Parse(match.Groups["xoffset"].Value);
//                     var yoffset = int.Parse(match.Groups["yoffset"].Value);
//                     var xadvance = int.Parse(match.Groups["xadvance"].Value);
//                     
//                     CharacterInfo info = new CharacterInfo();
//                     info.index = id;
//                     
//                     // Calculate UV coordinates
//                     float uvx = (float)x / texWidth;
//                     float uvy = 1 - (float)(y + height) / texHeight;
//                     float uvw = (float)width / texWidth;
//                     float uvh = (float)height / texHeight;
//                     
//                     info.uvBottomLeft = new Vector2(uvx, uvy);
//                     info.uvBottomRight = new Vector2(uvx + uvw, uvy);
//                     info.uvTopLeft = new Vector2(uvx, uvy + uvh);
//                     info.uvTopRight = new Vector2(uvx + uvw, uvy + uvh);
//                     
//                     info.minX = xoffset;
//                     info.minY = yoffset - height;
//                     info.glyphWidth = width;
//                     info.glyphHeight = height;
//                     info.advance = xadvance;
//                     
//                     charList.Add(info);
//                 }
//             }
//         }
//         
//         font.characterInfo = charList.ToArray();
//     }
//     
//     (Vector2Int, List<Rect>) CalculateOptimalLayout(List<FontCharacter> chars)
//     {
//         // Sort characters by height (tallest first) for better packing
//         var sortedChars = chars.OrderByDescending(c => c.sourceImage.height).ThenByDescending(c => c.sourceImage.width).ToList();
//         
//         // Try different atlas sizes to find optimal
//         int[] possibleSizes = packingSettings.powerOfTwo ? 
//             new int[] { 128, 256, 512, 1024, 2048 } : 
//             new int[] { 200, 300, 400, 500, 600, 800, 1000, 1200 };
//         
//         foreach (int size in possibleSizes)
//         {
//             if (size > packingSettings.maxAtlasSize) break;
//             
//             var layout = TryPackCharacters(sortedChars, new Vector2Int(size, size));
//             if (layout != null)
//             {
//                 // Update character atlas rects
//                 for (int i = 0; i < chars.Count; i++)
//                 {
//                     var origChar = chars[i];
//                     var sortedIndex = sortedChars.FindIndex(c => c == origChar);
//                     if (sortedIndex >= 0 && sortedIndex < layout.Count)
//                     {
//                         origChar.atlasRect = layout[sortedIndex];
//                         origChar.actualWidth = (int)layout[sortedIndex].width;
//                         origChar.actualHeight = (int)layout[sortedIndex].height;
//                     }
//                 }
//                 
//                 return (new Vector2Int(size, size), layout);
//             }
//         }
//         
//         // Fallback to max size with simple grid layout
//         Vector2Int fallbackSize = new Vector2Int(packingSettings.maxAtlasSize, packingSettings.maxAtlasSize);
//         var fallbackLayout = ForcePackCharacters(sortedChars, fallbackSize);
//         
//         for (int i = 0; i < chars.Count; i++)
//         {
//             var origChar = chars[i];
//             var sortedIndex = sortedChars.FindIndex(c => c == origChar);
//             if (sortedIndex >= 0 && sortedIndex < fallbackLayout.Count)
//             {
//                 origChar.atlasRect = fallbackLayout[sortedIndex];
//                 origChar.actualWidth = (int)fallbackLayout[sortedIndex].width;
//                 origChar.actualHeight = (int)fallbackLayout[sortedIndex].height;
//             }
//         }
//         
//         return (fallbackSize, fallbackLayout);
//     }
//     
//     List<Rect> TryPackCharacters(List<FontCharacter> chars, Vector2Int atlasSize)
//     {
//         List<Rect> layout = new List<Rect>();
//         List<Rect> usedRects = new List<Rect>();
//         
//         foreach (var ch in chars)
//         {
//             if (ch.sourceImage == null) continue;
//             
//             int charWidth = ch.sourceImage.width + packingSettings.padding * 2;
//             int charHeight = ch.sourceImage.height + packingSettings.padding * 2;
//             
//             // Validate character fits in atlas
//             if (charWidth > atlasSize.x || charHeight > atlasSize.y)
//             {
//                 Debug.LogWarning($"Character '{ch.character}' too large for atlas ({charWidth}x{charHeight} > {atlasSize.x}x{atlasSize.y})");
//                 return null;
//             }
//             
//             Rect bestRect = FindBestPosition(usedRects, atlasSize, charWidth, charHeight);
//             
//             if (bestRect.width == 0) // Couldn't fit
//             {
//                 Debug.Log($"Couldn't fit character '{ch.character}' in {atlasSize.x}x{atlasSize.y} atlas");
//                 return null;
//             }
//             
//             layout.Add(bestRect);
//             usedRects.Add(bestRect);
//         }
//         
//         Debug.Log($"Successfully packed {layout.Count} characters in {atlasSize.x}x{atlasSize.y} atlas");
//         return layout;
//     }
//     
//     List<Rect> ForcePackCharacters(List<FontCharacter> chars, Vector2Int atlasSize)
//     {
//         List<Rect> layout = new List<Rect>();
//         
//         int x = packingSettings.padding;
//         int y = packingSettings.padding;
//         int rowHeight = 0;
//         
//         foreach (var ch in chars)
//         {
//             if (ch.sourceImage == null) continue;
//             
//             int charWidth = ch.sourceImage.width;
//             int charHeight = ch.sourceImage.height;
//             
//             // Check if we need to move to next row
//             if (x + charWidth + packingSettings.padding > atlasSize.x)
//             {
//                 x = packingSettings.padding;
//                 y += rowHeight + packingSettings.padding;
//                 rowHeight = 0;
//                 
//                 // Check if we exceed atlas height
//                 if (y + charHeight + packingSettings.padding > atlasSize.y)
//                 {
//                     Debug.LogError($"Atlas too small! Characters exceed {atlasSize.x}x{atlasSize.y}");
//                     break;
//                 }
//             }
//             
//             // Store layout for the actual texture area
//             layout.Add(new Rect(x, y, charWidth, charHeight));
//             
//             x += charWidth + packingSettings.padding;
//             rowHeight = Mathf.Max(rowHeight, charHeight);
//         }
//         
//         return layout;
//     }
//     
//     Rect FindBestPosition(List<Rect> usedRects, Vector2Int atlasSize, int width, int height)
//     {
//         // Try to find best position with minimum waste
//         for (int y = packingSettings.padding; y <= atlasSize.y - height; y += 1)
//         {
//             for (int x = packingSettings.padding; x <= atlasSize.x - width; x += 1)
//             {
//                 Rect testRect = new Rect(x, y, width - packingSettings.padding * 2, height - packingSettings.padding * 2);
//                 
//                 bool overlaps = false;
//                 foreach (var used in usedRects)
//                 {
//                     if (testRect.Overlaps(used))
//                     {
//                         overlaps = true;
//                         break;
//                     }
//                 }
//                 
//                 if (!overlaps)
//                 {
//                     return testRect;
//                 }
//             }
//         }
//         
//         return new Rect(0, 0, 0, 0); // Couldn't fit
//     }
//     
//     Texture2D CreateAtlasTexture(List<FontCharacter> chars, List<Rect> layout, Vector2Int atlasSize)
//     {
//         Texture2D atlas = new Texture2D(atlasSize.x, atlasSize.y, TextureFormat.RGBA32, false);
//         
//         // Clear with background color
//         Color[] clearPixels = new Color[atlasSize.x * atlasSize.y];
//         for (int i = 0; i < clearPixels.Length; i++)
//             clearPixels[i] = packingSettings.backgroundColor;
//         atlas.SetPixels(clearPixels);
//         
//         // Copy each character
//         var sortedChars = chars.OrderByDescending(c => c.sourceImage?.height ?? 0).ThenByDescending(c => c.sourceImage?.width ?? 0).ToList();
//         
//         for (int i = 0; i < sortedChars.Count && i < layout.Count; i++)
//         {
//             var ch = sortedChars[i];
//             if (ch.sourceImage == null) continue;
//             
//             var rect = layout[i];
//             
//             // Make texture readable
//             string path = AssetDatabase.GetAssetPath(ch.sourceImage);
//             TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
//             bool wasReadable = importer.isReadable;
//             
//             if (!wasReadable)
//             {
//                 importer.isReadable = true;
//                 AssetDatabase.ImportAsset(path);
//             }
//             
//             try
//             {
//                 // Get source pixels
//                 Color[] sourcePixels = ch.sourceImage.GetPixels();
//                 
//                 // Calculate target position with bounds checking
//                 int targetX = Mathf.RoundToInt(rect.x);
//                 int targetY = Mathf.RoundToInt(rect.y);
//                 int sourceWidth = ch.sourceImage.width;
//                 int sourceHeight = ch.sourceImage.height;
//                 
//                 // Validate bounds before SetPixels
//                 if (targetX + sourceWidth <= atlasSize.x && 
//                     targetY + sourceHeight <= atlasSize.y &&
//                     targetX >= 0 && targetY >= 0)
//                 {
//                     atlas.SetPixels(targetX, targetY, sourceWidth, sourceHeight, sourcePixels);
//                     Debug.Log($"Placed '{ch.character}' at ({targetX},{targetY}) size {sourceWidth}x{sourceHeight}");
//                 }
//                 else
//                 {
//                     Debug.LogError($"Character '{ch.character}' doesn't fit! " +
//                                  $"Target:({targetX},{targetY}) Size:{sourceWidth}x{sourceHeight} " +
//                                  $"Atlas:{atlasSize.x}x{atlasSize.y}");
//                 }
//             }
//             catch (System.Exception ex)
//             {
//                 Debug.LogError($"Error placing character '{ch.character}': {ex.Message}");
//             }
//             
//             // Restore texture settings
//             if (!wasReadable)
//             {
//                 importer.isReadable = false;
//                 AssetDatabase.ImportAsset(path);
//             }
//         }
//         
//         atlas.Apply();
//         return atlas;
//     }
//     
//     void GenerateFntFile(List<FontCharacter> chars, string fntPath, Vector2Int atlasSize, string atlasFileName)
//     {
//         var validChars = chars.Where(c => c.sourceImage != null && !string.IsNullOrEmpty(c.character)).ToList();
//         var sortedChars = validChars.OrderByDescending(c => c.sourceImage?.height ?? 0).ThenByDescending(c => c.sourceImage?.width ?? 0).ToList();
//         
//         using (StreamWriter writer = new StreamWriter(fntPath))
//         {
//             // Header
//             writer.WriteLine($"info face=\"{fontName}\" size={fontSize} bold=0 italic=0 charset=\"\" unicode=0 stretchH=100 smooth=1 aa=1 padding=0,0,0,0 spacing=1,1");
//             writer.WriteLine($"common lineHeight={lineHeight} base={fontSize} scaleW={atlasSize.x} scaleH={atlasSize.y} pages=1 packed=0 alphaChnl=1 redChnl=0 greenChnl=0 blueChnl=0");
//             writer.WriteLine($"page id=0 file=\"{atlasFileName}\"");
//             writer.WriteLine($"chars count={validChars.Count}");
//             
//             // Characters - match with original layout order
//             for (int i = 0; i < validChars.Count; i++)
//             {
//                 var ch = validChars[i];
//                 var sortedIndex = sortedChars.FindIndex(sc => sc == ch);
//                 
//                 if (sortedIndex >= 0)
//                 {
//                     var rect = ch.atlasRect;
//                     int charId = (int)ch.character[0];
//                     
//                     // BMFont format coordinates
//                     int fntX = Mathf.RoundToInt(rect.x);
//                     int fntY = Mathf.RoundToInt(rect.y);
//                     int fntWidth = ch.sourceImage.width;
//                     int fntHeight = ch.sourceImage.height;
//                     
//                     writer.WriteLine($"char id={charId} " +
//                                    $"x={fntX} " +
//                                    $"y={fntY} " +
//                                    $"width={fntWidth} " +
//                                    $"height={fntHeight} " +
//                                    $"xoffset={ch.xoffset} " +
//                                    $"yoffset={ch.yoffset} " +
//                                    $"xadvance={ch.xadvance} " +
//                                    $"page=0 chnl=0 " +
//                                    $"letter=\"{ch.character}\"");
//                 }
//             }
//             
//             writer.WriteLine("kernings count=0");
//         }
//         
//         Debug.Log($"Generated .fnt file with {validChars.Count} characters");
//     }
//     
//     void TrimFinalAtlas()
//     {
//         if (previewAtlas == null)
//         {
//             Debug.LogError("No atlas to trim! Generate preview first.");
//             return;
//         }
//         
//         Debug.Log("Starting final atlas trimming...");
//         
//         // Find bounds of all character content
//         int minX = previewAtlas.width, maxX = -1;
//         int minY = previewAtlas.height, maxY = -1;
//         
//         Color[] atlasPixels = previewAtlas.GetPixels();
//         
//         for (int y = 0; y < previewAtlas.height; y++)
//         {
//             for (int x = 0; x < previewAtlas.width; x++)
//             {
//                 Color pixel = atlasPixels[y * previewAtlas.width + x];
//                 
//                 // Check if pixel has content (not transparent and not background color)
//                 bool hasContent = pixel.a > 0.01f;
//                 if (packingSettings.backgroundColor.a > 0.01f)
//                 {
//                     // If background has color, check if pixel is different from background
//                     hasContent = hasContent && !ColorsEqual(pixel, packingSettings.backgroundColor, 0.02f);
//                 }
//                 
//                 if (hasContent)
//                 {
//                     minX = Mathf.Min(minX, x);
//                     maxX = Mathf.Max(maxX, x);
//                     minY = Mathf.Min(minY, y);
//                     maxY = Mathf.Max(maxY, y);
//                 }
//             }
//         }
//         
//         if (maxX < 0 || maxY < 0)
//         {
//             Debug.LogError("No content found in atlas!");
//             return;
//         }
//         
//         // Add small padding to trimmed atlas
//         int trimPadding = Mathf.Max(1, packingSettings.padding / 2);
//         minX = Mathf.Max(0, minX - trimPadding);
//         minY = Mathf.Max(0, minY - trimPadding);
//         maxX = Mathf.Min(previewAtlas.width - 1, maxX + trimPadding);
//         maxY = Mathf.Min(previewAtlas.height - 1, maxY + trimPadding);
//         
//         int trimmedWidth = maxX - minX + 1;
//         int trimmedHeight = maxY - minY + 1;
//         
//         // Check if trimming is beneficial
//         float originalSize = previewAtlas.width * previewAtlas.height;
//         float trimmedSize = trimmedWidth * trimmedHeight;
//         float savings = (originalSize - trimmedSize) / originalSize * 100;
//         
//         if (savings < 5f)
//         {
//             Debug.Log($"Atlas already well-optimized (only {savings:F1}% savings possible)");
//             EditorUtility.DisplayDialog("Atlas Optimization", $"Atlas is already well-optimized!\nCurrent size: {previewAtlas.width}x{previewAtlas.height}\nPotential savings: {savings:F1}%", "OK");
//             return;
//         }
//         
//         // Create trimmed atlas
//         Texture2D trimmedAtlas = new Texture2D(trimmedWidth, trimmedHeight, TextureFormat.RGBA32, false);
//         Color[] trimmedPixels = new Color[trimmedWidth * trimmedHeight];
//         
//         for (int y = 0; y < trimmedHeight; y++)
//         {
//             for (int x = 0; x < trimmedWidth; x++)
//             {
//                 int sourceX = x + minX;
//                 int sourceY = y + minY;
//                 trimmedPixels[y * trimmedWidth + x] = atlasPixels[sourceY * previewAtlas.width + sourceX];
//             }
//         }
//         
//         trimmedAtlas.SetPixels(trimmedPixels);
//         trimmedAtlas.Apply();
//         
//         // Update character atlas rects with new coordinates
//         Vector2 offset = new Vector2(minX, minY);
//         foreach (var ch in characters)
//         {
//             if (ch.atlasRect.width > 0)
//             {
//                 ch.atlasRect.x -= offset.x;
//                 ch.atlasRect.y -= offset.y;
//             }
//         }
//         
//         // Replace preview atlas
//         DestroyImmediate(previewAtlas);
//         previewAtlas = trimmedAtlas;
//         
//         Debug.Log($"Atlas trimmed successfully: {previewAtlas.width}x{previewAtlas.height} -> {trimmedWidth}x{trimmedHeight} (saved {savings:F1}%)");
//         EditorUtility.DisplayDialog("Atlas Optimized!", 
//             $"Atlas successfully optimized!\n\n" +
//             $"Original: {previewAtlas.width}x{previewAtlas.height}\n" +
//             $"Optimized: {trimmedWidth}x{trimmedHeight}\n" +
//             $"Space saved: {savings:F1}%\n\n" +
//             $"Now generate complete font to save the optimized version!", 
//             "Great!");
//     }
//     
//     bool ColorsEqual(Color a, Color b, float tolerance)
//     {
//         return Mathf.Abs(a.r - b.r) < tolerance && 
//                Mathf.Abs(a.g - b.g) < tolerance && 
//                Mathf.Abs(a.b - b.b) < tolerance && 
//                Mathf.Abs(a.a - b.a) < tolerance;
//     }
// }
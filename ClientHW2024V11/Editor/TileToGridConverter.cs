﻿using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TileToGridConverter : EditorWindow
{
    // Enum for tile patterns
    public enum TilePattern 
    { 
        Alternating, 
        ColumnAlternating,
        Random, 
        Tile1Only, 
        Tile2Only 
    }

    private Texture2D tile1;
    private Texture2D tile2;
    private int gridCols = 6;
    private int gridRows = 7;
    private string outputFileName = "combined_grid";
    private string outputPath = "Assets/Atlas/";
    private bool autoSetupSprite = true;
    private Vector2 scrollPos;
    private TilePattern tilePattern = TilePattern.Alternating;

    [MenuItem("Tools/Tile To Grid Converter", false, 100)]
    public static void OpenWindow()
    {
        TileToGridConverter window = GetWindow<TileToGridConverter>("Tile To Grid Converter");
        window.minSize = new Vector2(400, 500);
        window.Show();
    }

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        GUILayout.Label("Tile To Grid Converter", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Input Section
        DrawSeparator("Input Tiles");
        
        tile1 = (Texture2D)EditorGUILayout.ObjectField("Tile 1 (frame_lattice)", tile1, typeof(Texture2D), false);
        tile2 = (Texture2D)EditorGUILayout.ObjectField("Tile 2 (frame_lattice_1)", tile2, typeof(Texture2D), false);

        GUILayout.Space(10);

        // Grid Settings Section
        DrawSeparator("Grid Settings");
        
        gridCols = EditorGUILayout.IntField("Grid Columns", gridCols);
        gridRows = EditorGUILayout.IntField("Grid Rows", gridRows);
        
        if (gridCols < 1) gridCols = 1;
        if (gridRows < 1) gridRows = 1;

        GUILayout.Space(5);
        tilePattern = (TilePattern)EditorGUILayout.EnumPopup("Tile Pattern", tilePattern);

        GUILayout.Space(10);

        // Output Settings Section
        DrawSeparator("Output Settings");
        
        outputPath = EditorGUILayout.TextField("Output Path", outputPath);
        outputFileName = EditorGUILayout.TextField("File Name", outputFileName);
        autoSetupSprite = EditorGUILayout.Toggle("Auto Setup Sprite", autoSetupSprite);

        GUILayout.Space(10);

        // Preview Section
        DrawSeparator("Preview");
        DrawPreview();

        GUILayout.Space(20);

        // Buttons Section
        DrawButtons();

        EditorGUILayout.EndScrollView();
    }

    void DrawSeparator(string title)
    {
        GUILayout.Space(5);
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        GUILayout.Space(5);
    }

    void DrawPreview()
    {
        if (tile1 == null && tile2 == null)
        {
            EditorGUILayout.HelpBox("No tiles selected for preview", MessageType.Info);
            return;
        }

        // Calculate preview size
        int previewMaxSize = 200;
        int totalCols = gridCols;
        int totalRows = gridRows;
        
        if (tile1 != null || tile2 != null)
        {
            Texture2D sampleTile = tile1 != null ? tile1 : tile2;
            int tileWidth = sampleTile.width;
            int tileHeight = sampleTile.height;
            int gridWidth = tileWidth * totalCols;
            int gridHeight = tileHeight * totalRows;
            
            float scale = Mathf.Min((float)previewMaxSize / gridWidth, (float)previewMaxSize / gridHeight);
            int previewWidth = Mathf.RoundToInt(gridWidth * scale);
            int previewHeight = Mathf.RoundToInt(gridHeight * scale);

            EditorGUILayout.LabelField("Grid Size: " + gridWidth + " x " + gridHeight);
            EditorGUILayout.LabelField("Total Sprites: " + (totalCols * totalRows));
            EditorGUILayout.LabelField("Tile Size: " + tileWidth + " x " + tileHeight);

            // Draw preview grid
            Rect previewRect = GUILayoutUtility.GetRect(previewWidth, previewHeight);
            GUI.Box(previewRect, "");

            // Draw grid lines
            Handles.BeginGUI();
            Handles.color = Color.gray;
            
            float cellWidth = previewWidth / (float)totalCols;
            float cellHeight = previewHeight / (float)totalRows;
            
            // Vertical lines
            for (int i = 0; i <= totalCols; i++)
            {
                float x = previewRect.x + i * cellWidth;
                Handles.DrawLine(new Vector3(x, previewRect.y), new Vector3(x, previewRect.yMax));
            }
            
            // Horizontal lines
            for (int i = 0; i <= totalRows; i++)
            {
                float y = previewRect.y + i * cellHeight;
                Handles.DrawLine(new Vector3(previewRect.x, y), new Vector3(previewRect.xMax, y));
            }

            // Draw tile previews with pattern
            for (int row = 0; row < totalRows; row++)
            {
                for (int col = 0; col < totalCols; col++)
                {
                    Texture2D tileToShow = GetTileForPosition(col, row);
                    if (tileToShow != null)
                    {
                        Rect tileRect = new Rect(
                            previewRect.x + col * cellWidth, 
                            previewRect.y + row * cellHeight, 
                            cellWidth, 
                            cellHeight
                        );
                        GUI.DrawTexture(tileRect, tileToShow, ScaleMode.ScaleToFit);
                        
                        // Show tile number
                        string tileNum = tileToShow == tile1 ? "1" : "2";
                        GUI.Label(tileRect, tileNum, EditorStyles.whiteLabel);
                    }
                }
            }

            Handles.EndGUI();
        }
    }

    Texture2D GetTileForPosition(int col, int row)
    {
        switch (tilePattern)
        {
            case TilePattern.Alternating:
                return ((col + row) % 2 == 0) ? tile1 : tile2;
            
            case TilePattern.ColumnAlternating:
                return (col % 2 == 0) ? tile1 : tile2;
            
            case TilePattern.Random:
                // Use deterministic random based on position
                System.Random rand = new System.Random(col * 1000 + row);
                return (rand.Next(2) == 0) ? tile1 : tile2;
            
            case TilePattern.Tile1Only:
                return tile1;
            
            case TilePattern.Tile2Only:
                return tile2;
            
            default:
                return tile1;
        }
    }

    void DrawButtons()
    {
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Generate Grid", GUILayout.Height(30)))
        {
            GenerateGrid();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(10);

        if (GUILayout.Button("Reset", GUILayout.Height(25)))
        {
            ResetValues();
        }

        // Validation messages
        DrawValidationMessages();
    }

    void DrawValidationMessages()
    {
        bool needBothTiles = tilePattern == TilePattern.Alternating || 
                            tilePattern == TilePattern.ColumnAlternating || 
                            tilePattern == TilePattern.Random;
        bool needTile1 = tilePattern == TilePattern.Tile1Only || needBothTiles;
        bool needTile2 = tilePattern == TilePattern.Tile2Only || needBothTiles;
        
        if (needTile1 && tile1 == null)
        {
            EditorGUILayout.HelpBox("Tile 1 is required for this pattern", MessageType.Error);
            return;
        }
        
        if (needTile2 && tile2 == null)
        {
            EditorGUILayout.HelpBox("Tile 2 is required for this pattern", MessageType.Error);
            return;
        }
        
        if (tile1 == null && tile2 == null)
        {
            EditorGUILayout.HelpBox("Please select at least one tile texture", MessageType.Warning);
            return;
        }

        if (tile1 != null && tile2 != null)
        {
            if (tile1.width != tile2.width || tile1.height != tile2.height)
            {
                EditorGUILayout.HelpBox("Tiles have different sizes. This may cause alignment issues.", MessageType.Warning);
            }
        }

        if (string.IsNullOrEmpty(outputFileName))
        {
            EditorGUILayout.HelpBox("Please enter a file name", MessageType.Error);
        }

        if (!Directory.Exists(outputPath))
        {
            EditorGUILayout.HelpBox("Output directory '" + outputPath + "' doesn't exist. It will be created.", MessageType.Info);
        }
        
        // Show pattern info
        switch (tilePattern)
        {
            case TilePattern.Alternating:
                EditorGUILayout.HelpBox("Pattern: Alternating checkerboard pattern", MessageType.Info);
                break;
            case TilePattern.ColumnAlternating:
                EditorGUILayout.HelpBox("Pattern: Alternating by columns (Tile1-Tile2-Tile1-Tile2...)", MessageType.Info);
                break;
            case TilePattern.Random:
                EditorGUILayout.HelpBox("Pattern: Random distribution (deterministic)", MessageType.Info);
                break;
            case TilePattern.Tile1Only:
                EditorGUILayout.HelpBox("Pattern: Only Tile 1 will be used", MessageType.Info);
                break;
            case TilePattern.Tile2Only:
                EditorGUILayout.HelpBox("Pattern: Only Tile 2 will be used", MessageType.Info);
                break;
        }
    }

    void GenerateGrid()
    {
        if (!ValidateInputs()) return;

        try
        {
            EditorUtility.DisplayProgressBar("Generating Grid", "Creating texture...", 0.3f);

            // Ensure output directory exists
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            // Get tile dimensions
            Texture2D sampleTile = tile1 != null ? tile1 : tile2;
            int tileWidth = sampleTile.width;
            int tileHeight = sampleTile.height;

            // Create grid texture
            int gridWidth = tileWidth * gridCols;
            int gridHeight = tileHeight * gridRows;
            
            Texture2D newGrid = new Texture2D(gridWidth, gridHeight, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[gridWidth * gridHeight];

            // Fill with transparent background
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.clear;
            }

            EditorUtility.DisplayProgressBar("Generating Grid", "Copying tiles...", 0.6f);

            // Fill grid with tiles according to pattern
            for (int row = 0; row < gridRows; row++)
            {
                for (int col = 0; col < gridCols; col++)
                {
                    Texture2D tileToUse = GetTileForPosition(col, row);
                    if (tileToUse != null)
                    {
                        CopyTileToGrid(tileToUse, pixels, col, row, tileWidth, tileHeight, gridWidth);
                    }
                }
            }

            // Apply texture
            newGrid.SetPixels(pixels);
            newGrid.Apply();

            // Save texture
            string fullPath = Path.Combine(outputPath, outputFileName + ".png");
            byte[] pngData = newGrid.EncodeToPNG();
            File.WriteAllBytes(fullPath, pngData);

            EditorUtility.DisplayProgressBar("Generating Grid", "Setting up sprite...", 0.9f);

            AssetDatabase.Refresh();

            // Auto setup sprite if enabled
            if (autoSetupSprite)
            {
                SetupSpriteImportSettings(fullPath, gridCols, gridRows, tileWidth, tileHeight);
            }

            EditorUtility.ClearProgressBar();

            // Show success message
            EditorUtility.DisplayDialog("Success", 
                "Grid texture created successfully!\n\nPath: " + fullPath + "\nSprites: " + (gridCols * gridRows), 
                "OK");

            // Ping the created asset
            Object createdAsset = AssetDatabase.LoadAssetAtPath<Object>(fullPath);
            EditorGUIUtility.PingObject(createdAsset);
        }
        catch (System.Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Error", "Failed to generate grid:\n" + e.Message, "OK");
            Debug.LogError("TileToGridConverter Error: " + e.Message);
        }
    }

    void CopyTileToGrid(Texture2D tile, Color[] gridPixels, int gridX, int gridY, 
                       int tileWidth, int tileHeight, int gridWidth)
    {
        // Make texture readable
        string tilePath = AssetDatabase.GetAssetPath(tile);
        TextureImporter tileImporter = AssetImporter.GetAtPath(tilePath) as TextureImporter;
        bool wasReadable = tileImporter.isReadable;
        
        if (!wasReadable)
        {
            tileImporter.isReadable = true;
            AssetDatabase.ImportAsset(tilePath);
        }

        Color[] tilePixels = tile.GetPixels();

        for (int y = 0; y < tileHeight; y++)
        {
            for (int x = 0; x < tileWidth; x++)
            {
                int gridIndex = ((gridY * tileHeight + y) * gridWidth) + (gridX * tileWidth + x);
                int tileIndex = (y * tileWidth) + x;

                if (gridIndex >= 0 && gridIndex < gridPixels.Length)
                {
                    gridPixels[gridIndex] = tilePixels[tileIndex];
                }
            }
        }

        // Restore original readable setting
        if (!wasReadable)
        {
            tileImporter.isReadable = false;
            AssetDatabase.ImportAsset(tilePath);
        }
    }

    void SetupSpriteImportSettings(string texturePath, int cols, int rows, int tileWidth, int tileHeight)
    {
        TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if (importer == null) return;

        // Basic sprite settings
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 100;
        importer.filterMode = FilterMode.Point;
        importer.mipmapEnabled = false;

        // Create sprite metadata
        List<SpriteMetaData> spritesheet = new List<SpriteMetaData>();

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                SpriteMetaData smd = new SpriteMetaData();
                smd.name = outputFileName + "_" + (row * cols + col);
                smd.rect = new Rect(
                    col * tileWidth,
                    (rows - row - 1) * tileHeight, // Unity uses bottom-left origin
                    tileWidth,
                    tileHeight
                );
                smd.pivot = new Vector2(0.5f, 0.5f);
                smd.alignment = (int)SpriteAlignment.Center;
                spritesheet.Add(smd);
            }
        }

        importer.spritesheet = spritesheet.ToArray();
        AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);

        Debug.Log("✅ Created grid with " + spritesheet.Count + " sprites!");
    }

    bool ValidateInputs()
    {
        bool needBothTiles = tilePattern == TilePattern.Alternating || 
                            tilePattern == TilePattern.ColumnAlternating || 
                            tilePattern == TilePattern.Random;
        bool needTile1 = tilePattern == TilePattern.Tile1Only || needBothTiles;
        bool needTile2 = tilePattern == TilePattern.Tile2Only || needBothTiles;
        
        if (needTile1 && tile1 == null)
        {
            EditorUtility.DisplayDialog("Error", "Tile 1 is required for this pattern", "OK");
            return false;
        }
        
        if (needTile2 && tile2 == null)
        {
            EditorUtility.DisplayDialog("Error", "Tile 2 is required for this pattern", "OK");
            return false;
        }
        
        if (tile1 == null && tile2 == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select at least one tile texture", "OK");
            return false;
        }

        if (string.IsNullOrEmpty(outputFileName))
        {
            EditorUtility.DisplayDialog("Error", "Please enter a file name", "OK");
            return false;
        }

        return true;
    }

    void ResetValues()
    {
        tile1 = null;
        tile2 = null;
        gridCols = 6;
        gridRows = 7;
        outputFileName = "combined_grid";
        outputPath = "Assets/Atlas/";
        autoSetupSprite = true;
        tilePattern = TilePattern.Alternating;
    }
}
/*
 * ============================================================================
 * Atlas Importer Tool Enhanced
 * ============================================================================
 *
 * Author: Ray
 * Created: November 2025
 *
 * Description:
 *   Enhanced Unity Editor tool for importing and managing atlas textures.
 *   Provides a professional UI with preview functionality, real-time logging,
 *   and automated atlas generation from source PNG files.
 *
 * Features:
 *   - Batch atlas export from source folders
 *   - Real-time preview of exported atlas textures
 *   - Split-view preview window (source files + atlas output)
 *   - Integrated logging system with timestamps
 *   - Language selection support (currently English)
 *   - Filter and search functionality
 *   - Automatic texture packing using TexturePacker
 *
 * Dependencies:
 *   - TexturePacker (external tool required)
 *   - Path: TexturePacker/bin/TexturePacker.exe
 *
 * IMPORTANT NOTE:
 *   If this is your first time running this tool on a new machine,
 *   you MUST activate TexturePacker before use:
 *   1. Navigate to: TexturePacker/bin/TexturePacker.exe
 *   2. Run the executable and complete activation
 *   3. After activation, this tool will work properly
 *
 * Usage:
 *   Unity Editor → Tools → Tamron Tool → Atlas Importer Enhanced
 *
 * ============================================================================
 */

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace SEZSJ
{
    /// <summary>
    /// Enhanced Atlas Importer Tool with improved UI and preview functionality
    /// </summary>
    public class AtlasImporterToolEnhance : EditorWindow
    {
        #region Constants
        private const string SOURCE_PATH = "图集本地化";
        private const string PNG_PATH = @"Assets/Atlas";
        private const string LANGUAGE = "English";
        private const string TP_CMD = @"TexturePacker/bin/TexturePacker.exe";
        private const string TP_ARG = @"""{0}"" --format cocos2d --disable-rotation --no-trim --algorithm MaxRects --max-size 2048 --size-constraints POT --data  ""{1}.xml"" --sheet ""{1}.png""";

        private const float TOOLBAR_HEIGHT = 35f;
        private const float BUTTON_WIDTH = 140f;
        private const float SMALL_BUTTON_WIDTH = 100f;
        private const int ITEMS_PER_ROW = 2;
        #endregion

        #region Fields
        private List<string> _atlasPathList = new List<string>();
        private Dictionary<int, bool> _atlasFolderSelected = new Dictionary<int, bool>();
        private Dictionary<string, int> _atlasFolderFileCount = new Dictionary<string, int>();

        private Vector2 _scrollPosition = Vector2.zero;
        private string _searchFilter = "";
        private bool _isExporting = false;

        // Log system
        private List<string> _logMessages = new List<string>();
        private bool _showLog = true;
        private Vector2 _logScrollPosition = Vector2.zero;
        #endregion

        #region Styles
        private static class Styles
        {
            public static GUIStyle headerStyle;
            public static GUIStyle sectionHeaderStyle;
            public static GUIStyle boxStyle;
            public static GUIStyle exportButtonStyle;
            public static GUIStyle normalButtonStyle;

            public static void Initialize()
            {
                if (headerStyle != null) return;

                // Header style
                headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 16,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = new Color(0.9f, 0.95f, 1f) }
                };

                // Section header style - larger and more prominent
                sectionHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 14,
                    alignment = TextAnchor.MiddleLeft,
                    normal = { textColor = new Color(1f, 1f, 1f) },
                    padding = new RectOffset(5, 5, 5, 5)
                };

                // Box style
                boxStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(10, 10, 10, 10),
                    margin = new RectOffset(5, 5, 5, 5)
                };

                // Export button style - green background
                exportButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.white },
                    padding = new RectOffset(10, 10, 8, 8)
                };

                // Normal button style
                normalButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 11,
                    padding = new RectOffset(10, 10, 6, 6)
                };
            }
        }
        #endregion

        #region Menu Item
        [MenuItem("Tools/Tamron Tool/Atlas Importer Enhanced", false)]
        static void OpenWindow()
        {
            var window = GetWindow<AtlasImporterToolEnhance>(true, "Atlas Importer Tool - Enhanced");
            window.minSize = new Vector2(700, 500);
            window.Show();
        }
        #endregion

        #region Unity Lifecycle
        private void OnEnable()
        {
            AddLog("Atlas Importer Tool initialized");
            RefreshAtlasFolders();
        }

        private void OnGUI()
        {
            Styles.Initialize();

            DrawHeader();
            DrawToolbar();

            // Main scroll area
            float scrollHeight = position.height - TOOLBAR_HEIGHT - 90;
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(scrollHeight));

            DrawAtlasFoldersSection();

            EditorGUILayout.EndScrollView();
        }
        #endregion

        #region UI Drawing Methods
        private void DrawHeader()
        {
            GUILayout.Space(10);
            GUILayout.Label("ATLAS IMPORTER TOOL", Styles.headerStyle);
            GUILayout.Space(10);
            DrawSeparator();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);

            // Export button with color
            GUI.enabled = !_isExporting && GetSelectedCount() > 0;
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f); // Green
            if (GUILayout.Button("EXPORT SELECTED", Styles.exportButtonStyle, GUILayout.Width(BUTTON_WIDTH), GUILayout.Height(TOOLBAR_HEIGHT)))
            {
                ExportSelectedAtlas();
            }
            GUI.backgroundColor = originalColor;

            GUILayout.Space(10);

            // Other buttons
            GUI.enabled = !_isExporting;
            if (GUILayout.Button("Refresh", Styles.normalButtonStyle, GUILayout.Width(SMALL_BUTTON_WIDTH), GUILayout.Height(TOOLBAR_HEIGHT)))
            {
                RefreshAtlasFolders();
            }

            GUI.enabled = true;
            GUILayout.FlexibleSpace();

            // Language selection on the right
            GUILayout.Label("Language:", GUILayout.Width(70));
            bool englishSelected = true;
            EditorGUILayout.ToggleLeft("English", englishSelected, GUILayout.Width(80));

            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            DrawSeparator();
        }

        private void DrawAtlasFoldersSection()
        {
            // Prominent box style for Atlas Folders
            GUIStyle prominentBox = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(15, 15, 15, 15),
                margin = new RectOffset(5, 5, 10, 10)
            };

            EditorGUILayout.BeginVertical(prominentBox);

            // Larger header
            GUIStyle atlasHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 15,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = new Color(1f, 1f, 0.7f) }, // Light yellow
                padding = new RectOffset(5, 5, 8, 8)
            };
            GUILayout.Label("ATLAS FOLDERS", atlasHeaderStyle);

            GUILayout.Space(10);
            DrawSeparator();
            GUILayout.Space(10);

            // Selection buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select All", GUILayout.Width(SMALL_BUTTON_WIDTH)))
            {
                SelectAllFolders(true);
            }
            if (GUILayout.Button("Deselect All", GUILayout.Width(SMALL_BUTTON_WIDTH)))
            {
                SelectAllFolders(false);
            }
            if (GUILayout.Button("Invert", GUILayout.Width(SMALL_BUTTON_WIDTH)))
            {
                InvertSelection();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Search filter
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Filter:", GUILayout.Width(50));
            string newFilter = EditorGUILayout.TextField(_searchFilter);
            if (newFilter != _searchFilter)
            {
                _searchFilter = newFilter;
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            DrawSeparator();
            GUILayout.Space(10);

            // Atlas folder list
            var filteredList = GetFilteredAtlasList();
            int displayIndex = 0;

            for (int i = 0; i < _atlasPathList.Count; i++)
            {
                var folderName = _atlasPathList[i];
                if (!filteredList.Contains(folderName)) continue;

                int col = displayIndex % ITEMS_PER_ROW;
                if (col == 0)
                {
                    EditorGUILayout.BeginHorizontal();
                }

                // Checkbox with folder name and file count
                int fileCount = _atlasFolderFileCount.ContainsKey(folderName) ? _atlasFolderFileCount[folderName] : 0;
                string displayName = $"{folderName} ({fileCount} files)";

                bool isSelected = _atlasFolderSelected[i];
                bool newSelected = EditorGUILayout.ToggleLeft(displayName, isSelected, GUILayout.Width(280));
                if (newSelected != isSelected)
                {
                    _atlasFolderSelected[i] = newSelected;
                }

                // Preview button
                if (GUILayout.Button("Preview", GUILayout.Width(80)))
                {
                    ShowPreview(folderName);
                }

                if (col == ITEMS_PER_ROW - 1 || displayIndex == filteredList.Count - 1)
                {
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(5);
                }

                displayIndex++;
            }

            // Show message if no results
            if (filteredList.Count == 0 && !string.IsNullOrEmpty(_searchFilter))
            {
                GUILayout.Space(10);
                GUILayout.Label("No folders match the filter", EditorStyles.centeredGreyMiniLabel);
                GUILayout.Space(10);
            }

            GUILayout.Space(10);
            DrawSeparator();
            GUILayout.Space(5);

            // Statistics
            int selectedCount = GetSelectedCount();
            int totalFiles = GetTotalFilesInSelection();
            GUIStyle statsStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.Label($"Selected: {selectedCount}/{_atlasPathList.Count} folders ({totalFiles} total files)", statsStyle);
            GUILayout.Space(5);

            EditorGUILayout.EndVertical();

            // Log section below Atlas Folders
            GUILayout.Space(10);
            DrawLogSection();
        }

        private void DrawLogSection()
        {
            EditorGUILayout.BeginVertical(Styles.boxStyle);

            // Log header with foldout and clear button
            EditorGUILayout.BeginHorizontal();
            _showLog = EditorGUILayout.Foldout(_showLog, $"LOG ({_logMessages.Count} messages)", true, EditorStyles.foldoutHeader);

            GUILayout.FlexibleSpace();

            // Clear log button (red, small)
            Color originalBgColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.8f, 0.3f, 0.3f); // Red
            if (GUILayout.Button("Clear", GUILayout.Width(60), GUILayout.Height(20)))
            {
                _logMessages.Clear();
            }
            GUI.backgroundColor = originalBgColor;

            EditorGUILayout.EndHorizontal();

            if (_showLog && _logMessages.Count > 0)
            {
                GUILayout.Space(5);
                _logScrollPosition = EditorGUILayout.BeginScrollView(_logScrollPosition, GUILayout.Height(120));

                foreach (var log in _logMessages)
                {
                    GUILayout.Label(log, EditorStyles.wordWrappedMiniLabel);
                }

                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawSeparator()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
        }
        #endregion

        #region Helper Methods
        private void RefreshAtlasFolders()
        {
            _atlasPathList.Clear();
            _atlasFolderSelected.Clear();
            _atlasFolderFileCount.Clear();

            string sourcePath = $"{SOURCE_PATH}/{LANGUAGE}";
            DirectoryInfo dir = new DirectoryInfo(sourcePath);

            if (dir == null || !dir.Exists)
            {
                AddLog($"ERROR: Source folder not found: {sourcePath}");
                return;
            }

            int index = 0;
            foreach (var item in dir.GetDirectories())
            {
                var pngFiles = item.GetFiles("*.png", SearchOption.AllDirectories);
                if (pngFiles.Length > 0)
                {
                    _atlasPathList.Add(item.Name);
                    _atlasFolderSelected[index] = false;
                    _atlasFolderFileCount[item.Name] = pngFiles.Length;
                    index++;
                }
                else
                {
                    AddLog($"WARNING: Folder has no PNG files: {item.Name}");
                }
            }

            AddLog($"Refreshed: Found {_atlasPathList.Count} atlas folders");
        }

        private List<string> GetFilteredAtlasList()
        {
            if (string.IsNullOrEmpty(_searchFilter))
                return new List<string>(_atlasPathList);

            return _atlasPathList.Where(x => x.ToLower().Contains(_searchFilter.ToLower())).ToList();
        }

        private void SelectAllFolders(bool select)
        {
            for (int i = 0; i < _atlasPathList.Count; i++)
            {
                _atlasFolderSelected[i] = select;
            }
        }

        private void InvertSelection()
        {
            for (int i = 0; i < _atlasPathList.Count; i++)
            {
                _atlasFolderSelected[i] = !_atlasFolderSelected[i];
            }
        }

        private int GetSelectedCount()
        {
            return _atlasFolderSelected.Count(x => x.Value);
        }

        private int GetTotalFilesInSelection()
        {
            int total = 0;
            for (int i = 0; i < _atlasPathList.Count; i++)
            {
                if (_atlasFolderSelected.ContainsKey(i) && _atlasFolderSelected[i])
                {
                    string folderName = _atlasPathList[i];
                    if (_atlasFolderFileCount.ContainsKey(folderName))
                    {
                        total += _atlasFolderFileCount[folderName];
                    }
                }
            }
            return total;
        }

        private void ShowPreview(string folderName)
        {
            string folderPath = $"{SOURCE_PATH}/{LANGUAGE}/{folderName}";
            AtlasPreviewWindow.ShowWindow(folderName, folderPath, _atlasFolderFileCount[folderName]);
        }

        private void AddLog(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            _logMessages.Add($"[{timestamp}] {message}");

            // Keep only last 100 messages
            if (_logMessages.Count > 100)
            {
                _logMessages.RemoveAt(0);
            }

            Repaint();
        }
        #endregion

        #region Export Logic
        private void ExportSelectedAtlas()
        {
            if (GetSelectedCount() == 0)
            {
                EditorUtility.DisplayDialog("Warning", "No atlas folders selected!", "OK");
                return;
            }

            if (!EditorUtility.DisplayDialog("Confirm Export",
                $"Export {GetSelectedCount()} atlas folders with {GetTotalFilesInSelection()} files?",
                "Export", "Cancel"))
            {
                return;
            }

            _isExporting = true;

            bool hasError = false;

            for (int i = 0; i < _atlasPathList.Count; i++)
            {
                if (!_atlasFolderSelected[i] || hasError) continue;

                string folderName = _atlasPathList[i];
                string folderPath = $"{SOURCE_PATH}/{LANGUAGE}/{folderName}";

                AddLog($"Exporting: {folderName}");

                if (!ExportPng(LANGUAGE, folderPath))
                {
                    hasError = true;
                    AddLog($"ERROR: Export failed: {folderName}");
                }
                else
                {
                    AddLog($"SUCCESS: Export successful: {folderName}");
                }
            }

            _isExporting = false;

            if (!hasError)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                AddLog("All atlas exported successfully!");
                EditorUtility.DisplayDialog("Success", "All selected atlas exported successfully!", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Export completed with errors. Check console for details.", "OK");
            }

            Repaint();
        }

        private bool ExportPng(string language, string dirName)
        {
            var name = language + "/" + new DirectoryInfo(dirName).Name;
            var sourcePath = dirName;
            var exportPath = PNG_PATH + "/" + name;
            var xmlPath = exportPath + ".xml";
            var pngPath = exportPath + ".png";

            var fullPath = GetFullPath(xmlPath);
            fullPath = fullPath.Replace("{n}", "0");

            var args = string.Format(TP_ARG, sourcePath, exportPath);
            Debug.Log($"TexturePacker command: {TP_CMD} {args}");

            var process = ProcessCommand(TP_CMD, args);
            var error = process.StandardError.ReadToEnd();

            if (!File.Exists(fullPath))
            {
                Debug.LogError($"TexturePacker failed: {fullPath} - {error}");
                process.Close();
                return false;
            }

            process.Close();
            AssetDatabase.Refresh();

            var listObjects = CreateAtlas(language, name, xmlPath, pngPath);
            if (listObjects.Count == 0)
            {
                return false;
            }

            AssetDatabase.Refresh();
            return CreatePrefab(listObjects, name);
        }

        private bool CreatePrefab(List<UnityEngine.Object> listObjects, string name)
        {
            AtlasImporter.GenerateAtlasPrefab(listObjects, name);
            return true;
        }

        private List<UnityEngine.Object> CreateAtlas(string group, string name, string xmlPath, string pngPath)
        {
            var listObjects = new List<UnityEngine.Object>();
            var xmlObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(xmlPath);

            if (xmlObject == null)
            {
                Debug.LogError($"XML not found: {xmlPath}");
                return listObjects;
            }

            AtlasImporter.GenerateAtlasMeta(xmlObject);

            var pngObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pngPath);
            if (pngObject == null)
            {
                Debug.LogError($"PNG export failed: {pngPath}");
                return listObjects;
            }

            // Rename meta file
            var pngMeta = GetFullPath(pngPath) + ".meta";
            var pngMetaTmp = pngMeta + ".txt";

            if (File.Exists(pngMetaTmp))
            {
                if (File.Exists(pngMeta))
                {
                    File.Delete(pngMeta);
                }
                File.Move(pngMetaTmp, pngMeta);
            }
            else
            {
                Debug.LogError($"PNG meta generation failed: {pngMetaTmp}");
                return listObjects;
            }

            // Delete XML files
            if (File.Exists(xmlPath))
            {
                File.Delete(xmlPath);
            }

            var xmlMeta = xmlPath + ".meta";
            if (File.Exists(xmlMeta))
            {
                File.Delete(xmlMeta);
            }

            listObjects.Add(pngObject);
            return listObjects;
        }

        private static string GetFullPath(string path)
        {
            return Application.dataPath.Substring(0, Application.dataPath.LastIndexOf(@"/") + 1) + path;
        }

        private static System.Diagnostics.Process ProcessCommand(string command, string argument)
        {
            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo(
                Application.dataPath.Replace("Assets", command));

            start.Arguments = argument;
            start.CreateNoWindow = true;
            start.ErrorDialog = true;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;
            start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;

            System.Diagnostics.Process process = System.Diagnostics.Process.Start(start);
            process.StandardInput.WriteLine("agree");
            process.WaitForExit();

            return process;
        }
        #endregion
    }

    #region Atlas Preview Window
    /// <summary>
    /// Simple preview window for atlas folder contents
    /// </summary>
    public class AtlasPreviewWindow : EditorWindow
    {
        private enum PreviewMode { Before, After }

        private string _folderName;
        private string _folderPath;
        private int _fileCount;
        private List<FileInfo> _pngFiles = new List<FileInfo>();
        private Vector2 _scrollPosition;
        private long _totalSize = 0;

        private Texture2D _atlasPreviewBefore = null;
        private Texture2D _atlasPreviewAfter = null;
        private string _atlasOutputPath = "";
        private PreviewMode _currentMode = PreviewMode.Before;
        private bool _isGeneratingPreview = false;
        private string _tempPreviewPath = "";

        private const string TP_CMD = @"TexturePacker/bin/TexturePacker.exe";
        private const string TP_ARG = @"""{0}"" --format cocos2d --disable-rotation --no-trim --algorithm MaxRects --max-size 2048 --size-constraints POT --data  ""{1}.xml"" --sheet ""{1}.png""";

        public static void ShowWindow(string folderName, string folderPath, int fileCount)
        {
            var window = GetWindow<AtlasPreviewWindow>(true, $"Preview: {folderName}");
            window.minSize = new Vector2(700, 600);
            window._folderName = folderName;
            window._folderPath = folderPath;
            window._fileCount = fileCount;
            window.LoadFileList();
            window.LoadAtlasPreview();
            window.Show();
        }

        private void OnDestroy()
        {
            CleanupTempPreview();
        }

        private void LoadFileList()
        {
            _pngFiles.Clear();
            _totalSize = 0;

            DirectoryInfo dir = new DirectoryInfo(_folderPath);
            if (dir.Exists)
            {
                var files = dir.GetFiles("*.png", SearchOption.AllDirectories);
                _pngFiles.AddRange(files);
                _totalSize = _pngFiles.Sum(f => f.Length);
            }
        }

        private void LoadAtlasPreview()
        {
            // Load existing atlas (Before)
            _atlasOutputPath = $"Assets/Atlas/English/{_folderName}.png";
            _atlasPreviewBefore = AssetDatabase.LoadAssetAtPath<Texture2D>(_atlasOutputPath);
        }

        private void GeneratePreviewAtlas()
        {
            if (_isGeneratingPreview) return;

            _isGeneratingPreview = true;
            CleanupTempPreview();

            // Create temp directory
            string tempDir = Path.Combine(Application.temporaryCachePath, "AtlasPreview");
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            _tempPreviewPath = Path.Combine(tempDir, _folderName);

            // Run TexturePacker to temp location
            var args = string.Format(TP_ARG, _folderPath, _tempPreviewPath);
            Debug.Log($"Generating preview: {TP_CMD} {args}");

            try
            {
                var process = ProcessCommand(TP_CMD, args);
                var error = process.StandardError.ReadToEnd();
                process.Close();

                // Load the generated preview
                string previewPngPath = _tempPreviewPath + ".png";
                if (File.Exists(previewPngPath))
                {
                    byte[] fileData = File.ReadAllBytes(previewPngPath);
                    _atlasPreviewAfter = new Texture2D(2, 2);
                    _atlasPreviewAfter.LoadImage(fileData);
                    Debug.Log("Preview atlas generated successfully");
                }
                else
                {
                    Debug.LogError($"Failed to generate preview: {previewPngPath}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error generating preview: {e.Message}");
            }

            _isGeneratingPreview = false;
            Repaint();
        }

        private void CleanupTempPreview()
        {
            if (!string.IsNullOrEmpty(_tempPreviewPath))
            {
                try
                {
                    // Delete temp files
                    string pngPath = _tempPreviewPath + ".png";
                    string xmlPath = _tempPreviewPath + ".xml";

                    if (File.Exists(pngPath)) File.Delete(pngPath);
                    if (File.Exists(xmlPath)) File.Delete(xmlPath);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Cleanup temp preview: {e.Message}");
                }
            }

            if (_atlasPreviewAfter != null)
            {
                DestroyImmediate(_atlasPreviewAfter);
                _atlasPreviewAfter = null;
            }
        }

        private static System.Diagnostics.Process ProcessCommand(string command, string argument)
        {
            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo(
                Application.dataPath.Replace("Assets", command));

            start.Arguments = argument;
            start.CreateNoWindow = true;
            start.ErrorDialog = true;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;
            start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;

            System.Diagnostics.Process process = System.Diagnostics.Process.Start(start);
            process.StandardInput.WriteLine("agree");
            process.WaitForExit();

            return process;
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            // Header
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.Label($"ATLAS PREVIEW: {_folderName}", headerStyle);
            GUILayout.Space(10);
            DrawSeparator();
            GUILayout.Space(10);

            // Summary
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label($"Folder: {_folderPath}", EditorStyles.miniLabel);
            GUILayout.Label($"Total PNG files: {_fileCount}", EditorStyles.miniLabel);
            GUILayout.Label($"Total size: {FormatBytes(_totalSize)}", EditorStyles.miniLabel);
            GUILayout.Label($"Estimated atlas size: 2048x2048", EditorStyles.miniLabel);
            GUILayout.Label($"Output: Assets/Atlas/English/{_folderName}.png", EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            DrawSeparator();
            GUILayout.Space(10);

            // Horizontal Split: Files List (Left) | Atlas Preview (Right)
            EditorGUILayout.BeginHorizontal();

            // LEFT: Source Files List (scrollable)
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.5f));
            GUILayout.Label("SOURCE FILES LIST:", EditorStyles.boldLabel);
            GUILayout.Space(5);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Table header
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("File Name", EditorStyles.boldLabel, GUILayout.Width(150));
            GUILayout.Label("Size (WxH)", EditorStyles.boldLabel, GUILayout.Width(80));
            GUILayout.Label("Bytes", EditorStyles.boldLabel, GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();

            foreach (var file in _pngFiles)
            {
                EditorGUILayout.BeginHorizontal();

                // Get image dimensions
                string dimensions = GetImageDimensions(file.FullName);
                string fileSize = FormatBytes(file.Length);

                GUILayout.Label(file.Name, GUILayout.Width(150));
                GUILayout.Label(dimensions, GUILayout.Width(80));
                GUILayout.Label(fileSize, GUILayout.Width(60));

                // Warning for large files
                if (file.Length > 1024 * 1024) // > 1MB
                {
                    GUILayout.Label("Large file", EditorStyles.miniLabel);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // RIGHT: Atlas Output Preview (fixed)
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.45f));
            GUILayout.Label("ATLAS OUTPUT PREVIEW:", EditorStyles.boldLabel);
            GUILayout.Space(5);

            // Before/After toggle buttons
            EditorGUILayout.BeginHorizontal();

            Color originalBgColor = GUI.backgroundColor;

            // Before button
            GUI.backgroundColor = (_currentMode == PreviewMode.Before) ? new Color(0.4f, 0.7f, 1f) : Color.white;
            if (GUILayout.Button("BEFORE (Current)", GUILayout.Height(30)))
            {
                _currentMode = PreviewMode.Before;
            }

            // After button
            GUI.backgroundColor = (_currentMode == PreviewMode.After) ? new Color(0.4f, 0.7f, 1f) : Color.white;
            if (GUILayout.Button("AFTER (Preview)", GUILayout.Height(30)))
            {
                _currentMode = PreviewMode.After;
            }

            GUI.backgroundColor = originalBgColor;
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Get the appropriate preview texture based on mode
            Texture2D currentPreview = (_currentMode == PreviewMode.Before) ? _atlasPreviewBefore : _atlasPreviewAfter;

            if (currentPreview != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                // Show atlas info
                string modeLabel = (_currentMode == PreviewMode.Before) ? "Current Atlas" : "Preview Atlas";
                GUILayout.Label($"{modeLabel}: {_atlasOutputPath}", EditorStyles.miniLabel);
                GUILayout.Label($"Size: {currentPreview.width}x{currentPreview.height}", EditorStyles.miniLabel);
                GUILayout.Space(5);

                // Calculate preview size (max 400x400 for right panel)
                float maxPreviewSize = 400f;
                float scale = Mathf.Min(maxPreviewSize / currentPreview.width, maxPreviewSize / currentPreview.height);
                float previewWidth = currentPreview.width * scale;
                float previewHeight = currentPreview.height * scale;

                // Draw preview
                Rect previewRect = GUILayoutUtility.GetRect(previewWidth, previewHeight);
                GUI.DrawTexture(previewRect, currentPreview, ScaleMode.ScaleToFit);

                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(20);

                if (_currentMode == PreviewMode.Before)
                {
                    GUILayout.Label("Atlas not yet exported.", EditorStyles.centeredGreyMiniLabel);
                    GUILayout.Label("Export first to see current atlas.", EditorStyles.centeredGreyMiniLabel);
                }
                else // After mode
                {
                    if (_isGeneratingPreview)
                    {
                        GUILayout.Label("Generating preview...", EditorStyles.centeredGreyMiniLabel);
                        GUILayout.Label("Please wait.", EditorStyles.centeredGreyMiniLabel);
                    }
                    else
                    {
                        GUILayout.Label("Preview not generated yet.", EditorStyles.centeredGreyMiniLabel);
                        GUILayout.Space(10);

                        // Generate Preview button
                        GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f); // Green
                        if (GUILayout.Button("Generate Preview", GUILayout.Height(30)))
                        {
                            GeneratePreviewAtlas();
                        }
                        GUI.backgroundColor = originalBgColor;
                    }
                }

                GUILayout.Space(20);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Close button
            if (GUILayout.Button("Close", GUILayout.Height(30)))
            {
                Close();
            }

            GUILayout.Space(10);
        }

        private void DrawSeparator()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
        }

        private string GetImageDimensions(string path)
        {
            try
            {
                Texture2D tex = new Texture2D(2, 2);
                byte[] fileData = File.ReadAllBytes(path);
                if (tex.LoadImage(fileData))
                {
                    string dims = $"{tex.width}x{tex.height}";
                    DestroyImmediate(tex);
                    return dims;
                }
            }
            catch
            {
                // Ignore errors
            }
            return "unknown";
        }

        private string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }
    }
    #endregion
}

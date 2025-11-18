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
        private const float PROGRESS_HEIGHT = 50f;
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
        private float _exportProgress = 0f;
        private string _currentExportItem = "";
        private int _exportedCount = 0;
        private int _totalExportCount = 0;
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
            RefreshAtlasFolders();
        }

        private void OnGUI()
        {
            Styles.Initialize();

            DrawHeader();
            DrawToolbar();

            // Main scroll area
            float scrollHeight = position.height - TOOLBAR_HEIGHT - 60 - PROGRESS_HEIGHT;
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(scrollHeight));

            DrawLanguageSection();
            GUILayout.Space(10);
            DrawAtlasFoldersSection();

            EditorGUILayout.EndScrollView();

            // Bottom area
            DrawExportProgress();
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
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            DrawSeparator();
        }

        private void DrawLanguageSection()
        {
            EditorGUILayout.BeginVertical(Styles.boxStyle);
            GUILayout.Label("LANGUAGE SELECTION", Styles.sectionHeaderStyle);
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            EditorGUILayout.Toggle("English", true, GUILayout.Width(150));
            GUI.enabled = true;
            GUILayout.Label("(Default language)", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
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
        }

        private void DrawExportProgress()
        {
            if (!_isExporting) return;

            EditorGUILayout.BeginVertical(Styles.boxStyle);
            GUILayout.Label("EXPORT PROGRESS", EditorStyles.boldLabel);

            // Progress bar
            Rect progressRect = EditorGUILayout.GetControlRect(false, 20);
            EditorGUI.ProgressBar(progressRect, _exportProgress, $"{Mathf.RoundToInt(_exportProgress * 100)}% ({_exportedCount}/{_totalExportCount})");

            // Current item
            GUILayout.Label($"Current: {_currentExportItem}", EditorStyles.miniLabel);

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
                Debug.LogError($"Source folder not found: {sourcePath}");
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
                    Debug.LogWarning($"Folder has no PNG files: {item.Name}");
                }
            }

            Debug.Log($"Refreshed: Found {_atlasPathList.Count} atlas folders");
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
            _exportedCount = 0;
            _totalExportCount = GetSelectedCount();
            _exportProgress = 0f;

            bool hasError = false;

            for (int i = 0; i < _atlasPathList.Count; i++)
            {
                if (!_atlasFolderSelected[i] || hasError) continue;

                string folderName = _atlasPathList[i];
                string folderPath = $"{SOURCE_PATH}/{LANGUAGE}/{folderName}";

                _currentExportItem = folderName;
                _exportProgress = (float)_exportedCount / _totalExportCount;
                Repaint();

                Debug.Log($"Exporting: {folderName}");

                if (!ExportPng(LANGUAGE, folderPath))
                {
                    hasError = true;
                    Debug.LogError($"Export failed: {folderName}");
                }
                else
                {
                    Debug.Log($"Export successful: {folderName}");
                }

                _exportedCount++;
                _exportProgress = (float)_exportedCount / _totalExportCount;
                Repaint();
            }

            _isExporting = false;
            _exportProgress = 1f;

            if (!hasError)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("All atlas exported successfully!");
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
        private string _folderName;
        private string _folderPath;
        private int _fileCount;
        private List<FileInfo> _pngFiles = new List<FileInfo>();
        private Vector2 _scrollPosition;
        private long _totalSize = 0;

        public static void ShowWindow(string folderName, string folderPath, int fileCount)
        {
            var window = GetWindow<AtlasPreviewWindow>(true, $"Preview: {folderName}");
            window.minSize = new Vector2(600, 500);
            window._folderName = folderName;
            window._folderPath = folderPath;
            window._fileCount = fileCount;
            window.LoadFileList();
            window.Show();
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

            // File list
            GUILayout.Label("FILES LIST:", EditorStyles.boldLabel);
            GUILayout.Space(5);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Table header
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("File Name", EditorStyles.boldLabel, GUILayout.Width(350));
            GUILayout.Label("Dimensions", EditorStyles.boldLabel, GUILayout.Width(100));
            GUILayout.Label("Size", EditorStyles.boldLabel, GUILayout.Width(80));
            GUILayout.Label("", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            foreach (var file in _pngFiles)
            {
                EditorGUILayout.BeginHorizontal();

                // Get image dimensions
                string dimensions = GetImageDimensions(file.FullName);
                string fileSize = FormatBytes(file.Length);

                GUILayout.Label(file.Name, GUILayout.Width(350));
                GUILayout.Label(dimensions, GUILayout.Width(100));
                GUILayout.Label(fileSize, GUILayout.Width(80));

                // Warning for large files
                if (file.Length > 1024 * 1024) // > 1MB
                {
                    GUILayout.Label("Large file", EditorStyles.miniLabel);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

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

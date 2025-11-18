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
    /// Author: Ray
    /// </summary>
    public class AtlasImporterToolEnhance : EditorWindow
    {
        #region Constants
        private const string SOURCE_PATH = "图集本地化";
        private const string PNG_PATH = @"Assets/Atlas";
        private const string LANGUAGE = "English";
        private const string TP_CMD = @"TexturePacker/bin/TexturePacker.exe";
        private const string TP_ARG = @"""{0}"" --format cocos2d --disable-rotation --no-trim --algorithm MaxRects --max-size 2048 --size-constraints POT --data  ""{1}.xml"" --sheet ""{1}.png""";

        private const float TOOLBAR_HEIGHT = 30f;
        private const float PROGRESS_HEIGHT = 60f;
        private const float MESSAGE_HEIGHT = 150f;
        private const float BUTTON_WIDTH = 120f;
        private const float SMALL_BUTTON_WIDTH = 80f;
        private const int ITEMS_PER_ROW = 2;
        #endregion

        #region Fields
        private List<string> _atlasPathList = new List<string>();
        private Dictionary<int, bool> _atlasFolderSelected = new Dictionary<int, bool>();
        private Dictionary<string, int> _atlasFolderFileCount = new Dictionary<string, int>();

        private Vector2 _scrollPosition = Vector2.zero;
        private Vector2 _messageScrollPosition = Vector2.zero;

        private string _searchFilter = "";
        private bool _isExporting = false;
        private float _exportProgress = 0f;
        private string _currentExportItem = "";
        private int _exportedCount = 0;
        private int _totalExportCount = 0;

        private List<LogMessage> _logMessages = new List<LogMessage>();
        private bool _showMessages = true;

        private static AtlasPreviewWindow _previewWindow;
        #endregion

        #region Styles
        private static class Styles
        {
            public static GUIStyle headerStyle;
            public static GUIStyle sectionHeaderStyle;
            public static GUIStyle separatorStyle;
            public static GUIStyle errorStyle;
            public static GUIStyle warningStyle;
            public static GUIStyle successStyle;
            public static GUIStyle progressBarBg;
            public static GUIStyle progressBarFg;
            public static GUIStyle boxStyle;

            public static void Initialize()
            {
                if (headerStyle != null) return;

                // Header style
                headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 14,
                    alignment = TextAnchor.MiddleLeft,
                    normal = { textColor = new Color(0.8f, 0.9f, 1f) }
                };

                // Section header style
                sectionHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 12,
                    normal = { textColor = Color.white }
                };

                // Separator style
                separatorStyle = new GUIStyle()
                {
                    normal = { background = EditorGUIUtility.whiteTexture },
                    margin = new RectOffset(0, 0, 5, 5),
                    fixedHeight = 1
                };

                // Error style
                errorStyle = new GUIStyle(EditorStyles.label)
                {
                    normal = { textColor = new Color(1f, 0.4f, 0.4f) },
                    wordWrap = true
                };

                // Warning style
                warningStyle = new GUIStyle(EditorStyles.label)
                {
                    normal = { textColor = new Color(1f, 0.9f, 0.3f) },
                    wordWrap = true
                };

                // Success style
                successStyle = new GUIStyle(EditorStyles.label)
                {
                    normal = { textColor = new Color(0.4f, 1f, 0.4f) },
                    wordWrap = true
                };

                // Box style
                boxStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(10, 10, 5, 5)
                };
            }
        }
        #endregion

        #region Log Message Class
        private class LogMessage
        {
            public enum MessageType { Success, Warning, Error }

            public MessageType Type;
            public string Message;
            public string Timestamp;

            public LogMessage(MessageType type, string message)
            {
                Type = type;
                Message = message;
                Timestamp = DateTime.Now.ToString("HH:mm:ss");
            }
        }
        #endregion

        #region Menu Item
        [MenuItem("Atlas Tools/Atlas Importer Enhanced", false)]
        static void OpenWindow()
        {
            var window = GetWindow<AtlasImporterToolEnhance>(true, "Atlas Importer Tool - Enhanced");
            window.minSize = new Vector2(600, 600);
            window.Show();
        }
        #endregion

        #region Unity Lifecycle
        private void OnEnable()
        {
            RefreshAtlasFolders();
            _logMessages.Clear();
            AddLog(LogMessage.MessageType.Success, "Atlas Importer Tool initialized successfully");
        }

        private void OnGUI()
        {
            Styles.Initialize();

            DrawHeader();
            DrawToolbar();

            // Main scroll area
            float scrollHeight = position.height - TOOLBAR_HEIGHT - 80 - PROGRESS_HEIGHT - MESSAGE_HEIGHT;
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(scrollHeight));

            DrawLanguageSection();
            DrawSeparator();
            DrawAtlasFoldersSection();

            EditorGUILayout.EndScrollView();

            // Bottom area
            DrawExportProgress();
            DrawMessagesSection();
        }
        #endregion

        #region UI Drawing Methods
        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(Styles.boxStyle);
            GUILayout.Label("🎨 ATLAS IMPORTER TOOL", Styles.headerStyle);
            GUILayout.Label("Author: Ray", EditorStyles.miniLabel);
            DrawSeparator();
            GUILayout.Label($"Source: {SOURCE_PATH}/{LANGUAGE} → Output: {PNG_PATH}", EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUI.enabled = !_isExporting && GetSelectedCount() > 0;
            if (GUILayout.Button("🚀 Export Selected", EditorStyles.toolbarButton, GUILayout.Width(BUTTON_WIDTH)))
            {
                ExportSelectedAtlas();
            }

            GUI.enabled = !_isExporting;
            if (GUILayout.Button("🔄 Refresh", EditorStyles.toolbarButton, GUILayout.Width(SMALL_BUTTON_WIDTH)))
            {
                RefreshAtlasFolders();
            }

            if (GUILayout.Button("🗑️ Clear Logs", EditorStyles.toolbarButton, GUILayout.Width(SMALL_BUTTON_WIDTH)))
            {
                _logMessages.Clear();
            }

            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawLanguageSection()
        {
            EditorGUILayout.BeginVertical(Styles.boxStyle);
            GUILayout.Label("🌐 LANGUAGE SELECTION", Styles.sectionHeaderStyle);
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            EditorGUILayout.Toggle("English", true, GUILayout.Width(150));
            GUI.enabled = true;
            GUILayout.Label("(Default language)", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            GUILayout.Space(10);
        }

        private void DrawAtlasFoldersSection()
        {
            EditorGUILayout.BeginVertical(Styles.boxStyle);
            GUILayout.Label("📁 ATLAS FOLDERS", Styles.sectionHeaderStyle);
            GUILayout.Space(5);

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

            GUILayout.Space(5);

            // Search filter
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("🔍 Filter:", GUILayout.Width(50));
            string newFilter = EditorGUILayout.TextField(_searchFilter);
            if (newFilter != _searchFilter)
            {
                _searchFilter = newFilter;
            }
            EditorGUILayout.EndHorizontal();

            DrawSeparator();

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
                bool newSelected = EditorGUILayout.ToggleLeft(displayName, isSelected, GUILayout.Width(250));
                if (newSelected != isSelected)
                {
                    _atlasFolderSelected[i] = newSelected;
                }

                // Preview button
                if (GUILayout.Button("👁️ Preview", GUILayout.Width(80)))
                {
                    ShowPreview(folderName);
                }

                if (col == ITEMS_PER_ROW - 1 || displayIndex == filteredList.Count - 1)
                {
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(3);
                }

                displayIndex++;
            }

            GUILayout.Space(5);
            DrawSeparator();

            // Statistics
            int selectedCount = GetSelectedCount();
            int totalFiles = GetTotalFilesInSelection();
            GUILayout.Label($"Selected: {selectedCount}/{_atlasPathList.Count} folders ({totalFiles} total files)", EditorStyles.boldLabel);

            EditorGUILayout.EndVertical();
        }

        private void DrawExportProgress()
        {
            if (!_isExporting) return;

            EditorGUILayout.BeginVertical(Styles.boxStyle);
            GUILayout.Label("📊 EXPORT PROGRESS", EditorStyles.boldLabel);

            // Progress bar
            Rect progressRect = EditorGUILayout.GetControlRect(false, 20);
            EditorGUI.ProgressBar(progressRect, _exportProgress, $"{Mathf.RoundToInt(_exportProgress * 100)}% ({_exportedCount}/{_totalExportCount})");

            // Current item
            GUILayout.Label($"Current: {_currentExportItem}", EditorStyles.miniLabel);

            EditorGUILayout.EndVertical();
        }

        private void DrawMessagesSection()
        {
            EditorGUILayout.BeginVertical(Styles.boxStyle);

            // Header with foldout
            EditorGUILayout.BeginHorizontal();
            _showMessages = EditorGUILayout.Foldout(_showMessages, $"📝 MESSAGES ({GetMessageStats()})", true, EditorStyles.foldoutHeader);
            EditorGUILayout.EndHorizontal();

            if (_showMessages && _logMessages.Count > 0)
            {
                _messageScrollPosition = EditorGUILayout.BeginScrollView(_messageScrollPosition, GUILayout.Height(MESSAGE_HEIGHT - 30));

                for (int i = _logMessages.Count - 1; i >= 0; i--)
                {
                    var log = _logMessages[i];
                    GUIStyle style = GetLogStyle(log.Type);
                    string icon = GetLogIcon(log.Type);
                    GUILayout.Label($"{icon} [{log.Timestamp}] {log.Message}", style);
                }

                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawSeparator()
        {
            GUILayout.Space(5);
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
            GUILayout.Space(5);
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
                AddLog(LogMessage.MessageType.Error, $"Source folder not found: {sourcePath}");
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
                    AddLog(LogMessage.MessageType.Warning, $"Folder has no PNG files: {item.Name}");
                }
            }

            AddLog(LogMessage.MessageType.Success, $"Refreshed: Found {_atlasPathList.Count} atlas folders");
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

        private string GetMessageStats()
        {
            int errors = _logMessages.Count(x => x.Type == LogMessage.MessageType.Error);
            int warnings = _logMessages.Count(x => x.Type == LogMessage.MessageType.Warning);
            int success = _logMessages.Count(x => x.Type == LogMessage.MessageType.Success);

            List<string> stats = new List<string>();
            if (errors > 0) stats.Add($"{errors} errors");
            if (warnings > 0) stats.Add($"{warnings} warnings");
            if (success > 0) stats.Add($"{success} success");

            return stats.Count > 0 ? string.Join(", ", stats) : "No messages";
        }

        private GUIStyle GetLogStyle(LogMessage.MessageType type)
        {
            switch (type)
            {
                case LogMessage.MessageType.Error: return Styles.errorStyle;
                case LogMessage.MessageType.Warning: return Styles.warningStyle;
                case LogMessage.MessageType.Success: return Styles.successStyle;
                default: return EditorStyles.label;
            }
        }

        private string GetLogIcon(LogMessage.MessageType type)
        {
            switch (type)
            {
                case LogMessage.MessageType.Error: return "❌";
                case LogMessage.MessageType.Warning: return "⚠️";
                case LogMessage.MessageType.Success: return "✅";
                default: return "ℹ️";
            }
        }

        private void AddLog(LogMessage.MessageType type, string message)
        {
            _logMessages.Add(new LogMessage(type, message));

            // Keep only last 100 messages
            if (_logMessages.Count > 100)
            {
                _logMessages.RemoveAt(0);
            }

            Repaint();
        }

        private void ShowPreview(string folderName)
        {
            string folderPath = $"{SOURCE_PATH}/{LANGUAGE}/{folderName}";
            AtlasPreviewWindow.ShowWindow(folderName, folderPath, _atlasFolderFileCount[folderName]);
        }
        #endregion

        #region Export Logic (Ported from AtlasImporterToolNew)
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

                AddLog(LogMessage.MessageType.Success, $"Exporting: {folderName}");

                if (!ExportPng(LANGUAGE, folderPath))
                {
                    hasError = true;
                    AddLog(LogMessage.MessageType.Error, $"Export failed: {folderName}");
                }
                else
                {
                    AddLog(LogMessage.MessageType.Success, $"Export successful: {folderName}");
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
                AddLog(LogMessage.MessageType.Success, "All atlas exported successfully!");
                EditorUtility.DisplayDialog("Success", "All selected atlas exported successfully!", "OK");
            }
            else
            {
                AddLog(LogMessage.MessageType.Error, "Export completed with errors. Check console for details.");
                EditorUtility.DisplayDialog("Error", "Export completed with errors. Check messages for details.", "OK");
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
                AddLog(LogMessage.MessageType.Error, $"TexturePacker failed: {fullPath} - {error}");
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
                AddLog(LogMessage.MessageType.Error, $"XML not found: {xmlPath}");
                return listObjects;
            }

            AtlasImporter.GenerateAtlasMeta(xmlObject);

            var pngObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pngPath);
            if (pngObject == null)
            {
                AddLog(LogMessage.MessageType.Error, $"PNG export failed: {pngPath}");
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
                AddLog(LogMessage.MessageType.Error, $"PNG meta generation failed: {pngMetaTmp}");
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
    /// Author: Ray
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
            window.minSize = new Vector2(500, 400);
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
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Header
            GUILayout.Label($"👁️ ATLAS PREVIEW: {_folderName}", EditorStyles.boldLabel);
            DrawSeparator();

            // Summary
            GUILayout.Label($"Folder: {_folderPath}", EditorStyles.miniLabel);
            GUILayout.Label($"Total PNG files: {_fileCount}", EditorStyles.miniLabel);
            GUILayout.Label($"Total size: {FormatBytes(_totalSize)}", EditorStyles.miniLabel);
            GUILayout.Label($"Estimated atlas size: 2048x2048", EditorStyles.miniLabel);
            GUILayout.Label($"Output: Assets/Atlas/English/{_folderName}.png", EditorStyles.miniLabel);

            DrawSeparator();

            // File list
            GUILayout.Label("📄 Files List:", EditorStyles.boldLabel);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (var file in _pngFiles)
            {
                EditorGUILayout.BeginHorizontal();

                // Get image dimensions if possible
                string dimensions = GetImageDimensions(file.FullName);
                string fileSize = FormatBytes(file.Length);

                GUILayout.Label($"• {file.Name}", GUILayout.Width(300));
                GUILayout.Label(dimensions, GUILayout.Width(100));
                GUILayout.Label(fileSize, GUILayout.Width(80));

                // Warning for large files
                if (file.Length > 1024 * 1024) // > 1MB
                {
                    GUILayout.Label("⚠️ Large file", EditorStyles.miniLabel);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

            // Close button
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close", GUILayout.Height(30)))
            {
                Close();
            }
        }

        private void DrawSeparator()
        {
            GUILayout.Space(5);
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
            GUILayout.Space(5);
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

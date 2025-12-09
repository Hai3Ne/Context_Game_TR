/*
 * ============================================================================
 * Smart Copy Diff Tool V5 (Dual Mode Version)
 * ============================================================================
 *
 * Author: Ray
 *
 * Description:
 * Tool to compare manifest files to export changed files with 2 modes:
 * - CDN Mode: Compare Local vs CDN manifest (download from URL)
 * - Local Mode: Compare 2 local manifest files (files.txt)
 *
 * Features:
 * - Removed EditorCoroutine, uses EditorApplication.update for CDN download.
 * - Dictionary comparison logic based on original source (CopyDiffFile.cs).
 * - Path Normalization to fix issues on Windows.
 * - Supports both modes: CDN and Local comparison.
 *
 * Usage:
 * Unity Editor -> Tamron -> SmartCopyDiff
 *
 * ============================================================================
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace SEZSJ
{
    public enum CompareMode
    {
        CDN,    // Compare with manifest from CDN
        Local   // Compare 2 local manifest files
    }

    public class SmartCopyDiff : EditorWindow
    {
        #region Constants
        // Mode Local prefs
        private const string PREF_OLD_ASSETS_PATH = "SmartCopy_OldAssetsPath";
        private const string PREF_NEW_ASSETS_PATH = "SmartCopy_NewAssetsPath";

        // Mode CDN prefs
        private const string PREF_LOCAL_PATH = "SmartCopy_LocalPath";
        private const string PREF_CDN_URL = "SmartCopy_CdnUrl";

        // Common prefs
        private const string PREF_OUT_PATH = "SmartCopy_OutPath";
        private const string PREF_MODE = "SmartCopy_Mode";

        private const string MANIFEST_NAME = "files.txt";
        #endregion

        #region Fields
        private CompareMode _mode = CompareMode.Local;

        // Mode Local: Compare 2 folders (like CopyDiffFile.cs)
        private string _oldAssetsPath = "";  // Path 1 (OLD folder)
        private string _newAssetsPath = "";  // Path 2 (NEW folder)

        // Mode CDN: Compare CDN vs Local
        private string _localBuildPath = "";  // Local folder
        private string _cdnBaseUrl = "";      // CDN URL

        // Common
        private string _outPath = "";  // Output export path

        private Vector2 _logScrollPosition = Vector2.zero;
        private StringBuilder _logBuilder = new StringBuilder();

        // Processing state
        private bool _isProcessing = false;
        private int _step = 0;
        private UnityWebRequest _currentRequest;
        #endregion

        #region Styles
        private static class Styles
        {
            public static GUIStyle headerStyle;
            public static GUIStyle sectionHeaderStyle;
            public static GUIStyle boxStyle;
            public static GUIStyle buildButtonStyle;
            public static GUIStyle logStyle;

            public static void Initialize()
            {
                if (headerStyle != null) return;

                headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 16,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = new Color(0.9f, 0.95f, 1f) }
                };

                sectionHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 13,
                    alignment = TextAnchor.MiddleLeft,
                    normal = { textColor = new Color(1f, 1f, 0.7f) },
                    padding = new RectOffset(5, 5, 5, 5)
                };

                boxStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(15, 15, 15, 15),
                    margin = new RectOffset(5, 5, 10, 10)
                };

                buildButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,
                    padding = new RectOffset(10, 10, 12, 12)
                };

                logStyle = new GUIStyle(EditorStyles.textArea);
                logStyle.richText = true;
            }
        }
        #endregion

        #region Menu Item
        [MenuItem("Tamron/SmartCopyDiff", false, 50)]
        static void OpenWindow()
        {
            var window = GetWindow<SmartCopyDiff>(true, "Smart Copy Diff");
            window.minSize = new Vector2(600, 700);
            window.Show();
        }
        #endregion

        #region Unity Lifecycle
        private void OnEnable()
        {
            LoadPreferences();
        }

        private void OnDisable()
        {
            SavePreferences();
            if (_currentRequest != null)
            {
                _currentRequest.Dispose();
                _currentRequest = null;
            }
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnGUI()
        {
            Styles.Initialize();
            
            DrawHeader();
            DrawConfig();
            DrawActions(); // Moved Actions ABOVE Logs
            DrawLogs();
        }
        #endregion

        #region UI Drawing Methods
        private void DrawHeader()
        {
            GUILayout.Space(10);
            GUILayout.Label("SMART COPY DIFF", Styles.headerStyle);
            
            GUILayout.Space(5);
            if (_mode == CompareMode.CDN)
                GUILayout.Label("Logic: Download CDN Manifest -> Hash Local -> Diff Dictionary -> Copy", EditorStyles.centeredGreyMiniLabel);
            else
                GUILayout.Label("Logic: Load Remote Manifest -> Hash Local -> Diff Dictionary -> Copy", EditorStyles.centeredGreyMiniLabel);
            
            GUILayout.Space(10);
            DrawSeparator();
        }

        private void DrawConfig()
        {
            EditorGUILayout.BeginVertical(Styles.boxStyle);
            GUILayout.Label("SETTINGS", Styles.sectionHeaderStyle);
            GUILayout.Space(5);
            DrawSeparator();
            GUILayout.Space(10);

            // Mode Selection (Compact Toolbar Style)
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Mode:", EditorStyles.boldLabel, GUILayout.Width(60));
            _mode = (CompareMode)GUILayout.Toolbar((int)_mode, new string[] { "CDN Comparison", "Local Comparison" }, GUILayout.Height(25));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (_mode == CompareMode.CDN)
            {
                // CDN Mode: Local folder + CDN URL + Output
                GUILayout.Label("Local Build Folder:");
                EditorGUILayout.BeginHorizontal();
                _localBuildPath = EditorGUILayout.TextField(_localBuildPath);
                if (GUILayout.Button("...", GUILayout.Width(30)))
                {
                    string path = EditorUtility.OpenFolderPanel("Choose Local Folder", _localBuildPath, "");
                    if (!string.IsNullOrEmpty(path)) _localBuildPath = path;
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Label("CDN URL (Folder containing files.txt):");
                _cdnBaseUrl = EditorGUILayout.TextField(_cdnBaseUrl);
                EditorGUILayout.HelpBox($"Target Manifest: {_cdnBaseUrl.TrimEnd('/')}/{MANIFEST_NAME}", MessageType.Info);

                GUILayout.Label("Output Folder (Export):");
                EditorGUILayout.BeginHorizontal();
                _outPath = EditorGUILayout.TextField(_outPath);
                if (GUILayout.Button("...", GUILayout.Width(30)))
                {
                    string path = EditorUtility.OpenFolderPanel("Choose Output", _outPath, "");
                    if (!string.IsNullOrEmpty(path)) _outPath = path;
                }
                EditorGUILayout.EndHorizontal();
            }
            else // Local mode
            {
                // Local Mode: Path 1 (OLD) + Path 2 (NEW) + Output (giống CopyDiffFile.cs)
                GUILayout.Label("Path 1: OLD Assets Bundle (Baseline):");
                EditorGUILayout.BeginHorizontal();
                _oldAssetsPath = EditorGUILayout.TextField(_oldAssetsPath);
                if (GUILayout.Button("...", GUILayout.Width(30)))
                {
                    string path = EditorUtility.OpenFolderPanel("Choose OLD Assets Folder", _oldAssetsPath, "");
                    if (!string.IsNullOrEmpty(path)) _oldAssetsPath = path;
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Label("Path 2: NEW Assets Bundle (Source to compare):");
                EditorGUILayout.BeginHorizontal();
                _newAssetsPath = EditorGUILayout.TextField(_newAssetsPath);
                if (GUILayout.Button("...", GUILayout.Width(30)))
                {
                    string path = EditorUtility.OpenFolderPanel("Choose NEW Assets Folder", _newAssetsPath, "");
                    if (!string.IsNullOrEmpty(path)) _newAssetsPath = path;
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Label("Path 3: Output Export Folder:");
                EditorGUILayout.BeginHorizontal();
                _outPath = EditorGUILayout.TextField(_outPath);
                if (GUILayout.Button("...", GUILayout.Width(30)))
                {
                    string path = EditorUtility.OpenFolderPanel("Choose Output", _outPath, "");
                    if (!string.IsNullOrEmpty(path)) _outPath = path;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox("Logic: Compare NEW vs OLD, copy different files from NEW to OUTPUT (like CopyDiffFile.cs)", MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawActions()
        {
            // Removed Spacer and Separator here to make it tighter with Config
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.enabled = !_isProcessing;
            Color originalBgColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f); // Green

            if (GUILayout.Button("START SMART EXPORT", Styles.buildButtonStyle, GUILayout.Width(250), GUILayout.Height(50)))
            {
                StartProcess();
            }

            GUI.backgroundColor = originalBgColor;
            GUI.enabled = true;

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        private void DrawLogs()
        {
            EditorGUILayout.BeginVertical(Styles.boxStyle);
            GUILayout.Label("LOGS", Styles.sectionHeaderStyle);
            
            _logScrollPosition = EditorGUILayout.BeginScrollView(_logScrollPosition, "box", GUILayout.Height(300)); // Fixed height for logs
            GUILayout.Label(_logBuilder.ToString(), Styles.logStyle);
            EditorGUILayout.EndScrollView();
            
            if (GUILayout.Button("Clear Logs", GUILayout.Height(25))) 
                _logBuilder.Clear();
            
            EditorGUILayout.EndVertical();
        }

        private void DrawSeparator()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
        }
        #endregion

        #region Core Logic
        private void StartProcess()
        {
            _logBuilder.Clear();

            if (_mode == CompareMode.Local)
            {
                // Mode Local: Compare 2 folders (like CopyDiffFile.cs)
                // Validate paths
                if (!Directory.Exists(_oldAssetsPath))
                {
                    AddLog("ERROR: OLD Assets path not found!", "red");
                    return;
                }
                if (!Directory.Exists(_newAssetsPath))
                {
                    AddLog("ERROR: NEW Assets path not found!", "red");
                    return;
                }

                AddLog("=== STARTING PROCESS (LOCAL MODE) ===", "cyan");
                AddLog($"OLD Path: {_oldAssetsPath}", "white");
                AddLog($"NEW Path: {_newAssetsPath}", "white");
                AddLog($"Output: {_outPath}", "white");

                // Process: Parse/Hash OLD → Parse/Hash NEW → Compare → Copy
                ProcessLocalComparison();
            }
            else // CDN Mode
            {
                if (!Directory.Exists(_localBuildPath))
                {
                    AddLog("ERROR: Local path not found!", "red");
                    return;
                }
                if (string.IsNullOrEmpty(_cdnBaseUrl))
                {
                    AddLog("ERROR: CDN URL is empty!", "red");
                    return;
                }

                _isProcessing = true;
                _step = 1; // Step 1: Download
                EditorApplication.update += OnEditorUpdate;
                AddLog("=== STARTING PROCESS (CDN MODE) ===", "cyan");
            }
        }

        // Main loop replacing Coroutine
        private void OnEditorUpdate()
        {
            if (!_isProcessing) return;

            if (_step == 1) // DOWNLOAD STEP
            {
                if (_currentRequest == null)
                {
                    string url = _cdnBaseUrl.TrimEnd('/') + "/" + MANIFEST_NAME;
                    AddLog($"Downloading: {url} ...", "yellow");
                    _currentRequest = UnityWebRequest.Get(url);
                    _currentRequest.SendWebRequest();
                }

                if (_currentRequest.isDone)
                {
                    if (_currentRequest.result != UnityWebRequest.Result.Success)
                    {
                        AddLog($"Download Failed: {_currentRequest.error}", "red");
                        AddLog("-> Assuming this is the FIRST UPLOAD (Remote list empty).", "orange");
                        ProcessDiff(new Dictionary<string, string>()); // Remote empty
                    }
                    else
                    {
                        string content = _currentRequest.downloadHandler.text;
                        AddLog($"Download Success! Size: {content.Length} bytes", "green");
                        // Print first 500 characters to debug if it is a valid text file
                        string preview = content.Length > 500 ? content.Substring(0, 500) + "..." : content;
                        AddLog($"Content Preview:\n{preview}", "grey");
                        
                        var remoteDic = ParseManifest(content);
                        ProcessDiff(remoteDic);
                    }

                    _currentRequest.Dispose();
                    _currentRequest = null;
                    _step = 0; // End download loop
                    _isProcessing = false; // ProcessDiff runs synchronously so it's done after this
                    EditorApplication.update -= OnEditorUpdate;
                }
                else
                {
                    EditorUtility.DisplayProgressBar("Smart Copy", "Downloading manifest...", _currentRequest.downloadProgress);
                }
            }
        }

        // Local Mode Logic: Compare 2 folders (like CopyDiffFile.cs)
        private void ProcessLocalComparison()
        {
            // Step 1: Parse/Hash OLD folder → dicOld
            AddLog("--------------------------------", "cyan");
            AddLog("STEP 1: Processing OLD Assets...", "yellow");
            var dicOld = ParseOrHashFolder(_oldAssetsPath, "OLD");

            // Step 2: Parse/Hash NEW folder → dicNew
            AddLog("--------------------------------", "cyan");
            AddLog("STEP 2: Processing NEW Assets...", "yellow");
            var dicNew = ParseOrHashFolder(_newAssetsPath, "NEW");

            // Step 3: Compare dicNew vs dicOld (like CopyDiffFile.cs logic)
            AddLog("--------------------------------", "cyan");
            AddLog("STEP 3: COMPARING NEW vs OLD...", "yellow");
            AddLog($"OLD Count: {dicOld.Count}", "white");
            AddLog($"NEW Count: {dicNew.Count}", "white");

            List<string> filesToCopy = new List<string>();
            int matchCount = 0;
            int changeCount = 0;
            int newCount = 0;

            foreach (var item in dicNew)
            {
                string pathKey = item.Key;
                string newMD5 = item.Value.md5;

                if (dicOld.ContainsKey(pathKey))
                {
                    string oldMD5 = dicOld[pathKey].md5;
                    if (!string.Equals(oldMD5, newMD5, StringComparison.OrdinalIgnoreCase))
                    {
                        // File changed
                        changeCount++;
                        filesToCopy.Add(item.Value.name);
                        AddLog($"[CHANGE] {pathKey}", "yellow");
                        AddLog($"  OLD MD5: {oldMD5}", "grey");
                        AddLog($"  NEW MD5: {newMD5}", "grey");
                    }
                    else
                    {
                        matchCount++;
                    }
                }
                else
                {
                    // New file
                    newCount++;
                    filesToCopy.Add(item.Value.name);
                    AddLog($"[NEW] {pathKey}", "green");
                }
            }

            AddLog("--------------------------------", "white");
            AddLog($"SUMMARY: Matched={matchCount}, Changed={changeCount}, New={newCount}", "cyan");
            AddLog($"Total files to copy: {filesToCopy.Count}", "cyan");

            // Step 4: Copy files from NEW to OUTPUT
            if (filesToCopy.Count == 0)
            {
                EditorUtility.DisplayDialog("Smart Copy", "No files to copy. Everything is up-to-date!", "OK");
                AddLog("DONE! No changes detected.", "green");
                return;
            }

            AddLog("--------------------------------", "cyan");
            AddLog("STEP 4: COPYING FILES...", "yellow");

            if (Directory.Exists(_outPath)) Directory.Delete(_outPath, true);
            Directory.CreateDirectory(_outPath);

            int copyIndex = 0;
            foreach (var srcFile in filesToCopy)
            {
                string relative = NormalizePath(srcFile.Substring(_newAssetsPath.Length));
                string destFile = Path.Combine(_outPath, relative.Replace("/", "\\"));
                string destDir = Path.GetDirectoryName(destFile);

                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                File.Copy(srcFile, destFile, true);

                copyIndex++;
                if (copyIndex % 10 == 0)
                    EditorUtility.DisplayProgressBar("Copying", $"Copying {relative}...", (float)copyIndex / filesToCopy.Count);
            }

            EditorUtility.ClearProgressBar();
            AddLog("--------------------------------", "green");
            AddLog($"SUCCESS! Copied {filesToCopy.Count} files to {_outPath}", "green");
            EditorUtility.RevealInFinder(_outPath);
            EditorUtility.DisplayDialog("Smart Copy", $"Done! Copied {filesToCopy.Count} files.", "OK");
        }

        // Helper: Parse manifest or hash folder
        private Dictionary<string, CopyDiffData> ParseOrHashFolder(string folderPath, string label)
        {
            var dict = new Dictionary<string, CopyDiffData>();

            // Check if folder has files.txt
            string manifestPath = Path.Combine(folderPath, MANIFEST_NAME);

            if (File.Exists(manifestPath))
            {
                // Parse manifest
                AddLog($"Found {label} manifest: {MANIFEST_NAME}", "cyan");
                AddLog($"Parsing {label} manifest (NO HASHING)...", "yellow");

                string content = File.ReadAllText(manifestPath);
                var manifest = ParseManifest(content);

                foreach (var entry in manifest)
                {
                    string relativePath = entry.Key;
                    string md5 = entry.Value;
                    string fullPath = Path.Combine(folderPath, relativePath.Replace("/", "\\"));

                    CopyDiffData data = new CopyDiffData { name = fullPath, md5 = md5 };
                    dict.Add(relativePath, data);
                }

                AddLog($"{label} Manifest Parsed: {dict.Count} files", "green");
            }
            else
            {
                // Hash folder
                AddLog($"No {label} manifest found. Scanning and hashing...", "yellow");

                string[] allFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
                int scanIndex = 0;

                foreach (var filePath in allFiles)
                {
                    string fileName = Path.GetFileName(filePath);
                    if (fileName.EndsWith(".meta") || fileName.EndsWith(".manifest") || fileName == MANIFEST_NAME) continue;

                    string relativePath = NormalizePath(filePath.Substring(folderPath.Length));
                    string md5 = Md5Util.GetFileHash(filePath);

                    CopyDiffData data = new CopyDiffData { name = filePath, md5 = md5 };
                    dict.Add(relativePath, data);

                    scanIndex++;
                    if (scanIndex % 50 == 0)
                        EditorUtility.DisplayProgressBar($"Hashing {label}", $"Processing {fileName}...", (float)scanIndex / allFiles.Length);
                }

                EditorUtility.ClearProgressBar();
                AddLog($"{label} Files Hashed: {dict.Count}", "green");
            }

            return dict;
        }

        // Main Logic: Parse/Hash -> Compare -> Copy (for CDN mode)
        private void ProcessDiff(Dictionary<string, string> dicRemote)
        {
            EditorUtility.ClearProgressBar();
            AddLog("--------------------------------", "white");
            AddLog($"Remote Files Count: {dicRemote.Count}", "white");

            // 1. Check if local folder has files.txt
            string localManifestPath = Path.Combine(_localBuildPath, MANIFEST_NAME);
            Dictionary<string, CopyDiffData> dicLocal = new Dictionary<string, CopyDiffData>();

            if (File.Exists(localManifestPath))
            {
                // Parse local files.txt instead of hashing
                AddLog($"Found local manifest: {MANIFEST_NAME}", "cyan");
                AddLog("Parsing local manifest (NO HASHING)...", "yellow");

                string content = File.ReadAllText(localManifestPath);
                var localManifest = ParseManifest(content);

                foreach (var entry in localManifest)
                {
                    string relativePath = entry.Key;
                    string md5 = entry.Value;
                    string fullPath = Path.Combine(_localBuildPath, relativePath.Replace("/", "\\"));

                    CopyDiffData data = new CopyDiffData { name = fullPath, md5 = md5 };
                    dicLocal.Add(relativePath, data);
                }

                AddLog($"Local Manifest Parsed: {dicLocal.Count} files", "green");
            }
            else
            {
                // Scan and hash if files.txt is missing
                AddLog($"No local manifest found. Scanning and hashing files...", "yellow");

                string[] allFiles = Directory.GetFiles(_localBuildPath, "*.*", SearchOption.AllDirectories);
                int scanIndex = 0;

                foreach (var filePath in allFiles)
                {
                    string fileName = Path.GetFileName(filePath);
                    if (fileName.EndsWith(".meta") || fileName.EndsWith(".manifest") || fileName == MANIFEST_NAME) continue;

                    // Normalize Path: Remove root, replace \ with /, trim slashes
                    string relativePath = NormalizePath(filePath.Substring(_localBuildPath.Length));

                    // Calculate MD5
                    string md5 = Md5Util.GetFileHash(filePath);

                    CopyDiffData data = new CopyDiffData { name = filePath, md5 = md5 };
                    dicLocal.Add(relativePath, data);

                    scanIndex++;
                    if (scanIndex % 50 == 0)
                        EditorUtility.DisplayProgressBar("Hashing", $"Processing {fileName}...", (float)scanIndex / allFiles.Length);
                }

                AddLog($"Local Files Scanned: {dicLocal.Count}", "white");
            }

            // Debug: Show sample paths for verification
            if (dicRemote.Count > 0)
            {
                AddLog("Sample Remote Paths:", "grey");
                int count = 0;
                foreach (var key in dicRemote.Keys)
                {
                    AddLog($"  - {key}", "grey");
                    if (++count >= 3) break;
                }
            }

            if (dicLocal.Count > 0)
            {
                AddLog("Sample Local Paths:", "grey");
                int count = 0;
                foreach (var key in dicLocal.Keys)
                {
                    AddLog($"  - {key}", "grey");
                    if (++count >= 3) break;
                }
            }

            // 2. Comparison (Logic from your old source)
            // Dic1 = dicRemote (Key: path, Value: md5)
            // Dic2 = dicLocal (Key: path, Value: data object)

            AddLog("--------------------------------", "white");
            AddLog("STARTING COMPARISON...", "cyan");

            List<string> listToCopy = new List<string>();
            StringBuilder newManifest = new StringBuilder();
            int matchCount = 0;
            int changeCount = 0;
            int newCount = 0;

            foreach (var item in dicLocal)
            {
                string pathKey = item.Key;     // Relative Path
                string localMD5 = item.Value.md5;

                // Add to new manifest immediately
                newManifest.AppendLine($"{pathKey}|{localMD5}");

                // Comparison logic:
                bool needCopy = false;
                if (dicRemote.ContainsKey(pathKey))
                {
                    string remoteMD5 = dicRemote[pathKey];
                    // Compare case-insensitive to be safe
                    if (!string.Equals(remoteMD5, localMD5, StringComparison.OrdinalIgnoreCase))
                    {
                        needCopy = true;
                        changeCount++;
                        AddLog($"[CHANGE] {pathKey}", "yellow");
                        AddLog($"  Remote MD5: {remoteMD5}", "grey");
                        AddLog($"  Local MD5:  {localMD5}", "grey");
                    }
                    else
                    {
                        matchCount++;
                    }
                }
                else
                {
                    needCopy = true; // New file
                    newCount++;
                    AddLog($"[NEW] {pathKey}", "green");
                    AddLog($"  Local MD5: {localMD5}", "grey");
                }

                if (needCopy)
                {
                    listToCopy.Add(item.Value.name); // Add full path
                }
            }

            AddLog("--------------------------------", "white");
            AddLog($"SUMMARY: Matched={matchCount}, Changed={changeCount}, New={newCount}", "cyan");

            // 3. Execute Copy
            if (Directory.Exists(_outPath)) Directory.Delete(_outPath, true);
            Directory.CreateDirectory(_outPath);

            int copyIndex = 0;
            foreach (var srcFile in listToCopy)
            {
                // Reconstruct destination path
                string relative = srcFile.Substring(_localBuildPath.Length).TrimStart('\\', '/');
                string destFile = Path.Combine(_outPath, relative);
                string destDir = Path.GetDirectoryName(destFile);

                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                File.Copy(srcFile, destFile, true);

                copyIndex++;
                EditorUtility.DisplayProgressBar("Copying", $"Copying {relative}...", (float)copyIndex / listToCopy.Count);
            }

            // 4. Export new files.txt
            string manifestPath = Path.Combine(_outPath, MANIFEST_NAME);
            File.WriteAllText(manifestPath, newManifest.ToString());

            EditorUtility.ClearProgressBar();
            AddLog("--------------------------------", "cyan");
            AddLog($"DONE! Copied {listToCopy.Count} files.", "cyan");
            EditorUtility.RevealInFinder(_outPath);
            EditorUtility.DisplayDialog("Smart Copy", $"Done! Copied {listToCopy.Count} files.", "OK");
        }

        // Helper method to normalize path
        private string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;
            // Replace \ with /, trim leading/trailing slashes
            return path.Replace("\\", "/").Trim('/');
        }

        private Dictionary<string, string> ParseManifest(string content)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            using (StringReader reader = new StringReader(content))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] parts = line.Split('|');
                    if (parts.Length >= 2)
                    {
                        // Normalize key from manifest file
                        string key = NormalizePath(parts[0].Trim());
                        string val = parts[1].Trim();
                        dict[key] = val;
                    }
                }
            }
            return dict;
        }

        private void LoadPreferences()
        {
            // Mode Local
            _oldAssetsPath = EditorPrefs.GetString(PREF_OLD_ASSETS_PATH, "");
            _newAssetsPath = EditorPrefs.GetString(PREF_NEW_ASSETS_PATH, "");

            // Mode CDN
            _localBuildPath = EditorPrefs.GetString(PREF_LOCAL_PATH, "");
            _cdnBaseUrl = EditorPrefs.GetString(PREF_CDN_URL, "");

            // Common
            _outPath = EditorPrefs.GetString(PREF_OUT_PATH, "");
            _mode = (CompareMode)EditorPrefs.GetInt(PREF_MODE, (int)CompareMode.Local);
        }

        private void SavePreferences()
        {
            // Mode Local
            EditorPrefs.SetString(PREF_OLD_ASSETS_PATH, _oldAssetsPath);
            EditorPrefs.SetString(PREF_NEW_ASSETS_PATH, _newAssetsPath);

            // Mode CDN
            EditorPrefs.SetString(PREF_LOCAL_PATH, _localBuildPath);
            EditorPrefs.SetString(PREF_CDN_URL, _cdnBaseUrl);

            // Common
            EditorPrefs.SetString(PREF_OUT_PATH, _outPath);
            EditorPrefs.SetInt(PREF_MODE, (int)_mode);
        }

        private void AddLog(string msg, string color = "white")
        {
            _logBuilder.AppendLine($"<color={color}>{msg}</color>");
            _logScrollPosition.y = float.MaxValue;
            Repaint();
        }
        #endregion

        // Helper class similar to your old code to store info
        public class CopyDiffData
        {
            public string name;
            public string md5; 
        }
    }
}
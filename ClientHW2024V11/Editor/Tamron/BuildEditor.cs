/*
 * ============================================================================
 * Build Editor Tool
 * ============================================================================
 *
 * Author: Ray
 * Created: November 2025
 *
 * Description:
 *   Unity Editor tool for building Android APK with customizable settings.
 *   Provides options for asset bundle export, SDK configuration, and
 *   automated keystore management.
 *
 * Features:
 *   - Optional asset bundle export before build
 *   - SDK type and channel configuration
 *   - AndroidManifest.xml modification via string replacement
 *   - Keystore configuration for APK signing
 *   - Custom APK naming convention
 *   - Build size estimation
 *   - Package settings configuration (turboid, turboname, dyid, wxapi)
 *
 * Usage:
 *   Unity Editor → Tools → Tamron → Build Editor
 *
 * ============================================================================
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace SEZSJ
{
    /// <summary>
    /// Build Editor Tool for Android APK building with advanced settings
    /// </summary>
    public class BuildEditor : EditorWindow
    {
        #region Constants
        private const string KEYSTORE_PATH = "keystore/test.jks";
        private const string KEYSTORE_ALIAS = "testalias";
        private const string KEYSTORE_PASSWORD = "123456";
        private const string MANIFEST_PATH = "Assets/Plugins/Android/AndroidManifest.xml";
        private const string OUTPUT_FOLDER = "AndroidApks";

        private static readonly int[] NUMBER_TYPE_OPTIONS = { 1, 1000, 2000, 9999 };
        private static readonly string[] NUMBER_TYPE_LABELS = { "1", "1000", "2000", "9999" };
        #endregion

        #region Fields
        private bool _exportAssetsBeforeBuild = false;
        private int _selectedNumberTypeIndex = 0;
        private string _apkVersionNumber = "";
        private Vector2 _scrollPosition = Vector2.zero;
        private int _selectedTab = 0;
        private readonly string[] _tabNames = { "Build Settings", "Package Settings" };

        // Package Settings
        private string _turboId = "";
        private string _turboName = "";
        private string _dyId = "";
        private string _manifestPackageName = "";

        // Build info
        private string _lastBuildPath = "";
        private long _estimatedSize = 0;
        #endregion

        #region Styles
        private static class Styles
        {
            public static GUIStyle headerStyle;
            public static GUIStyle sectionHeaderStyle;
            public static GUIStyle boxStyle;
            public static GUIStyle buildButtonStyle;
            public static GUIStyle tabButtonStyle;
            public static GUIStyle tabButtonActiveStyle;

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

                tabButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 12,
                    padding = new RectOffset(10, 10, 8, 8)
                };

                tabButtonActiveStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    padding = new RectOffset(10, 10, 8, 8),
                    normal = { background = Texture2D.grayTexture }
                };
            }
        }
        #endregion

        #region Menu Item
        [MenuItem("Tools/Tamron/Build Editor", false)]
        static void OpenWindow()
        {
            var window = GetWindow<BuildEditor>(true, "Build Editor");
            window.minSize = new Vector2(650, 650);
            window.Show();
        }
        #endregion

        #region Unity Lifecycle
        private void OnEnable()
        {
            LoadPreferences();
            LoadManifestDefaults();
        }

        private void OnDisable()
        {
            SavePreferences();
        }

        private void OnGUI()
        {
            Styles.Initialize();

            DrawHeader();
            DrawTabs();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            if (_selectedTab == 0)
            {
                DrawPreBuildOptionsSection();
                DrawBuildSettingsSection();
                DrawKeystoreSettingsSection();
                DrawBuildInfoSection();
            }
            else if (_selectedTab == 1)
            {
                DrawPackageSettingsSection();
            }

            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            DrawBuildActionsSection();
        }
        #endregion

        #region UI Drawing Methods
        private void DrawHeader()
        {
            GUILayout.Space(10);
            GUILayout.Label("BUILD EDITOR TOOL", Styles.headerStyle);
            GUILayout.Space(10);
            DrawSeparator();
        }

        private void DrawTabs()
        {
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            for (int i = 0; i < _tabNames.Length; i++)
            {
                GUIStyle style = (_selectedTab == i) ? Styles.tabButtonActiveStyle : Styles.tabButtonStyle;
                Color originalBg = GUI.backgroundColor;

                if (_selectedTab == i)
                {
                    GUI.backgroundColor = new Color(0.4f, 0.7f, 1f);
                }

                if (GUILayout.Button(_tabNames[i], style, GUILayout.Width(150), GUILayout.Height(30)))
                {
                    _selectedTab = i;
                }

                GUI.backgroundColor = originalBg;
                GUILayout.Space(5);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            DrawSeparator();
        }

        private void DrawPreBuildOptionsSection()
        {
            EditorGUILayout.BeginVertical(Styles.boxStyle);

            GUILayout.Label("PRE-BUILD OPTIONS", Styles.sectionHeaderStyle);
            GUILayout.Space(5);
            DrawSeparator();
            GUILayout.Space(10);

            _exportAssetsBeforeBuild = EditorGUILayout.ToggleLeft(
                "Export All Assets And SubPack before build",
                _exportAssetsBeforeBuild,
                GUILayout.Height(20)
            );

            if (_exportAssetsBeforeBuild)
            {
                EditorGUILayout.HelpBox(
                    "This will call HandleAllAndSubBundle() before building the APK.",
                    MessageType.Info
                );
            }

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }

        private void DrawBuildSettingsSection()
        {
            EditorGUILayout.BeginVertical(Styles.boxStyle);

            GUILayout.Label("BUILD SETTINGS", Styles.sectionHeaderStyle);
            GUILayout.Space(5);
            DrawSeparator();
            GUILayout.Space(10);

            // Number Type Selection
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Number Type:", GUILayout.Width(120));
            _selectedNumberTypeIndex = EditorGUILayout.Popup(_selectedNumberTypeIndex, NUMBER_TYPE_LABELS);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Show SDK configuration based on number type
            int numberType = NUMBER_TYPE_OPTIONS[_selectedNumberTypeIndex];
            var sdkConfig = GetSDKConfiguration(numberType);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("SDK Configuration:", EditorStyles.miniLabel);
            GUILayout.Label($"  SDK Type: {sdkConfig.sdkType}", EditorStyles.miniLabel);
            GUILayout.Label($"  SDK Channel: {sdkConfig.sdkChannel}", EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // APK Version Number
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("APK Version Number:", GUILayout.Width(120));
            _apkVersionNumber = EditorGUILayout.TextField(_apkVersionNumber);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            // APK Name Preview
            string packageName = PlayerSettings.applicationIdentifier;
            string apkName = string.IsNullOrEmpty(_apkVersionNumber)
                ? $"{packageName}-[number].apk"
                : $"{packageName}-{_apkVersionNumber}.apk";

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("APK Name Preview:", EditorStyles.miniLabel);
            GUILayout.Label($"  {apkName}", EditorStyles.boldLabel);
            GUILayout.Label($"Output: {OUTPUT_FOLDER}/{apkName}", EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }

        private void DrawPackageSettingsSection()
        {
            EditorGUILayout.BeginVertical(Styles.boxStyle);

            GUILayout.Label("PACKAGE SETTINGS", Styles.sectionHeaderStyle);
            GUILayout.Space(5);
            DrawSeparator();
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "These settings will be applied to AndroidManifest.xml during build.",
                MessageType.Info
            );

            GUILayout.Space(10);

            // Manifest Package Name
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Manifest Package:", GUILayout.Width(150));
            _manifestPackageName = EditorGUILayout.TextField(_manifestPackageName);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            // Turbo ID (kaishou id)
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Turbo ID (kaishou id):", GUILayout.Width(150));
            _turboId = EditorGUILayout.TextField(_turboId);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            // Turbo Name (kaishou name)
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Turbo Name (kaishou name):", GUILayout.Width(150));
            _turboName = EditorGUILayout.TextField(_turboName);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            // DY ID (douyin id)
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("DY ID (douyin id):", GUILayout.Width(150));
            _dyId = EditorGUILayout.TextField(_dyId);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(15);

            // Preview section
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("AndroidManifest.xml Preview:", EditorStyles.boldLabel);
            GUILayout.Space(5);

            GUILayout.Label($"package=\"{_manifestPackageName}\"", EditorStyles.miniLabel);
            GUILayout.Label($"turboid value=\"{_turboId}\"", EditorStyles.miniLabel);
            GUILayout.Label($"turboname value=\"{_turboName}\"", EditorStyles.miniLabel);
            GUILayout.Label($"dyid value=\"{_dyId}\"", EditorStyles.miniLabel);

            GUILayout.Space(5);
            GUILayout.Label("WXEntry Activities:", EditorStyles.miniLabel);
            GUILayout.Label($"  {_manifestPackageName}.wxapi.WXEntryActivity", EditorStyles.miniLabel);
            GUILayout.Label($"  {_manifestPackageName}.wxapi.WXPayEntryActivity", EditorStyles.miniLabel);

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // Reload defaults button
            if (GUILayout.Button("Reload Defaults from AndroidManifest.xml", GUILayout.Height(30)))
            {
                LoadManifestDefaults();
            }

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }

        private void DrawKeystoreSettingsSection()
        {
            EditorGUILayout.BeginVertical(Styles.boxStyle);

            GUILayout.Label("KEYSTORE SETTINGS", Styles.sectionHeaderStyle);
            GUILayout.Space(5);
            DrawSeparator();
            GUILayout.Space(10);

            // Check if keystore file exists
            string fullKeystorePath = Path.Combine(Application.dataPath.Replace("Assets", ""), KEYSTORE_PATH);
            bool keystoreExists = File.Exists(fullKeystorePath);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (keystoreExists)
            {
                EditorGUILayout.HelpBox("Keystore configuration valid", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Keystore file not found at: " + KEYSTORE_PATH, MessageType.Warning);
            }

            GUILayout.Space(5);
            GUILayout.Label($"Path: {KEYSTORE_PATH}", EditorStyles.miniLabel);
            GUILayout.Label($"Alias: {KEYSTORE_ALIAS}", EditorStyles.miniLabel);
            GUILayout.Label($"Password: {KEYSTORE_PASSWORD}", EditorStyles.miniLabel);

            EditorGUILayout.EndVertical();

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }

        private void DrawBuildInfoSection()
        {
            EditorGUILayout.BeginVertical(Styles.boxStyle);

            GUILayout.Label("BUILD INFORMATION", Styles.sectionHeaderStyle);
            GUILayout.Space(5);
            DrawSeparator();
            GUILayout.Space(10);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label($"Package Name: {PlayerSettings.applicationIdentifier}", EditorStyles.miniLabel);
            GUILayout.Label($"Version: {PlayerSettings.bundleVersion}", EditorStyles.miniLabel);
            GUILayout.Label($"Bundle Version Code: {PlayerSettings.Android.bundleVersionCode}", EditorStyles.miniLabel);
            GUILayout.Label($"Output Folder: {OUTPUT_FOLDER}/", EditorStyles.miniLabel);

            if (_estimatedSize > 0)
            {
                GUILayout.Space(5);
                GUILayout.Label($"Last Build Size: {FormatBytes(_estimatedSize)}", EditorStyles.miniLabel);
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }

        private void DrawBuildActionsSection()
        {
            GUILayout.Space(10);
            DrawSeparator();
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Build button
            Color originalBgColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f); // Green

            bool canBuild = !string.IsNullOrEmpty(_apkVersionNumber);
            GUI.enabled = canBuild;

            if (GUILayout.Button("BUILD APK", Styles.buildButtonStyle, GUILayout.Width(200), GUILayout.Height(50)))
            {
                BuildAPK();
            }

            GUI.enabled = true;
            GUI.backgroundColor = originalBgColor;

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(15);
        }

        private void DrawSeparator()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
        }
        #endregion

        #region Build Logic
        private void BuildAPK()
        {
            if (string.IsNullOrEmpty(_apkVersionNumber))
            {
                EditorUtility.DisplayDialog("Error", "Please enter an APK version number.", "OK");
                return;
            }

            // Validate keystore
            string fullKeystorePath = Path.Combine(Application.dataPath.Replace("Assets", ""), KEYSTORE_PATH);
            if (!File.Exists(fullKeystorePath))
            {
                bool proceed = EditorUtility.DisplayDialog(
                    "Keystore Not Found",
                    $"Keystore file not found at: {KEYSTORE_PATH}\n\nDo you want to continue anyway?",
                    "Continue", "Cancel"
                );

                if (!proceed) return;
            }

            // Step 1: Export assets if requested
            if (_exportAssetsBeforeBuild)
            {
                EditorUtility.DisplayProgressBar("Building APK", "Exporting assets...", 0.1f);
                ExportAllAssetsAndSubPack();
            }

            // Step 2: Update AndroidManifest.xml
            EditorUtility.DisplayProgressBar("Building APK", "Updating AndroidManifest.xml...", 0.3f);
            UpdateAndroidManifest();

            // Step 3: Configure keystore settings
            EditorUtility.DisplayProgressBar("Building APK", "Configuring keystore...", 0.4f);
            ConfigureKeystore();

            // Step 4: Build APK
            EditorUtility.DisplayProgressBar("Building APK", "Building APK file...", 0.5f);

            // Create output directory
            string outputDir = Path.Combine(Application.dataPath.Replace("Assets", ""), OUTPUT_FOLDER);
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            string packageName = PlayerSettings.applicationIdentifier;
            string apkName = $"{packageName}-{_apkVersionNumber}.apk";
            string outputPath = Path.Combine(outputDir, apkName);

            // Delete existing file if exists (overwrite)
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            try
            {
                BuildReport report = BuildPipeline.BuildPlayer(
                    GetBuildScenes(),
                    outputPath,
                    BuildTarget.Android,
                    BuildOptions.None
                );

                EditorUtility.ClearProgressBar();

                if (report.summary.result == BuildResult.Succeeded)
                {
                    _lastBuildPath = outputPath;

                    // Get actual size
                    if (File.Exists(outputPath))
                    {
                        FileInfo fileInfo = new FileInfo(outputPath);
                        _estimatedSize = fileInfo.Length;
                    }

                    EditorUtility.DisplayDialog(
                        "Build Successful",
                        $"APK built successfully!\n\nOutput: {OUTPUT_FOLDER}/{apkName}\nSize: {FormatBytes(_estimatedSize)}",
                        "OK"
                    );

                    // Open folder
                    EditorUtility.RevealInFinder(outputPath);
                }
                else
                {
                    string errorMsg = report.summary.result.ToString();
                    EditorUtility.DisplayDialog("Build Failed", $"Build failed with result: {errorMsg}", "OK");
                }
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Build Error", $"An error occurred during build:\n{e.Message}", "OK");
                Debug.LogError($"Build error: {e}");
            }
        }

        private void ExportAllAssetsAndSubPack()
        {
            try
            {
                // Call the Packager method
                Debug.Log("Calling HandleAllAndSubBundle...");
                Packager.HandleAllAndSubBundle();
                Debug.Log("HandleAllAndSubBundle completed");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during HandleAllAndSubBundle: {e.Message}");
                EditorUtility.DisplayDialog("Export Error", $"Error exporting assets:\n{e.Message}", "OK");
            }
        }

        private void UpdateAndroidManifest()
        {
            try
            {
                // Ensure Plugins/Android directory exists
                string pluginsAndroidPath = Path.Combine(Application.dataPath, "Plugins/Android");
                if (!Directory.Exists(pluginsAndroidPath))
                {
                    Directory.CreateDirectory(pluginsAndroidPath);
                }

                string manifestPath = Path.Combine(Application.dataPath.Replace("Assets", ""), MANIFEST_PATH);

                if (!File.Exists(manifestPath))
                {
                    Debug.LogWarning($"AndroidManifest.xml not found at {manifestPath}. Skipping manifest update.");
                    return;
                }

                // Get SDK configuration
                int numberType = NUMBER_TYPE_OPTIONS[_selectedNumberTypeIndex];
                var sdkConfig = GetSDKConfiguration(numberType);

                // Read manifest as text
                string manifestContent = File.ReadAllText(manifestPath);

                // Replace values using string replacement to preserve formatting
                manifestContent = ReplaceManifestValue(manifestContent, "sdkType", sdkConfig.sdkType.ToString());
                manifestContent = ReplaceManifestValue(manifestContent, "sdkChannel", sdkConfig.sdkChannel.ToString());
                manifestContent = ReplaceManifestValue(manifestContent, "turboid", _turboId);
                manifestContent = ReplaceManifestValue(manifestContent, "turboname", _turboName);
                manifestContent = ReplaceManifestValue(manifestContent, "dyid", _dyId);

                // Replace package name
                if (!string.IsNullOrEmpty(_manifestPackageName))
                {
                    manifestContent = Regex.Replace(
                        manifestContent,
                        @"package=""[^""]+""",
                        match => $"package=\"{_manifestPackageName}\""
                    );

                    // Replace wxapi activities
                    manifestContent = Regex.Replace(
                        manifestContent,
                        @"android:name=""[^""]*\.wxapi\.WXEntryActivity""",
                        match => $"android:name=\"{_manifestPackageName}.wxapi.WXEntryActivity\""
                    );

                    manifestContent = Regex.Replace(
                        manifestContent,
                        @"android:name=""[^""]*\.wxapi\.WXPayEntryActivity""",
                        match => $"android:name=\"{_manifestPackageName}.wxapi.WXPayEntryActivity\""
                    );
                }

                // Write back to file
                File.WriteAllText(manifestPath, manifestContent);

                Debug.Log($"AndroidManifest.xml updated successfully");
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error updating AndroidManifest.xml: {e.Message}");
                EditorUtility.DisplayDialog("Manifest Error", $"Error updating manifest:\n{e.Message}", "OK");
            }
        }

        private string ReplaceManifestValue(string content, string metaName, string newValue)
        {
            // Pattern to match: android:name="metaName" android:value="anything"
            string pattern = $@"(android:name=""{metaName}""\s+android:value="")[^""]*("")";
            return Regex.Replace(content, pattern, match =>
            {
                return match.Groups[1].Value + newValue + match.Groups[2].Value;
            });
        }

        private void ConfigureKeystore()
        {
            string fullKeystorePath = Path.Combine(Application.dataPath.Replace("Assets", ""), KEYSTORE_PATH);

            if (File.Exists(fullKeystorePath))
            {
                PlayerSettings.Android.keystoreName = fullKeystorePath;
                PlayerSettings.Android.keystorePass = KEYSTORE_PASSWORD;
                PlayerSettings.Android.keyaliasName = KEYSTORE_ALIAS;
                PlayerSettings.Android.keyaliasPass = KEYSTORE_PASSWORD;

                Debug.Log("Keystore configured successfully");
            }
            else
            {
                Debug.LogWarning($"Keystore file not found at: {fullKeystorePath}");
            }
        }

        private string[] GetBuildScenes()
        {
            List<string> pathList = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    pathList.Add(scene.path);
                }
            }
            return pathList.ToArray();
        }

        private (int sdkType, int sdkChannel) GetSDKConfiguration(int numberType)
        {
            switch (numberType)
            {
                case 1:
                    return (0, 1);
                case 1000:
                    return (1, 1000);
                case 2000:
                    return (2, 2000);
                case 9999:
                    return (0, 9999);
                default:
                    return (0, 1);
            }
        }
        #endregion

        #region Helper Methods
        private void LoadManifestDefaults()
        {
            try
            {
                string manifestPath = Path.Combine(Application.dataPath.Replace("Assets", ""), MANIFEST_PATH);

                if (File.Exists(manifestPath))
                {
                    string manifestContent = File.ReadAllText(manifestPath);

                    // Extract values using regex
                    _turboId = ExtractManifestValue(manifestContent, "turboid");
                    _turboName = ExtractManifestValue(manifestContent, "turboname");
                    _dyId = ExtractManifestValue(manifestContent, "dyid");

                    // Extract package name
                    Match packageMatch = Regex.Match(manifestContent, @"package=""([^""]+)""");
                    if (packageMatch.Success)
                    {
                        _manifestPackageName = packageMatch.Groups[1].Value;
                    }

                    Debug.Log("Loaded manifest defaults successfully");
                }
                else
                {
                    // Use default values
                    _manifestPackageName = PlayerSettings.applicationIdentifier;
                    _turboId = "";
                    _turboName = "";
                    _dyId = "";

                    Debug.LogWarning($"AndroidManifest.xml not found at {manifestPath}. Using default values.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading manifest defaults: {e.Message}");
            }
        }

        private string ExtractManifestValue(string content, string metaName)
        {
            // Pattern to match: android:name="metaName" android:value="value"
            string pattern = $@"android:name=""{metaName}""\s+android:value=""([^""]*)""";
            Match match = Regex.Match(content, pattern);
            return match.Success ? match.Groups[1].Value : "";
        }

        private void LoadPreferences()
        {
            _exportAssetsBeforeBuild = EditorPrefs.GetBool("BuildEditor_ExportAssets", false);
            _selectedNumberTypeIndex = EditorPrefs.GetInt("BuildEditor_NumberTypeIndex", 0);
            _apkVersionNumber = EditorPrefs.GetString("BuildEditor_APKVersionNumber", "");
            _turboId = EditorPrefs.GetString("BuildEditor_TurboId", "");
            _turboName = EditorPrefs.GetString("BuildEditor_TurboName", "");
            _dyId = EditorPrefs.GetString("BuildEditor_DyId", "");
            _manifestPackageName = EditorPrefs.GetString("BuildEditor_ManifestPackage", PlayerSettings.applicationIdentifier);
        }

        private void SavePreferences()
        {
            EditorPrefs.SetBool("BuildEditor_ExportAssets", _exportAssetsBeforeBuild);
            EditorPrefs.SetInt("BuildEditor_NumberTypeIndex", _selectedNumberTypeIndex);
            EditorPrefs.SetString("BuildEditor_APKVersionNumber", _apkVersionNumber);
            EditorPrefs.SetString("BuildEditor_TurboId", _turboId);
            EditorPrefs.SetString("BuildEditor_TurboName", _turboName);
            EditorPrefs.SetString("BuildEditor_DyId", _dyId);
            EditorPrefs.SetString("BuildEditor_ManifestPackage", _manifestPackageName);
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
        #endregion
    }
}

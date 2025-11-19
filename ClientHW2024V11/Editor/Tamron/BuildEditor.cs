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
 *   - Automated AndroidManifest.xml modification
 *   - Keystore configuration for APK signing
 *   - Custom APK naming convention
 *   - Build size estimation
 *
 * Usage:
 *   Unity Editor → Tools → Tamron → Build Editor
 *
 * ============================================================================
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
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

        private static readonly int[] NUMBER_TYPE_OPTIONS = { 1, 1000, 2000, 9999 };
        private static readonly string[] NUMBER_TYPE_LABELS = { "1", "1000", "2000", "9999" };
        #endregion

        #region Fields
        private bool _exportAssetsBeforeBuild = false;
        private int _selectedNumberTypeIndex = 0;
        private string _apkVersionNumber = "";
        private Vector2 _scrollPosition = Vector2.zero;

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
            }
        }
        #endregion

        #region Menu Item
        [MenuItem("Tools/Tamron/Build Editor", false)]
        static void OpenWindow()
        {
            var window = GetWindow<BuildEditor>(true, "Build Editor");
            window.minSize = new Vector2(600, 550);
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
        }

        private void OnGUI()
        {
            Styles.Initialize();

            DrawHeader();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            DrawPreBuildOptionsSection();
            DrawBuildSettingsSection();
            DrawKeystoreSettingsSection();
            DrawBuildInfoSection();

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
            EditorGUILayout.EndVertical();

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

            if (_estimatedSize > 0)
            {
                GUILayout.Space(5);
                GUILayout.Label($"Estimated Size: {FormatBytes(_estimatedSize)}", EditorStyles.miniLabel);
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

            string packageName = PlayerSettings.applicationIdentifier;
            string apkName = $"{packageName}-{_apkVersionNumber}.apk";
            string outputPath = Path.Combine(Application.dataPath.Replace("Assets", ""), apkName);

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

                    // Estimate size
                    if (File.Exists(outputPath))
                    {
                        FileInfo fileInfo = new FileInfo(outputPath);
                        _estimatedSize = fileInfo.Length;
                    }

                    EditorUtility.DisplayDialog(
                        "Build Successful",
                        $"APK built successfully!\n\nOutput: {apkName}\nSize: {FormatBytes(_estimatedSize)}",
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

                // Get SDK configuration
                int numberType = NUMBER_TYPE_OPTIONS[_selectedNumberTypeIndex];
                var sdkConfig = GetSDKConfiguration(numberType);

                XmlDocument doc = new XmlDocument();

                // Create or load manifest
                if (File.Exists(manifestPath))
                {
                    doc.Load(manifestPath);
                }
                else
                {
                    // Create basic manifest structure
                    CreateBasicManifest(doc);
                }

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("android", "http://schemas.android.com/apk/res/android");

                // Get or create application node
                XmlNode applicationNode = doc.SelectSingleNode("//application");
                if (applicationNode == null)
                {
                    XmlNode manifestNode = doc.SelectSingleNode("//manifest");
                    applicationNode = doc.CreateElement("application");
                    manifestNode.AppendChild(applicationNode);
                }

                // Update or create meta-data for sdkType
                UpdateOrCreateMetaData(doc, applicationNode, "sdkType", sdkConfig.sdkType.ToString());

                // Update or create meta-data for sdkChannel
                UpdateOrCreateMetaData(doc, applicationNode, "sdkChannel", sdkConfig.sdkChannel.ToString());

                // Save the document
                doc.Save(manifestPath);

                Debug.Log($"AndroidManifest.xml updated: sdkType={sdkConfig.sdkType}, sdkChannel={sdkConfig.sdkChannel}");
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error updating AndroidManifest.xml: {e.Message}");
                EditorUtility.DisplayDialog("Manifest Error", $"Error updating manifest:\n{e.Message}", "OK");
            }
        }

        private void CreateBasicManifest(XmlDocument doc)
        {
            XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(declaration);

            XmlElement manifest = doc.CreateElement("manifest");
            manifest.SetAttribute("xmlns:android", "http://schemas.android.com/apk/res/android");
            manifest.SetAttribute("package", PlayerSettings.applicationIdentifier);
            doc.AppendChild(manifest);

            XmlElement application = doc.CreateElement("application");
            manifest.AppendChild(application);
        }

        private void UpdateOrCreateMetaData(XmlDocument doc, XmlNode applicationNode, string metaName, string metaValue)
        {
            // Find existing meta-data node
            XmlNode metaDataNode = null;
            foreach (XmlNode child in applicationNode.ChildNodes)
            {
                if (child.Name == "meta-data")
                {
                    XmlAttribute nameAttr = child.Attributes["android:name"];
                    if (nameAttr != null && nameAttr.Value == metaName)
                    {
                        metaDataNode = child;
                        break;
                    }
                }
            }

            if (metaDataNode == null)
            {
                // Create new meta-data node
                metaDataNode = doc.CreateElement("meta-data");
                XmlAttribute nameAttr = doc.CreateAttribute("android", "name", "http://schemas.android.com/apk/res/android");
                nameAttr.Value = metaName;
                metaDataNode.Attributes.Append(nameAttr);
                applicationNode.AppendChild(metaDataNode);
            }

            // Update or create value attribute
            XmlAttribute valueAttr = metaDataNode.Attributes["android:value"];
            if (valueAttr == null)
            {
                valueAttr = doc.CreateAttribute("android", "value", "http://schemas.android.com/apk/res/android");
                metaDataNode.Attributes.Append(valueAttr);
            }
            valueAttr.Value = metaValue;
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
        private void LoadPreferences()
        {
            _exportAssetsBeforeBuild = EditorPrefs.GetBool("BuildEditor_ExportAssets", false);
            _selectedNumberTypeIndex = EditorPrefs.GetInt("BuildEditor_NumberTypeIndex", 0);
            _apkVersionNumber = EditorPrefs.GetString("BuildEditor_APKVersionNumber", "");
        }

        private void SavePreferences()
        {
            EditorPrefs.SetBool("BuildEditor_ExportAssets", _exportAssetsBeforeBuild);
            EditorPrefs.SetInt("BuildEditor_NumberTypeIndex", _selectedNumberTypeIndex);
            EditorPrefs.SetString("BuildEditor_APKVersionNumber", _apkVersionNumber);
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

/*
 * Copyright (c) 2015-2020 Beebyte Limited. All rights reserved.
 */
#if !BEEBYTE_OBFUSCATOR_DISABLE
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Beebyte.Obfuscator.Assembly;
using UnityEditor;
using UnityEngine;

namespace Beebyte.Obfuscator
{
	/**
	 * Handles obfuscation calls for a Unity project and controls restoration of backed up files.
	 */
	public class Project
	{
		private Options _options;

		private bool _monoBehaviourAssetsNeedReverting = false;
		private bool _hasError;
		private bool _hasObfuscated;
		private bool _noCSharpScripts;

		public bool ShouldObfuscate()
		{
			if (_options == null) _options = OptionsManager.LoadOptions();
			return _options.enabled && (_options.obfuscateReleaseOnly == false || Debug.isDebugBuild == false);
		}

		public bool IsSuccess()
		{
			return _hasObfuscated || !ShouldObfuscate();
		}

		public bool HasCSharpScripts()
		{
			return !_noCSharpScripts;
		}

		public bool HasMonoBehaviourAssetsThatNeedReverting()
		{
			return _monoBehaviourAssetsNeedReverting;
		}

		public void ObfuscateIfNeeded()
		{
#if UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			if (!EditorApplication.isPlayingOrWillChangePlaymode && !_hasObfuscated && _hasError == false)
#else
			if (!EditorApplication.isPlayingOrWillChangePlaymode && !_hasObfuscated && _hasError == false && BuildPipeline.isBuildingPlayer)
#endif
			{
#if !UNITY_5_6_OR_NEWER
				EditorApplication.update += PipelineHook.ClearProjectViaUpdate;
#endif
				try
				{
					EditorApplication.LockReloadAssemblies();
					ObfuscateWhileLocked();
				}
				catch (Exception e)
				{
					Debug.LogError("Obfuscation Failed: " + e);
					_hasError = true;
					throw new OperationCanceledException("Obfuscation failed", e);
				}
				finally
				{
					EditorApplication.UnlockReloadAssemblies();
				}
			}
		}

		private void ObfuscateWhileLocked()
		{
			if (_options == null) _options = OptionsManager.LoadOptions();

			if (ShouldObfuscate() == false) return;

			AssemblySelector selector = new AssemblySelector(_options);

			ICollection<string> compiledDlls = selector.GetCompiledAssemblyPaths();

			if (compiledDlls.Count > 0)
			{
				EditorApplication.update += RestoreUtils.RestoreOriginalDlls;
			}

			IDictionary<string, string> backupMap = FileBackup.GetBackupMap(compiledDlls);
			FileBackup.Backup(backupMap);

			ICollection<string> dlls = selector.GetAssemblyPaths();

			if (dlls.Count == 0 && compiledDlls.Count == 0)
			{
				_noCSharpScripts = true;
				return;
			}

#if UNITY_2017_3_OR_NEWER
			Obfuscator.AppendReferenceAssemblies(AssemblyReferenceLocator.GetAssemblyReferenceDirectories().ToArray());
#endif

#if UNITY_2018_2_OR_NEWER
			Obfuscator.ObfuscateMonoBehavioursByAssetDatabase(false);
			var obfuscateMonoBehaviourNames = _options.obfuscateMonoBehaviourClassNames;
			try
			{
				if (IsXCodeProject() && _options.obfuscateMonoBehaviourClassNames)
				{
					Debug.LogWarning("MonoBehaviour class names will not be obfuscated when creating Xcode projects");
					_options.obfuscateMonoBehaviourClassNames = false;
				}
#endif

				Obfuscator.Obfuscate(dlls, compiledDlls, _options, EditorUserBuildSettings.activeBuildTarget);


#if !UNITY_2018_2_OR_NEWER
			if (_options.obfuscateMonoBehaviourClassNames)
			{
				/*
				 * RestoreAssets must be registered via the update delegate because [PostProcessBuild] is not guaranteed to be called
				 */
				EditorApplication.update += RestoreUtils.RestoreMonobehaviourSourceFiles;
				_monoBehaviourAssetsNeedReverting = true;
			}
#else
			}
			finally
			{
				var parent = Application.dataPath + "/../";

				var filePath = parent + "Library/PlayerScriptAssemblies/";

				var pathArr1 = new string[1] { "HotUpdate.dll" };
				var pathArr = new string[3] { "mscorlib.dll", "System.Core.dll", "System.dll" };
				var filePath1 = parent + "HybridCLRData/AssembliesPostIl2CppStrip/Android/";
				var outPath = parent + "aaaaaaaaa/HotUpdate.dll";
				var outPath1 = parent + "aaaaaaaaa/";
				var outPath2 = Application.streamingAssetsPath + "/Dll/";
				for (int i = 0; i < pathArr1.Length; i++)
				{
					File.Copy(filePath + pathArr1[i], outPath1 + pathArr1[i], true);
				}

				//File.Copy(filePath, outPath, true);
				for (int i = 0; i < pathArr.Length; i++)
				{
					File.Copy(filePath1 + pathArr[i], outPath1 + pathArr[i], true);
				}
				if (Directory.Exists(parent + "aaaaaaaaa/out"))
				{
					Directory.Delete(parent + "aaaaaaaaa/out", true);
				}
				Directory.CreateDirectory(parent + "aaaaaaaaa/out");

				Packager.ClearAllAssets();
				Packager.CopyAssetToSteamingAsset();
				for (int i = 0; i < pathArr1.Length; i++)
				{
					var hotByte11 = File.ReadAllBytes(outPath1 + pathArr1[i]);
					var newHotByte11 = BinaryEncryptHelper.EncryptFile(hotByte11);
					File.WriteAllBytes(parent + "aaaaaaaaa/out/" + pathArr1[i] + ".bytes", newHotByte11);
					File.Copy(parent + "aaaaaaaaa/out/" + pathArr1[i] + ".bytes", outPath2 + pathArr1[i] + ".bytes", true);
				}


				for (int i = 0; i < pathArr.Length; i++)
				{

					var hotByte1 = File.ReadAllBytes(outPath1 + pathArr[i]);
					var newHotByte1 = BinaryEncryptHelper.EncryptFile(hotByte1);
					File.WriteAllBytes(parent + "aaaaaaaaa/out/" + pathArr[i] + ".bytes", newHotByte1);
					File.Copy(parent + "aaaaaaaaa/out/" + pathArr[i] + ".bytes", outPath2 + pathArr[i] + ".bytes", true);
				}
				Packager.WriteFiles();
				Packager.CopySteamingAssetToAsset();
				//Packager.ObscureAsset();
				var dirArr = Directory.GetDirectories(Application.streamingAssetsPath + "/" + AppConst.SubPackName);
				for (int i = 0; i < dirArr.Length; i++)
				{
					var path = Path.GetFileName(dirArr[i]);
					if (!AppConst.SubPackArr.Contains(path))
					{
						Directory.Delete(dirArr[i], true);
					}
					if (File.Exists(dirArr[i] + ".meta"))
						File.Delete(dirArr[i] + ".meta");
				}
				_options.obfuscateMonoBehaviourClassNames = obfuscateMonoBehaviourNames;
			}
#endif
			_hasObfuscated = true;
		}

#if UNITY_2018_2_OR_NEWER
		private bool IsXCodeProject()
		{
			return EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX &&
				   EditorUserBuildSettings.GetPlatformSettings("OSXUniversal", "CreateXcodeProject").Equals("true");
		}
#endif

		public void ObfuscateAssets(BuildTarget buildTarget, string pathToBuildProject)
		{
#if UNITY_2018_2_OR_NEWER
			if (IsXCodeProject()) return;
			if (_options == null) _options = OptionsManager.LoadOptions();
			if (_options.obfuscateMonoBehaviourClassNames && File.Exists("_AssetTranslations"))
			{
				string pathToGlobalGameManagersAsset = GlobalGameManagersPath.GetPathToGlobalGameManagersAsset(buildTarget, pathToBuildProject);
				Obfuscator.RenameScriptableAssets("_AssetTranslations", pathToGlobalGameManagersAsset);
			}
#endif
		}
	}
}
#endif

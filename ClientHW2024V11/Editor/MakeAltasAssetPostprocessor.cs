/**
* @file     : AssetPostprocessor.cs
* @brief    : 
* @details  : 自动设置pack tag 
* @author   : 
* @date     : 2017-6-12
*/

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
 
public class  Post : AssetPostprocessor 
{
 
	void OnPostprocessTexture (Texture2D texture) 
	{
//         string preFix = "Assets/Atlas";
//         if (!assetPath.Contains(preFix)) return;
// 
// 		string AtlasName =  new DirectoryInfo(Path.GetDirectoryName(assetPath)).Name;
//         if (AtlasName.Equals("bigBG")) return;
// 		TextureImporter textureImporter  = assetImporter as TextureImporter;
// 		textureImporter.textureType = TextureImporterType.Sprite;
// 		textureImporter.spritePackingTag = AtlasName;
// 		textureImporter.mipmapEnabled = false;
	}
 
}
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using SEZSJ;

namespace XuXiang.EditorTools
{
    /// <summary>
    /// 获取文件路径。
    /// </summary>
	public class GetFilePath
	{
        /// <summary>
        /// 复制资源路径到剪切板。
        /// </summary>
        [MenuItem("Assets/Copy Resource Path" , priority = 3)]
		public static void CopyResourcePath()
		{
            UnityEngine.Object obj = Selection.activeObject;
            if (obj != null)
            {
                string assetpath = AssetDatabase.GetAssetPath(obj);
                int sindex = assetpath.IndexOf("Resources/");
                int eindex = assetpath.LastIndexOf('.');
                if (sindex >= 0 && eindex >= 0)
                {
                    int subindex = sindex + 10;     //子串起始索引 10为"Resources/"的长度
                    string respath = assetpath.Substring(subindex, eindex - subindex);
                    EditorGUIUtility.systemCopyBuffer = respath;
                    LogMgr.Log(string.Format("Resource path is {0}", respath));
                }
                else
                {
                    LogMgr.LogError(string.Format("Error resource path {0}", assetpath));
                }
            }
            else
            {
                LogMgr.LogError("This is not a resource file!");
            }
		}
	}
}


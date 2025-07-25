using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEditor;

namespace XuXiang.EditorTools
{
    /// <summary>
    /// 图像字体导入。
    /// 使用方法:
    ///     1、用BMFont编辑文字并导出fnt(xml格式)和png文件。
    ///     2、将png文件重命名与fnt文件名相同(可不用修改fnt中的引用名称，用不着)，添加到Asset中。
    ///     3、在Unity编辑器Asset中选中fnt文件，择菜单"Tools->Generate Bitmap Font"项进行生成。
    ///     4、同目录下会多出同名的.fontsettings(自定义字体)和.mat(字体所用的材质)文件。
    ///     5、fnt文件就可以删掉了。
    ///     6、使用时除了将Text的Font属性设置为.fontsettings文件外，还要将Material属性设置为.mat，否则文本将是纯色的。
    /// </summary>
    public static class BitmapFontImporter
    {
        [MenuItem("Tools/Generate Bitmap Font")]
        public static void GenerateFont()
        {
            //判断选择的资源
            TextAsset selected = (TextAsset)Selection.activeObject;
            if (selected == null)
            {
                Debug.unityLogger.LogWarning("BitmapFontImporter", "Did not select the FNT file!");
                return;
            }

            //判断对应纹理
            string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(selected));
            string texfile = rootPath + "/" + selected.name + ".png";
            Texture2D texture = AssetDatabase.LoadAssetAtPath(texfile, typeof(Texture2D)) as Texture2D;
            if (!texture)
            {
                Debug.unityLogger.LogWarning("BitmapFontImporter", "Texture2d asset doesn't exist for " + selected.name);
                return;
            }
                
            //加载配置
            string exportPath = rootPath + "/" + Path.GetFileNameWithoutExtension(selected.name);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(selected.text);

            //读取字符信息
            float texW = texture.width;
            float texH = texture.height;
            XmlNodeList chars = xml.GetElementsByTagName("chars")[0].ChildNodes;
            CharacterInfo[] charInfos = new CharacterInfo[chars.Count];
            for (int i = 0; i < chars.Count; i++)
            {
                XmlNode charNode = chars[i];
                if (charNode.Attributes != null)
                {
                    //读取一个字符信息
                    CharacterInfo charInfo = new CharacterInfo();
                    charInfo.index = (int)ToInt(charNode, "id");
                    charInfo.advance = (int)ToInt(charNode, "xadvance");

                    //图像位置定义
                    int w = ToInt(charNode, "width");
                    int h = ToInt(charNode, "height");
                    int xo = ToInt(charNode, "xoffset");
                    int mh = h / 2;             //以图像中线作为对齐点
                    charInfo.minX = xo;
                    charInfo.minY = mh;
                    charInfo.maxX = xo + w;
                    charInfo.maxY = -mh;

                    //纹理坐标定义
                    float tx = ((float)ToInt(charNode, "x")) / texW;
                    float ty = 1 - ((float)ToInt(charNode, "y")) / texH;
                    float tw = (float)w / texW;
                    float th = -(float)h / texH;
                    charInfo.uvBottomLeft = new Vector2(tx, ty);
                    charInfo.uvBottomRight = new Vector2(tx + tw, ty);
                    charInfo.uvTopLeft = new Vector2(tx, ty + th);
                    charInfo.uvTopRight = new Vector2(tx + tw, ty + th);
                    charInfos[i] = charInfo;
                }
            }

            //创建材质
            string matfile = exportPath + ".mat";
            Material material = null;
            if (!File.Exists(matfile))
            {
                Shader shader = Shader.Find("UI/Default");
                material = new Material(shader);
                material.mainTexture = texture;
                AssetDatabase.CreateAsset(material, matfile);
            }
            else
            {
                material = AssetDatabase.LoadAssetAtPath<Material>(matfile);
            }

            //创建字体
            XmlNode info = xml.GetElementsByTagName("info")[0];
            string fontfile = exportPath + ".fontsettings";
            if (!File.Exists(fontfile))
            {
                Font font = new Font();
                font.name = info.Attributes.GetNamedItem("face").InnerText;
                font.material = material;
                font.characterInfo = charInfos;
                AssetDatabase.CreateAsset(font, fontfile);
            }
            else
            {
                Font font = AssetDatabase.LoadMainAssetAtPath(fontfile) as Font;
                font.name = info.Attributes.GetNamedItem("face").InnerText;
                font.characterInfo = charInfos;
                AssetDatabase.SaveAssets();
            }

            //string fontfile = exportPath + ".fontsettings";
            //if (!File.Exists(fontfile))
            //{
            //    Font font = new Font();
            //    font.material = material;
            //    font.characterInfo = charInfos;
            //    AssetDatabase.CreateAsset(font, fontfile);
            //}
            //else
            //{
            //    Font font = new Font();
            //    font.material = material;
            //    font.characterInfo = charInfos;

            //    Font oldfont = AssetDatabase.LoadAssetAtPath<Font>(fontfile);
            //    AssetDatabase.AddObjectToAsset(font, oldfont);
            //    AssetDatabase.SaveAssets();
            //}
        }

        /// <summary>
        /// 从XML节点中读取整数属性值。
        /// </summary>
        /// <param name="node">XML节点。</param>
        /// <param name="name">属性的名称。</param>
        /// <returns>属性值。</returns>
        private static int ToInt(XmlNode node, string name)
        {
            return int.Parse(node.Attributes.GetNamedItem(name).InnerText);
        }
    }
}

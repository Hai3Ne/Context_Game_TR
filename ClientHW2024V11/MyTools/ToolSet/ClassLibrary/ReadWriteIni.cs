using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace XuXiang.ClassLibrary
{
    #region 说明=========================================================================================

    //INI文件是一种按照特点方式排列的文本文件。
    //    每一个INI文件构成都非常类似，由若干段落（section）组成，在每个带括号的标题下面，是若干个以单个单词
    //开头的关键词（keyword）和一个等号，等号右边的就是关键字对应的值（value）。
    //
    //[Section1]
    //KeyWord1 = Valuel
    //KeyWord2 = Value2
    //……
    //[Section2]
    //KeyWord3 = Value3
    //KeyWord4 = Value4
    //
    //
    //INI文件主要用于保存配置信息，方便值的读写，通常情况下不会通过程序对Section和Key进行添加和删除操作。
    //对于INI文件常用操作有三个：
    //    1、从指定的Section中指定的Key读入一个整数。
    //    2、从指定的Section中指定的Key读入一个字符串。
    //    3、将一个字符串写入到指定的Section的指定的Key中。
    //
    //    没有写入整数的操作，因为写入如果要整数时可以把整数ToString()再写入。字符串写入函数可以通过指定特殊的
    //参数来对INI文件的Section和Value进行添加和删除操作。
    //
    //
    //还有两个不常用操作，可用在某些初始化和错误处理上。
    //    1、获取指定小节所有项名和值的一个列表。(Not test)
    //    2、获取指定INI文件的Section名字列表。(Not test)
    //    3、初始化INI文件中指定的小节设置所有项名和值。
    //
    //
    //将以下静态类拷贝到工程源代码中，即可以 类名.方法名的方式对INI文件进行操作。

    #endregion

    /// <summary>
    /// 静态类。提供一组读写Ini文件的方法。
    /// </summary>
    public static class ReadWriteIni
    {
        /// <summary>
        /// 从ini文件指定的Section中指定的Key读入一个整数。
        /// </summary>
        /// <param name="strSectionName">Section名字。不区分大小写。</param>
        /// <param name="strKeyName">Key名字。不区分大小写。</param>
        /// <param name="iDefault">默认值，若读取失败则返回该值。</param>
        /// <param name="strFileName">文件名。如果没有指定完整的绝对路径名，则在Windows目录中搜索文件。</param>
        /// <returns>若成功找到Section和Key则返回其值，否则返回默认值</returns>
        [DllImport("kernel32.dll")]
        public static extern Int32 GetPrivateProfileInt(String strSectionName, String strKeyName, int iDefault, String strFileName);

        /// <summary>
        /// 从ini文件指定的Section中指定的Key读入一个字符串。
        /// </summary>
        /// <param name="strSectionName">Section名字。不区分大小写。</param>
        /// <param name="strKeyName">Key名字。不区分大小写。</param>
        /// <param name="strDefault">默认值，若读取失败则返回该值。可设为空("")。</param>
        /// <param name="sbReturnedString">指定一个字串缓冲区，长度至少为iSize。</param>
        /// <param name="iSize">指定装载到sbReturnedString缓冲区的最大字符数量。</param>
        /// <param name="strFileName">文件名。如果没有指定完整的绝对路径名，则在Windows目录中搜索文件。</param>
        /// <returns>复制到sbReturnedString缓冲区的字节数量，其中不包括那些NULL中止字符。</returns>
        [DllImport("kernel32.dll")]
        public static extern Int32 GetPrivateProfileString(String strSectionName, String strKeyName, String strDefault, StringBuilder sbReturnedString, Int32 iSize, String strFileName);

        /// <summary>
        /// 将字符串写入到ini文件指定的Section中指定的Key。
        /// </summary>
        /// <param name="strSectionName">Section名字。不区分大小写。如果Section不存在，则创建。</param>
        /// <param name="strKeyName">Key名字。不区分大小写。如果该Key不存在，则创建。如果null则删除该Section</param>
        /// <param name="strValue">要写入的值。如果为null，则表示删除KeyName指定的Key</param>
        /// <param name="strFileName">文件名。如果没有指定完整的绝对路径名，则在Windows目录中搜索文件。</param>
        /// <returns>返回是否成功执行该函数。</returns>
        [DllImport("kernel32.dll")]
        public static extern Boolean WritePrivateProfileString(String strSectionName, String strKeyName, String strValue, String strFileName);

        /// <summary>
        /// 获取指定Section键值对。
        /// </summary>
        /// <param name="strSectionName">Section名字。不区分大小写。</param>
        /// <param name="sbReturnedString">指定一个字串缓冲区，长度至少为iSize。</param>
        /// <param name="iSize">指定装载到sbReturnedString缓冲区的最大字符数量。</param>
        /// <param name="strFileName">文件名。如果没有指定完整的绝对路径名，则在Windows目录中搜索文件。</param>
        /// <returns>复制到sbReturnedString缓冲区的字节数量，其中不包括那些NULL中止字符。</returns>
        [DllImport("kernel32.dll")]
        public static extern Int32 GetPrivateProfileSection(String strSectionName, StringBuilder sbReturnedString, Int32 iSize, String strFileName);

        /// <summary>
        /// 返回INI文件的Section名字列表。
        /// </summary>
        /// <param name="strReturnedString"></param>
        /// <param name="iSize"></param>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern Int32 GetPrivateProfileSectionNames(StringBuilder strReturnedString, Int32 iSize, String strFileName);

        /// <summary>
        /// 为INI文件中指定的Section设置键和值。原来的键和值都被删除。
        /// </summary>
        /// <param name="strSectionName">Section名字。不区分大小写。如果Section不存在，则创建。</param>
        /// <param name="strString">键值对。最多65,535 个字节。</param>
        /// <param name="strFileName">文件名。如果没有指定完整的绝对路径名，则在Windows目录中搜索文件。</param>
        /// <returns>返回是否成功设置。</returns>
        [DllImport("kernel32.dll")]
        public static extern Boolean WritePrivateProfileSection(String strSectionName, String strString, String strFileName);

    }
}

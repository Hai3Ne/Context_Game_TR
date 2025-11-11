using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

    class Utils
    {
        /**
	    * 方法名称: csvHandlerStr</br>
	    * 方法描述: 处理包含逗号，或者双引号的字段</br>
	    * 方法参数: @param forecastName
	    * 方法参数: @return  </br>
	    * 返回类型: String</br>
	    * 抛出异常:</br>
	    */
        public static string csvHandlerStr(string str)
        {
            //csv格式如果有逗号，整体用双引号括起来；如果里面还有双引号就替换成两个双引号，这样导出来的格式就不会有问题了	
            //如果有逗号
            //str = str.Replace("\r\n", "\n").Replace("\\n", "\n");//替换字符中的\n
            string tempDescription = str;
            if (str.IndexOfAny(",\n\"".ToCharArray()) >= 0)
            {
                //如果还有双引号，先将双引号转义，避免两边加了双引号后转义错误
                tempDescription = str.Replace("\"", "\"\"");
                    //在将逗号转义
                    tempDescription = "\"" + tempDescription + "\"";
            }
        return tempDescription;
    }

    #region Log
    const string logFile = @"log.txt";
        static List<string> listLog = new List<string>();
        public static void Log(string s, bool bSave = false, ConsoleColor color = ConsoleColor.White)
        {
            Console.WriteLine(s);
            if (bSave) listLog.Add("mylog: " + s);
        }

        public static void LogWarning(string s)
        {
            listLog.Add("mywarning: " + s);
        ExcelExporter.ConfigArchive.Instance.MyLog(s);
            //Console.ForegroundColor = ConsoleColor.DarkYellow;
            //Console.WriteLine(s);
            //Console.ForegroundColor = ConsoleColor.White;
    }
        public static void LogError(string s)
        {
            listLog.Add("myerror: " + s);
        ExcelExporter.ConfigArchive.Instance.MyLog(s);
        //Console.ForegroundColor = ConsoleColor.Red;
        //Console.WriteLine(s);
        //Console.ForegroundColor = ConsoleColor.White;
    }

    public static void SaveLog(string strLogPath)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in listLog)
            {
                sb.AppendLine(item);
            }

            //-- 保存文件
            using (FileStream file = new FileStream(Path.Combine(strLogPath, logFile), FileMode.Create, FileAccess.Write))
            {
                using (TextWriter writer = new StreamWriter(file, Encoding.Default))
                    writer.Write(sb.ToString());
            }
        }
        #endregion
    }

using UnityEngine;
using System.Collections;


namespace SEZSJ
{


    public class PathUtil
    {

        public  static string GetDirectoryPath(string path)
        {
            char[] cc = { '/', '\\' };

            int nIdx = path.LastIndexOfAny(cc);
            if (nIdx != -1)
                path = path.Substring(0, nIdx);

            return path;
        }

        public static string GetDirectoryName(string path)
        {
            char[] cc = { '/', '\\' };

            int nIdx = path.LastIndexOfAny(cc);
            if (nIdx != -1)
                path = path.Substring( nIdx+1);

            return path;
        }


        public static string GetFilePathWithOutExt(string path)
        {
            int index = path.LastIndexOf('.');

            return path.Substring(0, index);
        }





    }




}
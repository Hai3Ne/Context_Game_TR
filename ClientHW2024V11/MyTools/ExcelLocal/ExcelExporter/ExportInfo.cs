using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace ExcelExporter
{
    /// <summary>
    /// 导出类型。
    /// </summary>
    public enum ExportType
    {
        Unknow,

        CSV,

        Lua,
    }

    /// <summary>
    /// 导出目标。
    /// </summary>
    public enum ExportTarget
    {
        Unknow,

        Client,

        Server,

        All,
    }

    /// <summary>
    /// 导出信息。
    /// </summary>
    public class ExportInfo
    {
        /// <summary>
        /// 显示名称。
        /// </summary>
        public string Name;

        /// <summary>
        /// 解析导出类型。
        /// </summary>
        /// <param name="str">导出类型配置字符串。</param>
        /// <returns>导出类型。</returns>
        public ExportType ParseExportType(string str)
        {
            string l = str.ToLower();
            if (l.CompareTo("csv") == 0)
            {
                return ExportType.CSV;
            }
            if (l.CompareTo("lua") == 0)
            {
                return ExportType.Lua;
            }
            return ExportType.Unknow;
        }

        /// <summary>
        /// 获取单元格的值。
        /// </summary>
        /// <param name="cell">单元格。</param>
        /// <returns>字符串值。</returns>
        public static string GetCellValue(CellValue cell)
        {
            CellType ct = cell.CellType;
            string v = string.Empty;
            switch (ct)
            {
                case CellType.Boolean:
                    v = cell.BooleanValue ? "1" : "0";
                    break;
                case CellType.Numeric:
                    v = cell.NumberValue.ToString();
                    break;
                case CellType.String:
                    v = cell.StringValue;
                    break;
                default:
                    break;
            }
            return v;
        }

        /// <summary>
        /// 获取单元格的值。
        /// </summary>
        /// <param name="cell">单元格。</param>
        /// <returns>字符串值。</returns>
        public static string GetCellValue(ICell cell)
        {
            CellType ct = cell.CellType;
            string v = string.Empty;
            switch (ct)
            {
                case CellType.Boolean:
                    v = cell.BooleanCellValue ? "1" : "0";
                    break;
                case CellType.Numeric:
                    v = cell.NumericCellValue.ToString();
                    break;
                case CellType.String:
                    v = cell.StringCellValue;
                    break;
                default:
                    break;
            }
            return v;
        }
    }
}

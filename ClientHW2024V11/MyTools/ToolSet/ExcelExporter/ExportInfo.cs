using System;
using System.Collections.Generic;
//using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Reflection;
//using NPOI.XSSF.UserModel;
//using NPOI.SS.UserModel;
using ExcelDataReader;

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
        /// 开始导出。
        /// </summary>
        /// <returns>是否导出成功。</returns>
        //public bool StartExport()
        //{
        //    MainForm form = MainForm.CurForm;
        //    string file = string.Format("{0}\\{1}.xlsx", ConfigArchive.Instance.SourceFolder, Name);
        //    bool ret = true;            
        //    try
        //    {
        //        Excel.Application eapp = ConfigArchive.Instance.ExcelApp;
        //        object mv = Missing.Value;
        //        form.Log("打开Excel({0})... Native", Name);
        //        Excel.Workbook wb = ConfigArchive.Instance.ExcelApp.Workbooks.Open(file, true, true, mv, mv, mv, mv, mv, mv, mv, mv, mv, mv, mv, mv);

        //        //找到要导出的表
        //        List<ExportData> exportdata = new List<ExportData>();
        //        int count = wb.Worksheets.Count;
        //        for (int j = 1; j <= count; ++j)
        //        {
        //            //工作表内容
        //            Excel.Worksheet ws = wb.Worksheets[j] as Excel.Worksheet;
        //            if (ws.Name.StartsWith("t_"))
        //            {
        //                object[,] data = (object[,])ws.UsedRange.Cells.Value2;
        //                exportdata.Add(new ExportData(ws.Name, data, Name));
        //            }
        //        }
        //        ConfigArchive.Instance.AddExportData(exportdata);

        //        wb.Close(false);
        //        wb = null;
        //    }
        //    catch (Exception e)
        //    {
        //        form.Log("{0}\n{1}", e.Message, e.StackTrace);
        //        ret = false;
        //    }

        //    return ret;
        //}


        /// <summary>
        /// 开始导出。
        /// </summary>
        /// <returns>是否导出成功。</returns>
        public bool StartExportNPOI()
        {
            bool ret = true;
            try
            {
                string file = string.Format("{0}\\{1}.xlsx", ConfigArchive.Instance.SourceFolder, Name);
                MainForm.CurForm.Log("打开Excel({0})... NPOI", Name);
                //string temp = string.Format("excel_temp\\{1}.tmp.xlsx", ConfigArchive.Instance.SourceFolder, Name);
                //File.Copy(file, temp, true);
                using (var stream = File.Open(file, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var dateset = reader.AsDataSet();
                        List<ExportData> exportdata = GetExportData(dateset, Name);
                        ConfigArchive.Instance.AddExportData(exportdata);
                    }
                }
            }
            catch (Exception e)
            {
                MainForm.CurForm.Log("{0}\n{1}", e.Message, e.StackTrace);
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// 获取导出数据。
        /// </summary>
        /// <param name="wb">NPOI加载的Excel表格列表。</param>
        /// <returns>导出数据列表。</returns>
        public static List<ExportData> GetExportData(System.Data.DataSet wb, string fileName)
        {
            List<ExportData> exportdata = new List<ExportData>();
            for (int i = 0; i < wb.Tables.Count; ++i)
            {
                var sheet = wb.Tables[i];
                if (sheet.TableName.StartsWith("t_"))
                {
                    var firstRow = sheet.Rows[0];
                    int cellCount = firstRow.ItemArray.Length; //一行最后一个cell的编号 即总的列数
                    int rowCount = sheet.Rows.Count;
                    string[,] data = new string[rowCount, cellCount];
                    bool bEnd = false;
                    for (int j = 0; j < rowCount; ++j)
                    {
                        //没有数据的行默认是null
                        var row = sheet.Rows[j];
                        if (row == null)
                        {
                            if (j < ExportData.DATA_START_ROW) { continue; }else { break; }
                        }

                        //读取每一个单元格
                        for (int k = 0; k < cellCount; ++k)
                        {
                            //同理，没有数据的单元格都默认是null
                            //var cell = row[k];
                            string val = string.Empty;
                            if (k < row.ItemArray.Length)
                            {
                                val = row[k].ToString();
                            }
                            if (string.IsNullOrEmpty(val) && k == 0 && j >= ExportData.DATA_START_ROW)
                            {
                                bEnd = true;
                                break;
                            }
                            data[j, k] = val;
                            //data[j, k] = cell.CellType == CellType.Formula ? GetCellValue(e.Evaluate(cell)) : GetCellValue(cell);
                        }
                        if (bEnd)
                        {
                            MainForm.CurForm.Log("结束，表：{0}，行：{1}" , sheet.TableName, j + 1);
                            break;
                        }
                    }
                    exportdata.Add(new ExportData(sheet.TableName, data, fileName));
                }
            }
            return exportdata;
        }

        /// <summary>
        /// 获取单元格的值。
        /// </summary>
        /// <param name="cell">单元格。</param>
        /// <returns>字符串值。</returns>
        //public static string GetCellValue(CellValue cell)
        //{
        //    CellType ct = cell.CellType;
        //    string v = string.Empty;
        //    switch (ct)
        //    {
        //        case CellType.Boolean:
        //            v = cell.BooleanValue ? "1" : "0";
        //            break;
        //        case CellType.Numeric:
        //            v = cell.NumberValue.ToString();
        //            break;
        //        case CellType.String:
        //            v = cell.StringValue;
        //            break;
        //        default:
        //            break;
        //    }
        //    return v;
        //}

        /// <summary>
        /// 获取单元格的值。
        /// </summary>
        /// <param name="cell">单元格。</param>
        /// <returns>字符串值。</returns>
        //public static string GetCellValue(ICell cell)
        //{
        //    CellType ct = cell.CellType;
        //    string v = string.Empty;
        //    switch (ct)
        //    {
        //        case CellType.Boolean:
        //            v = cell.BooleanCellValue ? "1" : "0";
        //            break;
        //        case CellType.Numeric:
        //            v = cell.NumericCellValue.ToString();
        //            break;
        //        case CellType.String:
        //            v = cell.StringCellValue;
        //            break;
        //        default:
        //            break;
        //    }
        //    return v;
        //}
    }
}

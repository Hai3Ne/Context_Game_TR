using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;

namespace ExcelExporter
{
    /// <summary>
    /// 需要导出的数据。
    /// </summary>
    public class ExportData
    {
        /// <summary>
        /// 数据起始行。
        /// </summary>
        public const int DATA_START_ROW = 5;

        public ExportData(string _sheetName, ISheet _ws)
        {
            this.sheetName = _sheetName;
            this.Wooksheet = _ws;
        }

        /// <summary>
        /// 表格名称。
        /// </summary>
        public string m_Name;

        /// <summary>
        /// sheetName
        /// </summary>
        private string sheetName;
        private ISheet wooksheet;

        /// <summary>
        /// 
        /// </summary>
        public ISheet Wooksheet { get => wooksheet; set => wooksheet = value; }
    }
}

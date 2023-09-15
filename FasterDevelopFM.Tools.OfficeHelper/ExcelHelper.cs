using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;

namespace FasterDevelopFM.Tools.OfficeHelper
{

    /// <summary>
    /// 提供 Excel 读取、写入功能。此类不可被继承
    /// </summary>
    public sealed class ExcelHelper
    {
        #region 构造函数
        /// <summary>
        /// 初始化 <see cref="Excel"/> 类的新实例
        /// </summary>
        public ExcelHelper() { }
        #endregion

        #region 静态属性
        /// <summary>
        /// Excel 的 Content-Type
        /// </summary>
        public static string ContentType { get => "application/vnd.ms-excel"; }
        #endregion

        #region 将指定路径的 Excel 文件读取到 DataTable
        /// <summary>
        /// 将指定路径的 Excel 文件读取到 <see cref="DataTable"/>
        /// </summary>
        /// <param name="filePath">指定文件完整路径名</param>
        /// <param name="sheetName">指定读取 Excel 工作薄 sheet 的名称</param>
        /// <param name="firstRowIsColumnName">首行是否为 <see cref="DataColumn.ColumnName"/></param>
        /// <param name="addEmptyRow">是否添加空行，默认为 false，不添加</param>
        /// <returns>
        /// 如果 filePath 参数为 null 或者空字符串("")，则返回 null；
        /// 如果 filePath 参数值的磁盘中不存在 Excel 文件，则返回 null；
        /// 否则返回从指定 Excel 文件读取后的 <see cref="DataTable"/> 对象。
        /// </returns>
        public DataTable ReadExcelToDataTable(string filePath, string sheetName = null, bool firstRowIsColumnName = true, bool addEmptyRow = false)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return null;

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var dt = ReadStreamToDataTable(fileStream, sheetName, firstRowIsColumnName, addEmptyRow);
            return dt;
        }
        #endregion

        #region 将指定路径的 Excel 文件读取到 ICollection<DataTable>
        /// <summary>
        /// 将指定路径的 Excel 文件读取到 <see cref="ICollection{DataTable}"/>
        /// </summary>
        /// <param name="filePath">指定文件完整路径名</param>
        /// <param name="firstRowIsColumnName">首行是否为 <see cref="DataColumn.ColumnName"/></param>
        /// <param name="addEmptyRow">是否添加空行，默认为 false，不添加</param>
        /// <returns>
        /// 如果 filePath 参数为 null 或者空字符串("")，则返回 null；
        /// 如果 filePath 参数值的磁盘中不存在 Excel 文件，则返回 null；
        /// 否则返回从指定 Excel 文件读取后的 <see cref="ICollection{DataTable}"/> 对象，
        /// 其中一个 <see cref="DataTable"/> 对应一个 Sheet 工作簿。
        /// </returns>
        public ICollection<DataTable> ReadExcelToTables(string filePath, bool firstRowIsColumnName = true, bool addEmptyRow = false)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return null;

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var tables = ReadStreamToTables(fileStream, firstRowIsColumnName, addEmptyRow);
            return tables;
        }
        #endregion

        #region 将指定路径的 Excel 文件用异步方式读取到 DataTable
        /// <summary>
        /// 将指定路径的 Excel 文件用异步方式读取到 <see cref="DataTable"/>
        /// </summary>
        /// <param name="filePath">指定文件完整路径名</param>
        /// <param name="sheetName">指定读取 Excel 工作薄 sheet 的名称</param>
        /// <param name="firstRowIsColumnName">首行是否为 <see cref="DataColumn.ColumnName"/></param>
        /// <param name="addEmptyRow">是否添加空行，默认为 false，不添加</param>
        /// <returns>
        /// 如果 filePath 参数为 null 或者空字符串("")，则返回 null；
        /// 如果 filePath 参数值的磁盘中不存在 Excel 文件，则返回 null；
        /// 否则返回从指定 Excel 文件读取后的 <see cref="DataTable"/> 对象。
        /// </returns>
        public async Task<DataTable> ReadExcelToDataTableAsync(string filePath, string sheetName = null, bool firstRowIsColumnName = true, bool addEmptyRow = false) =>
            await Task.Run(() => ReadExcelToDataTable(filePath, sheetName, firstRowIsColumnName, addEmptyRow));
        #endregion

        #region 将指定路径的 Excel 文件用异步方式读取到 ICollection<DataTable>
        /// <summary>
        /// 将指定路径的 Excel 文件用异步方式读取到 <see cref="ICollection{DataTable}"/>
        /// </summary>
        /// <param name="filePath">指定文件完整路径名</param>
        /// <param name="firstRowIsColumnName">首行是否为 <see cref="DataColumn.ColumnName"/></param>
        /// <param name="addEmptyRow">是否添加空行，默认为 false，不添加</param>
        /// <returns>
        /// 如果 filePath 参数为 null 或者空字符串("")，则返回 null；
        /// 如果 filePath 参数值的磁盘中不存在 Excel 文件，则返回 null；
        /// 否则返回从指定 Excel 文件读取后的 <see cref="ICollection{DataTable}"/> 对象，
        /// 其中一个 <see cref="DataTable"/> 对应一个 Sheet 工作簿。
        /// </returns>
        public async Task<ICollection<DataTable>> ReadExcelToTablesAsync(string filePath, bool firstRowIsColumnName = true, bool addEmptyRow = false) =>
            await Task.Run(() => ReadExcelToTables(filePath, firstRowIsColumnName, addEmptyRow));
        #endregion

        #region 将 IFormFile 对象读取到 DataTable
        /// <summary>
        /// 将 <see cref="IFormFile"/> 对象读取到 <see cref="DataTable"/>
        /// </summary>
        /// <param name="formFile"><see cref="IFormFile"/>对象</param>
        /// <param name="sheetName">指定读取 Excel 工作薄 sheet 的名称</param>
        /// <param name="firstRowIsColumnName">首行是否为 <see cref="DataColumn.ColumnName"/></param>
        /// <param name="addEmptyRow">是否添加空行，默认为 false，不添加</param>
        /// <returns>
        /// 如果 formFile 参数为 null，则返回 null；
        /// 如果 formFile 参数的 <see cref="IFormFile.Length"/> 属性为 小于或者等于 0，则返回 null；
        /// 否则返回从 <see cref="IFormFile"/>读取后的 <see cref="DataTable"/> 对象。
        /// </returns>
        public DataTable ReadFormFileToDataTable(IFormFile formFile, string sheetName = null, bool firstRowIsColumnName = true, bool addEmptyRow = false)
        {
            if (formFile == null || formFile.Length <= 0) return null;

            var stream = formFile.OpenReadStream();
            var dt = ReadStreamToDataTable(stream, sheetName, firstRowIsColumnName, addEmptyRow, dispose: false);
            return dt;
        }
        #endregion

        #region 将 IFormFile 读取到 ICollection<DataTable>
        /// <summary>
        /// 将 <see cref="IFormFile"/> 读取到 <see cref="ICollection{DataTable}"/>
        /// </summary>
        /// <param name="formFile">要读取的 <see cref="IFormFile"/> 对象</param>
        /// <param name="firstRowIsColumnName">首行是否为 <see cref="DataColumn.ColumnName"/></param>
        /// <param name="addEmptyRow">是否添加空行，默认为 false，不添加</param>
        /// <returns>
        /// 如果 formFile 参数为 null，则返回 null；
        /// 如果 formFile 参数的 <see cref="IFormFile.Length"/> 属性小于或者等于 0，则返回 null；
        /// 否则返回从 <see cref="IFormFile"/>读取后的 <see cref="ICollection{DataTable}"/> 对象，
        /// 其中一个 <see cref="DataTable"/> 对应一个 Sheet 工作簿。
        /// </returns>
        public ICollection<DataTable> ReadFormFileToTables(IFormFile formFile,
            bool firstRowIsColumnName = true,
            bool addEmptyRow = false)
        {
            if (formFile == null || formFile.Length <= 0) return null;

            var stream = formFile.OpenReadStream();
            var tables = ReadStreamToTables(stream, firstRowIsColumnName, addEmptyRow, dispose: false);
            return tables;
        }
        #endregion

        #region 将 IFormFile 用异步方式读取到 DataTable
        /// <summary>
        /// 将 <see cref="IFormFile"/> 用异步方式读取到 <see cref="DataTable"/>
        /// </summary>
        /// <param name="formFile">要读取的 <see cref="IFormFile"/> 对象</param>
        /// <param name="sheetName">指定读取 Excel 工作薄 sheet 的名称</param>
        /// <param name="firstRowIsColumnName">首行是否为 <see cref="DataColumn.ColumnName"/></param>
        /// <param name="addEmptyRow">是否添加空行，默认为 false，不添加</param>
        /// <returns>
        /// 如果 httpPostedFile 参数为 null，则返回 null；
        /// 如果 httpPostedFile 参数的 <see cref="IFormFile.Length"/> 属性为 小于或者等于 0，则返回 null；
        /// 否则返回从 <see cref="IFormFile"/>读取后的 <see cref="DataTable"/> 对象。
        /// </returns>
        public async Task<DataTable> ReadFormFileToDataTableAsync(
            IFormFile formFile,
            string sheetName = null,
            bool firstRowIsColumnName = true,
            bool addEmptyRow = false) => await Task.Run(() => ReadFormFileToDataTable(formFile, sheetName, firstRowIsColumnName, addEmptyRow));
        #endregion

        #region 将 IFormFile 用异步方式读取到 ICollection<DataTable>
        /// <summary>
        /// 将 <see cref="IFormFile"/> 用异步方式读取到 <see cref="ICollection{DataTable}"/>
        /// </summary>
        /// <param name="formFile">要读取的 <see cref="IFormFile"/> 对象</param>
        /// <param name="firstRowIsColumnName">首行是否为 <see cref="DataColumn.ColumnName"/></param>
        /// <param name="addEmptyRow">是否添加空行，默认为 false，不添加</param>
        /// <returns>
        /// 如果 httpPostedFile 参数为 null，则返回 null；
        /// 如果 httpPostedFile 参数的 <see cref="IFormFile.Length"/> 属性小于或者等于 0，则返回 null；
        /// 否则返回从 <see cref="IFormFile"/>读取后的 <see cref="ICollection{DataTable}"/> 对象，
        /// 其中一个 <see cref="DataTable"/> 对应一个 Sheet 工作簿。
        /// </returns>
        public async Task<ICollection<DataTable>> ReadFormFileToTablesAsync(IFormFile formFile,
            bool firstRowIsColumnName = true,
            bool addEmptyRow = false) => await Task.Run(() => ReadFormFileToTables(formFile, firstRowIsColumnName, addEmptyRow));
        #endregion

        #region 将 Stream 对象读取到 DataTable
        /// <summary>
        /// 将 <see cref="Stream"/> 对象读取到 <see cref="DataTable"/>
        /// </summary>
        /// <param name="stream">当前 <see cref="Stream"/> 对象</param>
        /// <param name="sheetName">指定读取 Excel 工作薄 sheet 的名称</param>
        /// <param name="firstRowIsColumnName">首行是否为 <see cref="DataColumn.ColumnName"/></param>
        /// <param name="addEmptyRow">是否添加空行，默认为 false，不添加</param>
        /// <param name="dispose">是否释放 <see cref="Stream"/> 资源</param>
        /// <returns>
        /// 如果 stream 参数为 null，则返回 null；
        /// 如果 stream 参数的 <see cref="Stream.CanRead"/> 属性为 false，则返回 null；
        /// 如果 stream 参数的 <see cref="Stream.Length"/> 属性为 小于或者等于 0，则返回 null；
        /// 否则返回从 <see cref="Stream"/> 读取后的 <see cref="DataTable"/> 对象。
        /// </returns>
        public DataTable ReadStreamToDataTable(Stream stream, string sheetName = null, bool firstRowIsColumnName = true, bool addEmptyRow = false,
            bool dispose = true)
        {
            if (stream == null || !stream.CanRead || stream.Length <= 0) return null;

            var workbook = WorkbookFactory.Create(stream);
            var sheet = workbook.GetSheetAt(0);
            if (string.IsNullOrWhiteSpace(sheetName)) sheet = workbook.GetSheet(sheetName);
            if (sheet == null) return null;

            var table = ReadSheetToDataTable(sheet, firstRowIsColumnName, addEmptyRow);

            if (dispose)
            {
                stream.Flush();
                stream.Close();
            }
            return table;
        }
        #endregion

        #region 将 Stream 对象读取到 ICollection<DataTable>
        /// <summary>
        /// 将 <see cref="Stream"/> 对象读取到 <see cref="ICollection{DataTable}"/>
        /// </summary>
        /// <param name="stream">要读取的 <see cref="Stream"/> 对象</param>
        /// <param name="firstRowIsColumnName">首行是否为 <see cref="DataColumn.ColumnName"/></param>
        /// <param name="addEmptyRow">是否添加空行，默认为 false，不添加</param>
        /// <param name="dispose">是否释放 <see cref="Stream"/> 资源</param>
        /// <returns>
        /// 如果 stream 参数为 null，则返回 null；
        /// 如果 stream 参数的 <see cref="Stream.CanRead"/> 属性为 false，则返回 null；
        /// 如果 stream 参数的 <see cref="Stream.Length"/> 属性小于或者等于 0，则返回 null；
        /// 否则返回从 <see cref="Stream"/> 读取后的 <see cref="ICollection{DataTable}"/> 对象，
        /// 其中一个 <see cref="DataTable"/> 对应一个 Sheet 工作簿。
        /// </returns>
        public ICollection<DataTable> ReadStreamToTables(Stream stream, bool firstRowIsColumnName = true, bool addEmptyRow = false,
            bool dispose = true)
        {
            if (stream == null || !stream.CanRead || stream.Length <= 0) return null;

            var workbook = WorkbookFactory.Create(stream);
            var tables = new HashSet<DataTable>();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var sheet = workbook.GetSheetAt(i);
                if (sheet == null) continue;

                var dataTable = ReadSheetToDataTable(sheet, firstRowIsColumnName, addEmptyRow);
                tables.Add(dataTable);
            }

            if (dispose)
            {
                stream.Flush();
                stream.Close();
            }
            return tables;
        }
        #endregion

        #region 将 Stream 对象用异步方式读取到 DataTable
        /// <summary>
        /// 将 <see cref="Stream"/> 对象用异步方式读取到 <see cref="DataTable"/>
        /// </summary>
        /// <param name="stream">要读取的 <see cref="Stream"/> 对象</param>
        /// <param name="sheetName">指定读取 Excel 工作薄 sheet 的名称</param>
        /// <param name="firstRowIsColumnName">首行是否为 <see cref="DataColumn.ColumnName"/></param>
        /// <param name="addEmptyRow">是否添加空行，默认为 false，不添加</param>
        /// <param name="dispose">是否释放 <see cref="Stream"/> 资源</param>
        /// <returns>
        /// 如果 stream 参数为 null，则返回 null；
        /// 如果 stream 参数的 <see cref="Stream.CanRead"/> 属性为 false，则返回 null；
        /// 如果 stream 参数的 <see cref="Stream.Length"/> 属性为 小于或者等于 0，则返回 null；
        /// 否则返回从 <see cref="Stream"/> 读取后的 <see cref="DataTable"/> 对象。
        /// </returns>
        public async Task<DataTable> ReadStreamToDataTableAsync(Stream stream,
            string sheetName = null,
            bool firstRowIsColumnName = true,
            bool addEmptyRow = false,
            bool dispose = true) => await Task.Run(() => ReadStreamToDataTable(stream, sheetName, firstRowIsColumnName, addEmptyRow, dispose));
        #endregion

        #region 将 Stream 对象用异步方式读取到 ICollection<DataTable>
        /// <summary>
        /// 将 <see cref="Stream"/> 对象用异步方式读取到 <see cref="ICollection{DataTable}"/>
        /// </summary>
        /// <param name="stream">要读取的 <see cref="Stream"/> 对象</param>
        /// <param name="firstRowIsColumnName">首行是否为 <see cref="DataColumn.ColumnName"/></param>
        /// <param name="addEmptyRow">是否添加空行，默认为 false，不添加</param>
        /// <param name="dispose">是否释放 <see cref="Stream"/> 资源</param>
        /// <returns>
        /// 如果 stream 参数为 null，则返回 null；
        /// 如果 stream 参数的 <see cref="Stream.CanRead"/> 属性为 false，则返回 null；
        /// 如果 stream 参数的 <see cref="Stream.Length"/> 属性小于或者等于 0，则返回 null；
        /// 否则返回从 <see cref="Stream"/> 读取后的 <see cref="ICollection{DataTable}"/> 对象，
        /// 其中一个 <see cref="DataTable"/> 对应一个 Sheet 工作簿。
        /// </returns>
        public async Task<ICollection<DataTable>> ReadStreamToTablesAsync(Stream stream,
            bool firstRowIsColumnName = true,
            bool addEmptyRow = false,
            bool dispose = true) => await Task.Run(() => ReadStreamToTables(stream, firstRowIsColumnName, addEmptyRow, dispose));
        #endregion

        #region 将 IFormFileCollection 对象读取到 ICollection<Collection<DataTable>> 集合
        /// <summary>
        /// 将 <see cref="IFormFileCollection"/> 对象读取到 <see cref="ICollection{Collection}"/> 集合
        /// </summary>
        /// <param name="formFiles">要读取的 <see cref="IFormFileCollection"/></param>
        /// <param name="firstRowIsColumnName">首行是否为 <see cref="DataColumn.ColumnName"/></param>
        /// <param name="addEmptyRow">是否添加空行，默认为 false，不添加</param>
        /// <returns>
        /// 如果 formFiles 参数为 null，则返回 null；
        /// 如果 formFiles 参数的 <see cref="IFormFileCollection"/> 的 Count 属性小于或者等于 0，则返回 null；
        /// 否则返回从 <see cref="IFormFileCollection"/> 读取后的 <see cref="ICollection{Collection}"/> 集合。
        /// 结构说明：
        /// <see cref="IFormFile"/> 对应 <see cref="Collection{DataTable}"/>;
        /// <see cref="DataTable"/> 对应 Sheet 工作簿。
        /// </returns>
        public ICollection<Collection<DataTable>> ReadHttpFileCollectionToTableCollection(IFormFileCollection formFiles,
            bool firstRowIsColumnName = true,
            bool addEmptyRow = false)
        {
            if (formFiles == null || formFiles.Count <= 0) return null;

            var collection = new HashSet<Collection<DataTable>>();
            for (int i = 0; i < formFiles.Count; i++)
            {
                var file = formFiles[i];
                var stream = file.OpenReadStream();
                if (stream == null || !stream.CanRead || stream.Length <= 0) continue;

                var workbook = WorkbookFactory.Create(stream);
                var tables = new Collection<DataTable>();
                for (int j = 0; j < workbook.NumberOfSheets; j++)
                {
                    var sheet = workbook.GetSheetAt(j);
                    if (sheet == null) continue;

                    var dataTable = ReadSheetToDataTable(sheet, firstRowIsColumnName, addEmptyRow);
                    tables.Add(dataTable);
                }
                collection.Add(tables);
            }

            return collection;
        }
        #endregion

        #region 读取 Excel 工作簿到 DataTable
        /// <summary>
        /// 读取 Excel 工作簿到 DataTable
        /// </summary>
        /// <param name="sheet">指定的 Sheet 工作簿</param>
        /// <param name="firstRowIsColumnName">首行是否为 <see cref="DataColumn.ColumnName"/></param>
        /// <param name="addEmptyRow">是否添加空行，默认为 false，不添加</param>
        /// <returns></returns>
        private DataTable ReadSheetToDataTable(ISheet sheet,
            bool firstRowIsColumnName = true,
            bool addEmptyRow = false)
        {
            var table = new DataTable(sheet.SheetName);
            var headerRow = sheet.GetRow(0);
            var cellCount = headerRow.LastCellNum;
            var rowCount = sheet.LastRowNum;
            var startRowIndex = sheet.FirstRowNum;

            if (firstRowIsColumnName)
            {
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    var column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    table.Columns.Add(column);
                }
                startRowIndex = sheet.FirstRowNum + 1;
            }

            for (var i = startRowIndex; i <= rowCount; i++)
            {
                var row = sheet.GetRow(i);
                var dataRow = table.NewRow();

                if (row == null)
                {
                    if (addEmptyRow) table.Rows.Add(dataRow);
                    continue;
                }

                for (var j = row.FirstCellNum; j < cellCount; j++)
                {
                    var value = row.GetCell(j);
                    if (value != null) dataRow[j] = GetCellValue(value);
                }

                table.Rows.Add(dataRow);
            }

            return table;
        }
        #endregion

        #region 获取单元格的值
        /// <summary>
        /// 获取单元格的值
        /// </summary>
        /// <param name="cell">要获取值的 <see cref="ICell"/></param>
        /// <returns>从 <see cref="ICell"/> 获取到的值</returns>
        private static string GetCellValue(ICell cell)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.Blank: return string.Empty;
                case CellType.Boolean: return cell.BooleanCellValue.ToString();
                case CellType.Error: return cell.ErrorCellValue.ToString();
                case CellType.Numeric:
                case CellType.Unknown:
                default: return cell.ToString();
                case CellType.String: return cell.StringCellValue;
                case CellType.Formula:
                    try
                    {
                        var e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }
        #endregion

        #region 将 List 集合写入到 MemoryStream 对象
        /// <summary>
        /// 将 <see cref="List{T}"/> 集合写入到 <see cref="MemoryStream"/> 对象
        /// </summary>
        /// <typeparam name="T">要写入 <see cref="MemoryStream"/> 的集合元素的类型</typeparam>
        /// <param name="list">要写入的 <see cref="List{T}"/> 集合</param>
        /// <param name="columns"></param>
        /// <param name="sheetsName">Excel 的工作簿名称</param>
        /// <returns>Excel 形式的 <see cref="MemoryStream"/> 对象</returns>
        public MemoryStream WriteToMemoryStream<T>(T[] list, Dictionary<string, string> columns, string sheetsName = "sheet1")
        {
            if (list == null) return null;

            var memoryStream = new MemoryStream();
            var propertys = typeof(T).GetProperties();

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet(sheetsName);
            var row = sheet.CreateRow(0);

            var columnIndex = 0;
            foreach (var item in columns)
            {
                row.CreateCell(columnIndex).SetCellValue(item.Value);
                columnIndex++;
            }


            if (list.Length > 0)
            {
                var rowIndex = 0;
                foreach (var rowData in list)
                {
                    var dataRow = sheet.CreateRow(rowIndex + 1);

                    columnIndex = 0;
                    foreach (var column in columns)
                    {
                        var cell = dataRow.CreateCell(columnIndex);
                        var property = propertys.FirstOrDefault(a => a.Name == column.Key);
                        var value = property.GetValue(row);
                        if (value == null) value = string.Empty;
                        DateTime? date = null;

                        if (value != null && property.PropertyType == typeof(DateTime))
                        {
                            var _date = DateTime.Parse(value.ToString());
                            value = _date.ToString("yyyy-MM-dd HH:mm:ss");
                        }

                        if (value != null && property.PropertyType == typeof(DateTime?))
                        {
                            date = (DateTime?)value;
                            value = date.HasValue ? date.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
                        }

                        cell.SetCellValue(value.ToString());
                        columnIndex++;
                    }
                    rowIndex++;
                }
            }

            workbook.Write(memoryStream);
            return memoryStream;
        }

        #endregion

        #region 将 List 集合写入到 MemoryStream 对象
        /// <summary>
        /// 将 <see cref="List{T}"/> 集合写入到 <see cref="MemoryStream"/> 对象
        /// </summary>
        /// <param name="objects">要写入的 <see cref="List{JObject}"/> 集合</param>
        /// <param name="columns"></param>
        /// <param name="sheetsName">Excel 的工作簿名称</param>
        /// <param name="title">标题</param>
        /// <returns>Excel 形式的 <see cref="MemoryStream"/> 对象</returns>
        public MemoryStream WriteToMemoryStream(JObject[] objects, Dictionary<string, string> columns, string sheetsName = "sheet1", string title = "")
        {
            if (objects == null || objects.Length <= 0) return null;

            var memoryStream = new MemoryStream();
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet(sheetsName);
            var row = sheet.CreateRow(0);
            var columnIndex = 0;
            foreach (var column in columns)
            {
                row.CreateCell(columnIndex).SetCellValue(column.Value);
                columnIndex++;
            }
            var rowIndex = 0;
            foreach (var rowData in objects)
            {
                var dataRow = sheet.CreateRow(rowIndex + 1);

                columnIndex = 0;
                foreach (var column in columns)
                {
                    var cell = dataRow.CreateCell(columnIndex);
                    cell.SetCellValue(rowData[column.Key].ToString());
                    columnIndex++;
                }


                rowIndex++;

            }

            return memoryStream;
        }

        #endregion

        #region 将 List 集合用异步方式写入到 MemoryStream 对象
        /// <summary>
        /// 将 <see cref="List{T}"/> 集合用异步方式写入到 <see cref="MemoryStream"/> 对象
        /// </summary>
        /// <typeparam name="T">要写入 <see cref="MemoryStream"/> 的集合元素的类型</typeparam>
        /// <param name="list">要写入的 <see cref="List{T}"/> 集合</param>
        /// <param name="columns"></param>
        /// <param name="sheetsName">Excel 的工作簿名称</param>
        /// <returns>Excel 形式的 <see cref="MemoryStream"/> 对象</returns>
        public async Task<MemoryStream> WriteToMemoryStreamAsync<T>(T[] list, Dictionary<string, string> columns, string sheetsName = "sheet1") =>
            await Task.Factory.StartNew(() => WriteToMemoryStream(list, columns, sheetsName));
        #endregion
    }

}
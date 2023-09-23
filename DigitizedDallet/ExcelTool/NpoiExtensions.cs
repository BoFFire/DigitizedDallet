using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;

namespace DigitizedDallet.ExcelTool;

public static class NpoiExtensions
{
    public static ISheet CreateSheet(this IWorkbook workbook, string name, string[] columns, int[] widths)
    {
        var sheet = workbook.CreateSheet(name);
        sheet.DefaultColumnWidth = 30;

        var headerRow = sheet.CreateRow(0);

        for (int i = 0; i < columns.Length; i++)
        {
            var column = columns[i];
            var width = widths[i];
            headerRow.CreateHeaderCell(column);
            sheet.SetColumnWidth(i, width);
        }

        headerRow.Sheet.CreateFreezePane(0, 1);

        return sheet;
    }


    public static void CreateHeaderCell(this IRow row, string value)
    {
        IFont font = row.Sheet.Workbook.CreateFont();
        font.Color = HSSFColor.Black.Index;
        font.IsBold = true;

        ICellStyle style = row.Sheet.Workbook.CreateCellStyle();
        style.SetFont(font);
        style.FillForegroundColor = HSSFColor.Grey25Percent.Index;
        style.BorderBottom = BorderStyle.Medium;
        style.BorderTop = BorderStyle.Medium;
        style.FillPattern = FillPattern.SolidForeground;

        ICell cell = row.CreateCell(row.Cells.Count, CellType.String);
        cell.SetCellValue(value);
        cell.CellStyle = style;
        cell.SetCellType(CellType.String);
    }

    public static IRow CreateRowWithCells(this ISheet sheet, params object?[] values)
        => sheet.CreateRow(sheet.GetRowsCount()).AddCells(values);

    public static int GetRowsCount(this ISheet sheet) => sheet.LastRowNum + 1;

    public static IRow AddCells(this IRow row, params object?[] values)
    {
        foreach (var value in values)
        {
            row.AddCell(value);
        }

        return row;
    }

    public static void AddCell(this IRow row, object? objValue)
    {
        var cell = row.CreateCell(row.Cells.Count);

        //if (row.Sheet.GetRowsCount() % 2 == 0)
        //{
        //    ICellStyle style = row.Sheet.Workbook.CreateCellStyle();
        //    IFont font = row.Sheet.Workbook.CreateFont();
        //    font.IsBold = true;
        //    style.SetFont(font);
        //    cell.CellStyle = style;
        //}

        if (objValue != null)
        {
            if (objValue is string stringValue)
            {
                cell.SetCellType(CellType.String);
                cell.SetCellValue(stringValue);
            }
            else if (objValue is long longValue)
            {
                cell.SetCellValue(longValue);
            }
            else if (objValue is int intValue)
            {
                cell.SetCellValue(intValue);
            }
            else if (objValue is double doubleValue)
            {
                cell.SetCellValue(doubleValue);
            }
            else if (objValue is bool boolValue)
            {
                cell.SetCellValue(boolValue);
            }
            else if (objValue is DateTime dataValue)
            {
                cell.SetCellValue(dataValue);
            }
            else
            {
                cell.SetCellValue(objValue.ToString());
            }
        }
    }



    static public List<T> Build<T>(Stream stream, string path, out string[] warnings) where T : class
    {
        using (stream)
        {
            return GetBook(stream, path).Build<T>(out warnings);
        }
    }

    static public List<T> Build<T>(string path, out string[] warnings) where T : class => GetBook(path).Build<T>(out warnings);

    static public List<T> Build<T>(this IWorkbook book, out string[] warnings) where T : class
    {
        List<T> models = new List<T>();

        var properties = typeof(T).GetProperties().ToDictionary(x => x.Name, y => y);

        var datatable = book.SelectDataTableFrom();

        warnings = properties.Where(x => !datatable.Columns.Contains(x.Key)).Select(x => x.Key).ToArray();

        foreach (var row in datatable.AsEnumerable())
        {
            T model = (T)Activator.CreateInstance(typeof(T))!;

            foreach (var kvp in properties)
            {
                if (!datatable.Columns.Contains(kvp.Key))
                {
                    continue;
                }

                string? string_value = row.Field<string>(kvp.Key)?.Trim();
                if (string.IsNullOrWhiteSpace(string_value))
                {
                    string_value = null;
                }

                object? value;
                if (kvp.Value.PropertyType == typeof(decimal)
                   || kvp.Value.PropertyType == typeof(decimal?))
                {
                    value = decimal.TryParse(string_value, out var temp) ? temp : null;
                }
                else if (kvp.Value.PropertyType == typeof(int)
                   || kvp.Value.PropertyType == typeof(int?))
                {
                    value = int.TryParse(string_value, out var temp) ? temp : null;
                }
                else if (kvp.Value.PropertyType == typeof(bool)
                   || kvp.Value.PropertyType == typeof(bool?))
                {
                    value = string_value == null ? null : string_value.Equals("oui", StringComparison.OrdinalIgnoreCase);
                }
                else if (kvp.Value.PropertyType == typeof(DateTime)
                   || kvp.Value.PropertyType == typeof(DateTime?))
                {
                    if (double.TryParse(string_value, out var temp_num))
                    {
                        value = DateTime.FromOADate(temp_num);
                    }
                    else
                    {
                        value = string_value == null ? null : DateTime.TryParseExact(string_value, "dd/MM/yy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var temp) ? temp : null;
                    }
                }
                else
                {
                    value = string_value;
                }

                kvp.Value.SetValue(model, value);
            }
            models.Add(model);
        }
        return models;

    }

    public static IWorkbook GetBook(Stream stream, string path)
    {
        var fileExtension = Path.GetExtension(path).ToLower();

        try
        {
            if (fileExtension == ".xls")
            {
                return new HSSFWorkbook(stream);
            }
            else if (fileExtension == ".xlsx")
            {
                return new XSSFWorkbook(stream);
            }

            throw new ExcelToolException(string.Format("The file type {0} is not supported, only .xls and .xlsx files are allowed", fileExtension));

        }
        catch (Exception ex)
        {
            throw new ExcelToolException("ET_OPEN", ex);
        }
    }

    public static IWorkbook GetBook(string path)
    {
        var fileExtension = Path.GetExtension(path).ToLower();

        try
        {
            using FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            if (fileExtension == ".xls")
            {
                return new HSSFWorkbook(file);
            }
            else if (fileExtension == ".xlsx")
            {
                return new XSSFWorkbook(file);
            }

            throw new ExcelToolException(string.Format("The file type {0} is not supported, only .xls and .xlsx files are allowed", fileExtension));
        }
        catch (Exception ex)
        {
            throw new ExcelToolException("ET_OPEN", ex);
        }
    }

    public static EnumerableRowCollection<DataRow> SelectDataFrom(this IWorkbook workbook) => SelectDataTableFrom(workbook).AsEnumerable();

    public static DataTable SelectDataTableFrom(this IWorkbook workbook) => workbook.GetSheetAt(0).GetDataTableFromExcel();

    public static DataTable GetDataTableFromExcel(this ISheet sh)
    {
        DataTable data_table = new DataTable(sh.SheetName);

        // add neccessary columns
        if (data_table.Columns.Count < sh.GetRow(0).Cells.Count)
        {
            for (int j = 0; j < sh.GetRow(0).Cells.Count; j++)
            {
                var colName = sh.GetRow(0).GetCell(j).StringCellValue;
                data_table.Columns.Add(colName, typeof(string));
            }
        }

        int i = 0;

        while (sh.GetRow(i + 1) != null && !string.IsNullOrWhiteSpace(sh.GetRow(i + 1).GetCell(0)?.ToString()))
        {

            // add row
            data_table.Rows.Add();

            // write row value
            for (int j = 0; j < data_table.Columns.Count; j++)
            {
                var cell = sh.GetRow(i + 1).GetCell(j);

                if (cell != null)
                {
                    // TODO: you can add more cell types capatibility, e. g. formula
                    switch (cell.CellType)
                    {
                        case NPOI.SS.UserModel.CellType.Numeric:
                            data_table.Rows[i][j] = cell.NumericCellValue;
                            //dataGridView1[j, i].Value = sh.GetRow(i).GetCell(j).NumericCellValue;
                            break;
                        case NPOI.SS.UserModel.CellType.String:
                            data_table.Rows[i][j] = cell.StringCellValue;
                            break;
                        case NPOI.SS.UserModel.CellType.Boolean:
                            data_table.Rows[i][j] = cell.BooleanCellValue;
                            break;
                    }
                }
            }

            i++;
        }

        return data_table;
    }

}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using Helper.Model;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;

namespace Helper
{
    public class ExcelHelper:IDisposable
    {
        private Application Application { get; set; }
        private Workbook Workbook { get; set; }
        private Sheets Sheets { get; set; }

        public ExcelHelper(){}

        public ExcelHelper(string filePath)
        {
            Application = new Application();
            Workbook = this.Application.Workbooks.Open(filePath);
            Sheets = this.Workbook == null ? null : this.Workbook.Sheets;
        }

        public void InitExcel(string filePath)
        {
            Application = new Application();
            Workbook = this.Application.Workbooks.Open(filePath);
            Sheets = this.Workbook == null ? null : this.Workbook.Sheets;
        }

        public List<List<string>> GetAllSheetData()
        {
            var dt = new List<List<string>>();
            if (Sheets != null)
            {
                foreach (Worksheet sheet in Sheets)
                {
                    foreach (Range row in sheet.UsedRange.Rows)
                    {
                        List<string> strs = new List<string>();
                        foreach (Range cell in row.Columns)
                        {
                            strs.Add(cell.Value != null ? cell.Value[Type.Missing].ToString() : "");
                        }
                        dt.Add(strs);
                    }
                }
            }
            return dt;
        }

        public List<List<string>> GetDataBySheetName(object sheetName)
        {
            var dt = new List<List<string>>();
            if (Sheets != null)
            {
                var sheet = (Worksheet)Sheets[sheetName];
                if (sheet != null)
                {
                    foreach (Range row in sheet.UsedRange.Rows)
                    {
                        List<string> strs=new List<string>();
                        foreach (Range cell in row.Columns)
                        {
                            try
                            {
                                strs.Add(cell.Value != null ? cell.Value[Type.Missing].ToString() : "");
                            }
                            catch (Exception e)
                            {
                                LogHelper.Log(e.Message,e,LogHelper.LogType.Error);
                                strs.Add("");
                            }
                        }
                        dt.Add(strs);
                    }
                }
            }
            return dt;
        }

        public List<Dictionary<string,string>> GetDicsExceptHeader(object sheetName)
        {
            var dt = new List<Dictionary<string, string>>();
            if (Sheets != null)
            {
                var sheet = (Worksheet)Sheets[sheetName];
                if (sheet != null)
                {
                    //get the excel headers
                    var headers =new string[sheet.UsedRange.Columns.Count];
                    for (int i = 1; i<=sheet.UsedRange.Columns.Count; i++)
                    {
                        var text = ((Range)sheet.UsedRange.Cells[1, i]);
                        string head = (text != null && text.Text != null)?text.Text.ToString():"";
                        headers[i-1]=head;
                    }

                    try
                    {
                        for (int i = 2; i <= sheet.UsedRange.Rows.Count; i++)
                        {
                            var dics = new Dictionary<string, string>();
                            for (int j = 1; j <= sheet.UsedRange.Columns.Count; j++)
                            {
                                var text = ((Range)sheet.UsedRange.Cells[1, i]);
                                string head = (text != null && text.Text != null) ? text.Text.ToString() : "";
                                dics.Add(headers[j - 1], head);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return dt;
        }
    
        public List<Dictionary<string,string>> GetDicsExceptHeader(object sheetName,params string[] headers)
        {
            var dt = new List<Dictionary<string, string>>();
            if (Sheets != null)
            {
                var sheet = (Worksheet)Sheets[sheetName];
                if (sheet != null)
                {
                    try
                    {
                        for (int i = 2; i <= sheet.UsedRange.Rows.Count; i++)
                        {
                            var dics = new Dictionary<string, string>();
                            for (int j = 1; j <= sheet.UsedRange.Columns.Count; j++)
                            {
                                var text = ((Range)sheet.UsedRange.Cells[1, i]);
                                string head = (text != null && text.Text != null) ? text.Text.ToString() : "";
                                dics.Add(headers[j - 1], head);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return dt;
        }

        public bool ExportExcel(string filename,List<List<string>> data,string sheetname,XlFileFormat fileFormat,List<ExcelColorModel> excelColorEntities )
        {
            var xlApp = new Application();
            var workbooks = xlApp.Workbooks;
            var workbook=workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            var worksheet =(Worksheet) workbook.Worksheets[1];
            worksheet.Name = sheetname;
            worksheet.Activate();
            var result = true;
            if (data != null)
            {
                for (int i = 1; i <= data.Count; i++)
                {
                    for (int j = 1; j <= data[i-1].Count; j++)
                    {
                        worksheet.Cells[i, j] = data[i-1][j-1];
                    }
                }
                foreach (var entity in excelColorEntities)
                {
                    Range range = worksheet.Range[worksheet.Cells[entity.RowIndex, entity.ColumnIndex], worksheet.Cells[entity.RowIndex, entity.ColumnIndex]];
                    range.Interior.Color = entity.XlRgbColor;
                }
                worksheet.SaveAs(filename, fileFormat, Type.Missing, Type.Missing, Type.Missing, Type.Missing,Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                //worksheet.SaveAs(filename, fileFormat, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,Type.Missing,Type.Missing,Type.Missing);
            }
            else
            {
                result = false;
            }
            workbook.Close();
            xlApp.Quit();
            return result;
        }

        /// <summary>
        /// wether the filepath is excel doc,true means it's excel doc,false means it's not excel
        /// </summary>
        /// <param name="filePath">the file path</param>
        /// <returns>bool</returns>
        public static bool IsExcel(string filePath)
        {
            var extension = FileHelper.GetFileExtension(filePath);
            var shouldExtension = ConfigurationSettings.AppSettings.Get("exceltype");

            if (string.IsNullOrEmpty(extension) || !shouldExtension.Contains(extension))
            {
                return false;
            }
            return true;
        }

        ~ExcelHelper()
        {
            
        }

        public void Dispose()
        {
            if (Workbook != null)
            {
                Workbook.Close();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(Workbook);
            }
            
            Sheets = null;
            Workbook = null;
            if (Application!=null)
            {
                Application.Quit();
                Application = null;
            }
            GC.Collect();
        }
    }

    
}

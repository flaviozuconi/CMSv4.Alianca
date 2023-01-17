using Framework.DataLayer;
using Framework.Utilities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSv4.BusinessLayer
{
    public class BLFaleConoscoRelatorios
    {
        public static MemoryStream GerarExcel(string view, MLPortal portal)
        {
            MemoryStream stream = new MemoryStream();
            var query = "SELECT * FROM " + view + " order by 1 desc";
            using (var command = Database.NewCommand(string.Empty, portal.ConnectionString))
            {
                command.CommandText = query;

                var ds =  Database.ExecuteDataSet(command);

                if(ds.Tables.Count > 0)
                {
                    using (ExcelPackage excel = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("Planilha 1");
                        ExcelRange cells = worksheet.Cells;
                        int row = 1;

                        /*Formatação Generica*/
                        cells.Style.Font.Name = "Arial";
                        cells.Style.Font.Size = 10;

                        /*Cabeçalho*/
                        for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                        {
                            cells[row, (i + 1)].Value = ds.Tables[0].Columns[i].ColumnName;
                            cells[row, (i + 1)].Style.Font.Bold = true;
                            cells[row, (i + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cells[row, (i + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }

                        //for (int r = 0; r < ds.Tables[0].Rows.Count; r++)
                        foreach(DataRow r in ds.Tables[0].Rows)
                        {
                            row++;

                            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                            {
                                cells[row, (i + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                //Encontrar o atributo excelField na propriedade da model
                                //var excelField = (CsvField)propriedades[i].GetCustomAttributes(typeof(CsvField), true)[0];

                                //Obter o valor formatado de acordo com o tipo da propriedade da model
                                cells[row, (i + 1)].Value = r[i].ToString(); //excelFormater.GetValue(excelField, propriedades[i].GetValue(item, null));

                                //if (!string.IsNullOrEmpty(excelField.Format))
                                //    cells[row, (i + 1)].Style.Numberformat.Format = excelField.Format;

                            }
                        }


                        excel.Save();

                    }
                    

                }
            }
            return stream;

        }
    }
}

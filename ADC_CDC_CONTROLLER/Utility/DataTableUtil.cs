using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ADC_CDC_CONTROLLER
{
    static class DataTableUtil
    {
        static public DataTable DataTableInit(string colName, string[] columns, string rowName, string[] rows)
        {
            return DataTableInit("DataTable", colName, columns, rowName, rows);
        }

        static public DataTable DataTableInit(string tableName, string colName, string[] columns, string rowName, string[] rows)
        {
            DataTable dt = new DataTable(tableName);

            dt.Columns.Add(rowName + @"\" + colName);

            DataTableAddColumnsName(dt, columns);
            DataTableAddRowsName(dt, rows);

            return dt;
        }

        static public void DataTableAddColumnsName(DataTable dt, string[] columns)
        {
            foreach (string col in columns)
                dt.Columns.Add(col);
            List<string> colNames = new List<string>();
        }

        static public void DataTableAddRowsName(DataTable dt, string[] rows)
        {
            DataTableAddRowsName(dt, rows, false);
        }
        static public void DataTableAddRowsName(DataTable dt, string[] rows, bool isSorted)
        {
            foreach (string row in rows)
            {
                DataRow dr = dt.NewRow();
                dr[0] = row;
                dt.Rows.Add(dr);
            }
            if (isSorted)
            {
                string firstColumnName = dt.Columns[0].ColumnName;
                dt = dt.Clone().Rows.Cast<DataRow>().OrderBy(r => Convert.ToDecimal((string)r[firstColumnName])).CopyToDataTable();
            }
        }

        static public void DataTableAddData(DataTable dt, int columnIndex, int rowIndex, string data)
        {
            dt.Rows[rowIndex][columnIndex + 1] = data;
        }

        static public void DataTableAddData(DataTable dt, string column, string row, string data)
        {
            string firstColumnName = dt.Columns[0].ColumnName;
            DataRow dr = dt.Select("[" + firstColumnName + "]='" + row + "'").First();
            dr[column] = data;
        }

        public static string DatatableToCSV(string info, DataTable dt, string pathFile, string colSplitStr=",", string rowSplitStr = "\r\n")
        {
            string strLine = "";
            StringBuilder sb;
            try
            {
                sb = new StringBuilder();

                if (!string.IsNullOrEmpty(info))
                    sb.AppendLine(info);

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i > 0)
                        strLine += colSplitStr;
                    strLine += dt.Columns[i].ColumnName;
                }
                strLine.Remove(strLine.Length - 1);
                strLine += rowSplitStr;
                sb.Append(strLine);
                strLine = "";

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    strLine = "";
                    int colCount = dt.Columns.Count;
                    for (int k = 0; k < colCount; k++)
                    {
                        if (k > 0 && k < colCount)
                            strLine += colSplitStr;
                        if (dt.Rows[j][k] == null)
                            strLine += "";
                        else
                        {
                            string cell = dt.Rows[j][k].ToString().Trim();

                            cell = cell.Replace("\"", "\"\"");
                            cell = "\"" + cell + "\"";
                            strLine += cell;
                        }
                    }
                    strLine += rowSplitStr;
                    sb.Append(strLine);
                    strLine = "";
                }

                if (!string.IsNullOrEmpty(pathFile))
                    File.WriteAllText(pathFile, sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
            return sb.ToString();
        }
    }
}

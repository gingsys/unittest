using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Configuration;
using SisConAxs_DM.Repository;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;

namespace SisConAxs_DM.Repository.Util
{
    public class ExcelReader
    {
        SisConAxsContext db = new SisConAxsContext();

        public DataTable fillDtFromExcel(string filePath, string fileName, string sheetName)
        {
            string connectionString = "";
            switch (Path.GetExtension(fileName))
            {
                case ".xls": //Excel 97-03
                    connectionString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                    break;
                case ".xlsx": //Excel 07
                    connectionString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                    break;
            }
            connectionString = String.Format(connectionString, filePath + @"/" + fileName);
            OleDbConnection connection = new OleDbConnection(connectionString);
            // Get the name of First Sheet
            connection.Open();
            DataTable dtExcelSchema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            connection.Close();

            // validate sheetName
            if (!dtExcelSchema.Select().Any(x => x["TABLE_NAME"].ToString() == sheetName))
                return null;
            // Read Data from selected sheet
            OleDbDataAdapter oda = new OleDbDataAdapter("SELECT * From [" + sheetName + "]", connection);
            DataTable dt = new DataTable();
            oda.Fill(dt);
            // Format column names
            foreach (DataColumn col in dt.Columns)
                col.ColumnName = col.ColumnName.Trim().ToLower();
            return dt;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using PaoloTestMS.Entities.ViewModel;
using PaoloTestMS.Services.Interfaces;
using PaoloTestMS.Services.Extensions;

namespace PaoloTestMS.Services
{
    public class DownloadService : IDownloadService
    {
        private const string _truncateLiveTableCommandText = @"TRUNCATE TABLE [dotnet].[TestLoadTrx]";
        private const int _batchSize = 1000;
        private List<string> mappedColumns = new List<string> { "Period", "Outlet_Id", "Source_Provider", "Channel", "IMA_Sector", "Transaction_Type", "Supplied_Value" };

    #region public function - interface implementation

    /// <summary>
    /// Retrieve the AuditID and time elapsed
    /// these infos will be used to get back the Audit in the details view
    /// </summary>
    /// <param name="fileFullPath"></param>
    /// <param name="filename"></param>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public Result Download(string fileFullPath, string filename, string connectionString)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var result = new Result();

            var errorMessage = "";

            var filePath = fileFullPath;
            
            try
            {
                var createdCount = 0;
                
                using (var textFieldParser = new TextFieldParser(filePath))
                {
                    textFieldParser.TextFieldType = FieldType.Delimited;
                    textFieldParser.Delimiters = new[] { "|" };
                    textFieldParser.HasFieldsEnclosedInQuotes = false;

                    var dataTable = new DataTable("data");
                    
                    using (var sqlConnection = new SqlConnection(connectionString))
                    {
                        sqlConnection.Open();

                        // Truncate the live table
                        using (var sqlCommand = new SqlCommand(_truncateLiveTableCommandText, sqlConnection))
                        {
                            sqlCommand.ExecuteNonQuery();
                        }

                        // Create the bulk copy object
                        var sqlBulkCopy = new SqlBulkCopy(sqlConnection)
                        {
                            DestinationTableName = "[dotnet].[TestLoadTrx]"
                        };

                        // Setup the column mappings, anything ommitted is skipped
                        mappedColumns.ForEach(mc => sqlBulkCopy.ColumnMappings.Add(mc, mc));

                        var firstLine = true;

                        // Loop through the CSV and load each set of 1,000 records into a DataTable
                        // Then send it to the LiveTable
                        while (!textFieldParser.EndOfData)
                        {
                            string[] fields = textFieldParser.ReadFields();

                            if (firstLine)
                            {
                                foreach (var val in fields)
                                {
                                    dataTable.Columns.Add(val);
                                }

                                firstLine = false;

                                continue;
                            }

                            dataTable.Rows.Add(fields);

                            dataTable.Rows[createdCount].ItemArray[0] = Convert.ToInt32(dataTable.Rows[createdCount].ItemArray[0]);
                            dataTable.Rows[createdCount].ItemArray[1] = Convert.ToInt32(dataTable.Rows[createdCount].ItemArray[1]);
                            dataTable.Rows[createdCount].ItemArray[5] = Convert.ToInt16(dataTable.Rows[createdCount].ItemArray[5]);
                            dataTable.Rows[createdCount].ItemArray[6] = Convert.ToDecimal(dataTable.Rows[createdCount].ItemArray[6]);

                            createdCount++;

                            if (createdCount % _batchSize == 0)
                            {
                                createdCount = 0;
                                InsertDataTable(sqlBulkCopy, sqlConnection, dataTable);
                            }
                        }

                        InsertDataTable(sqlBulkCopy, sqlConnection, dataTable);

                        stopwatch.Stop();

                        sqlConnection.Close();
                    }
                }
            }
            catch (Exception)
            {
                errorMessage = "something went wrong";
            }
            finally
            {
                var timeElapsed = stopwatch.Elapsed.TotalSeconds;

                if(!string.IsNullOrEmpty(errorMessage))
                {
                    result = new Result
                    {
                        AuditID = -1,
                        TimeElapsed = timeElapsed
                    };
                }
                else
                    result = GetResult(connectionString, timeElapsed, filename, errorMessage);
            }

            return result;
        }

        /// <summary>
        /// Get back the previously created Audit
        /// </summary>
        /// <param name="auditID"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public LoadAudit GetAudit(int auditID, string connectionString)
        {
            var audit = new LoadAudit();

            var ds = new DataSet("Audit");

            if(auditID != -1)
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    var cmd = new SqlCommand("[dotnet].[sp_GetAudit]", conn);

                    cmd.Parameters.Add("@AuditID", SqlDbType.Int).Value = auditID;

                    cmd.CommandType = CommandType.StoredProcedure;

                    var da = new SqlDataAdapter();
                    da.SelectCommand = cmd;

                    da.Fill(ds);
                    var dataRow = ds.Tables[0].Rows[0];

                    audit = new LoadAudit
                    {
                        AuditID = Convert.ToInt32(dataRow["AuditID"].ToString()),
                        Client = dataRow["Client"].ToString(),
                        ErrorMessage = dataRow["ErrorMessage"].ToString(),
                        IsSuccess = Convert.ToBoolean(dataRow["IsSuccess"].ToString()),
                        RowCount = Convert.ToInt32(dataRow["RowCount"].ToString()),
                        TotalValue = Convert.ToDecimal(dataRow["Total_Value"].ToString()),
                        Year = Convert.ToInt32(dataRow["DataYear"].ToString())
                    };
                }
            }
            else
            {
                return new LoadAudit
                {
                    IsSuccess = false,
                    AuditID = auditID,
                    ErrorMessage = "something went wrong"
                };
            }

            return audit;
        }

        #endregion

        #region private function

        private Result GetResult(string connectionString, double timeElapsed, string filename, string errorMessage)
        {
            var result = new Result();

            var ds = new DataSet("Audit");
            
            using (var conn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("[dotnet].[sp_CreateAudit]", conn);

                cmd.Parameters.Add("@Client", SqlDbType.VarChar).Value = filename.ToClient();
                cmd.Parameters.Add("@DataYear", SqlDbType.SmallInt).Value = filename.ToYear();
                cmd.Parameters.Add("@IsSuccess", SqlDbType.Bit).Value = true;
                cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar).Value = errorMessage;

                cmd.CommandType = CommandType.StoredProcedure;

                var da = new SqlDataAdapter();
                da.SelectCommand = cmd;

                da.Fill(ds);

                var dataRow = ds.Tables[0].Rows[0];

                result = new Result
                {
                    AuditID = Convert.ToInt32(dataRow["AuditID"].ToString()),
                    TimeElapsed = timeElapsed
                };
            }

            return result;
        }

        private void InsertDataTable(SqlBulkCopy sqlBulkCopy, SqlConnection sqlConnection, DataTable dataTable)
        {
            sqlBulkCopy.WriteToServer(dataTable);

            dataTable.Rows.Clear();
        }

        #endregion
    }
}

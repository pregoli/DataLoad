using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace PaoloTestMS.Configuration
{
    public static class Configuration
    {
        public static class DirectorySettings
        {
            private static string directoryFilesPath;

            /// <summary>
            /// Get The directory path containing the files
            /// </summary>
            public static string DirectoryFilesPath
            {
                get
                {
                    if(string.IsNullOrEmpty(directoryFilesPath))
                        directoryFilesPath = ConfigurationManager.AppSettings["directoryFilesPath"];

                    return directoryFilesPath;
                }
            }

            private static string connectionString;

            /// <summary>
            /// Get the connectionstring
            /// </summary>
            public static string ConnectionString
            {
                get
                {
                    if (string.IsNullOrEmpty(connectionString))
                        connectionString = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;

                    return connectionString;
                }
            }
        }
    }
}
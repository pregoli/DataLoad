using System;
using System.Text.RegularExpressions;

namespace PaoloTestMS.Services.Extensions
{
    public static class FilesExtension
    {
        /// <summary>
        /// Get a human readable file size
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToFileSize(this long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);

                max /= scale;
            }
            return "0 Bytes";
        }

        /// <summary>
        /// Check if the filename is valid and metches with the standard pattern
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool IsValidFilename(this string filename)
        {
            var regex = new Regex(@"\d+");
            return regex.Match(filename).Success;
        }

        /// <summary>
        /// Take the client name from filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string ToClient(this string filename)
        {
            var underscoreIndex = filename.IndexOf('_');

            return filename.Substring(0, underscoreIndex);
        }

        /// <summary>
        /// Take the year from filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static int ToYear(this string filename)
        {
            var underscoreIndex = filename.IndexOf('_') + 1;

            return Convert.ToInt32(filename.Substring(underscoreIndex, 4));
        }
    }
}

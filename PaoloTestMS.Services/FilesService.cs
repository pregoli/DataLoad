using System.Collections.Generic;
using System.IO;
using System.Linq;
using PaoloTestMS.Entities.ViewModel;
using PaoloTestMS.Services.Extensions;
using PaoloTestMS.Services.Interfaces;

namespace PaoloTestMS.Services
{
    public class FilesService : IFilesService
    {
        public IEnumerable<DirectoryFile> GetDirectoryFiles(string path)
        {
            var dirinfo = new DirectoryInfo(path);
            return from FileInfo fileinfo in dirinfo.GetFiles()
                   orderby fileinfo.Length descending
                   select new DirectoryFile
                   {
                       IsValid = fileinfo.Name.IsValidFilename(),
                       Name = fileinfo.Name,
                       Size = fileinfo.Length.ToFileSize(),
                       ClientID = fileinfo.Name.IsValidFilename() ? fileinfo.Name.ToClient() : "invalid",
                       Year = fileinfo.Name.IsValidFilename() ? fileinfo.Name.ToYear() : 0,
                       LastModified = fileinfo.LastWriteTime
                   };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaoloTestMS.Entities.ViewModel;

namespace PaoloTestMS.Services.Interfaces
{
    public interface IFilesService
    {
        IEnumerable<DirectoryFile> GetDirectoryFiles(string path);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaoloTestMS.Entities.ViewModel
{
    public class DirectoryFile
    {
        public string Name { get; set; }

        public string Size { get; set; }

        public string ClientID { get; set; }

        public int Year { get; set; }

        public bool IsValid { get; set; }

        public DateTime LastModified { get; set; }
    }
}

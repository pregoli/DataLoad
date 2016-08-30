using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaoloTestMS.Entities.ViewModel
{
    public class LoadAudit
    {
        public int AuditID { get; set; }

        public string Client { get; set; }

        public int Year { get; set; }

        public int RowCount { get; set; }

        public decimal TotalValue { get; set; }

        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }

        public double TimeElapsed { get; set; }
    }
}

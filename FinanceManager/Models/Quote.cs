using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class Quote
    {
        public string NomeInvestitore { get; set; }
        public double Ammontare { get; set; }
        public double Quota { get; set; }
        public double Totale { get; set; }
    }
}

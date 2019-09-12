using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class QuoteGuadagno
    {
        public string Nome { get; set; }
        public int Anno { get; set; }
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }
        public double QuotaInv { get; set; }
        public double TotaleCedole { get; set; }
        public double TotaleUtili { get; set; }
        public double TotaleVolatili { get; set; }
        public double TotaleGuadagno { get; set; }
    }
}

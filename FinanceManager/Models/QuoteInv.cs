using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class QuoteInv
    {
        public string NomeInvestitore { get; set; }
        public double CapitaleImmesso { get; set; }
        public double TotaleImmesso { get; set; }
        public double CapitalePrelevato { get; set; }
        public double TotalePrelevato { get; set; }
        public double CapitaleAttivo { get; set; }
        public double TotaleAttivo { get; set; }
        public double QuotaInv { get; set; }
        public double CapitaleAssegnato { get; set; }
        public double TotaleAssegnato { get; set; }
        public double CapitaleDisponibile { get; set; }
        public double TotaleDisponibile { get; set; }
    }
}

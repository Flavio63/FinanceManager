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
        public double CapitaleVersato { get; set; }
        public double TotaleVersato { get; set; }
        public double QuotaVersato { get; set; }
        public double CapitalePrelevato { get; set; }
        public double TotalePrelevato { get; set; }
        public double QuotaPrelevato { get; set; }
        public double CapitaleDisinvestito { get; set; }
        public double TotaleDisinvestito { get; set; }
        public double QuotaDisinvestito { get; set; }
        public double CapitaleInvestito { get; set; }
        public double TotaleInvestito { get; set; }
        public double QuotaInvestito { get; set; }
        public double CapitaleDisponibile { get; set; }
        public double TotaleDisponibile { get; set; }
        public double QuotaDisponibile { get; set; }
    }
}

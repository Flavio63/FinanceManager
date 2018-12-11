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
        public double Investito { get; set; }
        public double Quota { get; set; }
        public double Totale { get; set; }
        public double Disponibili { get; set; }
        public double TotDisponibile { get; set; }
        public double Guadagno { get; set; }
        public double QuotaGuadagno { get; set; }
        public double GuadagnoTotale { get; set; }
        public double Cedole { get; set; }
        public double Utili { get; set; }
    }
}

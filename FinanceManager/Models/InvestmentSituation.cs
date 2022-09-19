using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class InvestmentSituation
    {
        public int IdGestione { get; set; }
        public string Socio { get; set; }
        public int IdValuta { get; set; }
        public string CodValuta { get; set; }
        public double Versato { get; set; }
        public double Investito { get; set; }
        public double Disinvestito { get; set; }
        public double Prelevato { get; set; }
        public double Disponibile { get; set; }
    }
}

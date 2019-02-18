using FinanceManager.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class ReportProfitLoss : ViewModelBase
    {
        public int Anno { get; set; }
        public string Gestione { get; set; }
        public string TipoSoldi { get; set; }
        public double Azioni { get; set; }
        public double Obbligazioni { get; set; }
        public double ETF { get; set; }
        public double Fondo { get; set; }
        public double Volatili { get; set; }
        public double Totale { get; set; }
    }
}

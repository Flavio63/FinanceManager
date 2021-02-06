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
        public string Valuta { get; set; }
        public string TipoSoldi { get; set; }
        public string NomeTitolo { get; set; }
        public string ISIN { get; set; }
        public double Azioni { get; set; }
        public double Obbligazioni { get; set; }
        public double Certificati { get; set; }
        public double ETF_C_P { get; set; }
        public double Fondo { get; set; }
        public double Futures { get; set; }
        public double Opzioni { get; set; }
        public double Commodities { get; set; }
        public double Costi { get; set; }
        public double Totale { get; set; }
    }
}

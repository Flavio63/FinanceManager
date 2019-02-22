using FinanceManager.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class ReportMovementDetailed : ViewModelBase
    {
        public string Gestione { get; set; }
        public string Conto { get; set; }
        public string Movimento { get; set; }
        public string Tipo_Titolo { get; set; }
        public string Nome_Titolo { get; set; }
        public string Isin { get; set; }
        public string Tipo_Soldi { get; set; }
        public DateTime DataMovimento { get; set; }
        public double Uscite { get; set; }
        public double Entrate { get; set; }
        public string Causale { get; set; }
    }
}

using FinanceManager.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class ReportTitoliAttivi : ViewModelBase
    {
        public string Gestione { get; set; }
        public string Conto { get; set; }
        public string Movimento { get; set; }
        public string Tipo_Titolo { get; set; }
        public string Nome_Titolo { get; set; }
        public string Isin { get; set; }
        public double N_Titoli { get; set; }
        public double ValoreAcquisto { get; set; }
        public double CMC { get; set; }
        public string Note { get; set; }
    }
}

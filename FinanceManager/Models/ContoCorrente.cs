using FinanceManager.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class ContoCorrente : ViewModelBase
    {
        public int Id_RowConto { get; set; }
        public int Id_Conto { get; set; }
        public int Id_Quote_Investimenti { get; set; }
        public int Id_Valuta { get; set; }
        public int Id_Portafoglio_Titoli { get; set; }
        public int Id_Tipo_Movimento { get; set; }
        public int Id_Gestione { get; set; }
        public int Id_Titolo { get; set; }
        public DateTime Data_Movimento { get; set; }
        public double Ammontare { get; set; }
        public double Valore_Cambio { get; set; }
        public string Causale { set; get; }
    }
}

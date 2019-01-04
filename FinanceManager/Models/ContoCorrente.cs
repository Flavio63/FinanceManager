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
        public string Desc_Conto { get; set; }
        public int Id_Quote_Investimenti { get; set; }
        public int Id_Valuta { get; set; }
        public string Cod_Valuta { get; set; }
        public int Id_Portafoglio_Titoli { get; set; }
        public int Id_tipo_movimento { get; set; }
        public string Desc_tipo_movimento { get; set; }
        public int Id_Gestione { get; set; }
        public string NomeGestione { get; set; }
        public int Id_Titolo { get; set; }
        public string Desc_Titolo { get; set; }
        public string ISIN { get; set; }
        public DateTime DataMovimento { get; set; }
        public double Ammontare { get; set; }
        public double Valore_Cambio { get; set; }
        public string Causale { set; get; }
        public int Id_Tipo_Soldi { set; get; }
        public string Desc_Tipo_Soldi { set; get; }
    }
}

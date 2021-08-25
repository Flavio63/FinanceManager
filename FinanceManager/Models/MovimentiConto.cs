using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class MovimentiConto
    {
        public int Id_Riga_Conto { get; set; }
        public string Desc_Conto { get; set; }
        public string Nome_Gestione { get; set; }
        public string Desc_Movimento { get; set; }
        public string Desc_TipoTitolo { get; set; }
        public string Desc_Titolo { get; set; }
        public string Isin { get; set; }
        public string Desc_Valuta { get; set; }
        public DateTime DataMovimento { get; set; }
        public double Entrate { get; set; }
        public double Uscite { get; set; }
        public double Cumulato { get; set; }
        public string Causale { get; set; }
        public string Desc_Tipo_Soldi { get; set; }
    }
}

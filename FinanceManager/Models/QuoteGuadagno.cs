using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class QuoteGuadagno
    {
        public int id_quota_guadagno { get; set; }
        public int id_socio { get; set; }
        public string nome_socio { get; set; }
        public int id_quote_periodi { get; set; }
        public DateTime data_inizio { get; set; }
        public DateTime data_fine { get; set; }
        public double ammontare { get; set; }
        public double cum_socio { get; set; }
        public double cum_totale { get; set; }
        public double quota { get; set; }
        public int id_conto_corrente { get; set; }
        public int id_valuta { get; set; }
        public string cod_valuta { get; set; }
        public int id_tipo_gestione { get; set; }
        public string tipo_gestione { get; set; }
        public QuoteGuadagno()
        {
            ammontare = 0;
            cum_socio = 0;
        }
    }
}

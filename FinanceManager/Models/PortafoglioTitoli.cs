using FinanceManager.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class PortafoglioTitoli : ViewModelBase
    {
        public int Id_portafoglio { get; set; }
        public int Id_gestione { get; set; }
        public string Nome_Gestione { get; set; }

        public int Id_Conto { get; set; }
        public string Desc_Conto { get; set; }

        public int Id_valuta { get; set; }
        public string Cod_valuta { get; set; }

        public int Id_tipo_movimento { get; set; }
        public string Desc_tipo_movimento { get; set; }

        public uint Id_azienda { get; set; }
        public string Desc_azienda { get; set; }

        public uint? Id_titolo { get; set; }
        public string Desc_titolo { get; set; }
        public string Isin { get; set; }

        public uint Id_tipo_titolo { get; set; }
        public string Desc_tipo_titolo { get; set; }

        public double N_titoli { get; set; }
        public double Costo_unitario_in_valuta { get; set; }
        public double Commissioni_totale { get; set; }
        public double TobinTax { get; set; }
        public double Disaggio_anticipo_cedole { get; set; }
        public double RitenutaFiscale { get; set; }
        public DateTime Data_Movimento { get; set; }
        public double Importo_totale { get; set; }
        public double Valore_di_cambio { get; set; }
        public double Importo_cambiato { get; set; }
        public bool Available { get; set; }
        public string Note { get; set; }
        public int Attivo { get; set; }
        public int Id_Tipo_Gestione { get; set; }
        public string Tipo_Gestione { get; set; }
        public DateTime Link_Movimenti { get; set; }

        public PortafoglioTitoli()
        {
            Id_gestione = 0;
            Nome_Gestione = "";
            Id_valuta = 0;
            Cod_valuta = "";
            Id_tipo_movimento = 0;
            Desc_tipo_movimento = "";
            Id_azienda = 0;
            Desc_azienda = "";
            Id_titolo = 0;
            Desc_titolo = "";
            Isin = "";
            Id_tipo_titolo = 0;
            Desc_tipo_titolo = "";
            N_titoli = 0;
            Costo_unitario_in_valuta = 0;
            Commissioni_totale = 0;
            TobinTax = 0;
            Disaggio_anticipo_cedole = 0;
            RitenutaFiscale = 0;
            Data_Movimento = DateTime.Now.Date;
            Importo_totale = 0;
            Valore_di_cambio = 1;
            Importo_cambiato = 0;
            Attivo = 1;
            Note = "";
            Link_Movimenti = DateTime.Now;
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

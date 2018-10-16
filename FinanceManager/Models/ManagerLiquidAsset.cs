using FinanceManager.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class ManagerLiquidAsset : ViewModelBase
    {
        private DateTime _MovementDate;

        public int Id_portafoglio { get; set; }
        public int Id_gestione { get; set; }
        public string Nome_Gestione { get; set; }

        public int Id_conto { get; set; }
        public string Desc_conto { get; set; }

        public int Id_valuta { get; set; }
        public string Cod_valuta { get; set; }

        public int Id_valuta_2 { get; set; }
        public string Code_valuta_2 { get; set; }

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
        public double ProfitLoss { get; set; }

        public DateTime Data_Movimento
        {
            get
            {
                if (_MovementDate.Date.ToShortDateString() == "01/01/0001")
                    _MovementDate = DateTime.Now.Date;
                return _MovementDate;
            }
            set
            {
                if (value.Date.ToShortDateString() == "01/01/0001")
                    _MovementDate = DateTime.Now.Date;
                _MovementDate = value;
            }
        }
        //[ExcludeChar("/[a-z][A-Z]!@#$£€", ErrorMessage = "Sono permessi solo numeri")]
        //[VerifyNumber(ErrorMessage = "Inserire una cifra diversa da zero e che sia disponibile.")]
        public double Importo_totale
        {
            get { return GetValue(() => Importo_totale); }
            set { SetValue(() => Importo_totale, value); }
        }
        //[Range(0.2, 1.8, ErrorMessage = "Controllare la cifra immessa.")]
        public double Valore_di_cambio
        {
            get { return GetValue(() => Valore_di_cambio); }
            set { SetValue(() => Valore_di_cambio, value); }
        }
        public double Importo_cambiato { get; set; }
        public bool Available { get; set; }
        public string Note { get; set; }
    }
}

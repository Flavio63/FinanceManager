using FinanceManager.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class Ptf_CC : ViewModelBase
    {
        // Campi derivati dal Portafoglio Titoli
        public int Id_portafoglio_titoli { get; set; }
        public int Id_gestione { get; set; }
        public int Id_Conto { get; set; }
        public int Id_valuta { get; set; }
        public int Id_tipo_movimento { get; set; }
        public int Id_titolo { get; set; }
        public DateTime Data_Movimento
        {
            get { return GetValue(() => Data_Movimento); }
            set
            {
                if (value.Date.ToShortDateString() == "01/01/0001" || value.Date.ToShortDateString() == "01/01/01")
                    SetValue(() => Data_Movimento, DateTime.Now.Date);
                else
                    SetValue(() => Data_Movimento, value);
            }
        }
        public double ValoreAzione
        {
            get { return GetValue(() => ValoreAzione); }
            set { SetValue(() => ValoreAzione, value); }
        }
        public double N_titoli { get; set; }
        public double Valore_unitario_in_valuta { get; set; }
        public double Commissioni_totale { get; set; }
        public double TobinTax { get; set; }
        public double Disaggio_anticipo_cedole { get; set; }
        public double RitenutaFiscale { get; set; }
        public double Valore_di_cambio
        {
            get { return GetValue(() => Valore_di_cambio); }
            set { SetValue(() => Valore_di_cambio, value); }
        }
        public string Note { get; set; }
        // Campi derivati dal Conto Corrente
        public int Id_RowConto { get; set; }
        public double Valore_in_CC { get; set; }
        public int Id_Tipo_Soldi { set; get; }
    }
}

using FinanceManager.Events;
using FinanceManager.Models.Enum;
using System;

namespace FinanceManager.Models
{
    public class ContoCorrente : ViewModelBase
    {
        private DateTime _MovementDate;

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
        public DateTime DataMovimento
        {
            get
            {
                if (_MovementDate.Date.ToShortDateString() == "01/01/0001" || _MovementDate.Date.ToShortDateString() == "01/01/01")
                    _MovementDate = DateTime.Now.Date;
                return _MovementDate;
            }
            set
            {
                //if (value.Date.ToShortDateString() == "01/01/0001" || value.Date.ToShortDateString() == "01/01/01")
                //    _MovementDate = DateTime.Now.Date;
                _MovementDate = value;
            }
        }
        public double Ammontare { get; set; }
        public double Valore_Cambio { get; set; }
        public string Causale { set; get; }
        public int Id_Tipo_Soldi { set; get; }
        public string Desc_Tipo_Soldi { set; get; }

        /// <summary>
        /// Inizializza un record vuoto di ContoCorrente
        /// </summary>
        public ContoCorrente() { }

        /// <summary>
        /// Inizializza un record di ContoCorrente prendendo
        /// i campi principali dal record di PortafoglioTitoli
        /// specificando il valore dell'operazione per il c/c e la tipologia di soldi
        /// </summary>
        /// <param name="portafoglioTitoli">Record di PortafoglioTitoli</param>
        /// <param name="valoreInCC">Valore da registrare in c/c</param>
        /// <param name="idTipoSoldi">Destinazione d'uso dei soldi</param>
        public ContoCorrente(PortafoglioTitoli portafoglioTitoli, double valoreInCC, TipologiaSoldi idTipoSoldi)
        {
            Id_Conto = portafoglioTitoli.Id_conto;
            Id_Quote_Investimenti = 0;
            Id_Valuta = portafoglioTitoli.Id_valuta;
            Id_Portafoglio_Titoli = portafoglioTitoli.Id_portafoglio;
            Id_tipo_movimento = portafoglioTitoli.Id_tipo_movimento;
            Id_Gestione = portafoglioTitoli.Id_gestione;
            Id_Titolo = (int)portafoglioTitoli.Id_titolo;
            DataMovimento = portafoglioTitoli.Data_Movimento;
            Ammontare = valoreInCC;
            Valore_Cambio = portafoglioTitoli.Valore_di_cambio;
            Causale = portafoglioTitoli.Note;
            Id_Tipo_Soldi = (int)idTipoSoldi;
        }
    }
}

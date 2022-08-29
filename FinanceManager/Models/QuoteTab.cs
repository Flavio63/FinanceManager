using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class QuoteTab
    {
        private DateTime _MovementDate;

        public int Id_Quote_Investimenti { get; set; }
        public int Id_Gestione { get; set; }
        public string NomeInvestitore { get; set; }
        public int Id_tipo_movimento { get; set; }
        public string Desc_tipo_movimento { get; set; }
        public int Id_Periodo_Quote { get; set; }
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
                if (value.Date.ToShortDateString() == "01/01/0001" || _MovementDate.Date.ToShortDateString() == "01/01/01")
                    _MovementDate = DateTime.Now.Date;
                _MovementDate = value;
            }
        }
        public int Id_Valuta { get; set; }
        public string CodeCurrency { get; set; }
        public double ChangeValue { get; set; }
        public double AmmontareEuro { get; set; }
        public double AmmontareValuta { get; set; }
        public string Note { get; set; }
    }
}

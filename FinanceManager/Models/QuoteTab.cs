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

        public int IdQuote { get; set; }
        public int IdGestione { get; set; }
        public string NomeInvestitore { get; set; }
        public int Id_tipo_movimento { get; set; }
        public string Desc_tipo_movimento { get; set; }
        public int Id_Periodo_Quote { get; set; }
        public int Anno { get; set; }
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
        public double Ammontare { get; set; }
        public string Note { get; set; }
    }
}

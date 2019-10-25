using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class GuadagnoPerQuote
    {
        private DateTime _MovementDate;
        public int IdGuadagno { get; set; }
        public int Anno { get; set; }
        public int IdGestione { get; set; }
        public string Nome { get; set; }
        public int IdTipoMovimento { get; set; }
        public string DescTipoSoldi { get; set; }
        public DateTime DataOperazione 
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
        public double QuotaInv { get; set; }
        public double Guadagno { get; set; }
        public double Preso { get; set; }
        public double In_Cassa { get; set; }
        public string Causale { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class GuadagnoPerQuote
    {
        public int Anno { get; set; }
        public string Nome { get; set; }
        public string DescTipoSoldi { get; set; }
        public DateTime DataOperazione { get; set; }
        public double QuotaInv { get; set; }
        public double Guadagno { get; set; }
        public double Preso { get; set; }
        public double In_Cassa { get; set; }
    }
}

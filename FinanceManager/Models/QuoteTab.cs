using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class QuoteTab
    {
        public int IdQuote { get; set; }
        public int IdInvestitore { get; set; }
        public string NomeInvestitore { get; set; }
        public int IdMovimento { get; set; }
        public string Movimento { get; set; }
        public DateTime DataMovimento { get; set; }
        public double Ammontare { get; set; }
        public string Note { get; set; }
    }
}

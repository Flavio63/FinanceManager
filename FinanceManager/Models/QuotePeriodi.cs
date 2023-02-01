using System;

namespace FinanceManager.Models
{
    public class QuotePeriodi
    {
        public int IdPeriodoQuote { get; set; }
        public int IdTipoGestione { get; set; }
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }
        public DateTime DataInsert { get; set; }
    }
}

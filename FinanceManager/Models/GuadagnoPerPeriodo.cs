using FinanceManager.Events;
using System;

namespace FinanceManager.Models
{
    public class GuadagnoPerPeriodo : ViewModelBase
    {
        public int IdGestione { get; set; }
        public string Gestione { get; set; }
        public string Valuta { get; set; }
        public string Mese { get; set; }
        public double GuadagnoAnno1 { get; set; }
        public double GuadagnoAnno2 { get; set; }
        public double Differenza { get; set; }
        public double Delta { get; set; }
    }
}

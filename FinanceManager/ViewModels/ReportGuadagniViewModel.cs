using FinanceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.ViewModels
{
    public class ReportGuadagniViewModel
    {
        public ReportGuadagniViewModel(GuadagnoPerQuoteList Dettaglio, GuadagnoPerQuoteList Sintesi, GuadagnoPerQuoteList SuperSintesi)
        {
            this.Dettaglio = Dettaglio;
            this.Sintesi = Sintesi;
            this.SuperSintesi = SuperSintesi;
        }

        public GuadagnoPerQuoteList Dettaglio { get; set; }
        public GuadagnoPerQuoteList Sintesi { get; set; }
        public GuadagnoPerQuoteList SuperSintesi { get; set; }
    }
}

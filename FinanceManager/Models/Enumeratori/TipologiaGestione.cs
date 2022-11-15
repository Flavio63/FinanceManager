using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models.Enumeratori
{
    public enum TipologiaGestione
    {
        [Display(Name = "Base quote investimento")]
        BaseQuote = 1,
        [Display(Name = "Al 50% (-%Aurora)")]
        Base50 = 2,
    }
}

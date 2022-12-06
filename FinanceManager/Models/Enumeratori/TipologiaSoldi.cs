
using System.ComponentModel.DataAnnotations;

namespace FinanceManager.Models.Enumeratori
{
    public enum TipologiaSoldi
    {
        Nothing = 0,
        Capitale = 1,
        [Display(Name = "Perdita di Capitale")] PerditaCapitale = 11,
        [Display(Name = "Utili da Vendite")] Utili_da_Vendite = 15,
        [Display(Name = "Utili a Forfait")] Utili_da_Volatili = 16,
        [Display(Name = "Utili da Cedole")] Utili_da_Cedole = 17,
        [Display(Name = "Utili + Capitale")] Utili_Lordi = 18
    }
}

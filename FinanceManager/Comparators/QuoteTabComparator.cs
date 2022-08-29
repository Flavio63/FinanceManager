using FinanceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Comparators
{
    public class QuoteTabComparator : EqualityComparer<QuoteTab>
    {
        public override bool Equals(QuoteTab x, QuoteTab y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            return
                x.Id_Quote_Investimenti == y.Id_Quote_Investimenti &&
                x.Id_Gestione == y.Id_Gestione &&
                x.Id_tipo_movimento == y.Id_tipo_movimento &&
                x.AmmontareEuro == y.AmmontareEuro &&
                x.DataMovimento == y.DataMovimento &&
                x.Note == y.Note;
        }

        public override int GetHashCode(QuoteTab obj)
        {
            return
                obj.Id_Quote_Investimenti.GetHashCode() ^
                obj.Id_Gestione.GetHashCode() ^
                obj.Id_tipo_movimento.GetHashCode() ^
                obj.AmmontareEuro.GetHashCode() ^
                obj.DataMovimento.GetHashCode() ^
                obj.Note.GetHashCode();
        }
    }
}

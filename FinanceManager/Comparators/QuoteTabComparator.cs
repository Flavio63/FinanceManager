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
                x.IdQuote == y.IdQuote &&
                x.IdGestione == y.IdGestione &&
                x.Id_tipo_movimento == y.Id_tipo_movimento &&
                x.Ammontare == y.Ammontare &&
                x.DataMovimento == y.DataMovimento &&
                x.Note == y.Note;
        }

        public override int GetHashCode(QuoteTab obj)
        {
            return
                obj.IdQuote.GetHashCode() ^
                obj.IdGestione.GetHashCode() ^
                obj.Id_tipo_movimento.GetHashCode() ^
                obj.Ammontare.GetHashCode() ^
                obj.DataMovimento.GetHashCode() ^
                obj.Note.GetHashCode();
        }
    }
}

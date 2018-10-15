using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services.SQL
{
    public class ReportScripts
    {
        public static readonly string GetAvailableYears = "SELECT YEAR(data_movimento) AS anni FROM portafoglio GROUP BY anni ORDER BY anni DESC";

        public static readonly string GetProfitLoss1 = "SELECT YEAR(data_movimento) as Anno, A.id_valuta, C.cod_valuta, D.id_tipo_titolo, D.desc_tipo_titolo, A.id_tipo_movimento, B.desc_Movimento, " +
            "SUM(CASE WHEN A.id_valuta = 1 THEN profit_loss WHEN A.id_valuta = 2 THEN profit_loss * @EuroDollaro WHEN A.id_valuta = 3 " +
            "THEN profit_loss * @EuroSterlina WHEN A.id_valuta = 4 THEN profit_loss * @EuroFranchiSvizzeri END) AS PL " +
            "FROM portafoglio A, tipo_movimento B, valuta C, tipo_titoli D, titoli E " +
            "WHERE  A.id_tipo_movimento = B.id_tipo_movimento AND A.id_valuta = C.id_valuta AND A.id_titolo = E.id_titolo AND E.id_tipo_titolo = D.id_tipo_titolo " +
            "AND D.id_tipo_titolo > 0 AND ({0}) AND ({1}) AND ({2}) " +
            "GROUP BY Anno, cod_valuta, D.id_tipo_titolo, id_tipo_movimento " +
            "ORDER BY Anno, A.id_valuta, D.id_tipo_titolo, id_tipo_movimento; ";
    }
}

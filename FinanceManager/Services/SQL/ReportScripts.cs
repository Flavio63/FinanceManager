using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services.SQL
{
    public class ReportScripts
    {
        public static readonly string GetAvailableYears = "SELECT YEAR(data_movimento) AS anni FROM portafoglio_titoli GROUP BY anni ORDER BY anni DESC";

        public static readonly string GetProfitLoss1 = "SELECT YEAR(data_movimento) AS Anno, D.desc_tipo_soldi, " +
            "ROUND(SUM(CASE WHEN C.id_tipo_titolo = 1 THEN ammontare ELSE 0 END), 2) AS Azioni, " +
            "round(sum(case when C.id_tipo_titolo = 2 then ammontare else 0 end), 2) as Obbligazioni, " +
            "round(sum(case when C.id_tipo_titolo = 5 then ammontare else 0 end), 2) as ETF, " +
            "round(sum(case when C.id_tipo_titolo = 7 then ammontare else 0 end), 2) as Fondo, " +
            "round(sum(case when C.id_tipo_titolo = 13 then ammontare else 0 end), 2) as Volatili, " +
            "round(sum(case when C.id_tipo_titolo >= 1 then ammontare else 0 end), 2) as Totale " +
            "from conto_corrente A, gestioni B, titoli C, tipo_soldi D " +
            "where A.id_gestione = B.id_gestione and A.id_titolo = C.id_titolo and A.id_tipo_soldi = D.id_tipo_soldi AND " +
            "A.id_tipo_soldi > 1 AND {0} group by Anno, A.id_tipo_soldi";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services.SQL
{
    public class ReportScripts
    {
        public static readonly string GetAvailableYears = "SELECT YEAR(data_movimento) AS anni FROM conto_corrente WHERE (id_tipo_movimento = 4 or id_tipo_movimento = 6) GROUP BY anni ORDER BY anni DESC";

        public static readonly string GetProfitLoss = "SELECT YEAR(data_movimento) AS Anno, B.nome_gestione, D.desc_tipo_soldi, " +
            "ROUND(SUM(CASE WHEN C.id_tipo_titolo = 1 THEN ammontare ELSE 0 END), 2) AS Azioni, " +
            "round(sum(case when C.id_tipo_titolo = 2 THEN ammontare else 0 end), 2) AS Obbligazioni, " +
            "round(sum(case when C.id_tipo_titolo = 5 THEN ammontare else 0 end), 2) AS ETF, " +
            "round(sum(case when C.id_tipo_titolo = 7 THEN ammontare else 0 end), 2) AS Fondo, " +
            "round(sum(case when C.id_tipo_titolo = 13 or C.id_tipo_titolo = 4 THEN ammontare else 0 end), 2) AS Volatili, " +
            "round(sum(case when C.id_tipo_titolo >= 1 THEN ammontare else 0 end), 2) AS Totale " +
            "FROM conto_corrente A, gestioni B, titoli C, tipo_soldi D " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_titolo = C.id_titolo AND A.id_tipo_soldi = D.id_tipo_soldi AND " +
            "A.id_tipo_soldi > 1 AND {0} " +
            "GROUP BY Anno, A.id_gestione, A.id_tipo_soldi " +
            "ORDER BY  A.id_gestione, A.id_tipo_soldi DESC;";
    }
}

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

        public static readonly string GetMovementDetailed = "SELECT G.nome_gestione, B.desc_conto, C.desc_movimento, E.desc_tipo_titolo, D.desc_titolo, D.isin, F.desc_tipo_soldi, " +
            "data_movimento, ROUND(case when ammontare < 0 then ammontare ELSE 0 END , 2) AS uscite, ROUND (case when ammontare > 0 then ammontare ELSE 0 END, 2) AS entrate, causale " +
            "FROM conto_corrente A, conti B, tipo_movimento C, titoli D, tipo_titoli E, tipo_soldi F, gestioni G WHERE A.id_conto = B.id_conto AND A.id_tipo_movimento = C.id_tipo_movimento AND " +
            "A.id_titolo = D.id_titolo AND D.id_tipo_titolo = E.id_tipo_titolo AND A.id_tipo_soldi = F.id_tipo_soldi AND A.id_gestione = G.id_gestione " +
            "AND A.id_gestione = @id_gestione AND A.id_titolo = @id_titolo ORDER BY data_movimento DESC, A.id_titolo, A.id_tipo_soldi";

        public static readonly string GetActiveAsset = "SELECT nome_gestione, desc_conto, desc_tipo_titolo, desc_titolo, isin, n_titoli, rimanenze * -1 AS costoTotale, " +
            "ROUND(CASE WHEN id_tipo_titolo = 2 THEN (rimanenze* -1) / n_titoli* 100 ELSE(rimanenze* -1) / n_titoli END, 4) AS CMC, note FROM " +
            "( SELECT G.nome_gestione, B.desc_conto, E.id_tipo_titolo, E.desc_tipo_titolo, D.desc_titolo, D.isin, SUM(shares_quantity) AS n_titoli, " +
            "ROUND(sum(ammontare + (total_commission + tobin_tax + disaggio_cedole) * -1 ) , 2) AS rimanenze, note FROM portafoglio_titoli A, conti B, titoli D, tipo_titoli E, gestioni G " +
            "WHERE A.id_conto = B.id_conto AND A.id_titolo = D.id_titolo AND D.id_tipo_titolo = E.id_tipo_titolo AND  A.id_gestione = G.id_gestione AND " +
            "{0} AND {1} GROUP BY G.nome_gestione, B.desc_conto, E.desc_tipo_titolo, D.isin " +
            "ORDER BY A.id_gestione, A.id_conto, E.id_tipo_titolo, D.desc_titolo) AS AA WHERE n_titoli > 0";

    }
}

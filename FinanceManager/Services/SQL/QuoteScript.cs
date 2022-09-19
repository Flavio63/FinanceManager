using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services.SQL
{
    public class QuoteScript
    {
        /// <summary> Verifico se nella data di inserimento è già presente un investimento</summary>
        public static readonly string VerifyInvestmentDate = "SELECT IFNULL(A.data_insert, IFNULL(SUM(A.id_periodo_quote), -1)) AS result FROM quote_periodi A " +
            "WHERE A.id_aggregazione = @id_tipo_soldi AND A.data_inizio = @data_inizio; ";

        public static readonly string quote_periodi = "UPDATE quote_periodi SET data_fine = date(@StartDate, '-1 day') WHERE id_periodo_quote = (" +
    "SELECT quote_periodi.id_periodo_quote FROM quote_periodi WHERE quote_periodi.id_periodo_quote > 0 AND quote_periodi.id_aggregazione = @TipoSoldi " +
    "AND @StartDate > quote_periodi.data_inizio AND @StartDate <= quote_periodi.data_fine);" +
    "INSERT INTO quote_periodi (id_periodo_quote, id_aggregazione, data_inizio, data_fine) VALUES (null, @TipoSoldi, @StartDate, '2099-12-31'); ";

        public static readonly string ultima_riga = "SELECT last_insert_rowid() as ultima_riga; ";

        public static readonly string ComputesQuoteGuadagno = "DROP TABLE IF EXISTS valore_cumulato; CREATE TEMP TABLE IF NOT EXISTS valore_cumulato AS " +
            "SELECT id_periodo_quote, cum FROM (SELECT A.id_periodo_quote, SUM(ammontare) OVER (ORDER BY data_movimento) AS cum FROM quote_investimenti A, quote_periodi B " +
            "WHERE A.id_periodo_quote = B.id_periodo_quote AND id_tipo_movimento <> 12 AND STRFTIME('%Y', data_movimento) > 2010 AND B.id_aggregazione = @Tipo_Soldi " +
            "ORDER BY A.id_periodo_quote) AS ABC GROUP BY id_periodo_quote; " +
            "DROP TABLE IF EXISTS getione_cumulata; CREATE TEMP TABLE IF NOT EXISTS gestione_cumulata AS SELECT id_gestione, A.id_periodo_quote, SUM(CASE WHEN id_gestione = 3 " +
            "THEN ammontare ELSE 0 END) OVER (PARTITION BY id_gestione ORDER BY A.id_periodo_quote) AS cumulativeFv, SUM(CASE WHEN id_gestione = 5 THEN ammontare ELSE 0 END) " +
            "OVER (PARTITION BY id_gestione ORDER BY A.id_periodo_quote) AS cumulativeDp, SUM(CASE WHEN id_gestione = 4 THEN ammontare ELSE 0 END) OVER (PARTITION BY id_gestione " +
            "ORDER BY A.id_periodo_quote) AS cumulativeAu FROM quote_investimenti A, quote_periodi B WHERE A.id_periodo_quote = B.id_periodo_quote AND id_tipo_movimento <> 12 AND " +
            "STRFTIME('%Y', data_movimento) > 2010 AND B.id_aggregazione = @Tipo_Soldi ORDER BY data_movimento, id_gestione;";

        public static readonly string InsertQuotaGuadagno = "INSERT INTO quote_guadagno (id_gestione, id_quote_periodi, quota) SELECT id_gestione, id_periodo_quote, quota FROM " +
            "(SELECT id_gestione, AA.id_periodo_quote, QFV + QDP + QAU AS quota FROM (SELECT id_gestione, A.id_periodo_quote, cumulativeFv / cum AS QFV, cumulativeDp / cum AS QDP, " +
            "cumulativeAu / cum AS QAU FROM gestione_cumulata A, valore_cumulato B WHERE A.id_periodo_quote = B.id_periodo_quote) AS AA WHERE AA.id_periodo_quote = @Nuovo_Periodo);";

        public static readonly string UpdateQuotaGuadagno = "UPDATE quote_guadagno SET quota = BB.quota FROM (SELECT id_gestione, AA.id_periodo_quote, QFV + QDP + QAU AS quota FROM ( " +
            "SELECT id_gestione, A.id_periodo_quote, cumulativeFv / cum AS QFV, cumulativeDp / cum AS QDP, cumulativeAu / cum AS QAU FROM gestione_cumulata A, valore_cumulato B WHERE " +
            "A.id_periodo_quote = B.id_periodo_quote) AS AA) AS BB WHERE quote_guadagno.id_gestione = BB.id_gestione AND quote_guadagno.id_quote_periodi = BB.id_periodo_quote;";

        public static readonly string UpdateGuadagniTotaleAnno = "UPDATE conto_corrente SET id_quote_periodi = BB.id_periodo_quote FROM " +
            "(SELECT A.id_fineco_euro, A.data_movimento, A.id_tipo_movimento, A.ammontare, A.Causale, A.id_tipo_soldi, B.id_periodo_quote " +
            "FROM conto_corrente A, quote_periodi B WHERE B.id_periodo_quote = @IdPeriodoQuote AND A.data_movimento BETWEEN B.data_inizio AND B.data_fine AND A.id_tipo_soldi = @IdTipoSoldi) AS BB " +
            "WHERE conto_corrente.id_fineco_euro = BB.id_fineco_euro; " +
            "UPDATE guadagni_totale_anno SET quota = BB.quota, guadagnato = BB.guadagnato FROM (SELECT B.id_gestione, A.id_tipo_soldi, A.id_tipo_movimento, STRFTIME('%Y', data_movimento) AS anno, " +
            "case when B.id_gestione = 4 AND A.id_tipo_movimento = 8 then 0 ELSE(case when A.id_tipo_movimento = 8 then 0.5 ELSE B.quota END) END AS quota, " +
            "case when B.id_gestione = 4 AND A.id_tipo_movimento = 8 then 0 ELSE(case when A.id_tipo_movimento = 8 then 0.5 * A.ammontare ELSE A.ammontare * B.quota END) END AS guadagnato, " +
            "A.id_valuta, A.data_movimento, A.Causale, A.id_quote_periodi FROM conto_corrente A, quote_guadagno B, quote_periodi C WHERE A.id_quote_periodi = B.id_quote_periodi AND " +
            "B.id_gestione > 0 AND B.id_quote_periodi = C.id_periodo_quote AND (id_tipo_movimento = 4 OR id_tipo_movimento = 6 OR id_tipo_movimento = 7 OR id_tipo_movimento = 8) " +
            "AND A.id_tipo_soldi = @IdTipoSoldi AND A.data_movimento BETWEEN C.data_inizio AND C.data_fine AND C.id_periodo_quote >= @IdPeriodoQuote ORDER BY A.data_movimento, A.id_tipo_movimento, B.id_gestione " +
            ") AS BB WHERE guadagni_totale_anno.id_gestione = BB.id_gestione AND guadagni_totale_anno.id_tipo_soldi = BB.id_tipo_soldi AND guadagni_totale_anno.id_tipo_movimento = " +
            "BB.id_tipo_movimento AND guadagni_totale_anno.data_operazione = BB.data_movimento and guadagni_totale_anno.Causale = BB.Causale;";


    }
}

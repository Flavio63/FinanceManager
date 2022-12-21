using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services.SQL
{
    public class QuoteGuadagniScript
    {
        /// <summary> Verifico se nella data di inserimento è già presente un investimento</summary>
        public static readonly string VerifyInvestmentDate = "SELECT IFNULL(A.data_insert, IFNULL(SUM(A.id_periodo_quote), -1)) AS result FROM quote_periodi A " +
            "WHERE A.id_tipo_gestione = @id_tipo_soldi AND A.data_inizio = @data_inizio; ";

        public static readonly string quote_periodi = "UPDATE quote_periodi SET data_fine = date(@StartDate, '-1 day') WHERE id_periodo_quote = (" +
    "SELECT quote_periodi.id_periodo_quote FROM quote_periodi WHERE quote_periodi.id_periodo_quote > 0 AND quote_periodi.id_tipo_gestione = @TipoSoldi " +
    "AND @StartDate > quote_periodi.data_inizio AND @StartDate <= quote_periodi.data_fine);" +
    "INSERT INTO quote_periodi (id_periodo_quote, id_tipo_gestione, data_inizio, data_fine, data_insert) VALUES (null, @TipoSoldi, @StartDate, '2099-12-31', @Date_Time);";

        public static readonly string ultima_riga = "SELECT id_periodo_quote, id_tipo_gestione, data_inizio, data_fine, data_insert " +
            "FROM quote_periodi ORDER BY id_periodo_quote DESC LIMIT 1 ";

        private static readonly string CQG1 = "DROP TABLE IF EXISTS valore_cumulato; ";
        private static readonly string CQG2 = "CREATE TEMP TABLE IF NOT EXISTS valore_cumulato AS " +
            "SELECT id_quote_periodi, cum FROM (SELECT A.id_quote_periodi, SUM(ammontare) OVER (ORDER BY data_movimento) AS cum " +
            "FROM conto_corrente A, quote_periodi B " +
            "WHERE A.id_quote_periodi = B.id_periodo_quote AND id_tipo_movimento <> 12 AND A.id_conto = 1 AND STRFTIME('%Y', data_movimento) > '2000' AND B.id_tipo_gestione = @Tipo_Gestione " +
            "ORDER BY A.id_quote_periodi) AS ABC GROUP BY id_quote_periodi; ";
        private static readonly string CQG3 = "DROP TABLE IF EXISTS getione_cumulata; ";
        private static readonly string CQG4 = "CREATE TEMP TABLE IF NOT EXISTS gestione_cumulata AS " +
            "SELECT A.id_socio, A.id_quote_periodi, " +
            "SUM(CASE WHEN id_socio = 1 THEN ammontare ELSE 0 END) OVER (PARTITION BY id_socio ORDER BY A.id_quote_periodi) AS cumulativeFv, " +
            "SUM(CASE WHEN id_socio = 2 THEN ammontare ELSE 0 END) OVER (PARTITION BY id_socio ORDER BY A.id_quote_periodi) AS cumulativeDp, " +
            "SUM(CASE WHEN id_socio = 3 THEN ammontare ELSE 0 END) OVER (PARTITION BY id_socio ORDER BY A.id_quote_periodi) AS cumulativeAu " +
            "FROM conto_corrente A, quote_periodi B " +
            "WHERE A.id_quote_periodi = B.id_periodo_quote AND id_tipo_movimento <> 12 AND A.id_conto = 1 AND STRFTIME('%Y', data_movimento) > '2000' AND B.id_tipo_gestione = @Tipo_Gestione " +
            "ORDER BY data_movimento, id_socio;";

        public static readonly string ComputesQuoteGuadagno = CQG1 + CQG2 + CQG3 + CQG4;

        public static readonly string InsertQuotaGuadagno = "INSERT INTO quote_guadagno (id_socio, id_quote_periodi, quota) " +
            "SELECT id_socio, id_quote_periodi, quota FROM " +
            "(SELECT id_socio, AA.id_quote_periodi, QFV + QDP + QAU AS quota FROM " +
            "(SELECT id_socio, A.id_quote_periodi, cumulativeFv / cum AS QFV, cumulativeDp / cum AS QDP, cumulativeAu / cum AS QAU " +
            "FROM gestione_cumulata A, valore_cumulato B WHERE A.id_quote_periodi = B.id_quote_periodi) AS AA WHERE AA.id_quote_periodi = @Nuovo_Periodo);";

        public static readonly string UpdateQuotaGuadagno = "UPDATE quote_guadagno SET quota = BB.quota " +
            "FROM (SELECT id_socio, AA.id_quote_periodi, QFV + QDP + QAU AS quota " +
            "FROM ( SELECT id_socio, A.id_quote_periodi, cumulativeFv / cum AS QFV, cumulativeDp / cum AS QDP, cumulativeAu / cum AS QAU " +
            "FROM gestione_cumulata A, valore_cumulato B WHERE A.id_quote_periodi = B.id_quote_periodi) AS AA) AS BB " +
            "WHERE quote_guadagno.id_socio = BB.id_socio AND quote_guadagno.id_quote_periodi = BB.id_quote_periodi;";

        public static readonly string UpdateGuadagniTotaleAnno = "UPDATE conto_corrente SET id_quote_periodi = BB.id_periodo_quote FROM " +
            "(SELECT A.id_fineco_euro, A.data_movimento, A.id_tipo_movimento, A.ammontare, A.Causale, A.id_tipo_soldi, B.id_periodo_quote " +
            "FROM conto_corrente A, quote_periodi B WHERE B.id_periodo_quote = @IdPeriodoQuote AND A.data_movimento BETWEEN B.data_inizio AND B.data_fine AND A.id_tipo_soldi = @Tipo_Gestione) AS BB " +
            "WHERE conto_corrente.id_fineco_euro = BB.id_fineco_euro; " +
            "UPDATE guadagni_totale_anno SET quota = BB.quota, guadagnato = BB.guadagnato FROM (SELECT B.id_socio, A.id_tipo_soldi, A.id_tipo_movimento, STRFTIME('%Y', data_movimento) AS anno, " +
            "case when B.id_socio = 3 AND A.id_tipo_movimento = 8 then 0 ELSE(case when A.id_tipo_movimento = 8 then 0.5 ELSE B.quota END) END AS quota, " +
            "case when B.id_socio = 3 AND A.id_tipo_movimento = 8 then 0 ELSE(case when A.id_tipo_movimento = 8 then 0.5 * A.ammontare ELSE A.ammontare * B.quota END) END AS guadagnato, " +
            "A.id_valuta, A.data_movimento, A.Causale, A.id_quote_periodi FROM conto_corrente A, quote_guadagno B, quote_periodi C WHERE A.id_quote_periodi = B.id_quote_periodi AND " +
            "B.id_socio > 0 AND B.id_quote_periodi = C.id_periodo_quote AND (id_tipo_movimento = 4 OR id_tipo_movimento = 6 OR id_tipo_movimento = 7 OR id_tipo_movimento = 8 " +
            "OR id_tipo_movimento = 13 OR id_tipo_movimento = 14) " +
            "AND A.id_tipo_soldi = @Tipo_Gestione AND A.data_movimento BETWEEN C.data_inizio AND C.data_fine AND C.id_periodo_quote >= @IdPeriodoQuote ORDER BY A.data_movimento, A.id_tipo_movimento, B.id_socio " +
            ") AS BB WHERE guadagni_totale_anno.id_socio = BB.id_socio AND guadagni_totale_anno.id_tipo_soldi = BB.id_tipo_soldi AND guadagni_totale_anno.id_tipo_movimento = " +
            "BB.id_tipo_movimento AND guadagni_totale_anno.data_operazione = BB.data_movimento and guadagni_totale_anno.Causale = BB.Causale;";

        /// <summary>Trovo il codice del record da ricalcolare con le nuove quote</summary>
        public static readonly string GetIdPeriodoQuote = "SELECT A.id_periodo_quote FROM quote_periodi A " +
            "WHERE A.id_tipo_gestione = @id_tipo_gestione AND @data_movimento BETWEEN A.data_inizio AND A.data_fine";

        /// <summary>Dopo l'inserimento sul conto corrente, registro il guadagno</summary>
        public static readonly string AddSingoloGuadagno = "INSERT INTO guadagni_totale_anno " +
            "(id_socio, id_gestione, id_tipo_gestione, id_tipo_soldi, id_tipo_movimento, anno, quota, guadagnato, id_valuta, data_operazione, causale, id_quote_periodi, id_conto_corrente) " +
            "SELECT B.id_socio, A.id_gestione, D.id_tipo_gestione, id_tipo_soldi, id_tipo_movimento, strftime('%Y', data_movimento) AS anno, " +
            "case when B.id_socio = 3 AND A.id_tipo_movimento = 8 then 0 ELSE (case when A.id_tipo_movimento = 8 then 0.5 ELSE B.quota END) END AS quota, " +
            "case when B.id_socio = 3 AND A.id_tipo_movimento = 8 then 0 ELSE (case when A.id_tipo_movimento = 8 then 0.5 * A.ammontare ELSE A.ammontare * B.quota END) END AS guadagnato, " +
            "id_valuta, data_movimento, causale, A.id_quote_periodi, A.id_fineco_euro FROM conto_corrente A, quote_guadagno B, gestioni C, tipo_gestioni D " +
            "WHERE A.id_quote_periodi = B.id_quote_periodi AND A.id_gestione = C.id_gestione AND C.id_tipo_gestione = D.id_tipo_gestione AND A.id_fineco_euro = " +
            "(SELECT id_fineco_euro FROM conto_corrente C WHERE C.id_tipo_movimento = @id_tipo_movimento AND id_tipo_soldi = @id_tipo_soldi AND id_quote_periodi = @id_quote_periodi ORDER BY id_fineco_euro DESC LIMIT 1) " +
            "GROUP BY B.id_socio, A.id_gestione;";

        /// <summary>
        /// Elimino un record della tabella quote_guadagno
        /// </summary>
        public static readonly string DeleteRecordGuadagno_Totale_anno = "DELETE FROM guadagni_totale_anno WHERE id_conto_corrente = @id_conto_corrente ";

        /// <summary>Dopo la modifica sul conto corrente, registro la modifica sul guadagno totale anno</summary>
        public static readonly string ModifySingoloGuadagno = "UPDATE guadagni_totale_anno SET " +
            "anno = BB.anno, quota = BB.quota, id_gestione = BB.id_gestione, id_tipo_gestione = BB.id_tipo_gestione, guadagnato = BB.guadagnato, data_operazione = BB.data_movimento, causale = BB.causale, " +
            "id_quote_periodi = BB.id_quote_periodi, id_tipo_soldi = BB.id_tipo_soldi, id_valuta = BB.id_valuta FROM (SELECT B.id_socio, A.id_gestione, C.id_tipo_gestione, id_tipo_soldi, id_tipo_movimento, " +
            "strftime('%Y', data_movimento) AS anno, CASE WHEN B.id_socio = 3 AND A.id_tipo_movimento = 8 THEN 0 ELSE (CASE WHEN A.id_tipo_movimento = 8 THEN 0.5 ELSE B.quota END) END AS quota, " +
            "CASE WHEN B.id_socio = 3 AND A.id_tipo_movimento = 8 THEN 0 ELSE (CASE WHEN A.id_tipo_movimento = 8 THEN 0.5 * A.ammontare ELSE A.ammontare * B.quota END) END AS guadagnato, id_valuta, " +
            "data_movimento, Causale, A.id_quote_periodi, A.id_fineco_euro FROM conto_corrente A, quote_guadagno B, gestioni C WHERE A.id_quote_periodi = B.id_quote_periodi AND " +
            "A.id_gestione = C.id_gestione AND A.id_fineco_euro = @id_fineco_euro ) AS BB WHERE guadagni_totale_anno.id_socio = BB.id_socio AND id_conto_corrente = BB.id_fineco_euro;";

    }
}

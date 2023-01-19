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
        public static readonly string VerifyInvestmentDate = "SELECT IFNULL(A.data_insert, IFNULL(SUM(A.id_quote_periodi), -1)) AS result FROM quote_periodi A " +
            "WHERE A.id_tipo_gestione = @id_tipo_soldi AND A.data_inizio = @data_inizio; ";

        public static readonly string quote_periodi = "UPDATE quote_periodi SET data_fine = date(@StartDate, '-1 day') WHERE id_quote_periodi = (" +
    "SELECT quote_periodi.id_quote_periodi FROM quote_periodi WHERE quote_periodi.id_quote_periodi > 0 AND quote_periodi.id_tipo_gestione = @TipoSoldi " +
    "AND @StartDate > quote_periodi.data_inizio AND @StartDate <= quote_periodi.data_fine);" +
    "INSERT INTO quote_periodi (id_quote_periodi, id_tipo_gestione, data_inizio, data_fine, data_insert) VALUES (null, @TipoSoldi, @StartDate, '2099-12-31', @Date_Time);";

        public static readonly string ultima_riga = "SELECT id_quote_periodi, id_tipo_gestione, data_inizio, data_fine, data_insert " +
            "FROM quote_periodi ORDER BY id_quote_periodi DESC LIMIT 1 ";

        public static readonly string InsertQuotaGuadagno = "INSERT INTO quote_guadagno (id_socio, id_quote_periodi, quota) " +
            "SELECT id_socio, id_quote_periodi, quota FROM " +
            "(SELECT id_socio, AA.id_quote_periodi, QFV + QDP + QAU AS quota FROM " +
            "(SELECT id_socio, A.id_quote_periodi, cumulativeFv / cum AS QFV, cumulativeDp / cum AS QDP, cumulativeAu / cum AS QAU " +
            "FROM gestione_cumulata A, valore_cumulato B WHERE A.id_quote_periodi = B.id_quote_periodi) AS AA WHERE AA.id_quote_periodi = @Nuovo_Periodo);";

        public static readonly string UpdateGuadagniTotaleAnno =
            "UPDATE guadagni_totale_anno SET quota = BB.quota, id_tipo_gestione = BB.id_tipo_gestione, guadagnato = BB.guadagnato, id_quote_periodi = BB.id_quote_periodi FROM (" +
            "SELECT B.id_socio, A.id_gestione, C.id_tipo_gestione, id_tipo_soldi, id_tipo_movimento, strftime('%Y', data_movimento) AS anno, " +
            "CASE WHEN B.id_socio = 3 AND A.id_tipo_movimento = 8 THEN 0 ELSE (CASE WHEN A.id_tipo_movimento = 8 THEN 0.5 ELSE B.quota END) END AS quota, " +
            "CASE WHEN B.id_socio = 3 AND A.id_tipo_movimento = 8 THEN 0 ELSE (CASE WHEN A.id_tipo_movimento = 8 THEN 0.5 * A.ammontare ELSE A.ammontare * B.quota END) END AS guadagnato, " +
            "A.id_valuta, data_movimento, Causale, A.id_quote_periodi, A.id_fineco_euro FROM conto_corrente A, quote_guadagno B, gestioni C " +
            "WHERE A.id_quote_periodi = B.id_quote_periodi AND A.id_gestione = C.id_gestione AND A.id_valuta = B.id_valuta AND A.id_fineco_euro = @id_fineco_euro) AS BB " +
            "WHERE guadagni_totale_anno.id_socio = BB.id_socio AND id_conto_corrente = BB.id_fineco_euro;";

        /// <summary>Trovo il codice del record da ricalcolare con le nuove quote</summary>
        public static readonly string GetIdPeriodoQuote = "SELECT A.id_quote_periodi FROM quote_periodi A " +
            "WHERE A.id_tipo_gestione = @id_tipo_gestione AND @data_movimento BETWEEN A.data_inizio AND A.data_fine";

        /// <summary>Dopo l'inserimento sul conto corrente, registro il guadagno</summary>
        public static readonly string AddSingoloGuadagno = "INSERT INTO guadagni_totale_anno " +
            "(id_socio, id_gestione, id_tipo_gestione, id_tipo_soldi, id_tipo_movimento, anno, quota, guadagnato, id_valuta, data_operazione, causale, id_quote_periodi, id_conto_corrente) " +
            "SELECT B.id_socio, A.id_gestione, D.id_tipo_gestione, id_tipo_soldi, id_tipo_movimento, strftime('%Y', data_movimento) AS anno, " +
            "case when B.id_socio = 3 AND A.id_tipo_movimento = 8 then 0 ELSE (case when A.id_tipo_movimento = 8 then 0.5 ELSE B.quota END) END AS quota, " +
            "case when B.id_socio = 3 AND A.id_tipo_movimento = 8 then 0 ELSE (case when A.id_tipo_movimento = 8 then 0.5 * A.ammontare ELSE A.ammontare * B.quota END) END AS guadagnato, " +
            "A.id_valuta, data_movimento, causale, A.id_quote_periodi, A.id_fineco_euro FROM conto_corrente A, quote_guadagno B, gestioni C, tipo_gestioni D " +
            "WHERE A.id_quote_periodi = B.id_quote_periodi AND A.id_gestione = C.id_gestione AND C.id_tipo_gestione = D.id_tipo_gestione AND A.id_valuta = B.id_valuta AND A.id_fineco_euro = " +
            "(SELECT id_fineco_euro FROM conto_corrente C WHERE C.id_tipo_movimento = @id_tipo_movimento AND id_tipo_soldi = @id_tipo_soldi AND id_quote_periodi = @id_quote_periodi ORDER BY id_fineco_euro DESC LIMIT 1) " +
            "GROUP BY B.id_socio, A.id_gestione;";

        public static readonly string AddPrelievo = "INSERT INTO guadagni_totale_anno " +
            "(id_socio, id_gestione, id_tipo_gestione, id_tipo_soldi, id_tipo_movimento, anno, quota, guadagnato, prelevato, id_valuta, data_operazione, causale, id_quote_periodi, id_conto_corrente) " +
            "values (@id_socio, 0, 0, 0, @id_tipo_movimento, @year, 0, 0, @prelevato, @id_valuta, @data_operazione, @causale, 0, @id_conto_corrente)";

        /// <summary>
        /// Elimino un record della tabella quote_guadagno
        /// </summary>
        public static readonly string DeleteRecordGuadagno_Totale_anno = "DELETE FROM guadagni_totale_anno WHERE id_conto_corrente = @id_conto_corrente ";

        /// <summary>Dopo la modifica sul conto corrente, registro la modifica sul guadagno totale anno</summary>
        public static readonly string ModifySingoloGuadagno = "UPDATE guadagni_totale_anno SET " +
            "anno = BB.anno, quota = BB.quota, id_gestione = BB.id_gestione, id_tipo_gestione = BB.id_tipo_gestione, guadagnato = BB.guadagnato, data_operazione = BB.data_movimento, causale = BB.causale, " +
            "id_quote_periodi = BB.id_quote_periodi, id_tipo_soldi = BB.id_tipo_soldi, id_valuta = BB.id_valuta FROM (SELECT B.id_socio, A.id_gestione, C.id_tipo_gestione, id_tipo_soldi, id_tipo_movimento, " +
            "strftime('%Y', data_movimento) AS anno, CASE WHEN B.id_socio = 3 AND A.id_tipo_movimento = 8 THEN 0 ELSE (CASE WHEN A.id_tipo_movimento = 8 THEN 0.5 ELSE B.quota END) END AS quota, " +
            "CASE WHEN B.id_socio = 3 AND A.id_tipo_movimento = 8 THEN 0 ELSE (CASE WHEN A.id_tipo_movimento = 8 THEN 0.5 * A.ammontare ELSE A.ammontare * B.quota END) END AS guadagnato, A.id_valuta, " +
            "data_movimento, Causale, A.id_quote_periodi, A.id_fineco_euro FROM conto_corrente A, quote_guadagno B, gestioni C WHERE A.id_quote_periodi = B.id_quote_periodi AND " +
            "A.id_gestione = C.id_gestione AND A.id_valuta = B.id_valuta AND A.id_fineco_euro = @id_fineco_euro ) AS BB WHERE guadagni_totale_anno.id_socio = BB.id_socio AND id_conto_corrente = BB.id_fineco_euro;";

        public static readonly string ModifyPrelievo = "UPDATE guadagni_totale_anno SET id_socio = @id_socio, prelevato = @prelevato, id_valuta = @id_valuta, " +
            "data_operazione = @data_operazione, causale = @causale WHERE id_guadagno = @id_guadagno";

        /// estrazione dei dati in quote_guadagno serve a visualizzare i dati caricati
        public static readonly string GetQuoteGuadagni = "SELECT id_quota_guadagno, A.id_socio, B.nome_socio, A.id_quote_periodi, C.data_inizio, C.data_fine, ammontare, " +
            "cum_socio, cum_totale, quota, A.id_conto_corrente, A.id_valuta, D.cod_valuta, A.id_tipo_gestione, E.tipo_gestione FROM quote_guadagno A, soci B, quote_periodi C, valuta D, " +
            "tipo_gestioni E WHERE A.id_socio = B.id_socio AND A.id_quote_periodi = C.id_quote_periodi AND A.id_valuta = D.id_valuta AND A.id_tipo_gestione = E.id_tipo_gestione";

        // calcolo valore cumulato per socio
        public static readonly string GetTotaleCumulatoAnnoSocioValuta = "SELECT A.id_socio, B.nome_socio, A.id_valuta, D.cod_valuta, SUM(guadagnato) AS GuadagnoAnno1, SUM(prelevato) AS Preso, " +
            "SUM(guadagnato) + SUM(prelevato) AS RisparmioAnno FROM guadagni_totale_anno A, soci B, valuta D " +
            "WHERE anno >= 2015 AND A.id_socio = B.id_socio AND A.id_valuta = D.id_valuta  AND A.id_tipo_soldi <> 11 GROUP BY A.id_socio, A.id_valuta ORDER BY A.id_socio, A.id_valuta;";

        // calcolo valore cumulato per socio
        public static readonly string GetTotaleSocioValuta = "SELECT SUM(guadagnato) + SUM(prelevato) AS Risparmio " +
            "FROM guadagni_totale_anno A, soci B, valuta D " +
            "WHERE anno >= 2015 AND A.id_socio = B.id_socio AND A.id_valuta = D.id_valuta AND A.id_tipo_soldi <> 11 AND A.id_socio = @id_socio AND A.id_valuta = @id_valuta " +
            "GROUP BY A.id_socio, A.id_valuta ORDER BY A.id_socio, A.id_valuta;";


        // calcolo valore totale per tipologia
        public static readonly string GetMovimentiPrelievi = "SELECT id_guadagno, A.id_socio, B.nome_socio, prelevato, A.id_valuta, C.cod_valuta, data_operazione, causale, id_conto_corrente " +
            "FROM guadagni_totale_anno A, soci B, valuta C WHERE A.id_socio = B.id_socio AND A.id_valuta = C.id_valuta AND id_tipo_movimento = 2 ORDER BY data_operazione DESC";

        public static readonly string GetLastRecordBySocioValuta = GetQuoteGuadagni + 
            " AND A.id_socio = @id_socio AND A.id_valuta = @id_valuta AND A.id_tipo_gestione = @id_tipo_gestione ORDER BY A.id_quota_guadagno DESC LIMIT 1";

        public static readonly string InsertRecordQuoteGuadagno = "INSERT INTO quote_guadagno (id_socio, id_quote_periodi, ammontare, cum_socio, cum_totale, quota, " +
            "id_conto_corrente, id_valuta, id_tipo_gestione) " +
            "VALUES (@id_socio, @id_quote_periodi, @ammontare, @cum_socio, @cum_totale, @quota, @id_conto_corrente, @id_valuta, @id_tipo_gestione)";

        public static readonly string ModifyQuoteGuadagno = "UPDATE quote_guadagno SET ammontare = @ammontare, cum_socio = @cum_socio, cum_totale = @cum_totale, quota = @quota, " +
            "id_conto_corrente = @id_conto_corrente WHERE id_quota_guadagno = @id_quota_guadagno";
    }
}

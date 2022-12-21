using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services.SQL
{
    public class ManagerScripts
    {
        private static readonly string DescLimit1 = " DESC LIMIT 1 ";
        private static readonly string AndGestione = " AND A.id_gestione = @id_gestione ";
        private static readonly string AndConto = " AND A.id_conto = @id_conto ";
        private static readonly string AndValuta = " AND A.id_valuta = @id_valuta ";
        private static readonly string OrderBy = " ORDER BY ";
        private static readonly string GroupBy = " GROUP BY ";
        private static readonly string Comma = ", ";
        private static readonly string AIdConto = " A.id_conto ";
        private static readonly string AIdValuta = " A.id_valuta ";
        private static readonly string AIdQuoteInv = " A.id_quote_inv ";
        private static readonly string ADataMovimento = " A.data_movimento ";

        private static readonly string GetTableQuote = "SELECT id_quote_inv, A.id_gestione, B.nome_gestione, A.id_tipo_movimento, C.desc_movimento, data_movimento, ammontare, A.id_valuta, " +
            "D.cod_valuta, A.valuta_base, A.valore_cambio, A.note FROM quote_investimenti A, gestioni B, tipo_movimento C, valuta D " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_tipo_movimento = C.id_tipo_movimento AND A.id_valuta = D.id_valuta AND id_quote_inv > 0 ";

        /// <summary>
        /// Data una gestione estrae il totale cedole, utili e disponibilità  +             "GROUP BY A.id_conto, A.id_valuta "
        /// per c/c e per valuta
        /// </summary>
        private static readonly string GetCurrencyAvailable = "SELECT C.desc_conto, B.cod_valuta, " +
            "ROUND(SUM(CASE WHEN A.id_tipo_soldi = 17 THEN ammontare ELSE 0 END), 2) AS Cedole, " +
            "ROUND(SUM(CASE WHEN A.id_tipo_soldi = 15 OR A.id_tipo_soldi = 16 THEN ammontare ELSE 0 END), 2) AS Utili, " +
            "ROUND(SUM(CASE WHEN id_tipo_soldi = 1 THEN ammontare ELSE 0 END), 2) AS Disponibili " +
            "FROM conto_corrente A, valuta B, conti C WHERE A.id_conto = C.id_conto and A.id_valuta = B.id_valuta ";

        public static readonly string GetCurrencyAvByOwner = GetCurrencyAvailable + AndGestione + GroupBy + AIdConto + Comma + AIdValuta;
        public static readonly string GetCurrencyAvByOwnerContoValuta = GetCurrencyAvailable + AndGestione + AndConto + AndValuta;

        /// <summary>
        /// Data una gestione e una location, estrae tutti i record relativi ai movimenti di titoli
        /// </summary>
        public static readonly string GetManagerSharesMovementByOwnerAndLocation = "SELECT id_portafoglio_titoli, B.id_gestione, nome_gestione, C.id_conto, desc_conto, D.id_valuta, " +
            "cod_valuta, E.id_tipo_movimento, desc_Movimento, A.id_titolo, F.desc_titolo, F.isin, F.id_tipo_titolo, H.desc_tipo_titolo, F.id_azienda, I.desc_azienda, data_movimento, " +
            "shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, A.note, attivo " +
            "FROM portafoglio_titoli A, gestioni B, conti C, valuta D, tipo_movimento E, titoli F, tipo_titoli H, aziende I " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_conto = C.id_conto AND A.id_valuta = D.id_valuta AND A.id_tipo_movimento = E.id_tipo_movimento AND A.id_titolo = F.id_titolo " +
            "AND F.id_tipo_titolo = H.id_tipo_titolo AND F.id_azienda = I.id_azienda AND B.id_gestione = @id_gestione AND C.id_conto = @id_conto AND A.id_titolo IS NOT NULL " +
            "ORDER BY data_movimento DESC";

        /// <summary>
        /// Estrae la somma del profit loss data una gestione e un dato conto
        /// </summary>
            public static readonly string GetProfitLossByCurrency = "SELECT SUM(profit_loss) as TotalProfitLoss FROM portafoglio_titoli " +
            "WHERE id_gestione = @id_gestione AND id_conto = @id_conto AND id_valuta =@id_valuta";

        /// <summary>
        /// Preleva i movimenti di acquito (5) e vendita (6)
        /// del portafoglio titoli per un dato gestore, un dato conto e un dato titolo
        /// </summary>
        public static readonly string GetShareMovements = "SELECT id_portafoglio_titoli, B.id_gestione, nome_gestione, C.id_conto, desc_conto, D.id_valuta, " +
            "cod_valuta, E.id_tipo_movimento, desc_Movimento, A.id_titolo, F.desc_titolo, F.isin, F.id_tipo_titolo, H.desc_tipo, F.id_azienda, I.desc_azienda, data_movimento, " +
            "shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, A.note " +
            "FROM portafoglio_titoli A, gestioni B, conti C, valuta D, tipo_movimento E, titoli F, tipo_titoli H, aziende I " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_conto = C.id_conto AND A.id_valuta = D.id_valuta AND A.id_tipo_movimento = E.id_tipo_movimento AND A.id_titolo = F.id_titolo " +
            "AND F.id_tipo_titoli = H.id_tipo_titoli AND F.id_azienda = I.id_azienda AND B.id_portafoglio_titoli = @owner AND C.id_conto = @id_conto AND id_titolo = @id_titolo " +
            "AND (id_movimento = 5 or id_movimento = 6) " +
            "ORDER BY data_movimento";

        /// <summary>
        /// calcola le quote per investitore del guadagno
        /// in base al periodo di validità delle quote di investimento
        /// </summary>
        public static readonly string GetQuoteDettaglioGuadagno = "SELECT anno, A.id_guadagno, A.id_gestione, B.nome_gestione, A.id_tipo_movimento, C.desc_tipo_soldi, A.id_valuta, D.cod_valuta, A.data_operazione, " +
            "A.quota, guadagnato AS GuadagnoAnno1, prelevato AS Preso, Causale FROM guadagni_totale_anno A, gestioni B, tipo_soldi C, valuta D " +
            "WHERE anno >= 2019 AND A.id_gestione = B.id_gestione AND A.id_tipo_soldi <> 11 AND A.id_tipo_soldi = C.id_tipo_soldi AND A.id_valuta = D.id_valuta ORDER BY anno DESC, A.data_operazione DESC, A.id_tipo_soldi, A.id_gestione DESC";

        public static readonly string GetQuoteSintesiGuadagno = "SELECT anno, B.nome_gestione, C.desc_tipo_soldi, A.id_valuta, D.cod_valuta, SUM(guadagnato) AS GuadagnoAnno1, SUM(prelevato) AS Preso " +
            "FROM guadagni_totale_anno A, gestioni B, tipo_soldi C, valuta D WHERE anno >= 2019 AND A.id_gestione = B.id_gestione AND A.id_tipo_soldi <> 11 AND A.id_tipo_soldi = C.id_tipo_soldi " +
            "AND A.id_valuta = D.id_valuta GROUP BY anno, A.id_gestione, A.id_tipo_soldi, A.id_valuta ORDER BY anno DESC, A.id_gestione DESC, A.id_tipo_soldi; ";

        public static readonly string GetQuoteGuadagno = "SELECT * FROM tmpGain ORDER BY anno DESC, nome_gestione, id_valuta;";

        /// <summary>Estraggo gli anni dalla tabella guadagni_totale_anno</summary>
        public static readonly string GetAnniFromGuadagni = "SELECT A.anno FROM guadagni_totale_anno A GROUP BY A.anno ORDER BY A.anno";

        /// <summary>
        /// Trovo la data che precede quella nuova basandomi sulla tabella degli investimenti (serve per gli inserimenti)
        /// </summary>
        public static readonly string GetDataPrecedente = "SELECT data_movimento FROM quote_investimenti A " +
            "WHERE A.id_quote_inv > 0 AND A.id_tipo_movimento <> 12 AND A.data_movimento < @data_movimento " +
            "ORDER BY A.data_movimento DESC LIMIT 1;";

        public static readonly string GetLastPeriodoValiditaQuote = "SELECT id_periodo_quote FROM quote_periodi A ORDER BY A.id_periodo_quote DESC LIMIT 1";

        /// <summary>
        /// Esporto tutti i record della tabella quote_guadagno aggiungendo le descrizioni
        /// di investitore e di tipologia investimento
        /// </summary>
        public static readonly string GetAllRecordQuote_Guadagno = "SELECT A.id_quota, A.id_gestione, B.nome_gestione, D.id_aggregazione, C.desc_tipo_soldi, A.id_quote_periodi, D.data_inizio, D.data_fine, A.quota " +
            "FROM quote_guadagno A, gestioni B, tipo_soldi C, quote_periodi D WHERE A.id_gestione = B.id_gestione AND D.id_aggregazione = C.id_aggregazione AND A.id_quote_periodi = D.id_periodo_quote " +
            "ORDER BY data_fine DESC, A.id_gestione DESC ";

        /// <summary>
        /// Inserisco un record nuovo nella tabella quote_guadagno
        /// </summary>
        public static readonly string InsertRecordQuote_Guadagno = "INSERT INTO quote_guadagno (id_quota, id_gestione, id_quote_periodi, quota) values (" +
            "null, @id_gestione, @id_quote_periodi, @quota)";

        /// <summary>
        /// Esporta tutti i record della quote prendendo anche le descrizioni
        /// </summary>
        public static readonly string GetQuoteTab = GetTableQuote + OrderBy + ADataMovimento + " DESC" + Comma + AIdQuoteInv;

        /// <summary>
        /// Aggiungo questo pezzo di script alla precendente stringa
        /// </summary>
        public static readonly string GetLastQuoteTab = GetTableQuote + OrderBy + AIdQuoteInv + DescLimit1;

        /// <summary>
        /// Calcola e restituisce il totale (somma algebrica) di quanto investito da un soggetto
        /// </summary>
        public static readonly string GetInvestmentByIdGestione = "SELECT SUM(ammontare) AS totale FROM quote_investimenti A WHERE A.id_tipo_movimento <> 12 AND id_gestione = @id_gestione";

        public static readonly string GetIdQuoteTab = "SELECT id_quote_inv FROM quote_investimenti A WHERE A.id_gestione = @id_gestione AND A.id_tipo_movimento = @id_tipo_movimento " +
            "AND A.id_periodo_quote = @id_periodo_quote ";

        public static readonly string GetQuoteTabById = "SELECT id_quote_inv, A.id_gestione, nome_gestione, A.id_tipo_movimento, desc_movimento, id_periodo_quote, data_movimento, A.id_valuta, cod_valuta, " +
            "valore_cambio, ammontare, valuta_base, note FROM quote_investimenti A, gestioni B, tipo_movimento C, valuta D " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_tipo_movimento = C.id_tipo_movimento and A.id_valuta = D.id_valuta AND id_quote_inv = @Id_quote ";

        /// <summary>
        /// Modifica un record nella tabella quote_investimenti
        /// </summary>
        public static readonly string UpdateQuoteTab = "UPDATE quote_investimenti SET id_gestione = @id_gestione, id_tipo_movimento = @id_tipo_movimento, data_movimento = @data_movimento, " +
            "ammontare = @ammontare, id_valuta = @id_valuta, valuta_base = @valuta_base, valore_cambio = @valore_cambio, note = @note WHERE id_quote_inv = @id_quote_inv";

        /// <summary>
        /// Elimina un record nella tabella quote_investimenti
        /// </summary>
        public static readonly string DeleteRecordQuoteTab = "DELETE FROM quote_investimenti WHERE id_quote_inv = @id_quote_inv";

        /// <summary>
        /// Trovo quanta disponibilità di utili ci sono
        /// </summary>
        public static readonly string VerifyDisponibilitaUtili = "SELECT SUM(guadagnato + prelevato) + @daInserire FROM guadagni_totale_anno WHERE id_tipo_soldi <> 11 AND id_valuta = @id_valuta AND id_gestione = @id_gestione AND anno = @anno";
        /// <summary>
        /// Registro la cifra prelevata
        /// </summary>
        public static readonly string InsertPrelievoUtili = "INSERT INTO guadagni_totale_anno (id_gestione, id_tipo_movimento, id_valuta, anno, prelevato, data_operazione, Causale) " +
            "VALUES (@id_gestione, @id_tipo_movimento, @id_valuta, @anno, @ammontare, @data_operazione, @Causale)";

        public static readonly string InsertPrelievoUtiliBkd = "INSERT INTO prelievi (id_prelievo, id_gestione, id_tipo_movimento, id_valuta, anno, prelevato, data_operazione, Causale) " +
            "VALUES (@id_prelievo, @id_gestione, @id_tipo_movimento, @id_valuta, @anno, @ammontare, @data_operazione, @Causale)";

        /// <summary>Registro la modifica sui prelievi di utili</summary>
        public static readonly string UpdatePrelievoUtili = "UPDATE guadagni_totale_anno SET id_gestione = @id_gestione, anno = @anno, prelevato = @prelevato, data_operazione = @data, " +
            "Causale = @Causale WHERE id_guadagno = @id_guadagno ";
        public static readonly string UpdatePrelievoUtiliBkd = "UPDATE prelievi SET id_gestione = @id_gestione, anno = @anno, prelevato = @prelevato, data_operazione = @data, " +
            "Causale = @Causale WHERE id_prelievo = @id_prelievo ";

        /// <summary> Elimino un recordo di prelievo utili </summary>
        public static readonly string DeletePrelievoUtiliBKd = "DELETE FROM prelievi WHERE id_prelievo = @id_prelievo;";
        public static readonly string DeletePrelievoUtili = "DELETE FROM guadagni_totale_anno WHERE id_guadagno = @id_guadagno;";

        public static readonly string GetTotalAmountByCurrency = "SELECT C.nome_gestione as Nome, sum(valuta_base) as Soldi, B.desc_valuta as Valuta FROM main.quote_investimenti A, main.valuta B, main.gestioni C " +
            "WHERE A.id_valuta = B.id_valuta AND A.id_gestione = C.id_gestione AND C.id_gestione = @IdInvestitore and A.id_tipo_movimento > 0 {0} GROUP BY C.id_gestione, A.id_valuta";
    }
}

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
        private static readonly string AndModified = " AND A.modified = @modified ";
        private static readonly string AndQuote = " AND A.id_quote_investimenti = @id_quote_investimenti ";
        private static readonly string AndGestione = " AND A.id_gestione = @id_gestione ";
        private static readonly string AndConto = " AND A.id_conto = @id_conto ";
        private static readonly string AndTitolo = " AND A.id_titolo = @id_titolo ";
        private static readonly string AndValuta = " AND A.id_valuta = @id_valuta ";
        private static readonly string AndIdPortafoglioTitoli = " AND A.id_portafoglio_titoli = @id_portafoglio_titoli ";
        private static readonly string AndIdFinecoEuro = " AND A.id_fineco_euro = @id_fineco_euro ";
        private static readonly string AndLinkMovimenti = " AND A.link_movimenti = @link_movimenti ";
        private static readonly string DataMovimentoDesc = " data_movimento desc ";
        private static readonly string OrderBy = " ORDER BY ";
        private static readonly string GroupBy = " GROUP BY ";
        private static readonly string Comma = ", ";
        private static readonly string TipoMovimento = " id_tipo_movimento ";
        private static readonly string AIdConto = " A.id_conto ";
        private static readonly string AIdValuta = " A.id_valuta ";
        private static readonly string AIdQuoteInv = " A.id_quote_inv ";
        private static readonly string ADataMovimento = " A.data_movimento ";
        private static readonly string AId_fineco_euro = " A.id_fineco_euro";

        private static readonly string GetTableQuote = "SELECT id_quote_inv, A.id_gestione, B.nome_gestione, A.id_tipo_movimento, C.desc_movimento, data_movimento, ammontare, A.note " +
            "FROM quote_investimenti A, gestioni B, tipo_movimento C WHERE A.id_gestione = B.id_gestione AND A.id_tipo_movimento = C.id_tipo_movimento AND id_quote_inv > 0 ";

        private static readonly string GetManagerLiquidAssetList = "SELECT id_portafoglio_titoli, B.id_gestione, nome_gestione, C.id_conto, desc_conto, D.id_valuta, cod_valuta, " +
            "E.id_tipo_movimento, desc_Movimento, G.id_tipo_titolo, G.desc_tipo_titolo, H.id_azienda, H.desc_azienda, A.id_titolo, F.desc_titolo, F.isin, data_movimento, " +
            "shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, A.note, attivo, link_movimenti " +
            "FROM portafoglio_titoli A, gestioni B, conti C, valuta D, tipo_movimento E, titoli F, tipo_titoli G, aziende H " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_conto = C.id_conto AND A.id_valuta = D.id_valuta AND A.id_tipo_movimento = E.id_tipo_movimento AND A.id_titolo = F.id_titolo AND " +
            "F.id_tipo_titolo = G.id_tipo_titolo AND F.id_azienda = H.id_azienda AND id_portafoglio_titoli > 0 ";

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
        /// Estrae tutti i movimenti dal portafoglio titoli
        /// </summary>
        public static readonly string GetManagerLiquidAssetListTotal = GetManagerLiquidAssetList + OrderBy + DataMovimentoDesc + Comma + TipoMovimento;

        public static readonly string GetPortafoglioTitoliById = GetManagerLiquidAssetList + AndIdPortafoglioTitoli;
        /// <summary>
        /// Estrae tutti i movimenti dal portafoglio titoli di una gestione
        /// </summary>
        public static readonly string GetManagerLiquidAssetListByOwner = GetManagerLiquidAssetList + AndGestione + OrderBy + DataMovimentoDesc + Comma + TipoMovimento;

        public static readonly string GetManagerLiquidAssetListByLocation = GetManagerLiquidAssetList + AndConto + OrderBy + DataMovimentoDesc + Comma + TipoMovimento;

        public static readonly string GetManagerLiquidAssetListByOwnerLocatioAndShare = GetManagerLiquidAssetList + AndGestione + AndConto + AndTitolo + OrderBy + DataMovimentoDesc;

        public static readonly string GetManagerLiquidAssetListByLinkMovimenti = GetManagerLiquidAssetList + AndLinkMovimenti + OrderBy + ADataMovimento;
        /// <summary>
        /// Data una gestione e un conto estrae tutti i record
        /// </summary>
        public static readonly string GetManagerLiquidAssetListByOwnerAndLocation = GetManagerLiquidAssetList + AndGestione + AndConto + OrderBy + DataMovimentoDesc + Comma + TipoMovimento;

        /// <summary>
        /// Data una gestione e una location, estrae tutti i record relativi ai tipi di movimenti richiesti
        /// </summary>
        public static readonly string GetManagerLiquidAssetByOwnerLocationAndMovementType = GetManagerLiquidAssetList + AndGestione + AndConto + " AND {0} " + OrderBy + DataMovimentoDesc;

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
        /// Data una gestione e una location, estrae l'ultimo record caricato
        /// </summary>
        public static readonly string GetLastSharesMovementByOwnerAndLocation = "SELECT id_portafoglio_titoli, B.id_gestione, nome_gestione, C.id_conto, desc_conto, D.id_valuta, " +
            "cod_valuta, E.id_tipo_movimento, desc_Movimento, A.id_titolo, F.desc_titolo, F.isin, F.id_tipo_titolo, H.desc_tipo_titolo, F.id_azienda, I.desc_azienda, data_movimento, " +
            "shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, A.note, attivo, link_movimenti " +
            "FROM portafoglio_titoli A, gestioni B, conti C, valuta D, tipo_movimento E, titoli F, tipo_titoli H, aziende I " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_conto = C.id_conto AND A.id_valuta = D.id_valuta AND A.id_tipo_movimento = E.id_tipo_movimento AND A.id_titolo = F.id_titolo " +
            "AND F.id_tipo_titolo = H.id_tipo_titolo AND F.id_azienda = I.id_azienda AND B.id_gestione = @id_gestione AND C.id_conto = @id_conto AND A.id_titolo IS NOT NULL " +
            "ORDER BY id_portafoglio_titoli DESC LIMIT 1";

        /// <summary>
        /// Estrae il numero di azioni possedute dato una gestione, un conto e un id azione
        /// </summary>
        public static readonly string GetSharesQuantity = "SELECT SUM(shares_quantity) TotalShares FROM portafoglio_titoli " +
            "WHERE id_gestione = @id_gestione AND id_conto = @id_conto AND id_titolo = @id_titolo";

        /// <summary>
        /// Estrae la somma del profit loss data una gestione e un dato conto
        /// </summary>
            public static readonly string GetProfitLossByCurrency = "SELECT SUM(profit_loss) as TotalProfitLoss FROM portafoglio_titoli " +
            "WHERE id_gestione = @id_gestione AND id_conto = @id_conto AND id_valuta =@id_valuta";

        /// <summary>
        /// Aggiunge un movimento al portafoglio titoli
        /// </summary>
        public static readonly string AddManagerLiquidAsset = "INSERT INTO portafoglio_titoli (id_portafoglio_titoli, id_gestione, id_conto, id_valuta, id_tipo_movimento, " +
            "id_titolo, data_movimento, ammontare, shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, " +
            "valore_cambio, note, attivo, link_movimenti) " +
            "VALUE (null, @id_gestione, @id_conto, @id_valuta, @id_tipo_movimento, @id_titolo, @data_movimento, @ammontare, @shares_quantity, " +
            "@unity_local_value, @total_commission, @tobin_tax, @disaggio_cedole, @ritenuta_fiscale, @valore_cambio, @note, @attivo, @link_movimenti);";

        /// <summary>
        /// Aggiorna un movimento del portafoglio titoli
        /// </summary>
        public static readonly string UpdateManagerLiquidAsset = "UPDATE portafoglio_titoli SET id_gestione = @id_gestione, id_conto = @id_conto, id_valuta = @id_valuta, " +
            "id_tipo_movimento = @id_tipo_movimento, id_titolo = @id_titolo, data_movimento = @data_movimento, ammontare = @ammontare, shares_quantity = @shares_quantity, " +
            "unity_local_value = @unity_local_value, total_commission = @total_commission, tobin_tax = @tobin_tax, disaggio_cedole = @disaggio_cedole, " +
            "ritenuta_fiscale = @ritenuta_fiscale, valore_cambio = @valore_cambio, " +
            "note = @note, attivo = @attivo, link_movimenti = @link_movimenti WHERE id_portafoglio_titoli = @id_portafoglio_titoli";
        
        /// <summary>
        /// Cancella un movimento del portafoglio titoli
        /// </summary>
        public static readonly string DeleteManagerLiquidAsset = "DELETE FROM portafoglio_titoli WHERE id_portafoglio_titoli = @id_portafoglio_titoli;";

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
        /// Dato un id_titoli, una gestione e un conto restituisce i titoli ancora in portafoglio
        /// </summary>
        public static readonly string GetShareActiveAndAccountMovement = "SELECT AA.id_portafoglio_titoli, AA.id_gestione, AA.id_conto, AA.id_valuta, AA.id_tipo_movimento, AA.id_titolo, " +
            "AA.data_movimento, AA.ammontare AS ValoreAzione, AA.shares_quantity, AA.unity_local_value, AA.total_commission, AA.tobin_tax, AA.disaggio_cedole, AA.ritenuta_fiscale, AA.valore_cambio, " +
            "AA.note, BB.id_fineco_euro, BB.ammontare AS Valore_in_CC, BB.id_tipo_soldi FROM portafoglio_titoli AA LEFT JOIN conto_corrente BB ON AA.id_portafoglio_titoli = BB.id_portafoglio_titoli " +
            "WHERE AA.id_conto = @id_conto AND AA.id_gestione = @id_gestione AND AA.id_titolo = @id_titolo AND AA.attivo = 1 ORDER BY AA.data_movimento";

        /// <summary>
        /// Inserisce un movimento nel conto corrente
        /// </summary>
        public static readonly string InsertAccountMovement = "INSERT INTO conto_corrente (id_conto, id_quote_investimenti, id_valuta, id_portafoglio_titoli, id_tipo_movimento, " +
            "id_gestione, id_titolo, data_movimento, ammontare, cambio, causale, id_tipo_soldi, id_quote_periodi, modified) VALUE ( @id_conto, @id_quote_investimenti, @id_valuta, @id_portafoglio_titoli, @id_tipo_movimento, " +
            "@id_gestione, @id_titolo, @data_movimento, @ammontare, @cambio, @causale, @id_tipo_soldi, @id_quote_periodi, @modified)";

        /// <summary>
        /// Estrae tutti i movimenti di tutti i conti correnti di tutte le gestioni
        /// </summary>
        protected static readonly string ContoCorrente = "SELECT id_fineco_euro, A.id_conto, B.desc_conto, id_quote_investimenti, A.id_valuta, C.cod_valuta, id_portafoglio_titoli, A.id_tipo_movimento, " +
            "D.desc_movimento, A.id_gestione, E.nome_gestione, A.id_titolo, F.isin, F.desc_titolo, data_movimento, ammontare, cambio, causale, A.id_tipo_soldi, G.desc_tipo_soldi, modified " +
            "FROM conto_corrente A, conti B, valuta C, tipo_movimento D, gestioni E, titoli F, tipo_soldi G " +
            "WHERE A.id_conto = B.id_conto AND A.id_valuta = C.id_valuta AND A.id_tipo_movimento = D.id_tipo_movimento AND " +
            "A.id_gestione = E.id_gestione AND A.id_titolo = F.id_titolo AND A.id_tipo_soldi = G.id_tipo_soldi AND id_fineco_euro > 0 ";
        // varie opzioni sul conto corrente
        public static readonly string GetContoCorrente = ContoCorrente + OrderBy + DataMovimentoDesc + Comma + AId_fineco_euro;
        public static readonly string GetContoCorrenteByIdCC = ContoCorrente + AndIdFinecoEuro;
        public static readonly string GetContoCorrenteByIdQuote = ContoCorrente + AndQuote + OrderBy + ADataMovimento;
        public static readonly string Get2ContoCorrentes = ContoCorrente + AndModified + OrderBy + ADataMovimento + Comma + AId_fineco_euro;
        public static readonly string GetContoCorrenteByIdPortafoglio = ContoCorrente + AndIdPortafoglioTitoli + OrderBy + AId_fineco_euro;

        // aggiorno un record conto corrente sulla base dell'ID QUOTE INVESTIMENTI//
        public static readonly string UpdateContoCorrenteByIdQuote = "UPDATE conto_corrente SET id_conto = @id_conto, id_valuta = @id_valuta, id_portafoglio_titoli = @id_portafoglio_titoli, " +
            "id_tipo_movimento = @id_tipo_movimento, id_gestione = @id_gestione, id_titolo = @id_titolo, data_movimento = @data_movimento, ammontare = @ammontare, " +
            "cambio = @cambio, causale = @causale, id_tipo_soldi = @id_tipo_soldi, id_quote_periodi = @id_quote_periodi WHERE id_quote_investimenti = @id_quote_investimenti";

        // aggiorno un record conto corrente sulla base dell'ID PORTAFOGLIO TITOLI //
        public static readonly string UpdateContoCorrenteByIdPortafoglioTitoli = "UPDATE conto_corrente SET id_conto = @id_conto, id_valuta = @id_valuta, id_quote_investimenti = @id_quote_investimenti, " +
            "id_tipo_movimento = @id_tipo_movimento, id_gestione = @id_gestione, id_titolo = @id_titolo, data_movimento = @data_movimento, ammontare = @ammontare, " +
            "cambio = @cambio, causale = @causale, id_tipo_soldi = @id_tipo_soldi, id_quote_periodi = @id_quote_periodi WHERE id_portafoglio_titoli = @id_portafoglio_titoli";

        // aggiorno un record conto corrente sulla base dell'ID CONTO CORRENTE //
        public static readonly string UpdateContoCorrenteByIdCC = "UPDATE conto_corrente SET id_conto = @id_conto, id_quote_investimenti = @id_quote_investimenti, id_valuta = @id_valuta, " +
            "id_portafoglio_titoli = @id_portafoglio_titoli, id_tipo_movimento = @id_tipo_movimento, id_gestione = @id_gestione, id_titolo = @id_titolo, data_movimento = @data_movimento, " +
            "ammontare = @ammontare, cambio = @cambio, causale = @causale, id_tipo_soldi = @id_tipo_soldi, id_quote_periodi = @id_quote_periodi WHERE id_fineco_euro = @id_fineco_euro";

        // cancello un record del conto corrente //
        public static readonly string DeleteAccount = "DELETE FROM conto_corrente WHERE id_fineco_euro = @id_fineco_euro";

        /// <summary>
        /// calcola le quote per investitore
        /// </summary>
        public static readonly string GetQuoteInv = "SELECT B.nome_gestione, sum(case when A.id_tipo_movimento = 1 then ammontare ELSE 0 END) AS Versato, " +
            "sum(case when A.id_tipo_movimento = 12 AND ammontare < 0 then ammontare ELSE 0 END) AS Investito, sum(case when A.id_tipo_movimento = 12 AND ammontare > 0 then ammontare ELSE 0 END) AS Disinvestito, " +
            "sum(case when A.id_tipo_movimento = 2 then ammontare ELSE 0 END) AS Prelevato, SUM(ammontare) AS Disponibile FROM quote_investimenti A, gestioni B " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_tipo_movimento<> 0 GROUP BY A.id_gestione;";

        /// <summary>
        /// calcola le quote per investitore del guadagno
        /// in base al periodo di validità delle quote di investimento
        /// </summary>
        public static readonly string GetQuoteDettaglioGuadagno = "SELECT anno, A.id_guadagno, A.id_gestione, B.nome_gestione, A.id_tipo_movimento, C.desc_tipo_soldi, A.id_valuta, D.cod_valuta, A.data_operazione, " +
            "A.quota, guadagnato AS GuadagnoAnno1, prelevato AS Preso, causale FROM guadagni_totale_anno A, gestioni B, tipo_soldi C, valuta D " +
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

        /// <summary>Trovo il codice dei record da ricalcolare con le nuove quote</summary>
        //public static readonly string GetIdPeriodoQuote = "SELECT * FROM quote_periodi A WHERE A.data_inizio = @data_movimento and A.id_tipo_soldi = @id_tipo_soldi;";
        /// <summary>Trovo il codice del periodo quote basandomi sulla data del movimento e sul tipo soldi</summary>
        public static readonly string GetIdPeriodoQuote = "SELECT id_periodo_quote FROM quote_periodi A, tipo_soldi B WHERE A.id_aggregazione = B.id_aggregazione AND @data_movimento " +
            "BETWEEN A.data_inizio AND A.data_fine AND B.id_tipo_soldi = @id_tipo_soldi";

        public static readonly string GetLastPeriodoValiditaQuote = "SELECT id_periodo_quote FROM quote_periodi A ORDER BY A.id_periodo_quote DESC LIMIT 1";

        /// <summary>
        /// Esporto tutti i record della tabella quote_guadagno aggiungendo le descrizioni
        /// di investitore e di tipologia investimento
        /// </summary>
        public static readonly string GetAllRecordQuote_Guadagno = "SELECT A.id_quota, A.id_gestione, B.nome_gestione, D.id_aggregazione, C.desc_tipo_soldi, A.id_quote_periodi, D.data_inizio, D.data_fine, A.quota " +
            "FROM quote_guadagno A, gestioni B, tipo_soldi C, quote_periodi D WHERE A.id_gestione = B.id_gestione AND D.id_aggregazione = C.id_aggregazione AND A.id_quote_periodi = D.id_periodo_quote " +
            "ORDER BY data_fine DESC, id_gestione DESC ";

        /// <summary>
        /// Inserisco un record nuovo nella tabella quote_guadagno
        /// </summary>
        public static readonly string InsertRecordQuote_Guadagno = "INSERT INTO quote_guadagno (id_quota, id_gestione, id_quote_periodi, quota) values (" +
            "null, @id_gestione, @id_quote_periodi, @quota)";

        /// <summary>
        /// Elimino un record della tabella quote_guadagno
        /// </summary>
        public static readonly string DeleteRecordGuadagno_Totale_anno = "DELETE FROM guadagni_totale_anno WHERE id_conto_corrente = @id_conto_corrente ";

        /// <summary>
        /// Esporta tutti i record della quote prendendo anche le descrizioni
        /// </summary>
        public static readonly string GetQuoteTab = GetTableQuote + OrderBy + ADataMovimento + " DESC" + Comma + AIdQuoteInv;

        /// <summary>
        /// Aggiungo questo pezzo di script alla precendente stringa
        /// </summary>
        public static readonly string GetLastQuoteTab = GetTableQuote + OrderBy + AIdQuoteInv + DescLimit1;

        /// <summary> Verifico se nella data di inserimento è già presente un investimento</summary>
        public static readonly string VerifyInvestmentDate = "SELECT IFNULL(SUM(A.id_periodo_quote), -1) AS result FROM quote_periodi A " +
            "WHERE A.id_aggregazione = @id_tipo_soldi AND A.data_inizio = @data_inizio; ";

        /// <summary>Inserisce un nuovo record nella tabella quote_investimenti</summary>
        public static readonly string InsertInvestment = "INSERT INTO quote_investimenti (id_quote_inv, id_gestione, id_tipo_movimento, id_periodo_quote, data_movimento, ammontare, note) " +
            "VALUES (null, @id_gestione, @id_tipo_movimento, @id_periodo_quote, @data_movimento, @ammontare, @note)";

        /// <summary>
        /// Calcola e restituisce il totale (somma algebrica) di quanto investito da un soggetto
        /// </summary>
        public static readonly string GetInvestmentByIdGestione = "SELECT SUM(ammontare) AS totale FROM quote_investimenti A WHERE A.id_tipo_movimento <> 12 AND id_gestione = @id_gestione";

        public static readonly string GetIdQuoteTab = "SELECT id_quote_inv FROM quote_investimenti A WHERE A.id_gestione = @id_gestione AND A.id_tipo_movimento = @id_tipo_movimento " +
            "AND A.id_periodo_quote = @id_periodo_quote ";

        /// <summary>
        /// Modifica un record nella tabella quote_investimenti
        /// </summary>
        public static readonly string UpdateQuoteTab = "UPDATE quote_investimenti SET id_gestione = @id_gestione, id_tipo_movimento = @id_tipo_movimento, data_movimento = @data_movimento, " +
            "ammontare = @ammontare, note = @note WHERE id_quote_inv = @id_quote_inv";

        /// <summary>
        /// Elimina un record nella tabella quote_investimenti
        /// </summary>
        public static readonly string DeleteRecordQuoteTab = "DELETE FROM quote_investimenti WHERE id_quote_inv = @id_quote_inv";

        /// <summary>
        /// Elimina un record dalla tabella conto_corrente sulla base di una eliminazione fatta sul portafoglio titoli
        /// </summary>
        public static readonly string DeleteContoCorrenteByIdPortafoglioTitoli = "DELETE FROM conto_corrente WHERE id_portafoglio_titoli = @id_portafoglio_titoli";

        /// <summary>
        /// Trovo quanta disponibilità di utili ci sono
        /// </summary>
        public static readonly string VerifyDisponibilitaUtili = "SELECT SUM(guadagnato + prelevato) + @daInserire FROM guadagni_totale_anno WHERE id_valuta = @id_valuta AND id_gestione = @id_gestione AND anno = @anno";
        /// <summary>
        /// Registro la cifra prelevata
        /// </summary>
        public static readonly string InsertPrelievoUtili = "INSERT INTO guadagni_totale_anno (id_gestione, id_tipo_movimento, id_valuta, anno, prelevato, data_operazione, causale) " +
            "VALUES (@id_gestione, @id_tipo_movimento, @id_valuta, @anno, @ammontare, @data_operazione, @causale)";

        public static readonly string InsertPrelievoUtiliBkd = "INSERT INTO prelievi (id_prelievo, id_gestione, id_tipo_movimento, id_valuta, anno, prelevato, data_operazione, causale) " +
            "VALUES (@id_prelievo, @id_gestione, @id_tipo_movimento, @id_valuta, @anno, @ammontare, @data_operazione, @causale)";

        /// <summary>Registro la modifica sui prelievi di utili</summary>
        public static readonly string UpdatePrelievoUtili = "UPDATE guadagni_totale_anno SET id_gestione = @id_gestione, anno = @anno, prelevato = @prelevato, data_operazione = @data, " +
            "causale = @causale WHERE id_guadagno = @id_guadagno ";
        public static readonly string UpdatePrelievoUtiliBkd = "UPDATE prelievi SET id_gestione = @id_gestione, anno = @anno, prelevato = @prelevato, data_operazione = @data, " +
            "causale = @causale WHERE id_prelievo = @id_prelievo ";

        /// <summary> Elimino un recordo di prelievo utili </summary>
        public static readonly string DeletePrelievoUtiliBKd = "DELETE FROM prelievi WHERE id_prelievo = @id_prelievo;";
        public static readonly string DeletePrelievoUtili = "DELETE FROM guadagni_totale_anno WHERE id_guadagno = @id_guadagno;";


        /// <summary>Dopo l'inserimento sul conto corrente, registro il guadagno</summary>
        public static readonly string AddSingoloGuadagno = "INSERT INTO guadagni_totale_anno (id_gestione, id_tipo_soldi, id_tipo_movimento, anno, quota, guadagnato, id_valuta, data_operazione, " +
            "causale, id_quote_periodi, id_conto_corrente) ( SELECT B.id_gestione, id_tipo_soldi, id_tipo_movimento, YEAR(data_movimento) AS anno, " +
            "case when B.id_gestione = 4 AND A.id_tipo_movimento = 8 then 0 ELSE (case when A.id_tipo_movimento = 8 then 0.5 ELSE B.quota END) END AS quota, " +
            "case when B.id_gestione = 4 AND A.id_tipo_movimento = 8 then 0 ELSE (case when A.id_tipo_movimento = 8 then 0.5 * A.ammontare ELSE (case when id_tipo_soldi = 11 then A.ammontare* B.quota * -1 ELSE A.ammontare*B.quota END) END) END AS guadagnato, " +
            "id_valuta, data_movimento, causale, A.id_quote_periodi, A.id_fineco_euro FROM conto_corrente A, quote_guadagno B WHERE A.id_quote_periodi = B.id_quote_periodi AND A.id_fineco_euro = " +
            "(SELECT id_fineco_euro FROM conto_corrente C WHERE C.id_tipo_movimento = @id_tipo_movimento AND id_tipo_soldi = @id_tipo_soldi AND id_quote_periodi = @id_quote_periodi " +
            "ORDER BY id_fineco_euro DESC LIMIT 1) GROUP BY B.id_gestione); ";

        /// <summary>Dopo la modifica sul conto corrente, registro la modifica sul guadagno totale anno</summary>
        public static readonly string ModifySingoloGuadagno = " UPDATE guadagni_totale_anno AA, (SELECT B.id_gestione, id_tipo_soldi, id_tipo_movimento, YEAR(data_movimento) AS anno, " +
            "case when B.id_gestione = 4 AND A.id_tipo_movimento = 8 then 0 ELSE(case when A.id_tipo_movimento = 8 then 0.5 ELSE B.quota END) END AS quota, " +
            "case when B.id_gestione = 4 AND A.id_tipo_movimento = 8 then 0 ELSE(case when A.id_tipo_movimento = 8 then 0.5 * A.ammontare ELSE A.ammontare* B.quota END) END AS guadagnato, " +
            "id_valuta, data_movimento, causale, A.id_quote_periodi, A.id_fineco_euro FROM conto_corrente A, quote_guadagno B WHERE A.id_quote_periodi = B.id_quote_periodi AND A.id_fineco_euro = @id_fineco_euro GROUP BY B.id_gestione ) BB " +
            "SET AA.anno = BB.anno, AA.guadagnato = BB.guadagnato, AA.data_operazione = BB.data_movimento, AA.causale = BB.causale, AA.id_quote_periodi = BB.id_quote_periodi, AA.id_tipo_soldi = BB.id_tipo_soldi, AA.id_valuta = BB.id_valuta " +
            "WHERE AA.id_gestione = BB.id_gestione AND AA.id_conto_corrente = BB.id_fineco_euro;";

        public static readonly string GetCostiMediPerTitolo = "SELECT C.nome_gestione, D.desc_conto, B.id_tipo_titolo, E.desc_tipo_titolo, B.desc_titolo, B.isin, " +
            "SUM(ammontare +(total_commission + tobin_tax + disaggio_cedole + ritenuta_fiscale)*-1) AS CostoMedio, SUM(shares_quantity) AS TitoliAttivi, " +
            "SUM(ammontare + (total_commission + tobin_tax + disaggio_cedole + ritenuta_fiscale) * -1) / SUM(shares_quantity) AS CostoUnitarioMedio " +
            "FROM portafoglio_titoli A, titoli B, gestioni C, conti D, tipo_titoli E " +
            "WHERE A.id_gestione<> 0 AND attivo > 0 AND A.id_tipo_movimento <> 6 AND A.id_titolo = B.id_titolo AND A.id_gestione = C.id_gestione AND A.id_conto = D.id_conto AND B.id_tipo_titolo = E.id_tipo_titolo " +
            "GROUP BY A.id_gestione, A.id_conto, E.id_tipo_titolo, A.id_titolo " +
            "ORDER BY A.id_gestione, A.id_conto, E.desc_tipo_titolo, B.desc_titolo";

        public static readonly string GetMovimentiContoGestioneValuta = "DROP TEMPORARY TABLE if EXISTS movimenti_conto; " +
            "CREATE TEMPORARY TABLE movimenti_conto (`id_fineco_euro` int(10) unsigned NOT NULL DEFAULT 0, `desc_conto` VARCHAR(50) DEFAULT NULL, `nome_gestione` VARCHAR(100) DEFAULT NULL," +
            "`desc_movimento` VARCHAR(50) DEFAULT NULL, `desc_tipo_titolo` VARCHAR(100) DEFAULT NULL, `desc_titolo` VARCHAR(100) DEFAULT NULL, `isin` VARCHAR(25) DEFAULT NULL, `id_valuta` int(10) unsigned NOT NULL DEFAULT 0, " +
            "`data_movimento` date NOT NULL, `ammontare` double NOT NULL, `causale` varchar(250) DEFAULT NULL, `desc_tipo_soldi` VARCHAR(50) DEFAULT NULL); " +
            "INSERT INTO movimenti_conto (id_fineco_euro, desc_conto, nome_gestione, desc_movimento, desc_tipo_titolo, desc_titolo, isin, id_valuta, data_movimento, ammontare, causale, desc_tipo_soldi) " +
            "SELECT 0 as id_fineco_euro, B.desc_conto, C.nome_gestione, 'Totale' as desc_movimento, '' as desc_tipo_titolo, 'Riporto al:' as desc_titolo, '' as isin, A.id_valuta, data_movimento, " +
            "SUM(ammontare) OVER(ORDER BY A.data_movimento, A.id_fineco_euro)  AS ammontare, '' as causale, '' as desc_tipo_soldi FROM conto_corrente A, conti B, gestioni C, tipo_movimento D, titoli E, tipo_titoli F, tipo_soldi G " +
            "WHERE A.id_conto = B.id_conto AND A.id_gestione = C.id_gestione AND A.id_tipo_movimento = D.id_tipo_movimento AND A.id_titolo = E.id_titolo AND E.id_tipo_titolo = F.id_tipo_titolo AND A.id_tipo_soldi = G.id_tipo_soldi " +
            "AND A.id_tipo_soldi <> 11 AND A.id_conto = @IdConto AND A.id_gestione = @IdGestione AND A.id_valuta = @IdCurrency AND year(A.data_movimento) <= @Year_1 ORDER BY A.data_movimento DESC, A.id_fineco_euro DESC LIMIT 1; " +
            "INSERT INTO movimenti_conto SELECT A.id_fineco_euro, B.desc_conto, C.nome_gestione, D.desc_movimento, F.desc_tipo_titolo, E.desc_titolo, E.isin, A.id_valuta, data_movimento, ammontare, causale, G.desc_tipo_soldi " +
            "FROM conto_corrente A, conti B, gestioni C, tipo_movimento D, titoli E, tipo_titoli F, tipo_soldi G WHERE A.id_conto = B.id_conto AND A.id_gestione = C.id_gestione AND A.id_tipo_movimento = D.id_tipo_movimento " +
            "AND A.id_titolo = E.id_titolo  AND E.id_tipo_titolo = F.id_tipo_titolo AND A.id_tipo_soldi = G.id_tipo_soldi AND A.id_tipo_soldi <> 11 AND A.id_conto = @IdConto AND A.id_gestione = @IdGestione AND A.id_valuta = @IdCurrency " +
            "AND year(A.data_movimento) = @Year ORDER BY A.id_conto, A.id_gestione, A.data_movimento DESC, A.id_fineco_euro DESC; " +
            "SELECT A.id_fineco_euro, desc_conto, nome_gestione, desc_movimento, desc_tipo_titolo, desc_titolo, isin, B.desc_valuta, data_movimento, CASE WHEN ammontare > 0 THEN ammontare ELSE 0 END AS ENTRATE, " +
            "CASE WHEN ammontare < 0 THEN ammontare ELSE 0 END AS USCITE, SUM(ammontare) OVER(ORDER BY A.data_movimento, A.id_fineco_euro)  AS CUMULATO, causale, desc_tipo_soldi FROM `movimenti_conto` A, valuta B " +
            "WHERE A.id_valuta = B.id_valuta ORDER BY A.data_movimento DESC, A.id_fineco_euro DESC;";
    }
}

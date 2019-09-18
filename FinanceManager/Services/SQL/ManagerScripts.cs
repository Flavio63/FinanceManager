﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services.SQL
{
    public class ManagerScripts
    {
        private static readonly string DescLimit1 = " DESC LIMIT 1 ";
        private static readonly string AndTipoMovimento = " AND A.id_tipo_movimento = @id_tipo_movimento ";
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
            "ROUND(SUM(CASE WHEN id_tipo_soldi = 4 THEN ammontare ELSE 0 END), 2) AS Cedole, " +
            "ROUND(SUM(CASE WHEN id_tipo_soldi = 15 THEN ammontare ELSE 0 END), 2) AS Utili, " +
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
        /// Aggiunge un movimento
        /// </summary>
        public static readonly string AddManagerLiquidAsset = "INSERT INTO portafoglio_titoli (id_portafoglio_titoli, id_gestione, id_conto, id_valuta, id_tipo_movimento, " +
            "id_titolo, data_movimento, ammontare, shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, " +
            "valore_cambio, note, attivo, link_movimenti) " +
            "VALUE (null, @id_gestione, @id_conto, @id_valuta, @id_tipo_movimento, @id_titolo, @data_movimento, @ammontare, @shares_quantity, " +
            "@unity_local_value, @total_commission, @tobin_tax, @disaggio_cedole, @ritenuta_fiscale, @valore_cambio, @note, @attivo, @link_movimenti);";

        /// <summary>
        /// Aggiorna un movimento
        /// </summary>
        public static readonly string UpdateManagerLiquidAsset = "UPDATE portafoglio_titoli SET id_gestione = @id_gestione, id_conto = @id_conto, id_valuta = @id_valuta, " +
            "id_tipo_movimento = @id_tipo_movimento, id_titolo = @id_titolo, data_movimento = @data_movimento, ammontare = @ammontare, shares_quantity = @shares_quantity, " +
            "unity_local_value = @unity_local_value, total_commission = @total_commission, tobin_tax = @tobin_tax, disaggio_cedole = @disaggio_cedole, " +
            "ritenuta_fiscale = @ritenuta_fiscale, valore_cambio = @valore_cambio, " +
            "note = @note, attivo = @attivo, link_movimenti = @link_movimenti WHERE id_portafoglio_titoli = @id_portafoglio_titoli";
        
        /// <summary>
        /// Cancella un movimento
        /// </summary>
        public static readonly string DeleteManagerLiquidAsset = "DELETE FROM portafoglio_titoli WHERE id_portafoglio_titoli = @id_portafoglio_titoli;";

        public static readonly string GetShareMovements = "SELECT id_portafoglio_titoli, B.id_gestione, nome_gestione, C.id_conto, desc_conto, D.id_valuta, " +
            "cod_valuta, E.id_tipo_movimento, desc_Movimento, A.id_titolo, F.desc_titolo, F.isin, F.id_tipo_titolo, H.desc_tipo, F.id_azienda, I.desc_azienda, data_movimento, " +
            "shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, A.note " +
            "FROM portafoglio_titoli A, gestioni B, conti C, valuta D, tipo_movimento E, titoli F, tipo_titoli H, aziende I " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_conto = C.id_conto AND A.id_valuta = D.id_valuta AND A.id_tipo_movimento = E.id_tipo_movimento AND A.id_titolo = F.id_titolo " +
            "AND F.id_tipo_titoli = H.id_tipo_titoli AND F.id_azienda = I.id_azienda AND B.id_portafoglio_titoli = @owner AND C.id_conto = @id_conto AND id_titolo = @id_titolo " +
            "AND (id_movimento = 5 or id_movimento = 6) " +
            "ORDER BY data_movimento";

        public static readonly string GetShareActiveAndAccountMovement = "SELECT AA.id_portafoglio_titoli, AA.id_gestione, AA.id_conto, AA.id_valuta, AA.id_tipo_movimento, AA.id_titolo, " +
            "AA.data_movimento, AA.ammontare AS ValoreAzione, AA.shares_quantity, AA.unity_local_value, AA.total_commission, AA.tobin_tax, AA.disaggio_cedole, AA.ritenuta_fiscale, AA.valore_cambio, " +
            "AA.note, BB.id_fineco_euro, BB.ammontare AS Valore_in_CC, BB.id_tipo_soldi FROM portafoglio_titoli AA LEFT JOIN conto_corrente BB ON AA.id_portafoglio_titoli = BB.id_portafoglio_titoli " +
            "WHERE AA.id_conto = @id_conto AND AA.id_gestione = @id_gestione AND AA.id_titolo = @id_titolo AND AA.attivo = 1 ORDER BY AA.data_movimento";

        public static readonly string InsertAccountMovement = "INSERT INTO conto_corrente (id_conto, id_quote_investimenti, id_valuta, id_portafoglio_titoli, id_tipo_movimento, " +
            "id_gestione, id_titolo, data_movimento, ammontare, cambio, causale, id_tipo_soldi) VALUE ( @id_conto, @id_quote_investimenti, @id_valuta, @id_portafoglio_titoli, @id_tipo_movimento, " +
            "@id_gestione, @id_titolo, @data_movimento, @ammontare, @cambio, @causale, @id_tipo_soldi)";

        protected static readonly string ContoCorrente = "SELECT id_fineco_euro, A.id_conto, B.desc_conto, id_quote_investimenti, A.id_valuta, C.cod_valuta, id_portafoglio_titoli, A.id_tipo_movimento, " +
            "D.desc_movimento, A.id_gestione, E.nome_gestione, A.id_titolo, F.isin, F.desc_titolo, data_movimento, ammontare, cambio, causale, A.id_tipo_soldi, G.desc_tipo_soldi " +
            "FROM conto_corrente A, conti B, valuta C, tipo_movimento D, gestioni E, titoli F, tipo_soldi G " +
            "WHERE A.id_conto = B.id_conto AND A.id_valuta = C.id_valuta AND A.id_tipo_movimento = D.id_tipo_movimento AND " +
            "A.id_gestione = E.id_gestione AND A.id_titolo = F.id_titolo AND A.id_tipo_soldi = G.id_tipo_soldi AND id_fineco_euro > 0 ";


        public static readonly string GetContoCorrente = ContoCorrente + OrderBy + DataMovimentoDesc + Comma + AId_fineco_euro;
        public static readonly string GetContoCorrenteByIdCC = ContoCorrente + AndIdFinecoEuro;
        public static readonly string GetContoCorrenteByIdQuote = ContoCorrente + AndQuote + OrderBy + ADataMovimento;
        public static readonly string GetContoCorrenteByMovement = ContoCorrente + AndTipoMovimento + OrderBy + ADataMovimento + Comma + AId_fineco_euro;
        public static readonly string GetContoCorrenteByIdPortafoglio = ContoCorrente + AndIdPortafoglioTitoli + OrderBy + AId_fineco_euro;

        public static readonly string UpdateContoCorrenteByIdQuote = "UPDATE conto_corrente SET id_conto = @id_conto, id_valuta = @id_valuta, id_portafoglio_titoli = @id_portafoglio_titoli, " +
            "id_tipo_movimento = @id_tipo_movimento, id_gestione = @id_gestione, id_titolo = @id_titolo, data_movimento = @data_movimento, ammontare = @ammontare, " +
            "cambio = @cambio, causale = @causale, id_tipo_soldi = @id_tipo_soldi WHERE id_quote_investimenti = @id_quote_investimenti";

        public static readonly string UpdateContoCorrenteByIdPortafoglioTitoli = "UPDATE conto_corrente SET id_conto = @id_conto, id_valuta = @id_valuta, id_quote_investimenti = @id_quote_investimenti, " +
            "id_tipo_movimento = @id_tipo_movimento, id_gestione = @id_gestione, id_titolo = @id_titolo, data_movimento = @data_movimento, ammontare = @ammontare, " +
            "cambio = @cambio, causale = @causale, id_tipo_soldi = @id_tipo_soldi WHERE id_portafoglio_titoli = @id_portafoglio_titoli";

        public static readonly string UpdateContoCorrenteByIdCC = "UPDATE conto_corrente SET id_conto = @id_conto, id_quote_investimenti = @id_quote_investimenti, id_valuta = @id_valuta, " +
            "id_portafoglio_titoli = @id_portafoglio_titoli, id_tipo_movimento = @id_tipo_movimento, id_gestione = @id_gestione, id_titolo = @id_titolo, data_movimento = @data_movimento, " +
            "ammontare = @ammontare, cambio = @cambio, causale = @causale, id_tipo_soldi = @id_tipo_soldi WHERE id_fineco_euro = @id_fineco_euro";

        public static readonly string DeleteAccount = "DELETE FROM conto_corrente WHERE id_fineco_euro = @id_fineco_euro";

        /// <summary>
        /// calcola le quote per investitore
        /// </summary>
        public static readonly string GetQuoteInv = "SELECT AA.nome_gestione, ROUND(SUM(CASE WHEN id_tipo_movimento = 1 THEN ammontare ELSE 0 END), 2) AS CapitaleImmesso, " +
            "TotaleImmesso, ROUND(SUM(CASE WHEN id_tipo_movimento = 2 THEN ammontare ELSE 0 END), 2) AS CapitalePrelevato, TotalePrelevato, " +
            "ROUND(SUM(CASE WHEN id_tipo_movimento = 1 OR id_tipo_movimento = 2 THEN ammontare ELSE 0 END), 2) AS CapitaleAttivo, TotaleAttivo, " +
            "ROUND(SUM(CASE WHEN id_tipo_movimento = 1 OR id_tipo_movimento = 2 THEN ammontare ELSE 0 END) / TotaleAttivo, 6) AS QuotaInv, " +
            "ROUND(SUM(CASE WHEN id_tipo_movimento = 12 THEN ammontare ELSE 0 END), 2) * -1 AS CapitaleAssegnato, TotaleAssegnato, " +
            "ROUND(SUM(CASE WHEN id_tipo_movimento = 1 OR id_tipo_movimento = 2 THEN ammontare ELSE 0 END), 2) + ROUND(SUM(CASE WHEN id_tipo_movimento = 12 THEN ammontare ELSE 0 END), 2) AS CapitaleDisponibile, " +
            "TotaleDisponibile FROM quote_investimenti BB, gestioni AA, (SELECT ROUND(SUM(CASE WHEN id_tipo_movimento = 1 THEN ammontare ELSE 0 END), 2) AS TotaleImmesso, " +
            "ROUND(SUM(CASE WHEN id_tipo_movimento = 2 THEN ammontare ELSE 0 END), 2) AS TotalePrelevato, " +
            "ROUND(SUM(CASE WHEN id_tipo_movimento = 1 OR id_tipo_movimento = 2 THEN ammontare ELSE 0 END), 2) AS TotaleAttivo, " +
            "ROUND(SUM(CASE WHEN id_tipo_movimento = 12 THEN ammontare ELSE 0 END), 2) * -1 AS TotaleAssegnato, " +
            "ROUND(SUM(CASE WHEN id_tipo_movimento = 1 OR id_tipo_movimento = 2 THEN ammontare ELSE 0 END), 2) + ROUND(SUM(CASE WHEN id_tipo_movimento = 12 THEN ammontare ELSE 0 END), 2) AS TotaleDisponibile " +
            "FROM quote_investimenti) AS A WHERE BB.id_gestione = AA.id_gestione AND BB.id_gestione > 0 GROUP BY BB.id_gestione ORDER BY BB.id_gestione DESC";

        /// <summary>
        /// calcola le quote per investitore del guadagno
        /// in base al periodo di validità delle quote di investimento
        /// </summary>
        public static readonly string GetQuoteDettaglioGuadagno = "SELECT D.nome_gestione, A.data_inizio, A.data_fine, B.quota, SUM(case when C.id_tipo_soldi = 4 then C.ammontare ELSE 0 END) * B.quota AS cedole, " +
            "SUM(case when C.id_tipo_soldi = 15 AND C.id_gestione <> 7 then ammontare ELSE 0 END) * B.quota AS utili, SUM(case when C.id_tipo_soldi = 15 AND C.id_gestione = 7 then ammontare ELSE 0 END) * 0.5 AS volatili, " +
            "SUM(case when (C.id_tipo_soldi = 4 OR C.id_tipo_soldi = 15) AND C.id_gestione <> 7 then ammontare ELSE 0 END) * B.quota + sum(case when C.id_tipo_soldi = 15 AND C.id_gestione = 7 then ammontare ELSE 0 END) * 0.5 AS totale " +
            "FROM quote_periodi A, quote_guadagno B, conto_corrente C , gestioni D WHERE A.id_periodo_quote = B.id_quote_periodi AND B.id_quote_periodi = C.id_quote_periodi AND B.id_gestione = D.id_gestione " +
            "GROUP BY B.id_gestione, B.id_quote_periodi ORDER BY A.data_inizio, D.nome_gestione;";

        public static readonly string GetQuoteSintesiGuadagno = "SELECT D.nome_gestione, YEAR(A.data_inizio) AS anno, SUM(case when C.id_tipo_soldi = 4 then C.ammontare ELSE 0 END) * B.quota AS cedole, " +
            "SUM(case when C.id_tipo_soldi = 15 AND C.id_gestione <> 7 then ammontare ELSE 0 END) * B.quota AS utili, SUM(case when C.id_tipo_soldi = 15 AND C.id_gestione = 7 then ammontare ELSE 0 END) * 0.5 AS volatili, " +
            "SUM(case when (C.id_tipo_soldi = 4 OR C.id_tipo_soldi = 15) AND C.id_gestione <> 7 then ammontare ELSE 0 END) * B.quota + sum(case when C.id_tipo_soldi = 15 AND C.id_gestione = 7 then ammontare ELSE 0 END) * 0.5 AS totale " +
            "FROM quote_periodi A, quote_guadagno B, conto_corrente C , gestioni D WHERE A.id_periodo_quote = B.id_quote_periodi AND B.id_quote_periodi = C.id_quote_periodi AND B.id_gestione = D.id_gestione " +
            "GROUP BY B.id_gestione, anno ORDER BY anno, D.nome_gestione;";

        /// <summary>
        /// Esporta tutti i record della quote prendendo anche le descrizioni
        /// </summary>
        public static readonly string GetQuoteTab = GetTableQuote + OrderBy + ADataMovimento + Comma + AIdQuoteInv;

        /// <summary>
        /// Aggiungo questo pezzo di script alla precendente stringa
        /// </summary>
        public static readonly string GetLastQuoteTab = GetTableQuote + OrderBy + AIdQuoteInv + DescLimit1;

        /// <summary>
        /// Inserisce un nuovo record nella tabella quote_investimenti
        /// </summary>
        public static readonly string InsertInvestment = "INSERT INTO quote_investimenti (id_quote_inv, id_gestione, id_tipo_movimento, data_movimento, ammontare, note) " +
            "VALUES (null, @id_gestione, @id_tipo_movimento, @data_movimento, @ammontare, @note)";

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
    }
}

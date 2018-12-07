using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services.SQL
{
    public class ManagerScripts
    {
        public static readonly string SrchGestione = " id_gestione = {0} ";
        public static readonly string SrchCurrency = " id_valuta = {0} ";
        public static readonly string SrchMovementType = " id_tipo_movimento = {0} ";
        public static readonly string NotMovementType = " id_tipo_movimento <> {0} ";
        /// <summary>
        /// Data una gestione estrae il totale cedole, utili e disponibilità
        /// per c/c e per valuta
        /// </summary>
        public static readonly string GetCurrencyAvailable = "SELECT C.desc_conto, B.cod_valuta, " +
            "ROUND(SUM(CASE WHEN id_tipo_movimento = 4 THEN ammontare ELSE 0 END), 2) AS Cedole, " +
            "ROUND(SUM(CASE WHEN id_tipo_movimento = 15 THEN ammontare ELSE 0 END), 2) AS Utili, " +
            "ROUND(SUM(CASE WHEN id_tipo_movimento <> 4 AND id_tipo_movimento <> 15 THEN ammontare ELSE 0 END), 2) AS Disponibili " +
            "FROM conto_corrente A, valuta B, conti C WHERE A.id_conto = C.id_conto and A.id_valuta = B.id_valuta and A.id_gestione = @id_gestione " +
            "GROUP BY A.id_conto, A.id_valuta ";

        /// <summary>
        /// Data una gestione estrae tutti i record
        /// </summary>
        public static readonly string GetManagerLiquidAssetListByOwnerAndLocation = "SELECT id_portafoglio_titoli, AA.id_gestione, nome_gestione, id_conto, desc_conto, AA.id_valuta, " +
            "cod_valuta, id_tipo_movimento, desc_Movimento, BB.id_tipo_titolo, EE.desc_tipo_titolo, BB.id_azienda, desc_azienda, AA.id_titolo, BB.desc_titolo, BB.Isin, data_movimento, " +
            "ammontare, shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, valore_cambio, note FROM ( " +
            "SELECT id_portafoglio_titoli, B.id_gestione, nome_gestione, C.id_conto, desc_conto, D.id_valuta, cod_valuta, E.id_tipo_movimento, desc_Movimento, A.id_titolo, " +
            "data_movimento, shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, note " +
            "FROM portafoglio_titoli A, gestioni B, conti C, valuta D, tipo_movimento E " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_conto = C.id_conto AND A.id_valuta = D.id_valuta AND A.id_tipo_movimento = E.id_tipo_movimento AND B.id_gestione = @gestione " +
            "AND A.id_conto = @id_conto) AA LEFT JOIN titoli BB ON BB.id_titolo = AA.id_titolo left join aziende DD ON BB.id_azienda = DD.id_azienda " +
            "Left Join tipo_titoli EE ON BB.id_tipo_titolo = EE.id_tipo_titolo " +
            "ORDER BY data_movimento DESC, id_tipo_movimento";

        /// <summary>
        /// Data una gestione e una location, estrae tutti i record relativi ai tipi di movimenti richiesti
        /// </summary>
        public static readonly string GetManagerLiquidAssetByOwnerLocationAndMovementType = "SELECT id_portafoglio_titoli, AA.id_gestione, nome_gestione, id_conto, desc_conto, AA.id_valuta, " +
            "cod_valuta, id_tipo_movimento, desc_Movimento, BB.id_tipo_titolo, EE.desc_tipo_titolo, BB.id_azienda, desc_azienda, AA.id_titolo, BB.desc_titolo, BB.Isin, data_movimento, " +
            "ammontare, shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, valore_cambio, note FROM ( " +
            "SELECT id_portafoglio_titoli, B.id_gestione, nome_gestione, C.id_conto, desc_conto, D.id_valuta, cod_valuta, E.id_tipo_movimento, desc_Movimento, A.id_titolo, " +
            "data_movimento, shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, note " +
            "FROM portafoglio_titoli A, gestioni B, conti C, valuta D, tipo_movimento E " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_conto = C.id_conto AND A.id_valuta = D.id_valuta AND A.id_tipo_movimento = E.id_tipo_movimento AND B.id_gestione = @id_gestione " +
            "AND C.id_conto = @id_conto AND {0} ) AA LEFT JOIN titoli BB ON BB.id_titolo = AA.id_titolo left join aziende DD ON BB.id_azienda = DD.id_azienda " +
            "Left Join tipo_titoli EE ON BB.id_tipo_titolo = EE.id_tipo_titolo " +
            "ORDER BY data_movimento DESC";

        /// <summary>
        /// Data una gestione e una location, estrae tutti i record relativi ai movimenti di titoli
        /// </summary>
        public static readonly string GetManagerSharesMovementByOwnerAndLocation = "SELECT id_portafoglio_titoli, B.id_gestione, nome_gestione, C.id_conto, desc_conto, D.id_valuta, " +
            "cod_valuta, E.id_tipo_movimento, desc_Movimento, A.id_titolo, F.desc_titolo, F.isin, F.id_tipo_titolo, H.desc_tipo_titolo, F.id_azienda, I.desc_azienda, data_movimento, " +
            "shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, note " +
            "FROM portafoglio_titoli A, gestioni B, conti C, valuta D, tipo_movimento E, titoli F, tipo_titoli H, aziende I " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_conto = C.id_conto AND A.id_valuta = D.id_valuta AND A.id_tipo_movimento = E.id_tipo_movimento AND A.id_titolo = F.id_titolo " +
            "AND F.id_tipo_titolo = H.id_tipo_titolo AND F.id_azienda = I.id_azienda AND B.id_gestione = @id_gestione AND C.id_conto = @id_conto AND A.id_titolo IS NOT NULL " +
            "ORDER BY data_movimento DESC";

        /// <summary>
        /// Data una gestione e una location, estrae l'ultimo record caricato
        /// </summary>
        public static readonly string GetLastSharesMovementByOwnerAndLocation = "SELECT id_portafoglio_titoli, B.id_gestione, nome_gestione, C.id_conto, desc_conto, D.id_valuta, " +
            "cod_valuta, E.id_tipo_movimento, desc_Movimento, A.id_titolo, F.desc_titolo, F.isin, F.id_tipo_titolo, H.desc_tipo_titolo, F.id_azienda, I.desc_azienda, data_movimento, " +
            "shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, note " +
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
            "valore_cambio, note) " +
            "VALUE (null, @id_gestione, @id_conto, @id_valuta, @id_tipo_movimento, @id_titolo, @data_movimento, @ammontare, @shares_quantity, " +
            "@unity_local_value, @total_commission, @tobin_tax, @disaggio_cedole, @ritenuta_fiscale, @valore_cambio, @note);";

        /// <summary>
        /// Aggiorna un movimento
        /// </summary>
        public static readonly string UpdateManagerLiquidAsset = "UPDATE portafoglio_titoli SET id_gestione = @id_gestione, id_conto = @id_conto, id_valuta = @id_valuta, " +
            "id_tipo_movimento = @id_tipo_movimento, id_titolo = @id_titolo, data_movimento = @data_movimento, ammontare = @ammontare, shares_quantity = @shares_quantity, " +
            "unity_local_value = @unity_local_value, total_commission = @total_commission, tobin_tax = @tobin_tax, disaggio_cedole = @disaggio_cedole, " +
            "ritenuta_fiscale = @ritenuta_fiscale, valore_cambio = @valore_cambio, " +
            "note = @note WHERE id_portafoglio_titoli = @id_portafoglio_titoli";
        
        /// <summary>
        /// Cancella un movimento
        /// </summary>
        public static readonly string DeleteManagerLiquidAsset = "DELETE FROM portafoglio_titoli WHERE id_portafoglio_titoli = @id_portafoglio_titoli;";

        public static readonly string GetShareMovements = "SELECT id_portafoglio_titoli, B.id_gestione, nome_gestione, C.id_conto, desc_conto, D.id_valuta, " +
            "cod_valuta, E.id_tipo_movimento, desc_Movimento, A.id_titolo, F.desc_titolo, F.isin, F.id_tipo_titolo, H.desc_tipo, F.id_azienda, I.desc_azienda, data_movimento, " +
            "shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, note " +
            "FROM portafoglio_titoli A, gestioni B, conti C, valuta D, tipo_movimento E, titoli F, tipo_titoli H, aziende I " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_conto = C.id_conto AND A.id_valuta = D.id_valuta AND A.id_tipo_movimento = E.id_tipo_movimento AND A.id_titolo = F.id_titolo " +
            "AND F.id_tipo_titoli = H.id_tipo_titoli AND F.id_azienda = I.id_azienda AND B.id_portafoglio_titoli = @owner AND C.id_conto = @id_conto AND id_titolo = @id_titolo " +
            "AND (id_movimento = 5 or id_movimento = 6) " +
            "ORDER BY data_movimento";

        public static readonly string InsertAccountMovement = "INSERT INTO conto_corrente (id_conto, id_quote_investimenti, id_valuta, id_portafoglio_titoli, id_tipo_movimento, " +
            "id_gestione, id_titolo, data_movimento, ammontare, cambio, causale) VALUE ( @id_conto, @id_quote_investimenti, @id_valuta, @id_portafoglio_titoli, @id_tipo_movimento, " +
            "@id_gestione, @id_titolo, @data_movimento, @ammontare, @cambio, @causale)";

        protected static readonly string ContoCorrente = "SELECT id_fineco_euro, A.id_conto, B.desc_conto, id_quote_investimenti, A.id_valuta, C.cod_valuta, A.id_tipo_movimento, " +
            "D.desc_movimento, A.id_gestione, E.nome_gestione, A.id_titolo, F.isin, F.desc_titolo, data_movimento, ammontare, cambio, causale FROM conto_corrente A, conti B, " +
            "valuta C, tipo_movimento D, gestioni E, titoli F WHERE A.id_conto = B.id_conto AND A.id_valuta = C.id_valuta AND A.id_tipo_movimento = D.id_tipo_movimento AND " +
            "A.id_gestione = E.id_gestione AND A.id_titolo = F.id_titolo AND id_fineco_euro > 0 ";

        protected static readonly string OrderByData = " ORDER BY data_movimento ";

        protected static readonly string Movimento = " AND A.id_tipo_movimento = @id_tipo_movimento ";
        protected static readonly string Quote = " AND A.id_quote_investimenti = @id_quote_investimenti";

        public static readonly string GetContoCorrente = ContoCorrente + OrderByData;
        public static readonly string GetContoCorrenteByIdQuote = ContoCorrente + Quote;
        public static readonly string GetContoCorrenteByMovement = ContoCorrente + Movimento + OrderByData;

        public static readonly string UpdateContoCorrenteByIdQuote = "UPDATE conto_corrente SET id_conto = @id_conto, id_valuta = @id_valuta, id_portafoglio_titoli = @id_portafoglio_titoli, " +
            "id_tipo_movimento = @id_tipo_movimento, id_gestione = @id_gestione, id_titolo = @id_titolo, data_movimento = @data_movimento, ammontare = @ammontare, " +
            "cambio = @cambio, causale = @causale WHERE id_quote_investimenti = @id_quote_investimenti";

        public static readonly string DeleteAccount = "DELETE FROM conto_corrente WHERE id_fineco_euro = @id_fineco_euro";

        /// <summary>
        /// calcola le quote per investitore
        /// </summary>
        public static readonly string GetQuote = "SELECT Investitori.Nome, ROUND(SUM(CASE WHEN id_tipo_movimento = 1 OR id_tipo_movimento = 2 THEN ammontare ELSE 0 END), 2) AS investito, " +
            "ROUND(SUM(CASE WHEN id_tipo_movimento = 1 OR id_tipo_movimento = 2 THEN ammontare ELSE 0 END) / totale * 100, 2) AS quota, totale, ROUND(SUM(ammontare), 2) AS disponibili, " +
            "tot_disponibile FROM (SELECT ROUND(SUM(CASE WHEN id_tipo_movimento = 1 OR id_tipo_movimento = 2 THEN ammontare ELSE 0 END), 2) AS totale, ROUND(SUM(ammontare), 2) AS tot_disponibile " +
            "FROM quote_investimenti) A, quote_investimenti, Investitori WHERE quote_investimenti.id_investitore = Investitori.id_investitore AND quote_investimenti.id_quote_inv > 0 " +
            "GROUP BY quote_investimenti.id_investitore ";

        /// <summary>
        /// esporta tutti gli investitori
        /// </summary>
        public static readonly string GetInvestitori = "SELECT id_investitore, Nome FROM Investitori WHERE id_investitore > 0 ORDER BY id_investitore";

        /// <summary>
        /// Esporta tutti i record della quote prendendo anche le descrizioni
        /// </summary>
        public static readonly string GetQuoteTab = "SELECT id_quote_inv, A.id_investitore, B.Nome, A.id_tipo_movimento, C.desc_movimento, data_movimento, ammontare, note " +
            "FROM quote_investimenti A, Investitori B, tipo_movimento C WHERE A.id_investitore = B.id_investitore AND A.id_tipo_movimento = C.id_tipo_movimento AND id_quote_inv > 0 ";

        /// <summary>
        /// Aggiungo questo pezzo di script alla precendente stringa
        /// </summary>
        public static readonly string GetLastQuoteTab = GetQuoteTab + " ORDER BY id_quote_inv DESC LIMIT 1 ";

        /// <summary>
        /// Inserisce un nuovo record nella tabella quote_investimenti
        /// </summary>
        public static readonly string InsertInvestment = "INSERT INTO quote_investimenti (id_quote_inv, id_investitore, id_tipo_movimento, data_movimento, ammontare, note) " +
            "VALUES (null, @id_investitore, @id_tipo_movimento, @data_movimento, @ammontare, @note)";

        /// <summary>
        /// Modifica un record nella tabella quote_investimenti
        /// </summary>
        public static readonly string UpdateQuoteTab = "UPDATE quote_investimenti SET id_investitore = @id_investitore, id_tipo_movimento = @id_tipo_movimento, data_movimento = @data_movimento, " +
            "ammontare = @ammontare, note = @note WHERE id_quote_inv = @id_quote_inv";

        /// <summary>
        /// Elimina un record nella tabella quote_investimenti
        /// </summary>
        public static readonly string DeleteRecordQuoteTab = "DELETE FROM quote_investimenti WHERE id_quote_inv = @id_quote_inv";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services.SQL
{
    public class ManagerScripts
    {
        /// <summary>
        /// Data una gestione estrae tutti i record
        /// </summary>
        public static readonly string GetManagerLiquidAssetListByOwnerAndLocation = "SELECT id_portafoglio, AA.id_gestione, nome_gestione, id_conto, desc_conto, AA.id_valuta, " +
            "cod_valuta, id_tipo_movimento, desc_Movimento, BB.id_tipo_titolo, EE.desc_tipo, BB.id_azienda, desc_azienda, id_titolo, BB.desc_titolo, BB.Isin, data_movimento, " +
            "ammontare, shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, valore_cambio, profit_loss, disp, note FROM ( " +
            "SELECT id_portafoglio, B.id_gestione, nome_gestione, C.id_conto, desc_conto, D.id_valuta, cod_valuta, E.id_tipo_movimento, desc_Movimento, id_titolo, " +
            "data_movimento, shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, profit_loss, if(disponibile = 1, 'true', 'false') AS disp, note " +
            "FROM portafoglio A, gestioni B, conti C, valuta D, tipo_movimento E " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_conto = C.id_conto AND A.id_valuta = D.id_valuta AND A.id_tipo_movimento = E.id_tipo_movimento AND B.id_gestione = @gestione " +
            "AND A.id_conto = @id_conto) AA LEFT JOIN titoli BB ON BB.id_titolo = AA.id_titolo left join aziende DD ON BB.id_azienda = DD.id_azienda " +
            "Left Join tipo_titoli EE ON BB.id_tipo_titolo = EE.id_tipo_titolo " +
            "ORDER BY data_movimento DESC";

        /// <summary>
        /// Data una gestione e una location, estrae tutti i record relativi ai tipi di movimenti richiesti
        /// </summary>
        public static readonly string GetManagerLiquidAssetByOwnerLocationAndMovementType = "SELECT id_portafoglio, AA.id_gestione, nome_gestione, id_conto, desc_conto, AA.id_valuta, " +
            "cod_valuta, id_tipo_movimento, desc_Movimento, BB.id_tipo_titolo, EE.desc_tipo, BB.id_azienda, desc_azienda, id_titolo, BB.desc_titolo, BB.Isin, data_movimento, " +
            "ammontare, shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, valore_cambio, profit_loss, disp, note FROM ( " +
            "SELECT id_portafoglio, B.id_portafoglio, nome_gestione, C.id_conto, desc_conto, D.id_valuta, cod_valuta, E.id_tipo_movimento, desc_Movimento, id_titolo, " +
            "data_movimento, shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, profit_loss, if(disponibile = 1, 'true', 'false') AS disp, note " +
            "FROM portafoglio A, gestioni B, conti C, valuta D, tipo_movimento E " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_conto = C.id_conto AND A.id_valuta = D.id_valuta AND A.id_tipo_movimento = E.id_tipo_movimento AND B.id_gestione = @id_gestione " +
            "AND C.id_conto = @id_conto AND {0} ) AA LEFT JOIN titoli BB ON BB.id_titolo = AA.id_titolo left join aziende DD ON BB.id_azienda = DD.id_azienda " +
            "Left Join tipo_titoli EE ON BB.id_tipo_titolo = EE.id_tipo_titolo " +
            "ORDER BY data_movimento DESC";

        /// <summary>
        /// Data una gestione e una location, estrae tutti i record relativi ai movimenti di titoli
        /// </summary>
        public static readonly string GetManagerSharesMovementByOwnerAndLocation = "SELECT id_portafoglio, B.id_gestione, nome_gestione, C.id_conto, desc_conto, D.id_valuta, " +
            "cod_valuta, E.id_tipo_movimento, desc_Movimento, id_titolo, F.desc_titolo, F.isin, F.id_tipo_titolo, H.desc_tipo, F.id_azienda, I.desc_azienda, A.G.desc_borsa, data_movimento, " +
            "shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, profit_loss, if(disponibile = 1, 'true', 'false') AS disp, note " +
            "FROM portafoglio A, gestioni B, conti C, valuta D, tipo_movimento E, titoli F, tipo_titoli H, aziende I " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_conto = C.id_conto AND A.id_valuta = D.id_valuta AND A.id_movimento = E.id_tipo_movimento AND A.id_titolo = F.id_titolo " +
            "AND F.id_tipo_titolo = H.id_tipo_titolo AND F.id_azienda = I.id_azienda AND B.id_gestione = @id_gestione AND C.id_conto = @id_conto AND id_titolo IS NOT NULL " +
            "ORDER BY data_movimento DESC";

        /// <summary>
        /// Data una gestione e una location, estrae l'ultimo record caricato
        /// </summary>
        public static readonly string GetLastSharesMovementByOwnerAndLocation = "SELECT id_portafoglio, B.id_gestione, nome_gestione, C.id_conto, desc_conto, D.id_valuta, " +
            "cod_valuta, E.id_tipo_movimento, desc_Movimento, id_titolo, F.desc_titolo, F.isin, F.id_tipo_titolo, H.desc_tipo, F.id_azienda, I.desc_azienda, data_movimento, " +
            "shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, profit_loss, if(disponibile = 1, 'true', 'false') AS disp, note " +
            "FROM portafoglio A, gestioni B, conti C, valuta D, tipo_movimento E, titoli F, tipo_titoli H, aziende I " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_conto = C.id_conto AND A.id_valuta = D.id_valuta AND A.id_movimento = E.id_tipo_movimento AND A.id_titolo = F.id_titolo " +
            "AND F.id_tipo_titolo = H.id_tipo_titolo AND F.id_azienda = I.id_azienda AND B.id_gestione = @id_gestione AND C.id_conto = @id_conto AND id_titolo IS NOT NULL " +
            "ORDER BY id_portafoglio DESC LIMIT 1";

        /// <summary>
        /// Verifica la disponibilità di liquidità per la gestione,
        /// per il conto (location) e per la valuta selezionate
        /// </summary>
        public static readonly string GetCurrencyAvailable = "SELECT SUM(ammontare)  - ( SUM(total_commission) +SUM(tobin_tax) + SUM(disaggio_cedole) + SUM(ritenuta_fiscale) *-1 ) AS total, " +
            "disponibile FROM portafoglio WHERE id_gestione = @id_gestione AND id_conto = @id_conto " +
            "AND id_valuta = @id_valuta GROUP BY disponibile ORDER BY disponibile DESC";

        /// <summary>
        /// Estrae il numero di azioni possedute dato una gestione, un conto e un id azione
        /// </summary>
        public static readonly string GetSharesQuantity = "SELECT SUM(shares_quantity) TotalShares FROM portafoglio " +
            "WHERE id_gestione = @id_gestione AND id_conto = @id_conto AND id_titolo = @id_titolo";

        /// <summary>
        /// Estrae la somma del profit loss data una gestione e un dato conto
        /// </summary>
            public static readonly string GetProfitLossByCurrency = "SELECT SUM(profit_loss) as TotalProfitLoss FROM portafoglio " +
            "WHERE id_gestione = @id_gestione AND id_conto = @id_conto AND id_valuta =@id_valuta";

        /// <summary>
        /// Aggiunge un movimento
        /// </summary>
        public static readonly string AddManagerLiquidAsset = "INSERT INTO portafoglio (id_portafoglio, id_gestione, id_conto, id_valuta, id_tipo_movimento, " +
            "id_titolo, data_movimento, ammontare, shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, " +
            "valore_cambio, profit_loss, disponibile, note) " +
            "VALUE (null, @id_gestione, @id_conto, @id_valuta, @id_tipo_movimento, @id_titolo, @data_movimento, @ammontare, @shares_quantity, " +
            "@unity_local_value, @total_commission, @tobin_tax, @disaggio_cedole, @ritenuta_fiscale, @valore_cambio, @profit_loss, @disponibile, @note);";

        /// <summary>
        /// Aggiorna un movimento
        /// </summary>
        public static readonly string UpdateManagerLiquidAsset = "UPDATE portafoglio SET id_gestione = @id_gestione, id_conto = @id_conto, id_valuta = @id_valuta, " +
            "id_tipo_movimento = @id_tipo_movimento, id_titolo = @id_titolo, data_movimento = @data_movimento, ammontare = @ammontare, shares_quantity = @shares_quantity, " +
            "unity_local_value = @unity_local_value, total_commission = @total_commission, tobin_tax = @tobin_tax, disaggio_cedole = @disaggio_cedole, " +
            "ritenuta_fiscale = @ritenuta_fiscale, valore_cambio = @valore_cambio, profit_loss = @profit_loss, " +
            "disponibile = @disponibile, note = @note WHERE id_portafoglio = @id_portafoglio";
        
        /// <summary>
        /// Cancella un movimento
        /// </summary>
        public static readonly string DeleteManagerLiquidAsset = "DELETE FROM portafoglio WHERE id_portafoglio = @id_portafoglio;";

        public static readonly string GetShareMovements = "SELECT id_portafoglio, B.id_gestione, nome_gestione, C.id_conto, desc_conto, D.id_valuta, " +
            "cod_valuta, E.id_tipoMovimento, desc_Movimento, id_titolo, F.desc_titolo, F.isin, F.id_tipo_titolo, H.desc_tipo, F.id_azienda, I.desc_azienda, data_movimento, " +
            "shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, profit_loss, if(disponibile = 1, 'true', 'false') AS disp, note " +
            "FROM portafoglio A, gestioni B, conti C, valuta D, tipo_movimento E, titoli F, tipo_titoli H, aziende I " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_conto = C.id_conto AND A.id_valuta = D.id_valuta AND A.id_tipo_movimento = E.id_tipo_movimento AND A.id_titolo = F.id_titolo " +
            "AND F.id_tipo_titoli = H.id_tipo_titoli AND F.id_azienda = I.id_azienda AND B.id_portafoglio = @owner AND C.id_conto = @id_conto AND id_titolo = @id_titolo " +
            "AND (id_movimento = 5 or id_movimento = 6) " +
            "ORDER BY data_movimento";
    }
}

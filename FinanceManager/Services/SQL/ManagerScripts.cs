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
        public static readonly string GetManagerLiquidAssetListByOwnerAndLocation = "SELECT id_liquid_movement, id_portafoglio, desc_portafoglio, id_location, desc_location, AA.id_valuta, " +
            "cod_valuta, id_tipoMovimento, desc_Movimento, BB.id_tipo, EE.desc_tipo, BB.id_azienda, desc_azienda, id_titolo, BB.desc_titolo, BB.Isin, AA.id_borsa, CC.desc_borsa, data_movimento, " +
            "ammontare, shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, valore_cambio, profit_loss, disp, note FROM ( " +
            "SELECT id_liquid_movement, id_portafoglio, desc_portafoglio, C.id_location, desc_location, D.id_valuta, cod_valuta, E.id_tipoMovimento, desc_Movimento, id_titolo, id_borsa, " +
            "data_movimento, shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, profit_loss, if(disponibile = 1, 'true', 'false') AS disp, note " +
            "FROM daf_portafoglio A, daf_portfolio_owner B, daf_location C, daf_valuta D, daf_tipo_movimento E " +
            "WHERE A.id_gestione = B.id_portafoglio AND A.id_location = C.id_location AND A.id_valuta = D.id_valuta AND A.id_movimento = E.id_tipoMovimento AND B.id_portafoglio = @owner " +
            "AND A.id_location = @location) AA LEFT JOIN daf_titoli BB ON BB.id_tit = AA.id_titolo left join daf_borsa CC ON CC.id_borsa = AA.id_borsa left join daf_aziende DD ON BB.id_azienda = DD.id_azienda " +
            "Left Join daf_tipo_titoli EE ON BB.id_tipo = EE.id_tipo " +
            "ORDER BY data_movimento DESC";

        /// <summary>
        /// Data una gestione e una location, estrae tutti i record relativi ai tipi di movimenti richiesti
        /// </summary>
        public static readonly string GetManagerLiquidAssetByOwnerLocationAndMovementType = "SELECT id_liquid_movement, id_portafoglio, desc_portafoglio, id_location, desc_location, AA.id_valuta, " +
            "cod_valuta, id_tipoMovimento, desc_Movimento, BB.id_tipo, EE.desc_tipo, BB.id_azienda, desc_azienda, id_titolo, BB.desc_titolo, BB.Isin, AA.id_borsa, CC.desc_borsa, data_movimento, " +
            "ammontare, shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, valore_cambio, profit_loss, disp, note FROM ( " +
            "SELECT id_liquid_movement, id_portafoglio, desc_portafoglio, C.id_location, desc_location, D.id_valuta, cod_valuta, E.id_tipoMovimento, desc_Movimento, id_titolo, id_borsa, " +
            "data_movimento, shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, profit_loss, if(disponibile = 1, 'true', 'false') AS disp, note " +
            "FROM daf_portafoglio A, daf_portfolio_owner B, daf_location C, daf_valuta D, daf_tipo_movimento E " +
            "WHERE A.id_gestione = B.id_portafoglio AND A.id_location = C.id_location AND A.id_valuta = D.id_valuta AND A.id_movimento = E.id_tipoMovimento AND B.id_portafoglio = @owner " +
            "AND C.id_location = @id_location AND {0} ) AA LEFT JOIN daf_titoli BB ON BB.id_tit = AA.id_titolo left join daf_borsa CC ON CC.id_borsa = AA.id_borsa left join daf_aziende DD ON BB.id_azienda = DD.id_azienda " +
            "Left Join daf_tipo_titoli EE ON BB.id_tipo = EE.id_tipo " +
            "ORDER BY data_movimento DESC";

        /// <summary>
        /// Data una gestione e una location, estrae tutti i record relativi ai movimenti di titoli
        /// </summary>
        public static readonly string GetManagerSharesMovementByOwnerAndLocation = "SELECT id_liquid_movement, id_portafoglio, desc_portafoglio, C.id_location, desc_location, D.id_valuta, " +
            "cod_valuta, E.id_tipoMovimento, desc_Movimento, id_titolo, F.desc_titolo, F.isin, F.id_tipo, H.desc_tipo, F.id_azienda, I.desc_azienda, A.id_borsa, G.desc_borsa, data_movimento, " +
            "shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, profit_loss, if(disponibile = 1, 'true', 'false') AS disp, note " +
            "FROM daf_portafoglio A, daf_portfolio_owner B, daf_location C, daf_valuta D, daf_tipo_movimento E, daf_titoli F, daf_borsa G, daf_tipo_titoli H, daf_aziende I " +
            "WHERE A.id_gestione = B.id_portafoglio AND A.id_location = C.id_location AND A.id_valuta = D.id_valuta AND A.id_movimento = E.id_tipoMovimento AND A.id_titolo = F.id_tit " +
            "AND A.id_borsa = G.id_borsa AND F.id_tipo = H.id_tipo AND F.id_azienda = I.id_azienda AND B.id_portafoglio = @owner AND C.id_location = @id_location AND id_titolo IS NOT NULL " +
            "ORDER BY data_movimento DESC";

        /// <summary>
        /// Data una gestione e una location, estrae l'ultimo record caricato
        /// </summary>
        public static readonly string GetLastSharesMovementByOwnerAndLocation = "SELECT id_liquid_movement, id_portafoglio, desc_portafoglio, C.id_location, desc_location, D.id_valuta, " +
            "cod_valuta, E.id_tipoMovimento, desc_Movimento, id_titolo, F.desc_titolo, F.isin, F.id_tipo, H.desc_tipo, F.id_azienda, I.desc_azienda, A.id_borsa, G.desc_borsa, data_movimento, " +
            "shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, profit_loss, if(disponibile = 1, 'true', 'false') AS disp, note " +
            "FROM daf_portafoglio A, daf_portfolio_owner B, daf_location C, daf_valuta D, daf_tipo_movimento E, daf_titoli F, daf_borsa G, daf_tipo_titoli H, daf_aziende I " +
            "WHERE A.id_gestione = B.id_portafoglio AND A.id_location = C.id_location AND A.id_valuta = D.id_valuta AND A.id_movimento = E.id_tipoMovimento AND A.id_titolo = F.id_tit " +
            "AND A.id_borsa = G.id_borsa AND F.id_tipo = H.id_tipo AND F.id_azienda = I.id_azienda AND B.id_portafoglio = @owner AND C.id_location = @id_location AND id_titolo IS NOT NULL " +
            "ORDER BY id_liquid_movement DESC LIMIT 1";

        /// <summary>
        /// Verifica la disponibilità di liquidità per la gestione,
        /// per il conto (location) e per la valuta selezionate
        /// </summary>
        public static readonly string GetCurrencyAvailable = "SELECT SUM(ammontare)  - ( SUM(total_commission) +SUM(tobin_tax) + SUM(disaggio_cedole) + SUM(ritenuta_fiscale) ) AS total, " +
            "disponibile FROM daf_portafoglio WHERE id_gestione = @owner AND id_location = @location " +
            "AND id_valuta = @currency GROUP BY disponibile ORDER BY disponibile DESC";

        /// <summary>
        /// Estrae il numero di azioni possedute dato una gestione, un conto e un id azione
        /// </summary>
        public static readonly string GetSharesQuantity = "SELECT SUM(shares_quantity) TotalShares FROM daf_portafoglio WHERE id_gestione = @id_gestione AND id_location = @id_location AND id_titolo = @id_titolo";

        public static readonly string GetProfitLossByCurrency = "SELECT SUM(profit_loss) as TotalProfitLoss FROM daf_portafoglio WHERE id_gestione = @id_gestione AND id_location = @id_location AND id_valuta =@id_valuta";

        /// <summary>
        /// Aggiunge un movimento
        /// </summary>
        public static readonly string AddManagerLiquidAsset = "INSERT INTO daf_portafoglio (id_liquid_movement, id_gestione, id_location, id_valuta, id_movimento, " +
            "id_titolo, id_borsa, data_movimento, ammontare, shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, " +
            "valore_cambio, profit_loss, disponibile, note) " +
            "VALUE (null, @id_portafoglio, @id_location, @id_valuta, @id_tipoMovimento, @id_titolo, @id_borsa, @data_movimento, @ammontare, @shares_quantity, " +
            "@unity_local_value, @total_commission, @tobin_tax, @disaggio_cedole, @ritenuta_fiscale, @valore_cambio, @profit_loss, @disponibile, @note);";

        /// <summary>
        /// Aggiorna un movimento
        /// </summary>
        public static readonly string UpdateManagerLiquidAsset = "UPDATE daf_portafoglio SET id_gestione = @id_portafoglio, id_location = @id_location, id_valuta = @id_valuta, " +
            "id_movimento = @id_tipoMovimento, id_titolo = @id_titolo, id_borsa = @id_borsa, data_movimento = @data_movimento, ammontare = @ammontare, shares_quantity = @shares_quantity, " +
            "unity_local_value = @unity_local_value, total_commission = @total_commission, tobin_tax = @tobin_tax, disaggio_cedole = @disaggio_cedole, " +
            "ritenuta_fiscale = @ritenuta_fiscale, valore_cambio = @valore_cambio, profit_loss = @profit_loss, " +
            "disponibile = @disponibile, note = @note WHERE id_liquid_movement = @id_liquid_movement";
        
        /// <summary>
        /// Cancella un movimento
        /// </summary>
        public static readonly string DeleteManagerLiquidAsset = "DELETE FROM daf_portafoglio WHERE id_liquid_movement = @id_liquid_movement;";

        public static readonly string GetShareMovements = "SELECT id_liquid_movement, id_portafoglio, desc_portafoglio, C.id_location, desc_location, D.id_valuta, " +
            "cod_valuta, E.id_tipoMovimento, desc_Movimento, id_titolo, F.desc_titolo, F.isin, F.id_tipo, H.desc_tipo, F.id_azienda, I.desc_azienda, A.id_borsa, G.desc_borsa, data_movimento, " +
            "shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, profit_loss, if(disponibile = 1, 'true', 'false') AS disp, note " +
            "FROM daf_portafoglio A, daf_portfolio_owner B, daf_location C, daf_valuta D, daf_tipo_movimento E, daf_titoli F, daf_borsa G, daf_tipo_titoli H, daf_aziende I " +
            "WHERE A.id_gestione = B.id_portafoglio AND A.id_location = C.id_location AND A.id_valuta = D.id_valuta AND A.id_movimento = E.id_tipoMovimento AND A.id_titolo = F.id_tit " +
            "AND A.id_borsa = G.id_borsa AND F.id_tipo = H.id_tipo AND F.id_azienda = I.id_azienda AND B.id_portafoglio = @owner AND C.id_location = @id_location AND id_titolo = @id_titolo " +
            "AND (id_movimento = 5 or id_movimento = 6) " +
            "ORDER BY data_movimento";
    }
}

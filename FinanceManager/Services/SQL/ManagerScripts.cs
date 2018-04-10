using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services.SQL
{
    public class ManagerScripts
    {
        public static readonly string GetManagerLiquidAssetList = "SELECT id_liquid_movement, id_portafoglio, desc_portafoglio, id_location, desc_location, AA.id_valuta, cod_valuta, " +
            "id_tipoMovimento, desc_Movimento, id_titolo, BB.Isin, data_movimento, ammontare, valore_cambio, disp, note FROM (" +
            "SELECT id_liquid_movement, id_portafoglio, desc_portafoglio, C.id_location, desc_location, D.id_valuta, " +
            "cod_valuta, E.id_tipoMovimento, desc_Movimento, id_titolo, data_movimento, ammontare, valore_cambio, " +
            "if(disponibile = 1, 'true', 'false') AS disp, note " +
            "FROM daf_portafoglio A, daf_portfolio_owner B, daf_location C, daf_valuta D, daf_tipo_movimento E " +
            "WHERE A.id_gestione = B.id_portafoglio AND A.id_location = C.id_location AND A.id_valuta = D.id_valuta AND A.id_movimento = E.id_tipoMovimento AND B.id_portafoglio = @owner " +
            ") AA LEFT JOIN daf_titoli BB ON BB.id_tit = AA.id_titolo ORDER BY data_movimento DESC ";

        public static readonly string GetManagerLiquidAssetById = "SELECT id_liquid_movement, id_portafoglio, desc_portafoglio, id_location, desc_location, AA.id_valuta, cod_valuta, " +
            "id_tipoMovimento, desc_Movimento, id_titolo, BB.Isin, data_movimento, ammontare, valore_cambio, disp, note FROM (" +
            "SELECT id_liquid_movement, id_portafoglio, desc_portafoglio, C.id_location, desc_location, D.id_valuta, " +
            "cod_valuta, E.id_tipoMovimento, desc_Movimento, id_titolo, data_movimento, ammontare, valore_cambio, " +
            "if(disponibile = 1, 'true', 'false') AS disp, note " +
            "FROM daf_portafoglio A, daf_portfolio_owner B, daf_location C, daf_valuta D, daf_tipo_movimento E " +
            "WHERE A.id_gestione = B.id_portafoglio AND A.id_location = C.id_location AND A.id_valuta = D.id_valuta AND A.id_movimento = E.id_tipoMovimento " +
            "AND B.id_portafoglio = @owner AND id_liquid_movement = @id_liquid_movement " +
            ") AA LEFT JOIN daf_titoli BB ON BB.id_tit = AA.id_titolo ORDER BY data_movimento DESC ";

        public static readonly string AddManagerLiquidAsset = "INSERT INTO daf_portafoglio (id_liquid_movement, id_gestione, id_location, id_valuta, id_movimento, " +
            "id_titolo, data_movimento, ammontare, valore_cambio, disponibile, note) VALUE (null, @id_portafoglio, @id_location, @id_valuta, @id_tipoMovimento, @id_titolo, " +
            "@data_movimento, @ammontare, @valore_cambio, @disponibile, @note);";

        public static readonly string UpdateManagerLiquidAsset = "UPDATE daf_portafoglio SET id_gestione = @id_portafoglio, id_location = @id_location, id_valuta = @id_valuta, " +
            "id_movimento = @id_tipoMovimento, id_titolo = @it_titolo, data_movimento = @data_movimento, ammontare = @ammontare, valore_cambio = @valore_cambio, " +
            "disponibile = @disponibile, note = @note WHERE id_liquid_movement = @id_liquid_movement";

        public static readonly string DeleteManagerLiquidAsset = "DELETE FROM daf_portafoglio WHERE id_liquid_movement = @id_liquid_movement;";
    }
}

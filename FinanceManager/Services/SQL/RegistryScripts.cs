using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services.SQL
{
    public class RegistryScripts
    {
        #region Gestione
        public readonly static string GetGestioneList = "SELECT id_gestione, nome_gestione FROM gestioni WHERE id_gestione > 0 ORDER BY id_gestione";
        public readonly static string UpdateGestioneName = "UPDATE gestioni SET nome_gestione = @nome WHERE id_gestione = @id";
        public readonly static string AddGestione = "INSERT INTO gestioni (id_gestione, nome_gestione) VALUE (null, @nome)";
        public readonly static string DeleteGestione = "DELETE FROM gestioni WHERE id_gestione = @id";
        public readonly static string GetGestione = "SELECT id_gestione, nome_gestione FROM gestioni WHERE id_gestione = @id_gestione";
        #endregion

        #region TipoTitoli
        public readonly static string GetRegistryShareTypeList = "SELECT id_tipo_titolo, desc_tipo_titolo FROM tipo_titoli WHERE id_tipo_titolo > 0 ORDER BY id_tipo_titolo;";
        public readonly static string UpdateShareType = "UPDATE tipo_titoli SET desc_tipo_titolo = @desc WHERE id_tipo_titolo = @id;";
        public readonly static string DeleteShareType = "DELETE FROM tipo_titoli WHERE id_tipo_titolo = @id";
        public readonly static string AddShareType = "INSERT INTO tipo_titoli (id_tipo_titolo, desc_tipo_titolo) VALUE (null, @desc);";
        #endregion
        
        #region Valuta
        public readonly static string GetRegistryCurrencyList = "SELECT id_valuta, desc_valuta, cod_valuta FROM valuta ORDER BY id_valuta;";
        public readonly static string UpdateCurrency = "UPDATE valuta SET desc_valuta = @desc, cod_valuta = @code WHERE id_valuta = @id;";
        public readonly static string DeleteCurrency = "DELETE FROM valuta WHERE id_valuta = @id";
        public readonly static string AddCurrency = "INSERT INTO valuta (id_valuta, desc_valuta, cod_valuta) VALUE (null, @desc, @code);";
        #endregion
        
        #region conti
        public readonly static string GetRegistryLocationList = "SELECT id_conto, desc_conto FROM conti WHERE id_conto > 0 ORDER BY id_conto;";
        public readonly static string UpdateLocation = "UPDATE conti SET desc_conto = @desc WHERE id_conto = @id;";
        public readonly static string DeleteLocation = "DELETE FROM conti WHERE id_conto = @id";
        public readonly static string AddLocation = "INSERT INTO conti (id_conto, desc_conto) VALUE (null, @desc);";
        public readonly static string GetLocation = "SELECT id_conto, desc_conto FROM conti WHERE id_conto = @id";
        #endregion

        #region Azienda
        public readonly static string GetRegistryFirmList = "SELECT id_azienda, desc_azienda FROM aziende WHERE id_azienda > 0 ORDER BY desc_azienda;";
        public readonly static string UpdateFirm = "UPDATE aziende SET desc_azienda = @desc WHERE id_azienda = @id;";
        public readonly static string DeleteFirm = "DELETE FROM aziende WHERE id_azienda = @id";
        public readonly static string AddFirm = "INSERT INTO aziende (id_azienda, desc_azienda) VALUE (null, @desc);";
        #endregion
        
        #region titoli
        public readonly static string GetRegistryShareList = "SELECT id_titolo, desc_titolo, isin, id_tipo_titolo, id_azienda " +
            "FROM titoli WHERE id_titolo > 0 ORDER BY isin, desc_titolo;";
        public readonly static string UpdateShare = "UPDATE titoli SET desc_titolo = @desc, isin = @isin, id_tipo_titolo = @tipo, id_azienda = @azienda " +
            "WHERE id_titolo = @id;";
        public readonly static string DeleteShare = "DELETE FROM titoli WHERE id_titolo = @id";
        public readonly static string AddShare = "INSERT INTO titoli (id_titolo, desc_titolo, isin, id_tipo_titolo, id_azienda) " +
            "VALUE (null, @desc, @isin, @tipo, @azienda);";

        public readonly static string GetSharesByType = "SELECT id_titolo, desc_titolo, isin, id_tipo_titolo, id_azienda FROM titoli WHERE {0} ORDER BY id_titolo";
        #endregion

        #region tipo movimento
        public readonly static string GetRegistryMovementTypeList = "SELECT id_tipo_movimento, desc_Movimento FROM tipo_movimento WHERE id_tipo_movimento > 0 ORDER BY id_tipo_movimento;";
        public readonly static string UpdateMovementType = "UPDATE tipo_movimento SET desc_Movimento = @desc WHERE id_tipo_movimento = @id;";
        public readonly static string DeleteMovementType = "DELETE FROM tipo_movimento WHERE id_tipo_movimento = @id;";
        public readonly static string AddMovementType = "INSERT INTO tipo_movimento (id_tipo_movimento, desc_Movimento) VALUE (null, @desc);";
        public readonly static string GetMovementType = "SELECT id_tipo_movimento, desc_Movimento FROM tipo_movimento WHERE id_tipo_movimento = @id";
        #endregion

        public static readonly string GetTipoSoldiList = "SELECT id_tipo_soldi, desc_tipo_soldi FROM tipo_soldi WHERE id_tipo_soldi > 0";

        public static readonly string GetTipoSoldiById = "SELECT id_tipo_soldi, desc_tipo_soldi FROM tipo_soldi WHERE id_tipo_soldi = @id_tipo_soldi";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services.SQL
{
    public class RegistryScripts
    {
        #region Owner
        public readonly static string GetAccountList = "SELECT id_portafoglio, desc_portafoglio FROM daf_portfolio_owner ORDER BY id_portafoglio;";
        public readonly static string UpdateName = "UPDATE daf_portfolio_owner SET desc_portafoglio = @name WHERE id_portafoglio = @id";
        public readonly static string AddOwner = "INSERT INTO daf_portfolio_owner (id_portafoglio, desc_portafoglio) VALUE (null, @name)";
        public readonly static string DeleteOwner = "DELETE FROM daf_portfolio_owner WHERE id_portafoglio = @id";
        #endregion
        #region TipoTitoli
        public readonly static string GetRegistryShareTypeList = "SELECT id_tipo, desc_tipo FROM daf_tipo_titoli ORDER BY id_tipo;";
        public readonly static string UpdateShareType = "UPDATE daf_tipo_titoli SET desc_tipo = @desc WHERE id_tipo = @id;";
        public readonly static string DeleteShareType = "DELETE daf_tipo_titoli WHERE id_tipo = @id";
        public readonly static string AddShareType = "INSERT INTO daf_tipo_titoli (id_tipo, desc_tipo) VALUE (null, @desc);";
        #endregion
        #region Valuta
        public readonly static string GetRegistryCurrencyList = "SELECT id_valuta, desc_valuta, cod_valuta FROM daf_valuta ORDER BY id_valuta;";
        public readonly static string UpdateCurrency = "UPDATE daf_valuta SET desc_valuta = @desc, cod_valuta = @cod WHERE id_valuta = @id;";
        public readonly static string DeleteCurrency = "DELETE daf_valuta WHERE id_valuta = @id";
        public readonly static string AddCurrency = "INSERT INTO daf_valuta (id_valuta, desc_valuta, cod_valuta) VALUE (null, @desc, @cod);";
        #endregion

    }
}

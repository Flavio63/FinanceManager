﻿using System;
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
        #region Location
        public readonly static string GetRegistryLocationList = "SELECT id_location, desc_location FROM daf_location ORDER BY id_location;";
        public readonly static string UpdateLocation = "UPDATE daf_location SET desc_location = @desc WHERE id_location = @id;";
        public readonly static string DeleteLocation = "DELETE daf_location WHERE id_location = @id";
        public readonly static string AddLocation = "INSERT INTO daf_location (id_location, desc_location) VALUE (null, @desc);";
        #endregion
        #region Market
        public readonly static string GetRegistryMarketList = "SELECT id_borsa, desc_borsa FROM daf_borsa ORDER BY id_borsa;";
        public readonly static string UpdateMarket = "UPDATE daf_borsa SET desc_borsa = @desc WHERE id_borsa = @id;";
        public readonly static string DeleteMarket = "DELETE daf_borsa WHERE id_borsa = @id";
        public readonly static string AddMarket = "INSERT INTO daf_borsa (id_borsa, desc_borsa) VALUE (null, @desc);";
        #endregion
        #region Firm
        public readonly static string GetRegistryFirmList = "SELECT id_azienda, desc_azienda FROM daf_aziende ORDER BY id_azienda;";
        public readonly static string UpdateFirm = "UPDATE daf_aziende SET desc_azienda = @desc WHERE id_azienda = @id;";
        public readonly static string DeleteFirm = "DELETE daf_aziende WHERE id_azienda = @id";
        public readonly static string AddFirm = "INSERT INTO daf_aziende (id_azienda, desc_azienda) VALUE (null, @desc);";
        #endregion
        #region Share
        public readonly static string GetRegistryShareList = "SELECT id_tit, desc_titolo, isin, id_tipo, id_azienda, id_borsa, id_valuta " +
            "FROM daf_titoli ORDER BY id_tit;";
        public readonly static string UpdateShare = "UPDATE daf_titoli SET desc_titolo = @desc, isin = @isin, id_tipo = @tipo, id_azienda = @azienda, " +
            "id_borsa = @borsa, id_valuta = @valuta WHERE id_tit = @id;";
        public readonly static string DeleteShare = "DELETE daf_titoli WHERE id_tit = @id";
        public readonly static string AddShare = "INSERT INTO daf_titoli (id_tit, desc_titolo, isin, id_tipo, id_azienda, id_borsa, id_valuta) " +
            "VALUE (null, @desc, @isin, @tipo, @azienda, @borsa, @valuta);";
        #endregion

    }
}

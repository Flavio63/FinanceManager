using System;
using System.Data;
using FinanceManager.Models;
using FinanceManager.Models.Enum;
using FinanceManager.Services.SQL;
using MySql.Data.MySqlClient;

namespace FinanceManager.Services
{
    class ManagerLiquidAssetServices : IManagerLiquidAssetServices
    {
        IDAFconnection DAFconnection;

        public ManagerLiquidAssetServices(IDAFconnection iDAFconnection)
        {
            DAFconnection = iDAFconnection ?? throw new ArgumentNullException("Manca la stringa di connessione al db");
        }

        /// <summary>
        /// Aggiunge un movimento
        /// </summary>
        /// <param name="managerLiquidAsset">Il movimento da aggiungere</param>
        public void AddManagerLiquidAsset(PortafoglioTitoli managerLiquidAsset)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.ManagerScripts.AddManagerLiquidAsset;
                    dbComm.Parameters.AddWithValue("id_gestione", managerLiquidAsset.Id_gestione);
                    dbComm.Parameters.AddWithValue("id_conto", managerLiquidAsset.Id_conto);
                    dbComm.Parameters.AddWithValue("id_valuta", managerLiquidAsset.Id_valuta);
                    dbComm.Parameters.AddWithValue("id_tipo_movimento", managerLiquidAsset.Id_tipo_movimento);
                    dbComm.Parameters.AddWithValue("id_titolo", managerLiquidAsset.Id_titolo);
                    dbComm.Parameters.AddWithValue("data_movimento", managerLiquidAsset.Data_Movimento.ToString("yyyy-MM-dd"));
                    dbComm.Parameters.AddWithValue("ammontare", managerLiquidAsset.Importo_totale);
                    dbComm.Parameters.AddWithValue("shares_quantity", managerLiquidAsset.N_titoli);
                    dbComm.Parameters.AddWithValue("unity_local_value", managerLiquidAsset.Costo_unitario_in_valuta);
                    dbComm.Parameters.AddWithValue("total_commission", managerLiquidAsset.Commissioni_totale);
                    dbComm.Parameters.AddWithValue("tobin_tax", managerLiquidAsset.TobinTax);
                    dbComm.Parameters.AddWithValue("disaggio_cedole", managerLiquidAsset.Disaggio_anticipo_cedole);
                    dbComm.Parameters.AddWithValue("ritenuta_fiscale", managerLiquidAsset.RitenutaFiscale);
                    dbComm.Parameters.AddWithValue("valore_cambio", managerLiquidAsset.Valore_di_cambio);
                    dbComm.Parameters.AddWithValue("note", managerLiquidAsset.Note);
                    dbComm.Parameters.AddWithValue("attivo", managerLiquidAsset.Attivo);
                    dbComm.Parameters.AddWithValue("link_movimenti", managerLiquidAsset.Link_Movimenti.ToString("yyyy-MM-dd HH:mm:ss"));
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbComm.Connection.Open();
                    dbComm.ExecuteNonQuery();
                    dbComm.Connection.Close();
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Elimina un movimento
        /// </summary>
        /// <param name="id">Identificativo del movimento da eliminare</param>
        public void DeleteManagerLiquidAsset(int id)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.ManagerScripts.DeleteManagerLiquidAsset;
                    dbComm.Parameters.AddWithValue("id_portafoglio_titoli", id);
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbComm.Connection.Open();
                    dbComm.ExecuteNonQuery();
                    dbComm.Connection.Close();
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Estrae tutti i valori di cedole, utili e disponibilità 
        /// sulla base della gestione richiesta
        /// </summary>
        /// <param name="IdGestione">La gestione</param>
        /// <returns>Observable collection </returns>
        public SintesiSoldiList GetCurrencyAvailable(int IdGestione, int IdConto, int IdValuta)
        {
            try
            {
                DataTable DT = new DataTable();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    if (IdGestione > 0 && IdConto > 0 && IdValuta > 0)
                        dbAdapter.SelectCommand.CommandText = ManagerScripts.GetCurrencyAvByOwnerContoValuta;
                    else if (IdGestione > 0 && IdConto == 0 && IdValuta == 0)
                        dbAdapter.SelectCommand.CommandText = ManagerScripts.GetCurrencyAvByOwner;
                    else
                        throw new Exception("Richiesta di liquidità non ancora disponibile.");
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", IdGestione);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_conto", IdConto);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_valuta", IdValuta);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
                    SintesiSoldiList sintesiSoldis = new SintesiSoldiList();
                    foreach (DataRow dataRow in DT.Rows)
                    {
                        SintesiSoldi sintesiSoldi = new SintesiSoldi();
                        sintesiSoldi.DescCont = dataRow.Field<string>("desc_conto");
                        sintesiSoldi.CodValuta = dataRow.Field<string>("cod_valuta");
                        sintesiSoldi.Cedole = dataRow.IsNull("Cedole") ? 0 : (double)dataRow.ItemArray[2];
                        sintesiSoldi.Utili = dataRow.IsNull("Utili") ? 0 : (double)dataRow.ItemArray[3];
                        sintesiSoldi.Disponibili = dataRow.IsNull("Disponibili") ? 0 : (double)dataRow.ItemArray[4];
                        sintesiSoldis.Add(sintesiSoldi);
                    }
                    return sintesiSoldis;
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Data una gestione ne estrae tutti i movimenti
        /// </summary>
        /// <param name="idOwner">La gestione</param>
        /// <returns>Lista dei movimenti</returns>
        public PortafoglioTitoliList GetManagerLiquidAssetListByOwnerAndLocation(int idOwner, int idLocation)
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                    if (idOwner == 0 && idLocation == 0)
                        dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetManagerLiquidAssetListTotal;
                    else if (idOwner > 0 && idLocation == 0)
                        dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetManagerLiquidAssetListByOwner;
                    else if (idOwner == 0 && idLocation > 0)
                        dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetManagerLiquidAssetListByLocation;
                    else
                        dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetManagerLiquidAssetListByOwnerAndLocation;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", idOwner);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_conto", idLocation);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    DataTable dt = new DataTable();
                    dbAdapter.Fill(dt);
                    return MLAL(dt);
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
        /// <summary>
        /// Estrae tutti i record di una gestione in un conto di un titolo
        /// </summary>
        /// <param name="idGestione">la gestione scelta</param>
        /// <param name="idConti">il conto corrente</param>
        /// <param name="idTitolo">il titolo</param>
        /// <returns></returns>
        public PortafoglioTitoliList GetManagerLiquidAssetListByOwnerLocationAndTitolo(int idGestione, int idConto, int idTitolo)
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = ManagerScripts.GetManagerLiquidAssetListByOwnerLocatioAndShare;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", idGestione);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_conto", idConto);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_titolo", idTitolo);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    DataTable dt = new DataTable();
                    dbAdapter.Fill(dt);
                    return MLAL(dt);
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Estrae tutti i record di una gestione in un conto di un titolo
        /// </summary>
        /// <param name="idGestione">la gestione scelta</param>
        /// <param name="idConti">il conto corrente</param>
        /// <param name="idTitolo">il titolo</param>
        /// <returns></returns>
        public PortafoglioTitoliList GetManagerLiquidAssetListByLinkMovimenti(DateTime link_movimenti)
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = ManagerScripts.GetManagerLiquidAssetListByLinkMovimenti;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("link_movimenti", link_movimenti);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    DataTable dt = new DataTable();
                    dbAdapter.Fill(dt);
                    return MLAL(dt);
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Data una gestione, un conto e dei tipi di movimenti
        /// ne estrae tutti i record
        /// </summary>
        /// <param name="IdOwner">La gestione</param>
        /// <param name="IdLocation">Il conto</param>
        /// <param name="IdsMovement">I tipi di movimenti</param>
        /// <returns>Una lista dei movimenti</returns>
        public PortafoglioTitoliList GetManagerLiquidAssetListByOwnerLocationAndMovementType(int IdOwner, int IdLocation, int[] IdsMovement)
        {
            try
            {
                if (IdsMovement.Length == 0)
                    throw new Exception("Errore nella richiesta dei movimenti");
                string query = "(";
                string IdRequest = "E.id_tipo_movimento = ";

                foreach (int I in IdsMovement)
                {
                    query += IdRequest + I + " OR ";
                }
                query = query.Substring(0, query.Length - 4) + ")";
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = string.Format(SQL.ManagerScripts.GetManagerLiquidAssetByOwnerLocationAndMovementType, query);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", IdOwner);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_conto", IdLocation);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    DataTable dt = new DataTable();
                    dbAdapter.Fill(dt);
                    return MLAL(dt);
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public PortafoglioTitoli GetLastShareMovementByOwnerAndLocation(int IdOwner, int IdLocation)
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetLastSharesMovementByOwnerAndLocation;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", IdOwner);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_conto", IdLocation);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    DataTable dt = new DataTable();
                    dbAdapter.Fill(dt);
                    return MLA(dt.Rows[0]);
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Data una gestione e il conto estrae tutti i record relativi alla
        /// compravendita di titoli
        /// </summary>
        /// <param name="IdOwner">La gestione</param>
        /// <param name="IdLocation">Il conto</param>
        /// <returns>Lista di movimenti</returns>
        public PortafoglioTitoliList GetManagerSharesMovementByOwnerAndLocation(int IdOwner, int IdLocation)
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetManagerSharesMovementByOwnerAndLocation;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", IdOwner);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_conto", IdLocation);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    DataTable dt = new DataTable();
                    dbAdapter.Fill(dt);
                    return MLAL(dt);
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }

        }

        public double GetProfitLossByCurrency(int IdOwner, int IdLocation, int IdCurrency)
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetProfitLossByCurrency;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", IdOwner);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_conto", IdLocation);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_valuta", IdCurrency);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    DataTable dataTable = new DataTable();
                    dbAdapter.Fill(dataTable);
                    return ((DataRow)dataTable.Rows[0]).Field<double?>("TotalProfitLoss") == null ? 0 : (double)((DataRow)dataTable.Rows[0]).Field<double?>("TotalProfitLoss");
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Data una gestione, un conto e il codice di un titolo
        /// restituisce quanti titoli si hanno in portafoglio
        /// </summary>
        /// <param name="IdOwner">La gestione</param>
        /// <param name="IdLocation">Il conto</param>
        /// <param name="id_titolo">Il titolo</param>
        /// <returns>ritorna il numero di titoli</returns>
        public double GetSharesQuantity(int IdOwner, int IdLocation, uint id_titolo)
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetSharesQuantity;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", IdOwner);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_conto", IdLocation);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_titolo", id_titolo);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    DataTable dataTable = new DataTable();
                    dbAdapter.Fill(dataTable);
                    return ((DataRow)dataTable.Rows[0]).Field<double?>("TotalShares") == null ? 0 : (double)((DataRow)dataTable.Rows[0]).Field<double?>("TotalShares");
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Aggiorna i campi di un movimento
        /// </summary>
        /// <param name="managerLiquidAsset">Il record da modificare</param>
        public void UpdateManagerLiquidAsset(PortafoglioTitoli managerLiquidAsset)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.ManagerScripts.UpdateManagerLiquidAsset;
                    dbComm.Parameters.AddWithValue("id_gestione", managerLiquidAsset.Id_gestione);
                    dbComm.Parameters.AddWithValue("id_conto", managerLiquidAsset.Id_conto);
                    dbComm.Parameters.AddWithValue("id_valuta", managerLiquidAsset.Id_valuta);
                    dbComm.Parameters.AddWithValue("id_tipo_movimento", managerLiquidAsset.Id_tipo_movimento);
                    dbComm.Parameters.AddWithValue("id_titolo", managerLiquidAsset.Id_titolo);
                    dbComm.Parameters.AddWithValue("data_movimento", managerLiquidAsset.Data_Movimento.ToString("yyyy-MM-dd"));
                    dbComm.Parameters.AddWithValue("ammontare", managerLiquidAsset.Importo_totale);
                    dbComm.Parameters.AddWithValue("shares_quantity", managerLiquidAsset.N_titoli);
                    dbComm.Parameters.AddWithValue("unity_local_value", managerLiquidAsset.Costo_unitario_in_valuta);
                    dbComm.Parameters.AddWithValue("total_commission", managerLiquidAsset.Commissioni_totale);
                    dbComm.Parameters.AddWithValue("tobin_tax", managerLiquidAsset.TobinTax);
                    dbComm.Parameters.AddWithValue("disaggio_cedole", managerLiquidAsset.Disaggio_anticipo_cedole);
                    dbComm.Parameters.AddWithValue("ritenuta_fiscale", managerLiquidAsset.RitenutaFiscale);
                    dbComm.Parameters.AddWithValue("valore_cambio", managerLiquidAsset.Valore_di_cambio);
                    dbComm.Parameters.AddWithValue("profit_loss", managerLiquidAsset.ProfitLoss);
                    dbComm.Parameters.AddWithValue("disponibile", managerLiquidAsset.Available);
                    dbComm.Parameters.AddWithValue("note", managerLiquidAsset.Note);
                    dbComm.Parameters.AddWithValue("attivo", managerLiquidAsset.Attivo);
                    dbComm.Parameters.AddWithValue("id_portafoglio_titoli", managerLiquidAsset.Id_portafoglio);
                    dbComm.Parameters.AddWithValue("link_movimenti", managerLiquidAsset.Link_Movimenti.ToString("yyyy-MM-dd HH:mm:ss"));
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbComm.Connection.Open();
                    dbComm.ExecuteNonQuery();
                    dbComm.Connection.Close();
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public PortafoglioTitoliList GetShareMovements(int IdOwner, int IdLocation, uint id_titolo)
        {
            try
            {
                DataTable DT = new DataTable();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetShareMovements;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("owner", IdOwner);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_conto", IdLocation);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_titolo", id_titolo);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
                    return MLAL(DT);
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        private PortafoglioTitoliList MLAL(DataTable dt)
        {
            PortafoglioTitoliList MLAL = new PortafoglioTitoliList();
            foreach (DataRow dr in dt.Rows)
            {
                MLAL.Add(MLA(dr));
            }
            return MLAL;
        }

        private PortafoglioTitoli MLA(DataRow dr)
        {
            PortafoglioTitoli MLA = new PortafoglioTitoli();
            MLA.Id_portafoglio = (int)dr.Field<uint>("id_portafoglio_titoli");
            MLA.Id_gestione = (int)dr.Field<uint>("id_gestione");
            MLA.Nome_Gestione = dr.Field<string>("nome_gestione");
            MLA.Id_conto = (int)dr.Field<uint>("id_conto");
            MLA.Desc_conto = dr.Field<string>("desc_conto");
            MLA.Id_valuta = (int)dr.Field<uint>("id_valuta");
            MLA.Cod_valuta = dr.Field<string>("cod_valuta");
            MLA.Id_tipo_movimento = (int)dr.Field<uint>("id_tipo_movimento");
            MLA.Desc_tipo_movimento = dr.Field<string>("desc_Movimento");
            MLA.Desc_azienda = dr.Field<string>("desc_azienda");
            MLA.Id_titolo = dr.Field<uint?>("id_titolo");
            MLA.Desc_titolo = dr.Field<string>("desc_titolo");
            MLA.Isin = dr.Field<string>("isin");
            MLA.Id_tipo_titolo = dr.Field<uint>("id_tipo_titolo");
            MLA.Desc_tipo_titolo = dr.Field<string>("desc_tipo_titolo");
            MLA.Data_Movimento = dr.Field<DateTime>("data_movimento");
            MLA.Importo_totale = dr.Field<double>("ammontare");
            MLA.N_titoli = dr.Field<double>("shares_quantity");
            MLA.Costo_unitario_in_valuta = dr.Field<double>("unity_local_value");
            MLA.Commissioni_totale = dr.Field<double>("total_commission");
            MLA.TobinTax = dr.Field<double>("tobin_tax");
            MLA.Disaggio_anticipo_cedole = dr.Field<double>("disaggio_cedole");
            MLA.RitenutaFiscale = dr.Field<double>("ritenuta_fiscale");
            MLA.Valore_di_cambio = dr.Field<double>("valore_cambio");
            MLA.Note = dr.Field<string>("note");
            MLA.Attivo = dr.Field<int>("attivo");
            MLA.Link_Movimenti = dr.Field<DateTime>("link_movimenti");
            return MLA;
        }
        #region ContoCorrente

        private ContoCorrenteList contoCorrentes(DataTable dataTable)
        {
            ContoCorrenteList lista = new ContoCorrenteList();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                lista.Add(contoCorrente(dataRow));
            }
            return lista;
        }

        private ContoCorrente contoCorrente(DataRow dataRow)
        {
            ContoCorrente conto = new ContoCorrente();
            conto.Id_RowConto = (int)dataRow.Field<uint>("id_fineco_euro");
            conto.Id_Conto = (int)dataRow.Field<uint>("id_conto");
            conto.Desc_Conto = dataRow.Field<string>("desc_conto");
            conto.Id_Quote_Investimenti = (int)dataRow.Field<uint>("id_quote_investimenti");
            conto.Id_Valuta = (int)dataRow.Field<uint>("id_valuta");
            conto.Cod_Valuta = dataRow.Field<string>("cod_valuta");
            conto.Id_Portafoglio_Titoli = (int)dataRow.Field<uint>("id_portafoglio_titoli");
            conto.Id_tipo_movimento = (int)dataRow.Field<uint>("id_tipo_movimento");
            conto.Desc_tipo_movimento = dataRow.Field<string>("desc_movimento");
            conto.Id_Gestione = (int)dataRow.Field<uint>("id_gestione");
            conto.NomeGestione = dataRow.Field<string>("nome_gestione");
            conto.Id_Titolo = (int)dataRow.Field<uint>("id_titolo");
            conto.ISIN = dataRow.Field<string>("isin");
            conto.Desc_Titolo = dataRow.Field<string>("desc_titolo");
            conto.DataMovimento = dataRow.Field<DateTime>("data_movimento");
            conto.Ammontare = dataRow.Field<double>("ammontare");
            conto.Valore_Cambio = dataRow.Field<double>("cambio");
            conto.Causale = dataRow.Field<string>("causale");
            conto.Id_Tipo_Soldi = (int)dataRow.Field<uint>("id_tipo_soldi");
            conto.Desc_Tipo_Soldi = dataRow.Field<string>("desc_tipo_soldi");
            return conto;
        }

        #endregion ContoCorrente
        public void InsertAccountMovement(ContoCorrente contoCorrente)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.ManagerScripts.InsertAccountMovement;
                    dbComm.Parameters.AddWithValue("id_conto", contoCorrente.Id_Conto);
                    dbComm.Parameters.AddWithValue("id_quote_investimenti", contoCorrente.Id_Quote_Investimenti);
                    dbComm.Parameters.AddWithValue("id_valuta", contoCorrente.Id_Valuta);
                    dbComm.Parameters.AddWithValue("id_portafoglio_titoli", contoCorrente.Id_Portafoglio_Titoli);
                    dbComm.Parameters.AddWithValue("id_tipo_movimento", contoCorrente.Id_tipo_movimento);
                    dbComm.Parameters.AddWithValue("id_gestione", contoCorrente.Id_Gestione);
                    dbComm.Parameters.AddWithValue("id_titolo", contoCorrente.Id_Titolo);
                    dbComm.Parameters.AddWithValue("data_movimento", contoCorrente.DataMovimento.ToString("yyyy-MM-dd"));
                    dbComm.Parameters.AddWithValue("ammontare", contoCorrente.Ammontare);
                    dbComm.Parameters.AddWithValue("cambio", contoCorrente.Valore_Cambio);
                    dbComm.Parameters.AddWithValue("causale", contoCorrente.Causale);
                    dbComm.Parameters.AddWithValue("id_tipo_soldi", contoCorrente.Id_Tipo_Soldi);
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbComm.Connection.Open();
                    dbComm.ExecuteNonQuery();
                    dbComm.Connection.Close();
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Calcola la quota ultima base investimento attivo
        /// restituendo il totale immesso, prelevato, assegnato e disponibile
        /// </summary>
        /// <returns>Una lista con le quote per investitore</returns>
        public QuoteInvList GetQuoteInv()
        {
            try
            {
                DataTable DT = new DataTable();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetQuoteInv;
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
                    QuoteInvList quotes = new QuoteInvList();
                    foreach (DataRow dataRow in DT.Rows)
                    {
                        QuoteInv quote = new QuoteInv();
                        quote.NomeInvestitore = dataRow.Field<string>("nome_gestione");
                        quote.CapitaleImmesso = dataRow.Field<double>("CapitaleImmesso");
                        quote.TotaleImmesso = dataRow.Field<double>("TotaleImmesso");
                        quote.CapitalePrelevato = dataRow.Field<double>("CapitalePrelevato");
                        quote.TotalePrelevato = dataRow.Field<double>("TotalePrelevato");
                        quote.CapitaleAttivo = dataRow.Field<double>("CapitaleAttivo");
                        quote.TotaleAttivo = dataRow.Field<double>("TotaleAttivo");
                        quote.QuotaInv = dataRow.Field<double>("QuotaInv");
                        quote.CapitaleAssegnato = dataRow.Field<double>("CapitaleAssegnato");
                        quote.TotaleAssegnato = dataRow.Field<double>("TotaleAssegnato");
                        quote.CapitaleDisponibile = dataRow.Field<double>("CapitaleDisponibile");
                        quote.TotaleDisponibile = dataRow.Field<double>("TotaleDisponibile");
                        quotes.Add(quote);
                    }
                    return quotes;
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Calcola le quote di guadagno per investitore applicando
        /// le quote di investimento per periodo.
        /// </summary>
        /// <param name="sintetico">se vero genera la sintesi altrimenti il dettaglio</param>
        /// <returns>Una lista con i dati per investitore</returns>
        public QuoteGuadagnoList GetQuoteGuadagno(bool sintetico)
        {
            try
            {
                DataTable DT = new DataTable();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = !sintetico ? SQL.ManagerScripts.GetQuoteDettaglioGuadagno : SQL.ManagerScripts.GetQuoteSintesiGuadagno;
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
                    QuoteGuadagnoList quotes = new QuoteGuadagnoList();
                    foreach (DataRow dataRow in DT.Rows)
                    {
                        QuoteGuadagno quote = new QuoteGuadagno();
                        quote.Nome = dataRow.Field<string>("nome_gestione");
                        if (sintetico)
                            quote.Anno = dataRow.Field<int>("anno");
                        else
                        {
                            quote.DataInizio = dataRow.Field<DateTime>("data_inizio");
                            quote.DataFine = dataRow.Field<DateTime>("data_fine");
                            quote.QuotaInv = dataRow.Field<double>("quota");
                        }
                        quote.TotaleCedole = dataRow.Field<double>("cedole");
                        quote.TotaleUtili = dataRow.Field<double>("utili");
                        quote.TotaleVolatili = dataRow.Field<double>("volatili");
                        quote.TotaleGuadagno = dataRow.Field<double>("totale");
                        quotes.Add(quote);
                    }
                    return quotes;
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Richiede una lista dei movimenti per data degli investimenti
        /// </summary>
        /// <returns>Una lista con i movimenti per data degli investimenti</returns>
        public QuoteTabList GetQuoteTab()
        {
            try
            {
                DataTable DT = new DataTable();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetQuoteTab;
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
                    QuoteTabList quotes = new QuoteTabList();
                    foreach (DataRow dataRow in DT.Rows)
                    {
                        QuoteTab quote = new QuoteTab();
                        quote.IdQuote = (int)dataRow.Field<uint>("id_quote_inv");
                        quote.IdGestione = (int)dataRow.Field<uint>("id_gestione");
                        quote.NomeInvestitore = dataRow.Field<string>("nome_gestione");
                        quote.Id_tipo_movimento = (int)dataRow.Field<uint>("id_tipo_movimento");
                        quote.Desc_tipo_movimento = dataRow.Field<string>("desc_movimento");
                        quote.DataMovimento = dataRow.Field<DateTime>("data_movimento");
                        quote.Ammontare = dataRow.Field<double>("ammontare");
                        quote.Note = dataRow.Field<string>("note");
                        quotes.Add(quote);
                    }
                    return quotes;
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Aggiorna la tabella con i movimenti degli investitori
        /// </summary>
        /// <param name="ActualQuote">I dati del movimento da modificare</param>
        public void UpdateQuoteTab(QuoteTab ActualQuote)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.ManagerScripts.UpdateQuoteTab;
                    dbComm.Parameters.AddWithValue("id_quote_inv", ActualQuote.IdQuote);
                    dbComm.Parameters.AddWithValue("id_gestione", ActualQuote.IdGestione);
                    dbComm.Parameters.AddWithValue("id_tipo_movimento", ActualQuote.Id_tipo_movimento);
                    dbComm.Parameters.AddWithValue("data_movimento", ActualQuote.DataMovimento.ToString("yyyy-MM-dd"));
                    dbComm.Parameters.AddWithValue("ammontare", ActualQuote.Ammontare);
                    dbComm.Parameters.AddWithValue("note", ActualQuote.Note);
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbComm.Connection.Open();
                    dbComm.ExecuteNonQuery();
                    dbComm.Connection.Close();
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Elimina un record dalla tabella di quote_investimenti
        /// </summary>
        /// <param name="idQuote">Il record da eliminare</param>
        public void DeleteRecordQuoteTab(int idQuote)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.ManagerScripts.DeleteRecordQuoteTab;
                    dbComm.Parameters.AddWithValue("id_quote_inv", idQuote);
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbComm.Connection.Open();
                    dbComm.ExecuteNonQuery();
                    dbComm.Connection.Close();
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public void AddGiroconto()
        {
            throw new NotImplementedException();
        }

        public void UpdateGiroconto(int idQuote)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserisce un nuovo movimento all'interno della tabella QuoteInvestimenti
        /// </summary>
        /// <param name="ActualQuote">I dati del movimento da inserire</param>
        public void InsertInvestment(QuoteTab ActualQuote)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.ManagerScripts.InsertInvestment;
                    dbComm.Parameters.AddWithValue("id_investitore", ActualQuote.IdGestione);
                    dbComm.Parameters.AddWithValue("id_tipo_movimento", ActualQuote.Id_tipo_movimento);
                    dbComm.Parameters.AddWithValue("data_movimento", ActualQuote.DataMovimento.ToString("yyyy-MM-dd"));
                    dbComm.Parameters.AddWithValue("ammontare", ActualQuote.Ammontare);
                    dbComm.Parameters.AddWithValue("note", ActualQuote.Note);
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbComm.Connection.Open();
                    dbComm.ExecuteNonQuery();
                    dbComm.Connection.Close();
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Ritorna i dati dell'ultimo movimento di capitali effettuato
        /// </summary>
        /// <returns>Record con i dati</returns>
        public QuoteTab GetLastQuoteTab()
        {
            try
            {
                DataTable DT = new DataTable();
                QuoteTab quote = new QuoteTab();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetLastQuoteTab;
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
                    quote.IdQuote = (int)DT.Rows[0].Field<uint>("id_quote_inv");
                    quote.IdGestione = (int)DT.Rows[0].Field<uint>("id_investitore");
                    quote.NomeInvestitore = DT.Rows[0].Field<string>("Nome");
                    quote.Id_tipo_movimento = (int)DT.Rows[0].Field<uint>("id_tipo_movimento");
                    quote.Desc_tipo_movimento = DT.Rows[0].Field<string>("desc_movimento");
                    quote.DataMovimento = DT.Rows[0].Field<DateTime>("data_movimento");
                    quote.Ammontare = DT.Rows[0].Field<double>("ammontare");
                    quote.Note = DT.Rows[0].Field<string>("note");
                    return quote;
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
        
        /// <summary>
        /// Estrae tutti i movimenti in ordine di data del conto corrente
        /// </summary>
        /// <returns>Lista con tutti i movimenti</returns>
        public ContoCorrenteList GetContoCorrenteList()
        {
            try
            {
                DataTable DT = new DataTable();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetContoCorrente;
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
                    return contoCorrentes(DT);
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
        
        /// <summary>
        /// Dato il codice di un movimento estrae tutti i record in ordine di data
        /// </summary>
        /// <param name="idMovimento">Codice del movimento</param>
        /// <returns>ObservableCollection con tutti i movimenti</returns>
        public ContoCorrenteList GetContoCorrenteByMovement(int idMovimento)
        {
            try
            {
                DataTable DT = new DataTable();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetContoCorrenteByMovement;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_tipo_movimento", idMovimento);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
                    return contoCorrentes(DT);
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Dato l'id del giroconto fra le 2 tabelle investimenti e cc
        /// estrae i dati dalla tabella conto_corrente
        /// </summary>
        /// <param name="idQuote">id_quote_inv</param>
        /// <returns>Record di tipo Conto Corrente</returns>
        public ContoCorrenteList GetContoCorrenteByIdQuote(int idQuote)
        {
            try
            {
                DataTable DT = new DataTable();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetContoCorrenteByIdQuote;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_quote_investimenti", idQuote);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
                    return contoCorrentes(DT);
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Dato l'id del portafoglio titoli estrae i dati dalla tabella conto_corrente
        /// </summary>
        /// <param name="idPortafoglioTitoli">id_portafoglio_titoli</param>
        /// <returns>Record di tipo Conto Corrente</returns>
        public ContoCorrenteList GetContoCorrenteByIdPortafoglio(int idPortafoglioTitoli)
        {
            try
            {
                DataTable DT = new DataTable();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetContoCorrenteByIdPortafoglio;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_portafoglio_titoli", idPortafoglioTitoli);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
                    return contoCorrentes(DT);
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Elimina un record dalla tabella ContoCorrente
        /// sulla base di un id di riga
        /// </summary>
        /// <param name="idCC">id del record da eliminare</param>
        public void DeleteRecordContoCorrente(int idCC)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.ManagerScripts.DeleteAccount;
                    dbComm.Parameters.AddWithValue("id_fineco_euro", idCC);
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbComm.Connection.Open();
                    dbComm.ExecuteNonQuery();
                    dbComm.Connection.Close();
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Elimina un record dalla tabella conto_corrente
        /// sulla base del id_portafoglio_titoli
        /// </summary>
        /// <param name="idContoTitoli">id-portafoglio_titoli</param>
        public void DeleteContoCorrenteByIdPortafoglioTitoli(int idContoTitoli)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = ManagerScripts.DeleteContoCorrenteByIdPortafoglioTitoli;
                    dbComm.Parameters.AddWithValue("id_portafoglio_titoli", idContoTitoli);
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbComm.Connection.Open();
                    dbComm.ExecuteNonQuery();
                    dbComm.Connection.Close();
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public void UpdateRecordContoCorrente(ContoCorrente contoCorrente, TipologiaIDContoCorrente tipologiaID)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    if (tipologiaID == TipologiaIDContoCorrente.IdContoCorrente)
                        dbComm.CommandText = ManagerScripts.UpdateContoCorrenteByIdCC;
                    else if (tipologiaID == TipologiaIDContoCorrente.IdContoTitoli)
                        dbComm.CommandText = SQL.ManagerScripts.UpdateContoCorrenteByIdPortafoglioTitoli;
                    else if (tipologiaID == TipologiaIDContoCorrente.IdQuoteInvestimenti)
                        dbComm.CommandText = SQL.ManagerScripts.UpdateContoCorrenteByIdQuote;
                    dbComm.Parameters.AddWithValue("id_fineco_euro", contoCorrente.Id_RowConto);
                    dbComm.Parameters.AddWithValue("id_conto", contoCorrente.Id_Conto);
                    dbComm.Parameters.AddWithValue("id_quote_investimenti", contoCorrente.Id_Quote_Investimenti);
                    dbComm.Parameters.AddWithValue("id_valuta", contoCorrente.Id_Valuta);
                    dbComm.Parameters.AddWithValue("id_portafoglio_titoli", contoCorrente.Id_Portafoglio_Titoli);
                    dbComm.Parameters.AddWithValue("id_tipo_movimento", contoCorrente.Id_tipo_movimento);
                    dbComm.Parameters.AddWithValue("id_gestione", contoCorrente.Id_Gestione);
                    dbComm.Parameters.AddWithValue("id_titolo", contoCorrente.Id_Titolo);
                    dbComm.Parameters.AddWithValue("data_movimento", contoCorrente.DataMovimento.ToString("yyyy-MM-dd"));
                    dbComm.Parameters.AddWithValue("ammontare", contoCorrente.Ammontare);
                    dbComm.Parameters.AddWithValue("cambio", contoCorrente.Valore_Cambio);
                    dbComm.Parameters.AddWithValue("causale", contoCorrente.Causale);
                    dbComm.Parameters.AddWithValue("id_tipo_soldi", contoCorrente.Id_Tipo_Soldi);
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbComm.Connection.Open();
                    dbComm.ExecuteNonQuery();
                    dbComm.Connection.Close();
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public Ptf_CCList GetShareActiveAndAccountMovement(int id_gestione, int id_conto, int id_titolo)
        {
            try
            {
                DataTable DT = new DataTable();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetShareActiveAndAccountMovement;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", id_gestione);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_conto", id_conto);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_titolo", id_titolo);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
                    Ptf_CCList _CCs = new Ptf_CCList();
                    foreach (DataRow row in DT.Rows)
                    {
                        Ptf_CC ptf_CC = new Ptf_CC();
                        ptf_CC.Id_portafoglio_titoli = (int)row.Field<uint>("id_portafoglio_titoli");
                        ptf_CC.Id_gestione = (int)row.Field<uint>("id_gestione");
                        ptf_CC.Id_conto = (int)row.Field<uint>("id_conto");
                        ptf_CC.Id_valuta = (int)row.Field<uint>("id_valuta");
                        ptf_CC.Id_tipo_movimento = (int)row.Field<uint>("id_tipo_movimento");
                        ptf_CC.Id_titolo = (int)row.Field<uint>("id_titolo");
                        ptf_CC.Data_Movimento = row.Field<DateTime>("data_movimento");
                        ptf_CC.ValoreAzione = row.Field<double>("ValoreAzione");
                        ptf_CC.N_titoli = row.Field<double>("shares_quantity");
                        ptf_CC.Valore_unitario_in_valuta = row.Field<double>("unity_local_value");
                        ptf_CC.Commissioni_totale = row.Field<double>("total_commission");
                        ptf_CC.TobinTax = row.Field<double>("tobin_tax");
                        ptf_CC.Disaggio_anticipo_cedole = row.Field<double>("disaggio_cedole");
                        ptf_CC.RitenutaFiscale = row.Field<double>("ritenuta_fiscale");
                        ptf_CC.Valore_di_cambio = row.Field<double>("valore_cambio");
                        ptf_CC.Note = row.Field<string>("note");
                        ptf_CC.Id_RowConto = (int)row.Field<uint>("id_fineco_euro");
                        ptf_CC.Valore_in_CC = row.Field<double>("Valore_in_CC");
                        ptf_CC.Id_Tipo_Soldi = (int)row.Field<uint>("id_tipo_soldi");
                        _CCs.Add(ptf_CC);
                    }
                    return _CCs;
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public PortafoglioTitoli GetPortafoglioTitoliById(int IdPortafoglioTitoli)
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetPortafoglioTitoliById;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_portafoglio_titoli", IdPortafoglioTitoli);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    DataTable dt = new DataTable();
                    dbAdapter.Fill(dt);
                    return MLA(dt.Rows[0]);
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public ContoCorrente GetContoCorrenteByIdCC(int idRecord)
        {
            try
            {
                DataTable DT = new DataTable();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetContoCorrenteByIdCC;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_fineco_euro", idRecord);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
                    return contoCorrente(DT.Rows[0]);
                }
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
    }
}

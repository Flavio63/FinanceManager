using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceManager.Models;
using MySql.Data.MySqlClient;

namespace FinanceManager.Services
{
    class ManagerLiquidAssetServices : SQL.DAFconnection, IManagerLiquidAssetServices
    {
        /// <summary>
        /// Aggiunge un movimento
        /// </summary>
        /// <param name="managerLiquidAsset">Il movimento da aggiungere</param>
        public void AddManagerLiquidAsset(ManagerLiquidAsset managerLiquidAsset)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.ManagerScripts.AddManagerLiquidAsset;
                    dbComm.Parameters.AddWithValue("id_portafoglio", managerLiquidAsset.IdOwner);
                    dbComm.Parameters.AddWithValue("id_location", managerLiquidAsset.IdLocation);
                    dbComm.Parameters.AddWithValue("id_valuta", managerLiquidAsset.IdCurrency);
                    dbComm.Parameters.AddWithValue("id_tipoMovimento", managerLiquidAsset.IdMovement);
                    dbComm.Parameters.AddWithValue("id_titolo", managerLiquidAsset.IdShare);
                    dbComm.Parameters.AddWithValue("id_borsa", managerLiquidAsset.IdMarket);
                    dbComm.Parameters.AddWithValue("data_movimento", managerLiquidAsset.MovementDate.ToString("yyyy-MM-dd"));
                    dbComm.Parameters.AddWithValue("ammontare", managerLiquidAsset.Amount);
                    dbComm.Parameters.AddWithValue("shares_quantity", managerLiquidAsset.SharesQuantity);
                    dbComm.Parameters.AddWithValue("unity_local_value", managerLiquidAsset.UnityLocalValue);
                    dbComm.Parameters.AddWithValue("total_commission", managerLiquidAsset.TotalCommission);
                    dbComm.Parameters.AddWithValue("tobin_tax", managerLiquidAsset.TobinTax);
                    dbComm.Parameters.AddWithValue("disaggio_cedole", managerLiquidAsset.DisaggioCoupons);
                    dbComm.Parameters.AddWithValue("ritenuta_fiscale", managerLiquidAsset.RitenutaFiscale);
                    dbComm.Parameters.AddWithValue("valore_cambio", managerLiquidAsset.ExchangeValue);
                    dbComm.Parameters.AddWithValue("disponibile", managerLiquidAsset.Available);
                    dbComm.Parameters.AddWithValue("note", managerLiquidAsset.Note);
                    dbComm.Connection = new MySqlConnection(DafConnection);
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
                    dbComm.Parameters.AddWithValue("id_liquid_movement", id);
                    dbComm.Connection = new MySqlConnection(DafConnection);
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
        /// Estrae tutti i record di una gestione, in un conto per la valuta selezionata
        /// </summary>
        /// <param name="IdOwner">La gestione</param>
        /// <param name="IdLocation">Il conto</param>
        /// <param name="IdCurrency">La valuta</param>
        /// <returns>Tabella con 2 record: il totale disponibile e quello messo da parte</returns>
        public DataTable GetCurrencyAvailable(int IdOwner, int IdLocation, int IdCurrency)
        {
            try
            {
                DataTable DT = new DataTable();
                using(MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetCurrencyAvailable;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("owner", IdOwner);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("location", IdLocation);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("currency", IdCurrency);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    dbAdapter.Fill(DT);
                    return DT;
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
        public ManagerLiquidAssetList GetManagerLiquidAssetListByOwnerAndLocation(int idOwner, int idLocation)
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetManagerLiquidAssetListByOwnerAndLocation;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("owner", idOwner);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("location", idLocation);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    DataTable dt = new DataTable();
                    dbAdapter.Fill(dt);
                    ManagerLiquidAssetList MLAL = new ManagerLiquidAssetList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        ManagerLiquidAsset MLA = new ManagerLiquidAsset();
                        MLA.idLiquidAsset = (int)dr.Field<uint>("id_liquid_movement");
                        MLA.IdOwner = (int)dr.Field<uint>("id_portafoglio");
                        MLA.OwnerName = dr.Field<string>("desc_portafoglio");
                        MLA.IdLocation = (int)dr.Field<uint>("id_location");
                        MLA.DescLocation = dr.Field<string>("desc_location");
                        MLA.IdCurrency = (int)dr.Field<uint>("id_valuta");
                        MLA.CodeCurrency = dr.Field<string>("cod_valuta");
                        MLA.IdMovement = (int)dr.Field<uint>("id_tipoMovimento");
                        MLA.DescMovement = dr.Field<string>("desc_Movimento");
                        MLA.IdShare = dr.Field<uint?>("id_titolo");
                        MLA.Isin = dr.Field<string>("isin");
                        MLA.IdMarket = dr.Field<uint?>("id_borsa");
                        MLA.DescMarket = dr.Field<string>("desc_borsa");
                        MLA.MovementDate = dr.Field<DateTime>("data_movimento");
                        MLA.Amount = dr.Field<double>("ammontare");
                        MLA.SharesQuantity = dr.Field<double>("shares_quantity");
                        MLA.UnityLocalValue = dr.Field<double>("unity_local_value");
                        MLA.TotalCommission = dr.Field<double>("total_commission");
                        MLA.TobinTax = dr.Field<double>("tobin_tax");
                        MLA.DisaggioCoupons = dr.Field<double>("disaggio_cedole");
                        MLA.RitenutaFiscale = dr.Field<double>("ritenuta_fiscale");
                        MLA.ExchangeValue = dr.Field<double>("valore_cambio");
                        MLA.Available = Convert.ToBoolean(dr.Field<string>("disp"));
                        MLA.Note = dr.Field<string>("note");
                        MLAL.Add(MLA);
                    }
                    return MLAL;
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
        public ManagerLiquidAssetList GetManagerLiquidAssetListByOwnerLocationAndMovementType(int IdOwner, int IdLocation, int[] IdsMovement)
        {
            try
            {
                if (IdsMovement.Length == 0)
                    throw new Exception("Errore nella richiesta dei movimenti");
                string query = "(";
                string IdRequest = "id_tipoMovimento = ";

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
                    dbAdapter.SelectCommand.Parameters.AddWithValue("owner", IdOwner);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_location", IdLocation);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    DataTable dt = new DataTable();
                    dbAdapter.Fill(dt);
                    ManagerLiquidAssetList MLAL = new ManagerLiquidAssetList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        ManagerLiquidAsset MLA = new ManagerLiquidAsset();
                        MLA.idLiquidAsset = (int)dr.Field<uint>("id_liquid_movement");
                        MLA.IdOwner = (int)dr.Field<uint>("id_portafoglio");
                        MLA.OwnerName = dr.Field<string>("desc_portafoglio");
                        MLA.IdLocation = (int)dr.Field<uint>("id_location");
                        MLA.DescLocation = dr.Field<string>("desc_location");
                        MLA.IdCurrency = (int)dr.Field<uint>("id_valuta");
                        MLA.CodeCurrency = dr.Field<string>("cod_valuta");
                        MLA.IdMovement = (int)dr.Field<uint>("id_tipoMovimento");
                        MLA.DescMovement = dr.Field<string>("desc_Movimento");
                        MLA.IdShare = dr.Field<uint?>("id_titolo");
                        MLA.Isin = dr.Field<string>("isin");
                        MLA.IdMarket = dr.Field<uint?>("id_borsa");
                        MLA.DescMarket = dr.Field<string>("desc_borsa");
                        MLA.MovementDate = dr.Field<DateTime>("data_movimento");
                        MLA.Amount = dr.Field<double>("ammontare");
                        MLA.SharesQuantity = dr.Field<double>("shares_quantity");
                        MLA.UnityLocalValue = dr.Field<double>("unity_local_value");
                        MLA.TotalCommission = dr.Field<double>("total_commission");
                        MLA.TobinTax = dr.Field<double>("tobin_tax");
                        MLA.DisaggioCoupons = dr.Field<double>("disaggio_cedole");
                        MLA.RitenutaFiscale = dr.Field<double>("ritenuta_fiscale");
                        MLA.ExchangeValue = dr.Field<double>("valore_cambio");
                        MLA.Available = Convert.ToBoolean(dr.Field<string>("disp"));
                        MLA.Note = dr.Field<string>("note");
                        MLAL.Add(MLA);
                    }
                    return MLAL;
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

        public ManagerLiquidAsset GetLastShareMovementByOwnerAndLocation(int IdOwner, int IdLocation)
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetLastSharesMovementByOwnerAndLocation;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("owner", IdOwner);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_location", IdLocation);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    DataTable dt = new DataTable();
                    dbAdapter.Fill(dt);
                    DataRow dr = dt.Rows[0];
                    ManagerLiquidAsset MLA = new ManagerLiquidAsset();
                    MLA.idLiquidAsset = (int)dr.Field<uint>("id_liquid_movement");
                    MLA.IdOwner = (int)dr.Field<uint>("id_portafoglio");
                    MLA.OwnerName = dr.Field<string>("desc_portafoglio");
                    MLA.IdLocation = (int)dr.Field<uint>("id_location");
                    MLA.DescLocation = dr.Field<string>("desc_location");
                    MLA.IdCurrency = (int)dr.Field<uint>("id_valuta");
                    MLA.CodeCurrency = dr.Field<string>("cod_valuta");
                    MLA.IdMovement = (int)dr.Field<uint>("id_tipoMovimento");
                    MLA.DescMovement = dr.Field<string>("desc_Movimento");
                    MLA.DescFirm = dr.Field<string>("desc_azienda");
                    MLA.IdShare = dr.Field<uint?>("id_titolo");
                    MLA.DescShare = dr.Field<string>("desc_titolo");
                    MLA.Isin = dr.Field<string>("isin");
                    MLA.IdShareType = dr.Field<uint>("id_tipo");
                    MLA.DescShareType = dr.Field<string>("desc_tipo");
                    MLA.IdMarket = dr.Field<uint?>("id_borsa");
                    MLA.DescMarket = dr.Field<string>("desc_borsa");
                    MLA.MovementDate = dr.Field<DateTime>("data_movimento");
                    MLA.Amount = dr.Field<double>("ammontare");
                    MLA.SharesQuantity = dr.Field<double>("shares_quantity");
                    MLA.UnityLocalValue = dr.Field<double>("unity_local_value");
                    MLA.TotalCommission = dr.Field<double>("total_commission");
                    MLA.TobinTax = dr.Field<double>("tobin_tax");
                    MLA.DisaggioCoupons = dr.Field<double>("disaggio_cedole");
                    MLA.RitenutaFiscale = dr.Field<double>("ritenuta_fiscale");
                    MLA.ExchangeValue = dr.Field<double>("valore_cambio");
                    MLA.Available = Convert.ToBoolean(dr.Field<string>("disp"));
                    MLA.Note = dr.Field<string>("note");
                    return MLA;
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
        public ManagerLiquidAssetList GetManagerSharesMovementByOwnerAndLocation(int IdOwner, int IdLocation)
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetManagerSharesMovementByOwnerAndLocation;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("owner", IdOwner);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_location", IdLocation);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    DataTable dt = new DataTable();
                    dbAdapter.Fill(dt);
                    ManagerLiquidAssetList MLAL = new ManagerLiquidAssetList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        ManagerLiquidAsset MLA = new ManagerLiquidAsset();
                        MLA.idLiquidAsset = (int)dr.Field<uint>("id_liquid_movement");
                        MLA.IdOwner = (int)dr.Field<uint>("id_portafoglio");
                        MLA.OwnerName = dr.Field<string>("desc_portafoglio");
                        MLA.IdLocation = (int)dr.Field<uint>("id_location");
                        MLA.DescLocation = dr.Field<string>("desc_location");
                        MLA.IdCurrency = (int)dr.Field<uint>("id_valuta");
                        MLA.CodeCurrency = dr.Field<string>("cod_valuta");
                        MLA.IdMovement = (int)dr.Field<uint>("id_tipoMovimento");
                        MLA.DescMovement = dr.Field<string>("desc_Movimento");
                        MLA.DescFirm = dr.Field<string>("desc_azienda");
                        MLA.IdShare = dr.Field<uint?>("id_titolo");
                        MLA.DescShare = dr.Field<string>("desc_titolo");
                        MLA.Isin = dr.Field<string>("isin");
                        MLA.IdShareType = dr.Field<uint>("id_tipo");
                        MLA.DescShareType = dr.Field<string>("desc_tipo");
                        MLA.IdMarket = dr.Field<uint?>("id_borsa");
                        MLA.DescMarket = dr.Field<string>("desc_borsa");
                        MLA.MovementDate = dr.Field<DateTime>("data_movimento");
                        MLA.Amount = dr.Field<double>("ammontare");
                        MLA.SharesQuantity = dr.Field<double>("shares_quantity");
                        MLA.UnityLocalValue = dr.Field<double>("unity_local_value");
                        MLA.TotalCommission = dr.Field<double>("total_commission");
                        MLA.TobinTax = dr.Field<double>("tobin_tax");
                        MLA.DisaggioCoupons = dr.Field<double>("disaggio_cedole");
                        MLA.RitenutaFiscale = dr.Field<double>("ritenuta_fiscale");
                        MLA.ExchangeValue = dr.Field<double>("valore_cambio");
                        MLA.Available = Convert.ToBoolean(dr.Field<string>("disp"));
                        MLA.Note = dr.Field<string>("note");
                        MLAL.Add(MLA);
                    }
                    return MLAL;
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
        /// <param name="idShare">Il titolo</param>
        /// <returns>ritorna il numero di titoli</returns>
        public double GetSharesQuantity(int IdOwner, int IdLocation, uint idShare)
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetSharesQuantity;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", IdOwner);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_location", IdLocation);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_titolo", idShare);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
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
        public void UpdateManagerLiquidAsset(ManagerLiquidAsset managerLiquidAsset)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.ManagerScripts.UpdateManagerLiquidAsset;
                    dbComm.Parameters.AddWithValue("id_portafoglio", managerLiquidAsset.IdOwner);
                    dbComm.Parameters.AddWithValue("id_location", managerLiquidAsset.IdLocation);
                    dbComm.Parameters.AddWithValue("id_valuta", managerLiquidAsset.IdCurrency);
                    dbComm.Parameters.AddWithValue("id_tipoMovimento", managerLiquidAsset.IdMovement);
                    dbComm.Parameters.AddWithValue("id_titolo", managerLiquidAsset.IdShare);
                    dbComm.Parameters.AddWithValue("id_borsa", managerLiquidAsset.IdMarket);
                    dbComm.Parameters.AddWithValue("data_movimento", managerLiquidAsset.MovementDate.ToString("yyyy-MM-dd"));
                    dbComm.Parameters.AddWithValue("ammontare", managerLiquidAsset.Amount);
                    dbComm.Parameters.AddWithValue("shares_quantity", managerLiquidAsset.SharesQuantity);
                    dbComm.Parameters.AddWithValue("unity_local_value", managerLiquidAsset.UnityLocalValue);
                    dbComm.Parameters.AddWithValue("total_commission", managerLiquidAsset.TotalCommission);
                    dbComm.Parameters.AddWithValue("tobin_tax", managerLiquidAsset.TobinTax);
                    dbComm.Parameters.AddWithValue("disaggio_cedole", managerLiquidAsset.DisaggioCoupons);
                    dbComm.Parameters.AddWithValue("ritenuta_fiscale", managerLiquidAsset.RitenutaFiscale);
                    dbComm.Parameters.AddWithValue("valore_cambio", managerLiquidAsset.ExchangeValue);
                    dbComm.Parameters.AddWithValue("disponibile", managerLiquidAsset.Available);
                    dbComm.Parameters.AddWithValue("note", managerLiquidAsset.Note);
                    dbComm.Parameters.AddWithValue("id_liquid_movement", managerLiquidAsset.idLiquidAsset);
                    dbComm.Connection = new MySqlConnection(DafConnection);
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
    }
}

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
        /// Estrae tutti il valore disponibile sulla base dei parametri dati
        /// </summary>
        /// <param name="IdOwner">La gestione</param>
        /// <param name="IdLocation">Il conto</param>
        /// <param name="IdCurrency">La valuta</param>
        /// <returns>Tabella con 2 record: il totale disponibile e quello messo da parte</returns>
        public double GetCurrencyAvailable(int IdOwner, int IdLocation, int IdCurrency)
        {
            try
            {
                DataTable DT = new DataTable();
                using(MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetCurrencyAvailable;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", IdOwner);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_conto", IdLocation);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_valuta", IdCurrency);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    dbAdapter.Fill(DT);
                    if (((DataRow)DT.Rows[0]).ItemArray[0].ToString() != "")
                        return Convert.ToDouble(((DataRow)DT.Rows[0]).ItemArray[0]);
                    else
                        return 0;
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
        public ManagerLiquidAssetList GetManagerLiquidAssetListByOwnerLocationAndMovementType(int IdOwner, int IdLocation, int[] IdsMovement)
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
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
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

        public ManagerLiquidAsset GetLastShareMovementByOwnerAndLocation(int IdOwner, int IdLocation)
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
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
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
        public ManagerLiquidAssetList GetManagerSharesMovementByOwnerAndLocation(int IdOwner, int IdLocation)
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
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
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
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
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
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_conto", IdLocation);
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
                    dbComm.Parameters.AddWithValue("id_portafoglio", managerLiquidAsset.Id_portafoglio);
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

        public ManagerLiquidAssetList GetShareMovements(int IdOwner, int IdLocation, uint IdShare)
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
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_titolo", IdShare);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
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
                throw  new Exception(err.Message);
            }
        }

        private ManagerLiquidAssetList MLAL (DataTable dt)
        {
            ManagerLiquidAssetList MLAL = new ManagerLiquidAssetList();
            foreach (DataRow dr in dt.Rows)
            {
                MLAL.Add(MLA(dr));
            }
            return MLAL;
        }

        private ManagerLiquidAsset MLA (DataRow dr)
        {
            ManagerLiquidAsset MLA = new ManagerLiquidAsset();
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
            return MLA;
        }

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
                    dbComm.Parameters.AddWithValue("id_tipo_movimento", contoCorrente.Id_Tipo_Movimento);
                    dbComm.Parameters.AddWithValue("id_gestione", contoCorrente.Id_Gestione);
                    dbComm.Parameters.AddWithValue("id_titolo", contoCorrente.Id_Titolo);
                    dbComm.Parameters.AddWithValue("data_movimento", contoCorrente.Data_Movimento.ToString("yyyy-MM-dd"));
                    dbComm.Parameters.AddWithValue("ammontare", contoCorrente.Ammontare);
                    dbComm.Parameters.AddWithValue("valore_cambio", contoCorrente.Valore_Cambio);
                    dbComm.Parameters.AddWithValue("causale", contoCorrente.Causale);
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

        public QuoteList GetQuote()
        {
            try
            {
                DataTable DT = new DataTable();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetQuote;
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    dbAdapter.Fill(DT);
                    QuoteList quotes = new QuoteList();
                    foreach(DataRow dataRow in DT.Rows)
                    {
                        Quote quote = new Quote();
                        quote.NomeInvestitore = dataRow.Field<string>("Nome");
                        quote.Ammontare = dataRow.Field<double>("investimento");
                        quote.Quota = dataRow.Field<double>("quota");
                        quote.Totale = dataRow.Field<double>("totale");
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
        public InvestitoreList GetInvestitori()
        {
            try
            {
                DataTable DT = new DataTable();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetInvestitori;
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    dbAdapter.Fill(DT);
                    InvestitoreList investitori = new InvestitoreList();
                    foreach (DataRow dataRow in DT.Rows)
                    {
                        Investitore investitore = new Investitore();
                        investitore.IdInvestitore = (int)dataRow.Field<uint>("id_investitore");
                        investitore.NomeInvestitore = dataRow.Field<string>("Nome");
                        investitori.Add(investitore);
                    }
                    return investitori;
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
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    dbAdapter.Fill(DT);
                    QuoteTabList quotes = new QuoteTabList();
                    foreach (DataRow dataRow in DT.Rows)
                    {
                        QuoteTab quote = new QuoteTab();
                        quote.IdQuote = (int)dataRow.Field<uint>("id_quote_inv");
                        quote.IdInvestitore = (int)dataRow.Field<uint>("id_investitore");
                        quote.NomeInvestitore = dataRow.Field<string>("Nome");
                        quote.IdMovimento = (int)dataRow.Field<uint>("id_movimento");
                        quote.Movimento = dataRow.Field<string>("desc_movimento");
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

        public void AddGiroconto()
        {
            throw new NotImplementedException();
        }

        public void UpdateQuoteTab(int idQuote)
        {
            throw new NotImplementedException();
        }

        public void UpdateGiroconto(int idQuote)
        {
            throw new NotImplementedException();
        }
    }
}

﻿using System;
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
                    dbComm.Parameters.AddWithValue("data_movimento", managerLiquidAsset.MovementDate.ToString("yyyy-MM-dd"));
                    dbComm.Parameters.AddWithValue("ammontare", managerLiquidAsset.Amount);
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

        public ManagerLiquidAssetList GetManagerLiquidAssetListByOwner(int idOwner)
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetManagerLiquidAssetListByOwner;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("owner", idOwner);
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
                        MLA.IdShare = (int?)dr.Field<uint?>("id_titolo");
                        MLA.Isin = dr.Field<string>("isin");
                        MLA.MovementDate = dr.Field<DateTime>("data_movimento");
                        MLA.Amount = dr.Field<double>("ammontare");
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

        public ManagerLiquidAssetList GetManagerLiquidAssetListByOwner_MovementType(int IdOwner, int[] IdsMovement)
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
                    dbAdapter.SelectCommand.CommandText = string.Format(SQL.ManagerScripts.GetManagerLiquidAssetByOwnerByMovementType, query);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("owner", IdOwner);
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
                        MLA.IdShare = (int?)dr.Field<uint?>("id_titolo");
                        MLA.Isin = dr.Field<string>("isin");
                        MLA.MovementDate = dr.Field<DateTime>("data_movimento");
                        MLA.Amount = dr.Field<double>("ammontare");
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
                    dbComm.Parameters.AddWithValue("data_movimento", managerLiquidAsset.MovementDate.ToString("yyyy-MM-dd"));
                    dbComm.Parameters.AddWithValue("ammontare", managerLiquidAsset.Amount);
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

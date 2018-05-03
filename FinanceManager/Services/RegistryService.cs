using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Text;
using System.Threading.Tasks;
using FinanceManager.Models;

namespace FinanceManager.Services
{
    public class RegistryService : SQL.DAFconnection, IRegistryServices
    {
        #region Owner
        public void AddOwner(string name)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.Connection = new MySqlConnection(DafConnection);
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.AddOwner;
                    dbComm.Parameters.AddWithValue("name", name);
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

        public void DeleteOwner(int id)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.Connection = new MySqlConnection(DafConnection);
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.DeleteOwner;
                    dbComm.Parameters.AddWithValue("id", id);
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

        public RegistryOwner GetOwnerById(int id)
        {
            throw new NotImplementedException();
        }

        public RegistryOwner GetOwnerByName(string name)
        {
            throw new NotImplementedException();
        }

        public RegistryOwnersList GetRegistryOwners()
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetAccountList;
                    DataTable dataTable = new DataTable();
                    dbAdapter.Fill(dataTable);
                    RegistryOwnersList ROL = new RegistryOwnersList();
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        RegistryOwner RO = new RegistryOwner();
                        RO.IdOwner = (int)dr.Field<uint>("id_portafoglio");
                        RO.OwnerName = dr.Field<string>("desc_portafoglio");
                        ROL.Add(RO);
                    }
                    return ROL;
                }
            }
            catch (MySqlException err)
            {
                throw new Exception("GetRegistryOwners " + err.Message);
            }
            catch (Exception err)
            {
                throw new Exception("GetRegistryOwners " + err.Message);
            }
        }

        public void UpdateOwner(RegistryOwner owner)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.Connection = new MySqlConnection(DafConnection);
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.UpdateName;
                    dbComm.Parameters.AddWithValue("name", owner.OwnerName);
                    dbComm.Parameters.AddWithValue("id", owner.IdOwner);
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

        #endregion

        #region ShareType
        public void AddShareType(string description)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.Connection = new MySqlConnection(DafConnection);
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.AddShareType;
                    dbComm.Parameters.AddWithValue("desc", description);
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

        public void DeleteShareType(int id)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.Connection = new MySqlConnection(DafConnection);
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.DeleteShareType;
                    dbComm.Parameters.AddWithValue("id", id);
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

        public RegistryShareType GetRegistryShareTypeById(int id)
        {
            throw new NotImplementedException();
        }

        public RegistryShareType GetRegistryShareTypeByName(string name)
        {
            throw new NotImplementedException();
        }

        public RegistryShareTypeList GetRegistryShareTypeList()
        {
            try
            {
                using (MySqlDataAdapter dbAdaptar = new MySqlDataAdapter())
                {
                    dbAdaptar.SelectCommand = new MySqlCommand();
                    dbAdaptar.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    dbAdaptar.SelectCommand.CommandType = CommandType.Text;
                    dbAdaptar.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryShareTypeList;
                    DataTable dt = new DataTable();
                    dbAdaptar.Fill(dt);
                    RegistryShareTypeList RSTL = new RegistryShareTypeList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        RegistryShareType RST = new RegistryShareType();
                        RST.IdShareType = (int)dr.Field<uint>("id_tipo");
                        RST.TypeName = dr.Field<string>("desc_tipo");
                        RSTL.Add(RST);
                    }
                    return RSTL;
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

        public void UpdateShareType(RegistryShareType registryShareType)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.Connection = new MySqlConnection(DafConnection);
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.UpdateShareType;
                    dbComm.Parameters.AddWithValue("id", registryShareType.IdShareType);
                    dbComm.Parameters.AddWithValue("desc", registryShareType.TypeName);
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

        #endregion

        #region Currency

        public RegistryCurrencyList GetRegistryCurrencyList()
        {
            try
            {
                using (MySqlDataAdapter dbAdaptar = new MySqlDataAdapter())
                {
                    dbAdaptar.SelectCommand = new MySqlCommand();
                    dbAdaptar.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    dbAdaptar.SelectCommand.CommandType = CommandType.Text;
                    dbAdaptar.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryCurrencyList;
                    DataTable dt = new DataTable();
                    dbAdaptar.Fill(dt);
                    RegistryCurrencyList RcL = new RegistryCurrencyList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        RegistryCurrency Rc = new RegistryCurrency();
                        Rc.IdCurrency = (int)dr.Field<uint>("id_valuta");
                        Rc.DescCurrency = dr.Field<string>("desc_valuta");
                        Rc.CodeCurrency = dr.Field<string>("cod_valuta");
                        RcL.Add(Rc);
                    }
                    return RcL;
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

        public RegistryCurrency GetRegistryCurrencyByName(string name)
        {
            throw new NotImplementedException();
        }

        public RegistryCurrency GetRegistryCurrencyById(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateCurrency(RegistryCurrency registryCurrency)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.UpdateCurrency;
                    dbComm.Parameters.AddWithValue("desc", registryCurrency.DescCurrency);
                    dbComm.Parameters.AddWithValue("code", registryCurrency.CodeCurrency);
                    dbComm.Parameters.AddWithValue("id", registryCurrency.IdCurrency);
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

        public void AddCurrency(RegistryCurrency registryCurrency)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.AddCurrency;
                    dbComm.Parameters.AddWithValue("desc", registryCurrency.DescCurrency);
                    dbComm.Parameters.AddWithValue("code", registryCurrency.CodeCurrency);
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

        public void DeleteCurrency(int id)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.DeleteCurrency;
                    dbComm.Parameters.AddWithValue("id", id);
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
        #endregion

        #region Location
        public RegistryLocationList GetRegistryLocationList()
        {
            try
            {
                using (MySqlDataAdapter dbAdaptar = new MySqlDataAdapter())
                {
                    dbAdaptar.SelectCommand = new MySqlCommand();
                    dbAdaptar.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    dbAdaptar.SelectCommand.CommandType = CommandType.Text;
                    dbAdaptar.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryLocationList;
                    DataTable dt = new DataTable();
                    dbAdaptar.Fill(dt);
                    RegistryLocationList RLL = new RegistryLocationList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        RegistryLocation RL = new RegistryLocation();
                        RL.IdLocation = (int)dr.Field<uint>("id_location");
                        RL.DescLocation = dr.Field<string>("desc_location");
                        RLL.Add(RL);
                    }
                    return RLL;
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

        public RegistryLocation GetRegistryLocationByName(string name)
        {
            throw new NotImplementedException();
        }

        public RegistryLocation GetRegistryLocationById(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateLocation(RegistryLocation registryLocation)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.UpdateLocation;
                    dbComm.Parameters.AddWithValue("desc", registryLocation.DescLocation);
                    dbComm.Parameters.AddWithValue("id", registryLocation.IdLocation);
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

        public void AddLocation(string description)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.AddLocation;
                    dbComm.Parameters.AddWithValue("desc", description);
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

        public void DeleteLocation(int id)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.DeleteLocation;
                    dbComm.Parameters.AddWithValue("id", id);
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
        #endregion

        #region Firm
        public RegistryFirmList GetRegistryFirmList()
        {
            try
            {
                using (MySqlDataAdapter dbAdaptar = new MySqlDataAdapter())
                {
                    dbAdaptar.SelectCommand = new MySqlCommand();
                    dbAdaptar.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    dbAdaptar.SelectCommand.CommandType = CommandType.Text;
                    dbAdaptar.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryFirmList;
                    DataTable dt = new DataTable();
                    dbAdaptar.Fill(dt);
                    RegistryFirmList RFL = new RegistryFirmList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        RegistryFirm RF = new RegistryFirm();
                        RF.IdFirm = (int)dr.Field<uint>("id_azienda");
                        RF.DescFirm = dr.Field<string>("desc_azienda");
                        RFL.Add(RF);
                    }
                    return RFL;
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

        public RegistryFirm GetRegistryFirmByName(string name)
        {
            throw new NotImplementedException();
        }

        public RegistryFirm GetRegistryFirmById(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateFirm(RegistryFirm registryFirm)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.UpdateFirm;
                    dbComm.Parameters.AddWithValue("desc", registryFirm.DescFirm);
                    dbComm.Parameters.AddWithValue("id", registryFirm.IdFirm);
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

        public void AddFirm(string description)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.AddFirm;
                    dbComm.Parameters.AddWithValue("desc", description);
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

        public void DeleteFirm(int id)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.DeleteFirm;
                    dbComm.Parameters.AddWithValue("id", id);
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
        #endregion

        #region Market
        public RegistryMarketList GetRegistryMarketList()
        {
            try
            {
                using (MySqlDataAdapter dbAdaptar = new MySqlDataAdapter())
                {
                    dbAdaptar.SelectCommand = new MySqlCommand();
                    dbAdaptar.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    dbAdaptar.SelectCommand.CommandType = CommandType.Text;
                    dbAdaptar.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryMarketList;
                    DataTable dt = new DataTable();
                    dbAdaptar.Fill(dt);
                    RegistryMarketList RML = new RegistryMarketList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        RegistryMarket RM = new RegistryMarket();
                        RM.IdMarket = (int)dr.Field<uint>("id_borsa");
                        RM.DescMarket = dr.Field<string>("desc_borsa");
                        RML.Add(RM);
                    }
                    return RML;
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

        public RegistryMarket GetRegistryMarketByName(string name)
        {
            throw new NotImplementedException();
        }

        public RegistryMarket GetRegistryMarketById(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateMarket(RegistryMarket registryMarket)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.UpdateMarket;
                    dbComm.Parameters.AddWithValue("desc", registryMarket.DescMarket);
                    dbComm.Parameters.AddWithValue("id", registryMarket.IdMarket);
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

        public void AddMarket(string description)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.AddMarket;
                    dbComm.Parameters.AddWithValue("desc", description);
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

        public void DeleteMarket(int id)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.DeleteMarket;
                    dbComm.Parameters.AddWithValue("id", id);
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
        #endregion

        #region Share

        public RegistryShareList GetRegistryShareList()
        {
            try
            {
                using (MySqlDataAdapter dbAdaptar = new MySqlDataAdapter())
                {
                    dbAdaptar.SelectCommand = new MySqlCommand();
                    dbAdaptar.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    dbAdaptar.SelectCommand.CommandType = CommandType.Text;
                    dbAdaptar.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryShareList;
                    DataTable dt = new DataTable();
                    dbAdaptar.Fill(dt);
                    RegistryShareList RSL = new RegistryShareList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        RegistryShare RS = new RegistryShare();
                        RS.IdShare = dr.Field<uint>("id_tit");
                        RS.DescShare = dr.Field<string>("desc_titolo");
                        RS.Isin = dr.Field<string>("isin");
                        RS.IdShareType = (int)dr.Field<uint>("id_tipo");
                        RS.IdFirm = (int)dr.Field<uint>("id_azienda");
                        RS.IdMarket = (int)dr.Field<uint>("id_borsa");
                        RS.IdCurrency = (int)dr.Field<uint>("id_valuta");
                        RSL.Add(RS);
                    }
                    return RSL;
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

        public RegistryShareList GetRegistryByShareType(int shareTypeID)
        {
            throw new NotImplementedException();
        }

        public RegistryShareList GetRegistryByCurrency(int currencyID)
        {
            throw new NotImplementedException();
        }

        public RegistryShareList GetRegistryByFirm(int firmID)
        {
            throw new NotImplementedException();
        }

        public RegistryShare GetRegistryShareByName(string name)
        {
            throw new NotImplementedException();
        }

        public RegistryShare GetRegistryShareById(int id)
        {
            throw new NotImplementedException();
        }

        public RegistryShare GetRegistryShareByIsin(string isin)
        {
            throw new NotImplementedException();
        }

        public void UpdateShare(RegistryShare registryShare)
        {
            try
            {
                using(MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.Connection = new MySqlConnection(DafConnection);
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.UpdateShare;
                    dbComm.Parameters.AddWithValue("desc", registryShare.DescShare);
                    dbComm.Parameters.AddWithValue("isin", registryShare.Isin);
                    dbComm.Parameters.AddWithValue("tipo", registryShare.IdShareType);
                    dbComm.Parameters.AddWithValue("azienda", registryShare.IdFirm);
                    dbComm.Parameters.AddWithValue("borsa", registryShare.IdMarket);
                    dbComm.Parameters.AddWithValue("valuta", registryShare.IdCurrency);
                    dbComm.Parameters.AddWithValue("id", registryShare.IdShare);
                    dbComm.Connection.Open();
                    dbComm.ExecuteNonQuery();
                    dbComm.Connection.Close();
                }
            }
            catch(MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public void AddShare(RegistryShare registryShare)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.Connection = new MySqlConnection(DafConnection);
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.AddShare;
                    dbComm.Parameters.AddWithValue("desc", registryShare.DescShare);
                    dbComm.Parameters.AddWithValue("isin", registryShare.Isin);
                    dbComm.Parameters.AddWithValue("tipo", registryShare.IdShareType);
                    dbComm.Parameters.AddWithValue("azienda", registryShare.IdFirm);
                    dbComm.Parameters.AddWithValue("borsa", registryShare.IdMarket);
                    dbComm.Parameters.AddWithValue("valuta", registryShare.IdCurrency);
                    dbComm.Parameters.AddWithValue("id", registryShare.IdShare);
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

        public void DeleteShare(uint id)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.Connection = new MySqlConnection(DafConnection);
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.DeleteShare;
                    dbComm.Parameters.AddWithValue("id", id);
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

        #endregion

        #region MovementType

        public RegistryMovementTypeList GetRegistryMovementTypesList()
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryMovementTypeList;
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    DataTable dt = new DataTable();
                    dbAdapter.Fill(dt);
                    RegistryMovementTypeList RMTL = new RegistryMovementTypeList();
                    foreach(DataRow dr in dt.Rows)
                    {
                        RegistryMovementType RMT = new RegistryMovementType();
                        RMT.IdMovement = (int)dr.Field<uint>("id_tipoMovimento");
                        RMT.DescMovement = dr.Field<string>("desc_Movimento");
                        RMTL.Add(RMT);
                    }
                    return RMTL;
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

        public void UpdateMovementType(RegistryMovementType registryMovementType)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.UpdateMovementType;
                    dbComm.Parameters.AddWithValue("desc", registryMovementType.DescMovement);
                    dbComm.Parameters.AddWithValue("id", registryMovementType.IdMovement);
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

        public void AddMovementType(string name)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.AddMovementType;
                    dbComm.Parameters.AddWithValue("desc", name);
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
        /// Delete Query
        /// </summary>
        /// <param name="id">id</param>
        public void DeleteMovementType(int id)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.DeleteMovementType;
                    dbComm.Connection = new MySqlConnection(DafConnection);
                    dbComm.Parameters.AddWithValue("id", id);
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

        #endregion

    }
}

using System;
using System.Data;
using MySql.Data.MySqlClient;
using FinanceManager.Models;
using FinanceManager.Services.SQL;

namespace FinanceManager.Services
{
    public class RegistryService : IRegistryServices
    {
        IDAFconnection DAFconnection;

        public RegistryService(IDAFconnection iDAFconnection)
        {
            DAFconnection = iDAFconnection ?? throw new ArgumentNullException("Manca la stringa di connessione al db");
        }

        #region Owner
        public void AddGestione(string name)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.AddGestione;
                    dbComm.Parameters.AddWithValue("nome", name);
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

        public void DeleteGestione(int id)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.DeleteGestione;
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

        public RegistryOwner GetGestione(int id)
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetGestione;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", id);
                    DataTable dataTable = new DataTable();
                    dbAdapter.Fill(dataTable);
                    return new RegistryOwner(){
                        Id_gestione = Convert.ToInt16(dataTable.Rows[0].ItemArray[0]),
                        Nome_Gestione = dataTable.Rows[0].ItemArray[1].ToString()
                    };


                }
            }
            catch (MySqlException err)
            {
                throw new Exception("GetOwnersName " + err.Message);
            }
            catch (Exception err)
            {
                throw new Exception("GetOwnersName " + err.Message);
            }
        }

        public RegistryOwnersList GetGestioneList()
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetGestioneList;
                    DataTable dataTable = new DataTable();
                    dbAdapter.Fill(dataTable);
                    RegistryOwnersList ROL = new RegistryOwnersList();
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        RegistryOwner RO = new RegistryOwner();
                        RO.Id_gestione = (int)dr.Field<uint>("id_gestione");
                        RO.Nome_Gestione = dr.Field<string>("nome_gestione");
                        ROL.Add(RO);
                    }
                    return ROL;
                }
            }
            catch (MySqlException err)
            {
                throw new Exception("GetGestioneList " + err.Message);
            }
            catch (Exception err)
            {
                throw new Exception("GetGestioneList " + err.Message);
            }
        }

        public void UpdateGestioneName(RegistryOwner owner)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.UpdateGestioneName;
                    dbComm.Parameters.AddWithValue("nome", owner.Nome_Gestione);
                    dbComm.Parameters.AddWithValue("id", owner.Id_gestione);
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
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
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

        public void DeleteShareType(uint id)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
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

        public RegistryShareTypeList GetRegistryShareTypeList()
        {
            try
            {
                using (MySqlDataAdapter dbAdaptar = new MySqlDataAdapter())
                {
                    dbAdaptar.SelectCommand = new MySqlCommand();
                    dbAdaptar.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdaptar.SelectCommand.CommandType = CommandType.Text;
                    dbAdaptar.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryShareTypeList;
                    DataTable dt = new DataTable();
                    dbAdaptar.Fill(dt);
                    RegistryShareTypeList RSTL = new RegistryShareTypeList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        RegistryShareType RST = new RegistryShareType();
                        RST.id_tipo_titolo = dr.Field<uint>("id_tipo_titolo");
                        RST.desc_tipo_titolo = dr.Field<string>("desc_tipo_titolo");
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
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.UpdateShareType;
                    dbComm.Parameters.AddWithValue("id", registryShareType.id_tipo_titolo);
                    dbComm.Parameters.AddWithValue("desc", registryShareType.desc_tipo_titolo);
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
                    dbAdaptar.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
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

        public void DeleteCurrency(int id)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.DeleteCurrency;
                    dbComm.Parameters.AddWithValue("id", id);
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
        #endregion

        #region Location
        public RegistryLocationList GetRegistryLocationList()
        {
            try
            {
                using (MySqlDataAdapter dbAdaptar = new MySqlDataAdapter())
                {
                    dbAdaptar.SelectCommand = new MySqlCommand();
                    dbAdaptar.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdaptar.SelectCommand.CommandType = CommandType.Text;
                    dbAdaptar.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryLocationList;
                    DataTable dt = new DataTable();
                    dbAdaptar.Fill(dt);
                    RegistryLocationList RLL = new RegistryLocationList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        RegistryLocation RL = new RegistryLocation();
                        RL.Id_conto = (int)dr.Field<uint>("id_conto");
                        RL.Desc_conto = dr.Field<string>("desc_conto");
                        RL.Note = dr.Field<string>("note");
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

        public void UpdateLocation(RegistryLocation registryLocation)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.UpdateLocation;
                    dbComm.Parameters.AddWithValue("desc", registryLocation.Desc_conto);
                    dbComm.Parameters.AddWithValue("note", registryLocation.Note);
                    dbComm.Parameters.AddWithValue("id", registryLocation.Id_conto);
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

        public void AddLocation(string description, string note)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.AddLocation;
                    dbComm.Parameters.AddWithValue("desc", description);
                    dbComm.Parameters.AddWithValue("note", note);
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

        public void DeleteLocation(int id)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.DeleteLocation;
                    dbComm.Parameters.AddWithValue("id", id);
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
        
        public RegistryLocation GetLocation(int id)
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetLocation;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id", id);
                    DataTable dataTable = new DataTable();
                    dbAdapter.Fill(dataTable);
                    return new RegistryLocation() {
                        Id_conto = Convert.ToInt16(dataTable.Rows[0].ItemArray[0]),
                        Desc_conto = dataTable.Rows[0].ItemArray[1].ToString(),
                        Note = dataTable.Rows[0].ItemArray[2].ToString()
                    };
                }
            }
            catch (MySqlException err)
            {
                throw new Exception("GetLocationName " + err.Message);
            }
            catch (Exception err)
            {
                throw new Exception("GetLocationName " + err.Message);
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
                    dbAdaptar.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdaptar.SelectCommand.CommandType = CommandType.Text;
                    dbAdaptar.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryFirmList;
                    DataTable dt = new DataTable();
                    dbAdaptar.Fill(dt);
                    RegistryFirmList RFL = new RegistryFirmList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        RegistryFirm RF = new RegistryFirm();
                        RF.id_azienda = dr.Field<uint>("id_azienda");
                        RF.desc_azienda = dr.Field<string>("desc_azienda");
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

        public void UpdateFirm(RegistryFirm registryFirm)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.UpdateFirm;
                    dbComm.Parameters.AddWithValue("desc", registryFirm.desc_azienda);
                    dbComm.Parameters.AddWithValue("id", registryFirm.id_azienda);
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

        public void AddFirm(string description)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.AddFirm;
                    dbComm.Parameters.AddWithValue("desc", description);
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

        public void DeleteFirm(uint id)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.DeleteFirm;
                    dbComm.Parameters.AddWithValue("id", id);
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
        #endregion

        #region Share

        public RegistryShareList GetRegistryShareList()
        {
            try
            {
                using (MySqlDataAdapter dbAdaptar = new MySqlDataAdapter())
                {
                    dbAdaptar.SelectCommand = new MySqlCommand();
                    dbAdaptar.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdaptar.SelectCommand.CommandType = CommandType.Text;
                    dbAdaptar.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryShareList;
                    DataTable dt = new DataTable();
                    dbAdaptar.Fill(dt);
                    RegistryShareList RSL = new RegistryShareList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        RegistryShare RS = new RegistryShare();
                        foreach (var property in RS.GetType().GetProperties())
                        {
                            property.SetValue(RS, dr.Field<object>(property.Name));
                        }
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

        public RegistryShare GetShareById(uint id)
        {
            try
            {
                using(MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetShareById;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_titolo", id);
                    DataTable dt = new DataTable();
                    dbAdapter.Fill(dt);
                    RegistryShare registryShare = new RegistryShare();
                    foreach (var property in registryShare.GetType().GetProperties())
                    {
                        property.SetValue(registryShare, dt.Rows[0].Field<object>(property.Name));
                    }
                    return registryShare;
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

        public RegistryShareList GetSharesByType(int[] id_tipo_titolo)
        {
            try
            {
                string where = "";
                foreach(int x in id_tipo_titolo)
                {
                    where += string.Format(" id_tipo_titolo = {0} OR ", x);
                }
                where = where.Remove(where.Length - 3, 3);

                using (MySqlDataAdapter dbAdaptar = new MySqlDataAdapter())
                {
                    dbAdaptar.SelectCommand = new MySqlCommand();
                    dbAdaptar.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdaptar.SelectCommand.CommandType = CommandType.Text;
                    dbAdaptar.SelectCommand.CommandText = string.Format(SQL.RegistryScripts.GetSharesByType, where);
                    DataTable dt = new DataTable();
                    dbAdaptar.Fill(dt);
                    RegistryShareList RSL = new RegistryShareList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        RegistryShare RS = new RegistryShare();
                        foreach (var property in RS.GetType().GetProperties())
                        {
                            property.SetValue(RS, dr.Field<object>(property.Name));
                        }
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
        public void UpdateShare(RegistryShare registryShare)
        {
            try
            {
                using(MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.UpdateShare;
                    foreach (var property in registryShare.GetType().GetProperties())
                    {
                        dbComm.Parameters.AddWithValue(property.Name, property.GetValue(registryShare));
                    }
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
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.AddShare;
                    foreach (var property in registryShare.GetType().GetProperties())
                    {
                        if (property.Name == "id_titolo")
                            dbComm.Parameters.AddWithValue("id_titolo", null);
                        else
                            dbComm.Parameters.AddWithValue(property.Name, property.GetValue(registryShare));
                    }

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
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
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
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    DataTable dt = new DataTable();
                    dbAdapter.Fill(dt);
                    RegistryMovementTypeList RMTL = new RegistryMovementTypeList();
                    foreach(DataRow dr in dt.Rows)
                    {
                        RegistryMovementType RMT = new RegistryMovementType();
                        RMT.Id_tipo_movimento = (int)dr.Field<uint>("id_tipo_movimento");
                        RMT.Desc_tipo_movimento = dr.Field<string>("desc_Movimento");
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
                    dbComm.Parameters.AddWithValue("desc", registryMovementType.Desc_tipo_movimento);
                    dbComm.Parameters.AddWithValue("id", registryMovementType.Id_tipo_movimento);
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

        public void AddMovementType(string name)
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = SQL.RegistryScripts.AddMovementType;
                    dbComm.Parameters.AddWithValue("desc", name);
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
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
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

        public RegistryMovementType GetMovementType (int id)
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetMovementType;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id", id);
                    DataTable dataTable = new DataTable();
                    dbAdapter.Fill(dataTable);
                    return new RegistryMovementType()
                    {
                        Id_tipo_movimento = Convert.ToInt16(dataTable.Rows[0].ItemArray[0]),
                        Desc_tipo_movimento = dataTable.Rows[0].ItemArray[1].ToString()
                    };
                }
            }
            catch (MySqlException err)
            {
                throw new Exception("GetMovementTypeName " + err.Message);
            }
            catch (Exception err)
            {
                throw new Exception("GetMovementTypeName " + err.Message);
            }
        }
        #endregion

        public TipoSoldiList GetTipoSoldiList()
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetTipoSoldiList;
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    DataTable dt = new DataTable();
                    dbAdapter.Fill(dt);
                    TipoSoldiList TSL = new TipoSoldiList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        TipoSoldi TS = new TipoSoldi((Models.Enum.TipologiaSoldi)dr.Field<uint>("id_tipo_soldi"));
                        //TS.Id_Tipo_Soldi = (int)dr.Field<uint>("id_tipo_soldi");
                        //TS.Desc_Tipo_Soldi = dr.Field<string>("desc_tipo_soldi");
                        TSL.Add(TS);
                    }
                    return TSL;
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

        public TipoSoldi GetTipoSoldiById(int id)
        {
            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetTipoSoldiById;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_tipo_soldi", id);
                    DataTable dataTable = new DataTable();
                    dbAdapter.Fill(dataTable);
                    return new TipoSoldi((Models.Enum.TipologiaSoldi)dataTable.Rows[0].ItemArray[0]);
                    //{
                    //    Id_Tipo_Soldi = Convert.ToInt16(dataTable.Rows[0].ItemArray[0]),
                    //    Desc_Tipo_Soldi = dataTable.Rows[0].ItemArray[1].ToString()
                    //};
                }
            }
            catch (MySqlException err)
            {
                throw new Exception("GetTipoSoldiById " + err.Message);
            }
            catch (Exception err)
            {
                throw new Exception("GetTipoSoldiById " + err.Message);
            }
        }

    }
}

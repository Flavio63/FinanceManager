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
                using(MySqlCommand dbComm = new MySqlCommand())
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
            throw new NotImplementedException();
        }

        public void AddCurrency(RegistryCurrency registryCurrency)
        {
            throw new NotImplementedException();
        }

        public void DeleteCurrency(int id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

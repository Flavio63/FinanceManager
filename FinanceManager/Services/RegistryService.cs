using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.SQLite;
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
        /// <summary>
        /// Aggiunge una persona
        /// </summary>
        /// <param name="name">Il nome</param>
        /// <param name="tipologia">La tipologia</param>
        public void AddGestione(string name, string tipologia)
        {
            try
            {
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = RegistryScripts.AddGestione;
                        cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        cmd.Parameters.AddWithValue("nome", name);
                        cmd.Parameters.AddWithValue("tipologia", tipologia);
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                else
                {
                    using (MySqlCommand dbComm = new MySqlCommand())
                    {
                        dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                        dbComm.CommandType = CommandType.Text;
                        dbComm.CommandText = SQL.RegistryScripts.AddGestione;
                        dbComm.Parameters.AddWithValue("nome", name);
                        dbComm.Parameters.AddWithValue("tipologia", tipologia);
                        dbComm.Connection.Open();
                        dbComm.ExecuteNonQuery();
                        dbComm.Connection.Close();
                    }
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
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
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SQL.RegistryScripts.DeleteGestione;
                        cmd.Parameters.AddWithValue("id", id);
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                else
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
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
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
        /// Estrae la lista di tutti i gestori
        /// </summary>
        /// <returns>Observable Collection</returns>
        public RegistryOwnersList GetGestioneList()
        {
            DataTable dataTable = new DataTable();
            RegistryOwnersList ROL = new RegistryOwnersList();
            if (DAFconnection.GetConnectionType().Contains("sqlite"))
            {
                try
                {
                    using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                    {
                        dataAdapter.SelectCommand = new SQLiteCommand();
                        dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dataAdapter.SelectCommand.CommandType = CommandType.Text;
                        dataAdapter.SelectCommand.CommandText = RegistryScripts.GetGestioneList;
                        dataAdapter.Fill(dataTable);
                    }
                }
                catch (SQLiteException err)
                {
                    throw new Exception(err.Message);
                }
            }
            else
            {
                try
                {
                    using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                    {
                        dbAdapter.SelectCommand = new MySqlCommand();
                        dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                        dbAdapter.SelectCommand.CommandType = CommandType.Text;
                        dbAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetGestioneList;
                        dbAdapter.Fill(dataTable);
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
            foreach (DataRow dr in dataTable.Rows)
            {
                RegistryOwner RO = new RegistryOwner();
                RO.Id_gestione = DAFconnection.GetConnectionType().Contains("sqlite") ? dr.Field<int>("id_gestione") : (int)dr.Field<uint>("id_gestione");
                RO.Nome_Gestione = dr.Field<string>("nome_gestione");
                RO.Tipologia = dr.Field<string>("tipologia");
                ROL.Add(RO);
            }
            return ROL;
        }
        /// <summary>
        /// Aggiorna i dati di una persona
        /// </summary>
        /// <param name="owner">Il record da aggiornare</param>
        public void UpdateGestioneName(RegistryOwner owner)
        {
            if (DAFconnection.GetConnectionType().Contains("sqlite"))
            {
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = RegistryScripts.UpdateGestioneName;
                        cmd.Parameters.AddWithValue("nome", owner.Nome_Gestione);
                        cmd.Parameters.AddWithValue("tipologia", owner.Tipologia);
                        cmd.Parameters.AddWithValue("id", owner.Id_gestione);
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                catch (SQLiteException err)
                {
                    throw new Exception(err.Message);
                }
            }
            else
            {
                try
                {
                    using (MySqlCommand dbComm = new MySqlCommand())
                    {
                        dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                        dbComm.CommandType = CommandType.Text;
                        dbComm.CommandText = SQL.RegistryScripts.UpdateGestioneName;
                        dbComm.Parameters.AddWithValue("nome", owner.Nome_Gestione);
                        dbComm.Parameters.AddWithValue("tipologia", owner.Tipologia);
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
        }

        #endregion

        #region ShareType
        public void AddShareType(string description)
        {
            try
            {
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteCommand dbComm = new SQLiteCommand())
                    {
                        dbComm.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dbComm.CommandType = CommandType.Text;
                        dbComm.CommandText = RegistryScripts.AddShareType;
                        dbComm.Parameters.AddWithValue("desc", description);
                        dbComm.Connection.Open();
                        dbComm.ExecuteNonQuery();
                        dbComm.Connection.Close();
                    }
                }
                else
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
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
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
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = RegistryScripts.DeleteShareType;
                        cmd.Parameters.AddWithValue("id", id);
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                else
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
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
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
            DataTable dt = new DataTable();
            try
            {
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                    {
                        dataAdapter.SelectCommand = new SQLiteCommand();
                        dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dataAdapter.SelectCommand.CommandText = RegistryScripts.GetRegistryShareTypeList;
                        dataAdapter.Fill(dt);
                    }
                }
                else
                {
                    using (MySqlDataAdapter dbAdaptar = new MySqlDataAdapter())
                    {
                        dbAdaptar.SelectCommand = new MySqlCommand();
                        dbAdaptar.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                        dbAdaptar.SelectCommand.CommandType = CommandType.Text;
                        dbAdaptar.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryShareTypeList;
                        dbAdaptar.Fill(dt);
                    }
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            RegistryShareTypeList RSTL = new RegistryShareTypeList();
            foreach (DataRow dr in dt.Rows)
            {
                RegistryShareType RST = new RegistryShareType();
                RST.id_tipo_titolo = DAFconnection.GetConnectionType().Contains("sqlite") ? (uint)dr.Field<int>("id_tipo_titolo") : dr.Field<uint>("id_tipo_titolo");
                RST.desc_tipo_titolo = dr.Field<string>("desc_tipo_titolo");
                RSTL.Add(RST);
            }
            return RSTL;
        }

        public void UpdateShareType(RegistryShareType registryShareType)
        {
            try
            {
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        cmd.CommandText = RegistryScripts.UpdateShareType;
                        cmd.Parameters.AddWithValue("id", registryShareType.id_tipo_titolo);
                        cmd.Parameters.AddWithValue("desc", registryShareType.desc_tipo_titolo);
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                else
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
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
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
            DataTable dt = new DataTable();
            RegistryCurrencyList RcL = new RegistryCurrencyList();

            if (DAFconnection.GetConnectionType().Contains("sqlite"))
            {
                using (SQLiteConnection conn = new SQLiteConnection(DAFconnection.GetConnectionType()))
                {
                    try
                    {
                        conn.Open();
                        SQLiteCommand cmd = new SQLiteCommand(RegistryScripts.GetRegistryCurrencyList, conn);
                        cmd.CommandType = CommandType.Text;
                        SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                        da.Fill(dt);
                    }
                    catch (SQLiteException err)
                    {
                        throw new Exception(err.Message);
                    }

                }
            }
            else
            {
                try
                {
                    using (MySqlDataAdapter dbAdaptar = new MySqlDataAdapter())
                    {
                        dbAdaptar.SelectCommand = new MySqlCommand();
                        dbAdaptar.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                        dbAdaptar.SelectCommand.CommandType = CommandType.Text;
                        dbAdaptar.SelectCommand.CommandText = RegistryScripts.GetRegistryCurrencyList;
                        dbAdaptar.Fill(dt);
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
            foreach (DataRow dr in dt.Rows)
            {
                RegistryCurrency Rc = new RegistryCurrency();
                Rc.IdCurrency = DAFconnection.GetConnectionType().Contains("sqlite") ? dr.Field<int>("id_valuta") : (int)dr.Field<uint>("id_valuta");
                Rc.DescCurrency = dr.Field<string>("desc_valuta");
                Rc.CodeCurrency = dr.Field<string>("cod_valuta");
                RcL.Add(Rc);
            }
            return RcL;
        }

        public void UpdateCurrency(RegistryCurrency registryCurrency)
        {
            if (DAFconnection.GetConnectionType().Contains("sqlite"))
            {
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        cmd.CommandText = RegistryScripts.UpdateCurrency;
                        cmd.Parameters.AddWithValue("desc", registryCurrency.DescCurrency);
                        cmd.Parameters.AddWithValue("code", registryCurrency.CodeCurrency);
                        cmd.Parameters.AddWithValue("id", registryCurrency.IdCurrency);
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                catch (SQLiteException err)
                {
                    throw new Exception(err.Message);
                }
            }
            else
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
        }

        public void AddCurrency(RegistryCurrency registryCurrency)
        {
            if (DAFconnection.GetConnectionType().Contains("sqlite"))
            {
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.CommandText = SQL.RegistryScripts.AddCurrency;
                        cmd.Parameters.AddWithValue("desc", registryCurrency.DescCurrency);
                        cmd.Parameters.AddWithValue("code", registryCurrency.CodeCurrency);
                        cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                catch (SQLiteException err)
                {
                    throw new Exception(err.Message);
                }
            }
            else
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
        }

        public void DeleteCurrency(int id)
        {
            if (DAFconnection.GetConnectionType().Contains("sqlite"))
            {
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = RegistryScripts.DeleteCurrency;
                        cmd.Parameters.AddWithValue("id", id);
                        cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                catch (SQLiteException err)
                {
                    throw new Exception(err.Message);
                }
            }
            else
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
        }
        #endregion

        #region Location
        public RegistryLocationList GetRegistryLocationList()
        {
            DataTable dt = new DataTable();
            try
            {
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                    {
                        dataAdapter.SelectCommand = new SQLiteCommand();
                        dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dataAdapter.SelectCommand.CommandText = RegistryScripts.GetRegistryLocationList;
                        dataAdapter.Fill(dt);
                    }
                }
                else
                {
                    using (MySqlDataAdapter dbAdaptar = new MySqlDataAdapter())
                    {
                        dbAdaptar.SelectCommand = new MySqlCommand();
                        dbAdaptar.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                        dbAdaptar.SelectCommand.CommandType = CommandType.Text;
                        dbAdaptar.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryLocationList;
                        dbAdaptar.Fill(dt);
                    }
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            RegistryLocationList RLL = new RegistryLocationList();
            foreach (DataRow dr in dt.Rows)
            {
                RegistryLocation RL = new RegistryLocation();
                RL.Id_Conto = DAFconnection.GetConnectionType().Contains("sqlite") ? dr.Field<int>("id_conto") : (int)dr.Field<uint>("id_conto");
                RL.Desc_Conto = dr.Field<string>("desc_conto");
                RL.Note = dr.Field<string>("note");
                RLL.Add(RL);
            }
            return RLL;
        }

        public void UpdateLocation(RegistryLocation registryLocation)
        {
            try
            {
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.CommandText = RegistryScripts.UpdateLocation;
                        cmd.Parameters.AddWithValue("desc", registryLocation.Desc_Conto);
                        cmd.Parameters.AddWithValue("note", registryLocation.Note);
                        cmd.Parameters.AddWithValue("id", registryLocation.Id_Conto);
                        cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                else
                {
                    using (MySqlCommand dbComm = new MySqlCommand())
                    {
                        dbComm.CommandType = CommandType.Text;
                        dbComm.CommandText = SQL.RegistryScripts.UpdateLocation;
                        dbComm.Parameters.AddWithValue("desc", registryLocation.Desc_Conto);
                        dbComm.Parameters.AddWithValue("note", registryLocation.Note);
                        dbComm.Parameters.AddWithValue("id", registryLocation.Id_Conto);
                        dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                        dbComm.Connection.Open();
                        dbComm.ExecuteNonQuery();
                        dbComm.Connection.Close();
                    }
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
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
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteCommand dbComm = new SQLiteCommand())
                    {
                        dbComm.CommandType = CommandType.Text;
                        dbComm.CommandText = SQL.RegistryScripts.AddLocation;
                        dbComm.Parameters.AddWithValue("desc", description);
                        dbComm.Parameters.AddWithValue("note", note);
                        dbComm.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dbComm.Connection.Open();
                        dbComm.ExecuteNonQuery();
                        dbComm.Connection.Close();
                    }
                }
                else
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
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
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
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteCommand dbComm = new SQLiteCommand())
                    {
                        dbComm.CommandType = CommandType.Text;
                        dbComm.CommandText = SQL.RegistryScripts.DeleteLocation;
                        dbComm.Parameters.AddWithValue("id", id);
                        dbComm.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dbComm.Connection.Open();
                        dbComm.ExecuteNonQuery();
                        dbComm.Connection.Close();
                    }
                }
                else
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
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
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
            DataTable dt = new DataTable();
            try
            {
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                    {
                        dbAdapter.SelectCommand = new SQLiteCommand();
                        dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dbAdapter.SelectCommand.CommandType = CommandType.Text;
                        dbAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryFirmList;
                        dbAdapter.Fill(dt);
                    }
                }
                else
                {
                    using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                    {
                        dbAdapter.SelectCommand = new MySqlCommand();
                        dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                        dbAdapter.SelectCommand.CommandType = CommandType.Text;
                        dbAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryFirmList;
                        dbAdapter.Fill(dt);
                    }
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            RegistryFirmList RFL = new RegistryFirmList();
            foreach (DataRow dr in dt.Rows)
            {
                RegistryFirm RF = new RegistryFirm();
                RF.id_azienda = DAFconnection.GetConnectionType().Contains("sqlite") ? (uint)dr.Field<int>("id_azienda") : dr.Field<uint>("id_azienda");
                RF.desc_azienda = dr.Field<string>("desc_azienda");
                RFL.Add(RF);
            }
            return RFL;
        }

        public void UpdateFirm(RegistryFirm registryFirm)
        {
            try
            {
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteCommand dbComm = new SQLiteCommand())
                    {
                        dbComm.CommandType = CommandType.Text;
                        dbComm.CommandText = SQL.RegistryScripts.UpdateFirm;
                        dbComm.Parameters.AddWithValue("desc", registryFirm.desc_azienda);
                        dbComm.Parameters.AddWithValue("id", registryFirm.id_azienda);
                        dbComm.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dbComm.Connection.Open();
                        dbComm.ExecuteNonQuery();
                        dbComm.Connection.Close();
                    }
                }
                else
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
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
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
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteCommand dbComm = new SQLiteCommand())
                    {
                        dbComm.CommandType = CommandType.Text;
                        dbComm.CommandText = SQL.RegistryScripts.AddFirm;
                        dbComm.Parameters.AddWithValue("desc", description);
                        dbComm.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dbComm.Connection.Open();
                        dbComm.ExecuteNonQuery();
                        dbComm.Connection.Close();
                    }
                }
                else
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
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
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
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteCommand dbComm = new SQLiteCommand())
                    {
                        dbComm.CommandType = CommandType.Text;
                        dbComm.CommandText = SQL.RegistryScripts.DeleteFirm;
                        dbComm.Parameters.AddWithValue("id", id);
                        dbComm.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dbComm.Connection.Open();
                        dbComm.ExecuteNonQuery();
                        dbComm.Connection.Close();
                    }
                }
                else
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
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
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
            DataTable dt = new DataTable();
            try
            {
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                    {
                        dataAdapter.SelectCommand = new SQLiteCommand();
                        dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dataAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryShareList;
                        dataAdapter.Fill(dt);
                    }
                }
                else
                {
                    using (MySqlDataAdapter dbAdaptar = new MySqlDataAdapter())
                    {
                        dbAdaptar.SelectCommand = new MySqlCommand();
                        dbAdaptar.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                        dbAdaptar.SelectCommand.CommandType = CommandType.Text;
                        dbAdaptar.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryShareList;
                        dbAdaptar.Fill(dt);
                    }
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            RegistryShareList RSL = new RegistryShareList();
            foreach (DataRow dr in dt.Rows)
            {
                RegistryShare RS = new RegistryShare();
                foreach (var property in RS.GetType().GetProperties())
                {
                    if (property.Name == "id_titolo")
                        property.SetValue(RS, Convert.ToUInt32(dr.Field<object>(property.Name)));
                    else if (property.Name == "id_tipo_titolo")
                        property.SetValue(RS, Convert.ToUInt32(dr.Field<object>(property.Name)));
                    else if (property.Name == "id_azienda")
                        property.SetValue(RS, Convert.ToUInt32(dr.Field<object>(property.Name)));
                    else
                        property.SetValue(RS, dr.Field<object>(property.Name));
                }
                RSL.Add(RS);
            }
            return RSL;
        }

        public RegistryShare GetShareById(uint id)
        {
            DataTable dt = new DataTable();
            try
            {
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                    {
                        dataAdapter.SelectCommand = new SQLiteCommand();
                        dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dataAdapter.SelectCommand.CommandText = RegistryScripts.GetShareById;
                        dataAdapter.SelectCommand.Parameters.AddWithValue("id_titolo", id);
                        dataAdapter.Fill(dt);
                    }
                }
                else
                {
                    using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                    {
                        dbAdapter.SelectCommand = new MySqlCommand();
                        dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                        dbAdapter.SelectCommand.CommandType = CommandType.Text;
                        dbAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetShareById;
                        dbAdapter.SelectCommand.Parameters.AddWithValue("id_titolo", id);
                        dbAdapter.Fill(dt);
                    }
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            RegistryShare registryShare = new RegistryShare();
            foreach (var property in registryShare.GetType().GetProperties())
            {
                if (property.Name == "id_titolo")
                    property.SetValue(registryShare, Convert.ToUInt32(dt.Rows[0].Field<object>(property.Name)));
                else if (property.Name == "id_tipo_titolo")
                    property.SetValue(registryShare, Convert.ToUInt32(dt.Rows[0].Field<object>(property.Name)));
                else if (property.Name == "id_azienda")
                    property.SetValue(registryShare, Convert.ToUInt32(dt.Rows[0].Field<object>(property.Name)));
                else
                    property.SetValue(registryShare, dt.Rows[0].Field<object>(property.Name));
            }
            return registryShare;
        }

        public RegistryShareList GetSharesByFirms(int[] id_aziende)
        {
            DataTable dt = new DataTable();
            try
            {
                string where = "";
                foreach (int x in id_aziende)
                {
                    where += string.Format(" id_azienda = {0} OR ", x);
                }
                where = where.Remove(where.Length - 3, 3);
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                    {
                        dataAdapter.SelectCommand = new SQLiteCommand();
                        dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dataAdapter.SelectCommand.CommandText = String.Format(RegistryScripts.GetSharesByFirms, where);
                        dataAdapter.Fill(dt);
                    }
                }
                else
                {
                    using (MySqlDataAdapter dbAdaptar = new MySqlDataAdapter())
                    {
                        dbAdaptar.SelectCommand = new MySqlCommand();
                        dbAdaptar.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                        dbAdaptar.SelectCommand.CommandType = CommandType.Text;
                        dbAdaptar.SelectCommand.CommandText = string.Format(SQL.RegistryScripts.GetSharesByFirms, where);
                        dbAdaptar.Fill(dt);
                    }
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            RegistryShareList RSL = new RegistryShareList();
            foreach (DataRow dr in dt.Rows)
            {
                RegistryShare RS = new RegistryShare();
                foreach (var property in RS.GetType().GetProperties())
                {
                    if (property.Name == "id_titolo")
                        property.SetValue(RS, Convert.ToUInt32(dr.Field<object>(property.Name)));
                    else if (property.Name == "id_tipo_titolo")
                        property.SetValue(RS, Convert.ToUInt32(dr.Field<object>(property.Name)));
                    else if (property.Name == "id_azienda")
                        property.SetValue(RS, Convert.ToUInt32(dr.Field<object>(property.Name)));
                    else
                        property.SetValue(RS, dr.Field<object>(property.Name));

                }
                RSL.Add(RS);
            }
            return RSL;
        }
        public void UpdateShare(RegistryShare registryShare)
        {
            try
            {
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        cmd.CommandText = RegistryScripts.UpdateShare;
                        foreach (var property in registryShare.GetType().GetProperties())
                        {
                            cmd.Parameters.AddWithValue(property.Name, property.GetValue(registryShare));
                        }
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                else
                {
                    using (MySqlCommand dbComm = new MySqlCommand())
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
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
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

        public void AddShare(RegistryShare registryShare)
        {
            try
            {
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        cmd.CommandText = RegistryScripts.AddShare;
                        foreach (var property in registryShare.GetType().GetProperties())
                        {
                            if (property.Name == "id_titolo")
                                cmd.Parameters.AddWithValue(property.Name, null);
                            else
                                cmd.Parameters.AddWithValue(property.Name, property.GetValue(registryShare));
                        }
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                else
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
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
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
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        cmd.CommandText = RegistryScripts.DeleteShare;
                        cmd.Parameters.AddWithValue("id", id);
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                else
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
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
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
            DataTable dt = new DataTable();
            try
            {
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                    {
                        dataAdapter.SelectCommand = new SQLiteCommand();
                        dataAdapter.SelectCommand.CommandText = RegistryScripts.GetRegistryMovementTypeList;
                        dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dataAdapter.Fill(dt);
                    }
                }
                else
                {
                    using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                    {
                        dbAdapter.SelectCommand = new MySqlCommand();
                        dbAdapter.SelectCommand.CommandType = CommandType.Text;
                        dbAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryMovementTypeList;
                        dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                        dbAdapter.Fill(dt);
                    }
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            RegistryMovementTypeList RMTL = new RegistryMovementTypeList();
            foreach (DataRow dr in dt.Rows)
            {
                RegistryMovementType RMT = new RegistryMovementType();
                RMT.Id_tipo_movimento = DAFconnection.GetConnectionType().Contains("sqlite") ? dr.Field<int>("id_tipo_movimento") : (int)dr.Field<uint>("id_tipo_movimento");
                RMT.Desc_tipo_movimento = dr.Field<string>("desc_Movimento");
                RMTL.Add(RMT);
            }
            return RMTL;
        }

        public void UpdateMovementType(RegistryMovementType registryMovementType)
        {
            try
            {
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.CommandText = RegistryScripts.UpdateMovementType;
                        cmd.Parameters.AddWithValue("desc", registryMovementType.Desc_tipo_movimento);
                        cmd.Parameters.AddWithValue("id", registryMovementType.Id_tipo_movimento);
                        cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                else
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
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
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
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.CommandText = RegistryScripts.AddMovementType;
                        cmd.Parameters.AddWithValue("desc", name);
                        cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                else
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
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
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
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.CommandText = RegistryScripts.DeleteMovementType;
                        cmd.Parameters.AddWithValue("id", id);
                        cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                else
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
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
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

        public TipoSoldiList GetTipoSoldiList()
        {
            DataTable dt = new DataTable();
            try
            {
                if (DAFconnection.GetConnectionType().Contains("sqlite"))
                {
                    using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                    {
                        dataAdapter.SelectCommand = new SQLiteCommand();
                        dataAdapter.SelectCommand.CommandText = RegistryScripts.GetTipoSoldiList;
                        dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dataAdapter.Fill(dt);
                    }
                }
                else
                {
                    using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                    {
                        dbAdapter.SelectCommand = new MySqlCommand();
                        dbAdapter.SelectCommand.CommandType = CommandType.Text;
                        dbAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetTipoSoldiList;
                        dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                        dbAdapter.Fill(dt);
                    }
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
            catch (MySqlException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            TipoSoldiList TSL = new TipoSoldiList();
            foreach (DataRow dr in dt.Rows)
            {
                TipoSoldi TS = DAFconnection.GetConnectionType().Contains("sqlite") ?
                    new TipoSoldi((Models.Enumeratori.TipologiaSoldi)dr.Field<int>("id_tipo_soldi")) :
                    new TipoSoldi((Models.Enumeratori.TipologiaSoldi)dr.Field<uint>("id_tipo_soldi"));
                TSL.Add(TS);
            }
            return TSL;
        }

    }
}

using System;
using System.Data;
using System.Data.SQLite;
using FinanceManager.Models;
using FinanceManager.Models.Enumeratori;
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

        #region Soci
        /// <summary>
        /// Ritorna tutti i nominativi legati alla
        /// gestione dei socii
        /// </summary>
        /// <returns>Observable Collection</returns>
        public RegistrySociList GetSociList()
        {
            DataTable dataTable = new DataTable();
            RegistrySociList ROL = new RegistrySociList();
            try
            {
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dataAdapter.SelectCommand.CommandText = RegistryScripts.GetSociList;
                    dataAdapter.Fill(dataTable);
                }
                foreach (DataRow dr in dataTable.Rows)
                {
                    RegistrySoci RO = new RegistrySoci();
                    RO.Id_Socio = Convert.ToInt32(dr.Field<long>("id_socio"));
                    RO.Nome_Socio = dr.Field<string>("nome_socio");
                    RO.Id_tipo_gestione = Convert.ToInt32(dr.Field<long>("id_tipo_gestione"));
                    RO.Tipo_Gestione = dr.Field<string>("tipo_gestione");
                    ROL.Add(RO);
                }
                return ROL;
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception("GetSocioList " + err.Message);
            }
        }
        /// <summary>
        /// Aggiorna il nome di un socio
        /// </summary>
        /// <param name="socio">Il record da modificare</param>
        public void UpdateSocioName(RegistrySoci socio)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    cmd.CommandText = RegistryScripts.UpdateSocioName;
                    cmd.Parameters.AddWithValue("nome", socio.Nome_Socio);
                    cmd.Parameters.AddWithValue("id_tipo_gestione", socio.Id_tipo_gestione);
                    cmd.Parameters.AddWithValue("id_socio", socio.Id_Socio);
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
        /// <summary>
        /// Aggiunge una voce alla tabella soci
        /// </summary>
        /// <param name="socio">Il record da aggiungere</param>
        public void AddSocio(RegistrySoci socio)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = RegistryScripts.AddSocio;
                    cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    cmd.Parameters.AddWithValue("nome", socio.Nome_Socio);
                    cmd.Parameters.AddWithValue("id_tipo_gestione", socio.Id_tipo_gestione);
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
        public void DeleteSocio(int id_socio)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    cmd.CommandText = SQL.RegistryScripts.DeleteSocio;
                    cmd.Parameters.AddWithValue("id", id_socio);
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
        #endregion

        #region Owner
        /// <summary>
        /// Aggiunge una persona
        /// </summary>
        /// <param name="owner">Il record da aggiornare</param>
        public void AddGestione(RegistryGestioni owner)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = RegistryScripts.AddGestione;
                    cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    cmd.Parameters.AddWithValue("nome", owner.Nome_Gestione);
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
            catch (SQLiteException err)
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
            catch (SQLiteException err)
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
        public RegistryGestioniList GetGestioneList()
        {
            DataTable dataTable = new DataTable();
            RegistryGestioniList ROL = new RegistryGestioniList();
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
                foreach (DataRow dr in dataTable.Rows)
                {
                    RegistryGestioni RO = new RegistryGestioni();
                    RO.Id_Gestione = Convert.ToInt32(dr.Field<long>("id_gestione"));
                    RO.Nome_Gestione = dr.Field<string>("nome_gestione");
                    ROL.Add(RO);
                }
                return ROL;
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception("GetGestioneList " + err.Message);
            }
        }
        /// <summary>
        /// Aggiorna i dati di una persona
        /// </summary>
        /// <param name="owner">Il record da aggiornare</param>
        public void UpdateGestioneName(RegistryGestioni owner)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = RegistryScripts.UpdateGestioneName;
                    cmd.Parameters.AddWithValue("nome", owner.Nome_Gestione);
                    cmd.Parameters.AddWithValue("id", owner.Id_Gestione);
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
            catch (SQLiteException err)
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
            catch (SQLiteException err)
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
            catch (SQLiteException err)
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
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dataAdapter.SelectCommand.CommandText = RegistryScripts.GetRegistryShareTypeList;
                    dataAdapter.Fill(dt);
                }
            }
            catch (SQLiteException err)
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
                RST.id_tipo_titolo = Convert.ToUInt32(dr.Field<long>("id_tipo_titolo"));
                RST.desc_tipo_titolo = dr.Field<string>("desc_tipo_titolo");
                RSTL.Add(RST);
            }
            return RSTL;
        }

        public void UpdateShareType(RegistryShareType registryShareType)
        {
            try
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
            catch (SQLiteException err)
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

            using (SQLiteConnection conn = new SQLiteConnection(DAFconnection.GetConnectionType()))
            {
                try
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(RegistryScripts.GetRegistryCurrencyList, conn);
                    cmd.CommandType = CommandType.Text;
                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                    da.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        RegistryCurrency Rc = new RegistryCurrency();
                        Rc.IdCurrency = Convert.ToInt32(dr.Field<long>("id_valuta"));
                        Rc.DescCurrency = dr.Field<string>("desc_valuta");
                        Rc.CodeCurrency = dr.Field<string>("cod_valuta");
                        RcL.Add(Rc);
                    }
                    return RcL;
                }
                catch (SQLiteException err)
                {
                    throw new Exception(err.Message);
                }

            }
        }

        public void UpdateCurrency(RegistryCurrency registryCurrency)
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

        public void AddCurrency(RegistryCurrency registryCurrency)
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

        public void DeleteCurrency(int id)
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
        #endregion

        #region Location
        public RegistryLocationList GetRegistryLocationList()
        {
            DataTable dt = new DataTable();
            RegistryLocationList RLL = new RegistryLocationList();
            try
            {
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dataAdapter.SelectCommand.CommandText = RegistryScripts.GetRegistryLocationList;
                    dataAdapter.Fill(dt);
                }
                foreach (DataRow dr in dt.Rows)
                {
                    RegistryLocation RL = new RegistryLocation();
                    RL.Id_Conto = Convert.ToInt32(dr.Field<long>("id_conto"));
                    RL.Desc_Conto = dr.Field<string>("desc_conto");
                    RL.Note = dr.Field<string>("note");
                    RLL.Add(RL);
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            return RLL;
        }

        public void UpdateLocation(RegistryLocation registryLocation)
        {
            try
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
            catch (SQLiteException err)
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
            catch (SQLiteException err)
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
            catch (SQLiteException err)
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
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryFirmList;
                    dbAdapter.Fill(dt);
                }
            }
            catch (SQLiteException err)
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
                RF.id_azienda = Convert.ToUInt32(dr.Field<long>("id_azienda"));
                RF.desc_azienda = dr.Field<string>("desc_azienda");
                RFL.Add(RF);
            }
            return RFL;
        }

        public void UpdateFirm(RegistryFirm registryFirm)
        {
            try
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
            catch (SQLiteException err)
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
            catch (SQLiteException err)
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
            catch (SQLiteException err)
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
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dataAdapter.SelectCommand.CommandText = SQL.RegistryScripts.GetRegistryShareList;
                    dataAdapter.Fill(dt);
                }
            }
            catch (SQLiteException err)
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
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dataAdapter.SelectCommand.CommandText = RegistryScripts.GetShareById;
                    dataAdapter.SelectCommand.Parameters.AddWithValue("id_titolo", id);
                    dataAdapter.Fill(dt);
                }
            }
            catch (SQLiteException err)
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
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dataAdapter.SelectCommand.CommandText = String.Format(RegistryScripts.GetSharesByFirms, where);
                    dataAdapter.Fill(dt);
                }
            }
            catch (SQLiteException err)
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
            catch (SQLiteException err)
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
            catch (SQLiteException err)
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
            catch (SQLiteException err)
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
                    using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                    {
                        dataAdapter.SelectCommand = new SQLiteCommand();
                        dataAdapter.SelectCommand.CommandText = RegistryScripts.GetRegistryMovementTypeList;
                        dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dataAdapter.Fill(dt);
                    }
            }
            catch (SQLiteException err)
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
                RMT.Id_tipo_movimento = Convert.ToInt32(dr.Field<long>("id_tipo_movimento"));
                RMT.Desc_tipo_movimento = dr.Field<string>("desc_Movimento");
                RMTL.Add(RMT);
            }
            return RMTL;
        }

        public void UpdateMovementType(RegistryMovementType registryMovementType)
        {
            try
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
            catch (SQLiteException err)
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
            catch (SQLiteException err)
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
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
        #endregion

        #region TIPO GESTIONE UTILI
        public RegistryTipoGestioniUtiliList GetTipoGestioniUtiliList()
        {
            DataTable dataTable= new DataTable();
            RegistryTipoGestioniUtiliList registryTipoGestioniUtilis = new RegistryTipoGestioniUtiliList();
            try
            {
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dataAdapter.SelectCommand.CommandText = RegistryScripts.GetTipoGestioniUtiliList;
                    dataAdapter.Fill(dataTable);
                }
                foreach(DataRow row in dataTable.Rows)
                {
                    RegistryTipoGestioniUtili registry = new RegistryTipoGestioniUtili();
                    registry.Id_tipo_gestione = Convert.ToInt32(row.Field<long>("id_tipo_gestione"));
                    registry.Tipo_Gestione = row.Field<string>("tipo_gestione");
                    registryTipoGestioniUtilis.Add(registry);
                }
                return registryTipoGestioniUtilis;
            }
            catch(SQLiteException err)
            {
                throw new Exception("Get registry tipo gestione utili " + err.Message);
            }
            catch(Exception err)
            {
                throw new Exception("Get registry tipo gestione utili " + err.Message);
            }
        }
        public void UpdateTipoGestioniUtili(RegistryTipoGestioniUtili tipoGestioniUtili)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    cmd.CommandText = RegistryScripts.UpdateTipoGestioniUtili;
                    cmd.Parameters.AddWithValue("tipo_gestione", tipoGestioniUtili.Tipo_Gestione);
                    cmd.Parameters.AddWithValue("id_tipo_gestione", tipoGestioniUtili.Id_tipo_gestione);
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception("Update registry tipo gestione utili " + err.Message);
            }
            catch (Exception err)
            {
                throw new Exception("Update registry tipo gestione utili " + err.Message);
            }
        }
        public void InsertTipoGestioniUtili(RegistryTipoGestioniUtili tipoGestioniUtili)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    cmd.CommandText = RegistryScripts.InsertTipoGestioniUtili;
                    cmd.Parameters.AddWithValue("tipo_gestione", tipoGestioniUtili.Tipo_Gestione);
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception("Insert registry tipo gestione utili " + err.Message);
            }
            catch (Exception err)
            {
                throw new Exception("Insert registry tipo gestione utili " + err.Message);
            }
        }
        public void DeleteTipoGestioniUtili(int id_tipoGestioniUtili)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    cmd.CommandText = RegistryScripts.DeleteTipoGestioniUtili;
                    cmd.Parameters.AddWithValue("id_tipo_gestione", id_tipoGestioniUtili);
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }

            }
            catch (SQLiteException err)
            {
                throw new Exception("Delete registry tipo gestione utili " + err.Message);
            }
            catch (Exception err)
            {
                throw new Exception("Delete registry tipo gestione utili " + err.Message);
            }
        }

        #endregion

        public TipoSoldiList GetTipoSoldiList()
        {
            DataTable dt = new DataTable();
            try
            {
                    using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                    {
                        dataAdapter.SelectCommand = new SQLiteCommand();
                        dataAdapter.SelectCommand.CommandText = RegistryScripts.GetTipoSoldiList;
                        dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dataAdapter.Fill(dt);
                    }
            }
            catch (SQLiteException err)
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
                TipoSoldi TS = new TipoSoldi((Models.Enumeratori.TipologiaSoldi)Convert.ToInt32(dr.Field<long>("id_tipo_soldi")));
                TSL.Add(TS);
            }
            return TSL;
        }

    }
}

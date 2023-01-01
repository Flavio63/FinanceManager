using FinanceManager.Models;
using FinanceManager.Services.SQL;
using System;
using System.Data;
using System.Data.SQLite;

namespace FinanceManager.Services
{
    public class ContoTitoliServices : IContoTitoliServices
    {
        IDAFconnection DAFconnection;
        public ContoTitoliServices(IDAFconnection iDAFconnection)
        {
            DAFconnection = iDAFconnection ?? throw new ArgumentNullException("Manca la stringa di connessione al db");
        }

        /// <summary>
        /// Aggiunge un movimento
        /// </summary>
        /// <param name="RecordTitolo">Il movimento da aggiungere</param>
        public void AddMovimentoTitoli(PortafoglioTitoli RecordTitolo)
        {
            using (SQLiteConnection con = new SQLiteConnection(DAFconnection.GetConnectionType()))
            {
                con.Open();
                using (SQLiteTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(con))
                        {
                            cmd.CommandText = ContoTitoliScript.AddMovimentoTitoli;
                            cmd.Parameters.AddWithValue("id_gestione", RecordTitolo.Id_gestione);
                            cmd.Parameters.AddWithValue("id_conto", RecordTitolo.Id_Conto);
                            cmd.Parameters.AddWithValue("id_valuta", RecordTitolo.Id_valuta);
                            cmd.Parameters.AddWithValue("id_tipo_movimento", RecordTitolo.Id_tipo_movimento);
                            cmd.Parameters.AddWithValue("id_titolo", RecordTitolo.Id_titolo);
                            cmd.Parameters.AddWithValue("data_movimento", RecordTitolo.Data_Movimento.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("ammontare", RecordTitolo.Importo_totale);
                            cmd.Parameters.AddWithValue("shares_quantity", RecordTitolo.N_titoli);
                            cmd.Parameters.AddWithValue("unity_local_value", RecordTitolo.Costo_unitario_in_valuta);
                            cmd.Parameters.AddWithValue("total_commission", RecordTitolo.Commissioni_totale);
                            cmd.Parameters.AddWithValue("tobin_tax", RecordTitolo.TobinTax);
                            cmd.Parameters.AddWithValue("disaggio_cedole", RecordTitolo.Disaggio_anticipo_cedole);
                            cmd.Parameters.AddWithValue("ritenuta_fiscale", RecordTitolo.RitenutaFiscale);
                            cmd.Parameters.AddWithValue("valore_cambio", RecordTitolo.Valore_di_cambio);
                            cmd.Parameters.AddWithValue("note", RecordTitolo.Note);
                            cmd.Parameters.AddWithValue("attivo", RecordTitolo.Attivo);
                            cmd.Parameters.AddWithValue("link_movimenti", RecordTitolo.Link_Movimenti.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("id_tipo_gestione", RecordTitolo.Id_Tipo_Gestione);
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                    catch (SQLiteException err)
                    {
                        transaction.Rollback();
                        throw new Exception(err.Message);
                    }
                    catch (Exception err)
                    {
                        transaction.Rollback();
                        throw new Exception(err.Message);
                    }
                }
            }
        }
        /// <summary>
        /// Ottiente la lista di tutti i titoli posseduti nel tempo
        /// (sia attivi che non )
        /// </summary>
        /// <param name="idOwner">Chi li possiede</param>
        /// <param name="idLocation">Dove sono</param>
        /// <returns>Lista di titoli</returns>
        public PortafoglioTitoliList GetListTitoliByOwnerAndLocation(int idOwner, int idLocation)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    if (idOwner == 0 && idLocation == 0)
                        dbAdapter.SelectCommand.CommandText = ContoTitoliScript.GetListaTitoliListTotal;
                    else if (idOwner > 0 && idLocation == 0)
                        dbAdapter.SelectCommand.CommandText = ContoTitoliScript.GetListaTitoliListByOwner;
                    else if (idOwner == 0 && idLocation > 0)
                        dbAdapter.SelectCommand.CommandText = ContoTitoliScript.GetListaTitoliListByLocation;
                    else
                        dbAdapter.SelectCommand.CommandText = ContoTitoliScript.GetListaTitoliByOwnerAndLocation;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", idOwner);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_conto", idLocation);
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
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
            return MLAL(dt);
        }
        /// <summary>
        /// Preleva il record di portafoglio titoli
        /// identifica dal suo id di riga
        /// </summary>
        /// <param name="IdPortafoglioTitoli">Identificativo di riga</param>
        /// <returns>Il record con tutti i campi</returns>
        public PortafoglioTitoli GetPortafoglioTitoliById(int IdPortafoglioTitoli)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = ContoTitoliScript.GetListaTitoliById;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_portafoglio_titoli", IdPortafoglioTitoli);
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(dt);
                }
                return MLA(dt.Rows[0]);
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
        /// Estrae tutti i record legati a un titolo
        /// </summary>
        /// <param name="link_movimenti">il link di tutti i movimenti di un titolo</param>
        /// <returns>Lista di record</returns>
        public PortafoglioTitoliList GetListaTitoliByLinkMovimenti(DateTime link_movimenti)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = ContoTitoliScript.GetListaTitoliListByLinkMovimenti;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("link_movimenti", link_movimenti);
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
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
            return MLAL(dt);
        }
        /// <summary>
        /// Ritorna l'ultimo movimento titoli in base ai
        /// parametri inseriti
        /// </summary>
        /// <param name="IdOwner">La gestione del titolo</param>
        /// <param name="IdLocation">Il conto a cui è legato</param>
        /// <returns>Il record in questione</returns>
        public PortafoglioTitoli GetLastShareMovementByOwnerAndLocation(int IdOwner, int IdLocation)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = ContoTitoliScript.GetLastMovimentoTitoliByOwnerAndLocation;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", IdOwner);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_conto", IdLocation);
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
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
            return MLA(dt.Rows[0]);
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
            DataTable dataTable = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = ContoTitoliScript.GetSharesQuantity;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", IdOwner);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_conto", IdLocation);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_titolo", id_titolo);
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(dataTable);
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
            return ((DataRow)dataTable.Rows[0]).Field<double?>("TotalShares") == null ? 0 : (double)((DataRow)dataTable.Rows[0]).Field<double?>("TotalShares");
        }
        /// <summary>
        /// Aggiorna i campi di un movimento titoli
        /// </summary>
        /// <param name="RecordPortafoglioTitoli">Il record da modificare</param>
        public void UpdateMovimentoTitoli(PortafoglioTitoli RecordPortafoglioTitoli)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = ContoTitoliScript.UpdateMovimentoTitoli;
                    dbComm.Parameters.AddWithValue("id_gestione", RecordPortafoglioTitoli.Id_gestione);
                    dbComm.Parameters.AddWithValue("id_conto", RecordPortafoglioTitoli.Id_Conto);
                    dbComm.Parameters.AddWithValue("id_valuta", RecordPortafoglioTitoli.Id_valuta);
                    dbComm.Parameters.AddWithValue("id_tipo_movimento", RecordPortafoglioTitoli.Id_tipo_movimento);
                    dbComm.Parameters.AddWithValue("id_titolo", RecordPortafoglioTitoli.Id_titolo);
                    dbComm.Parameters.AddWithValue("data_movimento", RecordPortafoglioTitoli.Data_Movimento.ToString("yyyy-MM-dd"));
                    dbComm.Parameters.AddWithValue("ammontare", RecordPortafoglioTitoli.Importo_totale);
                    dbComm.Parameters.AddWithValue("shares_quantity", RecordPortafoglioTitoli.N_titoli);
                    dbComm.Parameters.AddWithValue("unity_local_value", RecordPortafoglioTitoli.Costo_unitario_in_valuta);
                    dbComm.Parameters.AddWithValue("total_commission", RecordPortafoglioTitoli.Commissioni_totale);
                    dbComm.Parameters.AddWithValue("tobin_tax", RecordPortafoglioTitoli.TobinTax);
                    dbComm.Parameters.AddWithValue("disaggio_cedole", RecordPortafoglioTitoli.Disaggio_anticipo_cedole);
                    dbComm.Parameters.AddWithValue("ritenuta_fiscale", RecordPortafoglioTitoli.RitenutaFiscale);
                    dbComm.Parameters.AddWithValue("valore_cambio", RecordPortafoglioTitoli.Valore_di_cambio);
                    dbComm.Parameters.AddWithValue("disponibile", RecordPortafoglioTitoli.Available);
                    dbComm.Parameters.AddWithValue("note", RecordPortafoglioTitoli.Note);
                    dbComm.Parameters.AddWithValue("attivo", RecordPortafoglioTitoli.Attivo);
                    dbComm.Parameters.AddWithValue("id_portafoglio_titoli", RecordPortafoglioTitoli.Id_portafoglio);
                    dbComm.Parameters.AddWithValue("link_movimenti", RecordPortafoglioTitoli.Link_Movimenti.ToString("yyyy-MM-dd HH:mm:ss"));
                    dbComm.Parameters.AddWithValue("id_tipo_gestione", RecordPortafoglioTitoli.Id_Tipo_Gestione);
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
        /// <summary>
        /// Restituisce i titoli attivi in portafoglio sulla base dei parametri dati
        /// </summary>
        /// <param name="id_gestione">La gestione</param>
        /// <param name="id_conto">Il conto</param>
        /// <param name="id_titolo">Il titolo</param>
        /// <returns>Lista di titoli attivi</returns>
        public Ptf_CCList GetListaTitoliAttiviByContoGestioneTitolo(int id_gestione, int id_conto, int id_titolo)
        {
            try
            {
                DataTable DT = new DataTable();
                Ptf_CCList _CCs = new Ptf_CCList();
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = ContoTitoliScript.GetListaTitoliAttiviByContoGestioneTitolo;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", id_gestione);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_conto", id_conto);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_titolo", id_titolo);
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
                    foreach (DataRow row in DT.Rows)
                    {
                        Ptf_CC ptf_CC = new Ptf_CC();
                        ptf_CC.Id_portafoglio_titoli = (int)row.Field<long>("id_portafoglio_titoli");
                        ptf_CC.Id_gestione = (int)row.Field<long>("id_gestione");
                        ptf_CC.Id_Conto = (int)row.Field<long>("id_conto");
                        ptf_CC.Id_valuta = (int)row.Field<long>("id_valuta");
                        ptf_CC.Id_tipo_movimento = (int)row.Field<long>("id_tipo_movimento");
                        ptf_CC.Id_titolo = (int)row.Field<long>("id_titolo");
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
                        ptf_CC.Id_RowConto = row.Field<object>("id_fineco_euro") == null ? 0 : (int)row.Field<long>("id_fineco_euro");
                        ptf_CC.Valore_in_CC = row.Field<object>("id_fineco_euro") == null ? 0 : row.Field<double>("Valore_in_CC");
                        ptf_CC.Id_Tipo_Soldi = row.Field<object>("id_fineco_euro") == null ? 0 : (int)row.Field<long>("id_tipo_soldi");
                        _CCs.Add(ptf_CC);
                    }
                }
                return _CCs;

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
        /// Elimina un movimento nel conto titoli
        /// </summary>
        /// <param name="id">Identificativo di riga</param>
        public void DeleteManagerLiquidAsset(int id)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandText = ContoTitoliScript.DeleteManagerLiquidAsset;
                    dbComm.Parameters.AddWithValue("id_portafoglio_titoli", id);
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
            MLA.Id_portafoglio = (int)dr.Field<long>("id_portafoglio_titoli");
            MLA.Id_gestione = (int)dr.Field<long>("id_gestione");
            MLA.Nome_Gestione = dr.Field<string>("nome_gestione");
            MLA.Id_Conto = (int)dr.Field<long>("id_conto");
            MLA.Desc_Conto = dr.Field<string>("desc_conto");
            MLA.Id_valuta = (int)dr.Field<long>("id_valuta");
            MLA.Cod_valuta = dr.Field<string>("cod_valuta");
            MLA.Id_tipo_movimento = (int)dr.Field<long>("id_tipo_movimento");
            MLA.Desc_tipo_movimento = dr.Field<string>("desc_Movimento");
            MLA.Desc_azienda = dr.Field<string>("desc_azienda");
            MLA.Id_titolo = (uint)dr.Field<long>("id_titolo");
            MLA.Desc_titolo = dr.Field<string>("desc_titolo");
            MLA.Isin = dr.Field<string>("isin");
            MLA.Id_tipo_titolo = (uint)dr.Field<long>("id_tipo_titolo");
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
            MLA.Attivo = (int)dr.Field<long>("attivo");
            MLA.Link_Movimenti = dr.Field<DateTime>("link_movimenti");
            MLA.Id_Tipo_Gestione = (int)dr.Field<long>("id_tipo_gestione");
            MLA.Tipo_Gestione = dr.Field<string>("Tipo_Gestione");
            return MLA;
        }

    }
}

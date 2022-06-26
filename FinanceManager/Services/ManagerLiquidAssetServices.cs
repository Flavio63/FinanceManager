using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using FinanceManager.Models;
using FinanceManager.Models.Enumeratori;
using FinanceManager.Services.SQL;

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
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.CommandText = ManagerScripts.AddManagerLiquidAsset;
                    cmd.Parameters.AddWithValue("id_gestione", managerLiquidAsset.Id_gestione);
                    cmd.Parameters.AddWithValue("id_conto", managerLiquidAsset.Id_Conto);
                    cmd.Parameters.AddWithValue("id_valuta", managerLiquidAsset.Id_valuta);
                    cmd.Parameters.AddWithValue("id_tipo_movimento", managerLiquidAsset.Id_tipo_movimento);
                    cmd.Parameters.AddWithValue("id_titolo", managerLiquidAsset.Id_titolo);
                    cmd.Parameters.AddWithValue("data_movimento", managerLiquidAsset.Data_Movimento.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("ammontare", managerLiquidAsset.Importo_totale);
                    cmd.Parameters.AddWithValue("shares_quantity", managerLiquidAsset.N_titoli);
                    cmd.Parameters.AddWithValue("unity_local_value", managerLiquidAsset.Costo_unitario_in_valuta);
                    cmd.Parameters.AddWithValue("total_commission", managerLiquidAsset.Commissioni_totale);
                    cmd.Parameters.AddWithValue("tobin_tax", managerLiquidAsset.TobinTax);
                    cmd.Parameters.AddWithValue("disaggio_cedole", managerLiquidAsset.Disaggio_anticipo_cedole);
                    cmd.Parameters.AddWithValue("ritenuta_fiscale", managerLiquidAsset.RitenutaFiscale);
                    cmd.Parameters.AddWithValue("valore_cambio", managerLiquidAsset.Valore_di_cambio);
                    cmd.Parameters.AddWithValue("note", managerLiquidAsset.Note);
                    cmd.Parameters.AddWithValue("attivo", managerLiquidAsset.Attivo);
                    cmd.Parameters.AddWithValue("link_movimenti", managerLiquidAsset.Link_Movimenti.ToString("yyyy-MM-dd HH:mm:ss"));
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
        /// Elimina un movimento
        /// </summary>
        /// <param name="id">Identificativo del movimento da eliminare</param>
        public void DeleteManagerLiquidAsset(int id)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandText = ManagerScripts.DeleteManagerLiquidAsset;
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

        /// <summary>
        /// Estrae tutti i valori di cedole, utili e disponibilità 
        /// sulla base della gestione richiesta
        /// </summary>
        /// <param name="IdGestione">La gestione</param>
        /// <returns>Observable collection </returns>
        public SintesiSoldiList GetCurrencyAvailable(int IdGestione, int IdConto, int IdValuta)
        {
            DataTable DT = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    if (IdGestione > 0 && IdConto > 0 && IdValuta > 0)
                        dbAdapter.SelectCommand.CommandText = ManagerScripts.GetCurrencyAvByOwnerContoValuta;
                    else if (IdGestione > 0 && IdConto == 0 && IdValuta == 0)
                        dbAdapter.SelectCommand.CommandText = ManagerScripts.GetCurrencyAvByOwner;
                    else
                        throw new Exception("Richiesta di liquidità non ancora disponibile.");
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", IdGestione);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_conto", IdConto);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_valuta", IdValuta);
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
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

        /// <summary>
        /// Data una gestione ne estrae tutti i movimenti
        /// </summary>
        /// <param name="idOwner">La gestione</param>
        /// <returns>Lista dei movimenti</returns>
        public PortafoglioTitoliList GetManagerLiquidAssetListByOwnerAndLocation(int idOwner, int idLocation)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
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
        /// Estrae tutti i record legati di un titolo
        /// </summary>
        /// <param name="link_movimenti">il link di tutti i movimenti di un titolo</param>
        /// <returns></returns>
        public PortafoglioTitoliList GetManagerLiquidAssetListByLinkMovimenti(DateTime link_movimenti)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = ManagerScripts.GetManagerLiquidAssetListByLinkMovimenti;
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

        public PortafoglioTitoli GetLastShareMovementByOwnerAndLocation(int IdOwner, int IdLocation)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = ManagerScripts.GetLastSharesMovementByOwnerAndLocation;
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
                    dbAdapter.SelectCommand.CommandText = ManagerScripts.GetSharesQuantity;
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
        /// Aggiorna i campi di un movimento
        /// </summary>
        /// <param name="managerLiquidAsset">Il record da modificare</param>
        public void UpdateManagerLiquidAsset(PortafoglioTitoli managerLiquidAsset)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    dbComm.CommandText = ManagerScripts.UpdateManagerLiquidAsset;
                    dbComm.Parameters.AddWithValue("id_gestione", managerLiquidAsset.Id_gestione);
                    dbComm.Parameters.AddWithValue("id_conto", managerLiquidAsset.Id_Conto);
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
            conto.Id_RowConto = (int)dataRow.Field<long>("id_fineco_euro");
            conto.Id_Conto = (int)dataRow.Field<long>("id_conto");
            conto.Desc_Conto = dataRow.Field<string>("desc_conto");
            conto.Id_Quote_Investimenti = (int)dataRow.Field<long>("id_quote_investimenti");
            conto.Id_Valuta = (int)dataRow.Field<long>("id_valuta");
            conto.Cod_Valuta = dataRow.Field<string>("cod_valuta");
            conto.Id_Portafoglio_Titoli = (int)dataRow.Field<long>("id_portafoglio_titoli");
            conto.Id_tipo_movimento = (int)dataRow.Field<long>("id_tipo_movimento");
            conto.Desc_tipo_movimento = dataRow.Field<string>("desc_movimento");
            conto.Id_Gestione = (int)dataRow.Field<long>("id_gestione");
            conto.NomeGestione = dataRow.Field<string>("nome_gestione");
            conto.Id_Titolo = (int)dataRow.Field<long>("id_titolo");
            conto.ISIN = dataRow.Field<string>("isin");
            conto.Desc_Titolo = dataRow.Field<string>("desc_titolo");
            conto.DataMovimento = dataRow.Field<DateTime>("data_movimento");
            conto.Ammontare = dataRow.Field<double>("ammontare");
            conto.Valore_Cambio = dataRow.Field<double>("cambio");
            conto.Causale = dataRow.Field<string>("causale");
            conto.Id_Tipo_Soldi = (int)dataRow.Field<long>("id_tipo_soldi");
            conto.Desc_Tipo_Soldi = dataRow.Field<string>("desc_tipo_soldi");
            conto.Modified = dataRow.Field<DateTime>("modified");
            return conto;
        }

        #endregion ContoCorrente
        public void InsertAccountMovement(ContoCorrente contoCorrente)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandText = ManagerScripts.InsertAccountMovement;
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
                    dbComm.Parameters.AddWithValue("id_quote_periodi", contoCorrente.Id_Quote_Periodi);
                    dbComm.Parameters.AddWithValue("modified", contoCorrente.Modified.ToString("yyyy-MM-dd HH:mm:ss"));
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
        /// Tramite l'ultimo record conto_corrente inserito
        /// calcolo e inserisco le quote guadagno per ogni singolo socio
        /// </summary>
        /// <param name="RecordContoCorrente">record conto corrente con i dati</param>
        public void AddSingoloGuadagno(ContoCorrente RecordContoCorrente)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandText = ManagerScripts.AddSingoloGuadagno;
                    dbComm.Parameters.AddWithValue("id_tipo_movimento", RecordContoCorrente.Id_tipo_movimento);
                    dbComm.Parameters.AddWithValue("id_tipo_soldi", RecordContoCorrente.Id_Tipo_Soldi);
                    dbComm.Parameters.AddWithValue("id_quote_periodi", RecordContoCorrente.Id_Quote_Periodi);
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
        /// Tramite l'ultimo record conto_corrente inserito
        /// calcolo e inserisco le quote guadagno per ogni singolo socio
        /// </summary>
        /// <param name="RecordContoCorrente">record conto corrente con i dati</param>
        public void ModifySingoloGuadagno(ContoCorrente RecordContoCorrente)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandText = ManagerScripts.ModifySingoloGuadagno;
                    dbComm.Parameters.AddWithValue("id_fineco_euro", RecordContoCorrente.Id_RowConto);
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
        /// Calcola la quota ultima base investimento attivo
        /// restituendo il totale immesso, prelevato, assegnato e disponibile
        /// </summary>
        /// <returns>Una lista con le quote per investitore</returns>
        public QuoteInvList GetQuoteInv()
        {
            DataTable DT = new DataTable();
            try
            {
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dataAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetQuoteInv;
                    dataAdapter.Fill(DT);
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

            QuoteInvList quotes = new QuoteInvList();
            double versato = 0;
            double prelevato = 0;
            double disinvestito = 0;
            double investito = 0;
            double disponibile = 0;
            double patrimonio = 0;
            QuoteInv quoteInv = new QuoteInv();
            foreach (DataRow dataRow in DT.Rows)
            {
                if (dataRow.Field<string>("nome_gestione") != "Aury")
                {
                    versato = versato + dataRow.Field<double>("Versato");
                    prelevato = prelevato + dataRow.Field<double>("Prelevato");
                    disinvestito = disinvestito + dataRow.Field<double>("Disinvestito");
                    investito = investito + dataRow.Field<double>("Investito");
                    disponibile = disponibile + dataRow.Field<double>("Disponibile");
                    patrimonio = patrimonio + dataRow.Field<double>("Disponibile") + dataRow.Field<double>("Investito") * -1 - dataRow.Field<double>("Disinvestito");
                }
            }
            quoteInv.TotaleVersato = versato;
            quoteInv.TotalePrelevato = prelevato;
            quoteInv.TotaleDisinvestito = disinvestito;
            quoteInv.TotaleInvestito = investito;
            quoteInv.TotaleDisponibile = disponibile;
            quoteInv.TotalePatrimonio = patrimonio;

            foreach (DataRow dataRow in DT.Rows)
            {
                QuoteInv quote = new QuoteInv();
                quote.NomeInvestitore = dataRow.Field<string>("nome_gestione");
                quote.CapitaleVersato = dataRow.Field<double>("Versato");
                quote.QuotaVersato = quote.NomeInvestitore != "Aury" ? dataRow.Field<double>("Versato") / versato : 0;
                quote.CapitalePrelevato = dataRow.Field<double>("Prelevato");
                quote.QuotaPrelevato = quote.NomeInvestitore != "Aury" ? dataRow.Field<double>("Prelevato") / prelevato : 0;
                quote.CapitaleDisinvestito = dataRow.Field<double>("Disinvestito");
                quote.QuotaDisinvestito = quote.NomeInvestitore != "Aury" ? dataRow.Field<double>("disinvestito") / disinvestito : 0;
                quote.CapitaleInvestito = dataRow.Field<double>("Investito");
                quote.QuotaInvestito = quote.NomeInvestitore != "Aury" ? dataRow.Field<double>("Investito") / investito : 0;
                quote.CapitaleDisponibile = dataRow.Field<double>("Disponibile");
                quote.QuotaDisponibile = quote.NomeInvestitore != "Aury" ? dataRow.Field<double>("Disponibile") / disponibile : 0;
                quote.Patrimonio = dataRow.Field<double>("Disponibile") + dataRow.Field<double>("Investito") * -1 - dataRow.Field<double>("Disinvestito");
                quote.QuotaPatrimonio = quote.NomeInvestitore != "Aury" ?
                    (dataRow.Field<double>("Disponibile") + dataRow.Field<double>("Investito") * -1 - dataRow.Field<double>("Disinvestito")) / patrimonio :
                    0;
                quotes.Add(quote);
            }
            quotes.Add(quoteInv);
            return quotes;

        }

        /// <summary>
        /// Prelevo tutti i record della tabella quote_guadagno
        /// </summary>
        public QuotePerPeriodoList GetAllRecordQuote_Guadagno()
        {
            DataTable DT = new DataTable();
            try
            {
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.CommandText = ManagerScripts.GetAllRecordQuote_Guadagno;
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dataAdapter.Fill(DT);
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
            QuotePerPeriodoList quotes = new QuotePerPeriodoList();
            foreach (DataRow dataRow in DT.Rows)
            {
                QuotePerPeriodo quote = new QuotePerPeriodo();
                quote.Id_Quota = (int)dataRow.Field<long>("id_quota");
                quote.Id_Gestione = (int)dataRow.Field<long>("id_gestione");
                quote.Nome_Gestione = dataRow.Field<string>("nome_gestione");
                quote.Id_Tipo_Soldi = (int)dataRow.Field<long>("id_aggregazione");
                quote.Desc_Tipo_Soldi = dataRow.Field<string>("desc_tipo_soldi");
                quote.Id_Quote_Periodi = (int)dataRow.Field<long>("id_quote_periodi");
                quote.Data_Inizio = dataRow.Field<DateTime>("data_inizio");
                quote.Data_Fine = dataRow.Field<DateTime>("data_fine");
                quote.Quota = dataRow.Field<double>("quota");
                quotes.Add(quote);
            }
            return quotes;
        }

        /// <summary>
        /// Calcolo le nuove quote e le inserisco nella tabella quote_guadagno
        /// </summary>
        /// <param name="Tipo_Soldi">Codice identificativo</param>
        /// <param name="NuovoPeriodo">Il nuovo periodo da inserire in tabella</param>
        public void ComputesAndInsertQuoteGuadagno(int Tipo_Soldi, int NuovoPeriodo)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.CommandText = UpdateQuotePeriodi.ComputesQuoteGuadagno;
                    cmd.Parameters.AddWithValue("Tipo_Soldi", Tipo_Soldi);
                    cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = UpdateQuotePeriodi.InsertQuotaGuadagno;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Nuovo_Periodo", NuovoPeriodo);
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
        /// Calcolo le nuove quote e modifico la tabella quote_guadagno
        /// </summary>
        /// <param name="Tipo_Soldi">Codice identificativo</param>
        public void ComputesAndModifyQuoteGuadagno(int Tipo_Soldi)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandText = UpdateQuotePeriodi.ComputesQuoteGuadagno;
                    dbComm.Parameters.AddWithValue("Tipo_Soldi", Tipo_Soldi);
                    dbComm.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbComm.Connection.Open();
                    dbComm.ExecuteNonQuery();
                    dbComm.CommandText = UpdateQuotePeriodi.UpdateQuotaGuadagno;
                    dbComm.Parameters.Clear();
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
        /// Aggiorno la tabella Guadagni_totale_anno con le nuove
        /// quote per il periodo interessato alle modifiche
        /// </summary>
        /// <param name="Id_Periodo_Quote">il periodo da modificare</param>
        /// <param name="Id_Tipo_Soldi">Il tipo soldi</param>
        public void UpdateGuadagniTotaleAnno(int Id_Periodo_Quote, int Id_Tipo_Soldi)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandText = UpdateQuotePeriodi.UpdateGuadagniTotaleAnno;
                    dbComm.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbComm.Parameters.AddWithValue("IdPeriodoQuote", Id_Periodo_Quote);
                    dbComm.Parameters.AddWithValue("IdTipoSoldi", Id_Tipo_Soldi);
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
        /// Aggiorno la tabella Guadagni_totale_anno nel caso di
        /// modifiche del record di prelievo utili
        /// </summary>
        /// <param name="RecordQuoteGuadagno">il record da modificare</param>
        public void UpdateGuadagniTotaleAnno(GuadagnoPerQuote RecordQuoteGuadagno)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandText = ManagerScripts.UpdatePrelievoUtili;
                    dbComm.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbComm.Parameters.AddWithValue("id_gestione", RecordQuoteGuadagno.IdGestione);
                    dbComm.Parameters.AddWithValue("anno", RecordQuoteGuadagno.Anno);
                    dbComm.Parameters.AddWithValue("prelevato", RecordQuoteGuadagno.Preso);
                    dbComm.Parameters.AddWithValue("data", RecordQuoteGuadagno.DataOperazione);
                    dbComm.Parameters.AddWithValue("causale", RecordQuoteGuadagno.Causale);
                    dbComm.Parameters.AddWithValue("id_guadagno", RecordQuoteGuadagno.IdGuadagno);
                    dbComm.Connection.Open();
                    dbComm.ExecuteNonQuery();
                    dbComm.CommandText = ManagerScripts.UpdatePrelievoUtiliBkd;
                    dbComm.Parameters.RemoveAt("id_guadagno");
                    dbComm.Parameters.AddWithValue("id_prelievo", RecordQuoteGuadagno.IdGuadagno);
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
        /// Elimino un record dalla tabella quote_guadagno
        /// </summary>
        /// <param name="id_quota">identificativo del record base conto corrente</param>
        public void DeleteRecordGuadagno_Totale_anno(int id_quota)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandText = ManagerScripts.DeleteRecordGuadagno_Totale_anno;
                    dbComm.Parameters.AddWithValue("id_conto_corrente", id_quota);
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

        /// <summary>Estraggo gli anni dalla tabella guadagni_totale_anno</summary>
        public List<int> GetAnniFromGuadagni()
        {
            DataTable DT = new DataTable();
            try
            {
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.CommandText = ManagerScripts.GetAnniFromGuadagni;
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dataAdapter.Fill(DT);
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
            List<int> anni = new List<int>();
            foreach (DataRow row in DT.Rows)
                anni.Add(row.Field<int>("anno"));
            return anni;
        }

        /// <summary>
        /// Trovo il codice dei record da ricalcolare con le nuove quote
        /// </summary>
        /// <param name="dateTime">la data dell'investimento</param>
        /// <param name="Id_tipoSoldi">Identifica chi sta modificando l'investimento</param>
        /// <returns>int</returns>
        public int GetIdPeriodoQuote(DateTime dateTime, int Id_tipoSoldi)
        {
            DataTable DT = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = ManagerScripts.GetIdPeriodoQuote;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("data_movimento", dateTime.ToString("yyyy-MM-dd"));
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_tipo_soldi", Id_tipoSoldi);
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
                    return DT.Rows.Count == 0 ? 0 : (int)DT.Rows[0].Field<long>("id_periodo_quote");
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception("GetIdPeriodoQuote " + err.Message);
            }
        }
        /// <summary>
        /// Modifico la tabella quote_periodi modificando la data di fine
        /// e inserendo il nuovo record
        /// </summary>
        /// <param name="DataDal">Data da cercare</param>
        /// <param name="TipoSoldi">Tipologia dei soldi</param>
        /// <returns>Last id record inserted</returns>
        public int Update_InsertQuotePeriodi(DateTime DataDal, int TipoSoldi)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.CommandText = UpdateQuotePeriodi.quote_periodi;
                    cmd.Parameters.AddWithValue("StartDate", DataDal.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("TipoSoldi", TipoSoldi);
                    cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = UpdateQuotePeriodi.ultima_riga;
                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);
                    return Convert.ToInt32(dt.Rows[0].Field<object>("ultima_riga"));

                }
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
            catch (Exception err)
            {
                throw new Exception("Update_InsertQuotePeriodi " + err.Message);
            }
        }

        /// <summary>
        /// Calcola le quote di guadagno per investitore applicando
        /// le quote di investimento per periodo.
        /// </summary>
        /// <param name="tipoReport">se 0 genera estrama sintesi, se 1 sintesi, se 2 il dettaglio</param>
        /// <returns>Una lista con i dati per investitore</returns>
        public GuadagnoPerQuoteList GetQuoteGuadagno(int tipoReport)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    switch (tipoReport)
                    {
                        case 2:
                            dataAdapter.SelectCommand.CommandText = SintesiGuadagniPerValute.dettagliato;
                            dataAdapter.Fill(dt);
                            break;
                        case 1:
                            dataAdapter.SelectCommand.CommandText = SintesiGuadagniPerValute.sintesi_tipologia;
                            dataAdapter.Fill(dt);
                            break;
                        case 0:
                            dataAdapter.SelectCommand.CommandText = SintesiGuadagniPerValute.sintesi;
                            dataAdapter.Fill(dt);
                            int idxVal;
                            for (int r = 0; r < dt.Rows.Count;)
                            {
                                Dictionary<int, double> keyValuePairs = new Dictionary<int, double>();
                                string name = dt.Rows[r].Field<string>("nome_gestione");
                                while (name == dt.Rows[r].Field<string>("nome_gestione"))
                                {
                                    idxVal = (int)dt.Rows[r].Field<long>("id_valuta");
                                    if (keyValuePairs.ContainsKey(idxVal))
                                    {
                                        keyValuePairs[idxVal] += dt.Rows[r].Field<double>("RisparmioAnno");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add(idxVal, dt.Rows[r].Field<double>("RisparmioAnno"));
                                    }
                                    DataRow dr = dt.Rows[r];
                                    dr["RisparmioCumulato"] = keyValuePairs[idxVal];
                                    r++;
                                    if (r == dt.Rows.Count)
                                        break;
                                }
                            }
                            dt.DefaultView.Sort = "anno DESC";
                            dt = dt.DefaultView.ToTable();
                            break;
                    }
                    GuadagnoPerQuoteList quotes = new GuadagnoPerQuoteList();
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        GuadagnoPerQuote quote = new GuadagnoPerQuote();
                        quote.Anno = dataRow.Field<int>("anno");
                        quote.Nome = dataRow.Field<string>("nome_gestione");
                        quote.IdCurrency = (int)dataRow.Field<long>("id_valuta");
                        quote.CodeCurrency = dataRow.Field<string>("cod_valuta");
                        if (tipoReport == 1 || tipoReport == 2)
                        {
                            quote.DescTipoSoldi = dataRow.Field<string>("desc_tipo_soldi");
                            if (tipoReport == 2)
                            {
                                quote.IdGuadagno = (int)dataRow.Field<long>("id_guadagno");
                                quote.IdGestione = (int)dataRow.Field<long>("id_gestione");
                                quote.IdTipoMovimento = (int)dataRow.Field<long>("id_tipo_movimento");
                                quote.DataOperazione = dataRow.Field<DateTime>("data_operazione");
                                quote.QuotaInv = dataRow.Field<double>("quota");
                                quote.Causale = dataRow.Field<string>("causale");
                            }
                        }
                        else
                        {
                            quote.RisparmioCumulato = dataRow.Field<double>("RisparmioCumulato");
                            quote.RisparmioAnno = dataRow.Field<double>("RisparmioAnno");
                        }
                        quote.Guadagno = dataRow.Field<double>("GuadagnoAnno1");
                        quote.Preso = dataRow.Field<double>("Preso");
                        quotes.Add(quote);
                    }
                    return quotes;
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
        /// Richiede una lista dei movimenti per data degli investimenti
        /// </summary>
        /// <returns>Una lista con i movimenti per data degli investimenti</returns>
        public QuoteTabList GetQuoteTab()
        {
            DataTable DT = new DataTable();
            try
            {
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.CommandText = SQL.ManagerScripts.GetQuoteTab;
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dataAdapter.Fill(DT);
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
            QuoteTabList quotes = new QuoteTabList();
            foreach (DataRow dataRow in DT.Rows)
            {
                QuoteTab quote = new QuoteTab();
                quote.IdQuote = (int)dataRow.Field<long>("id_quote_inv");
                quote.IdGestione = (int)dataRow.Field<long>("id_gestione");
                quote.NomeInvestitore = dataRow.Field<string>("nome_gestione");
                quote.Id_tipo_movimento = (int)dataRow.Field<long>("id_tipo_movimento");
                quote.Desc_tipo_movimento = dataRow.Field<string>("desc_movimento");
                quote.DataMovimento = dataRow.Field<DateTime>("data_movimento");
                quote.AmmontareEuro = dataRow.Field<double>("ammontare");
                quote.IdCurrency = (int)dataRow.Field<long>("id_valuta");
                quote.CodeCurrency = dataRow.Field<string>("cod_valuta");
                quote.AmmontareValuta = dataRow.Field<double>("valuta_base");
                quote.ChangeValue = dataRow.Field<double>("valore_cambio");
                quote.Note = dataRow.Field<string>("note");
                quotes.Add(quote);
            }
            return quotes;
        }

        /// <summary>
        /// Aggiorna la tabella con i movimenti degli investitori
        /// </summary>
        /// <param name="ActualQuote">I dati del movimento da modificare</param>
        public void UpdateQuoteTab(QuoteTab ActualQuote)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandText = ManagerScripts.UpdateQuoteTab;
                    dbComm.Parameters.AddWithValue("id_quote_inv", ActualQuote.IdQuote);
                    dbComm.Parameters.AddWithValue("id_gestione", ActualQuote.IdGestione);
                    dbComm.Parameters.AddWithValue("id_tipo_movimento", ActualQuote.Id_tipo_movimento);
                    dbComm.Parameters.AddWithValue("data_movimento", ActualQuote.DataMovimento.ToString("yyyy-MM-dd"));
                    dbComm.Parameters.AddWithValue("ammontare", ActualQuote.AmmontareEuro);
                    dbComm.Parameters.AddWithValue("id_valuta", ActualQuote.IdCurrency);
                    dbComm.Parameters.AddWithValue("valuta_base", ActualQuote.AmmontareValuta);
                    dbComm.Parameters.AddWithValue("valore_cambio", ActualQuote.ChangeValue);
                    dbComm.Parameters.AddWithValue("note", ActualQuote.Note);
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
        /// Verifico se nella data di inserimento è già presente
        /// un investimento
        /// </summary>
        /// <param name="ActualQuote">Il record per verificare</param>
        /// <param name="Id_Tipo_Soldi">Il tipo soldi che si sta movimentando</param>
        /// <returns>-1 se falso altrimenti il numero del periodo quote</returns>
        public int VerifyInvestmentDate(QuoteTab ActualQuote, int Id_Tipo_Soldi)
        {
            try
            {
                DataTable DT = new DataTable();
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.CommandText = SQL.ManagerScripts.VerifyInvestmentDate;
                    dataAdapter.SelectCommand.Parameters.AddWithValue("id_tipo_soldi", Id_Tipo_Soldi);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("data_inizio", ActualQuote.DataMovimento.ToString("yyyy-MM-dd"));
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dataAdapter.Fill(DT);
                    return Convert.ToInt16(DT.Rows[0].ItemArray[0].ToString());
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
        /// Trovo l'id del record da modificare
        /// </summary>
        /// <param name="ActualQuote">Il record con le modifiche</param>
        /// <returns>id_quoteTab</returns>
        public int GetIdQuoteTab(QuoteTab ActualQuote)
        {
            DataTable DT = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = ManagerScripts.GetIdQuoteTab;
                    if (ActualQuote.Id_Periodo_Quote == 0)
                        dbAdapter.SelectCommand.CommandText += "ORDER BY id_quote_inv DESC LIMIT 1";
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", ActualQuote.IdGestione);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_tipo_movimento", ActualQuote.Id_tipo_movimento);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_periodo_quote", ActualQuote.Id_Periodo_Quote);
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
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
            return Convert.ToInt32(DT.Rows[0].ItemArray[0]);
        }

        /// <summary>
        /// Inserisce un nuovo movimento all'interno della tabella QuoteInvestimenti
        /// </summary>
        /// <param name="ActualQuote">I dati del movimento da inserire</param>
        public void InsertInvestment(QuoteTab ActualQuote)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.CommandText = "PRAGMA FOREIGN_KEYS = false; " + ManagerScripts.InsertInvestment + " PRAGMA FOREIGN_KEYS = true;";
                    cmd.Parameters.AddWithValue("id_gestione", ActualQuote.IdGestione);
                    cmd.Parameters.AddWithValue("id_tipo_movimento", ActualQuote.Id_tipo_movimento);
                    cmd.Parameters.AddWithValue("id_periodo_quote", ActualQuote.Id_Periodo_Quote);
                    cmd.Parameters.AddWithValue("data_movimento", ActualQuote.DataMovimento.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("id_valuta", ActualQuote.IdCurrency);
                    cmd.Parameters.AddWithValue("valuta_base", ActualQuote.AmmontareValuta);
                    cmd.Parameters.AddWithValue("valore_cambio", ActualQuote.ChangeValue);
                    cmd.Parameters.AddWithValue("ammontare", ActualQuote.AmmontareEuro);
                    cmd.Parameters.AddWithValue("note", ActualQuote.Note);
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
        /// Estrae tutti i movimenti in ordine di data del conto corrente
        /// </summary>
        /// <returns>Lista con tutti i movimenti</returns>
        public ContoCorrenteList GetContoCorrenteList()
        {
            DataTable DT = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = ManagerScripts.GetContoCorrente;
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
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
            return contoCorrentes(DT);
        }

        /// <summary>
        /// Estrazione dei 2 record coinvolti nel giroconto interno o
        /// nel cambio valuta.
        /// </summary>
        /// <param name="modified">DateTime</param>
        /// <returns>List ContoCorrente</returns>
        public ContoCorrenteList Get2ContoCorrentes(DateTime modified)
        {
            DataTable DT = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = ManagerScripts.Get2ContoCorrentes;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("modified", modified.ToString("yyyy-MM-dd HH:mm:ss"));
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
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
            return contoCorrentes(DT);
        }

        /// <summary>
        /// Dato l'id del giroconto fra le 2 tabelle investimenti e cc
        /// estrae i dati dalla tabella conto_corrente
        /// </summary>
        /// <param name="idQuote">id_quote_inv</param>
        /// <returns>Record di tipo Conto Corrente</returns>
        public ContoCorrente GetContoCorrenteByIdQuote(int idQuote)
        {
            DataTable DT = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = ManagerScripts.GetContoCorrenteByIdQuote;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_quote_investimenti", idQuote);
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
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
            return contoCorrente(DT.Rows[0]);
        }

        /// <summary>
        /// Dato l'id del portafoglio titoli estrae i dati dalla tabella conto_corrente
        /// </summary>
        /// <param name="idPortafoglioTitoli">id_portafoglio_titoli</param>
        /// <returns>Record di tipo Conto Corrente</returns>
        public ContoCorrenteList GetContoCorrenteByIdPortafoglio(int idPortafoglioTitoli)
        {
            DataTable DT = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = ManagerScripts.GetContoCorrenteByIdPortafoglio;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_portafoglio_titoli", idPortafoglioTitoli);
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
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
            return contoCorrentes(DT);
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
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandText = ManagerScripts.DeleteAccount;
                    dbComm.Parameters.AddWithValue("id_fineco_euro", idCC);
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
        /// Elimina un record dalla tabella conto_corrente
        /// sulla base del id_portafoglio_titoli
        /// </summary>
        /// <param name="idContoTitoli">id-portafoglio_titoli</param>
        public void DeleteContoCorrenteByIdPortafoglioTitoli(int idContoTitoli)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandText = ManagerScripts.DeleteContoCorrenteByIdPortafoglioTitoli;
                    dbComm.Parameters.AddWithValue("id_portafoglio_titoli", idContoTitoli);
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

        public void UpdateRecordContoCorrente(ContoCorrente contoCorrente, TipologiaIDContoCorrente tipologiaID)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandType = CommandType.Text;
                    if (tipologiaID == TipologiaIDContoCorrente.IdContoCorrente)
                        dbComm.CommandText = ManagerScripts.UpdateContoCorrenteByIdCC;
                    else if (tipologiaID == TipologiaIDContoCorrente.IdContoTitoli)
                        dbComm.CommandText = ManagerScripts.UpdateContoCorrenteByIdPortafoglioTitoli;
                    else if (tipologiaID == TipologiaIDContoCorrente.IdQuoteInvestimenti)
                        dbComm.CommandText = ManagerScripts.UpdateContoCorrenteByIdQuote;
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
                    dbComm.Parameters.AddWithValue("id_quote_periodi", contoCorrente.Id_Quote_Periodi);
                    dbComm.Parameters.AddWithValue("modified", contoCorrente.Modified.ToString("yyyy-MM-dd HH:mm:ss"));
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

        public Ptf_CCList GetShareActiveAndAccountMovement(int id_gestione, int id_conto, int id_titolo)
        {
            try
            {
                DataTable DT = new DataTable();
                Ptf_CCList _CCs = new Ptf_CCList();
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = ManagerScripts.GetShareActiveAndAccountMovement;
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
                        ptf_CC.Id_RowConto = (int)row.Field<long>("id_fineco_euro");
                        ptf_CC.Valore_in_CC = row.Field<double>("Valore_in_CC");
                        ptf_CC.Id_Tipo_Soldi = (int)row.Field<long>("id_tipo_soldi");
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

        public PortafoglioTitoli GetPortafoglioTitoliById(int IdPortafoglioTitoli)
        {
            try
            {
                DataTable dt = new DataTable();
                    using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                    {
                        dbAdapter.SelectCommand = new SQLiteCommand();
                        dbAdapter.SelectCommand.CommandText = ManagerScripts.GetPortafoglioTitoliById;
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
        /// Estraggo la quantità di utile disponibile
        /// sulla base dell'anno e della gestione
        /// </summary>
        /// <param name="guadagnoQuote">Il record con i dati da verificare</param>
        /// <returns>Disponibilità di utili</returns>
        public double VerifyDisponibilitaUtili(GuadagnoPerQuote guadagnoQuote)
        {
            try
            {
                DataTable DT = new DataTable();
                    using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                    {
                        dbAdapter.SelectCommand = new SQLiteCommand();
                        dbAdapter.SelectCommand.CommandText = ManagerScripts.VerifyDisponibilitaUtili;
                        dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", guadagnoQuote.IdGestione);
                        dbAdapter.SelectCommand.Parameters.AddWithValue("anno", guadagnoQuote.Anno);
                        dbAdapter.SelectCommand.Parameters.AddWithValue("daInserire", guadagnoQuote.Preso);
                        dbAdapter.SelectCommand.Parameters.AddWithValue("id_valuta", guadagnoQuote.IdCurrency);
                        dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dbAdapter.Fill(DT);
                        return DT.Rows[0].ItemArray[0] is DBNull ? -1.0 : Convert.ToDouble(DT.Rows[0].ItemArray[0]);
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
        /// Registro il prelievo di utili
        /// </summary>
        /// <param name="guadagnoQuote">Il record da inserire</param>
        public void InsertPrelievoUtili(GuadagnoPerQuote guadagnoQuote)
        {
            try
            {
                int result = 0;
                    using (SQLiteCommand dbComm = new SQLiteCommand())
                    {
                        dbComm.CommandText = ManagerScripts.InsertPrelievoUtili;
                        dbComm.Parameters.AddWithValue("id_gestione", guadagnoQuote.IdGestione);
                        dbComm.Parameters.AddWithValue("id_tipo_movimento", guadagnoQuote.IdTipoMovimento);
                        dbComm.Parameters.AddWithValue("id_valuta", guadagnoQuote.IdCurrency);
                        dbComm.Parameters.AddWithValue("anno", guadagnoQuote.Anno);
                        dbComm.Parameters.AddWithValue("ammontare", guadagnoQuote.Preso);
                        dbComm.Parameters.AddWithValue("data_operazione", guadagnoQuote.DataOperazione.ToString("yyyy-MM-dd"));
                        dbComm.Parameters.AddWithValue("causale", guadagnoQuote.Causale);
                        dbComm.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dbComm.Connection.Open();
                        dbComm.ExecuteNonQuery();
                        dbComm.Connection.Close();
                    }
                    using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                    {
                        dataAdapter.SelectCommand = new SQLiteCommand();
                        dataAdapter.SelectCommand.CommandText = "SELECT id_guadagno FROM guadagni_totale_anno ORDER BY id_guadagno DESC LIMIT 1";
                        dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        DataTable dt = new DataTable();
                        dataAdapter.Fill(dt);
                        result = Convert.ToInt32(dt.Rows[0].ItemArray[0]);
                    }
                    using (SQLiteCommand dbComm = new SQLiteCommand())
                    {
                        dbComm.CommandText = ManagerScripts.InsertPrelievoUtiliBkd;
                        dbComm.Parameters.AddWithValue("id_prelievo", result);
                        dbComm.Parameters.AddWithValue("id_gestione", guadagnoQuote.IdGestione);
                        dbComm.Parameters.AddWithValue("id_tipo_movimento", guadagnoQuote.IdTipoMovimento);
                        dbComm.Parameters.AddWithValue("id_valuta", guadagnoQuote.IdCurrency);
                        dbComm.Parameters.AddWithValue("anno", guadagnoQuote.Anno);
                        dbComm.Parameters.AddWithValue("ammontare", guadagnoQuote.Preso);
                        dbComm.Parameters.AddWithValue("data_operazione", guadagnoQuote.DataOperazione.ToString("yyyy-MM-dd"));
                        dbComm.Parameters.AddWithValue("causale", guadagnoQuote.Causale);
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
        /// Elimino una registrazione di prelievo utili
        /// </summary>
        /// <param name="guadagnoPerQuote"></param>
        public void DeletePrelievoUtili(GuadagnoPerQuote guadagnoPerQuote)
        {
            try
            {
                    using (SQLiteCommand dbComm = new SQLiteCommand())
                    {
                        dbComm.CommandText = ManagerScripts.DeletePrelievoUtiliBKd;
                        dbComm.Parameters.AddWithValue("id_prelievo", guadagnoPerQuote.IdGuadagno);
                        dbComm.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dbComm.Connection.Open();
                        dbComm.ExecuteNonQuery();
                        dbComm.Connection.Close();
                    }

                    using (SQLiteCommand dbComm = new SQLiteCommand())
                    {
                        dbComm.CommandText = ManagerScripts.DeletePrelievoUtili;
                        dbComm.Parameters.AddWithValue("id_guadagno", guadagnoPerQuote.IdGuadagno);
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
        /// Prelevo le info per i costi medi dei titoli attivi
        /// </summary>
        /// <returns></returns>
        public PortafoglioTitoliList GetCostiMediPerTitolo()
        {
            try
            {
                DataTable dt = new DataTable();
                PortafoglioTitoliList PTL = new PortafoglioTitoliList();
                    using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                    {
                        dbAdapter.SelectCommand = new SQLiteCommand();
                        dbAdapter.SelectCommand.CommandText = ManagerScripts.GetCostiMediPerTitolo;
                        dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dbAdapter.Fill(dt);
                    }
                foreach (DataRow DR in dt.Rows)
                {
                    PortafoglioTitoli PT = new PortafoglioTitoli();
                    PT.Nome_Gestione = DR.Field<string>("nome_gestione");
                    PT.Desc_Conto = DR.Field<string>("desc_conto");
                    PT.Id_tipo_titolo = (uint)DR.Field<long>("id_tipo_titolo") ;
                    PT.Desc_tipo_titolo = DR.Field<string>("desc_tipo_titolo");
                    PT.Desc_titolo = DR.Field<string>("desc_titolo");
                    PT.Isin = DR.Field<string>("isin");
                    PT.Importo_totale = DR.Field<double>("CostoMedio");
                    PT.N_titoli = DR.Field<double>("TitoliAttivi");
                    PT.Costo_unitario_in_valuta = DR.Field<double>("CostoUnitarioMedio");
                    PTL.Add(PT);
                }
                return PTL;
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
        /// Estrae tutti i movimenti di un dato conto per una data gestione di un anno per una valuta
        /// e Costruisce il dato cumulato partendo dal primo giorno inserito nel database
        /// </summary>
        /// <param name="IdConto">E' il conto corrente</param>
        /// <param name="IdGestione">E' la gestione nel conto</param>
        /// <param name="AnnoSelezionato">l'anno di cui si vuole il dettaglio</param>
        /// <param name="IdValuta">la valuta</param>
        /// <returns></returns>
        public MovimentiContoList GetMovimentiContoGestioneValuta(int IdConto, int IdGestione, int AnnoSelezionato, int IdValuta)
        {
            try
            {
                DataTable DT = new DataTable();
                MovimentiContoList MCL = new MovimentiContoList();
                    using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                    {
                        dbAdapter.SelectCommand = new SQLiteCommand();
                        dbAdapter.SelectCommand.CommandText = ManagerScripts.GetMovimentiContoGestioneValuta;
                        dbAdapter.SelectCommand.Parameters.AddWithValue("IdGestione", IdGestione);
                        dbAdapter.SelectCommand.Parameters.AddWithValue("IdConto", IdConto);
                        dbAdapter.SelectCommand.Parameters.AddWithValue("IdCurrency", IdValuta);
                        dbAdapter.SelectCommand.Parameters.AddWithValue("Year_1", AnnoSelezionato - 1);
                        dbAdapter.SelectCommand.Parameters.AddWithValue("Year", AnnoSelezionato);
                        dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                        dbAdapter.Fill(DT);
                    }
                foreach (DataRow row in DT.Rows)
                {
                    MovimentiConto MC = new MovimentiConto();
                    MC.Id_Riga_Conto = DAFconnection.GetConnectionType().Contains("sqlite") ? (int)row.Field<long>("id_fineco_euro") : (int)row.Field<uint>("id_fineco_euro");
                    MC.Desc_Conto = row.Field<string>("desc_conto");
                    MC.Nome_Gestione = row.Field<string>("nome_gestione");
                    MC.Desc_Movimento = row.Field<string>("desc_movimento");
                    MC.Desc_TipoTitolo = row.Field<string>("desc_tipo_titolo");
                    MC.Desc_Titolo = row.Field<string>("desc_titolo");
                    MC.Isin = row.Field<string>("isin");
                    MC.Desc_Valuta = row.Field<string>("desc_valuta");
                    MC.DataMovimento = row.Field<DateTime>("data_movimento");
                    MC.Entrate = row.Field<double>("ENTRATE");
                    MC.Uscite = row.Field<double>("USCITE");
                    MC.Cumulato = row.Field<double>("CUMULATO");
                    MC.Causale = row.Field<string>("causale");
                    MC.Desc_Tipo_Soldi = row.Field<string>("desc_tipo_soldi");
                    MCL.Add(MC);
                }
                return MCL;
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
    }
}

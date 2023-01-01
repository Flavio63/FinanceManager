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
                    dbComm.Parameters.AddWithValue("Causale", RecordQuoteGuadagno.Causale);
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
                                quote.Causale = dataRow.Field<string>("Causale");
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
                quote.Id_Quote_Investimenti = (int)dataRow.Field<long>("id_quote_inv");
                quote.Id_Gestione = (int)dataRow.Field<long>("id_gestione");
                quote.NomeInvestitore = dataRow.Field<string>("nome_gestione");
                quote.Id_tipo_movimento = (int)dataRow.Field<long>("id_tipo_movimento");
                quote.Desc_tipo_movimento = dataRow.Field<string>("desc_movimento");
                quote.DataMovimento = dataRow.Field<DateTime>("data_movimento");
                quote.AmmontareEuro = dataRow.Field<double>("ammontare");
                quote.Id_Valuta = (int)dataRow.Field<long>("id_valuta");
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
                    dbComm.Parameters.AddWithValue("id_quote_inv", ActualQuote.Id_Quote_Investimenti);
                    dbComm.Parameters.AddWithValue("id_gestione", ActualQuote.Id_Gestione);
                    dbComm.Parameters.AddWithValue("id_tipo_movimento", ActualQuote.Id_tipo_movimento);
                    dbComm.Parameters.AddWithValue("data_movimento", ActualQuote.DataMovimento.ToString("yyyy-MM-dd"));
                    dbComm.Parameters.AddWithValue("ammontare", ActualQuote.AmmontareEuro);
                    dbComm.Parameters.AddWithValue("id_valuta", ActualQuote.Id_Valuta);
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
        /// Trovo l'id dell'ultimo record inserito
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
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", ActualQuote.Id_Gestione);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_tipo_movimento", ActualQuote.Id_tipo_movimento);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_periodo_quote", ActualQuote.Id_Periodo_Quote);
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
                    return Convert.ToInt32(DT.Rows[0].ItemArray[0]);
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
        /// Inserisce un nuovo movimento all'interno della tabella QuoteInvestimenti
        /// </summary>
        /// <param name="ActualQuote">I dati del movimento da inserire</param>
        public void InsertInvestment(QuoteTab ActualQuote)
        {
            //using (SQLiteConnection con = new SQLiteConnection(DAFconnection.GetConnectionType()))
            //{
            //    con.Open();
            //    using (SQLiteTransaction transaction = con.BeginTransaction())
            //    {
            //        using (SQLiteCommand command = new SQLiteCommand(ManagerScripts.InsertInvestment, con))
            //        {
            //            try
            //            {
            //                command.Parameters.AddWithValue("id_gestione", ActualQuote.Id_Gestione);
            //                command.Parameters.AddWithValue("id_tipo_movimento", ActualQuote.Id_tipo_movimento);
            //                command.Parameters.AddWithValue("id_periodo_quote", ActualQuote.Id_Periodo_Quote);
            //                command.Parameters.AddWithValue("data_movimento", ActualQuote.DataMovimento.ToString("yyyy-MM-dd"));
            //                command.Parameters.AddWithValue("id_valuta", ActualQuote.Id_Valuta);
            //                command.Parameters.AddWithValue("valuta_base", ActualQuote.AmmontareValuta);
            //                command.Parameters.AddWithValue("valore_cambio", ActualQuote.ChangeValue);
            //                command.Parameters.AddWithValue("ammontare", ActualQuote.AmmontareEuro);
            //                command.Parameters.AddWithValue("note", ActualQuote.Note);
            //                command.ExecuteNonQuery();
            //                transaction.Commit();
            //            }
            //            catch (SQLiteException err)
            //            {
            //                transaction.Rollback();
            //                throw new Exception(err.Message + " Investimento non inserito");
            //            }
            //            catch (Exception err)
            //            {
            //                transaction.Rollback();
            //                throw new Exception(err.Message);
            //            }
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Elimina il record dalla tabella investimenti in
        /// base al suo identificativo
        /// </summary>
        /// <param name="Id_ActualQuote"></param>
        public void DeleteRecordQuoteTab(int Id_ActualQuote)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DAFconnection.GetConnectionType()))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(ManagerScripts.DeleteRecordQuoteTab))
                    {
                        try
                        {
                            command.ExecuteNonQuery();
                            transaction.Commit();
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
                    dbComm.Parameters.AddWithValue("Causale", guadagnoQuote.Causale);
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
                    dbComm.Parameters.AddWithValue("Causale", guadagnoQuote.Causale);
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
        /// Restituisco la somma dei soldi disponibili nella tabella
        /// degli investimenti
        /// </summary>
        /// <param name="IdInvestitore"></param>
        /// <param name="IdValuta"></param>
        /// <returns>QuoteTabList</returns>
        public QuoteTabList GetTotalAmountByCurrency(int IdInvestitore, int IdValuta = 0)
        {
            try
            {
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.CommandText = IdValuta > 0 ?
                        string.Format(ManagerScripts.GetTotalAmountByCurrency, string.Format("AND A.id_valuta = {0} ", IdValuta)) :
                        string.Format(ManagerScripts.GetTotalAmountByCurrency, "");
                    dataAdapter.SelectCommand.Parameters.AddWithValue("IdInvestitore", IdInvestitore);
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);
                    QuoteTabList QTL = new QuoteTabList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        QTL.Add(new QuoteTab()
                        {
                            NomeInvestitore = (string)dr["Nome"],
                            AmmontareValuta = (double)dr["Soldi"],
                            CodeCurrency = (string)dr["Valuta"]
                        });

                    }
                    return QTL;
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

        public QuoteTab GetQuoteTabById(int Id)
        {
            try
            {
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.CommandText = ManagerScripts.GetQuoteTabById;
                    dataAdapter.SelectCommand.Parameters.AddWithValue("Id_quote", Id);
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);
                    QuoteTab qt = new QuoteTab();
                    qt.Id_Quote_Investimenti = dt.Rows[0].Field<int>("id_quote_inv");
                    qt.Id_Gestione = dt.Rows[0].Field<int>("id_gestione");
                    qt.NomeInvestitore = dt.Rows[0].Field<string>("nome_gestione");
                    qt.Id_tipo_movimento = dt.Rows[0].Field<int>("id_tipo_movimento");
                    qt.Desc_tipo_movimento = dt.Rows[0].Field<string>("desc_movimento");
                    qt.Id_Periodo_Quote = dt.Rows[0].Field<int>("id_periodo_quote");
                    qt.DataMovimento = dt.Rows[0].Field<DateTime>("data_movimento");
                    qt.Id_Valuta = dt.Rows[0].Field<int>("id_valuta");
                    qt.CodeCurrency = dt.Rows[0].Field<string>("cod_valuta");
                    qt.ChangeValue = dt.Rows[0].Field<double>("valore_cambio");
                    qt.AmmontareEuro = dt.Rows[0].Field<double>("ammontare");
                    qt.AmmontareValuta = dt.Rows[0].Field<double>("valuta_base");
                    qt.Note = dt.Rows[0].Field<string>("");
                    return qt;
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
    }
}

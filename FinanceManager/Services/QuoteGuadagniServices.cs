using FinanceManager.Models;
using FinanceManager.Services.SQL;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services
{
    public class QuoteGuadagniServices : IQuoteGuadagniServices
    {
        IDAFconnection DAFconnection;

        public QuoteGuadagniServices(IDAFconnection iDAFconnection)
        {
            DAFconnection = iDAFconnection ?? throw new ArgumentNullException("Manca la stringa di connessione al db");
        }

        /// <summary>
        /// Verifico se nella data di inserimento è già presente
        /// un investimento
        /// </summary>
        /// <param name="ActualCC">Il record per verificare la presenza di un gemello</param>
        /// <param name="Id_Tipo_Gestione">Il tipo di gestione per il calcolo dei guadagni</param>
        /// <returns>-1 se falso altrimenti il numero del record</returns>
        public object VerifyInvestmentDate(ContoCorrente ActualCC, int Id_Tipo_Gestione)
        {
            try
            {
                DataTable DT = new DataTable();
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.CommandText = QuoteGuadagniScript.VerifyInvestmentDate;
                    dataAdapter.SelectCommand.Parameters.AddWithValue("id_tipo_soldi", Id_Tipo_Gestione);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("data_inizio", ActualCC.DataMovimento.ToString("yyyy-MM-dd"));
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dataAdapter.Fill(DT);
                    if (DT.Rows[0].ItemArray[0] is long)
                        return DT.Rows[0].ItemArray[0];
                    else
                    {
                        DT = new DataTable();
                        dataAdapter.SelectCommand.CommandText = QuoteGuadagniScript.GetIdPeriodoQuote;
                        dataAdapter.SelectCommand.Parameters.Clear();
                        dataAdapter.SelectCommand.Parameters.AddWithValue("data_movimento", ActualCC.DataMovimento.ToString("yyyy-MM-dd"));
                        dataAdapter.SelectCommand.Parameters.AddWithValue("id_tipo_gestione", Id_Tipo_Gestione);
                        dataAdapter.Fill(DT);
                        return DT.Rows[0].ItemArray[0];
                    }
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
        /// Modifico la tabella quote_periodi modificando la data di fine
        /// e inserendo il nuovo record
        /// </summary>
        /// <param name="DataDal">Data da cercare</param>
        /// <param name="TipoSoldi">Tipo_Gestione dei soldi</param>
        /// <returns>Il record di quote_periodi</returns>
        public QuotePeriodi Update_InsertQuotePeriodi(DateTime DataDal, int TipoSoldi)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.CommandText = QuoteGuadagniScript.quote_periodi;
                    cmd.Parameters.AddWithValue("StartDate", DataDal.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("TipoSoldi", TipoSoldi);
                    cmd.Parameters.AddWithValue("Date_Time", DateTime.Now);
                    cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();  // aggiorna la penultima riga e inserisce la nuova riga
                    cmd.CommandText = QuoteGuadagniScript.ultima_riga;  // dalla nuova riga ritorna il valore del date time
                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);
                    QuotePeriodi QP = new QuotePeriodi
                    {
                        IdPeriodoQuote = Convert.ToInt16(dt.Rows[0].Field<object>("id_quote_periodi")),
                        IdAggregazione = Convert.ToInt16(dt.Rows[0].Field<object>("id_tipo_gestione")),
                        DataInizio = dt.Rows[0].Field<DateTime>("data_inizio"),
                        DataFine = dt.Rows[0].Field<DateTime>("data_fine"),
                        DataInsert = dt.Rows[0].Field<DateTime>("data_insert")
                    };

                    return QP;

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
        /// Calcolo le nuove quote e le inserisco nella tabella quote_guadagno
        /// </summary>
        /// <param name="Tipo_Gestione">La gestione che determina la quota</param>
        /// <param name="NuovoPeriodo">Il nuovo periodo da inserire in tabella</param>
        public void ComputesAndInsertQuoteGuadagno(int Tipo_Gestione, int NuovoPeriodo)
        {
            using (SQLiteConnection con = new SQLiteConnection(DAFconnection.GetConnectionType()))
            {
                con.Open();
                using (SQLiteTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand())
                        {
                            cmd.CommandText = QuoteGuadagniScript.ComputesQuoteGuadagno;
                            cmd.Parameters.AddWithValue("Tipo_Gestione", Tipo_Gestione);
                            cmd.Connection = con;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = QuoteGuadagniScript.InsertQuotaGuadagno;
                            cmd.Parameters.Clear();
                            cmd.Connection = con;
                            cmd.Parameters.AddWithValue("Nuovo_Periodo", NuovoPeriodo);
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
        /// Calcolo le nuove quote e modifico la tabella quote_guadagno
        /// </summary>
        /// <param name="Tipo_Gestione">Long, Volatili, etc.</param>
        public void ComputesAndModifyQuoteGuadagno(int Tipo_Gestione)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandText = QuoteGuadagniScript.ComputesQuoteGuadagno;
                    dbComm.Parameters.AddWithValue("Tipo_Gestione", Tipo_Gestione);
                    dbComm.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbComm.Connection.Open();
                    dbComm.ExecuteNonQuery();
                    dbComm.CommandText = QuoteGuadagniScript.UpdateQuotaGuadagno;
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
        /// <param name="Tipo_Gestione">La gestione long, volatili, etc</param>
        public void UpdateGuadagniTotaleAnno(int Id_Periodo_Quote, int Tipo_Gestione)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandText = QuoteGuadagniScript.UpdateGuadagniTotaleAnno;
                    dbComm.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbComm.Parameters.AddWithValue("IdPeriodoQuote", Id_Periodo_Quote);
                    dbComm.Parameters.AddWithValue("Tipo_Gestione", Tipo_Gestione);
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
        /// Trovo il codice dei record da ricalcolare con le nuove quote
        /// </summary>
        /// <param name="dateTime">la data dell'investimento</param>
        /// <param name="Id_Tipo_Gestione">Identifica il tipo di gestione da cui si deducono le quote</param>
        /// <returns>int</returns>
        public int GetIdPeriodoQuote(DateTime dateTime, int Id_Tipo_Gestione)
        {
            DataTable DT = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = QuoteGuadagniScript.GetIdPeriodoQuote;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("data_movimento", dateTime.ToString("yyyy-MM-dd"));
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_tipo_gestione", Id_Tipo_Gestione);
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
                    return DT.Rows.Count == 0 ? 0 : (int)DT.Rows[0].Field<long>("id_quote_periodi");
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
        /// Tramite l'ultimo record conto_corrente inserito
        /// calcolo e inserisco le quote guadagno per ogni singolo socio
        /// </summary>
        /// <param name="RecordContoCorrente">record conto corrente con i dati</param>
        public void AddSingoloGuadagno(ContoCorrente RecordContoCorrente)
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection(DAFconnection.GetConnectionType()))
            {
                dbConnection.Open();
                using (SQLiteTransaction transaction = dbConnection.BeginTransaction())
                {
                    try
                    {
                        using (SQLiteCommand dbComm = new SQLiteCommand())
                        {
                            dbComm.CommandText = QuoteGuadagniScript.AddSingoloGuadagno;
                            dbComm.Parameters.AddWithValue("id_tipo_movimento", RecordContoCorrente.Id_tipo_movimento);
                            dbComm.Parameters.AddWithValue("id_tipo_soldi", RecordContoCorrente.Id_Tipo_Soldi);
                            dbComm.Parameters.AddWithValue("id_quote_periodi", RecordContoCorrente.Id_Quote_Periodi);
                            dbComm.Connection = dbConnection;
                            dbComm.ExecuteNonQuery();
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
        /// Elimino un record dalla tabella quote_guadagno
        /// </summary>
        /// <param name="id_quota">identificativo del record base conto corrente</param>
        public void DeleteRecordGuadagno_Totale_anno(int id_quota)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandText = QuoteGuadagniScript.DeleteRecordGuadagno_Totale_anno;
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
                    dbComm.CommandText = QuoteGuadagniScript.ModifySingoloGuadagno;
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
        /// Restituisce una tabella con il valore cumulato
        /// suddiviso fra soci, valuta e tipo di gestione
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable GetTotaleCumulatoSocio()
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = QuoteGuadagniScript.GetTotaleCumulatoSocio;
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(dataTable);
                    return dataTable;
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception("GetTotaleCumulatoSocio " + err.Message);
            }
            catch (Exception err)
            {
                throw new Exception("GetTotaleCumulatoSocio " + err.Message);
            }
        }
        /// <summary>
        /// Restituisce una tabella con il valore totale
        /// suddiviso fra valuta e tipo di gestione
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable GetTotaleGenerale()
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = QuoteGuadagniScript.GetTotaleGenerale;
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(dataTable);
                    return dataTable;
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception("GetTotaleGenerale " + err.Message);
            }
            catch (Exception err)
            {
                throw new Exception("GetTotaleGenerale " + err.Message);
            }
        }
        /// <summary>
        /// Ritorna una tabella con tutti i movimenti dei capitali
        /// </summary>
        /// <returns>QuoteGuadagnoList</returns>
        public QuoteGuadagnoList GetQuoteGuadagni()
        {
            QuoteGuadagnoList quoteGuadagnos = new QuoteGuadagnoList();
            DataTable dataTable = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = QuoteGuadagniScript.GetTotaleCumulatoSocio;
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(dataTable);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        QuoteGuadagno quoteGuadagno = new QuoteGuadagno();
                        quoteGuadagno.id_quota_guadagno = Convert.ToInt32( row.Field<long>("id_quota_guadagno"));
                        quoteGuadagno.id_socio = Convert.ToInt32(row.Field<long>("id_socio"));
                        quoteGuadagno.nome_socio = row.Field<string>("nome_socio");
                        quoteGuadagno.id_quote_periodi = Convert.ToInt32(row.Field<long>("id_quote_periodi"));
                        quoteGuadagno.data_inizio = row.Field<DateTime>("data_inizio");
                        quoteGuadagno.data_fine = row.Field<DateTime>("data_fine");
                        quoteGuadagno.ammontare = row.Field<double>("ammontare");
                        quoteGuadagno.cum_socio = row.Field<double>("cum_socio");
                        quoteGuadagno.cum_totale = row.Field<double>("cum_totale");
                        quoteGuadagno.quota = row.Field<double>("quota");
                        quoteGuadagno.id_conto_corrente = Convert.ToInt32(row.Field<long>("id_conto_corrente"));
                        quoteGuadagno.id_valuta = Convert.ToInt32(row.Field<long>("id_valuta"));
                        quoteGuadagno.cod_valuta = row.Field<string>("cod_valuta");
                        quoteGuadagnos.Add(quoteGuadagno);
                    }
                    return quoteGuadagnos;
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception("GetTotaleCumulatoSocio " + err.Message);
            }
            catch (Exception err)
            {
                throw new Exception("GetTotaleCumulatoSocio " + err.Message);
            }
        }
        /// <summary>
        /// Ritorna il valore di cum_socio, cum_totale nel periodo e per la valuta dati
        /// </summary>
        /// <param name="id_socio">il socio</param>
        /// <param name="id_valuta">la valuta</param>
        /// <param name="id_tipo_gestione">il tipo di gestione degli utili</param>
        public QuoteGuadagno GetLastRecordBySocioValuta(int id_socio, int id_valuta, int id_tipo_gestione)
        {
            QuoteGuadagno quoteGuadagno = new QuoteGuadagno();
            DataTable dataTable = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = QuoteGuadagniScript.GetLastRecordBySocioValuta;
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_socio", id_socio);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_valuta", id_valuta);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_tipo_gestione", id_tipo_gestione);
                    dbAdapter.Fill(dataTable);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        quoteGuadagno.id_quota_guadagno = Convert.ToInt32(row.Field<long>("id_quota_guadagno"));
                        quoteGuadagno.id_socio = Convert.ToInt32(row.Field<long>("id_socio"));
                        quoteGuadagno.nome_socio = row.Field<string>("nome_socio");
                        quoteGuadagno.id_quote_periodi = Convert.ToInt32(row.Field<long>("id_quote_periodi"));
                        quoteGuadagno.data_inizio = row.Field<DateTime>("data_inizio");
                        quoteGuadagno.data_fine = row.Field<DateTime>("data_fine");
                        quoteGuadagno.ammontare = row.Field<double>("ammontare");
                        quoteGuadagno.cum_socio = row.Field<double>("cum_socio");
                        quoteGuadagno.cum_totale = row.Field<double>("cum_totale");
                        quoteGuadagno.quota = row.Field<double>("quota");
                        quoteGuadagno.id_conto_corrente = Convert.ToInt32(row.Field<long>("id_conto_corrente"));
                        quoteGuadagno.id_valuta = Convert.ToInt32(row.Field<long>("id_valuta"));
                        quoteGuadagno.cod_valuta = row.Field<string>("cod_valuta");
                        quoteGuadagno.id_tipo_gestione = Convert.ToInt32(row.Field<long>("id_tipo_gestione"));
                        quoteGuadagno.tipo_gestione = row.Field<string>("tipo_gestione");
                    }
                    return quoteGuadagno;
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception("GetLastRecordBySocioValuta " + err.Message);
            }
            catch (Exception err)
            {
                throw new Exception("GetLastRecordBySocioValuta " + err.Message);
            }
        }
        /// <summary>
        /// Registra un record di quote guadagno
        /// </summary>
        /// <param name="quoteGuadagno">il record da registrare</param>
        public void InsertRecordQuoteGuadagno(QuoteGuadagno quoteGuadagno)
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection(DAFconnection.GetConnectionType()))
            {
                dbConnection.Open();
                using (SQLiteTransaction transaction = dbConnection.BeginTransaction())
                {
                    try
                    {
                        using (SQLiteCommand dbComm = new SQLiteCommand())
                        {
                            dbComm.CommandText = QuoteGuadagniScript.InsertRecordQuoteGuadagno;
                            dbComm.Parameters.AddWithValue("id_socio", quoteGuadagno.id_socio);
                            dbComm.Parameters.AddWithValue("id_quote_periodi", quoteGuadagno.id_quote_periodi);
                            dbComm.Parameters.AddWithValue("ammontare", quoteGuadagno.ammontare);
                            dbComm.Parameters.AddWithValue("cum_socio", quoteGuadagno.cum_socio);
                            dbComm.Parameters.AddWithValue("cum_totale", quoteGuadagno.cum_totale);
                            dbComm.Parameters.AddWithValue("quota", quoteGuadagno.quota);
                            dbComm.Parameters.AddWithValue("id_conto_corrente", quoteGuadagno.id_conto_corrente);
                            dbComm.Parameters.AddWithValue("id_valuta", quoteGuadagno.id_valuta);
                            dbComm.Parameters.AddWithValue("id_tipo_gestione", quoteGuadagno.id_tipo_gestione);
                            dbComm.Connection = dbConnection;
                            dbComm.ExecuteNonQuery();
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
        /// Modifica i dati di un record di quote guadagno
        /// </summary>
        /// <param name="quoteGuadagno">il record da modificare</param>
        public void ModifyQuoteGuadagno(QuoteGuadagno quoteGuadagno)
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection(DAFconnection.GetConnectionType()))
            {
                dbConnection.Open();
                using (SQLiteTransaction transaction = dbConnection.BeginTransaction())
                {
                    try
                    {
                        using (SQLiteCommand dbComm = new SQLiteCommand())
                        {
                            dbComm.CommandText = QuoteGuadagniScript.ModifyQuoteGuadagno;
                            dbComm.Parameters.AddWithValue("id_socio", quoteGuadagno.id_socio);
                            dbComm.Parameters.AddWithValue("ammontare", quoteGuadagno.ammontare);
                            dbComm.Parameters.AddWithValue("cum_socio", quoteGuadagno.cum_socio);
                            dbComm.Parameters.AddWithValue("cum_totale", quoteGuadagno.cum_totale);
                            dbComm.Parameters.AddWithValue("quota", quoteGuadagno.quota);
                            dbComm.Parameters.AddWithValue("id_conto_corrente", quoteGuadagno.id_conto_corrente);
                            dbComm.Parameters.AddWithValue("id_quota_guadagno", quoteGuadagno.id_quota_guadagno);
                            dbComm.Connection = dbConnection;
                            dbComm.ExecuteNonQuery();
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

    }
}

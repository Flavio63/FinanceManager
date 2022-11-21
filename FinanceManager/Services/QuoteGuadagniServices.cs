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
        /// <param name="Id_Tipo_Soldi">Il tipo soldi che si sta movimentando</param>
        /// <returns>-1 se falso altrimenti il numero del record</returns>
        public object VerifyInvestmentDate(ContoCorrente ActualCC, int Id_Tipo_Soldi)
        {
            try
            {
                DataTable DT = new DataTable();
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.CommandText = QuoteGuadagniScript.VerifyInvestmentDate;
                    dataAdapter.SelectCommand.Parameters.AddWithValue("id_tipo_soldi", Id_Tipo_Soldi);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("data_inizio", ActualCC.DataMovimento.ToString("yyyy-MM-dd"));
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dataAdapter.Fill(DT);
                    return DT.Rows[0].ItemArray[0];
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
                        IdPeriodoQuote = Convert.ToInt16(dt.Rows[0].Field<object>("id_periodo_quote")),
                        IdAggregazione = Convert.ToInt16(dt.Rows[0].Field<object>("id_aggregazione")),
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
        /// <param name="Tipo_Soldi">Codice identificativo</param>
        /// <param name="NuovoPeriodo">Il nuovo periodo da inserire in tabella</param>
        public void ComputesAndInsertQuoteGuadagno(int Tipo_Soldi, int NuovoPeriodo)
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
                            cmd.Parameters.AddWithValue("Tipo_Soldi", Tipo_Soldi);
                            cmd.Connection = con;
                            cmd.ExecuteNonQuery();
//                            transaction.Commit();

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
        /// <param name="Tipo_Soldi">Codice identificativo</param>
        public void ComputesAndModifyQuoteGuadagno(int Tipo_Soldi)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandText = QuoteGuadagniScript.ComputesQuoteGuadagno;
                    dbComm.Parameters.AddWithValue("Tipo_Soldi", Tipo_Soldi);
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
        /// <param name="Id_Tipo_Soldi">Il tipo soldi</param>
        public void UpdateGuadagniTotaleAnno(int Id_Periodo_Quote, int Id_Tipo_Soldi)
        {
            try
            {
                using (SQLiteCommand dbComm = new SQLiteCommand())
                {
                    dbComm.CommandText = QuoteGuadagniScript.UpdateGuadagniTotaleAnno;
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
        /// Trovo il codice dei record da ricalcolare con le nuove quote
        /// </summary>
        /// <param name="dateTime">la data dell'investimento</param>
        /// <param name="Id_Gestione">Identifica il tipo di gestione da cui si deducono le quote</param>
        /// <returns>int</returns>
        public int GetIdPeriodoQuote(DateTime dateTime, int Id_Gestione)
        {
            DataTable DT = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = QuoteGuadagniScript.GetIdPeriodoQuote;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("data_movimento", dateTime.ToString("yyyy-MM-dd"));
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", Id_Gestione);
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

    }
}

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
    public class QuoteServices : IQuoteServices
    {
        IDAFconnection DAFconnection;

        public QuoteServices(IDAFconnection iDAFconnection)
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
                    dataAdapter.SelectCommand.CommandText = QuoteScript.VerifyInvestmentDate;
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
        /// <param name="TipoSoldi">Tipologia dei soldi</param>
        /// <returns>Last id record inserted</returns>
        public int Update_InsertQuotePeriodi(DateTime DataDal, int TipoSoldi)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.CommandText = QuoteScript.quote_periodi;
                    cmd.Parameters.AddWithValue("StartDate", DataDal.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("TipoSoldi", TipoSoldi);
                    cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = QuoteScript.ultima_riga;
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
                    cmd.CommandText = QuoteScript.ComputesQuoteGuadagno;
                    cmd.Parameters.AddWithValue("Tipo_Soldi", Tipo_Soldi);
                    cmd.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = QuoteScript.InsertQuotaGuadagno;
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
                    dbComm.CommandText = QuoteScript.ComputesQuoteGuadagno;
                    dbComm.Parameters.AddWithValue("Tipo_Soldi", Tipo_Soldi);
                    dbComm.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbComm.Connection.Open();
                    dbComm.ExecuteNonQuery();
                    dbComm.CommandText = QuoteScript.UpdateQuotaGuadagno;
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
                    dbComm.CommandText = QuoteScript.UpdateGuadagniTotaleAnno;
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

    }
}

using FinanceManager.Models;
using FinanceManager.Models.Enumeratori;
using FinanceManager.Services.SQL;
using System;
using System.Data;
using System.Data.SQLite;

namespace FinanceManager.Services
{
    public class ContoCorrenteServices : IContoCorrenteServices
    {

        IDAFconnection DAFconnection;

        public ContoCorrenteServices(IDAFconnection iDAFconnection)
        {
            DAFconnection = iDAFconnection ?? throw new ArgumentNullException("Manca la stringa di connessione al db");
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
                    dbComm.CommandText = ContoCorrenteScript.DeleteContoCorrenteByIdPortafoglioTitoli;
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
                    dbComm.CommandText = ContoCorrenteScript.DeleteRecordContoCorrente;
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
                    dbAdapter.SelectCommand.CommandText = ContoCorrenteScript.Get2ContoCorrentes;
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
                    dbAdapter.SelectCommand.CommandText = ContoCorrenteScript.GetContoCorrenteByIdPortafoglio;
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
                    dbAdapter.SelectCommand.CommandText = ContoCorrenteScript.GetContoCorrente;
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
        /// Estrae tutti i record con codice 1 e 2
        /// (deposito/prelievo) del conto 0 (zero)
        /// </summary>
        /// <returns>Lista di ContoCorrente</returns>
        public ContoCorrenteList GetCCListByInvestmentSituation()
        {
            DataTable DT = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = ContoCorrenteScript.GetCCListByInvestmentSituation;
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
        /// Preleva dal conto zero la situazione degli investimenti per socio
        /// facendo la somma fra versati, investiti, disinvestiti e prelevati
        /// </summary>
        /// <returns>Situazione per socio</returns>
        public InvestmentSituationList GetInvestmentSituation()
        {
            DataTable DT = new DataTable();
            try
            {
                using (SQLiteDataAdapter dbAdapter = new SQLiteDataAdapter())
                {
                    dbAdapter.SelectCommand = new SQLiteCommand();
                    dbAdapter.SelectCommand.CommandText = ContoCorrenteScript.GetInvestmentSituation;
                    dbAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(DT);
                    InvestmentSituationList iSituationList = new InvestmentSituationList();
                    foreach (DataRow inv in DT.Rows)
                    {
                        iSituationList.Add(new InvestmentSituation()
                        {
                            IdGestione = Convert.ToInt16(inv["id_gestione"]),
                            Socio = (string)inv["Socio"],
                            IdValuta = Convert.ToInt16(inv["id_valuta"]),
                            CodValuta = (string)inv["cod_valuta"],
                            Versato = Convert.ToDouble(inv["Versato"]),
                            Investito = Convert.ToDouble(inv["Investito"]),
                            Disinvestito = Convert.ToDouble(inv["Disinvestito"]),
                            Disponibile = Convert.ToDouble(inv["Disponibile"])
                        });
                    }
                    return iSituationList;
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

        public ContoCorrente GetLastContoCorrente()
        {
            try
            {
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    dataAdapter.SelectCommand.CommandText = ContoCorrenteScript.GetLastContoCorrente;
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);
                    return contoCorrente(dt.Rows[0]);
                }
            }
            catch (SQLiteException err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Restituisco le somme dei soldi presenti nei conti correnti
        /// suddivisi per gestione e valuta
        /// </summary>
        /// <param name="IdConto"></param>
        /// <param name="IdGestione"></param>
        /// <param name="IdValuta"></param>
        /// <returns>ContoCorrenteList</returns>
        public ContoCorrenteList GetTotalAmountByAccount(int IdConto, int IdGestione = 0, int IdValuta = 0, int IdTipoSoldi = 0)
        {
            try
            {
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
                {
                    dataAdapter.SelectCommand = new SQLiteCommand();
                    string query0 = "";
                    string query1 = "";
                    string query2 = "";
                    string query3 = "";
                    if (IdConto == 0)
                        dataAdapter.SelectCommand.CommandText = string.Format(ContoCorrenteScript.GetTotalAmountByAccount, "", "", "", "");
                    else if (IdConto > 0 && IdGestione == 0 && IdValuta == 0 && IdTipoSoldi == 0)
                    {
                        query0 = String.Format(" AND A.id_conto = {0} ", IdConto);
                        dataAdapter.SelectCommand.CommandText = string.Format(ContoCorrenteScript.GetTotalAmountByAccount, query0, "", "", "");
                    }
                    else if (IdConto > 0 && IdGestione > 0 && IdValuta == 0 && IdTipoSoldi == 0)
                    {
                        query0 = String.Format(" AND A.id_conto = {0} ", IdConto);
                        query1 = string.Format(" AND A.id_gestione = {0} ", IdGestione);
                        dataAdapter.SelectCommand.CommandText = string.Format(ContoCorrenteScript.GetTotalAmountByAccount, query0, query1, "", "");
                    }
                    else if (IdConto > 0 && IdGestione > 0 && IdValuta > 0 && IdTipoSoldi == 0)
                    {
                        query0 = String.Format(" AND A.id_conto = {0} ", IdConto);
                        query1 = string.Format(" AND A.id_gestione = {0} ", IdGestione);
                        query2 = string.Format(" AND A.id_valuta = {0} ", IdValuta);
                        dataAdapter.SelectCommand.CommandText = string.Format(ContoCorrenteScript.GetTotalAmountByAccount, query0, query1, query2, "");
                    }
                    else if (IdConto > 0 && IdGestione > 0 && IdValuta > 0 && IdTipoSoldi > 0)
                    {
                        query0 = String.Format(" AND A.id_conto = {0} ", IdConto);
                        query1 = string.Format(" AND A.id_gestione = {0} ", IdGestione);
                        query2 = string.Format(" AND A.id_valuta = {0} ", IdValuta);
                        query3 = string.Format(" AND A.id_tipo_soldi = {0} ", IdTipoSoldi);
                        dataAdapter.SelectCommand.CommandText = String.Format(ContoCorrenteScript.GetTotalAmountByAccount, query0, query1, query2, query3);
                    }
                    dataAdapter.SelectCommand.Parameters.AddWithValue("id_conto", IdConto);
                    dataAdapter.SelectCommand.Connection = new SQLiteConnection(DAFconnection.GetConnectionType());
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);
                    ContoCorrenteList CCL = new ContoCorrenteList();
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        CCL.Add(new ContoCorrente()
                        {
                            Id_Conto = Convert.ToInt32(dataRow["id_conto"]),
                            Desc_Conto = (string)dataRow["Conto"],
                            Id_Gestione = Convert.ToInt32(dataRow["id_gestione"]),
                            NomeGestione = (string)dataRow["Gestione"],
                            Ammontare = (double)dataRow["Soldi"],
                            Cod_Valuta = (string)dataRow["Valuta"],
                            Id_Tipo_Soldi = Convert.ToInt32(dataRow["id_tipo_soldi"]),
                            Desc_Tipo_Soldi = (string)dataRow["desc_tipo_soldi"],
                            Valore_Cambio = (double)dataRow["cambio"]
                        });
                    }
                    return CCL;
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
        /// Scrive un nuovo record per il conto corrente
        /// </summary>
        /// <param name="contoCorrente"></param>
        /// <exception cref="Exception"></exception>
        public void InsertAccountMovement(ContoCorrente contoCorrente)
        {
            using (SQLiteConnection con = new SQLiteConnection(DAFconnection.GetConnectionType()))
            {
                con.Open();
                using (SQLiteTransaction transaction = con.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(ContoCorrenteScript.InsertAccountMovement, con))
                    {
                        try
                        {
                            command.Parameters.AddWithValue("id_conto", contoCorrente.Id_Conto);
                            command.Parameters.AddWithValue("id_valuta", contoCorrente.Id_Valuta);
                            command.Parameters.AddWithValue("id_portafoglio_titoli", contoCorrente.Id_Portafoglio_Titoli);
                            command.Parameters.AddWithValue("id_tipo_movimento", contoCorrente.Id_tipo_movimento);
                            command.Parameters.AddWithValue("id_gestione", contoCorrente.Id_Gestione);
                            command.Parameters.AddWithValue("id_titolo", contoCorrente.Id_Titolo);
                            command.Parameters.AddWithValue("data_movimento", contoCorrente.DataMovimento.ToString("yyyy-MM-dd"));
                            command.Parameters.AddWithValue("ammontare", contoCorrente.Ammontare);
                            command.Parameters.AddWithValue("cambio", contoCorrente.Valore_Cambio);
                            command.Parameters.AddWithValue("Causale", contoCorrente.Causale);
                            command.Parameters.AddWithValue("id_tipo_soldi", contoCorrente.Id_Tipo_Soldi);
                            command.Parameters.AddWithValue("id_quote_periodi", contoCorrente.Id_Quote_Periodi);
                            command.Parameters.AddWithValue("modified", contoCorrente.Modified.ToString("yyyy-MM-dd HH:mm:ss"));
                            command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                        catch (SQLiteException err)
                        {
                            transaction.Rollback();
                            throw new Exception(err.Message + " Investimento non inserito");
                        }
                        catch (Exception err)
                        {
                            transaction.Rollback();
                            throw new Exception(err.Message + " Investimento non inserito");
                        }
                    }
                }
            }
        }

        public void UpdateRecordContoCorrente(ContoCorrente contoCorrente, TipologiaIDContoCorrente tipologiaID)
        {
            using (SQLiteConnection con = new SQLiteConnection(DAFconnection.GetConnectionType()))
            {
                con.Open();
                using (SQLiteTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (SQLiteCommand dbComm = new SQLiteCommand(con))
                        {
                            dbComm.CommandType = CommandType.Text;
                            if (tipologiaID == TipologiaIDContoCorrente.IdContoCorrente)
                                dbComm.CommandText = ContoCorrenteScript.UpdateContoCorrenteByIdCC;
                            else if (tipologiaID == TipologiaIDContoCorrente.IdContoTitoli)
                                dbComm.CommandText = ContoCorrenteScript.UpdateContoCorrenteByIdPortafoglioTitoli;
                            dbComm.Parameters.AddWithValue("id_fineco_euro", contoCorrente.Id_RowConto);
                            dbComm.Parameters.AddWithValue("id_conto", contoCorrente.Id_Conto);
                            dbComm.Parameters.AddWithValue("id_valuta", contoCorrente.Id_Valuta);
                            dbComm.Parameters.AddWithValue("id_portafoglio_titoli", contoCorrente.Id_Portafoglio_Titoli);
                            dbComm.Parameters.AddWithValue("id_tipo_movimento", contoCorrente.Id_tipo_movimento);
                            dbComm.Parameters.AddWithValue("id_gestione", contoCorrente.Id_Gestione);
                            dbComm.Parameters.AddWithValue("id_titolo", contoCorrente.Id_Titolo);
                            dbComm.Parameters.AddWithValue("data_movimento", contoCorrente.DataMovimento.ToString("yyyy-MM-dd"));
                            dbComm.Parameters.AddWithValue("ammontare", contoCorrente.Ammontare);
                            dbComm.Parameters.AddWithValue("cambio", contoCorrente.Valore_Cambio);
                            dbComm.Parameters.AddWithValue("Causale", contoCorrente.Causale);
                            dbComm.Parameters.AddWithValue("id_tipo_soldi", contoCorrente.Id_Tipo_Soldi);
                            dbComm.Parameters.AddWithValue("id_quote_periodi", contoCorrente.Id_Quote_Periodi);
                            dbComm.Parameters.AddWithValue("modified", contoCorrente.Modified.ToString("yyyy-MM-dd HH:mm:ss"));
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
            conto.Causale = dataRow.Field<string>("Causale");
            conto.Id_Tipo_Soldi = (int)dataRow.Field<long>("id_tipo_soldi");
            conto.Desc_Tipo_Soldi = dataRow.Field<string>("desc_tipo_soldi");
            conto.Modified = dataRow.Field<DateTime>("modified");
            return conto;
        }


    }
}

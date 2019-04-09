using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using FinanceManager.Models;
using System.Data;

namespace FinanceManager.Services
{
    public class ManagerReportServices : SQL.DAFconnection, IManagerReportServices
    {
        public ReportTitoliAttiviList GetActiveAssets(IList<int> _selectedOwners, IList<int> _selectedAccount)
        {
            try
            {
                string owners = " (";
                foreach (int o in _selectedOwners)
                    owners += " A.id_gestione = " + o + " or ";
                owners = owners.Substring(0, owners.Length - 4);
                owners += ") ";
                string accounts = " (";
                foreach (int a in _selectedAccount)
                    accounts += " A.id_conto = " + a + " or ";
                accounts = accounts.Substring(0, accounts.Length - 4);
                accounts += ") ";

                ReportTitoliAttiviList RTAL = new ReportTitoliAttiviList();
                DataTable table = new DataTable();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = string.Format(SQL.ReportScripts.GetActiveAsset, owners, accounts);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    dbAdapter.Fill(table);
                    foreach (DataRow dataRow in table.Rows)
                    {
                        ReportTitoliAttivi RTA = new ReportTitoliAttivi();
                        RTA.Gestione = dataRow.Field<string>("nome_gestione");
                        RTA.Conto = dataRow.Field<string>("desc_conto");
                        RTA.Tipo_Titolo = dataRow.Field<string>("desc_tipo_titolo");
                        RTA.Nome_Titolo = dataRow.Field<string>("desc_titolo");
                        RTA.Isin = dataRow.Field<string>("isin");
                        RTA.N_Titoli = dataRow.Field<double>("n_titoli");
                        RTA.ValoreAcquisto = dataRow.Field<double>("costoTotale");
                        RTA.CMC = dataRow.Field<double>("CMC");
                        RTA.Note = dataRow.Field<string>("note");
                        RTAL.Add(RTA);
                    }
                    return RTAL;
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

        public IList<int> GetAvailableYears()
        {
            try
            {
                using (MySqlCommand dbComm = new MySqlCommand())
                {
                    dbComm.CommandType = System.Data.CommandType.Text;
                    dbComm.CommandText = SQL.ReportScripts.GetAvailableYears;
                    dbComm.Connection = new MySqlConnection(DafConnection);
                    dbComm.Connection.Open();
                    IList<int> anni = new List<int>();
                    using (MySqlDataReader dbReader = dbComm.ExecuteReader())
                    {
                        while (dbReader.Read())
                        {
                            anni.Add((int)dbReader.GetValue(0));
                        }
                    }
                    dbComm.Connection.Close();
                    return anni;
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

        public ReportMovementDetailedList GetMovementDetailed(int IdGestione, int IdTitolo)
        {
            try
            {
                ReportMovementDetailedList RMDL = new ReportMovementDetailedList();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = SQL.ReportScripts.GetMovementDetailed;
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_gestione", IdGestione);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("id_titolo", IdTitolo);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    DataTable dataTable = new DataTable();
                    dbAdapter.Fill(dataTable);
                    foreach(DataRow dr in dataTable.Rows)
                    {
                        ReportMovementDetailed RMD = new ReportMovementDetailed();
                        RMD.Gestione = dr.Field<string>("nome_gestione");
                        RMD.Conto = dr.Field<string>("desc_conto");
                        RMD.Movimento = dr.Field<string>("desc_movimento");
                        RMD.Tipo_Titolo = dr.Field<string>("desc_tipo_titolo");
                        RMD.Nome_Titolo = dr.Field<string>("desc_titolo");
                        RMD.Isin = dr.Field<string>("isin");
                        RMD.Tipo_Soldi = dr.Field<string>("desc_tipo_soldi");
                        RMD.DataMovimento = dr.Field<DateTime>("data_movimento");
                        RMD.Uscite = dr.Field<double>("uscite");
                        RMD.Entrate = dr.Field<double>("entrate");
                        RMD.Causale = dr.Field<string>("causale");
                        RMDL.Add(RMD);
                    }
                }
                return RMDL;
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

        public ReportProfitLossList GetReport1(IList<int> _selectedOwners,
            IList<int> _selectedYears)
        {
            string owners = " (";
            foreach (int i in _selectedOwners)
                owners += " A.id_gestione = " + i + " or ";
            owners = owners.Substring(0, owners.Length - 4);
            owners += ") ";
            string anni = " (";
            foreach (int i in _selectedYears)
                anni += " year(data_movimento) = " + i + " or ";
            anni = anni.Substring(0, anni.Length - 4);
            anni += ") ";
            string query = owners + " and " + anni;
            try
            {
                ReportProfitLossList reportProfitLossList = new ReportProfitLossList();
                DataTable data = new DataTable();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = string.Format(SQL.ReportScripts.GetProfitLoss, query);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    dbAdapter.Fill(data);
                    foreach(DataRow dr in data.Rows)
                    {
                        ReportProfitLoss RPL = new ReportProfitLoss();
                        RPL.Anno = dr.Field<int>("Anno");
                        RPL.Gestione = dr.Field<string>("nome_gestione");
                        RPL.TipoSoldi = dr.Field<string>("desc_tipo_soldi");

                        RPL.Azioni = RPL.TipoSoldi == "Perdita di Capitale" ? dr.Field<double>("Azioni") * -1 : dr.Field<double>("Azioni");
                        RPL.Obbligazioni = RPL.TipoSoldi == "Perdita di Capitale" ? dr.Field<double>("Obbligazioni") * -1 : dr.Field<double>("Obbligazioni");
                        RPL.ETF = RPL.TipoSoldi == "Perdita di Capitale" ? dr.Field<double>("ETF") * -1 : dr.Field<double>("ETF");
                        RPL.Fondo = RPL.TipoSoldi == "Perdita di Capitale" ? dr.Field<double>("Fondo") * -1 : dr.Field<double>("Fondo");
                        RPL.Volatili = RPL.TipoSoldi == "Perdita di Capitale" ? dr.Field<double>("Volatili") * -1 : dr.Field<double>("Volatili");
                        RPL.Totale = RPL.TipoSoldi == "Perdita di Capitale" ? dr.Field<double>("Totale") * -1 : dr.Field<double>("Totale");
                        reportProfitLossList.Add(RPL);
                    }
                }
                return reportProfitLossList;
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
}

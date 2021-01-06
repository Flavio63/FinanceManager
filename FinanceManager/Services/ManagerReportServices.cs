using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using FinanceManager.Models;
using System.Data;
using FinanceManager.Services.SQL;

namespace FinanceManager.Services
{
    public class ManagerReportServices : IManagerReportServices
    {
        IDAFconnection DAFconnection;
        public ManagerReportServices(IDAFconnection iDAFconnection)
        {
            DAFconnection = iDAFconnection ?? throw new ArgumentNullException("Manca la stringa di connessione al db");
        }

        public ReportTitoliAttiviList GetActiveAssets(IList<RegistryOwner> _selectedOwners, IList<int> _selectedAccount)
        {
            try
            {
                string owners = " (";
                foreach (RegistryOwner o in _selectedOwners)
                    owners += " A.id_gestione = " + o.Id_gestione + " or ";
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
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
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
                    dbComm.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
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

        public GuadagnoPerPeriodoList GetDeltaPeriod(IList<RegistryOwner> _selectedOwners, IList<int> _selectedYears, bool isYear, bool isAggregated)
        {
            GuadagnoPerPeriodoList GPPL = new GuadagnoPerPeriodoList();
            string gestione = "A.id_gestione = ";
            foreach(RegistryOwner RO in _selectedOwners)
            {
                gestione += RO.Id_gestione;
                gestione += " OR A.id_gestione = ";
            }
            gestione = gestione.Substring(0, gestione.Length - 20);
            
            string anni = "YEAR(data_movimento) = ";
            foreach(int anno in _selectedYears)
            {
                anni += anno + " OR YEAR(data_movimento) = ";
            }
            anni = anni.Substring(0, anni.Length - 27);

            string query = string.Format("AND ({0}) AND ({1})", gestione, anni);

            try
            {
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = CommandType.Text;
                    if (isYear && isAggregated)
                        dbAdapter.SelectCommand.CommandText = string.Format(ReportScripts.GetDeltaPerYearTot, query);
                    else if(isYear && !isAggregated)
                        dbAdapter.SelectCommand.CommandText = string.Format(ReportScripts.GetDeltaPerYear, query);
                    else if(!isYear && isAggregated)
                        dbAdapter.SelectCommand.CommandText = string.Format(ReportScripts.GetDeltaPerMonthTot, query);
                    else if(!isYear && !isAggregated)
                        dbAdapter.SelectCommand.CommandText = string.Format(ReportScripts.GetDeltaPerMonth, query);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("Anno1", _selectedYears[0] > _selectedYears[1] ? _selectedYears[1] : _selectedYears[0]);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("Anno2", _selectedYears[1] < _selectedYears[0] ? _selectedYears[0] : _selectedYears[1]);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    DataTable dataTable = new DataTable();
                    dbAdapter.Fill(dataTable);
                    foreach(DataRow DR in dataTable.Rows)
                    {
                        GuadagnoPerPeriodo GPP = new GuadagnoPerPeriodo();
                        GPP.IdGestione = Convert.ToInt32(DR.ItemArray[0]);
                        GPP.Gestione = DR.Field<string>("nome_gestione");
                        if (!isYear)
                        {
                            switch ( DR.Field<int>("Mese") )
                            {
                                case 1:
                                    GPP.Mese = "Gennaio";
                                    break;
                                case 2:
                                    GPP.Mese = "Febbraio";
                                    break;
                                case 3:
                                    GPP.Mese = "Marzo";
                                    break;
                                case 4:
                                    GPP.Mese = "Aprile";
                                    break;
                                case 5:
                                    GPP.Mese = "Maggio";
                                    break;
                                case 6:
                                    GPP.Mese = "Giugno";
                                    break;
                                case 7:
                                    GPP.Mese = "Luglio";
                                    break;
                                case 8:
                                    GPP.Mese = "Agosto";
                                    break;
                                case 9:
                                    GPP.Mese = "Settembre";
                                    break;
                                case 10:
                                    GPP.Mese = "Ottobre";
                                    break;
                                case 11:
                                    GPP.Mese = "Novembre";
                                    break;
                                case 12:
                                    GPP.Mese = "Dicembre";
                                    break;
                            }
                        }
                        GPP.GuadagnoAnno1 = DR.Field<double>("GuadagniAnno1");
                        GPP.GuadagnoAnno2 = DR.Field<double>("GuadagniAnno2");
                        GPP.Differenza = DR.Field<double>("Differenza");
                        GPP.Delta = DR.ItemArray[5] == System.DBNull.Value ? 0 : DR.Field<double>("Delta");
                        GPPL.Add(GPP);
                    }
                }
                return GPPL;
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
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    DataTable dataTable = new DataTable();
                    dbAdapter.Fill(dataTable);
                    foreach (DataRow dr in dataTable.Rows)
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

        public ReportProfitLossList GetReport1(IList<RegistryOwner> _selectedOwners,
            IList<int> _selectedYears, bool isSynthetic)
        {
            string owners = " (";
            foreach (RegistryOwner i in _selectedOwners)
                owners += " A.id_gestione = " + i.Id_gestione + " or ";
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
                    dbAdapter.SelectCommand.CommandText = isSynthetic == true ? string.Format(SQL.ReportScripts.GetProfitLoss, query) :
                         string.Format(SQL.ReportScripts.GetDetailedProfitLoss, query);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdapter.Fill(data);
                    foreach (DataRow dr in data.Rows)
                    {
                        ReportProfitLoss RPL = new ReportProfitLoss();
                        RPL.Anno = dr.Field<int>("Anno");
                        RPL.Gestione = dr.Field<string>("nome_gestione");
                        RPL.TipoSoldi = dr.Field<string>("desc_tipo_soldi");
                        if (!isSynthetic)
                        {
                            RPL.NomeTitolo = dr.Field<string>("desc_titolo");
                            RPL.ISIN = dr.Field<string>("isin");
                        }
                        else
                        {
                            RPL.NomeTitolo = "";
                            RPL.ISIN = "";
                        }
                        RPL.Azioni = RPL.TipoSoldi == "Perdita di Capitale" ? dr.Field<double>("Azioni") * -1 : dr.Field<double>("Azioni");
                        RPL.Obbligazioni = RPL.TipoSoldi == "Perdita di Capitale" ? dr.Field<double>("Obbligazioni") * -1 : dr.Field<double>("Obbligazioni");
                        RPL.ETF = RPL.TipoSoldi == "Perdita di Capitale" ? dr.Field<double>("ETF") * -1 : dr.Field<double>("ETF");
                        RPL.Fondo = RPL.TipoSoldi == "Perdita di Capitale" ? dr.Field<double>("Fondo") * -1 : dr.Field<double>("Fondo");
                        RPL.Volatili = RPL.TipoSoldi == "Perdita di Capitale" ? dr.Field<double>("Volatili") * -1 : dr.Field<double>("Volatili");
                        RPL.Costi = RPL.TipoSoldi == "Perdita di Capitale" ? dr.Field<double>("Costi") * -1 : dr.Field<double>("Costi");
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

        public AnalisiPortafoglio QuoteInvGeoSettori(IList<RegistryOwner> _selectedOwners)
        {
            string gestioni = "A.id_gestione = ";
            if (_selectedOwners.Count >= 1)
            {
                foreach (RegistryOwner item in _selectedOwners)
                    gestioni += item.Id_gestione + " OR A.id_gestione = ";
            }
            else
            {
                throw new Exception("ManagerReportService - Errore nel passaggio dei dati");
            }
            gestioni = gestioni.Substring(0, gestioni.Length - 20);
            try
            {
                using (MySqlDataAdapter dbAdaptar = new MySqlDataAdapter())
                {
                    dbAdaptar.SelectCommand = new MySqlCommand();
                    dbAdaptar.SelectCommand.Connection = new MySqlConnection(DAFconnection.GetConnectionType());
                    dbAdaptar.SelectCommand.CommandType = CommandType.Text;
                    dbAdaptar.SelectCommand.CommandText = string.Format(SQL.ReportScripts.QuoteInvGeoSettori, gestioni);
                    DataTable dt = new DataTable();
                    dbAdaptar.Fill(dt);
                    gestioni = "";
                    AnalisiPortafoglio RS = new AnalisiPortafoglio();
                    foreach (var property in RS.GetType().GetProperties())
                    {
                        if (property.Name != "id_titolo" && property.Name != "desc_titolo"
                            && property.Name != "Isin" && property.Name != "data_modifica"
                            && property.Name != "id_tipo_titolo" && property.Name != "id_azienda" 
                            && property.Name != "Nome")
                        {
                            property.SetValue(RS, dt.Rows[0].Field<object>(property.Name));
                        }
                        else if (property.Name == "Nome")
                        {
                            foreach (RegistryOwner item in _selectedOwners)
                                gestioni += item.Nome_Gestione + " + ";
                            gestioni = gestioni.Substring(0, gestioni.Length - 3);
                            property.SetValue(RS, gestioni);
                        }
                    }
                    return RS;
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
}

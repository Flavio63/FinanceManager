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

        public ReportProfitLossList GetReport1(IList<int> _selectedOwners,
            IList<int> _selectedYears, IList<int> _selectedCurrency, double[] exchangeValue = null)
        {
            double ED = exchangeValue == null ? 1 : exchangeValue[0];
            double ES = exchangeValue == null ? 1 : exchangeValue[1];
            double EFS = exchangeValue == null ? 1 : exchangeValue[2];
            string owners = "";
            foreach (int i in _selectedOwners)
                owners += "id_gestione = " + i + " or ";
            owners = owners.Substring(0, owners.Length - 4);
            string anni = "";
            foreach (int i in _selectedYears)
                anni += "year(data_movimento) = " + i + " or ";
            anni = anni.Substring(0, anni.Length - 4);
            string valute = "A.id_valuta = 1 or A.id_valuta = 2 or A.id_valuta = 3 or A.id_valuta = 4";
            if (_selectedCurrency.Count > 0)
            {
                valute = "";
                foreach (int i in _selectedCurrency)
                    valute += "A.id_valuta = " + i + " or ";
                valute = valute.Substring(0, valute.Length - 4);
            }
            try
            {
                ReportProfitLossList reportProfitLossList = new ReportProfitLossList();
                DataTable data = new DataTable();
                using (MySqlDataAdapter dbAdapter = new MySqlDataAdapter())
                {
                    dbAdapter.SelectCommand = new MySqlCommand();
                    dbAdapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                    dbAdapter.SelectCommand.CommandText = string.Format(SQL.ReportScripts.GetProfitLoss1, owners, anni, valute);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("EuroDollaro", ED);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("EuroSterlina", ES);
                    dbAdapter.SelectCommand.Parameters.AddWithValue("EuroFranchiSvizzeri", EFS);
                    dbAdapter.SelectCommand.Connection = new MySqlConnection(DafConnection);
                    dbAdapter.Fill(data);

                    ReportProfitLoss reportProfitLoss = new ReportProfitLoss();
                    int tmpAnno;
                    int tmpValuta = 0;
                    string tmpCodValuta = "";
                    DataRow dr;
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        dr = data.Rows[i];
                        tmpAnno = dr.Field<int>("Anno");
                        while (tmpAnno == dr.Field<int>("Anno"))
                        {
                            tmpValuta = exchangeValue != null ? 1 : (int)dr.Field<uint>("id_valuta");
                            tmpCodValuta = exchangeValue != null ? "EUR" : dr.Field<string>("cod_valuta");
                            while ((tmpValuta == (int)dr.Field<uint>("id_valuta") || exchangeValue != null) && tmpAnno == dr.Field<int>("Anno"))
                            {
                                reportProfitLoss.IdTipologia = (int)dr.Field<uint>("id_tipo");
                                reportProfitLoss.IdMovimento = (int)dr.Field<uint>("id_movimento");
                                switch (reportProfitLoss.IdTipologia)
                                {
                                    case 2:
                                        reportProfitLoss.Obb_Ced += reportProfitLoss.IdMovimento == 4 ? dr.Field<double>("PL") : 0;
                                        reportProfitLoss.Obb_Ven += reportProfitLoss.IdMovimento == 6 ? dr.Field<double>("PL") : 0;
                                        reportProfitLoss.Obb_Tot += (reportProfitLoss.IdMovimento == 4 || reportProfitLoss.IdMovimento == 6) ? dr.Field<double>("PL") : 0;
                                        break;
                                    case 1:
                                    case 4:
                                        reportProfitLoss.Azi_Ced += reportProfitLoss.IdMovimento == 4 ? dr.Field<double>("PL") : 0;
                                        reportProfitLoss.Azi_Ven += reportProfitLoss.IdMovimento == 6 ? dr.Field<double>("PL") : 0;
                                        reportProfitLoss.Azi_Tot += (reportProfitLoss.IdMovimento == 4 || reportProfitLoss.IdMovimento == 6) ? dr.Field<double>("PL") : 0;
                                        break;
                                    case 7:
                                        reportProfitLoss.Fon_Ced += reportProfitLoss.IdMovimento == 4 ? dr.Field<double>("PL") : 0;
                                        reportProfitLoss.Fon_Ven += reportProfitLoss.IdMovimento == 6 ? dr.Field<double>("PL") : 0;
                                        reportProfitLoss.Fon_Tot += (reportProfitLoss.IdMovimento == 4 || reportProfitLoss.IdMovimento == 6) ? dr.Field<double>("PL") : 0;
                                        break;
                                    case 5:
                                    case 6:
                                    case 8:
                                        reportProfitLoss.Etf_Ced += reportProfitLoss.IdMovimento == 4 ? dr.Field<double>("PL") : 0;
                                        reportProfitLoss.Etf_Ven += reportProfitLoss.IdMovimento == 6 ? dr.Field<double>("PL") : 0;
                                        reportProfitLoss.Etf_Tot += (reportProfitLoss.IdMovimento == 4 || reportProfitLoss.IdMovimento == 6) ? dr.Field<double>("PL") : 0;
                                        break;
                                    case 9:
                                    case 10:
                                        reportProfitLoss.Vol_Ced += reportProfitLoss.IdMovimento == 4 ? dr.Field<double>("PL") : 0;
                                        reportProfitLoss.Vol_Ven += reportProfitLoss.IdMovimento == 6 ? dr.Field<double>("PL") : 0;
                                        reportProfitLoss.Vol_Ced += (reportProfitLoss.IdMovimento == 4 || reportProfitLoss.IdMovimento == 6) ? dr.Field<double>("PL") : 0;
                                        break;
                                }
                                reportProfitLoss.Tot_Ced += reportProfitLoss.IdMovimento == 4 ? dr.Field<double>("PL") : 0;
                                reportProfitLoss.Tot_Ven += reportProfitLoss.IdMovimento == 6 ? dr.Field<double>("PL") : 0;
                                reportProfitLoss.Tot_Tot += (reportProfitLoss.IdMovimento == 4 || reportProfitLoss.IdMovimento == 6) ? dr.Field<double>("PL") : 0;
                                i++;
                                if (i > data.Rows.Count - 1)
                                {
                                    reportProfitLoss.Anno = tmpAnno;
                                    reportProfitLoss.Desc_Anno = tmpAnno.ToString();
                                    reportProfitLoss.IdValuta = tmpValuta;
                                    reportProfitLoss.CodiceValuta = tmpCodValuta;
                                    reportProfitLossList.Add(reportProfitLoss);
                                    return reportProfitLossList;
                                }
                                dr = data.Rows[i];
                            }
                            reportProfitLoss.Anno = tmpAnno;
                            reportProfitLoss.Desc_Anno = tmpAnno.ToString();
                            reportProfitLoss.IdValuta = tmpValuta;
                            reportProfitLoss.CodiceValuta = tmpCodValuta;
                            reportProfitLossList.Add(reportProfitLoss);
                            reportProfitLoss = new ReportProfitLoss();
                        }
                        i--;
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

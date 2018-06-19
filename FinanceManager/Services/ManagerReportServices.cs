using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text;
using System.Threading.Tasks;

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
                    using(MySqlDataReader dbReader = dbComm.ExecuteReader())
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
    }
}

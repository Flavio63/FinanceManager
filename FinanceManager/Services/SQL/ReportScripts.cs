using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services.SQL
{
    public class ReportScripts
    {
        public static readonly string GetAvailableYears = "SELECT YEAR(data_movimento) AS anni FROM daf_portafoglio GROUP BY anni ORDER BY anni DESC";
    }
}

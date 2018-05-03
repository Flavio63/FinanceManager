using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class RegistryShare
    {
        public uint IdShare { get; set; }
        public string DescShare { get; set; }
        public string Isin { get; set; }
        public int IdShareType { get; set; }
        public int IdFirm { get; set; }
        public int IdMarket { get; set; }
        public int IdCurrency { get; set; }
    }
}

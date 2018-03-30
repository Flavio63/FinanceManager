using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class ManagerLiquidAsset
    {
        public int idLiquidAsset { get; set; }
        public int IdOwner { get; set; }
        public string OwnerName { get; set; }

        public int IdLocation { get; set; }
        public string DescLocation { get; set; }
        
        public int IdCurrency { get; set; }
        public string CodeCurrency { get; set; }
        
        public int IdMovement { get; set; }
        public string DescMovement { get; set; }
        
        public DateTime MovementDate { get; set; }
        public double Amount { get; set; }
        public double ExchangeValue { get; set; }
        public bool Available { get; set; }
        public string Isin { get; set; }
        public string Note { get; set; }
    }
}

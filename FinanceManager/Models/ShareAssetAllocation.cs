using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class ShareAssetAllocation : RegistryShare
    {
        public double Azioni { get; set; }
        public double Obbligazioni { get; set; }
        public double Liquidita { get; set; }
        public double Altro { get; set; }
    }
}

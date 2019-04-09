using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class AnalisiPortafoglio : RegistryShare
    {
        public string Nome { get; set; }
        public double Totale { get; set; }
    }
}

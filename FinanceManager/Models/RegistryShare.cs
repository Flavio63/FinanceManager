using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class RegistryShare
    {
        public uint id_titolo { get; set; }
        public string desc_titolo { get; set; }
        public string Isin { get; set; }
        public uint id_tipo_titolo { get; set; }
        public uint id_azienda { get; set; }
        public DateTime data_modifica { get; set; }

        public RegistryShare()
        {
            data_modifica = DateTime.Now;
        }
    }
}

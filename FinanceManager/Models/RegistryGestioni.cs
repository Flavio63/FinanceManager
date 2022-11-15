using FinanceManager.Models.Enumeratori;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class RegistryGestioni
    {
        public int Id_Gestione { get; set; }
        public string Nome_Gestione { get; set; }
        public int Id_Tipo_Gestione { get; set; }
        public string Tipo_Gestione { get; set; }
        public int Id_Conto { get; set; }
    }
}

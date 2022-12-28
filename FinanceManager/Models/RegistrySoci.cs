using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class RegistrySoci
    {
        public int Id_Socio { get; set; }
        public string Nome_Socio { get; set; }
        public int Id_tipo_gestione { get; set; }
        public string Tipo_Gestione { get; set; }
    }
}

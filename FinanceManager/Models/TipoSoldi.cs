using FinanceManager.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class TipoSoldi:ViewModelBase
    {
        public int Id_Tipo_Soldi { get; set; }
        public string Desc_Tipo_Soldi { get; set; }
    }
}

using FinanceManager.Events;
using FinanceManager.Models.Enum;

namespace FinanceManager.Models
{
    public class TipoSoldi:ViewModelBase
    {
        public TipoSoldi (TipologiaSoldi tipologiaSoldi)
        {
            Id_Tipo_Soldi = (int)tipologiaSoldi;
            Desc_Tipo_Soldi = tipologiaSoldi.ToString();
        }
        public int Id_Tipo_Soldi { get; private set; }
        public string Desc_Tipo_Soldi { get; private set; }
    }
}

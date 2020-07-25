using FinanceManager.Events;
using FinanceManager.Models.Enumeratori;

namespace FinanceManager.Models
{
    public class TipoSoldi:ViewModelBase
    {
        public TipoSoldi (TipologiaSoldi tipologiaSoldi)
        {
            Id_Tipo_Soldi = (int)tipologiaSoldi;
            Short_Desc_Tipo_Soldi = tipologiaSoldi.ToString();
            Desc_Tipo_Soldi = tipologiaSoldi.GetDisplayName();
        }

        public int Id_Tipo_Soldi { get; set; }
        public string Desc_Tipo_Soldi { get; set; }
        public string Short_Desc_Tipo_Soldi { get; set; }
    }
}

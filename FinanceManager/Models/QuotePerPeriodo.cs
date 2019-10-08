using System;

namespace FinanceManager.Models
{
    public class QuotePerPeriodo
    {
        private DateTime Data1;
        private DateTime Data2;

        public int Id_Quota { get; set; }
        public int Id_Gestione { get; set; }
        public string Nome_Gestione { get; set; }
        public int Id_Tipo_Soldi { get; set; }
        public string Desc_Tipo_Soldi { get; set; }
        public int Id_Quote_Periodi { get; set; }
        public DateTime Data_Inizio
        {
            get
            {
                if (Data1.Date.ToShortDateString() == "01/01/0001" || Data1.Date.ToShortDateString() == "01/01/01")
                    Data1 = DateTime.Now.Date;
                return Data1;
            }
            set
            {
                Data1 = value;
            }
        }
        public DateTime Data_Fine
        {
            get
            {
                if (Data2.Date.ToShortDateString() == "01/01/0001" || Data2.Date.ToShortDateString() == "01/01/01")
                    Data2 = DateTime.Now.Date;
                return Data2;
            }
            set
            {
                Data2 = value;
            }
        }

        public double Quota { get; set; }
    }
}

using FinanceManager.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class ReportProfitLoss : ViewModelBase
    {
        public int Anno { get; set; }
        public string Desc_Anno { get; set; }
        public int IdValuta { get; set; }
        public string CodiceValuta { get; set; }
        public int IdTipologia { get; set; }
        public string DescTipologia { get; set; }
        public int IdMovimento { get; set; }
        public string DescMovimento { get; set; }
        public double Obb_Ced { get; set; }
        public double Obb_Ven { get; set; }
        public double Obb_Tot { get; set; }
        public double Azi_Ced { get; set; }
        public double Azi_Ven { get; set; }
        public double Azi_Tot { get; set; }
        public double Fon_Ced { get; set; }
        public double Fon_Ven { get; set; }
        public double Fon_Tot { get; set; }
        public double Etf_Ced { get; set; }
        public double Etf_Ven { get; set; }
        public double Etf_Tot { get; set; }
        public double Vol_Ced { get; set; }
        public double Vol_Ven { get; set; }
        public double Vol_Tot { get; set; }
        public double Tot_Ced { get; set; }
        public double Tot_Ven { get; set; }
        public double Tot_Tot { get; set; }
    }
}

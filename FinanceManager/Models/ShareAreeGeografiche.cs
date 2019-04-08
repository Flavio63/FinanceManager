using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class ShareAreeGeografiche : ShareAssetAllocation
    {
        public double USA { get; set; }
        public double Canada { get; set; }
        public double AmericaLatinaCentrale { get; set; }
        public double RegnoUnito { get; set; }
        public double EuropaOccEuro { get; set; }
        public double EuropaOccNoEuro { get; set; }
        public double EuropaEst { get; set; }
        public double Africa { get; set; }
        public double MedioOriente { get; set; }
        public double Giappone { get; set; }
        public double Australasia { get; set; }
        public double AsiaSviluppati { get; set; }
        public double AsiaEmergenti { get; set; }
        public double RegioniND { get; set; }
    }
}

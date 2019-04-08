using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class ShareSettori : ShareAreeGeografiche
    {
        public double MateriePrime { get; set; }
        public double BeniConsCiclici { get; set; }
        public double Finanza { get; set; }
        public double Immobiliare { get; set; }
        public double BeniConsDifensivi { get; set; }
        public double Salute { get; set; }
        public double ServiziPubbUtility { get; set; }
        public double ServiziComunic { get; set; }
        public double Energia { get; set; }
        public double BeniIndustriali { get; set; }
        public double Tecnologia { get; set; }
        public double SettoriND { get; set; }
    }
}

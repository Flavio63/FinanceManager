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
        public double Azioni { get; set; }
        public double Obbligazioni { get; set; }
        public double Liquidita { get; set; }
        public double Altro { get; set; }
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
        public DateTime data_modifica { get; set; }

        public RegistryShare()
        {
            data_modifica = DateTime.Now;
        }
    }
}

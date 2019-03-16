using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class RegistryShare
    {
        public uint IdShare { get; set; }
        public string DescShare { get; set; }
        public string Isin { get; set; }
        public uint IdShareType { get; set; }
        public uint IdFirm { get; set; }
        public double Azioni { get; set; }
        public double Obbligazioni { get; set; }
        public double Liquidita { get; set; }
        public double Altro { get; set; }
        public double USA { get; set; }
        public double Canada { get; set; }
        public double AmericaLatinaCentrale { get; set; }
        public double RegnoUnito { get; set; }
        public double EuropaOcc_Euro { get; set; }
        public double EuropaOcc_NoEuro { get; set; }
        public double EuropaEst { get; set; }
        public double Africa { get; set; }
        public double MedioOriente { get; set; }
        public double Giappone { get; set; }
        public double Australasia { get; set; }
        public double AsiaPaesiSviluppati { get; set; }
        public double AsiaPaesiEmergenti { get; set; }
        public double RegioniND { get; set; }
        public double MateriePrime { get; set; }
        public double BeniConsumoCiclici { get; set; }
        public double Finanza { get; set; }
        public double Immobiliare { get; set; }
        public double BeniConsumoDifensivi { get; set; }
        public double Salute { get; set; }
        public double ServiziPubblicaUtilita { get; set; }
        public double ServiziComunicazione { get; set; }
        public double Energia { get; set; }
        public double BeniIndustriali { get; set; }
        public double Tecnologia { get; set; }
        public double SettoriND { get; set; }
    }
}

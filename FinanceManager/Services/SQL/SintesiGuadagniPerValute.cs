using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services.SQL
{
    public class SintesiGuadagniPerValute
    {
        public static readonly string TemporaryTable = "DROP TABLE IF EXISTS tmpGain; " +
            "CREATE TEMPORARY TABLE IF NOT EXISTS tmpGain (anno INT(4) NOT NULL, id_gestione INT(10) NOT NULL, nome_gestione VARCHAR(100) NOT NULL, id_valuta INT(1) NOT NULL, cod_valuta VARCHAR(3) NOT NULL, " +
            "GuadagnoAnno1 DOUBLE NOT NULL DEFAULT 0, Preso DOUBLE NOT NULL DEFAULT 0, RisparmioAnno DOUBLE NOT NULL DEFAULT 0, RisparmioCumulato DOUBLE NOT NULL DEFAULT 0); ";

        public static readonly string InsertIntoTemp = "INSERT INTO tmpGain SELECT anno, A.id_gestione, B.nome_gestione, A.id_valuta, C.cod_valuta, SUM(guadagnato) AS GuadagnoAnno1, SUM(prelevato) AS Preso, " +
            "SUM(guadagnato + prelevato) AS RisparmioAnno, 0 FROM guadagni_totale_anno A, gestioni B, valuta C WHERE A.id_gestione = B.id_gestione AND A.id_valuta = C.id_valuta AND A.id_tipo_soldi <> 11 AND " +
            "anno >=2019 GROUP BY A.id_gestione, anno, A.id_valuta ORDER BY anno, A.id_gestione, A.id_valuta; ";

        public static readonly string MaxGestione = "SELECT MAX(id_gestione) FROM guadagni_totale_anno; ";
        public static readonly string MaxValute = "SELECT MAX(id_valuta) FROM guadagni_totale_anno; ";

        public static readonly string GetTemp = "SELECT * FROM tmpGain ORDER BY anno, id_gestione, id_valuta; ";

        public static readonly string dettagliato = "SELECT anno, A.id_guadagno, A.id_gestione, B.nome_gestione, A.id_tipo_movimento, C.desc_tipo_soldi, A.id_valuta, D.cod_valuta, A.data_operazione, A.quota, " +
            "guadagnato AS GuadagnoAnno1, prelevato AS Preso, causale FROM guadagni_totale_anno A, gestioni B, tipo_soldi C, valuta D WHERE anno >= 2019 AND A.id_gestione = B.id_gestione AND A.id_tipo_soldi<> 11 " +
            "AND A.id_tipo_soldi = C.id_tipo_soldi AND A.id_valuta = D.id_valuta ORDER BY anno DESC, A.data_operazione DESC, A.id_tipo_soldi, A.id_gestione DESC ";

        public static readonly string sintesi_tipologia = "SELECT anno, A.id_guadagno, A.id_gestione, B.nome_gestione, A.id_tipo_movimento, C.desc_tipo_soldi, A.id_valuta, D.cod_valuta, A.quota, sum(guadagnato) AS GuadagnoAnno1, " +
            "sum(prelevato) AS Preso FROM guadagni_totale_anno A, gestioni B, tipo_soldi C, valuta D WHERE anno >= 2019 AND A.id_gestione = B.id_gestione AND A.id_tipo_soldi<> 11 AND A.id_tipo_soldi = C.id_tipo_soldi " +
            "AND A.id_valuta = D.id_valuta group by anno, A.id_tipo_soldi, A.id_gestione ORDER BY anno DESC, A.id_tipo_soldi, A.id_gestione DESC";

        public static readonly string sintesi = "SELECT anno, B.nome_gestione, A.id_valuta, D.cod_valuta, SUM(guadagnato) AS GuadagnoAnno1, SUM(prelevato) AS Preso, SUM(guadagnato) + SUM(prelevato) AS RisparmioAnno, 0.0 AS RisparmioCumulato " +
            "FROM guadagni_totale_anno A, gestioni B, valuta D WHERE anno >= 2019 AND A.id_gestione = B.id_gestione AND A.id_tipo_soldi <> 11 AND A.id_valuta = D.id_valuta GROUP BY anno, A.id_gestione, A.id_valuta " +
            "ORDER BY A.id_gestione DESC, anno, A.id_valuta;";
    }
}

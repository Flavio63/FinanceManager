using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services.SQL
{
    public class ContoCorrenteScript
    {
        protected static readonly string ContoCorrente = "SELECT id_fineco_euro, A.id_conto, B.desc_conto, A.id_socio, H.nome_socio,  A.id_valuta, C.cod_valuta, id_portafoglio_titoli, A.id_tipo_movimento, " +
            "D.desc_movimento, A.id_gestione, E.nome_gestione, A.id_titolo, F.isin, F.desc_titolo, data_movimento, ammontare, cambio, Causale, A.id_tipo_soldi, G.desc_tipo_soldi, id_quote_periodi, A.id_tipo_gestione, modified " +
            "FROM conto_corrente A, conti B, valuta C, tipo_movimento D, gestioni E, titoli F, tipo_soldi G, soci H " +
            "WHERE A.id_conto = B.id_conto AND A.id_valuta = C.id_valuta AND A.id_tipo_movimento = D.id_tipo_movimento AND " +
            "A.id_gestione = E.id_gestione AND A.id_titolo = F.id_titolo AND A.id_tipo_soldi = G.id_tipo_soldi AND A.id_socio = H.id_socio AND id_fineco_euro > 0 ";

        public static readonly string GetContoCorrente = ContoCorrente + " ORDER BY data_movimento desc, A.id_fineco_euro";
        public static readonly string GetContoCorrenteByIdCC = ContoCorrente + " AND A.id_fineco_euro = @id_fineco_euro ";
        public static readonly string Get2ContoCorrentes = ContoCorrente + " AND A.modified = @modified ORDER BY  A.ammontare";
        public static readonly string GetContoCorrenteByIdPortafoglio = ContoCorrente + " AND A.id_portafoglio_titoli = @id_portafoglio_titoli ORDER BY A.data_movimento ";
        public static readonly string GetContoCorrenteByTipoGestioneDataValuta = ContoCorrente + " AND A.id_tipo_gestione = @id_tipo_gestione AND A.data_movimento > @data_movimento ";

        public static readonly string GetCCListByInvestmentSituation = ContoCorrente + " AND A.id_conto = 1 AND (A.id_tipo_movimento = 1 OR A.id_tipo_movimento = 2) ORDER BY A.data_movimento ";

        /// <summary>
        /// Ritorno l'ultimo record di conto corrente
        /// </summary>
        public static readonly string GetLastContoCorrente = ContoCorrente + " ORDER BY id_fineco_euro DESC LIMIT 1";

        /// <summary>
        /// Ritorna il totale finanziario per conto corrente
        /// a livello di codice viene aggiunto il filtro per gestione e per valuta
        /// </summary>
        public static readonly string GetTotalAmountByAccount = "SELECT A.id_conto, desc_conto as Conto, A.id_socio, nome_socio AS Socio, A.id_gestione, nome_gestione as Gestione, sum(ammontare) as Soldi, " +
            "cod_valuta as Valuta, A.id_tipo_soldi, E.desc_tipo_soldi, A.cambio FROM conto_corrente A, conti B, gestioni C, valuta D, tipo_soldi E, soci F where A.id_conto = B.id_conto " +
            "AND A.id_gestione = C.id_gestione AND A.id_valuta = D.id_valuta AND A.id_socio = F.id_socio and A.id_tipo_soldi = E.id_tipo_soldi {0} " +
            "GROUP BY A.id_conto, A.id_socio, A.id_gestione, A.id_valuta, A.id_tipo_soldi;";

        /// <summary>
        /// Fornisce quanto versato, prelevato, investito e disinvestito
        /// suddiviso per nome e valuta
        /// </summary>
        public static readonly string GetInvestmentSituation = "SELECT A.id_socio, E.nome_socio as Socio, A.id_valuta, C.cod_valuta, sum(case when A.id_tipo_movimento = 1 then ammontare else 0 end) as Versato, " +
            "sum (CASE WHEN A.id_tipo_movimento = 12 AND ammontare < 0 then ammontare ELSE 0 END) as Investito, sum (CASE WHEN A.id_tipo_movimento = 12 AND ammontare > 0 then ammontare ELSE 0 END) as Disinvestito, " +
            "sum (CASE WHEN A.id_tipo_movimento = 2 THEN ammontare else 0 end) as Prelevato, sum(ammontare) as Disponibile " +
            "FROM conto_corrente A, valuta C, soci E WHERE A.id_valuta = C.id_valuta AND A.id_socio = E.id_socio AND id_fineco_euro > 0 AND A.id_conto = 1 " +
            "GROUP BY A.id_socio, A.id_valuta";

        /// <summary>
        /// Inserisce un movimento nel conto corrente
        /// </summary>
        public static readonly string InsertAccountMovement = "INSERT INTO conto_corrente (id_conto, id_socio, id_valuta, id_portafoglio_titoli, id_tipo_movimento, " +
            "id_gestione, id_titolo, data_movimento, ammontare, cambio, Causale, id_tipo_soldi, id_quote_periodi, id_tipo_gestione, modified) VALUES ( @id_conto, @id_socio, @id_valuta, @id_portafoglio_titoli, @id_tipo_movimento, " +
            "@id_gestione, @id_titolo, @data_movimento, @ammontare, @cambio, @Causale, @id_tipo_soldi, @id_quote_periodi, @id_tipo_gestione, @modified)";


        /// <summary>
        /// Elimina un record dalla tabella conto_corrente sulla base di una eliminazione fatta sul portafoglio titoli
        /// </summary>
        public static readonly string DeleteContoCorrenteByIdPortafoglioTitoli = "DELETE FROM conto_corrente WHERE id_portafoglio_titoli = @id_portafoglio_titoli";

        // cancello un record del conto corrente //
        public static readonly string DeleteRecordContoCorrente = "DELETE FROM conto_corrente WHERE id_fineco_euro = @id_fineco_euro";

        // aggiorno un record conto corrente sulla base dell'ID CONTO CORRENTE //
        public static readonly string UpdateContoCorrenteByIdCC = "UPDATE conto_corrente SET id_conto = @id_conto, id_socio = @id_socio, id_valuta = @id_valuta, " +
            "id_portafoglio_titoli = @id_portafoglio_titoli, id_tipo_movimento = @id_tipo_movimento, id_gestione = @id_gestione, id_titolo = @id_titolo, data_movimento = @data_movimento, " +
            "ammontare = @ammontare, cambio = @cambio, Causale = @Causale, id_tipo_soldi = @id_tipo_soldi, id_quote_periodi = @id_quote_periodi, id_tipo_gestione = @id_tipo_gestione WHERE id_fineco_euro = @id_fineco_euro";

        // aggiorno un record conto corrente sulla base dell'ID PORTAFOGLIO TITOLI //
        public static readonly string UpdateContoCorrenteByIdPortafoglioTitoli = "UPDATE conto_corrente SET id_conto = @id_conto, id_socio = @id_socio, id_valuta = @id_valuta, " +
            "id_tipo_movimento = @id_tipo_movimento, id_gestione = @id_gestione, id_titolo = @id_titolo, data_movimento = @data_movimento, ammontare = @ammontare, " +
            "cambio = @cambio, Causale = @Causale, id_tipo_soldi = @id_tipo_soldi, id_quote_periodi = @id_quote_periodi, id_tipo_gestione = @id_tipo_gestione WHERE id_portafoglio_titoli = @id_portafoglio_titoli";

    }
}

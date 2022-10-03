using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services.SQL
{
    public class ContoTitoliScript
    {
        /// <summary>
        /// Aggiunge un movimento al portafoglio titoli
        /// </summary>
        public static readonly string AddMovimentoTitoli = "INSERT INTO portafoglio_titoli (id_portafoglio_titoli, id_gestione, id_conto, id_valuta, id_tipo_movimento, " +
            "id_titolo, data_movimento, ammontare, shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, " +
            "valore_cambio, note, attivo, link_movimenti) " +
            "VALUES (null, @id_gestione, @id_conto, @id_valuta, @id_tipo_movimento, @id_titolo, @data_movimento, @ammontare, @shares_quantity, " +
            "@unity_local_value, @total_commission, @tobin_tax, @disaggio_cedole, @ritenuta_fiscale, @valore_cambio, @note, @attivo, @link_movimenti);";

        private static readonly string GetListaTitoli = "SELECT id_portafoglio_titoli, B.id_gestione, nome_gestione, C.id_conto, desc_conto, D.id_valuta, cod_valuta, " +
    "E.id_tipo_movimento, desc_Movimento, G.id_tipo_titolo, G.desc_tipo_titolo, H.id_azienda, H.desc_azienda, A.id_titolo, F.desc_titolo, F.isin, data_movimento, " +
    "shares_quantity, unity_local_value, total_commission, tobin_tax, disaggio_cedole, ritenuta_fiscale, ammontare, valore_cambio, A.note, attivo, link_movimenti " +
    "FROM portafoglio_titoli A, gestioni B, conti C, valuta D, tipo_movimento E, titoli F, tipo_titoli G, aziende H " +
    "WHERE A.id_gestione = B.id_gestione AND A.id_conto = C.id_conto AND A.id_valuta = D.id_valuta AND A.id_tipo_movimento = E.id_tipo_movimento AND A.id_titolo = F.id_titolo AND " +
    "F.id_tipo_titolo = G.id_tipo_titolo AND F.id_azienda = H.id_azienda AND id_portafoglio_titoli > 0 ";

        /// <summary>
        /// Estrae tutti i movimenti dal portafoglio titoli
        /// </summary>
        public static readonly string GetListaTitoliListTotal = GetListaTitoli + " ORDER BY data_movimento DESC, A.id_tipo_movimento, A.id_portafoglio_titoli DESC";
        /// <summary>
        /// Estrae tutti i movimenti dal portafoglio titoli di una gestione
        /// </summary>
        public static readonly string GetListaTitoliListByOwner = GetListaTitoli + " AND A.id_gestione = @id_gestione ORDER BY data_movimento DESC, A.id_tipo_movimento";
        /// <summary>
        /// Estrae tutti i movimenti dal portafoglio titoli di un conto
        /// </summary>
        public static readonly string GetListaTitoliListByLocation = GetListaTitoli + " AND A.id_conto = @id_conto ORDER BY data_movimento DESC, A.id_tipo_movimento";
        /// <summary>
        /// Data una gestione e un conto estrae tutti i record
        /// </summary>
        public static readonly string GetListaTitoliByOwnerAndLocation = GetListaTitoli + " AND A.id_gestione = @id_gestione AND A.id_conto = @id_conto " +
            "ORDER BY data_movimento DESC, A.id_tipo_movimento";
        /// <summary>
        /// Estrae il singolo report (dato il suo id di riga)
        /// </summary>
        public static readonly string GetListaTitoliById = GetListaTitoli + " AND A.id_portafoglio_titoli = @id_portafoglio_titoli ";
        /// <summary>
        /// Estrae tutti i titoli legati dal date time usato come link
        /// </summary>
        public static readonly string GetListaTitoliListByLinkMovimenti = GetListaTitoli + " AND A.link_movimenti = @link_movimenti ORDER BY A.data_movimento";

        /// <summary>
        /// Data una gestione e una location, estrae l'ultimo record caricato
        /// </summary>
        public static readonly string GetLastMovimentoTitoliByOwnerAndLocation = GetListaTitoli +
            " AND B.id_gestione = @id_gestione AND C.id_conto = @id_conto AND A.id_titolo IS NOT NULL ORDER BY id_portafoglio_titoli DESC LIMIT 1";

        /// <summary>
        /// Calcola ed estrae i costi medi dei titoli suddivisi
        /// fra conto, gestione, tipo titolo e titolo stesso
        /// </summary>
        public static readonly string GetCostiMediPerTitolo = "SELECT C.nome_gestione, D.desc_conto, B.id_tipo_titolo, E.desc_tipo_titolo, B.desc_titolo, B.isin, " +
    "SUM(ammontare +(total_commission + tobin_tax + disaggio_cedole + ritenuta_fiscale)*-1) AS CostoMedio, SUM(shares_quantity) AS TitoliAttivi, " +
    "SUM(ammontare + (total_commission + tobin_tax + disaggio_cedole + ritenuta_fiscale) * -1) / SUM(shares_quantity) AS CostoUnitarioMedio " +
    "FROM portafoglio_titoli A, titoli B, gestioni C, conti D, tipo_titoli E " +
    "WHERE A.id_gestione<> 0 AND attivo > 0 AND A.id_tipo_movimento <> 6 AND A.id_titolo = B.id_titolo AND A.id_gestione = C.id_gestione AND A.id_conto = D.id_conto AND B.id_tipo_titolo = E.id_tipo_titolo " +
    "GROUP BY A.id_gestione, A.id_conto, E.id_tipo_titolo, A.id_titolo " +
    "ORDER BY A.id_gestione, A.id_conto, E.desc_tipo_titolo, B.desc_titolo";
        /// <summary>
        /// Estrae il numero di azioni possedute dato una gestione, un conto e un id azione
        /// </summary>
        public static readonly string GetSharesQuantity = "SELECT SUM(shares_quantity) TotalShares FROM portafoglio_titoli " +
            "WHERE id_gestione = @id_gestione AND id_conto = @id_conto AND id_titolo = @id_titolo";
        /// <summary>
        /// Aggiorna un movimento del portafoglio titoli
        /// </summary>
        public static readonly string UpdateMovimentoTitoli = "UPDATE portafoglio_titoli SET id_gestione = @id_gestione, id_conto = @id_conto, id_valuta = @id_valuta, " +
            "id_tipo_movimento = @id_tipo_movimento, id_titolo = @id_titolo, data_movimento = @data_movimento, ammontare = @ammontare, shares_quantity = @shares_quantity, " +
            "unity_local_value = @unity_local_value, total_commission = @total_commission, tobin_tax = @tobin_tax, disaggio_cedole = @disaggio_cedole, " +
            "ritenuta_fiscale = @ritenuta_fiscale, valore_cambio = @valore_cambio, " +
            "note = @note, attivo = @attivo, link_movimenti = @link_movimenti WHERE id_portafoglio_titoli = @id_portafoglio_titoli";
        /// <summary>
        /// Dato un id_titoli, una gestione e un conto restituisce i titoli ancora in portafoglio
        /// </summary>
        public static readonly string GetListaTitoliAttiviByContoGestioneTitolo = "SELECT AA.id_portafoglio_titoli, AA.id_gestione, AA.id_conto, AA.id_valuta, AA.id_tipo_movimento, AA.id_titolo, " +
            "AA.data_movimento, AA.ammontare AS ValoreAzione, AA.shares_quantity, AA.unity_local_value, AA.total_commission, AA.tobin_tax, AA.disaggio_cedole, AA.ritenuta_fiscale, AA.valore_cambio, " +
            "AA.note, BB.id_fineco_euro, BB.ammontare AS Valore_in_CC, BB.id_tipo_soldi FROM portafoglio_titoli AA LEFT JOIN conto_corrente BB ON AA.id_portafoglio_titoli = BB.id_portafoglio_titoli " +
            "WHERE AA.id_conto = @id_conto AND AA.id_gestione = @id_gestione AND AA.id_titolo = @id_titolo AND AA.attivo = 1 ORDER BY AA.data_movimento";
        /// <summary>
        /// Cancella un movimento del portafoglio titoli
        /// </summary>
        public static readonly string DeleteManagerLiquidAsset = "DELETE FROM portafoglio_titoli WHERE id_portafoglio_titoli = @id_portafoglio_titoli;";

    }
}

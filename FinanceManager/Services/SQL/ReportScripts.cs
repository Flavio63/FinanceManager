namespace FinanceManager.Services.SQL
{
    public class ReportScripts
    {
        public static readonly string GetAvailableYears = "SELECT YEAR(data_movimento) AS anni FROM conto_corrente WHERE (id_tipo_movimento = 4 or id_tipo_movimento = 6) GROUP BY anni ORDER BY anni DESC";

        public static readonly string GetProfitLoss = "SELECT YEAR(data_movimento) AS Anno, B.nome_gestione, D.desc_tipo_soldi, " +
            "ROUND(SUM(CASE WHEN C.id_tipo_titolo = 1 THEN ammontare ELSE 0 END), 2) AS Azioni, " +
            "round(sum(case when C.id_tipo_titolo = 2 THEN ammontare else 0 end), 2) AS Obbligazioni, " +
            "round(sum(case when (C.id_tipo_titolo = 4 OR C.id_tipo_titolo = 5 OR C.id_tipo_titolo = 6 OR C.id_tipo_titolo = 8) AND A.id_gestione <> 7 THEN ammontare else 0 end), 2) AS ETF, " +
            "round(sum(case when C.id_tipo_titolo = 7 THEN ammontare else 0 end), 2) AS Fondo, " +
            "round(sum(case when C.id_tipo_titolo = 13 or C.id_tipo_titolo = 4 THEN ammontare else 0 end), 2) AS Volatili, " +
            "round(sum(case when A.id_tipo_movimento = 8 THEN ammontare else 0 end), 2) AS Costi, " +
            "round(sum(case when (C.id_tipo_titolo >= 1 OR A.id_tipo_movimento = 8) THEN ammontare else 0 end), 2) AS Totale " +
            "FROM conto_corrente A, gestioni B, titoli C, tipo_soldi D " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_titolo = C.id_titolo AND A.id_tipo_soldi = D.id_tipo_soldi AND " +
            "A.id_tipo_soldi > 1 AND {0} " +
            "GROUP BY Anno, A.id_gestione, A.id_tipo_soldi " +
            "ORDER BY Anno DESC, A.id_gestione, A.id_tipo_soldi DESC;";

        public static readonly string GetDetailedProfitLoss = "SELECT Anno, nome_gestione, desc_tipo_soldi, desc_titolo, isin, Azioni, Obbligazioni, ETF, Fondo, Volatili, Costi, Totale FROM (" +
            "SELECT YEAR(data_movimento) AS Anno, B.nome_gestione, D.desc_tipo_soldi, C.desc_titolo, C.isin, " +
            "ROUND(SUM(CASE WHEN C.id_tipo_titolo = 1 THEN ammontare ELSE 0 END), 2) AS Azioni, " +
            "round(sum(case when C.id_tipo_titolo = 2 THEN ammontare else 0 end), 2) AS Obbligazioni, " +
            "round(sum(case when (C.id_tipo_titolo = 4 OR C.id_tipo_titolo = 5 OR C.id_tipo_titolo = 6 OR C.id_tipo_titolo = 8) AND A.id_gestione <> 7 THEN ammontare else 0 end), 2) AS ETF, " +
            "round(sum(case when C.id_tipo_titolo = 7 THEN ammontare else 0 end), 2) AS Fondo, " +
            "round(sum(case when C.id_tipo_titolo = 13 or C.id_tipo_titolo = 4 THEN ammontare else 0 end), 2) AS Volatili, " +
            "round(sum(case when A.id_tipo_movimento = 8 THEN ammontare else 0 end), 2) AS Costi, " +
            "round(sum(case when (C.id_tipo_titolo >= 1 OR A.id_tipo_movimento = 8) THEN ammontare else 0 end), 2) AS Totale " +
            "FROM conto_corrente A, gestioni B, titoli C, tipo_soldi D " +
            "WHERE A.id_gestione = B.id_gestione AND A.id_titolo = C.id_titolo AND A.id_tipo_soldi = D.id_tipo_soldi AND " +
            "A.id_tipo_soldi > 1 AND {0} " +
            "GROUP BY Anno, A.id_gestione, A.id_tipo_soldi, C.desc_titolo, C.isin " +
            "ORDER BY Anno DESC, A.id_gestione, A.id_tipo_soldi DESC) AS AAA WHERE Totale <> 0;";

        public static readonly string GetMovementDetailed = "SELECT G.nome_gestione, B.desc_conto, C.desc_movimento, E.desc_tipo_titolo, D.desc_titolo, D.isin, F.desc_tipo_soldi, " +
            "data_movimento, ROUND(case when ammontare < 0 then ammontare ELSE 0 END , 2) AS uscite, ROUND (case when ammontare > 0 then ammontare ELSE 0 END, 2) AS entrate, causale " +
            "FROM conto_corrente A, conti B, tipo_movimento C, titoli D, tipo_titoli E, tipo_soldi F, gestioni G WHERE A.id_conto = B.id_conto AND A.id_tipo_movimento = C.id_tipo_movimento AND " +
            "A.id_titolo = D.id_titolo AND D.id_tipo_titolo = E.id_tipo_titolo AND A.id_tipo_soldi = F.id_tipo_soldi AND A.id_gestione = G.id_gestione " +
            "AND A.id_gestione = @id_gestione AND A.id_titolo = @id_titolo ORDER BY data_movimento DESC, A.id_titolo, A.id_tipo_soldi";

        public static readonly string GetActiveAsset = "SELECT nome_gestione, desc_conto, desc_tipo_titolo, desc_titolo, isin, n_titoli, rimanenze * -1 AS costoTotale, " +
            "ROUND(CASE WHEN id_tipo_titolo = 2 THEN (rimanenze* -1) / n_titoli* 100 ELSE(rimanenze* -1) / n_titoli END, 4) AS CMC, note FROM " +
            "( SELECT G.nome_gestione, B.desc_conto, E.id_tipo_titolo, E.desc_tipo_titolo, D.desc_titolo, D.isin, SUM(shares_quantity) AS n_titoli, " +
            "ROUND(sum(ammontare + (total_commission + tobin_tax + disaggio_cedole) * -1 ) , 2) AS rimanenze, A.note FROM portafoglio_titoli A, conti B, titoli D, tipo_titoli E, gestioni G " +
            "WHERE A.id_conto = B.id_conto AND A.id_titolo = D.id_titolo AND D.id_tipo_titolo = E.id_tipo_titolo AND  A.id_gestione = G.id_gestione AND " +
            "{0} AND {1} GROUP BY G.nome_gestione, B.desc_conto, E.desc_tipo_titolo, D.isin " +
            "ORDER BY A.id_gestione, A.id_conto, E.id_tipo_titolo, D.desc_titolo) AS AA WHERE n_titoli > 0";

        public static readonly string QuoteInvGeoSettori = "SELECT Totale, Azione/Totale AS Azioni, Obbligazioni/Totale AS Obbligazioni, Liquidita/Totale AS Liquidita, Altro/Totale AS Altro, " +
            "Usa/Totale AS USA, Canada/Totale AS Canada, AmericaLatinaCentrale/Totale AS AmericaLatinaCentrale, RegnoUnito/Totale AS RegnoUnito, EuropaOccEuro/Totale AS EuropaOccEuro, " +
            "EuropaOccNoEuro/Totale AS EuropaOccNoEuro, EuropaEst/Totale AS EuropaEst, Africa/Totale AS Africa, MedioOriente/Totale AS MedioOriente, Giappone/Totale AS Giappone, " +
            "Australasia/Totale AS Australasia, AsiaSviluppati/Totale AS AsiaSviluppati, AsiaEmergenti/Totale AS AsiaEmergenti, GlobaleRegioni/Totale AS RegioniND, " +
            "MateriePrime/Totale AS MateriePrime, BeniConsCiclici/Totale AS BeniConsCiclici, Finanza/Totale AS Finanza, Immobiliare/Totale AS Immobiliare, " +
            "BeniConsDifensivi/Totale AS BeniConsDifensivi, Salute/Totale AS Salute, ServiziPubbUtility/Totale AS ServiziPubbUtility, ServiziComunic/Totale AS ServiziComunic, " +
            "Energia/Totale AS Energia, BeniIndustriali/Totale AS BeniIndustriali, Tecnologia/Totale AS Tecnologia, GlobaleSettori/Totale AS SettoriND " +
            "FROM (" +
            "SELECT nome_gestione, SUM(investimento) AS Totale, SUM(azioni) AS Azione, SUM(obbligazioni) AS Obbligazioni, SUM(liquidita) AS Liquidita, SUM(altro) AS Altro, " +
            "SUM(USA) AS Usa, SUM(Canada) AS Canada, SUM(AmericaLatinaCentrale) AS AmericaLatinaCentrale,	SUM(RegnoUnito) AS RegnoUnito, SUM(EuropaOccEuro) AS EuropaOccEuro, " +
            "SUM(EuropaOccNoEuro) AS EuropaOccNoEuro, SUM(EuropaEst) AS EuropaEst, SUM(Africa) AS Africa, SUM(MedioOriente) AS MedioOriente, SUM(Giappone) AS Giappone, " +
            "SUM(Australasia) AS Australasia, SUM(AsiaSviluppati) AS AsiaSviluppati, SUM(AsiaEmergenti) AS AsiaEmergenti, SUM(GlobaleRegioni) AS GlobaleRegioni, " +
            "SUM(MateriePrime) AS MateriePrime, SUM(BeniConsCiclici) AS BeniConsCiclici, SUM(Finanza) AS Finanza, SUM(Immobiliare) AS Immobiliare, " +
            "SUM(BeniConsDifensivi) AS BeniConsDifensivi, SUM(Salute) AS Salute, SUM(ServiziPubbUtility) AS ServiziPubbUtility, SUM(ServiziComunic) AS ServiziComunic, " +
            "SUM(energia) AS Energia, SUM(BeniIndustriali) AS BeniIndustriali, SUM(Tecnologia) AS Tecnologia, SUM(GlobaleSettori) AS GlobaleSettori " +
            "FROM (" +
            "SELECT F.nome_gestione, E.desc_tipo_titolo, B.desc_titolo, B.isin, ammontare * -1 AS investimento, " +
            "azioni * ammontare / 100 * -1 AS azioni, obbligazioni * ammontare / 100 * -1 AS obbligazioni, liquidita * ammontare / 100 * -1 AS liquidita, " +
            "altro * ammontare / 100 * -1 AS altro, USA * ammontare / 100 * -1 AS USA, Canada * ammontare / 100 * -1 AS Canada, " +
            "AmericaLatinaCentrale * ammontare / 100 * -1 AS AmericaLatinaCentrale, RegnoUnito * ammontare / 100 * -1 AS RegnoUnito, " +
            "EuropaOccEuro * ammontare / 100 * -1 AS EuropaOccEuro, EuropaOccNoEuro * ammontare / 100 * -1 AS EuropaOccNoEuro, EuropaEst * ammontare / 100 * -1 AS EuropaEst, " +
            "Africa * ammontare / 100 * -1 AS Africa, MedioOriente * ammontare / 100 * -1 AS MedioOriente, Giappone * ammontare / 100 * -1 AS Giappone, " +
            "Australasia * ammontare / 100 * -1 AS Australasia, AsiaSviluppati * ammontare / 100 * -1 AS AsiaSviluppati, AsiaEmergenti * ammontare / 100 * -1 AS AsiaEmergenti, " +
            "RegioniND * ammontare / 100 * -1 AS GlobaleRegioni, MateriePrime * ammontare / 100 * -1 AS MateriePrime, BeniConsCiclici * ammontare / 100 * -1 AS BeniConsCiclici, " +
            "Finanza * ammontare / 100 * -1 AS Finanza, Immobiliare * ammontare / 100 * -1 AS Immobiliare, BeniConsDifensivi * ammontare / 100 * -1 AS BeniConsDifensivi, " +
            "Salute * ammontare / 100 * -1 AS Salute, ServiziPubbUtility * ammontare / 100 * -1 AS ServiziPubbUtility, ServiziComunic * ammontare / 100 * -1 AS ServiziComunic, " +
            "Energia * ammontare / 100 * -1 AS Energia, BeniIndustriali * ammontare / 100 * -1 AS BeniIndustriali, Tecnologia * ammontare / 100 * -1 AS Tecnologia, " +
            "SettoriND * ammontare / 100 * -1 AS GlobaleSettori " +
            "FROM portafoglio_titoli A, titoli B, tipo_titoli E, gestioni F " +
            "WHERE A.id_titolo = B.id_titolo AND B.id_tipo_titolo = E.id_tipo_titolo AND A.id_gestione = F.id_gestione AND ({0}) ) AS XYZ " +
            " ) AS ABC";

        public static readonly string GetDeltaPerYear = "SELECT id_gestione, nome_gestione, GuadagniAnno1, GuadagniAnno2, GuadagniAnno2 - GuadagniAnno1 AS Differenza, " +
            "(GuadagniAnno2 - GuadagniAnno1)/GuadagniAnno1 AS Delta FROM ( SELECT A.id_gestione, B.nome_gestione, " +
            "SUM(CASE WHEN YEAR(A.data_movimento) = @Anno1 THEN CASE WHEN A.id_tipo_soldi = 11 THEN A.ammontare * -1 ELSE A.ammontare END ELSE 0 END) AS GuadagniAnno1, " +
            "SUM(CASE WHEN YEAR(A.data_movimento) = @Anno2 THEN CASE WHEN A.id_tipo_soldi = 11 THEN A.ammontare * -1 ELSE A.ammontare END ELSE 0 END) AS GuadagniAnno2 " +
            "FROM conto_corrente A, gestioni B WHERE A.id_gestione = B.id_gestione AND (A.id_tipo_soldi = 11 OR A.id_tipo_soldi = 15 OR A.id_tipo_soldi = 16) " +
            "{0} " +
            "GROUP BY A.id_gestione ORDER BY nome_gestione) A;";

        public static readonly string GetDeltaPerMonth = "SELECT id_gestione, nome_gestione, Mese, GuadagniAnno1, GuadagniAnno2, GuadagniAnno2 - GuadagniAnno1 AS Differenza, " +
            "(GuadagniAnno2 - GuadagniAnno1)/GuadagniAnno1 AS Delta FROM ( SELECT A.id_gestione, B.nome_gestione,  MONTH(A.data_movimento) AS Mese, " +
            "SUM(CASE WHEN YEAR(A.data_movimento) = @Anno1 THEN CASE WHEN A.id_tipo_soldi = 11 THEN A.ammontare * -1 ELSE A.ammontare END ELSE 0 END) AS GuadagniAnno1, " +
            "SUM(CASE WHEN YEAR(A.data_movimento) = @Anno2 THEN CASE WHEN A.id_tipo_soldi = 11 THEN A.ammontare * -1 ELSE A.ammontare END ELSE 0 END) AS GuadagniAnno2 " +
            "FROM conto_corrente A, gestioni B WHERE A.id_gestione = B.id_gestione AND (A.id_tipo_soldi = 11 OR A.id_tipo_soldi = 15 OR A.id_tipo_soldi = 16) " +
            "{0} " +
            "GROUP BY A.id_gestione, Mese ORDER BY nome_gestione, Mese) A;";

        public static readonly string GetDeltaPerYearTot = "SELECT id_gestione, nome_gestione, GuadagniAnno1, GuadagniAnno2, GuadagniAnno2 - GuadagniAnno1 AS Differenza, " +
                    "(GuadagniAnno2 - GuadagniAnno1)/GuadagniAnno1 AS Delta FROM ( SELECT 0 as id_gestione, 'Totale' as nome_gestione, " +
                    "SUM(CASE WHEN YEAR(A.data_movimento) = @Anno1 THEN CASE WHEN A.id_tipo_soldi = 11 THEN A.ammontare * -1 ELSE A.ammontare END ELSE 0 END) AS GuadagniAnno1, " +
                    "SUM(CASE WHEN YEAR(A.data_movimento) = @Anno2 THEN CASE WHEN A.id_tipo_soldi = 11 THEN A.ammontare * -1 ELSE A.ammontare END ELSE 0 END) AS GuadagniAnno2 " +
                    "FROM conto_corrente A WHERE (A.id_tipo_soldi = 11 OR A.id_tipo_soldi = 15 OR A.id_tipo_soldi = 16) " +
                    "{0} ) A; ";

        public static readonly string GetDeltaPerMonthTot = "SELECT id_gestione, nome_gestione, Mese, GuadagniAnno1, GuadagniAnno2, GuadagniAnno2 - GuadagniAnno1 AS Differenza, " +
            "(GuadagniAnno2 - GuadagniAnno1)/GuadagniAnno1 AS Delta FROM ( SELECT 0 as id_gestione, 'Totale' as nome_gestione,  MONTH(A.data_movimento) AS Mese, " +
            "SUM(CASE WHEN YEAR(A.data_movimento) = @Anno1 THEN CASE WHEN A.id_tipo_soldi = 11 THEN A.ammontare * -1 ELSE A.ammontare END ELSE 0 END) AS GuadagniAnno1, " +
            "SUM(CASE WHEN YEAR(A.data_movimento) = @Anno2 THEN CASE WHEN A.id_tipo_soldi = 11 THEN A.ammontare * -1 ELSE A.ammontare END ELSE 0 END) AS GuadagniAnno2 " +
            "FROM conto_corrente A WHERE (A.id_tipo_soldi = 11 OR A.id_tipo_soldi = 15 OR A.id_tipo_soldi = 16) " +
            "{0} " +
            "GROUP BY Mese ORDER BY Mese) A;";
    }
}

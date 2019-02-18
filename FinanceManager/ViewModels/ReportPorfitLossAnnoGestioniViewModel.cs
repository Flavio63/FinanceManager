using FinanceManager.Events;
using FinanceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.ViewModels
{
    public class ReportPorfitLossAnnoGestioniViewModel : ViewModelBase
    {
        ReportProfitLossList reportProfitLosses;
        public ReportPorfitLossAnnoGestioniViewModel(ReportProfitLossList reportProfitLossList)
        {
            reportProfitLosses = reportProfitLossList ?? throw new ArgumentNullException("Mancano i dati per la costruzione del report.");
            AddTotals(reportProfitLosses);
        }

        private void AddTotals(ReportProfitLossList RPLL)
        {
            ReportProfitLoss TotalProfitLoss = new ReportProfitLoss();
            for (int row = 0; row < RPLL.Count; row++)
            {
                int Rrow = row;
                int Anno = RPLL[Rrow].Anno;
                double Azioni = 0;
                double Obbligazione = 0;
                double Etf = 0;
                double Fondo = 0;
                double Volatili = 0;
                double Totali = 0;
                string gestione = RPLL[Rrow].Gestione;
                do
                {
                    Azioni += RPLL[Rrow].Azioni;
                    Obbligazione += RPLL[Rrow].Obbligazioni;
                    Etf += RPLL[Rrow].ETF;
                    Fondo += RPLL[Rrow].Fondo;
                    Volatili += RPLL[Rrow].Volatili;
                    Totali += RPLL[Rrow].Totale;
                    Rrow++;
                    if (Rrow >= RPLL.Count) break;
                } while (RPLL[Rrow].Gestione == gestione && RPLL[Rrow].Anno == Anno);
                if (Rrow - 1 > row)
                {
                    TotalProfitLoss.Anno = Anno;
                    TotalProfitLoss.Gestione = gestione;
                    TotalProfitLoss.TipoSoldi = "Totale";
                    TotalProfitLoss.Azioni = Azioni;
                    TotalProfitLoss.Obbligazioni = Obbligazione;
                    TotalProfitLoss.ETF = Etf;
                    TotalProfitLoss.Fondo = Fondo;
                    TotalProfitLoss.Volatili = Volatili;
                    TotalProfitLoss.Totale = Totali;
                    RPLL.Insert(Rrow, TotalProfitLoss);
                    row = Rrow; // - 1;
                }
            }
            ProfitLossData = RPLL;
        }

        public ReportProfitLossList ProfitLossData
        {
            get { return GetValue(() => ProfitLossData); }
            private set { SetValue(() => ProfitLossData, value); }
        }
    }
}

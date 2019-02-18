using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per ReportProfitLossAnnoGestioneView.xaml
    /// </summary>
    public partial class ReportProfitLossAnnoGestioneView : UserControl
    {
        public ReportProfitLossAnnoGestioneView(ReportPorfitLossAnnoGestioniViewModel dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
        }
    }
}

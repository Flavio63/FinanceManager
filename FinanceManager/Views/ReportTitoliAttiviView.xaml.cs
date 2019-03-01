using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per ReportTitoliAttiviView.xaml
    /// </summary>
    public partial class ReportTitoliAttiviView : UserControl
    {
        public ReportTitoliAttiviView(ReportTitoliAttiviViewModel dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
        }
    }
}

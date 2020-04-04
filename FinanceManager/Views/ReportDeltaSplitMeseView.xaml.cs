using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per ReportDeltaSplitMeseView.xaml
    /// </summary>
    public partial class ReportDeltaSplitMeseView : UserControl
    {
        public ReportDeltaSplitMeseView(ReportDeltaSplitMeseViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

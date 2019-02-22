using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per ReportMovementDetailedView.xaml
    /// </summary>
    public partial class ReportMovementDetailedView : UserControl
    {
        public ReportMovementDetailedView(ReportMovementDetailedViewModel dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
        }
    }
}

using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per Guadagni.xaml
    /// </summary>
    public partial class ReportGuadagniView : UserControl
    {
        public ReportGuadagniView(ReportGuadagniViewModel dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
        }
    }
}

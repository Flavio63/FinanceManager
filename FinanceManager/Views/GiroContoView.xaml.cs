using System.Windows.Controls;
using FinanceManager.ViewModels;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per GiroContoView.xaml
    /// </summary>
    public partial class GiroContoView : UserControl
    {
        public GiroContoView(GiroContoViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

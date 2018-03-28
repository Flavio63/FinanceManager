using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per RegistryMarketView.xaml
    /// </summary>
    public partial class RegistryMarketView : UserControl
    {
        public RegistryMarketView(RegistryMarketViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

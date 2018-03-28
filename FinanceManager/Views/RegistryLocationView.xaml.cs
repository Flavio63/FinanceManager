using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per RegistryLocationView.xaml
    /// </summary>
    public partial class RegistryLocationView : UserControl
    {
        public RegistryLocationView(RegistryLocationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

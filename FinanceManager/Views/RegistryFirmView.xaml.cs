using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per RegistryFirmView.xaml
    /// </summary>
    public partial class RegistryFirmView : UserControl
    {
        public RegistryFirmView(RegistryFirmViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

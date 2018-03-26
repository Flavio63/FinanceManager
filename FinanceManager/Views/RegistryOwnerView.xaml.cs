using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per RegistryOwnerView.xaml
    /// </summary>
    public partial class RegistryOwnerView : UserControl
    {
        public RegistryOwnerView(RegistryOwnerViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}

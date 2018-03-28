using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per RegistryShareView.xaml
    /// </summary>
    public partial class RegistryShareView : UserControl
    {
        public RegistryShareView(RegistryShareViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

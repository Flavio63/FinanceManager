using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per RegistryShareTypeView.xaml
    /// </summary>
    public partial class RegistryShareTypeView : UserControl
    {
        public RegistryShareTypeView(RegistryShareTypeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

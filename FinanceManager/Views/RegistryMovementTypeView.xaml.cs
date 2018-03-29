using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per RegistryMovementTypeView.xaml
    /// </summary>
    public partial class RegistryMovementTypeView : UserControl
    {
        public RegistryMovementTypeView(RegistryMovementTypeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

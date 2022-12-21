using System.Windows.Controls;
using FinanceManager.ViewModels;
namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per RegistryContoCorrenteView.xaml
    /// </summary>
    public partial class RegistryContoCorrenteView : UserControl
    {
        public RegistryContoCorrenteView(RegistryContoCorrenteViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

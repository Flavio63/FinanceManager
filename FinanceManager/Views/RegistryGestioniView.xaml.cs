using System.Windows.Controls;
using FinanceManager.ViewModels;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per RegistryGestioniView.xaml
    /// </summary>
    public partial class RegistryGestioniView : UserControl
    {
        public RegistryGestioniView(RegistryGestioniViewModel viewModel)
        {
            InitializeComponent();
            DataContext= viewModel;
        }
    }
}

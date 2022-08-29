using System.Windows.Controls;
using FinanceManager.ViewModels;


namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per CapitalsRegisterView.xaml
    /// </summary>
    public partial class CapitalsRegisterView : UserControl
    {
        public CapitalsRegisterView(CapitalsRegisterViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

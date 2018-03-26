using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per RegistryCurrencyView.xaml
    /// </summary>
    public partial class RegistryCurrencyView : UserControl
    {
        public RegistryCurrencyView(RegistryCurrencyViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

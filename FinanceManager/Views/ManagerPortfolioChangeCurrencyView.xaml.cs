using FinanceManager.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per ManagerPortfolioChangeCurrencyView.xaml
    /// </summary>
    public partial class ManagerPortfolioChangeCurrencyView : UserControl
    {
        public ManagerPortfolioChangeCurrencyView(ManagerPortfolioChangeCurrencyViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

    }
}

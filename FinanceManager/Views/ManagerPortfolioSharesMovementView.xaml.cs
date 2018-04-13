using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per ManagerPortfolioSharesMovementView.xaml
    /// </summary>
    public partial class ManagerPortfolioSharesMovementView : UserControl
    {
        public ManagerPortfolioSharesMovementView(ManagerPortfolioSharesMovementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

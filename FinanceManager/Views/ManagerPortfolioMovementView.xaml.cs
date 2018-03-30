using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per ManagerPortfolioMovement.xaml
    /// </summary>
    public partial class ManagerPortfolioMovementView : UserControl
    {
        public ManagerPortfolioMovementView(ManagerPortfolioMovementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

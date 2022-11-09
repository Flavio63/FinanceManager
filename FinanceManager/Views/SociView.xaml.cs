using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per SociView.xaml
    /// </summary>
    public partial class SociView : UserControl
    {
        public SociView(SociViewModels viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

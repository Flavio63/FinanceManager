using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per ContiSociGestioniView.xaml
    /// </summary>
    public partial class ContiSociGestioniView : UserControl
    {
        public ContiSociGestioniView(ContiSociGestioniViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

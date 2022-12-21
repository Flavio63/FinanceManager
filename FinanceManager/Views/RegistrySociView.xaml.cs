using FinanceManager.ViewModels;
using FinanceManager.Events;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per RegistrySociView.xaml
    /// </summary>
    public partial class RegistrySociView : UserControl
    {
        internal RegistrySociView(RegistrySociViewModel viewModel)
        {
            InitializeComponent();
            DataContext= viewModel;
        }
    }
}

using FinanceManager.ViewModels;

using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per TabControlSintesiView.xaml
    /// </summary>
    public partial class TabControlSintesiView : UserControl
    {
        public TabControlSintesiView(TabControlSintesiViewModel dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
        }
    }
}

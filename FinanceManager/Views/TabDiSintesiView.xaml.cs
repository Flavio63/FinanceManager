using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per TabDiSintesiView.xaml
    /// </summary>
    public partial class TabDiSintesiView : UserControl
    {
        public TabDiSintesiView(TabDiSintesiViewModel dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
        }
    }
}

using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per AnalisiPortafoglioView.xaml
    /// </summary>
    public partial class AnalisiPortafoglioView : UserControl
    {
        public AnalisiPortafoglioView(AnalisiPortafoglioViewModel dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
        }
    }
}

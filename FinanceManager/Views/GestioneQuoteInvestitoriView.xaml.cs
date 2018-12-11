using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per GestioneQuoteInvestitoriView.xaml
    /// </summary>
    public partial class GestioneQuoteInvestitoriView : UserControl
    {
        public GestioneQuoteInvestitoriView(GestioneQuoteInvestitoriViewModel dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
        }
    }
}

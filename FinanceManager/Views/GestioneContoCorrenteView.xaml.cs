using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per GestioneContoCorrenteView.xaml
    /// </summary>
    public partial class GestioneContoCorrenteView : UserControl
    {
        public GestioneContoCorrenteView(GestioneContoCorrenteViewModel dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
        }
    }
}

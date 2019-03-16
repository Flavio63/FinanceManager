using FinanceManager.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per SchedeTitoliView.xaml
    /// </summary>
    public partial class SchedeTitoliView : UserControl
    {
        public SchedeTitoliView(SchedeTitoliViewModel schedeTitoliViewModel)
        {
            InitializeComponent();
            DataContext = schedeTitoliViewModel;
        }
    }
}

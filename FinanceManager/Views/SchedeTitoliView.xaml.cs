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
#if DEBUG
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
#endif
            InitializeComponent();
            DataContext = schedeTitoliViewModel;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per ReportTrendAnno.xaml
    /// </summary>
    public partial class ReportTrendAnno : UserControl
    {
        public ReportTrendAnno()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty Desc_AnnoProperty = DependencyProperty.Register("Desc_Anno", typeof(string), typeof(ReportGuadagniAnnoView),
           new UIPropertyMetadata(string.Empty));

        public string Desc_Anno
        {
            get { return (string)GetValue(Desc_AnnoProperty); }
            set { SetValue(Desc_AnnoProperty, value); }
        }

    }
}

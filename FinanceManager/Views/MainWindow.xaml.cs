using System.Windows;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double _Altezza;

        public MainWindow()
        {
            InitializeComponent();
        }

        public double Altezza
        {
            get { return _Altezza; }
            set { _Altezza = value; }
        }


    }
}

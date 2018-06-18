using System.Windows;

namespace MultiSelectDemo
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel model = new ViewModel();
            DataContext = model;
        }
    }
}

using FinanceManager.Services;
using FinanceManager.ViewModels;
using FinanceManager.Views;
using System.Windows;

namespace FinanceManager
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            MainWindowView dataContext = new MainWindowView();
            InitializeComponent();
            this.DataContext = dataContext;
        }

    }
}

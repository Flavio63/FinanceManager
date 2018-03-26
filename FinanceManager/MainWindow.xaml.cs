﻿using FinanceManager.Services;
using FinanceManager.ViewModels;
using FinanceManager.Views;
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

namespace FinanceManager
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IRegistryServices _registryServices;

        public MainWindow()
        {
            InitializeComponent();
            _registryServices = new RegistryService();
        }

        private void OnClickGestioni(object sender, RoutedEventArgs e)
        {
            RegistryOwnerViewModel ownerViewModel = new RegistryOwnerViewModel(_registryServices);
            RegistryOwnerView ownerView = new RegistryOwnerView(ownerViewModel);
            mainGrid.Children.Add(ownerView);
        }

        private void OnClickTipoTitoli(object sender, RoutedEventArgs e)
        {
            RegistryShareTypeViewModel registryShareTypeViewModel = new RegistryShareTypeViewModel(_registryServices);
            RegistryShareTypeView shareTypeView = new RegistryShareTypeView(registryShareTypeViewModel);
            mainGrid.Children.Add(shareTypeView);
        }

        private void OnClickCurrency(object sender, RoutedEventArgs e)
        {
            RegistryCurrencyViewModel registryCurrencyViewModel = new RegistryCurrencyViewModel(_registryServices);
            RegistryCurrencyView currencyView = new RegistryCurrencyView(registryCurrencyViewModel);
            mainGrid.Children.Add(currencyView);
        }

    }
}

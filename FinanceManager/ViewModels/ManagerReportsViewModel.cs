using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class ManagerReportsViewModel : ViewModelBase
    {
        private IRegistryServices _services;
        private IManagerReportServices _reportServices;

        public ICommand CloseMeCommand { get; set; }
        public ICommand ViewCommand { get; set; }
        public ICommand DownloadCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        private ObservableCollection<RegistryShare> _SharesList;
        Predicate<object> _Filter;

        private IList<int> _selectedOwners;
        private double[] exchangeValue;

        public ManagerReportsViewModel(IRegistryServices registryServices, IManagerReportServices managerReportServices)
        {
            _services = registryServices ?? throw new ArgumentNullException("ManagerReportsViewModel with no registry services");
            _reportServices = managerReportServices ?? throw new ArgumentNullException("ManagerReportsViewModel with no report services");
            CloseMeCommand = new CommandHandler(CloseMe);
            ViewCommand = new CommandHandler(ViewReport, CanDoReport);
            ClearCommand = new CommandHandler(ClearReport, CanClearReport);
            SetUpViewModel();
        }

        private void SetUpViewModel()
        {
            try
            {
                OwnerList = _services.GetGestioneList();
                _selectedOwners = new List<int>();
                AvailableYears = _reportServices.GetAvailableYears();
                SelectedYears = new List<int>();
                SharesList = new ObservableCollection<RegistryShare>(_services.GetRegistryShareList());
                _Filter = new Predicate<object>(Filter);
                ReportSelezionato = "";
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "ManagerReportsView", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        #region events
        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox LB = sender as ListBox;
            if (LB != null)
            {
                switch (LB.Name)
                {
                    case "Conto":
                        _selectedOwners.Clear();
                        foreach (RegistryOwner item in LB.SelectedItems)
                            _selectedOwners.Add(item.Id_gestione);
                        break;
                    case "Anni":
                        SelectedYears.Clear();
                        foreach (int y in LB.SelectedItems)
                            SelectedYears.Add(y);
                        break;
                }
            }
        }

        public void IsChecked(object sender, RoutedEventArgs e)
        {
            ReportSelezionato = ((RadioButton)sender).Name;
        }
        /// <summary>
        /// E' il filtro da applicare all'elenco delle azioni
        /// e contestualmente al datagrid sottostante
        /// </summary>
        public bool Filter(object obj)
        {
            if (obj != null)
            {
                if (obj.GetType() == typeof(RegistryShare))
                {
                    var data = obj as RegistryShare;
                    if (!string.IsNullOrEmpty(SrchShares))
                        return data.Isin.ToUpper().Contains(SrchShares.ToUpper());
                }
            }
            return true;
        }

        #endregion events

        #region Getter&Setter
        /// <summary>
        /// La ricerca degli isin dei titoli per l'acquisto / vendita
        /// </summary>
        public string SrchShares
        {
            get { return GetValue(() => SrchShares); }
            set
            {
                SetValue(() => SrchShares, value);
                SharesListView.Filter = _Filter;
                SharesListView.Refresh();
            }
        }
        
        /// <summary>
        /// Elenco con i titoli disponibili
        /// </summary>
        private ObservableCollection<RegistryShare> SharesList
        {
            get { return _SharesList; }
            set
            {
                _SharesList = value;
                SharesListView = new ListCollectionView(value);
            }
        }

        /// <summary>
        /// Combo box con i titoli da selezionare filtrato da SrchShares
        /// </summary>
        public ListCollectionView SharesListView
        {
            get { return GetValue(() => SharesListView); }
            set { SetValue(() => SharesListView, value); }
        }

        public RegistryOwnersList OwnerList
        {
            get { return GetValue(() => OwnerList); }
            set { SetValue(() => OwnerList, value); }
        }
        public RegistryOwnersList SelectedOwner
        {
            get { return GetValue(() => SelectedOwner); }
            set { SetValue(() => SelectedOwner, value); }
        }
        public IList<int> AvailableYears
        {
            get { return GetValue(() => AvailableYears); }
            set { SetValue(() => AvailableYears, value); }
        }

        public ReportProfitLossList ReportProfitLosses
        {
            get { return GetValue(() => ReportProfitLosses); }
            set { SetValue(() => ReportProfitLosses, value); }
        }

        public IList<int> SelectedYears
        {
            get { return GetValue(() => SelectedYears); }
            set { SetValue(() => SelectedYears, value); }
        }

        public string ReportSelezionato
        {
            get { return GetValue(() => ReportSelezionato); }
            set { SetValue(() => ReportSelezionato, value); }
        }
        #endregion Getter&Setter

        #region command
        public void CloseMe(object param)
        {
            ManagerReportsView MRV = param as ManagerReportsView;
            DockPanel wp = MRV.Parent as DockPanel;
            wp.Children.Remove(MRV);
        }

        public bool CanDoReport(object param)
        {
            Grid grid = param as Grid;
            if (_selectedOwners.Count() > 0 && SelectedYears.Count() > 0 && !string.IsNullOrEmpty(ReportSelezionato))
                return true;
            return false;
        }

        public bool CanClearReport(object param)
        {
            //Grid grid = param as Grid;
            //if (grid.RowDefinitions.Count > 0)
            //    return true;
            return false;
        }

        public void ClearReport(object param)
        {
            Grid grid = param as Grid;
            grid.Children.Clear();
            for (int r = grid.RowDefinitions.Count - 1; r >= 0; r--)
            {
                grid.RowDefinitions.RemoveAt(r);
            }
            SetUpViewModel();
        }

        public void ViewReport(object param)
        {
            Border border = param as Border;

            switch (ReportSelezionato)
            {
                case "PL":
                    ReportPorfitLossAnnoGestioniViewModel DataReport = new ReportPorfitLossAnnoGestioniViewModel(_reportServices.GetReport1(_selectedOwners, SelectedYears));
                    ReportProfitLossAnnoGestioneView report1 = new ReportProfitLossAnnoGestioneView(DataReport);
                    border.Child = report1;
                    break;
                case "Delta":
                case "Scalare":
                case "Titolo":
                default:
                    break;
            }
        }

        #endregion command

    }
}

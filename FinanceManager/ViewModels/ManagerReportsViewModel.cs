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
                TitoloSelezionato = 0;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "ManagerReportsView", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        #region events
        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox LB)
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
                        {
                            SelectedYears.Add(y);
                        }
                        break;
                }
            }
            if (sender is ComboBox CB)
                TitoloSelezionato =(int)(((RegistryShare)CB.SelectedItem).IdShare);
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
        
        public bool CanClear { get; set; }

        public int TitoloSelezionato
        {
            get { return GetValue(() => TitoloSelezionato); }
            private set { SetValue(() => TitoloSelezionato, value); }
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
            if (_selectedOwners.Count() > 0 && SelectedYears.Count() > 0 && !string.IsNullOrEmpty(ReportSelezionato))
            {
                if (ReportSelezionato == "Titolo" && TitoloSelezionato == 0)
                    return false;
                return true;
            }
            return false;
        }

        public bool CanClearReport(object param)
        {
            if (CanClear)
                return true;
            return false;
        }

        public void ClearReport(object param)
        {
            UserControl userControl = param as UserControl;
            ((Border)userControl.FindName("BorderReport")).Child = null;
            ((ListBox)userControl.FindName("Anni")).SelectedIndex = -1;
            SetUpViewModel();
        }

        public void ViewReport(object param)
        {
            UserControl userControl = param as UserControl;
            Border border = ((Border)userControl.FindName("BorderReport"));
            switch (ReportSelezionato)
            {
                case "PL":
                    ReportPorfitLossAnnoGestioniViewModel ProfitLossData = new ReportPorfitLossAnnoGestioniViewModel(_reportServices.GetReport1(_selectedOwners, SelectedYears));
                    ReportProfitLossAnnoGestioneView report1 = new ReportProfitLossAnnoGestioneView(ProfitLossData);
                    border.Child = report1;
                    ((RadioButton)userControl.FindName(ReportSelezionato)).IsChecked = false;
                    CanClear = true;
                    break;
                case "Delta":
                case "Scalare":
                case "Titolo":
                    ReportMovementDetailedViewModel TitoloData = new ReportMovementDetailedViewModel(_reportServices.GetMovementDetailed(_selectedOwners[0], TitoloSelezionato));
                    ReportMovementDetailedView report2 = new ReportMovementDetailedView(TitoloData);
                    border.Child = report2;
                    ((RadioButton)userControl.FindName(ReportSelezionato)).IsChecked = false;
                    CanClear = true;
                    break;
                default:
                    break;
            }
        }

        #endregion command

    }
}

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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class ManagerReportsViewModel : ViewModelBase
    {
        private IRegistryServices _services;
        private IManagerReportServices _reportServices;
        private IManagerLiquidAssetServices _assetServices;

        public ICommand CloseMeCommand { get; set; }
        public ICommand ViewCommand { get; set; }
        public ICommand DownloadCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        private ObservableCollection<RegistryShare> _SharesList;
        Predicate<object> _Filter;

        private IList<RegistryOwner> _selectedOwners;
        private IList<int> _selectedAccount;

        public ManagerReportsViewModel(IRegistryServices registryServices, IManagerReportServices managerReportServices, IManagerLiquidAssetServices managerLiquidAssetServices)
        {
            _services = registryServices ?? throw new ArgumentNullException("ManagerReportsViewModel with no registry services");
            _reportServices = managerReportServices ?? throw new ArgumentNullException("ManagerReportsViewModel with no report services");
            _assetServices = managerLiquidAssetServices ?? throw new ArgumentNullException("ManagerLiquidAssetServices");
            CloseMeCommand = new CommandHandler(CloseMe);
            ViewCommand = new CommandHandler(ViewReport, CanDoReport);
            ClearCommand = new CommandHandler(ClearReport, CanClearReport);
            DownloadCommand = new CommandHandler(ExportReport, CanClearReport);
            SetUpViewModel();
        }

        #region private

        private void SetUpViewModel()
        {
            try
            {
                OwnerList = new RegistryOwnersList();
                RegistryOwnersList ListaOriginale = new RegistryOwnersList();
                ListaOriginale = _services.GetGestioneList();
                var LO = from risultato in ListaOriginale
                         where risultato.Tipologia == "Gestore"
                         select risultato;
                foreach (RegistryOwner registryOwner in LO)
                    OwnerList.Add(registryOwner);

                //OwnerList = _services.GetGestioneList();
                AccountList = _services.GetRegistryLocationList();
                _selectedOwners = new List<RegistryOwner>();
                _selectedAccount = new List<int>();
                AvailableYears = _reportServices.GetAvailableYears();
                SelectedYears = new List<int>();
                SharesList = new ObservableCollection<RegistryShare>(_services.GetRegistryShareList());
                _Filter = new Predicate<object>(Filter);
                ReportSelezionato = "";
                TitoloSelezionato = 0;
                CanClear = false;
            }
            catch (Exception err)
            {
                MessageBox.Show("Errore nella richiesta dei dati." + Environment.NewLine + err.Message, "DAF-C Gestione Report");
            }
        }

        private void DoTotals()
        {
            double Azioni;
            double Obbligazione;
            double Etf;
            double Fondo;
            double Volatili;
            double Costi;
            double Totali;

            switch (ReportSelezionato)
            {
                case "PL":
                    for (int row = 0; row < ReportProfitLosses.Count; row++)
                    {
                        int Rrow = row;
                        int Anno = ReportProfitLosses[Rrow].Anno;
                        Azioni = 0;
                        Obbligazione = 0;
                        Etf = 0;
                        Fondo = 0;
                        Volatili = 0;
                        Costi = 0;
                        Totali = 0;
                        string gestione = ReportProfitLosses[Rrow].Gestione;
                        do
                        {
                            Azioni += ReportProfitLosses[Rrow].Azioni;
                            Obbligazione += ReportProfitLosses[Rrow].Obbligazioni;
                            Etf += ReportProfitLosses[Rrow].ETF;
                            Fondo += ReportProfitLosses[Rrow].Fondo;
                            Volatili += ReportProfitLosses[Rrow].Volatili;
                            Costi += ReportProfitLosses[Rrow].Costi;
                            Totali += ReportProfitLosses[Rrow].Totale;
                            Rrow++;
                            if (Rrow >= ReportProfitLosses.Count) break;
                        } while (ReportProfitLosses[Rrow].Gestione == gestione && ReportProfitLosses[Rrow].Anno == Anno);
                        if (Rrow - 1 > row)
                        {
                            ReportProfitLoss TotalProfitLoss = new ReportProfitLoss();
                            TotalProfitLoss.Anno = Anno;
                            TotalProfitLoss.Gestione = gestione;
                            TotalProfitLoss.TipoSoldi = "";
                            TotalProfitLoss.NomeTitolo = string.Format("Totale {0} nel {1}.", gestione, Anno); ;
                            TotalProfitLoss.ISIN = "";
                            TotalProfitLoss.Azioni = Azioni;
                            TotalProfitLoss.Obbligazioni = Obbligazione;
                            TotalProfitLoss.ETF = Etf;
                            TotalProfitLoss.Fondo = Fondo;
                            TotalProfitLoss.Volatili = Volatili;
                            TotalProfitLoss.Costi = Costi;
                            TotalProfitLoss.Totale = Totali;
                            ReportProfitLosses.Insert(Rrow, TotalProfitLoss);
                            row = Rrow; // - 1;
                        }
                    }
                    break;
                case "DPL":
                    for (int Rrow = 0; Rrow < ReportProfitLosses.Count;)
                    {
                        int Anno = ReportProfitLosses[Rrow].Anno;
                        ReportProfitLoss TotaleAnno = new ReportProfitLoss();
                        int contAnno = 0;
                        while (Anno == ReportProfitLosses[Rrow].Anno) // ciclo anno
                        {
                            ReportProfitLoss TotaleGestione = new ReportProfitLoss();
                            string Gestione = ReportProfitLosses[Rrow].Gestione;
                            int contGestione = 0;
                            while (Anno == ReportProfitLosses[Rrow].Anno && Gestione == ReportProfitLosses[Rrow].Gestione)  // ciclo gestione
                            {
                                ReportProfitLoss TotaleTipoSoldi = new ReportProfitLoss();
                                string tipoSoldi = ReportProfitLosses[Rrow].TipoSoldi;
                                int contTipoSoldi = 0;
                                while (Anno == ReportProfitLosses[Rrow].Anno && Gestione == ReportProfitLosses[Rrow].Gestione
                                    && tipoSoldi == ReportProfitLosses[Rrow].TipoSoldi) // ciclo tiposoldi
                                {
                                    TotaleAnno.Azioni += ReportProfitLosses[Rrow].Azioni;
                                    TotaleAnno.Obbligazioni += ReportProfitLosses[Rrow].Obbligazioni;
                                    TotaleAnno.ETF += ReportProfitLosses[Rrow].ETF;
                                    TotaleAnno.Fondo += ReportProfitLosses[Rrow].Fondo;
                                    TotaleAnno.Volatili += ReportProfitLosses[Rrow].Volatili;
                                    TotaleAnno.Costi += ReportProfitLosses[Rrow].Costi;
                                    TotaleAnno.Totale += ReportProfitLosses[Rrow].Totale;
                                    TotaleGestione.Azioni += ReportProfitLosses[Rrow].Azioni;
                                    TotaleGestione.Obbligazioni += ReportProfitLosses[Rrow].Obbligazioni;
                                    TotaleGestione.ETF += ReportProfitLosses[Rrow].ETF;
                                    TotaleGestione.Fondo += ReportProfitLosses[Rrow].Fondo;
                                    TotaleGestione.Volatili += ReportProfitLosses[Rrow].Volatili;
                                    TotaleGestione.Costi += ReportProfitLosses[Rrow].Costi;
                                    TotaleGestione.Totale += ReportProfitLosses[Rrow].Totale;
                                    TotaleTipoSoldi.Azioni += ReportProfitLosses[Rrow].Azioni;
                                    TotaleTipoSoldi.Obbligazioni += ReportProfitLosses[Rrow].Obbligazioni;
                                    TotaleTipoSoldi.ETF += ReportProfitLosses[Rrow].ETF;
                                    TotaleTipoSoldi.Fondo += ReportProfitLosses[Rrow].Fondo;
                                    TotaleTipoSoldi.Volatili += ReportProfitLosses[Rrow].Volatili;
                                    TotaleTipoSoldi.Costi += ReportProfitLosses[Rrow].Costi;
                                    TotaleTipoSoldi.Totale += ReportProfitLosses[Rrow].Totale;
                                    contTipoSoldi++;
                                    if (Rrow + 1 >= ReportProfitLosses.Count) // verifico di non andare oltre
                                    {
                                        Rrow++; // incremento comunque il contatore per avere la stessa situazione in uscita dal ciclo
                                        break;
                                    }
                                    Rrow++;
                                } // fine ciclo tipo soldi
                                contGestione++;
                                if (contTipoSoldi > 1) // se il contatore è maggiore di 1 aggiungo riga x totali
                                {
                                    TotaleTipoSoldi.ISIN = "TOTALE TIPO SOLDI";
                                    TotaleTipoSoldi.NomeTitolo = "TOTALE TIPO SOLDI";
                                    TotaleTipoSoldi.TipoSoldi = tipoSoldi;
                                    TotaleTipoSoldi.Gestione = Gestione;
                                    TotaleTipoSoldi.Anno = Anno;
                                    ReportProfitLosses.Insert(Rrow, TotaleTipoSoldi);   // il contatore viene incrementato automaticamente
                                    Rrow++;                                                 // mi adeguo
                                }
                                else
                                {
                                    ReportProfitLosses[Rrow - 1].TipoSoldi += " TOTALE";
                                }
                                if (Rrow >= ReportProfitLosses.Count)
                                    break;
                            }   // fine ciclo gestione
                            contAnno++;
                            if (contGestione > 1)
                            {
                                TotaleGestione.ISIN = "TOTALE GESTIONE";
                                TotaleGestione.NomeTitolo = "TOTALE GESTIONE";
                                TotaleGestione.TipoSoldi = "TOTALE GESTIONE";
                                TotaleGestione.Gestione = Gestione;
                                TotaleGestione.Anno = Anno;
                                ReportProfitLosses.Insert(Rrow, TotaleGestione);      // il contatore viene incrementato automaticamente
                                Rrow++;                                                 // mi adeguo
                            }
                            else
                            {
                                ReportProfitLosses[Rrow  - 1].Gestione += " TOTALE";
                            }
                            if (Rrow >= ReportProfitLosses.Count)
                                break;
                        } // fine ciclo anno
                        if (contAnno > 1)
                        {
                            TotaleAnno.ISIN = "TOTALE ANNO";
                            TotaleAnno.NomeTitolo = "TOTALE ANNO";
                            TotaleAnno.TipoSoldi = "TOTALE ANNO";
                            TotaleAnno.Gestione = "TOTALE ANNO";
                            TotaleAnno.Anno = Anno;
                            ReportProfitLosses.Insert(Rrow, TotaleAnno);
                            Rrow++;
                        }
                        else
                        {
                            ReportProfitLosses[Rrow - 1].Gestione += " - TOTALE ANNO";
                        }
                    }
                    break;
            }

        }

        #endregion private

        #region events
        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox LB)
            {
                switch (LB.Name)
                {
                    case "Gestione":
                        _selectedOwners.Clear();
                        foreach (RegistryOwner item in LB.SelectedItems)
                            _selectedOwners.Add(item);
                        break;
                    case "Conto":
                        _selectedAccount.Clear();
                        foreach (RegistryLocation registryLocation in LB.SelectedItems)
                            _selectedAccount.Add(registryLocation.Id_Conto);
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
                TitoloSelezionato = (int)(((RegistryShare)CB.SelectedItem).id_titolo);
        }

        public void IsChecked(object sender, RoutedEventArgs e)
        {
            ReportSelezionato = ((RadioButton)sender).Name;
            switch (ReportSelezionato)
            {
                case "PL":
                case "DPL":
                case "DeltaMese":
                case "DeltaAnni":
                    YearsIsEnable = true;
                    break;
                default:
                    YearsIsEnable = false;
                    break;
            }
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
        public RegistryLocationList AccountList
        {
            get { return GetValue(() => AccountList); }
            private set { SetValue(() => AccountList, value); }
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
            private set { SetValue(() => ReportProfitLosses, value); }
        }

        public ReportMovementDetailedList ReportMovementDetaileds
        {
            get { return GetValue(() => ReportMovementDetaileds); }
            private set { SetValue(() => ReportMovementDetaileds, value); }
        }

        public ReportTitoliAttiviList ReportTitoliAttivis
        {
            get { return GetValue(() => ReportTitoliAttivis); }
            private set { SetValue(() => ReportTitoliAttivis, value); }
        }

        public ObservableCollection<AnalisiPortafoglio> GetAnalisiPortafoglio
        {
            get { return GetValue(() => GetAnalisiPortafoglio); }
            private set { SetValue(() => GetAnalisiPortafoglio, value); }
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

        public bool YearsIsEnable
        {
            get { return GetValue(() => YearsIsEnable); }
            private set { SetValue(() => YearsIsEnable, value); }
        }

        public GuadagnoPerQuoteList GuadagnoPerQuoteDettagliato
        {
            get { return GetValue(() => GuadagnoPerQuoteDettagliato); }
            set { SetValue(() => GuadagnoPerQuoteDettagliato, value); }
        }
        public GuadagnoPerQuoteList GuadagnoPerQuoteSintesi
        {
            get { return GetValue(() => GuadagnoPerQuoteSintesi); }
            set { SetValue(() => GuadagnoPerQuoteSintesi, value); }
        }
        public GuadagnoPerQuoteList GuadagnoPerQuote
        {
            get { return GetValue(() => GuadagnoPerQuote); }
            set { SetValue(() => GuadagnoPerQuote, value); }
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
            switch (ReportSelezionato)
            {
                case "PL":
                case "DPL":
                    if (_selectedOwners.Count() > 0 && SelectedYears.Count() > 0)
                        return true;
                    return false;
                case "Titolo":
                    if (_selectedOwners.Count() > 0 && TitoloSelezionato != 0)
                        return true;
                    return false;
                case "ElencoTitoliAttivi":
                    if (_selectedOwners.Count() > 0 && _selectedAccount.Count > 0)
                        return true;
                    return false;
                case "AnalisiPortafoglio":
                    if (_selectedOwners.Count() > 0)
                        return true;
                    return false;
                case "Guadagni":
                    return true;
                case "DeltaAnni":
                    if (_selectedOwners.Count() > 0 && SelectedYears.Count() >= 2)
                        return true;
                    return false;
                case "DeltaMese":
                    if (_selectedOwners.Count() > 0 && SelectedYears.Count() == 2)
                        return true;
                    return false;
                default:
                    return false;
            }
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
            ((ListBox)userControl.FindName("Gestione")).SelectedIndex = -1;
            SetUpViewModel();
        }

        public void ViewReport(object param)
        {
            UserControl userControl = param as UserControl;
            Border border = ((Border)userControl.FindName("BorderReport"));
            switch (ReportSelezionato)
            {
                case "PL":
                    ReportProfitLosses = _reportServices.GetReport1(_selectedOwners, SelectedYears);
                    DoTotals();
                    ReportPorfitLossAnnoGestioniViewModel ProfitLossData = new ReportPorfitLossAnnoGestioniViewModel(ReportProfitLosses, false);
                    ReportProfitLossAnnoGestioneView report1 = new ReportProfitLossAnnoGestioneView(ProfitLossData);
                    border.Child = report1;
                    ((RadioButton)userControl.FindName(ReportSelezionato)).IsChecked = false;
                    CanClear = true;
                    break;
                case "DPL":
                    ReportProfitLosses = _reportServices.GetReport1(_selectedOwners, SelectedYears, false);
                    DoTotals();
                    ReportPorfitLossAnnoGestioniViewModel ProfitLossDetailedData = new ReportPorfitLossAnnoGestioniViewModel(ReportProfitLosses, true);
                    ReportProfitLossAnnoGestioneView report1_1 = new ReportProfitLossAnnoGestioneView(ProfitLossDetailedData);
                    border.Child = report1_1;
                    ((RadioButton)userControl.FindName(ReportSelezionato)).IsChecked = false;
                    CanClear = true;
                    break;
                case "DeltaAnni":
                    break;
                case "DeltaMese":
                    ReportDeltaSplitMeseViewModel viewModel = new ReportDeltaSplitMeseViewModel(_reportServices, _selectedOwners, SelectedYears);
                    ReportDeltaSplitMeseView reportDeltaSplitMese = new ReportDeltaSplitMeseView(viewModel);
                    border.Child = reportDeltaSplitMese;
                    ((RadioButton)userControl.FindName(ReportSelezionato)).IsChecked = false;
                    CanClear = true;
                    break;
                case "Titolo":
                    ReportMovementDetaileds = _reportServices.GetMovementDetailed(_selectedOwners[0].Id_gestione, TitoloSelezionato);
                    ReportMovementDetailedViewModel TitoloData = new ReportMovementDetailedViewModel(ReportMovementDetaileds);
                    ReportMovementDetailedView report2 = new ReportMovementDetailedView(TitoloData);
                    border.Child = report2;
                    ((RadioButton)userControl.FindName(ReportSelezionato)).IsChecked = false;
                    CanClear = true;
                    break;
                case "ElencoTitoliAttivi":
                    ReportTitoliAttivis = _reportServices.GetActiveAssets(_selectedOwners, _selectedAccount);
                    ReportTitoliAttiviViewModel AssetsData = new ReportTitoliAttiviViewModel(ReportTitoliAttivis);
                    ReportTitoliAttiviView report3 = new ReportTitoliAttiviView(AssetsData);
                    border.Child = report3;
                    ((RadioButton)userControl.FindName(ReportSelezionato)).IsChecked = false;
                    CanClear = true;
                    break;
                case "AnalisiPortafoglio":
                    GetAnalisiPortafoglio = new ObservableCollection<AnalisiPortafoglio>();
                    if ((bool)((ToggleButton)userControl.FindName("switchBTN")).IsChecked)
                    {
                        GetAnalisiPortafoglio.Add(_reportServices.QuoteInvGeoSettori(_selectedOwners));
                        AnalisiPortafoglioViewModel analisiPortafoglioViewModel = new AnalisiPortafoglioViewModel(_reportServices.QuoteInvGeoSettori(_selectedOwners));
                        AnalisiPortafoglioView report4 = new AnalisiPortafoglioView(analisiPortafoglioViewModel);
                        border.Child = report4;
                    }
                    else
                    {
                        DockPanel dockPanel = new DockPanel();
                        foreach (RegistryOwner I in _selectedOwners)
                        {
                            List<RegistryOwner> temp = new List<RegistryOwner>();
                            temp.Add(I);
                            GetAnalisiPortafoglio.Add(_reportServices.QuoteInvGeoSettori(temp));
                            AnalisiPortafoglioViewModel analisiPortafoglioViewModel = new AnalisiPortafoglioViewModel(_reportServices.QuoteInvGeoSettori(temp));
                            AnalisiPortafoglioView report4 = new AnalisiPortafoglioView(analisiPortafoglioViewModel);
                            DockPanel.SetDock(report4, Dock.Left);
                            dockPanel.Children.Add(report4);
                        }
                        border.Child = dockPanel;
                    }
                    ((RadioButton)userControl.FindName(ReportSelezionato)).IsChecked = false;
                    CanClear = true;
                    break;
                case "Guadagni":
                    GuadagnoPerQuoteDettagliato = _assetServices.GetQuoteGuadagno(2);
                    GuadagnoPerQuoteSintesi = _assetServices.GetQuoteGuadagno(1);
                    GuadagnoPerQuote = _assetServices.GetQuoteGuadagno(0);
                    border.Child = new ReportGuadagniView(new ReportGuadagniViewModel(GuadagnoPerQuoteDettagliato, GuadagnoPerQuoteSintesi, GuadagnoPerQuote));
                    CanClear = true;
                    break;
                default:
                    break;
            }
        }

        public void ExportReport(object param)
        {
            try
            {
                switch (ReportSelezionato)
                {
                    case "PL":
                    case "DPL":
                        Exports.ManagerWorkbooks.ExportDataInXlsx(ReportProfitLosses);
                        break;
                    case "Titolo":
                        Exports.ManagerWorkbooks.ExportDataInXlsx(ReportMovementDetaileds);
                        break;
                    case "ElencoTitoliAttivi":
                        Exports.ManagerWorkbooks.ExportDataInXlsx(ReportTitoliAttivis);
                        break;
                    case "AnalisiPortafoglio":
                        Exports.ManagerWorkbooks.ExportDataInXlsx(GetAnalisiPortafoglio);
                        break;
                    case "Guadagni":
                        Exports.ManagerWorkbooks.ExportDataInXlsx(AddGuadagni());
                        break;
                    default:
                        break;
                }
                MessageBox.Show("Il file di excel è stato prodotto correttamente.", "Finance Manager - Export Report", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi con l'esportazione del report: " + Environment.NewLine + err, "Finance Manager - Export Report", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion command

        private GuadagnoPerQuoteList AddGuadagni()
        {
            GuadagnoPerQuoteList result = new GuadagnoPerQuoteList();
            try
            {
                foreach (GuadagnoPerQuote GPQ in GuadagnoPerQuote)
                {
                    GPQ.IdGestione = 100;
                    result.Add(GPQ);
                }
                foreach (GuadagnoPerQuote GPQ in GuadagnoPerQuoteSintesi)
                {
                    GPQ.IdGestione = 200;
                    result.Add(GPQ);
                }
                foreach (GuadagnoPerQuote GPQ in GuadagnoPerQuoteDettagliato)
                {
                    GPQ.IdGestione = 300;
                    result.Add(GPQ);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi con l'elaborazione dei guadagni: " + Environment.NewLine + err, "Finance Manager - Export Report", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return result;
        }
    }
}

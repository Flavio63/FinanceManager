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
        private IList<RegistryLocation> _selectedAccount;

        public ManagerReportsViewModel(IRegistryServices registryServices, IManagerReportServices managerReportServices, IManagerLiquidAssetServices managerLiquidAssetServices)
        {
            _services = registryServices ?? throw new ArgumentNullException("ManagerReportsViewModel with no registry services");
            _reportServices = managerReportServices ?? throw new ArgumentNullException("ManagerReportsViewModel with no report services");
            _assetServices = managerLiquidAssetServices ?? throw new ArgumentNullException("ManagerLiquidAssetServices");
            CloseMeCommand = new CommandHandler(CloseMe);
            ViewCommand = new CommandHandler(ViewReport, CanDoReport);
            ClearCommand = new CommandHandler(ClearReport, CanClearReport);
            DownloadCommand = new CommandHandler(ExportReport, CanExportReport);
            SetUpViewModel();
        }

        #region private

        private void SetUpViewModel()
        {
            try
            {
                OwnerList = new RegistryOwnersList();
                CurrenciesList = _services.GetRegistryCurrencyList();

                RegistryOwnersList ListaOriginale = new RegistryOwnersList();
                ListaOriginale = _services.GetGestioneList();
                var LO = from risultato in ListaOriginale
                         where risultato.Tipo_Gestione == "Gestore"
                         select risultato;
                foreach (RegistryOwner registryOwner in LO)
                    OwnerList.Add(registryOwner);

                AccountList = _services.GetRegistryLocationList();
                _selectedOwners = new List<RegistryOwner>();
                _selectedAccount = new List<RegistryLocation>();
                AvailableYears = _reportServices.GetAvailableYears();
                SelectedYears = new List<int>();
                SharesList = new ObservableCollection<RegistryShare>(_services.GetRegistryShareList());
                _Filter = new Predicate<object>(Filter);
                ReportSelezionato = "";
                TitoloSelezionato = 0;
                CanClear = false;
                CanExport = false;
                AttivaContoCorrente = false;
                AttivaGestioni = false;
                YearsIsEnable = false;

            }
            catch (Exception err)
            {
                MessageBox.Show("Errore nella richiesta dei dati." + Environment.NewLine + err.Message, "DAF-C Gestione Report");
            }
        }

        private void DoTotals()
        {
            switch (ReportSelezionato)
            {
                case "PL":
                    for (int RowCicle = 0; RowCicle < ReportProfitLosses.Count;)
                    {
                        int Anno = ReportProfitLosses[RowCicle].Anno;
                        while (Anno == ReportProfitLosses[RowCicle].Anno)    // ciclo anno
                        {
                            string Valuta = ReportProfitLosses[RowCicle].Valuta;
                            ReportProfitLoss TotaleAnnoValuta = new ReportProfitLoss();
                            int ContGestioni = 0;
                            while (Anno == ReportProfitLosses[RowCicle].Anno && Valuta == ReportProfitLosses[RowCicle].Valuta) // ciclo anno/valuta
                            {
                                ReportProfitLoss TotaleGestione = new ReportProfitLoss();
                                string Gestione = ReportProfitLosses[RowCicle].Gestione;
                                int ContTipoSoldi = 0;
                                while (Anno == ReportProfitLosses[RowCicle].Anno && Gestione == ReportProfitLosses[RowCicle].Gestione && 
                                    Valuta == ReportProfitLosses[RowCicle].Valuta) // ciclo anno/valuta/gestione
                                {
                                    TotaleAnnoValuta.Azioni += ReportProfitLosses[RowCicle].Azioni;
                                    TotaleAnnoValuta.Obbligazioni += ReportProfitLosses[RowCicle].Obbligazioni;
                                    TotaleAnnoValuta.Certificati += ReportProfitLosses[RowCicle].Certificati;
                                    TotaleAnnoValuta.ETF_C_P += ReportProfitLosses[RowCicle].ETF_C_P;
                                    TotaleAnnoValuta.Fondo += ReportProfitLosses[RowCicle].Fondo;
                                    TotaleAnnoValuta.Futures += ReportProfitLosses[RowCicle].Futures;
                                    TotaleAnnoValuta.Opzioni += ReportProfitLosses[RowCicle].Opzioni;
                                    TotaleAnnoValuta.Commodities += ReportProfitLosses[RowCicle].Commodities;
                                    TotaleAnnoValuta.Costi += ReportProfitLosses[RowCicle].Costi;
                                    TotaleAnnoValuta.Totale += ReportProfitLosses[RowCicle].Totale;

                                    TotaleGestione.Azioni += ReportProfitLosses[RowCicle].Azioni;
                                    TotaleGestione.Obbligazioni += ReportProfitLosses[RowCicle].Obbligazioni;
                                    TotaleGestione.Certificati += ReportProfitLosses[RowCicle].Certificati;
                                    TotaleGestione.ETF_C_P += ReportProfitLosses[RowCicle].ETF_C_P;
                                    TotaleGestione.Fondo += ReportProfitLosses[RowCicle].Fondo;
                                    TotaleGestione.Futures += ReportProfitLosses[RowCicle].Futures;
                                    TotaleGestione.Opzioni += ReportProfitLosses[RowCicle].Opzioni;
                                    TotaleGestione.Commodities += ReportProfitLosses[RowCicle].Commodities;
                                    TotaleGestione.Costi += ReportProfitLosses[RowCicle].Costi;
                                    TotaleGestione.Totale += ReportProfitLosses[RowCicle].Totale;
                                    ContTipoSoldi++;
                                    if (RowCicle + 1 >= ReportProfitLosses.Count)
                                    {
                                        RowCicle++;
                                        break;
                                    }
                                    RowCicle++;
                                }   // fine ciclo anno/valuta/gestione
                                ContGestioni++;
                                if (ContTipoSoldi > 1)
                                {
                                    TotaleGestione.ISIN = "TOTALE " + Gestione + " " + Valuta;
                                    ReportProfitLosses.Insert(RowCicle, TotaleGestione);
                                    RowCicle++;
                                }
                                else
                                {
                                    ReportProfitLosses[RowCicle - 1].ISIN += "TOTALE " + Gestione + " " + Valuta;
                                }
                                if (RowCicle + 1 > ReportProfitLosses.Count)
                                {
                                    break;
                                }
                            } // fine ciclo anno/valuta
                            if (ContGestioni > 1)
                            {
                                TotaleAnnoValuta.ISIN = "TOTALE " + Valuta + " " + Anno;
                                ReportProfitLosses.Insert(RowCicle, TotaleAnnoValuta);
                                RowCicle++;
                            }
                            else
                            {
                                ReportProfitLosses[RowCicle -1].ISIN += " TOTALE " + Valuta + " " + Anno;
                            }
                            if (RowCicle + 1 > ReportProfitLosses.Count)
                            {
                                break;
                            }
                        }
                    }
                    break;
                case "DPL":
                    for (int RowCicle = 0; RowCicle < ReportProfitLosses.Count;)
                    {
                        int Anno = ReportProfitLosses[RowCicle].Anno;
                        int contAnno = 0;
                        while (Anno == ReportProfitLosses[RowCicle].Anno) // ciclo anno
                        {
                            string Valuta = ReportProfitLosses[RowCicle].Valuta;
                            ReportProfitLoss TotaleAnnoValuta = new ReportProfitLoss();
                            int contGestione = 0;
                            while (Anno == ReportProfitLosses[RowCicle].Anno && Valuta == ReportProfitLosses[RowCicle].Valuta)  // ciclo anno/valuta
                            {
                                ReportProfitLoss TotaleGestione = new ReportProfitLoss();
                                string Gestione = ReportProfitLosses[RowCicle].Gestione;
                                while (Anno == ReportProfitLosses[RowCicle].Anno && Gestione == ReportProfitLosses[RowCicle].Gestione
                                    && Valuta == ReportProfitLosses[RowCicle].Valuta) // ciclo anno/valuta/gestione
                                {

                                    int contTipoSoldi = 0;
                                    string tipoSoldi = ReportProfitLosses[RowCicle].TipoSoldi;
                                    ReportProfitLoss TotaleTipoSoldi = new ReportProfitLoss();
                                    while (Anno == ReportProfitLosses[RowCicle].Anno && Gestione == ReportProfitLosses[RowCicle].Gestione
                                    && Valuta == ReportProfitLosses[RowCicle].Valuta && tipoSoldi == ReportProfitLosses[RowCicle].TipoSoldi) // ciclo anno/valuta/gestione/tipo_soldi
                                        {
                                        TotaleAnnoValuta.Azioni += ReportProfitLosses[RowCicle].Azioni;
                                        TotaleAnnoValuta.Obbligazioni += ReportProfitLosses[RowCicle].Obbligazioni;
                                        TotaleAnnoValuta.ETF_C_P += ReportProfitLosses[RowCicle].ETF_C_P;
                                        TotaleAnnoValuta.Fondo += ReportProfitLosses[RowCicle].Fondo;
                                        TotaleAnnoValuta.Futures += ReportProfitLosses[RowCicle].Futures;
                                        TotaleAnnoValuta.Costi += ReportProfitLosses[RowCicle].Costi;
                                        TotaleAnnoValuta.Totale += ReportProfitLosses[RowCicle].Totale;
                                        TotaleGestione.Azioni += ReportProfitLosses[RowCicle].Azioni;
                                        TotaleGestione.Obbligazioni += ReportProfitLosses[RowCicle].Obbligazioni;
                                        TotaleGestione.ETF_C_P += ReportProfitLosses[RowCicle].ETF_C_P;
                                        TotaleGestione.Fondo += ReportProfitLosses[RowCicle].Fondo;
                                        TotaleGestione.Futures += ReportProfitLosses[RowCicle].Futures;
                                        TotaleGestione.Costi += ReportProfitLosses[RowCicle].Costi;
                                        TotaleGestione.Totale += ReportProfitLosses[RowCicle].Totale;
                                        TotaleTipoSoldi.Azioni += ReportProfitLosses[RowCicle].Azioni;
                                        TotaleTipoSoldi.Obbligazioni += ReportProfitLosses[RowCicle].Obbligazioni;
                                        TotaleTipoSoldi.ETF_C_P += ReportProfitLosses[RowCicle].ETF_C_P;
                                        TotaleTipoSoldi.Fondo += ReportProfitLosses[RowCicle].Fondo;
                                        TotaleTipoSoldi.Futures += ReportProfitLosses[RowCicle].Futures;
                                        TotaleTipoSoldi.Costi += ReportProfitLosses[RowCicle].Costi;
                                        TotaleTipoSoldi.Totale += ReportProfitLosses[RowCicle].Totale;
                                        contTipoSoldi++;
                                        if (RowCicle + 1 >= ReportProfitLosses.Count) // verifico di non andare oltre
                                        {
                                            RowCicle++; // incremento comunque il contatore per avere la stessa situazione in uscita dal ciclo
                                            break;
                                        }
                                        RowCicle++;
                                    } // fine ciclo anno/valuta/gestione/tipo_soldi
                                    if (contTipoSoldi > 1) // se il contatore è maggiore di 1 aggiungo riga x totali
                                    {
                                        TotaleTipoSoldi.ISIN = "TOTALE TIPO SOLDI";
                                        TotaleTipoSoldi.NomeTitolo = "TOTALE TIPO SOLDI";
                                        TotaleTipoSoldi.TipoSoldi = tipoSoldi;
                                        TotaleTipoSoldi.Gestione = Gestione;
                                        TotaleTipoSoldi.Valuta = Valuta;
                                        TotaleTipoSoldi.Anno = Anno;
                                        ReportProfitLosses.Insert(RowCicle, TotaleTipoSoldi);   // il contatore viene incrementato automaticamente
                                        RowCicle++;                                                 // mi adeguo
                                    }
                                    else
                                    {
                                        ReportProfitLosses[RowCicle - 1].TipoSoldi += " TOTALE";
                                    }
                                    if (RowCicle >= ReportProfitLosses.Count)
                                        break;
                                }
                                contGestione++;
                                contAnno++;
                                if (contGestione > 1)
                                {
                                    TotaleGestione.ISIN = "TOTALE GESTIONE";
                                    TotaleGestione.NomeTitolo = "TOTALE GESTIONE";
                                    TotaleGestione.TipoSoldi = "TOTALE GESTIONE";
                                    TotaleGestione.Gestione = Gestione;
                                    TotaleGestione.Valuta = Valuta;
                                    TotaleGestione.Anno = Anno;
                                    ReportProfitLosses.Insert(RowCicle, TotaleGestione);      // il contatore viene incrementato automaticamente
                                    RowCicle++;                                                 // mi adeguo
                                }
                                else
                                {
                                    ReportProfitLosses[RowCicle - 1].Gestione += " TOTALE";
                                }
                                if (RowCicle >= ReportProfitLosses.Count)
                                    break;
                            }   // fine ciclo gestione
                            if (contAnno > 1)
                            {
                                TotaleAnnoValuta.ISIN = "TOTALE ANNO " + Valuta;
                                TotaleAnnoValuta.NomeTitolo = "TOTALE ANNO " + Valuta;
                                TotaleAnnoValuta.TipoSoldi = "TOTALE ANNO " + Valuta;
                                TotaleAnnoValuta.Gestione = "TOTALE ANNO" + Valuta;
                                TotaleAnnoValuta.Valuta = Valuta;
                                TotaleAnnoValuta.Anno = Anno;
                                ReportProfitLosses.Insert(RowCicle, TotaleAnnoValuta);
                                RowCicle++;
                            }
                            else
                            {
                                ReportProfitLosses[RowCicle - 1].Gestione += " - TOTALE ANNO";
                            }
                            if (RowCicle >= ReportProfitLosses.Count)
                            {
                                break;
                            }
                        } // fine ciclo anno
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
                            _selectedAccount.Add(registryLocation);
                        break;
                    case "Anni":
                        SelectedYears.Clear();
                        foreach (int y in LB.SelectedItems)
                        {
                            SelectedYears.Add(y);
                        }
                        break;
                    case "Valute":
                        if (e.AddedItems.Count > 0)
                            if (e.AddedItems[0] is RegistryCurrency RC)
                                SelectedCurrency = RC.IdCurrency;
                        break;
                }
            }
            if (sender is ComboBox CB)
            {
                if (CB.Items.Count > 0)
                    TitoloSelezionato = (int)(((RegistryShare)CB.SelectedItem).id_titolo);
            }
            CanClear = true;
        }

        public void IsChecked(object sender, RoutedEventArgs e)
        {
            ReportSelezionato = ((RadioButton)sender).Name;
            switch (ReportSelezionato)
            {
                case "AnalisiPortafoglio":
                    AttivaGestioni = true;
                    AggregateIsEnabled = true;
                    IsTotalYear = false;
                    break;
                case "PL":
                case "DPL":
                    IsTotalYear = false;
                    AttivaGestioni = true;
                    YearsIsEnable = true;
                    AggregateIsEnabled = false;
                    AttivaContoCorrente = false;
                    break;
                case "DeltaAnni":
                    AttivaGestioni = true;
                    IsTotalYear = true;
                    YearsIsEnable = true;
                    AggregateIsEnabled = true;
                    break;
                case "DeltaMese":
                    AttivaGestioni = true;
                    IsTotalYear = false;
                    YearsIsEnable = true;
                    AggregateIsEnabled = true;
                    break;
                case "MovimentiContoGestione":
                    AttivaContoCorrente = true;
                    AttivaGestioni = true;
                    YearsIsEnable = true;
                    AggregateIsEnabled = false;
                    break;
                case "ElencoTitoliAttivi":
                    AttivaContoCorrente = true;
                    AttivaGestioni = true;
                    break;
                case "Titolo":
                    AttivaGestioni = true;
                    break;
                default:
                    YearsIsEnable = false;
                    AggregateIsEnabled = false;
                    AttivaGestioni = false;
                    AttivaContoCorrente = false;
                    IsTotalYear = false;
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

        public bool AttivaContoCorrente
        {
            get { return GetValue(() => AttivaContoCorrente); }
            private set { SetValue(() => AttivaContoCorrente, value); }
        }

        #region Titoli
        /// <summary>
        /// La ricerca degli isin dei titoli per la selezione del report
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
        /// <summary>
        /// Il titolo selezionato per il report
        /// </summary>
        public int TitoloSelezionato
        {
            get { return GetValue(() => TitoloSelezionato); }
            private set { SetValue(() => TitoloSelezionato, value); }
        }
        #endregion

        #region Gestioni
        /// <summary>
        /// Gestisce l'attivazione delle gestioni
        /// </summary>
        public bool AttivaGestioni
        {
            get { return GetValue(() => AttivaGestioni); }
            private set { SetValue(() => AttivaGestioni, value); }
        }
        /// <summary>
        /// La lista di tutte le gestioni selezionabili
        /// </summary>
        public RegistryOwnersList OwnerList
        {
            get { return GetValue(() => OwnerList); }
            set { SetValue(() => OwnerList, value); }
        }
        
        /// <summary>
        /// L'elenco delle gestioni selezionate in maschera
        /// </summary>
        public RegistryOwnersList SelectedOwner
        {
            get { return GetValue(() => SelectedOwner); }
            set { SetValue(() => SelectedOwner, value); }
        }
        
        /// <summary>
        /// Serve ad aggregare i dati delle gestioni (true) o a
        /// mantenerle separate (false)
        /// </summary>
        public bool AggregateIsEnabled
        {
            get { return GetValue(() => AggregateIsEnabled); }
            set { SetValue(() => AggregateIsEnabled, value); }
        }
        #endregion

        /// <summary>
        /// Elenco con i conti correnti disponibili
        /// </summary>
        public RegistryLocationList AccountList
        {
            get { return GetValue(() => AccountList); }
            private set { SetValue(() => AccountList, value); }
        }

        #region Anni
        /// <summary>
        /// Anni disponibili nel database
        /// </summary>
        public IList<int> AvailableYears
        {
            get { return GetValue(() => AvailableYears); }
            set { SetValue(() => AvailableYears, value); }
        }
        
        /// <summary>
        /// Gestisce la selezionabilità degli anni in maschera
        /// </summary>
        public bool YearsIsEnable
        {
            get { return GetValue(() => YearsIsEnable); }
            private set { SetValue(() => YearsIsEnable, value); }
        }
        
        /// <summary>
        /// Gli anni selezionati in maschera
        /// </summary>
        public IList<int> SelectedYears
        {
            get { return GetValue(() => SelectedYears); }
            set { SetValue(() => SelectedYears, value); }
        }

        private bool IsTotalYear { get; set; }
        #endregion
        
        #region Currencies
        public RegistryCurrencyList CurrenciesList
        {
            get { return GetValue(() => CurrenciesList); }
            set { SetValue(() => CurrenciesList, value); }
        }

        public int SelectedCurrency
        {
            get { return GetValue(() => SelectedCurrency); }
            private set { SetValue(() => SelectedCurrency, value); }
        }

        #endregion

        #region Reports
        /// <summary>
        /// il report selezionato nel menu
        /// </summary>
        public string ReportSelezionato
        {
            get { return GetValue(() => ReportSelezionato); }
            set { SetValue(() => ReportSelezionato, value); }
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
        /// <summary>
        /// Sono i movimenti di un dato conto per una data
        /// gestione ordinati per data decrescente
        /// </summary>
        public MovimentiContoList MovimentiContos
        {
            get { return GetValue(() => MovimentiContos); }
            set { SetValue(() => MovimentiContos, value); }
        }

        /// <summary>
        /// I dati per ottenere la tabella con i delta per periodo
        /// sia splittati per gestione che aggregati
        /// </summary>
        public GuadagnoPerPeriodoList DataDeltaPerPeriodo
        {
            get { return GetValue(() => DataDeltaPerPeriodo); }
            set { SetValue(() => DataDeltaPerPeriodo, value); }
        }

        #endregion
        
        private bool CanClear { get; set; }
        private bool CanExport { get; set; }

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
                case "MovimentiContoGestione":
                    if (_selectedAccount.Count() == 1 && _selectedOwners.Count() == 1 && SelectedYears.Count() == 1 && SelectedCurrency != 0)
                        return true;
                    return false;
                default:
                    return false;
            }
        }

        private bool CanExportReport(object param)
        {
            if (CanExport)
                return true;
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
            ((ListBox)userControl.FindName("Gestione")).SelectedIndex = -1;
            ((RadioButton)userControl.FindName("PL")).IsChecked = false;
            ((RadioButton)userControl.FindName("DPL")).IsChecked = false;
            ((RadioButton)userControl.FindName("Titolo")).IsChecked = false;
            ((RadioButton)userControl.FindName("ElencoTitoliAttivi")).IsChecked = false;
            ((RadioButton)userControl.FindName("MovimentiContoGestione")).IsChecked = false;
            ((RadioButton)userControl.FindName("DeltaAnni")).IsChecked = false;
            ((RadioButton)userControl.FindName("DeltaMese")).IsChecked = false;
            ((RadioButton)userControl.FindName("Guadagni")).IsChecked = false;
            ((RadioButton)userControl.FindName("AnalisiPortafoglio")).IsChecked = false;
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
                    CanExport = true;
                    break;
                case "DPL":
                    ReportProfitLosses = _reportServices.GetReport1(_selectedOwners, SelectedYears, false);
                    DoTotals();
                    ReportPorfitLossAnnoGestioniViewModel ProfitLossDetailedData = new ReportPorfitLossAnnoGestioniViewModel(ReportProfitLosses, true);
                    ReportProfitLossAnnoGestioneView report1_1 = new ReportProfitLossAnnoGestioneView(ProfitLossDetailedData);
                    border.Child = report1_1;
                    ((RadioButton)userControl.FindName(ReportSelezionato)).IsChecked = false;
                    CanExport = true;
                    break;
                case "DeltaAnni":
                case "DeltaMese":
                    DataDeltaPerPeriodo = _reportServices.GetDeltaPeriod(_selectedOwners, SelectedYears, IsTotalYear, (bool)((ToggleButton)userControl.FindName("switchBTN")).IsChecked);
                    ReportDeltaSplitMeseViewModel viewModel = new ReportDeltaSplitMeseViewModel(DataDeltaPerPeriodo);
                    ReportDeltaSplitMeseView reportDeltaSplitMese = new ReportDeltaSplitMeseView(viewModel);
                    border.Child = reportDeltaSplitMese;
                    ((RadioButton)userControl.FindName(ReportSelezionato)).IsChecked = false;
                    CanExport = true;
                    break;
                case "Titolo":
                    ReportMovementDetaileds = _reportServices.GetMovementDetailed(_selectedOwners[0].Id_Gestione, TitoloSelezionato);
                    ReportMovementDetailedViewModel TitoloData = new ReportMovementDetailedViewModel(ReportMovementDetaileds);
                    ReportMovementDetailedView report2 = new ReportMovementDetailedView(TitoloData);
                    border.Child = report2;
                    ((RadioButton)userControl.FindName(ReportSelezionato)).IsChecked = false;
                    CanExport = true;
                    break;
                case "ElencoTitoliAttivi":
                    ReportTitoliAttivis = _reportServices.GetActiveAssets(_selectedOwners, _selectedAccount);
                    ReportTitoliAttiviViewModel AssetsData = new ReportTitoliAttiviViewModel(ReportTitoliAttivis);
                    ReportTitoliAttiviView report3 = new ReportTitoliAttiviView(AssetsData);
                    border.Child = report3;
                    ((RadioButton)userControl.FindName(ReportSelezionato)).IsChecked = false;
                    CanExport = true;
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
                    CanExport = true;
                    break;
                case "Guadagni":
                    GuadagnoPerQuoteDettagliato = _assetServices.GetQuoteGuadagno(2);
                    GuadagnoPerQuoteSintesi = _assetServices.GetQuoteGuadagno(1);
                    GuadagnoPerQuote = _assetServices.GetQuoteGuadagno(0);
                    border.Child = new ReportGuadagniView(new ReportGuadagniViewModel(GuadagnoPerQuoteDettagliato, GuadagnoPerQuoteSintesi, GuadagnoPerQuote));
                    CanExport = true;
                    CanClear = true;
                    break;
                case "MovimentiContoGestione":
                    MovimentiContos = _assetServices.GetMovimentiContoGestioneValuta(_selectedAccount[0].Id_Conto, _selectedOwners[0].Id_Gestione, SelectedYears[0], SelectedCurrency);
                    ReportMovimentiContoViewModel reportMovimentiContoViewModel = new ReportMovimentiContoViewModel(MovimentiContos);
                    ReportMovimentiContoView report5 = new ReportMovimentiContoView(reportMovimentiContoViewModel);
                    border.Child = report5;
                    CanExport = true;
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
                    case "DeltaAnni":
                    case "DeltaMese":
                        Exports.ManagerWorkbooks.ExportDataInXlsx(DataDeltaPerPeriodo);
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

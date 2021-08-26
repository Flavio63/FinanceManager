using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Models.Enumeratori;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class GestioneContoCorrenteViewModel : ViewModelBase
    {
        IRegistryServices _registryServices;
        IManagerLiquidAssetServices _liquidAssetServices;
        public ICommand CloseMeCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand EraseCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        private int[] typeOfShares = { 147 };
        Predicate<object> _Filter;

        private TabControl _TabControl = new TabControl();

        public GestioneContoCorrenteViewModel(IRegistryServices registryServices, IManagerLiquidAssetServices managerLiquidServices)
        {
            _registryServices = registryServices ?? throw new ArgumentNullException("Registry Services in Gestione Conto Corrente");
            _liquidAssetServices = managerLiquidServices ?? throw new ArgumentNullException("Registry Services in Gestione Conto Corrente");
            CloseMeCommand = new CommandHandler(CloseMe);
            InsertCommand = new CommandHandler(SaveCommand, CanSave);
            ModifyCommand = new CommandHandler(UpdateCommand, CanModify);
            EraseCommand = new CommandHandler(DeleteCommand, CanModify);
            ClearCommand = new CommandHandler(CleanCommand);
            try
            {
                SetUpData();
                Init();
            }
            catch (Exception err)
            {
                MessageBox.Show("Errore nella richiesta dei dati." + Environment.NewLine + err.Message, "DAF-C Quote Investitori");
            }
        }

        private void SetUpData()
        {
            try
            {
                ListMovimenti = new RegistryMovementTypeList();
                ListGestioni = new RegistryOwnersList();
                ListConti = new RegistryLocationList();
                ListValute = new RegistryCurrencyList();
                TipoSoldis = new TipoSoldiList();
                RegistryMovementTypeList listaOriginale = new RegistryMovementTypeList();
                listaOriginale = _registryServices.GetRegistryMovementTypesList();
                var RMTL = from movimento in listaOriginale
                           where (movimento.Id_tipo_movimento == (int)TipologiaMovimento.Cedola ||
                           movimento.Id_tipo_movimento == (int)TipologiaMovimento.InsertVolatili ||
                           movimento.Id_tipo_movimento == (int)TipologiaMovimento.Giroconto) ||
                           movimento.Id_tipo_movimento == (int)TipologiaMovimento.Costi ||
                           movimento.Id_tipo_movimento == (int)TipologiaMovimento.CambioValuta ||
                           movimento.Id_tipo_movimento == (int)TipologiaMovimento.AcquistoTitoli ||
                           movimento.Id_tipo_movimento == (int)TipologiaMovimento.VenditaTitoli
                           select movimento;
                foreach (RegistryMovementType registry in RMTL)
                    ListMovimenti.Add(registry);
                ListValute = _registryServices.GetRegistryCurrencyList();
                RegistryOwnersList ListaInvestitoreOriginale = new RegistryOwnersList();
                ListaInvestitoreOriginale = _registryServices.GetGestioneList();
                var ROL = from gestione in ListaInvestitoreOriginale
                          where (gestione.Tipologia == "Gestore")
                          select gestione;
                _TabControl.TabStripPlacement = Dock.Left;
                foreach (RegistryOwner registryOwner in ROL)
                {
                    // per ogni gestione acquisisco i dati per la sintesi soldi
                    TabItem tabItem = new TabItem();
                    tabItem.Header = registryOwner.Nome_Gestione;
                    tabItem.Content = new TabControlSintesiView(new TabControlSintesiViewModel(_liquidAssetServices.GetCurrencyAvailable(registryOwner.Id_gestione)));
                    _TabControl.Items.Add(tabItem);
                    ListGestioni.Add(registryOwner);
                }
                ListConti = _registryServices.GetRegistryLocationList();
                TipoSoldis = _registryServices.GetTipoSoldiList();
                SharesList = new ObservableCollection<RegistryShare>(_registryServices.GetRegistryShareList());
                _Filter = new Predicate<object>(Filter);

                BirdsListView = _registryServices.GetSharesByFirms(typeOfShares);
            }
            catch (Exception err)
            {
                throw new Exception("Errore nel setup." + Environment.NewLine + err.Message);
            }
        }


        private void Init()
        {
            try
            {
                Record2ContoCorrente = new ContoCorrente();
                RecordContoCorrente = new ContoCorrente();
                TwoRecordConto = new ContoCorrenteList();
                AmountChangedValue = 0;
                SrchShares = "";
                ListContoCorrente = _liquidAssetServices.GetContoCorrenteList();
                CommonFieldsEnabled = true;
                OperazioneEnabled = true;
                CedoleEnabled = false;
                GirocontoEnabled = false;
                VolatiliEnabled = false;
                CambioValutaEnabled = false;
                CanUpdateDelete = false;
                CanInsert = false;
                FiltroConto = "";
                FiltroGestione = "";
                FiltroTipoSoldi = "";
                FiltroTipoMovimento = "";
            }
            catch (Exception err)
            {
                throw new Exception("Errore in init." + Environment.NewLine + err.Message);
            }
        }

        #region Getter&Setter
        /// <summary>
        /// Combo box con i movimenti
        /// </summary>
        public RegistryMovementTypeList ListMovimenti
        {
            get { return GetValue(() => ListMovimenti); }
            set { SetValue(() => ListMovimenti, value); }
        }

        /// <summary>
        /// combo box con la lista delle valute
        /// </summary>
        public RegistryCurrencyList ListValute
        {
            get { return GetValue(() => ListValute); }
            set { SetValue(() => ListValute, value); }
        }

        /// <summary>
        /// combo box con la lista dei C/C
        /// </summary>
        public RegistryLocationList ListConti
        {
            get { return GetValue(() => ListConti); }
            set { SetValue(() => ListConti, value); }
        }

        /// <summary>
        /// combo box con la lista dei C/C
        /// </summary>
        public RegistryOwnersList ListGestioni
        {
            get { return GetValue(() => ListGestioni); }
            set { SetValue(() => ListGestioni, value); }
        }

        /// <summary>
        /// Elenco con tutti i movimenti del conto corrente
        /// </summary>
        public ContoCorrenteList ListContoCorrente
        {
            get { return GetValue(() => ListContoCorrente); }
            set { SetValue(() => ListContoCorrente, value); AccountCollectionView = CollectionViewSource.GetDefaultView(value); }
        }

        /// <summary>
        /// Lista con i 2 record coinvolti nel giroconto
        /// o nel cambia valuta
        /// </summary>
        public ContoCorrenteList TwoRecordConto
        {
            get { return GetValue(() => TwoRecordConto); }
            private set { SetValue(() => TwoRecordConto, value); }
        }
        /// <summary>
        /// E' la lista di tutti i record Conto corrente usata per i filtri della griglia
        /// </summary>
        public System.ComponentModel.ICollectionView AccountCollectionView
        {
            get { return GetValue(() => AccountCollectionView); }
            set { SetValue(() => AccountCollectionView, value); }
        }

        /// <summary>
        /// Elenco con i tipo soldi
        /// </summary>
        public TipoSoldiList TipoSoldis
        {
            get { return GetValue(() => TipoSoldis); }
            set { SetValue(() => TipoSoldis, value); }
        }

        /// <summary>
        /// Totale Contabile convertito in euro
        /// da mostrare nella maschera a video
        /// </summary>
        public double AmountChangedValue
        {
            get { return GetValue<double>(() => AmountChangedValue); }
            set { SetValue<double>(() => AmountChangedValue, value); }
        }

        /// <summary>
        /// E' la valuta disponibile 
        /// suddivisa in Cedole, Utili e Disponibili
        /// </summary>
        public SintesiSoldi CurrencyAvailable
        {
            get { return GetValue(() => CurrencyAvailable); }
            private set { SetValue(() => CurrencyAvailable, value); }
        }

        /// <summary>
        /// La ricerca degli isin dei titoli
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
        public ObservableCollection<RegistryShare> SharesList
        {
            get { return GetValue(() => SharesList); }
            set
            {
                SetValue(() => SharesList, value);
                SharesListView = new ListCollectionView(value);
            }
        }

        /// <summary>
        /// Elenco con i titoli disponibili da verificare se serve
        /// </summary>
        public ListCollectionView SharesListView
        {
            get { return GetValue(() => SharesListView); }
            set { SetValue(() => SharesListView, value); }
        }

        /// <summary>
        /// Elenco con i titoli disponibili filtrato per la tipologia 
        /// certificati e futures
        /// </summary>
        public RegistryShareList BirdsListView
        {
            get { return GetValue(() => BirdsListView); }
            set { SetValue(() => BirdsListView, value); }
        }

        /// <summary>
        /// Singolo record del portafoglio
        /// </summary>
        public ContoCorrente RecordContoCorrente
        {
            get { return GetValue(() => RecordContoCorrente); }
            set { SetValue(() => RecordContoCorrente, value); }
        }

        /// <summary>
        /// Singolo record del portafoglio da usare nel caso di Cambio Valuta
        /// o nel caso di Giroconto per duplicare le info di RecordContoCorrente
        /// modificando solo i campi di destinazione
        /// </summary>
        public ContoCorrente Record2ContoCorrente
        {
            get { return GetValue(() => Record2ContoCorrente); }
            set { SetValue(() => Record2ContoCorrente, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione del blocco dedicato alla registrazione delle cedole
        /// </summary>
        public bool CedoleEnabled
        {
            get { return GetValue(() => CedoleEnabled); }
            private set { SetValue(() => CedoleEnabled, value); }
        }

        ///<summary>
        /// Abilita / disabilita la possibilità di modificare i campi nei parametri comuni
        ///</summary>
        public bool CommonFieldsEnabled
        {
            get { return GetValue(() => CommonFieldsEnabled); }
            set { SetValue(() => CommonFieldsEnabled, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione del blocco dedicato al giroconto
        /// </summary>
        public bool GirocontoEnabled
        {
            get { return GetValue(() => GirocontoEnabled); }
            private set { SetValue(() => GirocontoEnabled, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione a inserire i profit/loss delle azioni volatili
        /// </summary>
        public bool VolatiliEnabled
        {
            get { return GetValue(() => VolatiliEnabled); }
            private set { SetValue(() => VolatiliEnabled, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione a inserire i cambi valuta
        /// </summary>
        public bool CambioValutaEnabled
        {
            get { return GetValue(() => CambioValutaEnabled); }
            private set { SetValue(() => CambioValutaEnabled, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione del tipo di operazione
        /// </summary>
        public bool OperazioneEnabled
        {
            get { return GetValue(() => OperazioneEnabled); }
            set { SetValue(() => OperazioneEnabled, value); }
        }
        #endregion

        #region Filtri per DataGrid
        /// <summary>
        /// Il filtro per i titoli
        /// </summary>
        /// <param name="obj"> Il tipo di voce nell'elenco da filtrare </param>
        /// <returns> La voce filtrata </returns>
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
                else if (obj is ContoCorrente CConto)
                {
                    if (!string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) && !string.IsNullOrWhiteSpace(FiltroTipoMovimento)) // tutti e 4 i filtri
                    {
                        return CConto.Desc_Conto.ToLower().Contains(FiltroConto.ToLower()) && CConto.NomeGestione.ToLower().Contains(FiltroGestione.ToLower()) && CConto.Desc_Tipo_Soldi.ToLower().Contains(FiltroTipoSoldi.ToLower()) && CConto.Desc_tipo_movimento.ToLower().Contains(FiltroTipoMovimento.ToLower());
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) && string.IsNullOrWhiteSpace(FiltroTipoMovimento)) // 3 su 4 attivi
                    {
                        return CConto.Desc_Conto.ToLower().Contains(FiltroConto.ToLower()) && CConto.NomeGestione.ToLower().Contains(FiltroGestione.ToLower()) && CConto.Desc_Tipo_Soldi.ToLower().Contains(FiltroTipoSoldi.ToLower());
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) && !string.IsNullOrWhiteSpace(FiltroTipoMovimento)) // 3 su 4 attivi
                    {
                        return CConto.Desc_Conto.ToLower().Contains(FiltroConto.ToLower()) && CConto.NomeGestione.ToLower().Contains(FiltroGestione.ToLower()) && CConto.Desc_tipo_movimento.ToLower().Contains(FiltroTipoMovimento.ToLower());
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) && !string.IsNullOrWhiteSpace(FiltroTipoMovimento)) // 3 su 4 attivi
                    {
                        return CConto.Desc_Conto.ToLower().Contains(FiltroConto.ToLower()) && CConto.Desc_Tipo_Soldi.ToLower().Contains(FiltroTipoSoldi.ToLower()) && CConto.Desc_tipo_movimento.ToLower().Contains(FiltroTipoMovimento.ToLower());
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) && !string.IsNullOrWhiteSpace(FiltroTipoMovimento)) // 3 su 4 attivi
                    {
                        return CConto.NomeGestione.ToLower().Contains(FiltroGestione.ToLower()) && CConto.Desc_Tipo_Soldi.ToLower().Contains(FiltroTipoSoldi.ToLower()) && CConto.Desc_tipo_movimento.ToLower().Contains(FiltroTipoMovimento.ToLower());
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) && string.IsNullOrWhiteSpace(FiltroTipoMovimento)) // 2 su 4 attivi
                    {
                        return CConto.Desc_Conto.ToLower().Contains(FiltroConto.ToLower()) && CConto.NomeGestione.ToLower().Contains(FiltroGestione.ToLower());
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) && string.IsNullOrWhiteSpace(FiltroTipoMovimento)) // 2 su 4 attivi
                    {
                        return CConto.Desc_Conto.ToLower().Contains(FiltroConto.ToLower()) && CConto.Desc_Tipo_Soldi.ToLower().Contains(FiltroTipoSoldi.ToLower());
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) && string.IsNullOrWhiteSpace(FiltroTipoMovimento)) // 2 su 4 attivi
                    {
                        return CConto.NomeGestione.ToLower().Contains(FiltroGestione.ToLower()) && CConto.Desc_Tipo_Soldi.ToLower().Contains(FiltroTipoSoldi.ToLower());
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) && !string.IsNullOrWhiteSpace(FiltroTipoMovimento)) // 2 su 4 attivi
                    {
                        return CConto.Desc_Conto.ToLower().Contains(FiltroConto.ToLower()) && CConto.Desc_tipo_movimento.ToLower().Contains(FiltroTipoMovimento.ToLower());
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) && !string.IsNullOrWhiteSpace(FiltroTipoMovimento)) // 2 su 4 attivi
                    {
                        return CConto.NomeGestione.ToLower().Contains(FiltroGestione.ToLower()) && CConto.Desc_tipo_movimento.ToLower().Contains(FiltroTipoMovimento.ToLower());
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) && !string.IsNullOrWhiteSpace(FiltroTipoMovimento)) // 2 su 4 attivi
                    {
                        return CConto.Desc_Tipo_Soldi.ToLower().Contains(FiltroTipoSoldi.ToLower()) && CConto.Desc_tipo_movimento.ToLower().Contains(FiltroTipoMovimento.ToLower());
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) && string.IsNullOrWhiteSpace(FiltroTipoMovimento)) // 1 su 4 attivi
                    {
                        return CConto.Desc_Conto.ToLower().Contains(FiltroConto.ToLower());
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) && string.IsNullOrWhiteSpace(FiltroTipoMovimento)) // 1 su 4 attivi
                    {
                        return CConto.NomeGestione.ToLower().Contains(FiltroGestione.ToLower());
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) && string.IsNullOrWhiteSpace(FiltroTipoMovimento)) // 1 su 4 attivi
                    {
                        return CConto.Desc_Tipo_Soldi.ToLower().Contains(FiltroTipoSoldi.ToLower());
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) && !string.IsNullOrWhiteSpace(FiltroTipoMovimento)) // 1 su 4 attivi
                    {
                        return CConto.Desc_tipo_movimento.ToLower().Contains(FiltroTipoMovimento.ToLower());
                    }
                }

            }
            return true;
        }

        private string _FiltroConto;
        private string FiltroConto
        {
            get { return _FiltroConto; }
            set { _FiltroConto = value; AccountCollectionView.Filter = _Filter; AccountCollectionView.Refresh(); }
        }
        private string _FiltroGestione;
        private string FiltroGestione
        {
            get { return _FiltroGestione; }
            set { _FiltroGestione = value; AccountCollectionView.Filter = _Filter; AccountCollectionView.Refresh(); }
        }
        private string _FiltroTipoSoldi;
        private string FiltroTipoSoldi
        {
            get { return _FiltroTipoSoldi; }
            set { _FiltroTipoSoldi = value; AccountCollectionView.Filter = _Filter; AccountCollectionView.Refresh(); }
        }
        private string _FiltroTipoMovimento;
        private string FiltroTipoMovimento
        {
            get { return _FiltroTipoMovimento; }
            set { _FiltroTipoMovimento = value; AccountCollectionView.Filter = _Filter; AccountCollectionView.Refresh(); }
        }
        #endregion


        #region Events

        /// <summary>
        /// Gli eventi al cambio di selezione del record nella griglia dell'investitore
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                // tranne che nel cambio valuta in tutti gli altri if imposto il record 2 identico al record 1
                // è in altri punti del codice che verrà effettuata la differenziazione dei 2
                if (e.AddedItems[0] is DateTime DT)
                {
                    RecordContoCorrente.DataMovimento = DT.Date;
                    Record2ContoCorrente.DataMovimento = DT.Date;
                }
                else if (e.AddedItems[0] is RegistryLocation RL)
                {
                    if (((ComboBox)e.OriginalSource).Name == "Conto2")
                    {
                        Record2ContoCorrente.Id_Conto = RL.Id_Conto;
                        Record2ContoCorrente.Desc_Conto = RL.Desc_Conto;
                    }
                    else
                    {
                        RecordContoCorrente.Id_Conto = RL.Id_Conto;
                        RecordContoCorrente.Desc_Conto = RL.Desc_Conto;
                        if (CanUpdateDelete != true) Record2ContoCorrente.Id_Conto = RL.Id_Conto;
                        if (CanUpdateDelete != true) Record2ContoCorrente.Desc_Conto = RL.Desc_Conto;
                    }
                    FiltroConto = RL.Desc_Conto;
                }
                else if (e.AddedItems[0] is RegistryOwner RO)
                {
                    if (((ComboBox)e.OriginalSource).Name == "Gestione2")
                    {
                        Record2ContoCorrente.Id_Gestione = RO.Id_gestione;
                        Record2ContoCorrente.NomeGestione = RO.Nome_Gestione;
                        CanInsert = true;
                    }
                    else
                    {
                        RecordContoCorrente.Id_Gestione = RO.Id_gestione;
                        RecordContoCorrente.NomeGestione = RO.Nome_Gestione;
                        if (CanUpdateDelete != true) Record2ContoCorrente.Id_Gestione = RO.Id_gestione;
                        if (CanUpdateDelete != true) Record2ContoCorrente.NomeGestione = RO.Nome_Gestione;
                    }
                    FiltroGestione = RO.Nome_Gestione;
                }
                else if (e.AddedItems[0] is RegistryCurrency RC)
                {
                    if (((ComboBox)e.OriginalSource).Name == "Valuta2")
                    {
                        if (RC.IdCurrency == RecordContoCorrente.Id_Valuta)
                        {
                            MessageBox.Show("Attenzione le 2 valute sono uguali!", "Gestione Conto Corrente", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                        Record2ContoCorrente.Id_Valuta = RC.IdCurrency;
                        Record2ContoCorrente.Cod_Valuta = RC.CodeCurrency;
                        // verifico la presenza della valuta di partenza
                    }
                    else
                    {
                        RecordContoCorrente.Id_Valuta = RC.IdCurrency;
                        RecordContoCorrente.Cod_Valuta = RC.CodeCurrency;
                        Record2ContoCorrente.Id_Valuta = RC.IdCurrency;
                        Record2ContoCorrente.Cod_Valuta = RC.CodeCurrency;
                    }
                }
                else if (e.AddedItems[0] is TipoSoldi TS)
                {
                    RecordContoCorrente.Id_Tipo_Soldi = TS.Id_Tipo_Soldi;
                    RecordContoCorrente.Desc_Tipo_Soldi = TS.Desc_Tipo_Soldi;
                    Record2ContoCorrente.Id_Tipo_Soldi = TS.Id_Tipo_Soldi;
                    Record2ContoCorrente.Desc_Tipo_Soldi = TS.Desc_Tipo_Soldi;
                    FiltroTipoSoldi = TS.Desc_Tipo_Soldi;
                }
                else if (e.AddedItems[0] is RegistryMovementType RMT)
                {
                    RecordContoCorrente.Id_tipo_movimento = RMT.Id_tipo_movimento;
                    RecordContoCorrente.Desc_tipo_movimento = RMT.Desc_tipo_movimento;
                    Record2ContoCorrente.Id_tipo_movimento = RMT.Id_tipo_movimento;
                    Record2ContoCorrente.Desc_tipo_movimento = RMT.Desc_tipo_movimento;
                    // abilito il blocco di input dati sulla base di questa scelta
                    CedoleEnabled = RMT.Id_tipo_movimento == (int)TipologiaMovimento.Cedola ? true : false;
                    GirocontoEnabled = RMT.Id_tipo_movimento == (int)TipologiaMovimento.Giroconto ? true : false;
                    VolatiliEnabled = RMT.Id_tipo_movimento == (int)TipologiaMovimento.InsertVolatili ? true : false;
                    CambioValutaEnabled = RMT.Id_tipo_movimento == (int)TipologiaMovimento.CambioValuta ? true : false;
                    CanInsert = RMT.Id_tipo_movimento == (int)TipologiaMovimento.Costi ? true : false;
                    FiltroTipoMovimento = RMT.Desc_tipo_movimento;
                }
                else if (e.AddedItems[0] is RegistryShare RS)
                {
                    RecordContoCorrente.Id_Titolo = (int)RS.id_titolo;
                    RecordContoCorrente.Desc_Titolo = RS.desc_titolo;
                    CanInsert = RecordContoCorrente.Id_RowConto > 0 ? false : true;
                }
            }
        }

        /// <summary>
        /// Imposto i campi sopra la griglia quando viene selezionata una riga
        /// nell'elenco sottostante
        /// </summary>
        /// <param name="sender">Grid dei dati</param>
        /// <param name="e">Cambio di selezione</param>
        public void GridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                e.Handled = true;
            }
            else if (e.AddedItems[0] is ContoCorrente CC)
            {
                RecordContoCorrente = CC;
                if (CC.Id_tipo_movimento == (int)TipologiaMovimento.Cedola ||
                    CC.Id_tipo_movimento == (int)TipologiaMovimento.InsertVolatili ||
                    CC.Id_tipo_movimento == (int)TipologiaMovimento.Costi)
                {
                    CommonFieldsEnabled = CC.Id_tipo_movimento == (int)TipologiaMovimento.Giroconto ? true : false;
                    OperazioneEnabled = false;
                    CedoleEnabled = CC.Id_tipo_movimento == (int)TipologiaMovimento.Cedola ? true : false;
                    VolatiliEnabled = CC.Id_tipo_movimento == (int)TipologiaMovimento.InsertVolatili ? true : false;
                    CambioValutaEnabled = false;
                    CanUpdateDelete = true;
                }
                else if (CC.Id_Quote_Investimenti == 0 & (CC.Id_tipo_movimento == (int)TipologiaMovimento.Giroconto || CC.Id_tipo_movimento == (int)TipologiaMovimento.CambioValuta))
                {
                    // cerco il record corrispondente al giroconto o al cambio valuta utilizzando il campo
                    // modified (e poi aggiorno il campo stesso per entrambi - forse non serve)
                    TwoRecordConto = _liquidAssetServices.Get2ContoCorrentes(CC.Modified);
                    // RecordContoCorrente è quello attivo in modifica quindi imposto il record derivato
                    Record2ContoCorrente = TwoRecordConto[0].Id_RowConto == RecordContoCorrente.Id_RowConto ? TwoRecordConto[1] : TwoRecordConto[0];
                    CommonFieldsEnabled = CC.Id_tipo_movimento == (int)TipologiaMovimento.Giroconto ? true : false;
                    GirocontoEnabled = CC.Id_tipo_movimento == (int)TipologiaMovimento.Giroconto ? true : false; ;
                    OperazioneEnabled = false;
                    CanUpdateDelete = true;
                }
                else if (CC.Id_Quote_Investimenti > 0 & CC.Id_tipo_movimento == (int)TipologiaMovimento.Giroconto)
                {

                    MessageBox.Show("Questo giroconto è stato gestito da `Quote investitori`" + Environment.NewLine +
                        "Qua puoi solo vederlo, se vuoi modificarlo devi andare in `Quote investitori`.", "Gestione Conto Corrente", MessageBoxButton.OK, MessageBoxImage.Information);
                    GirocontoEnabled = false;
                    CedoleEnabled = false;
                    VolatiliEnabled = false;
                    CambioValutaEnabled = false;
                    CommonFieldsEnabled = false;
                    CanUpdateDelete = false;
                    OperazioneEnabled = false;
                }
                else
                {
                    RecordContoCorrente = new ContoCorrente();
                    GirocontoEnabled = false;
                    CedoleEnabled = false;
                    VolatiliEnabled = false;
                    CambioValutaEnabled = false;
                    CommonFieldsEnabled = false;
                    CanUpdateDelete = false;
                    OperazioneEnabled = false;
                }
                CanInsert = false;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Dopo aver inserito l'importo verifico che sia congruo 
        /// rispetto alla selezione
        /// </summary>
        /// <param name="sender">TextBox</param>
        /// <param name="e">LostFocus</param>
        public void LostFocus(object sender, EventArgs e)
        {
            if (sender is TextBox TB)
            {
                switch (TB.Name)
                {
                    case ("Ammontare"):
                        RecordContoCorrente.Valore_Cambio = 1;
                        Record2ContoCorrente.Ammontare = RecordContoCorrente.Ammontare;
                        break;
                    case ("Causale"):
                        Record2ContoCorrente.Causale = TB.Text;
                        break;
                    case ("Valore_Cambio"):
                        CambioValuta(Convert.ToDouble(TB.Text));
                        CanInsert = true;
                        break;
                }
            }
        }

        private void CambioValuta(double ValoreCambio)
        {
            RecordContoCorrente.Valore_Cambio = ValoreCambio;
            Record2ContoCorrente.Ammontare = RecordContoCorrente.Valore_Cambio * RecordContoCorrente.Ammontare;
            AmountChangedValue = Record2ContoCorrente.Ammontare;
            Record2ContoCorrente.Valore_Cambio = RecordContoCorrente.Valore_Cambio > 0 ? 1 / RecordContoCorrente.Valore_Cambio : 0;

        }

        /// <summary>
        /// Controlla che il punto del tastierino numerico venga trasformato in virgola
        /// </summary>
        /// <param name="sender">Tastiera</param>
        /// <param name="e">Pressione del tasto</param>
        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && (textBox.Name == "Ammontare" || textBox.Name == "Valore_Cambio"))
                if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
                {
                    var pos = textBox.SelectionStart;
                    textBox.Text = textBox.Text.Insert(pos, ",");
                    textBox.SelectionStart = pos + 1;
                    e.Handled = true;
                }
        }

        public void TabControlLoaded(object sender, System.EventArgs e)
        {
            Border MyTabControl = sender as Border;
            MyTabControl.Child = _TabControl;
        }

        #endregion

        #region command
        public void SaveCommand(object param)
        {
            try
            {
                // In base all'operazione scelta decido:
                if (RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.Giroconto ||
                    RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.CambioValuta)
                {
                    CurrencyAvailable = _liquidAssetServices.GetCurrencyAvailable(IdGestione: RecordContoCorrente.Id_Gestione,
                        IdConto: RecordContoCorrente.Id_Conto, IdValuta: RecordContoCorrente.Id_Valuta)[0];

                    if (RecordContoCorrente.Ammontare > CurrencyAvailable.Disponibili && RecordContoCorrente.Id_Tipo_Soldi == (int)TipologiaSoldi.Capitale ||
                        RecordContoCorrente.Ammontare > CurrencyAvailable.Cedole && RecordContoCorrente.Id_Tipo_Soldi == (int)TipologiaSoldi.Utili_da_Cedole ||
                        RecordContoCorrente.Ammontare > CurrencyAvailable.Utili && RecordContoCorrente.Id_Tipo_Soldi == (int)TipologiaSoldi.Utili_da_Volatili ||
                        RecordContoCorrente.Ammontare > CurrencyAvailable.Utili && RecordContoCorrente.Id_Tipo_Soldi == (int)TipologiaSoldi.Utili_da_Vendite)
                    {
                        MessageBox.Show(String.Format("Non hai abbastanza soldi in {0} per effettuare un {1} di {2}.{3}" +
                            "Ricontrollare i parametri inseriti.", RecordContoCorrente.Cod_Valuta, RecordContoCorrente.Desc_tipo_movimento, RecordContoCorrente.Desc_Tipo_Soldi,
                            Environment.NewLine), "Gestione Conto Corrente", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    RecordContoCorrente.Ammontare = RecordContoCorrente.Ammontare * -1; //il segno dell'ammontare
                    _liquidAssetServices.InsertAccountMovement(RecordContoCorrente);
                    // imposto il date time del record 2 uguale al record 1
                    Record2ContoCorrente.Modified = RecordContoCorrente.Modified;
                    _liquidAssetServices.InsertAccountMovement(Record2ContoCorrente);
                }
                else if (RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.Cedola ||
                    RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.InsertVolatili ||
                    RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.Costi)
                {
                    // Inserisco il codice del periodo quote_guadagno
                    RecordContoCorrente.Id_Quote_Periodi = _liquidAssetServices.GetIdPeriodoQuote(RecordContoCorrente.DataMovimento, RecordContoCorrente.Id_Tipo_Soldi);
                    _liquidAssetServices.InsertAccountMovement(RecordContoCorrente);
                    // Inserisco il guadagno ripartito per i soci
                    _liquidAssetServices.AddSingoloGuadagno(RecordContoCorrente);
                }

                MessageBox.Show(string.Format("Ho effettuato l'operazione {0} correttamente.", RecordContoCorrente.Desc_tipo_movimento),
                    Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                Init();
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi nel caricamento del record: " + Environment.NewLine +
                    err.Message, Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void UpdateCommand(object param)
        {
            try
            {
                // se è una registrazione cedola modifico direttamente il record 1
                if (RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.Cedola ||
                    RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.InsertVolatili ||
                    RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.Costi)
                {
                    // nel caso si sia cambiata la data nella modifica
                    RecordContoCorrente.Id_Quote_Periodi = _liquidAssetServices.GetIdPeriodoQuote(RecordContoCorrente.DataMovimento, RecordContoCorrente.Id_Tipo_Soldi);
                    _liquidAssetServices.UpdateRecordContoCorrente(RecordContoCorrente, TipologiaIDContoCorrente.IdContoCorrente);    //registro la modifica in conto corrente
                    _liquidAssetServices.ModifySingoloGuadagno(RecordContoCorrente); // modifico di conseguenza i record del guadagno totale anno
                }
                else
                {
                    //// cerco il record corrispondente al giroconto o al cambio valuta utilizzando il campo
                    //// modified (e poi aggiorno il campo stesso per entrambi - forse non serve)
                    //TwoRecordConto = _liquidAssetServices.Get2ContoCorrentes(RecordContoCorrente.Modified);
                    //// RecordContoCorrente è quello attivo in modifica quindi imposto il record derivato
                    //Record2ContoCorrente = TwoRecordConto[0].Id_RowConto == RecordContoCorrente.Id_RowConto ? TwoRecordConto[1] : TwoRecordConto[0];
                    // Salvo le modifiche fatte al record attivo
                    _liquidAssetServices.UpdateRecordContoCorrente(RecordContoCorrente, TipologiaIDContoCorrente.IdContoCorrente);
                    if (RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.CambioValuta)
                    {
                        if (RecordContoCorrente.Ammontare * RecordContoCorrente.Valore_Cambio != Record2ContoCorrente.Ammontare)
                        {
                            Record2ContoCorrente.Ammontare = RecordContoCorrente.Ammontare * RecordContoCorrente.Valore_Cambio;
                            Record2ContoCorrente.Valore_Cambio = 1 / RecordContoCorrente.Valore_Cambio;
                        }
                        Record2ContoCorrente.Id_Tipo_Soldi = RecordContoCorrente.Id_Tipo_Soldi;
                        Record2ContoCorrente.DataMovimento = RecordContoCorrente.DataMovimento;
                        Record2ContoCorrente.Causale = RecordContoCorrente.Causale;
                        // Salvo le modifiche al record collegato
                        _liquidAssetServices.UpdateRecordContoCorrente(Record2ContoCorrente, TipologiaIDContoCorrente.IdContoCorrente);
                    }
                    else if (RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.Giroconto)
                    {
                        // modifico il record 2 sulla base delle modifiche apportate al record 1
                        Record2ContoCorrente.Id_Tipo_Soldi = RecordContoCorrente.Id_Tipo_Soldi;
                        Record2ContoCorrente.DataMovimento = RecordContoCorrente.DataMovimento;
                        Record2ContoCorrente.Ammontare = RecordContoCorrente.Ammontare * -1;
                        Record2ContoCorrente.Causale = RecordContoCorrente.Causale;
                        // Salvo le modifiche al record collegato
                        _liquidAssetServices.UpdateRecordContoCorrente(Record2ContoCorrente, TipologiaIDContoCorrente.IdContoCorrente);
                    }
                }
                MessageBox.Show("Record modificato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                Init();
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi nel modificare il record" + Environment.NewLine + err.Message, Application.Current.FindResource("DAF_Caption").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void DeleteCommand(object param)
        {
            try
            {
                if (RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.Cedola ||
                    RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.InsertVolatili ||
                    RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.Costi)
                {
                    // con il codice del record elimino anche le righe inserite nella tabella guadagno
                    _liquidAssetServices.DeleteRecordGuadagno_Totale_anno(RecordContoCorrente.Id_RowConto);
                    _liquidAssetServices.DeleteRecordContoCorrente(RecordContoCorrente.Id_RowConto);  // registro l'eliminazione in conto corrente
                }
                else
                {
                    // Ho trovato i 2 record collegati quando ho fatto la selezione dalla griglia
                    _liquidAssetServices.DeleteRecordContoCorrente(RecordContoCorrente.Id_RowConto);
                    _liquidAssetServices.DeleteRecordContoCorrente(Record2ContoCorrente.Id_RowConto);
                }
                MessageBox.Show("Record eliminato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                Init();
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi nell'eliminare il record" + Environment.NewLine + err.Message, Application.Current.FindResource("DAF_Caption").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void CleanCommand(object param)
        {
            Init();
        }

        public void CloseMe(object param)
        {
            GestioneContoCorrenteView MFMV = param as GestioneContoCorrenteView;
            DockPanel wp = MFMV.Parent as DockPanel;
            wp.Children.Remove(MFMV);
        }

        public bool CanInsert
        {
            get { return GetValue(() => CanInsert); }
            set { SetValue(() => CanInsert, value); }
        }

        public bool CanSave(object param)
        {
            return CanInsert;
        }

        public bool CanUpdateDelete
        {
            get { return GetValue(() => CanUpdateDelete); }
            set { SetValue(() => CanUpdateDelete, value); }
        }

        public bool CanModify(object param)
        {
            if (CanUpdateDelete)
                return true;
            return false;
        }

        #endregion
    }
}

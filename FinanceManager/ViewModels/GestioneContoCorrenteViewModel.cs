using FinanceManager.Events;
using FinanceManager.Models;
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
        Predicate<object> _Filter;

        public GestioneContoCorrenteViewModel(IRegistryServices registryServices, IManagerLiquidAssetServices managerLiquidServices)
        {
            _registryServices = registryServices ?? throw new ArgumentNullException("Registry Services in Gestione Conto Corrente");
            _liquidAssetServices = managerLiquidServices ?? throw new ArgumentNullException("Registry Services in Gestione Conto Corrente");
            CloseMeCommand = new CommandHandler(CloseMe);
            InsertCommand = new CommandHandler(SaveCommand, CanSave);
            ModifyCommand = new CommandHandler(UpdateCommand, CanModify);
            EraseCommand = new CommandHandler(DeleteCommand, CanModify);
            ClearCommand = new CommandHandler(CleanCommand);
            SetUpData();
            Init();
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
                           where (movimento.Id_tipo_movimento == 3 || movimento.Id_tipo_movimento == 4 || movimento.Id_tipo_movimento == 12)
                           select movimento;
                foreach (RegistryMovementType registry in RMTL)
                    ListMovimenti.Add(registry);
                ListValute = _registryServices.GetRegistryCurrencyList();
                ListGestioni = _registryServices.GetRegistryOwners();
                ListConti = _registryServices.GetRegistryLocationList();
                TipoSoldis = _registryServices.GetTipoSoldiList();
                SharesList = new ObservableCollection<RegistryShare>(_registryServices.GetRegistryShareList());
                _Filter = new Predicate<object>(Filter);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Set up Acquisto Vendita Titoli", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Init()
        {
            Record2ContoCorrente = new ContoCorrente();
            RecordContoCorrente = new ContoCorrente();
            AmountChangedValue = 0;
            SintesiSoldiR = _liquidAssetServices.GetCurrencyAvailable(1);
            SintesiSoldiDF = _liquidAssetServices.GetCurrencyAvailable(2);
            SrchShares = "";
            ListContoCorrente = _liquidAssetServices.GetContoCorrenteList();
            CambioValutaEnabled = false;
            CedoleEnabled = false;
            GirocontoEnabled = false;
            CanInsert = false;
        }

        #region Getter&Setter
        /// <summary>
        /// il riepilogo dei soldi per la gestione Dany&Fla
        /// </summary>
        public SintesiSoldiList SintesiSoldiDF
        {
            get { return GetValue(() => SintesiSoldiDF); }
            private set { SetValue(() => SintesiSoldiDF, value); }
        }

        /// <summary>
        /// il riepilogo dei soldi per la gestione Rubiu
        /// </summary>
        public SintesiSoldiList SintesiSoldiR
        {
            get { return GetValue(() => SintesiSoldiR); }
            private set { SetValue(() => SintesiSoldiR, value); }
        }

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
            set { SetValue(() => ListContoCorrente, value); }
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
            }
            return true;
        }

        /// <summary>
        /// Gestisce l'abilitazione del blocco dedicato al cambiavalute
        /// </summary>
        public bool CambioValutaEnabled
        {
            get { return GetValue(() => CambioValutaEnabled); }
            private set { SetValue(() => CambioValutaEnabled, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione del blocco dedicato alla registrazione delle cedole
        /// </summary>
        public bool CedoleEnabled
        {
            get { return GetValue(() => CedoleEnabled); }
            private set { SetValue(() => CedoleEnabled, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione del blocco dedicato al giroconto
        /// </summary>
        public bool GirocontoEnabled
        {
            get { return GetValue(() => GirocontoEnabled); }
            private set { SetValue(() => GirocontoEnabled, value); }
        }


        #endregion

        #region Events

        /// <summary>
        /// Levento al cambio di selezione del record nella griglia dell'investitore
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0].GetType().Name == "DateTime")
                {
                    RecordContoCorrente.DataMovimento = (DateTime)e.AddedItems[0];
                    Record2ContoCorrente.DataMovimento = (DateTime)e.AddedItems[0];
                }
                else if (e.AddedItems[0] is RegistryLocation RL)
                {
                    if (((ComboBox)e.OriginalSource).Name == "Conto2")
                    {
                        Record2ContoCorrente.Id_Conto = RL.Id_conto;
                        Record2ContoCorrente.Desc_Conto = RL.Desc_conto;
                    }
                    else
                    {
                        RecordContoCorrente.Id_Conto = RL.Id_conto;
                        RecordContoCorrente.Desc_Conto = RL.Desc_conto;
                        Record2ContoCorrente.Id_Conto = RL.Id_conto;
                        Record2ContoCorrente.Desc_Conto = RL.Desc_conto;
                    }
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
                        Record2ContoCorrente.Id_Gestione = RO.Id_gestione;
                        Record2ContoCorrente.NomeGestione = RO.Nome_Gestione;
                    }
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
                    RecordContoCorrente.Id_Tipo_Soldi = (Models.Enum.TipologiaSoldi) TS.Id_Tipo_Soldi;
                    RecordContoCorrente.Desc_Tipo_Soldi = TS.Desc_Tipo_Soldi;
                    Record2ContoCorrente.Id_Tipo_Soldi = (Models.Enum.TipologiaSoldi)TS.Id_Tipo_Soldi;
                    Record2ContoCorrente.Desc_Tipo_Soldi = TS.Desc_Tipo_Soldi;
                }
                else if (e.AddedItems[0] is RegistryMovementType RMT)
                {
                    RecordContoCorrente.Id_tipo_movimento = RMT.Id_tipo_movimento;
                    RecordContoCorrente.Desc_tipo_movimento = RMT.Desc_tipo_movimento;
                    Record2ContoCorrente.Id_tipo_movimento = RMT.Id_tipo_movimento;
                    Record2ContoCorrente.Desc_tipo_movimento = RMT.Desc_tipo_movimento;
                    // abilito il blocco di input dati sulla base di questa scelta
                    CambioValutaEnabled = RMT.Id_tipo_movimento == 3 ? true : false;
                    CedoleEnabled = RMT.Id_tipo_movimento == 4 ? true : false;
                    GirocontoEnabled = RMT.Id_tipo_movimento == 12 ? true : false;
                }
                else if (e.AddedItems[0] is RegistryShare RS)
                {
                    RecordContoCorrente.Id_Titolo = (int)RS.IdShare;
                    CanInsert = true;
                }
            }
        }

        /// <summary>
        /// Imposto i campi sopra la griglia quando viene selezionata una riga
        /// </summary>
        /// <param name="sender">Grid dei dati</param>
        /// <param name="e">Cambio di selezione</param>
        public void GridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                e.Handled = true;
                return;
            }
            if (e.AddedItems[0] is ContoCorrente CC)
            {
                RecordContoCorrente = CC;

                CanUpdateDelete = true;
                CanInsert = false;
            }
        }

        public void StartNewRow(object sender, AddingNewItemEventArgs e)
        {
            //ActualQuote = new QuoteTab();
            //e.NewItem = ActualQuote;
            //DataMovimento = DateTime.Now.Date;
        }

        /// <summary>
        /// Gestisco la cancellazione del record
        /// </summary>
        /// <param name="sender">tastiera</param>
        /// <param name="e">tasto premuto</param>
        public void DeleteRow(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                try
                {
                    DataGrid dataGrid = sender as DataGrid;
                    QuoteTab quoteTab = dataGrid.SelectedItem as QuoteTab;
                    if (quoteTab.IdQuote == 0) return;
                    // verifico che tipo di movimento voglio cancellare
                    if (quoteTab.Id_tipo_movimento == 1 || quoteTab.Id_tipo_movimento == 2)
                    {
                        //_managerLiquidServices.DeleteRecordQuoteTab(quoteTab.IdQuote);
                        MessageBox.Show("Il record è stato correttamente eliminato", "Gestione AndQuote Investitori", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        var Risposta = MessageBox.Show("Stai per eliminare il record selezionato da 2 tabelle." + Environment.NewLine + "Sei sicoro?",
                            "Gestione AndQuote", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (Risposta == MessageBoxResult.Yes)
                        {
                            //_managerLiquidServices.DeleteAccount(ListContoCorrente[0].Id_RowConto);
                            //_managerLiquidServices.DeleteRecordQuoteTab(ActualQuote.IdQuote);
                            //ActualQuote = new QuoteTab();
                            MessageBox.Show("I 2 record sono stati correttamente eliminati", "Gestione AndQuote Investitori", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    //UpdateCollection();     //Aggiorno tutti i dati della griglia
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
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
                        RecordContoCorrente.Valore_Cambio = Convert.ToDouble(TB.Text);
                        Record2ContoCorrente.Ammontare = RecordContoCorrente.Valore_Cambio * RecordContoCorrente.Ammontare;
                        AmountChangedValue = Record2ContoCorrente.Ammontare;
                        Record2ContoCorrente.Valore_Cambio = RecordContoCorrente.Valore_Cambio > 0 ? 1 / RecordContoCorrente.Valore_Cambio : 0;
                        CanInsert = true;
                        break;
                }
            }
        }

        #endregion

        #region command
        public void SaveCommand(object param)
        {
            try
            {
                // In base all'operazione scelta decido:
                if (RecordContoCorrente.Id_tipo_movimento == 3 || RecordContoCorrente.Id_tipo_movimento == 12)
                {
                    CurrencyAvailable = _liquidAssetServices.GetCurrencyAvailable(IdGestione: RecordContoCorrente.Id_Gestione,
                        IdConto: RecordContoCorrente.Id_Conto, IdValuta: RecordContoCorrente.Id_Valuta)[0];

                    if ( RecordContoCorrente.Ammontare > CurrencyAvailable.Disponibili && RecordContoCorrente.Id_Tipo_Soldi == Models.Enum.TipologiaSoldi.Capitale || 
                        RecordContoCorrente.Ammontare > CurrencyAvailable.Cedole && RecordContoCorrente.Id_Tipo_Soldi == Models.Enum.TipologiaSoldi.Cedole || 
                        RecordContoCorrente.Ammontare > CurrencyAvailable.Utili && RecordContoCorrente.Id_Tipo_Soldi == Models.Enum.TipologiaSoldi.Utili)
                    {
                        MessageBox.Show(String.Format("Non hai abbastanza soldi in {0} per effettuare un {1} di {2}.{3}" +
                            "Ricontrollare i parametri inseriti.", RecordContoCorrente.Cod_Valuta, RecordContoCorrente.Desc_tipo_movimento, RecordContoCorrente.Desc_Tipo_Soldi,
                            Environment.NewLine), "Gestione Conto Corrente", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    RecordContoCorrente.Ammontare = RecordContoCorrente.Ammontare * -1; //il segno dell'ammontare
                    _liquidAssetServices.InsertAccountMovement(RecordContoCorrente);
                    _liquidAssetServices.InsertAccountMovement(Record2ContoCorrente);
                }
                else if(RecordContoCorrente.Id_tipo_movimento == 4)
                {
                    _liquidAssetServices.InsertAccountMovement(RecordContoCorrente);
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
                //_liquidAssetServices.UpdateConto(RecordContoCorrente);    //registro la modifica in conto corrente

                // aggiorno la disponibilità
                Init();
                
                MessageBox.Show("Record modificato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
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
                //_liquidAssetServices.DeleteManagerLiquidAsset;  // registro l'eliminazione in conto corrente
                SintesiSoldiR = _liquidAssetServices.GetCurrencyAvailable(1);                           // aggiorno la disponibilità
                SintesiSoldiDF = _liquidAssetServices.GetCurrencyAvailable(2);                          // aggiorno la disponibilità
                Init();
                MessageBox.Show("Record eliminato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
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

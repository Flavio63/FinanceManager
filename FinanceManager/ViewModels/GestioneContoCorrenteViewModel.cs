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
            SintesiSoldiR = _liquidAssetServices.GetCurrencyAvailable(1);
            SintesiSoldiDF = _liquidAssetServices.GetCurrencyAvailable(2);
            SetUpData();
        }

        private void SetUpData()
        {
            try
            {
                ListMovimenti = new RegistryMovementTypeList();
                ListGestioni = new RegistryOwnersList();
                ListConti = new RegistryLocationList();
                ListValute = new RegistryCurrencyList();
                RegistryMovementTypeList listaOriginale = new RegistryMovementTypeList();
                listaOriginale = _registryServices.GetRegistryMovementTypesList();
                var RMTL = from movimento in listaOriginale
                           where (movimento.Id_tipo_movimento == 5 || movimento.Id_tipo_movimento == 6 || movimento.Id_tipo_movimento == 13 || movimento.Id_tipo_movimento == 14)
                           select movimento;
                foreach (RegistryMovementType registry in RMTL)
                    ListMovimenti.Add(registry);
                ListValute = _registryServices.GetRegistryCurrencyList();
                ListGestioni = _registryServices.GetRegistryOwners();
                ListConti = _registryServices.GetRegistryLocationList();
                ListContoCorrente = _liquidAssetServices.GetContoCorrenteList();
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
            CloseMeCommand = new CommandHandler(CloseMe);
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
        /// Totale Contabile convertito in euro
        /// </summary>
        public double AmountChangedValue
        {
            get { return GetValue<double>(() => AmountChangedValue); }
            set { SetValue<double>(() => AmountChangedValue, value); }
        }

        /// <summary>
        /// E' la valuta disponibile per effettuare acquisti
        /// </summary>
        public double CurrencyAvailable
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
        /// E' il totale comprensivo delle commissioni nella valuta locale
        /// </summary>
        public double TotalLocalValue
        {
            get { return GetValue<double>("TotalLocalValue"); }
            set { SetValue("TotalLocalValue", value); }
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
            //    if (e.AddedItems[0].GetType().Name == "DateTime")
            //    {
            //        DataMovimento = (DateTime)e.AddedItems[0];
            //        return;
            //    }
            //    if (e.AddedItems[0] is Investitore investitore)
            //    {
            //        ActualQuote.IdInvestitore = investitore.IdInvestitore;
            //        ActualQuote.NomeInvestitore = investitore.NomeInvestitore;
            //        return;
            //    }
            //    if (e.AddedItems[0].GetType().Name == "QuoteTab")
            //    {
            //        ActualQuote = e.AddedItems[0] as QuoteTab;
            //        QuoteTabPrevious = new QuoteTab()
            //        {
            //            IdQuote = ActualQuote.IdQuote,
            //            IdInvestitore = ActualQuote.IdInvestitore,
            //            Id_tipo_movimento = ActualQuote.Id_tipo_movimento,
            //            Ammontare = ActualQuote.Ammontare,
            //            DataMovimento = ActualQuote.DataMovimento,
            //            Note = ActualQuote.Note,
            //            Desc_tipo_movimento = ActualQuote.Desc_tipo_movimento,
            //            NomeInvestitore = ActualQuote.NomeInvestitore
            //        };
            //        if (ActualQuote.Id_tipo_movimento != 1 && ActualQuote.Id_tipo_movimento != 2 && ActualQuote.IdQuote > 0)
            //        {
            //            // estraggo solo il record corrispondente alla selezione nella griglia quote
            //            ListContoCorrente = _managerLiquidServices.GetContoCorrenteByIdQuote(ActualQuote.IdQuote);
            //            //ListContoCorrente = _managerLiquidServices.GetContoCorrenteByMovement(12);
            //        }
            //        else if (ActualQuote.Id_tipo_movimento == 12 && ActualQuote.Id_tipo_movimento == 0)
            //        {
            //            // estraggo tutti i record con codice "giroconto"
            //            ListContoCorrente = _managerLiquidServices.GetContoCorrenteByMovement(12);
            //        }
            //        else
            //        {
            //            ListContoCorrente = _managerLiquidServices.GetContoCorrenteByMovement(12);
            //        }
            //    }
            }
        }

        /// <summary>
        /// E' l'evento di edit nella cella di descrizione della gestione
        /// se il modello ha un valore di id vuol dire che è in modifica
        /// se il valore è zero vuol dire che è un inserimento di nuova gestione
        /// </summary>
        /// <param name="sender">la cella di descrizione</param>
        /// <param name="e">la conferma o meno della modifica</param>
        public void RowChanged(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
            //    if (e.EditAction == DataGridEditAction.Commit && !quoteTabComparator.Equals(QuoteTabPrevious, ActualQuote))
            //    {
            //        // se il movimento è 1 o 2 oppure se è 4 o 15 con cifra negativa allora l'operazione si scrive solo sulla
            //        // tabella quote_investitori
            //        if (ActualQuote.Id_tipo_movimento == 1 || ActualQuote.Id_tipo_movimento == 2 ||
            //            (ActualQuote.Ammontare < 0 && (ActualQuote.Id_tipo_movimento == 4 || ActualQuote.Id_tipo_movimento == 15)))
            //        {
            //            if (!VerifyQuoteTabOperation())
            //            {
            //                ActualQuote = QuoteTabPrevious;
            //                QuoteTabPrevious = new QuoteTab();
            //                return;
            //            }
            //            if (ActualQuote.IdQuote > 0)
            //            {
            //                _managerLiquidServices.UpdateQuoteTab(ActualQuote);
            //            }
            //            else
            //            {
            //                _managerLiquidServices.InsertInvestment(ActualQuote);
            //            }
            //            ListQuote = _managerLiquidServices.GetQuote();
            //            ListTabQuote = _managerLiquidServices.GetQuoteTab();
            //            SintesiSoldiR = _managerLiquidServices.GetCurrencyAvailable(1);
            //            SintesiSoldiDF = _managerLiquidServices.GetCurrencyAvailable(2);
            //            ListContoCorrente = _managerLiquidServices.GetContoCorrenteByMovement(12);
            //            QuoteTabPrevious = new QuoteTab();
            //        }
            //        else if ((ActualQuote.Id_tipo_movimento == 12 && ActualQuote.IdQuote == 0) ||
            //            (ActualQuote.Ammontare > 0 && ActualQuote.IdQuote == 0 && (ActualQuote.Id_tipo_movimento == 4 || ActualQuote.Id_tipo_movimento == 15)))
            //        {
            //            if (OnOpenDialog(this) == true)
            //            {
            //                _managerLiquidServices.InsertInvestment(ActualQuote);                           // aggiungo il record al db
            //                ActualQuote.IdQuote = ((QuoteTab)_managerLiquidServices.GetLastQuoteTab()).IdQuote; // aggiorno il record selezionato
            //                ContoCorrente cc = new ContoCorrente();                 // nuovo record
            //                cc.Ammontare = ActualQuote.Ammontare * -1;              // la cifra inversa di quella inserita in gestione investimenti
            //                cc.DataMovimento = ActualQuote.DataMovimento;          // la data dell'operazione
            //                cc.Id_Quote_Investimenti = ActualQuote.IdQuote;         // il record di riferimento della gestione investimenti
            //                cc.Id_Valuta = 1;                                       // codice valuta (sempre euro)
            //                cc.Cod_Valuta = "EUR";
            //                cc.Causale = ActualQuote.Note;                          // le stesse note
            //                cc.Id_Portafoglio_Titoli = 0;
            //                cc.Id_tipo_movimento = ActualQuote.Id_tipo_movimento;
            //                cc.Id_Titolo = 0;
            //                cc.Id_Gestione = RegistryOwner.Id_gestione;
            //                cc.NomeGestione = RegistryOwner.Nome_Gestione;
            //                cc.Id_Conto = RegistryLocation.Id_conto;
            //                cc.Desc_Conto = RegistryLocation.Desc_conto;
            //                cc.Valore_Cambio = 1;
            //                cc.Id_Tipo_Soldi = 1;
            //                _managerLiquidServices.InsertAccountMovement(cc);                               // aggiungo il record al db
            //                ListQuote = _managerLiquidServices.GetQuote();
            //                ListTabQuote = _managerLiquidServices.GetQuoteTab();
            //                SintesiSoldiR = _managerLiquidServices.GetCurrencyAvailable(1);
            //                SintesiSoldiDF = _managerLiquidServices.GetCurrencyAvailable(2);
            //                ListContoCorrente = _managerLiquidServices.GetContoCorrenteByMovement(12);
            //            }
            //            else
            //            {
            //                ActualQuote = QuoteTabPrevious;     // riporto la situazione all'origine
            //            }
            //        }
            //        else if ((ActualQuote.Id_tipo_movimento == 12 && ActualQuote.IdQuote > 0) ||
            //            ((ActualQuote.Id_tipo_movimento == 4 || ActualQuote.Id_tipo_movimento == 15) && ActualQuote.IdQuote > 0))
            //        {
            //            if (OnOpenDialog(this))
            //            {
            //                _managerLiquidServices.UpdateQuoteTab(ActualQuote);         // aggiorno i dati nel db
            //                ContoCorrente cc = ListContoCorrente[0];
            //                if (cc.Id_Quote_Investimenti == ActualQuote.IdQuote)
            //                {
            //                    cc.Ammontare = ActualQuote.Ammontare * -1;              // la cifra inversa di quella inserita in gestione investimenti
            //                    cc.DataMovimento = ActualQuote.DataMovimento;          // la data dell'operazione
            //                    cc.Causale = ActualQuote.Note;                          // le stesse note
            //                    cc.Id_Gestione = RegistryOwner.Id_gestione;
            //                    cc.NomeGestione = RegistryOwner.Nome_Gestione;
            //                    cc.Id_Conto = RegistryLocation.Id_conto;
            //                    cc.Desc_Conto = RegistryLocation.Desc_conto;
            //                    _managerLiquidServices.UpdateContoCorrenteByIdQuote(cc);         // aggiorno i dati nel db
            //                    ListQuote = _managerLiquidServices.GetQuote();
            //                    ListTabQuote = _managerLiquidServices.GetQuoteTab();
            //                    SintesiSoldiR = _managerLiquidServices.GetCurrencyAvailable(1);
            //                    SintesiSoldiDF = _managerLiquidServices.GetCurrencyAvailable(2);
            //                    ListContoCorrente = _managerLiquidServices.GetContoCorrenteByMovement(12);
            //                }
            //            }
            //            else
            //            {
            //                ActualQuote = QuoteTabPrevious;         // Riporta i dati com'erano
            //            }
            //        }
            //    }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Gestione AndQuote", MessageBoxButton.OK, MessageBoxImage.Error);
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
            //if (sender is TextBox TB && TB.Name == "txtAmmontare")
            //    if (!VerifyQuoteTabOperation())
            //        return;
        }

        #endregion

        #region command
        public void SaveCommand(object param)
        {
            try
            {
                // verifico la disponibilità di liquidità in conto corrente
                if (CurrencyAvailable < Math.Abs(TotalLocalValue))
                {
                    MessageBox.Show("Non hai abbastanza soldi per questo acquisto!", "Acquisto Vendita Titoli", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                _liquidAssetServices.InsertAccountMovement(RecordContoCorrente);     // inserisco il o i movimenti in conto corrente

                SrchShares = "";
                // aggiorno la disponibilità
                SintesiSoldiR = _liquidAssetServices.GetCurrencyAvailable(1);
                SintesiSoldiDF = _liquidAssetServices.GetCurrencyAvailable(2);

                MessageBox.Show("Record caricato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
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
                _liquidAssetServices.UpdateContoCorrenteByIdPortafoglioTitoli(RecordContoCorrente);    //registro la modifica in conto corrente

                // aggiorno la disponibilità
                SintesiSoldiR = _liquidAssetServices.GetCurrencyAvailable(1);
                SintesiSoldiDF = _liquidAssetServices.GetCurrencyAvailable(2);

                SrchShares = "";
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

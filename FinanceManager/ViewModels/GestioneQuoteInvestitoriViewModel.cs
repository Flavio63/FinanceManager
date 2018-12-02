using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using FinanceManager.Comparators;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class GestioneQuoteInvestitoriViewModel : ViewModelBase
    {
        private IRegistryServices _registryServices;
        private IManagerLiquidAssetServices _managerLiquidServices;
        public ICommand CloseMeCommand { get; set; }
        private QuoteTabComparator quoteTabComparator = new QuoteTabComparator();

        public GestioneQuoteInvestitoriViewModel(IRegistryServices registryServices, IManagerLiquidAssetServices managerLiquidServices)
        {
            _registryServices = registryServices ?? throw new ArgumentNullException("Services in Manager Portfolio Movement View Model");
            _managerLiquidServices = managerLiquidServices ?? throw new ArgumentNullException("Liquid Asset Services in Manager Portfolio Movement View Model");

            Init();

            ListQuote = _managerLiquidServices.GetQuote();
            ListTabQuote = _managerLiquidServices.GetQuoteTab();
            ListInvestitore = _managerLiquidServices.GetInvestitori();
            ListMovementType = _registryServices.GetRegistryMovementTypesList();
            
            DataMovimento = DateTime.Now.Date;

            Visibility2 = Visibility.Collapsed;
        }

        private void Init()
        {
            CloseMeCommand = new CommandHandler(CloseMe);
            ListQuote = new QuoteList();
            ListTabQuote = new QuoteTabList();
            ListInvestitore = new InvestitoreList();
            ListMovementType = new RegistryMovementTypeList();
            ListContoCorrente = new ContoCorrenteList();
            ActualQuote = new QuoteTab();
        }

        #region Getter&Setter
        /// <summary>
        /// Visualizza la griglia con le modifiche fatte alla tabella conto corrente
        /// </summary>
        public Visibility Visibility2
        {
            get { return GetValue(() => Visibility2); }
            set { SetValue(() => Visibility2, value); }
        }

        /// <summary>
        /// Gestisce l'elenco dei tipi di movimento
        /// </summary>
        public RegistryMovementTypeList ListMovementType
        {
            get { return GetValue(() => ListMovementType); }
            set { SetValue(() => ListMovementType, value); }
        }
        /// <summary>
        /// Aggiorna la tabella con le quote degli investitori
        /// </summary>
        public QuoteList ListQuote
        {
            get { return GetValue(() => ListQuote); }
            set { SetValue(() => ListQuote, value); }
        }
        /// <summary>
        /// Aggiorna la tabella degli investitori
        /// </summary>
        public QuoteTabList ListTabQuote
        {
            get { return GetValue(() => ListTabQuote); }
            set { SetValue(() => ListTabQuote, value); }
        }
        /// <summary>
        /// la lista degli investitori
        /// </summary>
        public InvestitoreList ListInvestitore
        {
            get { return GetValue(() => ListInvestitore); }
            set { SetValue(() => ListInvestitore, value); }
        }
        /// <summary>
        /// il record selezionato nella griglia dei record 
        /// </summary>
        public QuoteTab ActualQuote
        {
            get { return GetValue(() => ActualQuote); }
            set { SetValue(() => ActualQuote, value); }
        }
        /// <summary>
        /// Memorizzo il record prima della modifica
        /// </summary>
        public QuoteTab QuoteTabPrevious
        {
            get { return GetValue(() => QuoteTabPrevious); }
            set { SetValue(() => QuoteTabPrevious, value); }
        }

        /// <summary>
        /// Memorizzo i record dei conto corrente
        /// </summary>
        public ContoCorrenteList ListContoCorrente
        {
            get { return GetValue(() => ListContoCorrente); }
            set { SetValue(() => ListContoCorrente, value); }
        }

        public DateTime DataMovimento
        {
            get { return GetValue(() => DataMovimento); }
            set
            {
                SetValue(() => DataMovimento, value);
                ActualQuote.DataMovimento = value;
            }
        }

        #endregion
        
        /// <summary>
        /// Verifica che il codice dell'operazione corrisponda al segno della cifra
        /// </summary>
        /// <returns>True or False</returns>
        private bool VerifyQuoteTabOperation()
        {
            if (ActualQuote.Id_tipo_movimento == 2 && ActualQuote.Ammontare > 0)
            {
                MessageBox.Show("Attenzione devi inserire una cifra negativa se vuoi prelevare",
                    "Gestione Quote", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (ActualQuote.Id_tipo_movimento == 1 && ActualQuote.Ammontare < 0)
            {
                MessageBox.Show("Attenzione devi inserire una cifra positiva se vuoi versare",
                    "Gestione Quote", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

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
                    DataMovimento = (DateTime)e.AddedItems[0];
                    return;
                }
                if (e.AddedItems[0] is Investitore investitore)
                {
                    ActualQuote.IdInvestitore = investitore.IdInvestitore;
                    ActualQuote.NomeInvestitore = investitore.NomeInvestitore;
                    return;
                }
                if (e.AddedItems[0].GetType().Name == "QuoteTab")
                {
                    ActualQuote = e.AddedItems[0] as QuoteTab;
                    QuoteTabPrevious = new QuoteTab()
                    {
                        IdQuote = ActualQuote.IdQuote,
                        IdInvestitore = ActualQuote.IdInvestitore,
                        Id_tipo_movimento = ActualQuote.Id_tipo_movimento,
                        Ammontare = ActualQuote.Ammontare,
                        DataMovimento = ActualQuote.DataMovimento,
                        Note = ActualQuote.Note,
                        Desc_tipo_movimento = ActualQuote.Desc_tipo_movimento,
                        NomeInvestitore = ActualQuote.NomeInvestitore
                    };
                }
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
                if (e.EditAction == DataGridEditAction.Commit && !quoteTabComparator.Equals(QuoteTabPrevious, ActualQuote))
                {
                    if (ActualQuote.Id_tipo_movimento == 1 || ActualQuote.Id_tipo_movimento == 2)
                    {
                        if (!VerifyQuoteTabOperation())
                        {
                            ActualQuote = QuoteTabPrevious;
                            QuoteTabPrevious = new QuoteTab();
                            ListTabQuote = _managerLiquidServices.GetQuoteTab();
                            return;
                        }
                        if (ActualQuote.IdQuote > 0)
                        {
                            _managerLiquidServices.UpdateQuoteTab(ActualQuote);
                        }
                        else
                        {
                            _managerLiquidServices.InsertInvestment(ActualQuote);
                            ListTabQuote = _managerLiquidServices.GetQuoteTab();
                            ListQuote = _managerLiquidServices.GetQuote();
                        }
                        QuoteTabPrevious = new QuoteTab();
                        ListQuote = _managerLiquidServices.GetQuote();
                    }
                    else if (ActualQuote.Id_tipo_movimento == 12 && ActualQuote.IdQuote == 0)
                    {
                        OnOpenDialog(this);
                    }
                    else
                    {
                        // Riporta i dati com'erano
                        ActualQuote = QuoteTabPrevious;
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Gestione Quote", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void StartNewRow(object sender, AddingNewItemEventArgs e)
        {
            ActualQuote = new QuoteTab();
            e.NewItem = ActualQuote;
            DataMovimento = DateTime.Now.Date;
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
                    // verifico che tipo di movimento voglio cancellare
                    if (quoteTab.Id_tipo_movimento == 1 || quoteTab.Id_tipo_movimento == 2)
                    {
                        _managerLiquidServices.DeleteRecordQuoteTab(quoteTab.IdQuote);
                        ListTabQuote = _managerLiquidServices.GetQuoteTab();
                        ListQuote = _managerLiquidServices.GetQuote();
                        MessageBox.Show("Il record è stato correttamente eliminato", "Gestione Quote Investitori", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        ListTabQuote = _managerLiquidServices.GetQuoteTab();
                        ListQuote = _managerLiquidServices.GetQuote();
                        MessageBox.Show("Il record selezionato non si può, al momento, eliminare", "Gestione Quote Investitori", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
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
            if (sender is TextBox TB && TB.Name == "txtAmmontare")
                if (!VerifyQuoteTabOperation())
                    return;
        }

        #endregion

        #region Commands
        public void AzioneScelta(object param)
        {
            string scelta = param.ToString();
            if (scelta == "Giroconto")
            {
                ActualQuote = new QuoteTab();
                QuoteTabPrevious = new QuoteTab();
                DataMovimento = DateTime.Now.Date;
                ListTabQuote = _managerLiquidServices.GetQuoteTab();
                ListContoCorrente = _managerLiquidServices.GetContoCorrenteByMovement(12);
                Visibility2 = Visibility.Visible;
            }
        }
        public void SaveCommand(object param)
        {
            try
            {
                _managerLiquidServices.InsertInvestment(ActualQuote);
                ActualQuote = new QuoteTab();
                Visibility2 = Visibility.Collapsed;
                MessageBox.Show("Operazione eseguita correttamente.", "Gestione Quote Investitori", MessageBoxButton.OK, MessageBoxImage.Information);
                ListQuote = _managerLiquidServices.GetQuote();
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi con l'operazione richiesta." + Environment.NewLine + err.Message, "Gestione Quote Investitori", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateCommand(Object param)
        {
            try
            {
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public void CloseMe(object param)
        {
            GestioneQuoteInvestitoriView view = param as GestioneQuoteInvestitoriView;
            DockPanel wp = view.Parent as DockPanel;
            wp.Children.Remove(view);
        }

        private void OnOpenDialog(object param)
        {
            Dialogs.DialogService.DialogViewModelBase vm = new 
                Dialogs.DialogYesNo.DialogYesNoViewModel("Selezionare il conto e la gestione", _registryServices.GetRegistryLocationList(), _registryServices.GetRegistryOwners());
            Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(vm, param as Window);
            if (result == Dialogs.DialogService.DialogResult.Yes)
            {
                Visibility2 = Visibility.Visible;
                _managerLiquidServices.InsertInvestment(ActualQuote);
                int IdQuote = ((QuoteTab) _managerLiquidServices.GetLastQuoteTab()).IdQuote;
                ActualQuote.IdQuote = IdQuote;
                ContoCorrente cc = new ContoCorrente();
                cc.Ammontare = ActualQuote.Ammontare * -1;
                cc.Data_Movimento = ActualQuote.DataMovimento;
                cc.Id_Quote_Investimenti = IdQuote;
                cc.Id_Valuta = 1;
                cc.Cod_Valuta = "EUR";
                cc.Causale = ActualQuote.Note;
                cc.Id_Portafoglio_Titoli = 0;
                cc.Id_tipo_movimento = 12;
                cc.Id_Titolo = 0;
                cc.Desc_Conto = Dialogs.DialogService.DialogService.Location.DescLocation;
                cc.Id_Conto = Dialogs.DialogService.DialogService.Location.IdLocation;
                cc.Id_Gestione = Dialogs.DialogService.DialogService.Owner.IdOwner;
                cc.NomeGestione = Dialogs.DialogService.DialogService.Owner.OwnerName;
                ListContoCorrente.Add(cc);
                _managerLiquidServices.InsertAccountMovement(cc);
                ListContoCorrente = _managerLiquidServices.GetContoCorrenteByMovement(12);
                ListTabQuote = _managerLiquidServices.GetQuoteTab();
            }
            else
            {
                // pulisci tutto
            }
        }

        #endregion
    }
}

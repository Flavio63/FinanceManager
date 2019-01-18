using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using FinanceManager.Comparators;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;

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
        }

        private void Init()
        {
            CloseMeCommand = new CommandHandler(CloseMe);
            //ListQuote = new QuoteList();
            ListTabQuote = new QuoteTabList();
            ListInvestitore = new InvestitoreList();
            ListMovementType = new RegistryMovementTypeList();
            ListContoCorrente = new ContoCorrenteList();
            ActualQuote = new QuoteTab();

            RegistryMovementTypeList listaOriginale = new RegistryMovementTypeList();
            listaOriginale = _registryServices.GetRegistryMovementTypesList();
            var RMTL = from movimento in listaOriginale
                       where (movimento.Id_tipo_movimento == 1 ||
                       movimento.Id_tipo_movimento == 2 ||
                       movimento.Id_tipo_movimento == 12 ||
                       movimento.Id_tipo_movimento == 4 ||
                       movimento.Id_tipo_movimento == 15)
                       select movimento;
            foreach (RegistryMovementType registry in RMTL)
                ListMovementType.Add(registry);

            DataMovimento = DateTime.Now.Date;

            UpdateCollection();
        }

        private void UpdateCollection()
        {
            ListQuote = _managerLiquidServices.GetQuote();
            ListTabQuote = _managerLiquidServices.GetQuoteTab();
            ListInvestitore = _managerLiquidServices.GetInvestitori();
            SintesiSoldiR = _managerLiquidServices.GetCurrencyAvailable(1);
            SintesiSoldiDF = _managerLiquidServices.GetCurrencyAvailable(2);
            ListContoCorrente = _managerLiquidServices.GetContoCorrenteByMovement(12);
        }

        #region Getter&Setter
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
            set
            {
                SetValue(() => ListQuote, value);
                if (value.Count() == 0)
                    return;
                Totale = value[0].Totale;
                TotDisponibile = value[0].TotDisponibile;
                GuadagnoTotale = value[0].GuadagnoTotale;
                Cedole = value[0].Cedole;
                Utili = value[0].Utili;
            }
        }
        public double Totale
        {
            get { return GetValue(() => Totale); }
            set { SetValue(() => Totale, value); }
        }
        public double TotDisponibile
        {
            get { return GetValue(() => TotDisponibile); }
            set { SetValue(() => TotDisponibile, value); }
        }
        public double GuadagnoTotale
        {
            get { return GetValue(() => GuadagnoTotale); }
            set { SetValue(() => GuadagnoTotale, value); }
        }
        public double Cedole
        {
            get { return GetValue(() => Cedole); }
            set { SetValue(() => Cedole, value); }
        }
        public double Utili
        {
            get { return GetValue(() => Utili); }
            set { SetValue(() => Utili, value); }
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
        /// il record selezionato nella griglia dei record 
        /// </summary>
        public QuoteTab ActualQuote
        {
            get { return GetValue(() => ActualQuote); }
            set { SetValue(() => ActualQuote, value); }
        }

        /// <summary>
        /// Memorizzo i record dei conto corrente
        /// </summary>
        public ContoCorrenteList ListContoCorrente
        {
            get { return GetValue(() => ListContoCorrente); }
            set { SetValue(() => ListContoCorrente, value); }
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
        /// Memorizzo la scelta del conto
        /// </summary>
        public RegistryLocation RegistryLocation
        {
            get { return GetValue(() => RegistryLocation); }
            private set { SetValue(() => RegistryLocation, value); }
        }

        /// <summary>
        /// Memorizzo la scelta della gestione
        /// </summary>
        public RegistryOwner RegistryOwner
        {
            get { return GetValue(() => RegistryOwner); }
            private set { SetValue(() => RegistryOwner, value); }
        }

        public TipoSoldi Tipo_Soldi
        {
            get { return GetValue(() => Tipo_Soldi); }
            private set { SetValue(() => Tipo_Soldi, value); }
        }

        /// <summary>
        /// Memorizzo il record prima della modifica
        /// </summary>
        public QuoteTab QuoteTabPrevious
        {
            get { return GetValue(() => QuoteTabPrevious); }
            private set { SetValue(() => QuoteTabPrevious, value); }
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

        public SintesiSoldiList SintesiSoldiDF
        {
            get { return GetValue(() => SintesiSoldiDF); }
            private set { SetValue(() => SintesiSoldiDF, value); }
        }

        public SintesiSoldiList SintesiSoldiR
        {
            get { return GetValue(() => SintesiSoldiR); }
            private set { SetValue(() => SintesiSoldiR, value); }
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
                    "Gestione AndQuote", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (ActualQuote.Id_tipo_movimento == 1 && ActualQuote.Ammontare < 0)
            {
                MessageBox.Show("Attenzione devi inserire una cifra positiva se vuoi versare",
                    "Gestione AndQuote", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            // inserire anche la verifica di disponibilità della cifra nel caso di giroconto e di prelievo
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
                    if (ActualQuote.Id_tipo_movimento != 1 && ActualQuote.Id_tipo_movimento != 2 && ActualQuote.IdQuote > 0)
                    {
                        // estraggo solo il record corrispondente alla selezione nella griglia quote
                        ListContoCorrente = _managerLiquidServices.GetContoCorrenteByIdQuote(ActualQuote.IdQuote);
                        //ListContoCorrente = _managerLiquidServices.GetContoCorrenteByMovement(12);
                    }
                    else if (ActualQuote.Id_tipo_movimento == 12 && ActualQuote.Id_tipo_movimento == 0)
                    {
                        // estraggo tutti i record con codice "giroconto"
                        ListContoCorrente = _managerLiquidServices.GetContoCorrenteByMovement(12);
                    }
                    else
                    {
                        ListContoCorrente = _managerLiquidServices.GetContoCorrenteByMovement(12);
                    }
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
                    // se il movimento è 1 o 2 oppure se è 4 o 15 con cifra negativa allora l'operazione si scrive solo sulla
                    // tabella quote_investitori
                    if (ActualQuote.Id_tipo_movimento == 1 || ActualQuote.Id_tipo_movimento == 2 ||
                        (ActualQuote.Ammontare < 0 && (ActualQuote.Id_tipo_movimento == 4 || ActualQuote.Id_tipo_movimento == 15)))
                    {
                        if (!VerifyQuoteTabOperation())
                        {
                            ActualQuote = QuoteTabPrevious;
                            QuoteTabPrevious = new QuoteTab();
                            return;
                        }
                        if (ActualQuote.IdQuote > 0)
                        {
                            _managerLiquidServices.UpdateQuoteTab(ActualQuote);
                        }
                        else
                        {
                            _managerLiquidServices.InsertInvestment(ActualQuote);
                        }
                        ListQuote = _managerLiquidServices.GetQuote();
                        ListTabQuote = _managerLiquidServices.GetQuoteTab();
                        SintesiSoldiR = _managerLiquidServices.GetCurrencyAvailable(1);
                        SintesiSoldiDF = _managerLiquidServices.GetCurrencyAvailable(2);
                        ListContoCorrente = _managerLiquidServices.GetContoCorrenteByMovement(12);
                        QuoteTabPrevious = new QuoteTab();
                    }
                    else if ((ActualQuote.Id_tipo_movimento == 12 && ActualQuote.IdQuote == 0) || (ActualQuote.Ammontare > 0 && ActualQuote.IdQuote == 0 && (ActualQuote.Id_tipo_movimento == 4 || ActualQuote.Id_tipo_movimento == 15)))
                    {
                        RegistryLocation = null;
                        RegistryOwner = null;
                        Tipo_Soldi = null;
                        if (OnOpenDialog(this) == true)
                        {
                            _managerLiquidServices.InsertInvestment(ActualQuote);                           // aggiungo il record al db
                            ActualQuote.IdQuote = ((QuoteTab)_managerLiquidServices.GetLastQuoteTab()).IdQuote; // aggiorno il record selezionato
                            ContoCorrente cc = new ContoCorrente();                 // nuovo record
                            cc.Ammontare = ActualQuote.Ammontare * -1;              // la cifra inversa di quella inserita in gestione investimenti
                            cc.DataMovimento = ActualQuote.DataMovimento;          // la data dell'operazione
                            cc.Id_Quote_Investimenti = ActualQuote.IdQuote;         // il record di riferimento della gestione investimenti
                            cc.Id_Valuta = 1;                                       // codice valuta (sempre euro)
                            cc.Cod_Valuta = "EUR";
                            cc.Causale = ActualQuote.Note;                          // le stesse note
                            cc.Id_Portafoglio_Titoli = 0;
                            cc.Id_tipo_movimento = ActualQuote.Id_tipo_movimento;
                            cc.Id_Titolo = 0;
                            cc.Id_Gestione = RegistryOwner.Id_gestione;
                            cc.NomeGestione = RegistryOwner.Nome_Gestione;
                            cc.Id_Conto = RegistryLocation.Id_conto;
                            cc.Desc_Conto = RegistryLocation.Desc_conto;
                            cc.Valore_Cambio = 1;
                            cc.Id_Tipo_Soldi = (int)Tipo_Soldi.Id_Tipo_Soldi;
                            _managerLiquidServices.InsertAccountMovement(cc);                               // aggiungo il record al db
                            ListQuote = _managerLiquidServices.GetQuote();
                            ListTabQuote = _managerLiquidServices.GetQuoteTab();
                            SintesiSoldiR = _managerLiquidServices.GetCurrencyAvailable(1);
                            SintesiSoldiDF = _managerLiquidServices.GetCurrencyAvailable(2);
                            ListContoCorrente = _managerLiquidServices.GetContoCorrenteByMovement(12);
                        }
                        else
                        {
                            ActualQuote = QuoteTabPrevious;     // riporto la situazione all'origine
                        }
                    }
                    else if ((ActualQuote.Id_tipo_movimento == 12 && ActualQuote.IdQuote > 0) || ((ActualQuote.Id_tipo_movimento == 4 || ActualQuote.Id_tipo_movimento == 15) && ActualQuote.IdQuote > 0))
                    {
                        RegistryOwner = _registryServices.GetOwner(ListContoCorrente[0].Id_Gestione);
                        RegistryLocation = _registryServices.GetLocation(ListContoCorrente[0].Id_Conto);
                        Tipo_Soldi = _registryServices.GetTipoSoldiById((int)ListContoCorrente[0].Id_Tipo_Soldi);
                        if (OnOpenDialog(this))
                        {
                            _managerLiquidServices.UpdateQuoteTab(ActualQuote);         // aggiorno i dati nel db
                            ContoCorrente cc = ListContoCorrente[0];
                            if (cc.Id_Quote_Investimenti == ActualQuote.IdQuote)
                            {
                                cc.Ammontare = ActualQuote.Ammontare * -1;              // la cifra inversa di quella inserita in gestione investimenti
                                cc.DataMovimento = ActualQuote.DataMovimento;          // la data dell'operazione
                                cc.Causale = ActualQuote.Note;                          // le stesse note
                                cc.Id_Gestione = RegistryOwner.Id_gestione;
                                cc.NomeGestione = RegistryOwner.Nome_Gestione;
                                cc.Id_Conto = RegistryLocation.Id_conto;
                                cc.Desc_Conto = RegistryLocation.Desc_conto;
                                cc.Id_Tipo_Soldi = (int)Tipo_Soldi.Id_Tipo_Soldi;
                                cc.Desc_Tipo_Soldi = Tipo_Soldi.Desc_Tipo_Soldi;
                                _managerLiquidServices.UpdateContoCorrenteByIdQuote(cc);         // aggiorno i dati nel db
                                ListQuote = _managerLiquidServices.GetQuote();
                                ListTabQuote = _managerLiquidServices.GetQuoteTab();
                                SintesiSoldiR = _managerLiquidServices.GetCurrencyAvailable(1);
                                SintesiSoldiDF = _managerLiquidServices.GetCurrencyAvailable(2);
                                ListContoCorrente = _managerLiquidServices.GetContoCorrenteByMovement(12);
                            }
                        }
                        else
                        {
                            ActualQuote = QuoteTabPrevious;         // Riporta i dati com'erano
                        }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Gestione And Quote", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    if (quoteTab.IdQuote == 0) return;
                    // verifico che tipo di movimento voglio cancellare
                    if (quoteTab.Id_tipo_movimento == 1 || quoteTab.Id_tipo_movimento == 2)
                    {
                        _managerLiquidServices.DeleteRecordQuoteTab(quoteTab.IdQuote);
                        MessageBox.Show("Il record è stato correttamente eliminato", "Gestione AndQuote Investitori", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        var Risposta = MessageBox.Show("Stai per eliminare il record selezionato da 2 tabelle." + Environment.NewLine + "Sei sicoro?",
                            "Gestione AndQuote", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (Risposta == MessageBoxResult.Yes)
                        {
                            _managerLiquidServices.DeleteRecordContoCorrente(ListContoCorrente[0].Id_RowConto);
                            _managerLiquidServices.DeleteRecordQuoteTab(ActualQuote.IdQuote);
                            ActualQuote = new QuoteTab();
                            MessageBox.Show("I 2 record sono stati correttamente eliminati", "Gestione AndQuote Investitori", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    UpdateCollection();     //Aggiorno tutti i dati della griglia
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
        public void CloseMe(object param)
        {
            GestioneQuoteInvestitoriView view = param as GestioneQuoteInvestitoriView;
            DockPanel wp = view.Parent as DockPanel;
            wp.Children.Remove(view);
        }

        private bool OnOpenDialog(object param)
        {
            Dialogs.Dialog3Combos.Dialog3CombosViewModel vm = new
                Dialogs.Dialog3Combos.Dialog3CombosViewModel("Selezionare il conto, la gestione e il tipo soldi",
                _registryServices.GetRegistryLocationList(), _registryServices.GetRegistryOwners(), _registryServices.GetTipoSoldiList(),
                RegistryLocation, RegistryOwner, Tipo_Soldi);

            Dialogs.DialogService.DialogResult result = Dialogs.DialogService.Dialog3CombosService.OpenDialog(vm, param as Window);

            if (result == Dialogs.DialogService.DialogResult.Yes)
            {
                RegistryOwner = Dialogs.DialogService.Dialog3CombosService.Owner;
                RegistryLocation = Dialogs.DialogService.Dialog3CombosService.Location;
                Tipo_Soldi = Dialogs.DialogService.Dialog3CombosService.Soldi;
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}

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
using System.Collections.Generic;

namespace FinanceManager.ViewModels
{
    public class GestioneQuoteInvestitoriViewModel : ViewModelBase
    {
        private IRegistryServices _registryServices;
        private IManagerLiquidAssetServices _managerLiquidServices;
        public ICommand CloseMeCommand { get; set; }
        private QuoteTabComparator quoteTabComparator = new QuoteTabComparator();
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand EraseCommand { get; set; }
        public ICommand InsertQuotaCommand { get; set; }
        public ICommand ModifyQuotaCommand { get; set; }
        public ICommand EraseQuotaCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        public GestioneQuoteInvestitoriViewModel(IRegistryServices registryServices, IManagerLiquidAssetServices managerLiquidServices)
        {
            _registryServices = registryServices ?? throw new ArgumentNullException("Services in Manager Portfolio Movement View Model");
            _managerLiquidServices = managerLiquidServices ?? throw new ArgumentNullException("Liquid Asset Services in Manager Portfolio Movement View Model");

            Init();
        }

        private void Init()
        {
            #region start command
            CloseMeCommand = new CommandHandler(CloseMe);
            InsertCommand = new CommandHandler(SaveCommand, CanSave);
            ModifyCommand = new CommandHandler(UpdateCommand, CanModify);
            EraseCommand = new CommandHandler(DeleteCommand, CanModify);
            InsertQuotaCommand = new CommandHandler(SaveCommand, CanSaveQuota);
            ModifyQuotaCommand = new CommandHandler(UpdateCommand, CanModifyQuota);
            EraseQuotaCommand = new CommandHandler(DeleteCommand, CanModifyQuota);
            ClearCommand = new CommandHandler(CleanCommand);
            #endregion

            #region Inizializzazione Liste
            try
            {
                ListQuoteInv = new QuoteInvList();
                ListQuoteDettaglioGuadagno = new GuadagnoPerQuoteList();
                ListQuoteSintesiGuadagno = new GuadagnoPerQuoteList();
                ListTabQuote = new QuoteTabList();
                ListMovementType = new RegistryMovementTypeList();
                ListInvestitori = new RegistryOwnersList();
                ListGestioni = new RegistryOwnersList();
                ListLocation = new RegistryLocationList();
                ListQuoteGuadagno = new QuotePerPeriodoList();
                ListTipoSoldi = new TipoSoldiList();
                ListAnni = _managerLiquidServices.GetAnniFromGuadagni();

                #endregion

                UpdateCollection();
            }
            catch (Exception err)
            {
                MessageBox.Show("Errore nella richiesta dei dati." + Environment.NewLine + err.Message, "DAF-C Quote Investitori");
            }

        }

        private void UpdateCollection()
        {
            try
            {
                RegistryMovementTypeList listaOriginale = new RegistryMovementTypeList();
                listaOriginale = _registryServices.GetRegistryMovementTypesList();
                foreach (RegistryMovementType registry in listaOriginale)
                    if (registry.Id_tipo_movimento == 1 || registry.Id_tipo_movimento == 2)
                        ListMovementType.Add(registry);

                RegistryOwnersList ListaInvestitoreOriginale = new RegistryOwnersList();
                ListaInvestitoreOriginale = _registryServices.GetGestioneList();
                foreach (RegistryOwner RO in ListaInvestitoreOriginale)
                {
                    if (RO.Tipologia == "Investitore")
                        ListInvestitori.Add(RO);
                    else if (RO.Tipologia == "Gestore")
                        ListGestioni.Add(RO);
                }

                QuotePerPeriodoList ListQuoteGuadagnoOriginale = _managerLiquidServices.GetAllRecordQuote_Guadagno();
                foreach (QuotePerPeriodo quotePerPeriodo in ListQuoteGuadagnoOriginale)
                    if (quotePerPeriodo.Id_Tipo_Soldi == 16)
                        ListQuoteGuadagno.Add(quotePerPeriodo);

                ContoCorrenteSelected = new ContoCorrente();
                ActualQuote = new QuoteTab();
                QuotaPerPeriodo = new QuotePerPeriodo();

                ListQuoteInv = _managerLiquidServices.GetQuoteInv();
                ListTabQuote = _managerLiquidServices.GetQuoteTab();
                ListQuoteDettaglioGuadagno = _managerLiquidServices.GetQuoteGuadagno(false);
                ListQuoteSintesiGuadagno = _managerLiquidServices.GetQuoteGuadagno(true);
                ListLocation = _registryServices.GetRegistryLocationList();
                ListTipoSoldi = _registryServices.GetTipoSoldiList();
            }
            catch (Exception err)
            {
                MessageBox.Show("Errore nella richiesta dei dati." + Environment.NewLine + err.Message, "DAF-C Quote Investitori");
            }
        }

        #region Getter&Setter
        /// <summary>Estraggo gli anni dalla tabella guadagni_totale_anno</summary>
        public List<int> ListAnni
        {
            get { return GetValue(() => ListAnni); }
            private set { SetValue(() => ListAnni, value); }
        }
        /// <summary>
        /// E' il record nuovo o selezionato dal datagrid
        /// relativo alle quote per periodo
        /// </summary>
        public QuotePerPeriodo QuotaPerPeriodo
        {
            get { return GetValue(() => QuotaPerPeriodo); }
            set { SetValue(() => QuotaPerPeriodo, value); }
        }
        /// <summary>La lista con i tipo soldi </summary>
        public TipoSoldiList ListTipoSoldi
        {
            get { return GetValue(() => ListTipoSoldi); }
            set { SetValue(() => ListTipoSoldi, value); }
        }
        /// <summary>Prelevo tutti i record della tabella quote_guadagno</summary>
        public QuotePerPeriodoList ListQuoteGuadagno
        {
            get { return GetValue(() => ListQuoteGuadagno); }
            private set { SetValue(() => ListQuoteGuadagno, value); }
        }
        /// <summary>
        /// Per gestire la direzione del giroconto
        /// DA QuoteInvestimenti A ContoCorrente
        /// </summary>
        public bool CheckDa
        {
            get { return GetValue(() => CheckDa); }
            set { SetValue(() => CheckDa, value); }
        }
        /// <summary>
        /// Per gestire la direzione del giroconto
        /// A QuoteInvestimenti DA ContoCorrente
        /// </summary>
        public bool CheckA
        {
            get { return GetValue(() => CheckA); }
            set { SetValue(() => CheckA, value); }
        }
        /// <summary>
        /// La lista di dove sono depositati i soldini
        /// </summary>
        public RegistryLocationList ListLocation
        {
            get { return GetValue(() => ListLocation); }
            set { SetValue(() => ListLocation, value); }
        }
        /// <summary>
        /// Gestisce l'attivazione automatica
        /// del TabItem relativo al Versamento/Prelievo
        /// </summary>
        public bool TabVersPre
        {
            get { return GetValue(() => TabVersPre); }
            set { SetValue(() => TabVersPre, value); }
        }
        /// <summary>
        /// Gestisce l'attivazione automatica
        /// del TabItem relativo al Giroconto
        /// </summary>
        public bool TabGiroconto
        {
            get { return GetValue(() => TabGiroconto); }
            set { SetValue(() => TabGiroconto, value); }
        }
        /// <summary>
        /// E' la lista con gli investitori
        /// (filtro della tabella gestioni sul campo tipologia)
        /// </summary>
        public RegistryOwnersList ListInvestitori
        {
            get { return GetValue(() => ListInvestitori); }
            set { SetValue(() => ListInvestitori, value); }
        }
        /// <summary>
        /// E' la lista con i gestori degli investimenti
        /// (filtro della tabella gestioni sul campo tipologia)
        /// </summary>
        public RegistryOwnersList ListGestioni
        {
            get { return GetValue(() => ListGestioni); }
            set { SetValue(() => ListGestioni, value); }
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
        /// E' la tabella di sintesi con le quote degli investitori
        /// aggiornata con gli ultimi movimenti
        /// </summary>
        public QuoteInvList ListQuoteInv
        {
            get { return GetValue(() => ListQuoteInv); }
            set
            { SetValue(() => ListQuoteInv, value); }
        }
        /// <summary>
        /// Estrae i dati di guadagno dettagliati sulla base dei periodi
        /// di validità delle quote che si basano sugli investimenti
        /// attivi (tranne che per le volatili sempre al 50%)
        /// </summary>
        public GuadagnoPerQuoteList ListQuoteDettaglioGuadagno
        {
            get { return GetValue(() => ListQuoteDettaglioGuadagno); }
            private set { SetValue(() => ListQuoteDettaglioGuadagno, value); }
        }
        /// <summary>
        /// Estrae i dati di guadagno per anno solare sulla base dei periodi
        /// di validità delle quote che si basagno sugli investimenti
        /// attivi (tranne che per le volatili sempre al 50%)
        /// </summary>
        public GuadagnoPerQuoteList ListQuoteSintesiGuadagno
        {
            get { return GetValue(() => ListQuoteSintesiGuadagno); }
            private set { SetValue(() => ListQuoteSintesiGuadagno, value); }
        }
        /// <summary>
        /// E' la tabella degli investimenti con i movimenti dettagliati
        /// </summary>
        public QuoteTabList ListTabQuote
        {
            get { return GetValue(() => ListTabQuote); }
            private set { SetValue(() => ListTabQuote, value); }
        }
        /// <summary>
        /// il record selezionato nella griglia dei record 
        /// lo utilzzo anche per registrare inserimenti e modifiche
        /// </summary>
        public QuoteTab ActualQuote
        {
            get { return GetValue(() => ActualQuote); }
            set { SetValue(() => ActualQuote, value); }
        }
        /// <summary>
        /// Memorizzo i record dei conto corrente
        /// </summary>
        public ContoCorrente ContoCorrenteSelected
        {
            get { return GetValue(() => ContoCorrenteSelected); }
            set { SetValue(() => ContoCorrenteSelected, value); }
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
            if (sender is DataGrid && e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is QuoteTab quoteTab)
                {
                    ActualQuote = quoteTab;
                    if (ActualQuote.Id_tipo_movimento == 12)
                    {
                        // estraggo solo il record corrispondente alla selezione nella griglia quote
                        ContoCorrenteSelected = _managerLiquidServices.GetContoCorrenteByIdQuote(ActualQuote.IdQuote);
                        CheckDa = ContoCorrenteSelected.Ammontare > 0 ? true : false;
                        CheckA = !CheckDa;
                        TabGiroconto = true;
                        TabVersPre = !TabGiroconto;
                    }
                    else if (ActualQuote.Id_tipo_movimento != 12)
                    {
                        // Attivo il tabControl dei Ver
                        TabVersPre = true;
                        TabGiroconto = !TabVersPre;
                    }
                }
                else if (e.AddedItems[0] is QuotePerPeriodo quotaPerPeriodo)
                {
                    QuotaPerPeriodo = quotaPerPeriodo;
                }
            }
            else if (e.AddedItems.Count > 0 && e.AddedItems[0] is DatePicker)
            {
                ActualQuote.DataMovimento = (DateTime)e.AddedItems[0];
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

        /// <summary>
        /// Controlla che il punto del tastierino numerico venga trasformato in virgola
        /// </summary>
        /// <param name="sender">Tastiera</param>
        /// <param name="e">Pressione del tasto</param>
        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Name == "Ammontare")
                if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
                {
                    var pos = textBox.SelectionStart;
                    textBox.Text = textBox.Text.Insert(pos, ",");
                    textBox.SelectionStart = pos + 1;
                    e.Handled = true;
                }
        }

        #endregion

        #region Commands
        public void CloseMe(object param)
        {
            GestioneQuoteInvestitoriView view = param as GestioneQuoteInvestitoriView;
            DockPanel wp = view.Parent as DockPanel;
            wp.Children.Remove(view);
        }
        public void SaveCommand(object param)
        {
            try
            {
                if (((StackPanel)param).Name == "Bottoniera_1" && TabVersPre == true && ActualQuote.IdQuote == 0)
                {
                    DateTime DataDaModi = _managerLiquidServices.GetDataPrecedente(ActualQuote.DataMovimento); // scopro la coppia di date da modificare
                    _managerLiquidServices.UpdateDataFine(DataDaModi, ActualQuote.DataMovimento.AddDays(-1));   // modifico la data di fine della coppia
                    _managerLiquidServices.InsertPeriodoValiditaQuote(ActualQuote.DataMovimento);  // inserisco la nuova coppia di date
                    _managerLiquidServices.InsertInvestment(ActualQuote); // inserisco il nuovo movimento di capitali
                    if (ActualQuote.IdGestione != 4)
                    {
                        ActualQuote.IdGestione = ActualQuote.IdGestione == 3 ? 5 : 3;
                        ActualQuote.Ammontare = 0;
                        ActualQuote.Note = "Inserito per calcolo quote";
                        _managerLiquidServices.InsertInvestment(ActualQuote); // inserisco record vuoto per la gestione opposta
                        // calcolo le nuove quote per gli investimenti stabili e le memorizzo
                        _managerLiquidServices.ComputesAndInsertQuoteGuadagno();
                    }
                    else
                    {
                        // calcolo la nuova quota di aurora e modifico le nostre quote di conseguenza
                        // baso il 100% sul 0,22% circa attuale che vuol dire considerare un 250000 di budget totale come 100%
                        int lastID = _managerLiquidServices.GetLastPeriodoValiditaQuote();  // trovo l'id dell'ultima coppia di date
                        double VolatiliDAFLA = 249495;
                        double TotaleAury = _managerLiquidServices.GetInvestmentByIdGestione(ActualQuote.IdGestione);
                        double NuovaQuota = TotaleAury / (VolatiliDAFLA + TotaleAury);
                        QuotePerPeriodo QPP = new QuotePerPeriodo() { Id_Gestione = ActualQuote.IdGestione, Id_Quote_Periodi = lastID, Id_Tipo_Soldi = 16, Quota = NuovaQuota };
                        _managerLiquidServices.InsertRecordQuote_Guadagno(QPP);
                        QPP.Id_Gestione = 3; QPP.Quota = (1 - NuovaQuota) / 2; _managerLiquidServices.InsertRecordQuote_Guadagno(QPP); //nuova quota Flavio volatili
                        QPP.Id_Gestione = 5; _managerLiquidServices.InsertRecordQuote_Guadagno(QPP); //nuova quota Dany volatili

                    }
                    MessageBox.Show(string.Format("Ho effettuato l'operazione {0} correttamente.", ActualQuote.Desc_tipo_movimento),
                    Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                }
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
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi nel modificare il record" + Environment.NewLine + err.Message, Application.Current.FindResource("DAF_Caption").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void DeleteCommand(object param)
        {

            if (((StackPanel)param).Name == "Bottoniera_1")
            {
                try
                {

                }
                catch (Exception err)
                {
                    MessageBox.Show("Problemi nell'eliminare il record" + Environment.NewLine + err.Message, Application.Current.FindResource("DAF_Caption").ToString(),
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (((StackPanel)param).Name == "Bottoniera_2")
            {
                try
                {
                    _managerLiquidServices.DeleteRecordQuote_Guadagno(QuotaPerPeriodo.Id_Quota);
                    ListQuoteGuadagno = _managerLiquidServices.GetAllRecordQuote_Guadagno();
                }
                catch (Exception err)
                {
                    MessageBox.Show("Problemi nell'eliminare il record \"Quote per Investitore\"!" + Environment.NewLine + err.Message,
                        Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            MessageBox.Show("Eliminazione effettuata correttamente.", Application.Current.FindResource("DAF_Caption").ToString(),
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public void CleanCommand(object param)
        {
            if (((StackPanel)param).Name == "Bottoniera_1")
            {
            }
            else if (((StackPanel)param).Name == "Bottoniera_2")
            {
            }
            UpdateCollection();
        }

        public bool CanSave(object param)
        {
            if (ActualQuote.IdQuote == 0 && ActualQuote.Ammontare != 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Verifica se si può attivare il bottone per salvare un
        /// record nella tabella quote_guadagno
        /// </summary>
        /// <param name="param"></param>
        /// <returns>true or false</returns>
        public bool CanSaveQuota(object param)
        {
            if (QuotaPerPeriodo.Id_Quota == 0 &&
                QuotaPerPeriodo.Data_Fine > QuotaPerPeriodo.Data_Inizio &&
                QuotaPerPeriodo.Id_Quote_Periodi > 0)
                return true;
            return false;
        }

        public bool CanModify(object param)
        {
            if (ActualQuote.IdQuote > 0)
                return true;
            return false;
        }

        public bool CanModifyQuota(object param)
        {
            if (QuotaPerPeriodo.Id_Quota > 0)
                return true;
            return false;
        }

        #endregion
    }
}

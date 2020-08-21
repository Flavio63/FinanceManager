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
        private double PresoOld = 0;
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

            try
            {
                #region Inizializzazione Liste

                ListQuoteInv = new QuoteInvList();
                ListTabQuote = new QuoteTabList();
                ListMovementType = new RegistryMovementTypeList();
                ListInvestitori = new RegistryOwnersList();
                ListInvestitori2 = new RegistryOwnersList();
                ListGestioni = new RegistryOwnersList();
                ListLocation = new RegistryLocationList();
                ListQuoteGuadagno = new QuotePerPeriodoList();
                ListTipoSoldi = new TipoSoldiList();
                ListAnni = _managerLiquidServices.GetAnniFromGuadagni();

                #endregion

                #region liste combo
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
                    {
                        ListInvestitori.Add(RO);
                        ListInvestitori2.Add(RO);
                    }
                    else if (RO.Tipologia == "Gestore")
                        ListGestioni.Add(RO);
                }
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
                QuotePerPeriodoList ListQuoteGuadagnoOriginale = _managerLiquidServices.GetAllRecordQuote_Guadagno();
                ListQuoteGuadagno.Clear();
                foreach (QuotePerPeriodo quotePerPeriodo in ListQuoteGuadagnoOriginale)
                    if (quotePerPeriodo.Id_Tipo_Soldi == 16)
                        ListQuoteGuadagno.Add(quotePerPeriodo);

                ContoCorrenteSelected = new ContoCorrente();
                ActualQuote = new QuoteTab();
                RecordQuoteGuadagno = new GuadagnoPerQuote();

                Causale = "";

                ListQuoteInv = _managerLiquidServices.GetQuoteInv();
                ListTabQuote = _managerLiquidServices.GetQuoteTab();
                ListQuoteDettaglioGuadagno = _managerLiquidServices.GetQuoteGuadagno(2);
                ListQuoteSintesiGuadagno = _managerLiquidServices.GetQuoteGuadagno(1);
                ListQuoteSuperSintesiGuadagno = _managerLiquidServices.GetQuoteGuadagno(0);
                ListLocation = _registryServices.GetRegistryLocationList();
                ListTipoSoldi = _registryServices.GetTipoSoldiList();
            }
            catch (Exception err)
            {
                MessageBox.Show("Errore nella richiesta dei dati." + Environment.NewLine + err.Message, "DAF-C Quote Investitori");
            }
        }

        #region Getter&Setter
        /// <summary>
        /// E' la causale da utilizzare per la gestione dei capitali
        /// sia con ActualQuote che con contoCorrenteSelected
        /// </summary>
        public string Causale
        {
            get { return GetValue(() => Causale); }
            set { SetValue(() => Causale, value); ActualQuote.Note = value; ContoCorrenteSelected.Causale = value; }
        }
        /// <summary>Estraggo gli anni dalla tabella guadagni_totale_anno</summary>
        public List<int> ListAnni
        {
            get { return GetValue(() => ListAnni); }
            set { SetValue(() => ListAnni, value); }
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
        /// E' la lista con gli investitori
        /// (filtro della tabella gestioni sul campo tipologia)
        /// </summary>
        public RegistryOwnersList ListInvestitori2
        {
            get { return GetValue(() => ListInvestitori2); }
            set { SetValue(() => ListInvestitori2, value); }
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
        /// Come sopra ma globale per gestione e anno
        /// </summary>
        public GuadagnoPerQuoteList ListQuoteSuperSintesiGuadagno
        {
            get { return GetValue(() => ListQuoteSuperSintesiGuadagno); }
            private set { SetValue(() => ListQuoteSuperSintesiGuadagno, value); }
        }
        /// <summary>
        /// E' il record richiamato dalla grid e in fase di modifica
        /// </summary>
        public GuadagnoPerQuote RecordQuoteGuadagno
        {
            get { return GetValue(() => RecordQuoteGuadagno); }
            set { SetValue(() => RecordQuoteGuadagno, value); }
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
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is QuoteTab quoteTab)
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
                    Causale = ActualQuote.Note;
                }
                else if (ActualQuote.Id_tipo_movimento != 12)
                {
                    // Attivo il tabControl dei Ver
                    TabVersPre = true;
                    TabGiroconto = !TabVersPre;
                }
            }
            else if (e.AddedItems.Count > 0 && e.AddedItems[0] is GuadagnoPerQuote guadagnoPerQuote)
            {
                if (guadagnoPerQuote.IdTipoMovimento != 16)
                {
                    return;
                }
                RecordQuoteGuadagno = guadagnoPerQuote;
                PresoOld = RecordQuoteGuadagno.Preso;
            }
            else if (e.AddedItems.Count > 0)
            {
                if (sender is DatePicker DP)
                {
                    if (DP.Name == "DataCapitali")
                    {
                        ActualQuote.DataMovimento = (DateTime)e.AddedItems[0];
                        ContoCorrenteSelected.DataMovimento = (DateTime)e.AddedItems[0];
                    }
                    else
                        RecordQuoteGuadagno.DataOperazione = (DateTime)e.AddedItems[0];
                }
                else
                {
                    string namae = ((ComboBox)sender).Name;
                    if (namae.Contains("Capitali"))
                    {
                        if (!namae.Contains("Giro"))
                        {
                            if (e.AddedItems[0] is RegistryMovementType)
                            {
                                ActualQuote.Id_tipo_movimento = ((RegistryMovementType)e.AddedItems[0]).Id_tipo_movimento;
                                VerifyQuoteTabOperation();
                            }
                            if (e.AddedItems[0] is RegistryOwner)
                            {
                                ActualQuote.IdGestione = ((RegistryOwner)e.AddedItems[0]).Id_gestione;
                                ActualQuote.NomeInvestitore = ((RegistryOwner)e.AddedItems[0]).Nome_Gestione;
                            }
                        }
                        else if (namae.Contains("Giro") && TabGiroconto)
                        {
                            ContoCorrenteSelected.Id_tipo_movimento = 12;
                            ActualQuote.Id_tipo_movimento = 12;
                            if (e.AddedItems[0] is RegistryOwner && namae.Contains("Investitori")) ActualQuote.IdGestione = ((RegistryOwner)e.AddedItems[0]).Id_gestione;
                            if (e.AddedItems[0] is RegistryLocation) ContoCorrenteSelected.Id_Conto = ((RegistryLocation)e.AddedItems[0]).Id_Conto;
                            if (e.AddedItems[0] is RegistryOwner && namae.Contains("Gestori")) ContoCorrenteSelected.Id_Gestione = ((RegistryOwner)e.AddedItems[0]).Id_gestione;
                        }
                    }
                    else if (namae.Contains("Utili"))
                    {
                        if (e.AddedItems[0] is RegistryOwner)
                        {
                            RecordQuoteGuadagno.IdTipoMovimento = 16;
                            RecordQuoteGuadagno.IdGestione = ((RegistryOwner)e.AddedItems[0]).Id_gestione;
                            RecordQuoteGuadagno.Nome = ((RegistryOwner)e.AddedItems[0]).Nome_Gestione;
                        }
                        if (e.AddedItems[0] is Int32)
                            RecordQuoteGuadagno.Anno = (Int32)e.AddedItems[0];
                    }
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
            if (((TextBox)sender).Text != "")
                if (((TextBox)sender).Name.Contains("Capitali"))
                {
                    ActualQuote.Ammontare = Convert.ToDouble(((TextBox)sender).Text);
                    VerifyQuoteTabOperation();
                }
                else
                    RecordQuoteGuadagno.Preso = Convert.ToDouble(((TextBox)sender).Text);
        }

        /// <summary>
        /// Controlla che il punto del tastierino numerico venga trasformato in virgola
        /// </summary>
        /// <param name="sender">Tastiera</param>
        /// <param name="e">Pressione del tasto</param>
        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Name.Contains("Cifra"))
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
                if (((StackPanel)param).Name == "Bottoniera_1" && TabVersPre && ActualQuote.IdQuote == 0)
                {
                    int Id_Aggregazione = ActualQuote.IdGestione == 4 ? 16 : 15;
                    int result = _managerLiquidServices.VerifyInvestmentDate(ActualQuote, Id_Aggregazione); // verifico se alla stessa data c'è già un inserimento
                    if (result == -1)
                    {
                        if (ActualQuote.IdGestione != 4)
                        {
                            ActualQuote.Id_Periodo_Quote = _managerLiquidServices.Update_InsertQuotePeriodi(ActualQuote.DataMovimento, Id_Aggregazione);
                            _managerLiquidServices.InsertInvestment(ActualQuote); // inserisco il nuovo movimento di capitali
                            ActualQuote.IdGestione = ActualQuote.IdGestione == 3 ? 5 : 3;
                            ActualQuote.Ammontare = 0;
                            ActualQuote.Note = "Inserimento per Quote";
                            _managerLiquidServices.InsertInvestment(ActualQuote); // inserisco il movimento a 0 per effettuare le quote corrette.
                            _managerLiquidServices.ComputesAndInsertQuoteGuadagno(Id_Aggregazione, ActualQuote.Id_Periodo_Quote);
                        }
                        else if (ActualQuote.IdGestione == 4)
                        {
                            ActualQuote.Id_Periodo_Quote = _managerLiquidServices.Update_InsertQuotePeriodi(ActualQuote.DataMovimento, Id_Aggregazione);
                            _managerLiquidServices.InsertInvestment(ActualQuote); // inserisco il nuovo movimento di capitali
                            ActualQuote.IdGestione = 3; ActualQuote.Ammontare = 0; ActualQuote.Note = "Inserimento per Quote";
                            _managerLiquidServices.InsertInvestment(ActualQuote); // FLAVIO inserisco il movimento a 0 per effettuare le quote corrette.
                            ActualQuote.IdGestione = 5; ActualQuote.Ammontare = 0; ActualQuote.Note = "Inserimento per Quote";
                            _managerLiquidServices.InsertInvestment(ActualQuote); // DANIELA inserisco il movimento a 0 per effettuare le quote corrette.
                            _managerLiquidServices.ComputesAndInsertQuoteGuadagno(Id_Aggregazione, ActualQuote.Id_Periodo_Quote);
                        }
                    }
                    else
                    {
                        ActualQuote.Id_Periodo_Quote = result;
                        ActualQuote.IdQuote = _managerLiquidServices.GetIdQuoteTab(ActualQuote); // trovo il codice per modificare il record
                        _managerLiquidServices.UpdateQuoteTab(ActualQuote);     // modifico l'inserimento
                        _managerLiquidServices.ComputesAndModifyQuoteGuadagno(Id_Aggregazione);
                    }
                    // aggiorno la tabella con i guadagni totali
                    _managerLiquidServices.UpdateGuadagniTotaleAnno(ActualQuote.Id_Periodo_Quote, Id_Aggregazione);
                    MessageBox.Show(string.Format("Ho effettuato l'operazione {0} correttamente.", ActualQuote.Desc_tipo_movimento),
                    Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (((StackPanel)param).Name == "Bottoniera_1" && TabGiroconto && ActualQuote.IdQuote == 0)
                {
                    if (CheckDa && !CheckA)
                        ActualQuote.Ammontare = ActualQuote.Ammontare > 0 ? ActualQuote.Ammontare * -1 : ActualQuote.Ammontare;
                    else if (!CheckDa && CheckA)
                        ActualQuote.Ammontare = ActualQuote.Ammontare < 0 ? ActualQuote.Ammontare * -1 : ActualQuote.Ammontare;
                    ContoCorrenteSelected.Ammontare = ActualQuote.Ammontare * -1;
                    _managerLiquidServices.InsertInvestment(ActualQuote);
                    ContoCorrenteSelected.Id_Quote_Investimenti = _managerLiquidServices.GetIdQuoteTab(ActualQuote);
                    ContoCorrenteSelected.Id_Tipo_Soldi = 1;
                    ContoCorrenteSelected.Id_Valuta = 1;
                    ContoCorrenteSelected.Valore_Cambio = 1;
                    _managerLiquidServices.InsertAccountMovement(ContoCorrenteSelected);
                }
                else if (((StackPanel)param).Name == "Bottoniera_2")
                {
                    // verifica se puoi prelevare la cifra
                    double result = _managerLiquidServices.VerifyDisponibilitaUtili(RecordQuoteGuadagno);
                    // scrivi il record
                    if (result < 0)
                    {
                        var answer = MessageBox.Show("Non hai la cifra disponibile in questo anno, vuoi registrarla comunque?",
                            Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (answer == MessageBoxResult.No)
                            return;
                    }
                    _managerLiquidServices.InsertPrelievoUtili(RecordQuoteGuadagno); // in questo script inserisco il prelievo anche nella tabella prelievi
                    MessageBox.Show(string.Format("Ho effettuato il prelievo di {0} €. dal conto di {1} per l'anno {2}.",
                        RecordQuoteGuadagno.Preso, RecordQuoteGuadagno.Nome, RecordQuoteGuadagno.Anno),
                        Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                }
                // aggiorna la maschera
                UpdateCollection();
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
                if (((StackPanel)param).Name == "Bottoniera_1" && TabVersPre && ActualQuote.IdQuote > 0)
                {
                    _managerLiquidServices.UpdateQuoteTab(ActualQuote); // aggiorno il record con la nuova cifra
                    int Tipo_Soldi = ActualQuote.IdGestione == 4 ? 16 : 15;
                    // scopro il codice dei record da ricalcolare con le nuove quote
                    ActualQuote.Id_Periodo_Quote = _managerLiquidServices.GetIdPeriodoQuote(ActualQuote.DataMovimento, Tipo_Soldi);
                    _managerLiquidServices.ComputesAndModifyQuoteGuadagno(Tipo_Soldi);
                    // modifico i dati di guadagno per socio delle operazioni con il codice di periodo che ha subito la modifica
                    _managerLiquidServices.UpdateGuadagniTotaleAnno(ActualQuote.Id_Periodo_Quote, Tipo_Soldi);
                }
                else if (((StackPanel)param).Name == "Bottoniera_1" && TabGiroconto && ActualQuote.IdQuote > 0)
                {
                    if (CheckDa && !CheckA)
                        ActualQuote.Ammontare = ActualQuote.Ammontare > 0 ? ActualQuote.Ammontare * -1 : ActualQuote.Ammontare;
                    else if (!CheckDa && CheckA)
                        ActualQuote.Ammontare = ActualQuote.Ammontare < 0 ? ActualQuote.Ammontare * -1 : ActualQuote.Ammontare;
                    ContoCorrenteSelected.Ammontare = ActualQuote.Ammontare * -1;
                    _managerLiquidServices.UpdateQuoteTab(ActualQuote);
                    _managerLiquidServices.UpdateRecordContoCorrente(ContoCorrenteSelected, Models.Enumeratori.TipologiaIDContoCorrente.IdContoCorrente);
                }
                else if (((StackPanel)param).Name == "Bottoniera_2")
                {
                    double result = _managerLiquidServices.VerifyDisponibilitaUtili(RecordQuoteGuadagno);
                    result += (PresoOld * -1);
                    // scrivi il record
                    if (result < 0)
                    {
                        var answer = MessageBox.Show("Non hai la cifra disponibile in questo anno, vuoi registrarla comunque?",
                            Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (answer == MessageBoxResult.No)
                            return;
                    }
                    _managerLiquidServices.UpdateGuadagniTotaleAnno(RecordQuoteGuadagno);
                }
                UpdateCollection();
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
                }
                catch (Exception)
                {
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

        /// <summary>
        /// Attiva il tasto di inserimento per le operazioni di 
        /// Versamento, Prelevamento e Giroconto fra tabella quote_investimenti e tabella conto_corrente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool CanSave(object param)
        {
            if (ActualQuote.IdQuote == 0 && ActualQuote.Ammontare != 0 && ActualQuote.IdGestione > 0 && ActualQuote.Id_tipo_movimento > 0 && ActualQuote.Id_tipo_movimento < 13)
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
            if (RecordQuoteGuadagno.IdGuadagno == 0 && RecordQuoteGuadagno.Preso < 0 && RecordQuoteGuadagno.IdGestione > 0 && RecordQuoteGuadagno.IdTipoMovimento == 16 && RecordQuoteGuadagno.Anno > 2018)
            {
                return true;
            }
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
            if (RecordQuoteGuadagno.IdGuadagno > 0)
                return true;
            return false;
        }

        #endregion
    }
}

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
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand EraseCommand { get; set; }
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
            ClearCommand = new CommandHandler(CleanCommand);
            #endregion
            ListQuoteInv = new QuoteInvList();
            ListQuoteDettaglioGuadagno = new QuoteGuadagnoList();
            ListQuoteSintesiGuadagno = new QuoteGuadagnoList();
            ListTabQuote = new QuoteTabList();
            ListMovementType = new RegistryMovementTypeList();
            ContoCorrenteSelected = new ContoCorrente();
            ActualQuote = new QuoteTab();
            ListInvestitori = new RegistryOwnersList();
            ListGestioni = new RegistryOwnersList();
            ListLocation = new RegistryLocationList();

            RegistryMovementTypeList listaOriginale = new RegistryMovementTypeList();
            RegistryOwnersList ListaInvestitoreOriginale = new RegistryOwnersList();
            try
            {
                listaOriginale = _registryServices.GetRegistryMovementTypesList();
                foreach (RegistryMovementType registry in listaOriginale)
                    if (registry.Id_tipo_movimento == 1 || registry.Id_tipo_movimento == 2)
                        ListMovementType.Add(registry);

                ListaInvestitoreOriginale = _registryServices.GetGestioneList();
                foreach (RegistryOwner RO in ListaInvestitoreOriginale)
                {
                    if (RO.Tipologia == "Investitore")
                        ListInvestitori.Add(RO);
                    else if (RO.Tipologia == "Gestore")
                        ListGestioni.Add(RO);
                }

                DataMovimento = DateTime.Now.Date;

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
                ListQuoteInv = _managerLiquidServices.GetQuoteInv();
                ListTabQuote = _managerLiquidServices.GetQuoteTab();
                SintesiSoldiR = _managerLiquidServices.GetCurrencyAvailable(1);
                SintesiSoldiDF = _managerLiquidServices.GetCurrencyAvailable(2);
                SintesiSoldiDFV = _managerLiquidServices.GetCurrencyAvailable(7);

                ListQuoteDettaglioGuadagno = _managerLiquidServices.GetQuoteGuadagno(false);
                ListQuoteSintesiGuadagno = _managerLiquidServices.GetQuoteGuadagno(true);
                ListLocation = _registryServices.GetRegistryLocationList();
                
            }
            catch (Exception err)
            {
                MessageBox.Show("Errore nella richiesta dei dati." + Environment.NewLine + err.Message, "DAF-C Quote Investitori");
            }
        }

        #region Getter&Setter
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
            {
                SetValue(() => ListQuoteInv, value);
                UpdateGrid();
            }
        }
        #region getter&setter tabella investimenti
        public double ImmDany { get; private set; }
        public double ImmFla { get; private set; }
        public double ImmTot { get; private set; }
        public double PreDany { get; private set; }
        public double PreFla { get; private set; }
        public double PreTot { get; private set; }
        public double AttDany { get; private set; }
        public double AttFla { get; private set; }
        public double AttTot { get; private set; }
        public double QuotaDany { get; private set; }
        public double QuotaFla { get; private set; }
        public double AssDany { get; private set; }
        public double AssFla { get; private set; }
        public double AssTot { get; private set; }
        public double DisDany { get; private set; }
        public double DisFla { get; private set; }
        public double DisTot { get; private set; }
        #endregion

        /// <summary>
        /// Estrae i dati di guadagno dettagliati sulla base dei periodi
        /// di validità delle quote che si basano sugli investimenti
        /// attivi (tranne che per le volatili sempre al 50%)
        /// </summary>
        public QuoteGuadagnoList ListQuoteDettaglioGuadagno
        {
            get { return GetValue(() => ListQuoteDettaglioGuadagno); }
            private set { SetValue(() => ListQuoteDettaglioGuadagno, value); }
        }
        /// <summary>
        /// Estrae i dati di guadagno per anno solare sulla base dei periodi
        /// di validità delle quote che si basagno sugli investimenti
        /// attivi (tranne che per le volatili sempre al 50%)
        /// </summary>
        public QuoteGuadagnoList ListQuoteSintesiGuadagno
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

        public TipoSoldi Tipo_Soldi
        {
            get { return GetValue(() => Tipo_Soldi); }
            private set { SetValue(() => Tipo_Soldi, value); }
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
        /// <summary>
        /// il riepilogo dei soldi per la gestione Dany&Fla_Volatili
        /// </summary>
        public SintesiSoldiList SintesiSoldiDFV
        {
            get { return GetValue(() => SintesiSoldiDFV); }
            private set { SetValue(() => SintesiSoldiDFV, value); }
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
            }
            else if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0].GetType().Name == "DateTime")
                {
                    DataMovimento = (DateTime)e.AddedItems[0];
                    return;
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
            /*
            try
            {
                // In base all'operazione scelta decido:
                if (RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.Giroconto)
                {
                    CurrencyAvailable = _liquidAssetServices.GetCurrencyAvailable(IdGestione: RecordContoCorrente.Id_gestione,
                        IdConto: RecordContoCorrente.Id_Conto, IdValuta: RecordContoCorrente.Id_Valuta)[0];

                    if (RecordContoCorrente.Ammontare > CurrencyAvailable.Disponibili && RecordContoCorrente.Id_Tipo_Soldi == (int)TipologiaSoldi.Capitale ||
                        RecordContoCorrente.Ammontare > CurrencyAvailable.Cedole && RecordContoCorrente.Id_Tipo_Soldi == (int)TipologiaSoldi.Cedole ||
                        RecordContoCorrente.Ammontare > CurrencyAvailable.Utili && RecordContoCorrente.Id_Tipo_Soldi == (int)TipologiaSoldi.Utili)
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
                else if (RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.Cedola ||
                    RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.InsertVolatili ||
                    RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.Costi)
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
            */
        }
        public void UpdateCommand(object param)
        {
            /*
            try
            {
                // se è una registrazione cedola modifico direttamente il record 1
                if (RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.Cedola ||
                    RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.InsertVolatili ||
                    RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.Costi)
                {
                    _liquidAssetServices.UpdateContoCorrenteByIdCC(RecordContoCorrente);    //registro la modifica in conto corrente
                }
                else
                {
                    // cerco il record corrispondente al giroconto
                    Record2ContoCorrente = _liquidAssetServices.GetContoCorrenteByIdCC(RecordContoCorrente.Id_RowConto + 1);
                    // verifico che il record abbia lo stesso tipo di movimento
                    if (Record2ContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.Giroconto)
                    {
                        // modifico il record 2 sulla base delle modifiche apportate al record 1
                        Record2ContoCorrente.Id_Tipo_Soldi = RecordContoCorrente.Id_Tipo_Soldi;
                        Record2ContoCorrente.DataMovimento = RecordContoCorrente.DataMovimento;
                        Record2ContoCorrente.Ammontare = RecordContoCorrente.Ammontare * -1;
                        Record2ContoCorrente.Causale = RecordContoCorrente.Causale;
                    }
                    else
                    {
                        Record2ContoCorrente = _liquidAssetServices.GetContoCorrenteByIdCC(RecordContoCorrente.Id_RowConto - 1);
                        // modifico il record 2 sulla base delle modifiche apportate al record 1
                        Record2ContoCorrente.Id_Tipo_Soldi = RecordContoCorrente.Id_Tipo_Soldi;
                        Record2ContoCorrente.DataMovimento = RecordContoCorrente.DataMovimento;
                        Record2ContoCorrente.Ammontare = RecordContoCorrente.Ammontare * -1;
                        Record2ContoCorrente.Causale = RecordContoCorrente.Causale;
                    }
                    _liquidAssetServices.UpdateContoCorrenteByIdCC(Record2ContoCorrente);    //registro la modifica in conto corrente
                    _liquidAssetServices.UpdateContoCorrenteByIdCC(RecordContoCorrente);    //registro la modifica in conto corrente
                }
                MessageBox.Show("Record modificato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                Init();
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi nel modificare il record" + Environment.NewLine + err.Message, Application.Current.FindResource("DAF_Caption").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            */
        }
        public void DeleteCommand(object param)
        {
            /*
            try
            {
                if (RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.Cedola || RecordContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.InsertVolatili)
                {
                    _liquidAssetServices.DeleteRecordContoCorrente(RecordContoCorrente.Id_RowConto);  // registro l'eliminazione in conto corrente
                }
                else
                {
                    // cerco il record corrispondente al giroconto
                    Record2ContoCorrente = _liquidAssetServices.GetContoCorrenteByIdCC(RecordContoCorrente.Id_RowConto + 1);
                    // verifico che il record abbia lo stesso tipo di movimento
                    if (Record2ContoCorrente.Id_tipo_movimento == (int)TipologiaMovimento.Giroconto)
                    {
                        _liquidAssetServices.DeleteRecordContoCorrente(RecordContoCorrente.Id_RowConto);
                        _liquidAssetServices.DeleteRecordContoCorrente(Record2ContoCorrente.Id_RowConto);
                    }
                    else
                    {
                        Record2ContoCorrente = _liquidAssetServices.GetContoCorrenteByIdCC(RecordContoCorrente.Id_RowConto - 1);
                        _liquidAssetServices.DeleteRecordContoCorrente(RecordContoCorrente.Id_RowConto);
                        _liquidAssetServices.DeleteRecordContoCorrente(Record2ContoCorrente.Id_RowConto);
                    }
                }
                MessageBox.Show("Record eliminato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                Init();
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi nell'eliminare il record" + Environment.NewLine + err.Message, Application.Current.FindResource("DAF_Caption").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            */
        }
        public void CleanCommand(object param)
        {
            ActualQuote = new QuoteTab();
            ContoCorrenteSelected = new ContoCorrente();
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

        private void UpdateGrid()
        {
            foreach (QuoteInv quoteInv in ListQuoteInv)
            {
                switch (quoteInv.NomeInvestitore)
                {
                    case "Daniela":
                        ImmDany = quoteInv.CapitaleImmesso;
                        PreDany = quoteInv.CapitalePrelevato;
                        AttDany = quoteInv.CapitaleAttivo;
                        QuotaDany = quoteInv.QuotaInv;
                        AssDany = quoteInv.CapitaleAssegnato;
                        DisDany = quoteInv.CapitaleDisponibile;
                        break;
                    case "Flavio":
                        ImmFla = quoteInv.CapitaleImmesso;
                        PreFla = quoteInv.CapitalePrelevato;
                        AttFla = quoteInv.CapitaleAttivo;
                        QuotaFla = quoteInv.QuotaInv;
                        AssFla = quoteInv.CapitaleAssegnato;
                        DisFla = quoteInv.CapitaleDisponibile;
                        ImmTot = quoteInv.TotaleImmesso;
                        PreTot = quoteInv.TotalePrelevato;
                        AttTot = quoteInv.TotaleAttivo;
                        AssTot = quoteInv.TotaleAssegnato;
                        DisTot = quoteInv.TotaleDisponibile;
                        break;
                }
            }
        }
    }
}

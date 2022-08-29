using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using FinanceManager.Events;
using FinanceManager.Services;
using FinanceManager.Views;
using FinanceManager.Models;

namespace FinanceManager.ViewModels
{
    public class GiroContoViewModel : ViewModelBase
    {
        private readonly IRegistryServices _registryServices;
        private readonly IManagerLiquidAssetServices _managerLiquidServices;

        public ICommand CloseMeCommand { get; set; }
        public ICommand ClearMeCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public GiroContoViewModel(IRegistryServices registryServices, IManagerLiquidAssetServices managerLiquidServices)
        {
            _registryServices = registryServices ?? throw new ArgumentNullException("Manca collegamento con richiesta dati anagrafica Giroconto View Model");
            _managerLiquidServices = managerLiquidServices ?? throw new ArgumentNullException("Manca collegamento con richiesta dati conti Giroconto View Model");
            Init();
            UpdateCollection();
            ClearData();
        }

        private void Init()
        {
            CloseMeCommand = new CommandHandler(CloseMe);
            ClearMeCommand = new CommandHandler(ClearMe);
            InsertCommand = new CommandHandler(SaveCommand, CanSave);
            ModifyCommand = new CommandHandler(UpdateCommand, CanModify);
        }

        private void UpdateCollection()
        {
            ListValute = new RegistryCurrencyList();
            ListValute = _registryServices.GetRegistryCurrencyList();
            ListInvestitoriMittente = new RegistryOwnersList();
            ListGestioniMittente = new RegistryOwnersList();
            ListContoMittente = new RegistryLocationList();
            ListInvestitoriRicevente = new RegistryOwnersList();
            ListGestioniRicevente = new RegistryOwnersList();
            ListContoRicevente = new RegistryLocationList();
            RegistryOwnersList ListaInvestitoreOriginale = new RegistryOwnersList();
            ListaInvestitoreOriginale = _registryServices.GetGestioneList();
            foreach (RegistryOwner RO in ListaInvestitoreOriginale)
            {
                if (RO.Tipologia == "Investitore")
                {
                    ListInvestitoriMittente.Add(RO);
                    ListInvestitoriRicevente.Add(RO);
                }
                else if (RO.Tipologia == "Gestore")
                {
                    ListGestioniMittente.Add(RO);
                    ListGestioniRicevente.Add(RO);
                }
            }
            ListContoMittente = _registryServices.GetRegistryLocationList();
            ListContoRicevente = _registryServices.GetRegistryLocationList();

        }
        private void ClearData()
        {
            ContoCorrenteList conti_Correnti = new ContoCorrenteList();
            conti_Correnti = _managerLiquidServices.GetContoCorrenteList();
            var contos = from record in conti_Correnti where record.Id_tipo_movimento == 12 select record;
            ContiCorrenti = new ContoCorrenteList();
            foreach (ContoCorrente conto in contos)
            {
                if (conto.Id_tipo_movimento == 12)
                    ContiCorrenti.Add(conto);
            }

            QuoteTabList investments = new QuoteTabList();
            investments = _managerLiquidServices.GetQuoteTab();
            var inv = from record in investments where record.Id_tipo_movimento == 12 select record;
            Investimenti = new QuoteTabList();
            foreach (QuoteTab tab in inv)
            {
                if (tab.Id_tipo_movimento == 12)
                    Investimenti.Add(tab);
            }

            ChiMittente = true;
            ContoMittente = true;
            GestioneMittente = false;
            ChiRicevente = false;
            ContoRicevente = false;
            GestioneRicevente = false;
            ValutaEnabled = false;
            TotaleMittenteQuote = new QuoteTabList();
            TotaleRiceventeQuote = new QuoteTabList();
            TotaleMittenteConto = new ContoCorrenteList();
            TotaleRiceventeConto = new ContoCorrenteList();
            IdInvestitoreMittente = 0;
            IdInvestitoreRicevente = 0;
            IdContoMittente = 0;
            IdGestioneMittente = 0;
            IdContoRicevente = 0;
            IdGestioneRicevente = 0;
            IdValuta = 0;
            Valore = 0;
            dataModifica = DateTime.Now;
            causale = "";
        }

        #region Get_Set

        public QuoteTabList Investimenti
        {
            get { return GetValue(() => Investimenti); }
            set { SetValue(() => Investimenti, value); }
        }

        public ContoCorrenteList ContiCorrenti
        {
            get { return GetValue(() => ContiCorrenti); }
            set { SetValue(() => ContiCorrenti, value); }
        }

        #region boolean per autorizzazioni a cascata
        /// <summary>
        /// Gestisce l'abilitazione fra investitore
        /// e conto / gestione nel campo "mittente"
        /// </summary>
        public bool ChiMittente
        {
            get { return GetValue(() => ChiMittente); }
            set { SetValue(() => ChiMittente, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione fra investitore
        /// e conto / gestione nel campo "ricevente"
        /// </summary>
        public bool ChiRicevente
        {
            get { return GetValue(() => ChiRicevente); }
            set { SetValue(() => ChiRicevente, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione fra investitore
        /// e conto / gestione nel campo "mittente"
        /// </summary>
        public bool ContoMittente
        {
            get { return GetValue(() => ContoMittente); }
            set { SetValue(() => ContoMittente, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione fra investitore
        /// e conto / gestione nel campo "ricevente"
        /// </summary>
        public bool ContoRicevente
        {
            get { return GetValue(() => ContoRicevente); }
            set { SetValue(() => ContoRicevente, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione fra investitore
        /// e conto / gestione nel campo "mittente"
        /// </summary>
        public bool GestioneMittente
        {
            get { return GetValue(() => GestioneMittente); }
            set { SetValue(() => GestioneMittente, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione fra investitore
        /// e conto / gestione nel campo "ricevente"
        /// </summary>
        public bool GestioneRicevente
        {
            get { return GetValue(() => GestioneRicevente); }
            set { SetValue(() => GestioneRicevente, value); }
        }

        
        public bool ValutaEnabled
        {
            get { return GetValue(() => ValutaEnabled); }
            set { SetValue(() => ValutaEnabled, value); }
        }

        #endregion

        #region filtri

        public int IdInvestitoreMittente
        {
            get { return GetValue(() => IdInvestitoreMittente); }
            set { SetValue(() => IdInvestitoreMittente, value); }
        }

        public int IdInvestitoreRicevente
        {
            get { return GetValue(() => IdInvestitoreRicevente); }
            set { SetValue(() => IdInvestitoreRicevente, value); }
        }
        public int IdValuta
        {
            get { return GetValue(() => IdValuta); }
            set { SetValue(() => IdValuta, value); }
        }

        public int IdContoMittente
        {
            get { return GetValue(() => IdContoMittente); }
            set { SetValue(() => IdContoMittente, value); }
        }

        public int IdGestioneMittente
        {
            get { return GetValue(() => IdGestioneMittente); }
            set { SetValue(() => IdGestioneMittente, value); }
        }

        public int IdContoRicevente
        {
            get { return GetValue(() => IdContoRicevente); }
            set { SetValue(() => IdContoRicevente, value); }
        }

        public int IdGestioneRicevente
        {
            get { return GetValue(() => IdGestioneRicevente); }
            set { SetValue(() => IdGestioneRicevente, value); }
        }

        #endregion

        #region combobox

        /// <summary>
        /// E' la lista OwnerList da cui preleva solo
        /// la tipologia investitore
        /// </summary>
        public RegistryOwnersList ListInvestitoriMittente
        {
            get { return GetValue(() => ListInvestitoriMittente); }
            set { SetValue(() => ListInvestitoriMittente, value); }
        }
        /// <summary>
        /// E' la lista OwnerList da cui preleva solo
        /// la tipologia "gestore"
        /// </summary>
        public RegistryOwnersList ListGestioniMittente
        {
            get { return GetValue(() => ListGestioniMittente); }
            set { SetValue(() => ListGestioniMittente, value); }
        }

        /// <summary>
        /// E' la lista dei conti correnti
        /// </summary>
        public RegistryLocationList ListContoMittente
        {
            get { return GetValue(() => ListContoMittente); }
            set { SetValue(() => ListContoMittente, value); }
        }

        /// <summary>
        /// E' la lista OwnerList da cui preleva solo
        /// la tipologia investitore
        /// </summary>
        public RegistryOwnersList ListInvestitoriRicevente
        {
            get { return GetValue(() => ListInvestitoriRicevente); }
            set { SetValue(() => ListInvestitoriRicevente, value); }
        }
        /// <summary>
        /// E' la lista OwnerList da cui preleva solo
        /// la tipologia "gestore"
        /// </summary>
        public RegistryOwnersList ListGestioniRicevente
        {
            get { return GetValue(() => ListGestioniRicevente); }
            set { SetValue(() => ListGestioniRicevente, value); }
        }

        /// <summary>
        /// E' la lista dei conti correnti
        /// </summary>
        public RegistryLocationList ListContoRicevente
        {
            get { return GetValue(() => ListContoRicevente); }
            set { SetValue(() => ListContoRicevente, value); }
        }

        public RegistryCurrencyList ListValute
        {
            get { return GetValue(() => ListValute); }
            set { SetValue(() => ListValute, value); }
        }

        #endregion combobo

        #region dati da registrare
        public QuoteTabList TotaleMittenteQuote { get; set; }
        public QuoteTabList TotaleRiceventeQuote { get; set; }
        public ContoCorrenteList TotaleMittenteConto { get; set; }
        public ContoCorrenteList TotaleRiceventeConto { get; set; }
        /// <summary>
        /// è il valore da trasferire
        /// </summary>
        public double Valore
        {
            get { return GetValue(() => Valore); }
            set { SetValue(() => Valore, value); }
        }

        public DateTime dataModifica
        {
            get { return GetValue(() => dataModifica); }
            set { SetValue(() => dataModifica, value); }
        }

        public string causale
        {
            get { return GetValue(() => causale); }
            set { SetValue(() => causale, value); }
        }
        #endregion dati da registrare

        #endregion
        
        #region Events
        public void LostFocus(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text != "")
            {
                try { Valore = Convert.ToDouble(((TextBox)sender).Text); } catch { } 
            }
        }

        /// <summary>
        /// Controlla che il punto del tastierino numerico venga trasformato in virgola
        /// </summary>
        /// <param name="sender">Tastiera</param>
        /// <param name="e">Pressione del tasto</param>
        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Name == "importo")
                if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
                {
                    var pos = textBox.SelectionStart;
                    textBox.Text = textBox.Text.Insert(pos, ",");
                    textBox.SelectionStart = pos + 1;
                    e.Handled = true;
                }
        }

        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && sender is DatePicker DP)
            {
                dataModifica = (DateTime)e.AddedItems[0];
            }
            else if (e.AddedItems.Count > 0 && sender is ComboBox CB)
            {
                string CBname = CB.Name;
                switch (CBname)
                {
                    case "InvMittente":
                        ContoMittente = false;
                        ChiRicevente = false;
                        ContoRicevente = true;
                        GestioneRicevente = false;
                        IdInvestitoreMittente = ((RegistryOwner)e.AddedItems[0]).Id_gestione;
                        TotaleMittenteQuote = _managerLiquidServices.GetTotalAmountByCurrency(IdInvestitoreMittente);
                        TotaleMittenteConto = null;
                        CreaDataGrid(sender, TotaleMittenteQuote, TotaleMittenteConto, (DataGrid)CB.FindName("DGMittente"));
                        IdContoMittente = 0;
                        break;
                    case "ContoMittente":
                        ChiMittente = false;
                        GestioneMittente=true;
                        IdContoMittente = ((RegistryLocation)e.AddedItems[0]).Id_Conto;
                        TotaleMittenteQuote = null;
                        TotaleMittenteConto = _managerLiquidServices.GetTotalAmountByAccount(IdContoMittente);
                        CreaDataGrid(sender, TotaleMittenteQuote, TotaleMittenteConto, (DataGrid)CB.FindName("DGMittente"));
                        break;
                    case "GestioneMittente":
                        ContoMittente = false;
                        ChiRicevente = true;
                        ContoRicevente=true;
                        IdGestioneMittente = ((RegistryOwner)e.AddedItems[0]).Id_gestione;
                        TotaleMittenteConto = _managerLiquidServices.GetTotalAmountByAccount(IdContoMittente, IdGestioneMittente);
                        CreaDataGrid(sender, TotaleMittenteQuote, TotaleMittenteConto, (DataGrid)CB.FindName("DGMittente"));
                        break;
                    case "InvRicevente":
                        ContoRicevente = false;
                        GestioneMittente = false;
                        IdInvestitoreRicevente = ((RegistryOwner)e.AddedItems[0]).Id_gestione;
                        TotaleRiceventeQuote = _managerLiquidServices.GetTotalAmountByCurrency(IdInvestitoreRicevente);
                        TotaleRiceventeConto = null;
                        CreaDataGrid(sender, TotaleRiceventeQuote, TotaleRiceventeConto, (DataGrid)CB.FindName("DGRicevente"));
                        IdContoRicevente = 0;
                        ValutaEnabled = true;
                        break;
                    case "ContoRicevente":
                        ChiMittente = false;
                        ChiRicevente = false;
                        GestioneRicevente = true;
                        IdContoRicevente = ((RegistryLocation)e.AddedItems[0]).Id_Conto;
                        TotaleRiceventeQuote = null;
                        TotaleRiceventeConto = _managerLiquidServices.GetTotalAmountByAccount(IdContoRicevente);
                        CreaDataGrid(sender, TotaleRiceventeQuote, TotaleRiceventeConto, (DataGrid)CB.FindName("DGRicevente"));
                        break;
                    case "GestioneRicevente":
                        ContoRicevente = false;
                        ValutaEnabled = true;
                        IdGestioneRicevente = ((RegistryOwner)e.AddedItems[0]).Id_gestione;
                        TotaleRiceventeConto = _managerLiquidServices.GetTotalAmountByAccount(IdContoRicevente, IdGestioneRicevente);
                        CreaDataGrid(sender, TotaleRiceventeQuote, TotaleRiceventeConto, (DataGrid)CB.FindName("DGRicevente"));
                        break;
                    case "Valuta":
                        GestioneRicevente = false;
                        ChiRicevente = false;
                        IdValuta = ((RegistryCurrency)e.AddedItems[0]).IdCurrency;
                        if (TotaleMittenteQuote != null)
                            TotaleMittenteQuote = _managerLiquidServices.GetTotalAmountByCurrency(IdInvestitoreMittente, IdValuta);
                        else
                             TotaleMittenteConto = _managerLiquidServices.GetTotalAmountByAccount(IdContoMittente, IdGestioneMittente, IdValuta);
                        CreaDataGrid(sender, TotaleMittenteQuote, TotaleMittenteConto, (DataGrid)CB.FindName("DGMittente"));
                        if (TotaleRiceventeQuote != null)
                            TotaleRiceventeQuote = _managerLiquidServices.GetTotalAmountByCurrency(IdInvestitoreRicevente, IdValuta);
                        else
                            TotaleRiceventeConto =  _managerLiquidServices.GetTotalAmountByAccount(IdContoRicevente, IdGestioneRicevente, IdValuta);
                        CreaDataGrid(sender, TotaleRiceventeQuote, TotaleRiceventeConto, (DataGrid)CB.FindName("DGRicevente"));
                        break;
                }
            }
            else if (e.AddedItems.Count > 0 && sender is DataGrid DS)
            {
                if (DS.Name == "INVtrasfatti" && e.AddedItems[0] is QuoteTab QT)
                {
                    if (QT.AmmontareValuta < 0)
                    {
                        IdInvestitoreMittente = QT.Id_Gestione;
                        ContoCorrente contoRicevuto = _managerLiquidServices.GetContoCorrenteByIdQuote(QT.Id_Quote_Investimenti);
                        IdContoRicevente = contoRicevuto.Id_Conto;
                        IdGestioneRicevente = contoRicevuto.Id_Gestione;
                        IdValuta = QT.Id_Valuta;
                        Valore = QT.AmmontareValuta * -1;
                        dataModifica = QT.DataMovimento;
                        causale = QT.Note;
                    }
                    else
                    {
                        IdInvestitoreRicevente = QT.Id_Gestione;
                        IdValuta = QT.Id_Valuta;
                        Valore = QT.AmmontareValuta;
                        dataModifica = QT.DataMovimento;
                        causale = QT.Note;
                        ContoCorrente contoInviato = _managerLiquidServices.GetContoCorrenteByIdQuote(QT.Id_Quote_Investimenti);
                        IdContoMittente = contoInviato.Id_Conto;
                        IdGestioneMittente = contoInviato.Id_Gestione;
                    }
                }
                else if (DS.Name == "CONTOtrasfatti" && e.AddedItems[0] is ContoCorrente CC)
                {
                    if (CC.Ammontare < 0)
                    {
                        IdContoMittente = CC.Id_Conto;
                        IdGestioneMittente = CC.Id_Gestione;
                        Valore = CC.Ammontare;
                        dataModifica = CC.DataMovimento;
                        IdValuta = CC.Id_Valuta;
                        causale = CC.Causale;
                        if (CC.Id_Quote_Investimenti == 0)
                        {
                            ContoCorrenteList contiGemellati = _managerLiquidServices.Get2ContoCorrentes(CC.Modified);
                            ContoCorrente conto = contiGemellati[0].Id_Conto == CC.Id_Conto ? contiGemellati[1] : contiGemellati[0];
                            IdContoRicevente = conto.Id_Conto;
                            IdGestioneRicevente = conto.Id_Gestione;

                        }
                        else
                        {
                            QuoteTab socioRicevente = _managerLiquidServices.GetQuoteTabById(CC.Id_Quote_Investimenti);
                            IdInvestitoreRicevente = socioRicevente.Id_Gestione;
                        }
                    }
                    else
                    {

                    }
                }
                
            }
        }

        private void CreaDataGrid(object sender, QuoteTabList qtl, ContoCorrenteList ccl, DataGrid dataGrid)
        {
            //ComboBox cB = sender as ComboBox;
            //DataGrid dataGrid = (DataGrid)cB.FindName("DGMittente");
            if (dataGrid.Columns.Count == 0 && (ccl != null || qtl != null))
            {
                Style cellStyle = new Style(typeof(DataGridCell));
                Style cellLeftStyle = new Style(typeof(DataGridCell));
                Style cellRightStyle = new Style(typeof(DataGridCell));
                Setter setAlign = new Setter(DataGridCell.HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Right);
                Setter setLeftAlign = new Setter(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                Setter setRightAlign = new Setter(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Right);
                cellStyle.Setters.Add(setAlign);
                cellLeftStyle.Setters.Add(setLeftAlign);
                cellRightStyle.Setters.Add(setRightAlign);
                DataGridTextColumn txtColTxt = new DataGridTextColumn();
                DataGridTextColumn txtColTxt2 = new DataGridTextColumn();
                DataGridTextColumn txtColVal = new DataGridTextColumn();
                DataGridTextColumn txtCol2 = new DataGridTextColumn();
                dataGrid.Columns.Add(txtColTxt);
                if (ccl != null) { dataGrid.Columns.Add(txtColTxt2); }
                dataGrid.Columns.Add(txtColVal);
                dataGrid.Columns.Add(txtCol2);
                txtColTxt.Header = qtl != null ? "Nome" : "Conto Corrente";
                txtColTxt2.Header = "Gestione";
                txtColVal.Header = "Soldi";
                txtCol2.Header = "Valuta";
                txtColTxt.Binding = qtl != null ? new Binding("NomeInvestitore") : new Binding("Desc_Conto");
                txtColTxt2.Binding = new Binding("NomeGestione");
                txtColTxt.CellStyle = cellLeftStyle;
                txtColVal.CellStyle = cellStyle;
                txtCol2.CellStyle = cellRightStyle;
                if (qtl != null)
                {
                    txtColVal.Binding = new Binding("AmmontareValuta") { StringFormat = "N2", ConverterCulture = new System.Globalization.CultureInfo("it-IT") };
                    txtCol2.Binding = new Binding("CodeCurrency");
                }
                else
                {
                    txtColVal.Binding = new Binding("Ammontare") { StringFormat = "N2", ConverterCulture = new System.Globalization.CultureInfo("it-IT") };
                    txtCol2.Binding = new Binding("Cod_Valuta");
                }
            }
            if (qtl != null)
                dataGrid.ItemsSource = qtl;
            else
                dataGrid.ItemsSource = ccl;
        }

        #endregion

        #region Commands
        public void CloseMe(object param)
        {
            GiroContoView view = param as GiroContoView;
            DockPanel wp = view.Parent as DockPanel;
            wp.Children.Remove(view);
        }

        public void ClearMe(object param)
        {
            GiroContoView view = param as GiroContoView;
            DataGrid grid = (DataGrid) view.FindName("DGMittente");
            grid.ItemsSource = null;
            grid.Columns.Clear();
            grid = (DataGrid) view.FindName("DGRicevente");
            grid.ItemsSource = null;
            grid.Columns.Clear();
            ClearData();
        }

        public bool CanSave(object param)
        {
            if (ChiMittente == false && ChiRicevente == false && ValutaEnabled == true && Valore > 0 && dataModifica != new DateTime())
            {
                if (TotaleMittenteQuote != null)
                {
                    QuoteTab qt = TotaleMittenteQuote[0];
                    if (qt.AmmontareValuta < Valore)
                        return false;
                }
                else
                {
                    ContoCorrente cc = TotaleMittenteConto[0];
                    if (cc.Ammontare < Valore)
                        return false;
                }
                return true;
            }
            return false;
        }

        public void SaveCommand(object param)
        {
            try
            {
                int id_quote = 0;
                if (TotaleMittenteQuote != null)
                {
                    QuoteTab ActualQuote = new QuoteTab();
                    ActualQuote.Id_Valuta = IdValuta;
                    ActualQuote.DataMovimento = dataModifica;
                    ActualQuote.AmmontareValuta = Valore < 0 ? Valore : Valore * -1;
                    ActualQuote.Id_tipo_movimento = 12;
                    ActualQuote.Id_Gestione = IdInvestitoreMittente;
                    ActualQuote.Id_Periodo_Quote = 0;
                    ActualQuote.ChangeValue = 0;
                    ActualQuote.AmmontareEuro = 0;
                    ActualQuote.Note = causale;
                    _managerLiquidServices.InsertInvestment(ActualQuote);
                    id_quote = _managerLiquidServices.GetIdQuoteTab(ActualQuote);
                }
                if (TotaleRiceventeQuote != null)
                {
                    QuoteTab ActualQuote = new QuoteTab();
                    ActualQuote.Id_Valuta = IdValuta;
                    ActualQuote.DataMovimento = dataModifica;
                    ActualQuote.AmmontareValuta = Valore;
                    ActualQuote.Id_tipo_movimento = 12;
                    ActualQuote.Id_Gestione = IdInvestitoreRicevente;
                    ActualQuote.Id_Periodo_Quote = 0;
                    ActualQuote.ChangeValue = 0;
                    ActualQuote.AmmontareEuro = 0;
                    ActualQuote.Note = causale;
                    _managerLiquidServices.InsertInvestment(ActualQuote);
                    id_quote = _managerLiquidServices.GetIdQuoteTab(ActualQuote);
                }
                if (TotaleMittenteConto != null)
                {
                    ContoCorrente contoCorrente = new ContoCorrente();
                    contoCorrente.Id_Valuta = IdValuta;
                    contoCorrente.Id_Conto = IdContoMittente;
                    contoCorrente.Id_Quote_Investimenti = id_quote;
                    contoCorrente.DataMovimento = dataModifica;
                    contoCorrente.Id_Valuta = IdValuta;
                    contoCorrente.Id_Portafoglio_Titoli = 0;
                    contoCorrente.Id_tipo_movimento = 12;
                    contoCorrente.Id_Gestione = IdGestioneMittente;
                    contoCorrente.Id_Titolo = 0;
                    contoCorrente.Ammontare = Valore * -1;
                    contoCorrente.Valore_Cambio = 0;
                    contoCorrente.Causale = causale;
                    contoCorrente.Id_Tipo_Soldi = 1;    // nel prossimo aggiornamento togliere il tipo soldi
                    contoCorrente.Id_Quote_Periodi = 0;
                    contoCorrente.Modified = DateTime.Now;
                    _managerLiquidServices.InsertAccountMovement(contoCorrente);
                }
                if (TotaleRiceventeConto != null)
                {
                    ContoCorrente contoCorrente = new ContoCorrente();
                    contoCorrente.Id_Valuta = IdValuta;
                    contoCorrente.Id_Conto = IdContoRicevente;
                    contoCorrente.Id_Quote_Investimenti = id_quote;
                    contoCorrente.DataMovimento = dataModifica;
                    contoCorrente.Id_Valuta = IdValuta;
                    contoCorrente.Id_Portafoglio_Titoli = 0;
                    contoCorrente.Id_tipo_movimento = 12;
                    contoCorrente.Id_Gestione = IdGestioneRicevente;
                    contoCorrente.Id_Titolo = 0;
                    contoCorrente.Ammontare = Valore > 0 ? Valore : Valore * -1;
                    contoCorrente.Valore_Cambio = 0;
                    contoCorrente.Causale = causale;
                    contoCorrente.Id_Tipo_Soldi = 1;    // nel prossimo aggiornamento togliere il tipo soldi
                    contoCorrente.Id_Quote_Periodi = 0;
                    contoCorrente.Modified = DateTime.Now;
                    _managerLiquidServices.InsertAccountMovement(contoCorrente);
                }
                ClearMe(param);
                MessageBox.Show("Il passaggio di soldi è avvenuto con successo.", "Trasferimento Soldi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception err)
            {
                MessageBox.Show(string.Format("Modica dei dati con errori: ", err.Message), "Errore Trasferimento Soldi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool CanModify(object param)
        {
            //if(param == null)

            return false;
        }

        public void UpdateCommand(object param)
        {

        }

        #endregion

    }
}

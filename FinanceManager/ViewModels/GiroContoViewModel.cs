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
using System.Linq.Expressions;
using NPOI.SS.Formula.Functions;

namespace FinanceManager.ViewModels
{
    public class GiroContoViewModel : ViewModelBase
    {
        private readonly IRegistryServices _registryServices;
        private readonly IManagerLiquidAssetServices _managerLiquidServices;
        private readonly IContoCorrenteServices _contoCorrenteServices;

        public ICommand CloseMeCommand { get; set; }
        public ICommand ClearMeCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }

        public GiroContoViewModel
            (IRegistryServices registryServices, IManagerLiquidAssetServices managerLiquidServices, IContoCorrenteServices contoCorrenteServices)
        {
            _registryServices = registryServices ?? throw new ArgumentNullException("Manca collegamento con richiesta dati anagrafica Giroconto View Model");
            _managerLiquidServices = managerLiquidServices ?? throw new ArgumentNullException("Manca collegamento con richiesta dati conti Giroconto View Model");
            _contoCorrenteServices = contoCorrenteServices ?? throw new ArgumentNullException("Manca Concocorrente services");
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
            //------------------- in doppio per i combo box Mittente e Ricevente ------------------------------
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
            //===================================================================================================
        }
        private void ClearData()
        {
            //------- I DATI DELLE GRIGLIE DI CONTI CORRENTI E CONTO INVESTITORE SOLO GIROCONTO ----------------
            ContoCorrenteList conti_Correnti = new ContoCorrenteList();
            conti_Correnti = _contoCorrenteServices.GetContoCorrenteList();
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
            //===================================================================================================
            ActualQuote = new QuoteTab();
            ActualCCmittente = new ContoCorrente();
            ActualCCricevente = new ContoCorrente();
            ActualQuote.Id_tipo_movimento = 12;
            ActualCCmittente.Id_tipo_movimento = 12;
            ActualCCricevente.Id_tipo_movimento = 12;

            TotaleMittenteQuote = new QuoteTabList();
            TotaleRiceventeQuote = new QuoteTabList();
            TotaleMittenteConto = new ContoCorrenteList();
            TotaleRiceventeConto = new ContoCorrenteList();
            ChiMittente = true;
            ContoMittente = true;
            GestioneMittente = false;
            ChiRicevente = false;
            ContoRicevente = false;
            GestioneRicevente = false;
            ValutaEnabled = false;
            IdInvestitoreMittente = 0;
            IdInvestitoreRicevente = 0;
            IdContoMittente = 0;
            IdGestioneMittente = 0;
            IdContoRicevente = 0;
            IdGestioneRicevente = 0;
            IdValuta = 0;
            Valore = 0;
            DataModifica = DateTime.Now;
            Causale = "";
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
        /// <summary>
        /// E' il record dedicato agli investimenti dei soci
        /// </summary>
        public QuoteTab ActualQuote
        {
            get { return GetValue(() => ActualQuote); }
            set { SetValue(() => ActualQuote, value); }
        }
        /// <summary>
        /// E' il record per il giroconto mittente
        /// </summary>
        public ContoCorrente ActualCCmittente
        {
            get { return GetValue(() => ActualCCmittente); }
            set { SetValue(() => ActualCCmittente, value); }
        }
        /// <summary>
        /// E' il record per il giroconto ricevente
        /// </summary>
        public ContoCorrente ActualCCricevente
        {
            get { return GetValue(() => ActualCCricevente); }
            set { SetValue(() => ActualCCricevente, value); }
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
        // compila la griglia con la disponibilità finanziaria per selezione ------------------
        public QuoteTabList TotaleMittenteQuote { get; set; }
        public QuoteTabList TotaleRiceventeQuote { get; set; }
        public ContoCorrenteList TotaleMittenteConto { get; set; }
        public ContoCorrenteList TotaleRiceventeConto { get; set; }
        //=======================================================================================

        #region dati da registrare
        /// <summary>
        /// è il valore da trasferire
        /// </summary>
        public double Valore
        {
            get { return GetValue(() => Valore); }
            set { SetValue(() => Valore, value); }
        }

        public DateTime DataModifica
        {
            get { return GetValue(() => DataModifica); }
            set { SetValue(() => DataModifica, value); }
        }

        public string Causale
        {
            get { return GetValue(() => Causale); }
            set { SetValue(() => Causale, value); }
        }
        #endregion dati da registrare

        #endregion getter & setter

        #region Events
        public void LostFocus(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text != "" && ((TextBox)sender).Name != "Causale")
            {
                try { Valore = Convert.ToDouble(((TextBox)sender).Text); } catch { }
            }
            else if (((TextBox)sender).Text != "" && ((TextBox)sender).Name == "Causale")
            {
                ActualQuote.Note = Causale;
                ActualCCmittente.Causale = Causale;
                ActualCCricevente.Causale = Causale;
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
                DataModifica = (DateTime)e.AddedItems[0];

            }
            else if (e.AddedItems.Count > 0 && sender is ComboBox CB)
            {
                string CBname = CB.Name;
                // --- ATTIVO SELETTIVAMENTE I COMBO BOX E AGGIORNO LE GRIGLIE ---------
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
                        GestioneMittente = true;
                        IdContoMittente = ((RegistryLocation)e.AddedItems[0]).Id_Conto;
                        TotaleMittenteQuote = null;
                        TotaleMittenteConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoMittente);
                        CreaDataGrid(sender, TotaleMittenteQuote, TotaleMittenteConto, (DataGrid)CB.FindName("DGMittente"));
                        break;
                    case "GestioneMittente":
                        ContoMittente = false;
                        ChiRicevente = true;
                        ContoRicevente = true;
                        IdGestioneMittente = ((RegistryOwner)e.AddedItems[0]).Id_gestione;
                        TotaleMittenteConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoMittente, IdGestioneMittente);
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
                        TotaleRiceventeConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoRicevente);
                        CreaDataGrid(sender, TotaleRiceventeQuote, TotaleRiceventeConto, (DataGrid)CB.FindName("DGRicevente"));
                        break;
                    case "GestioneRicevente":
                        ContoRicevente = false;
                        ValutaEnabled = true;
                        IdGestioneRicevente = ((RegistryOwner)e.AddedItems[0]).Id_gestione;
                        TotaleRiceventeConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoRicevente, IdGestioneRicevente);
                        CreaDataGrid(sender, TotaleRiceventeQuote, TotaleRiceventeConto, (DataGrid)CB.FindName("DGRicevente"));
                        break;
                    case "Valuta":
                        GestioneRicevente = false;
                        ChiRicevente = false;
                        IdValuta = ((RegistryCurrency)e.AddedItems[0]).IdCurrency;
                        ActualQuote.Id_Valuta = IdValuta;
                        ActualCCmittente.Id_Valuta = IdValuta;
                        ActualCCricevente.Id_Valuta = IdValuta;
                        if (TotaleMittenteQuote != null)
                            TotaleMittenteQuote = _managerLiquidServices.GetTotalAmountByCurrency(IdInvestitoreMittente, IdValuta);
                        else
                            TotaleMittenteConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoMittente, IdGestioneMittente, IdValuta);
                        CreaDataGrid(sender, TotaleMittenteQuote, TotaleMittenteConto, (DataGrid)CB.FindName("DGMittente"));
                        if (TotaleRiceventeQuote != null)
                            TotaleRiceventeQuote = _managerLiquidServices.GetTotalAmountByCurrency(IdInvestitoreRicevente, IdValuta);
                        else
                            TotaleRiceventeConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoRicevente, IdGestioneRicevente, IdValuta);
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
                        IdContoRicevente = ActualCCricevente.Id_Conto;
                        IdGestioneRicevente = ActualCCricevente.Id_Gestione;
                        IdValuta = QT.Id_Valuta;
                        Valore = QT.AmmontareValuta * -1;
                    }
                    else
                    {
                        IdInvestitoreRicevente = QT.Id_Gestione;
                        IdValuta = QT.Id_Valuta;
                        Valore = QT.AmmontareValuta;
                        IdContoMittente = ActualCCmittente.Id_Conto;
                        IdGestioneMittente = ActualCCmittente.Id_Gestione;
                    }
                    ActualQuote = QT;
                }
                else if (DS.Name == "CONTOtrasfatti" && e.AddedItems[0] is ContoCorrente CC)
                {
                    if (CC.Ammontare < 0)
                    {
                        IdContoMittente = CC.Id_Conto;
                        IdGestioneMittente = CC.Id_Gestione;
                        Valore = CC.Ammontare;
                        IdValuta = CC.Id_Valuta;
                        ActualCCmittente = CC;
                        //if (CC.Id_Quote_Investimenti == 0)
                        //{
                        //    ContoCorrenteList contiGemellati = _contoCorrenteServices.Get2ContoCorrentes(CC.Modified);
                        //    ActualCCricevente = contiGemellati[0].Id_Conto == CC.Id_Conto ? contiGemellati[1] : contiGemellati[0];
                        //    IdContoRicevente = ActualCCricevente.Id_Conto;
                        //    IdGestioneRicevente = ActualCCricevente.Id_Gestione;

                        //}
                        //else
                        //{
                        //    ActualQuote = _managerLiquidServices.GetQuoteTabById(CC.Id_Quote_Investimenti);
                        //    IdInvestitoreRicevente = ActualQuote.Id_Gestione;
                        //}
                    }
                    else
                    {
                        IdContoRicevente = CC.Id_Conto;
                        IdGestioneRicevente = CC.Id_Gestione;
                        Valore = CC.Ammontare;
                        IdValuta = CC.Id_Valuta;
                        ActualCCricevente = CC;
                        //if (CC.Id_Quote_Investimenti == 0)
                        //{
                        //    ContoCorrenteList contiGemellati = _contoCorrenteServices.Get2ContoCorrentes(CC.Modified);
                        //    ActualCCmittente = contiGemellati[0].Id_Conto == CC.Id_Conto ? contiGemellati[1] : contiGemellati[0];
                        //    IdContoMittente = ActualCCmittente.Id_Conto;
                        //    IdGestioneMittente = ActualCCmittente.Id_Gestione;
                        //}
                        //else
                        //{
                        //    ActualQuote = _managerLiquidServices.GetQuoteTabById(CC.Id_Quote_Investimenti);
                        //    IdInvestitoreMittente = ActualQuote.Id_Gestione;
                        //}
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
            DataGrid grid = (DataGrid)view.FindName("DGMittente");
            grid.ItemsSource = null;
            grid.Columns.Clear();
            grid = (DataGrid)view.FindName("DGRicevente");
            grid.ItemsSource = null;
            grid.Columns.Clear();
            ClearData();
        }

        public bool CanSave(object param)
        {
            if (ChiMittente == false && ChiRicevente == false && ValutaEnabled == true && Valore > 0)
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
            Mouse.SetCursor(Cursors.Wait);
            int id_quote = 0;
            if (TotaleMittenteQuote != null && TotaleRiceventeConto != null)
                try
                {
                    id_quote = AddActualQuote(Valore < 0 ? Valore : Valore * -1, IdInvestitoreMittente);
                    AddActualCC(IdContoRicevente, id_quote, IdGestioneRicevente, Valore > 0 ? Valore : Valore * -1, DateTime.Now);
                }
                catch (Exception err)
                {
                    if (id_quote != 0)  // l'errore è successivo quindi devo eliminare il recod quote
                        _managerLiquidServices.DeleteRecordQuoteTab(id_quote);
                    Mouse.SetCursor(Cursors.Arrow);
                    MessageBox.Show(string.Format("Modica dei dati con errori: ", Environment.NewLine, err.Message),
                        "Giroconto", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            if (TotaleRiceventeQuote != null && TotaleMittenteConto != null)
                try
                {
                    id_quote = AddActualQuote(Valore > 0 ? Valore : Valore * -1, IdInvestitoreRicevente);
                    AddActualCC(IdContoMittente, id_quote, IdGestioneMittente, Valore < 0 ? Valore : Valore * -1, DateTime.Now);
                }
                catch (Exception err)
                {
                    if (id_quote != 0)  // l'errore è successivo quindi devo eliminare il recod quote
                        _managerLiquidServices.DeleteRecordQuoteTab(id_quote);
                    Mouse.SetCursor(Cursors.Arrow);
                    MessageBox.Show(string.Format("Modica dei dati con errori: ", Environment.NewLine, err.Message),
                        "Giroconto", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            else
                try
                {
                    AddActualCC(IdContoMittente, id_quote, IdGestioneMittente, Valore < 0 ? Valore : Valore * -1, DateTime.Now);
                    ContoCorrente cc1 = _contoCorrenteServices.GetLastContoCorrente();
                    try
                    {
                        AddActualCC(IdContoRicevente, id_quote, IdGestioneRicevente, Valore > 0 ? Valore : Valore * -1, cc1.Modified);
                    }
                    catch(Exception err)
                    {
                        // devo eliminare cc1 per il mittente che è anche l'ultimo record inserito
                        _contoCorrenteServices.DeleteRecordContoCorrente(cc1.Id_Conto);
                        Mouse.SetCursor(Cursors.Arrow);
                        MessageBox.Show(string.Format("Modifica dei dati con errori: ", Environment.NewLine, err.Message, Environment.NewLine,
                            "Il record mittente ", cc1.Id_Conto, " è stato eliminato."), "Giroconto", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception err)
                {
                    // non devo eliminare nulla in quanto l'errore è avvenuto nella prima scrittura
                    Mouse.SetCursor(Cursors.Arrow);
                    MessageBox.Show(string.Format("Modica dei dati con errori: ", Environment.NewLine, err.Message),
                        "Giroconto", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            Mouse.SetCursor(Cursors.Arrow);
            MessageBox.Show("Trasferimento effettuato", "Giroconto", MessageBoxButton.OK, MessageBoxImage.Information);
            ClearMe(param);
        }

        private int AddActualQuote(double valore, int gestione)
        {
            ActualQuote.AmmontareValuta = valore;
            ActualQuote.Id_Gestione = gestione;
            ActualQuote.Id_Periodo_Quote = 0;
            ActualQuote.ChangeValue = 0;
            ActualQuote.AmmontareEuro = 0;
            ActualQuote.DataMovimento = DataModifica;
            _managerLiquidServices.InsertInvestment(ActualQuote);
            return _managerLiquidServices.GetIdQuoteTab(ActualQuote);
        }

        private void AddActualCC(int conto, int quota, int gestione, double ammontare, DateTime dateLink)
        {
            ContoCorrente cc = new ContoCorrente();
            cc.Id_Conto = conto;
            //cc.Id_Quote_Investimenti = quota;
            cc.Id_Valuta = IdValuta;
            cc.Id_Portafoglio_Titoli = 0;
            cc.Id_tipo_movimento = 12;
            cc.Id_Gestione = gestione;
            cc.Id_Titolo = 0;
            cc.Ammontare = ammontare;
            cc.DataMovimento = DataModifica;
            cc.Valore_Cambio = 0;
            cc.Id_Tipo_Soldi = 1;    // nel prossimo aggiornamento togliere il tipo soldi
            cc.Id_Quote_Periodi = 0;
            cc.Modified = dateLink;
            //_managerLiquidServices.InsertAccountMovement(cc);
        }

        public bool CanModify(object param)
        {
            if (ActualQuote.Id_Quote_Investimenti > 0 || ActualCCmittente.Id_RowConto > 0 || ActualCCricevente.Id_RowConto > 0)
                return true;

            return false;
        }

        public void UpdateCommand(object param)
        {
            try
            {
                if (ActualQuote.Id_Quote_Investimenti > 0)
                {
                    _managerLiquidServices.UpdateQuoteTab(ActualQuote);
                    if (ActualCCmittente.Id_RowConto > 0)
                        _contoCorrenteServices.UpdateRecordContoCorrente(ActualCCmittente, 0);
                    else
                        _contoCorrenteServices.UpdateRecordContoCorrente(ActualCCricevente, 0);
                }
                else
                {
                    _contoCorrenteServices.UpdateRecordContoCorrente(ActualCCmittente, 0);
                    _contoCorrenteServices.UpdateRecordContoCorrente(ActualCCricevente, 0);
                }
                ClearMe(param);
                MessageBox.Show("La modifica è avvenuto con successo.", "Trasferimento Soldi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("Modica dei dati con errori: ", err.Message), "Errore Trasferimento Soldi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

    }
}

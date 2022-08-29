using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Models.Enumeratori;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class AcquistoVenditaTitoliViewModel : ViewModelBase
    {
        private IRegistryServices _registryServices;
        private IManagerLiquidAssetServices _liquidAssetServices;
        private double _CurrencyAvailable;
        private ObservableCollection<RegistryShare> _SharesList;
        public ICommand CloseMeCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand EraseCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        Predicate<object> _Filter;

        private TabControl _TabControl = new TabControl();

        public AcquistoVenditaTitoliViewModel
            (IRegistryServices services, IManagerLiquidAssetServices liquidAssetServices)
        {
            _registryServices = services ?? throw new ArgumentNullException("Services in Manager Portfolio Movement View Model");
            _liquidAssetServices = liquidAssetServices ?? throw new ArgumentNullException("Liquid Asset Services in Manager Portfolio Movement View Model");
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
                ListTipoTitoli = new RegistryShareTypeList();
                RegistryMovementTypeList listaOriginale = new RegistryMovementTypeList();
                listaOriginale = _registryServices.GetRegistryMovementTypesList();
                var RMTL = from movimento in listaOriginale
                           where (movimento.Id_tipo_movimento == 5 || movimento.Id_tipo_movimento == 6 || movimento.Id_tipo_movimento == 13 || movimento.Id_tipo_movimento == 14)
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
                ListTipoTitoli = _registryServices.GetRegistryShareTypeList();
                SharesList = new ObservableCollection<RegistryShare>(_registryServices.GetRegistryShareList());
                _Filter = new Predicate<object>(Filter);
            }
            catch (Exception err)
            {
                throw new Exception("Errore nel setup." + Environment.NewLine + err.Message);
            }
        }

        private void Init()
        {
            TobinOk = false;
            DisaggioOk = false;
            RitenutaOk = false;
            CanUpdateDelete = false;
            ImportoTotale = 0;
            TotalLocalValue = 0;
            TotaleContabile = 0;
            AmountChangedValue = 0;
            try
            {
                RecordPortafoglioTitoli = new PortafoglioTitoli();
                ListPortafoglioTitoli = _liquidAssetServices.GetManagerLiquidAssetListByOwnerAndLocation();
                ListCostiMediTitoli = _liquidAssetServices.GetCostiMediPerTitolo();
            }
            catch (Exception err)
            {
                throw new Exception("Errore in init." + Environment.NewLine + err.Message);
            }
            SrchShares = "";
            Conto = "";
            Gestione = "";
            ISIN = "";
            IdTipoTitolo = 0;
        }

        #region events
        /// <summary>
        /// Gestore dell'evento nei combo box dei parametri comuni
        /// </summary>
        /// <param name="sender">Combo Box</param>
        /// <param name="e">Cambio scelta item</param>
        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is RegistryMovementType RMT)
                {
                    RecordPortafoglioTitoli.Id_tipo_movimento = RMT.Id_tipo_movimento;
                    RecordPortafoglioTitoli.Desc_tipo_movimento = RMT.Desc_tipo_movimento;
                }
                if (e.AddedItems[0] is RegistryLocation RL)
                {
                    RecordPortafoglioTitoli.Id_Conto = RL.Id_Conto;
                    RecordPortafoglioTitoli.Desc_Conto = RL.Desc_Conto;
                    Conto = RL.Desc_Conto;
                }
                if (e.AddedItems[0] is RegistryOwner RO)
                {
                    RecordPortafoglioTitoli.Id_gestione = RO.Id_gestione;
                    RecordPortafoglioTitoli.Nome_Gestione = RO.Nome_Gestione;
                    Gestione = RO.Nome_Gestione;
                }
                if (e.AddedItems[0] is RegistryCurrency RC)
                {
                    RecordPortafoglioTitoli.Cod_valuta = RC.CodeCurrency;
                    RecordPortafoglioTitoli.Id_valuta = RC.IdCurrency;
                }
                if (e.AddedItems[0] is RegistryShare RS)
                {
                    RecordPortafoglioTitoli.Id_titolo = RS.id_titolo;
                    RecordPortafoglioTitoli.Id_tipo_titolo = RS.id_tipo_titolo;
                    RecordPortafoglioTitoli.Isin = RS.Isin;
                    RecordPortafoglioTitoli.Id_azienda = RS.id_azienda;
                    ISIN = RS.Isin;
                }
                if (e.AddedItems[0] is RegistryShareType RST)
                {
                    IdTipoTitolo = (int)RST.id_tipo_titolo;
                }
                if (e.AddedItems[0] is DateTime DT)
                {
                    RecordPortafoglioTitoli.Data_Movimento = DT.Date;
                }
                SharesOwned = _liquidAssetServices.GetSharesQuantity(RecordPortafoglioTitoli.Id_gestione, RecordPortafoglioTitoli.Id_Conto, (uint)RecordPortafoglioTitoli.Id_titolo);
                UpdateTotals();
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
            if (e.AddedItems[0] is PortafoglioTitoli PT)
            {
                RecordPortafoglioTitoli = PT;
                UpdateTotals();
                CanUpdateDelete = true;
                CanInsert = false;
            }
        }

        /// <summary>
        /// Controlla che il punto del tastierino numerico venga trasformato in virgola
        /// </summary>
        /// <param name="sender">Tastiera</param>
        /// <param name="e">Pressione del tasto</param>
        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Name.Contains("double"))
                if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
                {
                    int pos = textBox.SelectionStart;
                    textBox.Text = textBox.Text.Insert(pos, ",");
                    textBox.SelectionStart = pos + 1;
                    e.Handled = true;
                }
        }

        /// <summary>
        /// Gestore dell'evento nei text box dei campi da riempire
        /// </summary>
        /// <param name="sender">Text Box</param>
        /// <param name="e">Uscita del campo</param>
        public void LostFocus(object sender, EventArgs e)
        {
            TextBox TB = sender as TextBox;
            if (!string.IsNullOrEmpty(TB.Text) && double.TryParse(TB.Text, out _))
            {
                switch (TB.Name)
                {
                    case "doubleUnityLocalAmount":
                        RecordPortafoglioTitoli.Costo_unitario_in_valuta = Convert.ToDouble(TB.Text);
                        break;
                    case "doubleNShares":
                        if (RecordPortafoglioTitoli.Id_tipo_movimento == 5 && Convert.ToDouble(TB.Text) > 0)
                        {
                            RecordPortafoglioTitoli.N_titoli = Convert.ToDouble(TB.Text);
                        }
                        else if (RecordPortafoglioTitoli.Id_tipo_movimento == 6 && Convert.ToDouble(TB.Text) < 0)
                        {
                            RecordPortafoglioTitoli.N_titoli = Convert.ToDouble(TB.Text);
                        }
                        else
                        {
                            MessageBox.Show("Inserire un importo positivo se si compra o negativo se si vende.", "DAF-C registrazione movimenti titoli", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        break;
                    case "doubleCommissionValue":
                        RecordPortafoglioTitoli.Commissioni_totale = Convert.ToDouble(TB.Text);
                        break;
                    case "doubleValore_di_cambio":
                        RecordPortafoglioTitoli.Valore_di_cambio = Convert.ToDouble(TB.Text);
                        break;
                    case "doubleTobinTaxValue":
                        RecordPortafoglioTitoli.TobinTax = Convert.ToDouble(TB.Text);
                        break;
                    case "doubleRitenutaFiscale":
                        RecordPortafoglioTitoli.RitenutaFiscale = Convert.ToDouble(TB.Text);
                        break;
                    case "doubleDisaggioValue":
                        RecordPortafoglioTitoli.Disaggio_anticipo_cedole = Convert.ToDouble(TB.Text);
                        break;
                    case "Note":
                        RecordPortafoglioTitoli.Note = TB.Text;
                        break;
                }
            }
            UpdateTotals();
        }

        /// <summary>
        /// Verifico e aggiorno i totali da pubblicare e registrare
        /// </summary>
        private void UpdateTotals()
        {
            // verifico che ci sia un importo unitario e un numero di azioni
            if (RecordPortafoglioTitoli.Costo_unitario_in_valuta >= 0 && RecordPortafoglioTitoli.N_titoli != 0)
            {
                if (RecordPortafoglioTitoli.Id_portafoglio == 0)
                    CanInsert = true;
                // Totale 1 VALORE TRANSAZIONE (conteggio diverso fra obbligazioni e tutto il resto)
                RecordPortafoglioTitoli.Importo_totale = RecordPortafoglioTitoli.Id_tipo_titolo != 2 ? -1 * (RecordPortafoglioTitoli.Costo_unitario_in_valuta * RecordPortafoglioTitoli.N_titoli) :
                    -1 * (RecordPortafoglioTitoli.Costo_unitario_in_valuta * RecordPortafoglioTitoli.N_titoli) / 100;
                ImportoTotale = RecordPortafoglioTitoli.Importo_totale;

                if (RecordPortafoglioTitoli.Id_tipo_movimento == 5) //acquisto
                {
                    // totale 2 valore transazione più commissioni (negativo in caso di acquisto)
                    TotalLocalValue = RecordPortafoglioTitoli.Importo_totale + RecordPortafoglioTitoli.Commissioni_totale * -1;
                    // totale contabile in valuta
                    TotaleContabile = RecordPortafoglioTitoli.Id_valuta == 1 ?
                        TotalLocalValue + (RecordPortafoglioTitoli.TobinTax + RecordPortafoglioTitoli.Disaggio_anticipo_cedole + RecordPortafoglioTitoli.RitenutaFiscale) * -1 :
                        TotalLocalValue + (RecordPortafoglioTitoli.Disaggio_anticipo_cedole + (RecordPortafoglioTitoli.RitenutaFiscale * RecordPortafoglioTitoli.Valore_di_cambio)) * -1;
                    if ((RecordPortafoglioTitoli.Id_tipo_titolo == 1 || RecordPortafoglioTitoli.Id_tipo_titolo == 4) && RecordPortafoglioTitoli.Valore_di_cambio != 1)
                    {
                        TobinOk = true;
                    }
                    else
                    {
                        TobinOk = false;
                    }
                    if (RecordPortafoglioTitoli.Id_tipo_titolo == 2)
                    {
                        DisaggioOk = true;
                    }
                    else
                    {
                        DisaggioOk = false;
                    }
                }
                else if (RecordPortafoglioTitoli.Id_tipo_movimento == 6) //vendita
                {
                    // totale 2 valore transazione più commissioni (positivo in caso di vendita)
                    TotalLocalValue = RecordPortafoglioTitoli.Importo_totale - RecordPortafoglioTitoli.Commissioni_totale;
                    // totale contabile in valuta
                    TotaleContabile = RecordPortafoglioTitoli.Id_valuta == 1 ?
                        TotalLocalValue - (RecordPortafoglioTitoli.TobinTax + RecordPortafoglioTitoli.Disaggio_anticipo_cedole + RecordPortafoglioTitoli.RitenutaFiscale) :
                        TotalLocalValue - (RecordPortafoglioTitoli.Disaggio_anticipo_cedole + (RecordPortafoglioTitoli.RitenutaFiscale * RecordPortafoglioTitoli.Valore_di_cambio));
                    RitenutaOk = true;
                }
            }
            AmountChangedValue = TotaleContabile;
            // totale contabile in euro calcolato solo se è inserito un valore di cambio
            if (RecordPortafoglioTitoli.Id_valuta != 1)
                AmountChangedValue = RecordPortafoglioTitoli.Valore_di_cambio == 0 ? 0 : (TotaleContabile / RecordPortafoglioTitoli.Valore_di_cambio);
        }

        /// <summary>
        /// E' il filtro da applicare all'elenco delle azioni
        /// e contestualmente al datagrid sottostante
        /// </summary>
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
                else if (obj is PortafoglioTitoli Ptf)
                {
                    if (!string.IsNullOrWhiteSpace(Conto) && !string.IsNullOrWhiteSpace(Gestione) && IdTipoTitolo > 0 && !string.IsNullOrWhiteSpace(ISIN)) // 4 filtri su 4
                    {
                        return Ptf.Desc_Conto.ToLower().Contains(Conto.ToLower()) && Ptf.Nome_Gestione.ToLower().Contains(Gestione.ToLower()) && Ptf.Id_tipo_titolo == IdTipoTitolo && Ptf.Isin.ToLower().Contains(ISIN.ToLower());
                    }
                    else if (!string.IsNullOrWhiteSpace(Conto) && !string.IsNullOrWhiteSpace(Gestione) && IdTipoTitolo > 0 && string.IsNullOrWhiteSpace(ISIN)) // 3 filtri su 4
                    {
                        return Ptf.Desc_Conto.ToLower().Contains(Conto.ToLower()) && Ptf.Nome_Gestione.ToLower().Contains(Gestione.ToLower()) && Ptf.Id_tipo_titolo == IdTipoTitolo;
                    }
                    else if (!string.IsNullOrWhiteSpace(Conto) && string.IsNullOrWhiteSpace(Gestione) && IdTipoTitolo > 0 && !string.IsNullOrWhiteSpace(ISIN)) // 3 filtri su 4
                    {
                        return Ptf.Desc_Conto.ToLower().Contains(Conto.ToLower()) && Ptf.Id_tipo_titolo == IdTipoTitolo && Ptf.Isin.ToLower().Contains(ISIN.ToLower());
                    }
                    else if (!string.IsNullOrWhiteSpace(Conto) && !string.IsNullOrWhiteSpace(Gestione) && IdTipoTitolo == 0 && !string.IsNullOrWhiteSpace(ISIN)) // 3 filtri su 4
                    {
                        return Ptf.Desc_Conto.ToLower().Contains(Conto.ToLower()) && Ptf.Nome_Gestione.ToLower().Contains(Gestione.ToLower()) && Ptf.Isin.ToLower().Contains(ISIN.ToLower());
                    }
                    else if (string.IsNullOrWhiteSpace(Conto) && !string.IsNullOrWhiteSpace(Gestione) && IdTipoTitolo > 0 && !string.IsNullOrWhiteSpace(ISIN)) // 3 filtri su 4
                    {
                        return Ptf.Nome_Gestione.ToLower().Contains(Gestione.ToLower()) && Ptf.Id_tipo_titolo == IdTipoTitolo && Ptf.Isin.ToLower().Contains(ISIN.ToLower());
                    }
                    else if (!string.IsNullOrWhiteSpace(Conto) && !string.IsNullOrWhiteSpace(Gestione) && IdTipoTitolo == 0 && string.IsNullOrWhiteSpace(ISIN)) // 2 filtri su 4
                    {
                        return Ptf.Desc_Conto.ToLower().Contains(Conto.ToLower()) && Ptf.Nome_Gestione.ToLower().Contains(Gestione.ToLower());
                    }
                    else if (!string.IsNullOrWhiteSpace(Conto) && string.IsNullOrWhiteSpace(Gestione) && IdTipoTitolo > 0 && string.IsNullOrWhiteSpace(ISIN)) // 2 filtri su 4
                    {
                        return Ptf.Desc_Conto.ToLower().Contains(Conto.ToLower()) && Ptf.Id_tipo_titolo == IdTipoTitolo;
                    }
                    else if (string.IsNullOrWhiteSpace(Conto) && !string.IsNullOrWhiteSpace(Gestione) && IdTipoTitolo > 0 && string.IsNullOrWhiteSpace(ISIN)) // 2 filtri su 4
                    {
                        return Ptf.Nome_Gestione.ToLower().Contains(Gestione.ToLower()) && Ptf.Id_tipo_titolo == IdTipoTitolo;
                    }
                    else if (!string.IsNullOrWhiteSpace(Conto) && string.IsNullOrWhiteSpace(Gestione) && IdTipoTitolo == 0 && !string.IsNullOrWhiteSpace(ISIN)) // 2 filtri su 4
                    {
                        return Ptf.Desc_Conto.ToLower().Contains(Conto.ToLower()) && Ptf.Isin.ToLower().Contains(ISIN.ToLower());
                    }
                    else if (string.IsNullOrWhiteSpace(Conto) && !string.IsNullOrWhiteSpace(Gestione) && IdTipoTitolo == 0 && !string.IsNullOrWhiteSpace(ISIN)) // 2 filtri su 4
                    {
                        return Ptf.Nome_Gestione.ToLower().Contains(Gestione.ToLower()) && Ptf.Isin.ToLower().Contains(ISIN.ToLower());
                    }
                    else if (string.IsNullOrWhiteSpace(Conto) && string.IsNullOrWhiteSpace(Gestione) && IdTipoTitolo > 0 && !string.IsNullOrWhiteSpace(ISIN)) // 2 filtri su 4
                    {
                        return Ptf.Id_tipo_titolo == IdTipoTitolo && Ptf.Isin.ToLower().Contains(ISIN.ToLower());
                    }
                    else if (!string.IsNullOrWhiteSpace(Conto) && string.IsNullOrWhiteSpace(Gestione) && IdTipoTitolo == 0 && string.IsNullOrWhiteSpace(ISIN)) // 1 filtri su 4
                    {
                        return Ptf.Desc_Conto.ToLower().Contains(Conto.ToLower());
                    }
                    if (string.IsNullOrWhiteSpace(Conto) && !string.IsNullOrWhiteSpace(Gestione) && IdTipoTitolo == 0 && string.IsNullOrWhiteSpace(ISIN)) // 1 filtri su 4
                    {
                        return Ptf.Nome_Gestione.ToLower().Contains(Gestione.ToLower());
                    }
                    if (string.IsNullOrWhiteSpace(Conto) && string.IsNullOrWhiteSpace(Gestione) && IdTipoTitolo > 0 && string.IsNullOrWhiteSpace(ISIN)) // 1 filtri su 4
                    {
                        return Ptf.Id_tipo_titolo == IdTipoTitolo;
                    }
                    if (string.IsNullOrWhiteSpace(Conto) && string.IsNullOrWhiteSpace(Gestione) && IdTipoTitolo == 0 && !string.IsNullOrWhiteSpace(ISIN)) // 1 filtri su 4
                    {
                        return Ptf.Isin.ToLower().Contains(ISIN.ToLower());
                    }
                }
            }
            return true;
        }

        public void TabControlLoaded(object sender, System.EventArgs e)
        {
            Border MyTabControl = sender as Border;
            MyTabControl.Child = _TabControl;
        }

        #endregion

        #region Filtri per DataGrid
        private string _conto;
        private string Conto
        {
            get { return _conto; }
            set
            {
                _conto = value;
                PtfCollectionView.Filter = _Filter;
                PtfCollectionView.Refresh();
                CollectionCostiMedi.Filter = _Filter;
                CollectionCostiMedi.Refresh();
            }
        }
        private string _gestione;
        private string Gestione
        {
            get { return _gestione; }
            set
            {
                _gestione = value;
                PtfCollectionView.Filter = _Filter;
                PtfCollectionView.Refresh();
                CollectionCostiMedi.Filter = _Filter;
                CollectionCostiMedi.Refresh();
            }
        }
        private string _isin;
        private string ISIN
        {
            get { return _isin; }
            set
            {
                _isin = value;
                PtfCollectionView.Filter = _Filter;
                PtfCollectionView.Refresh();
                CollectionCostiMedi.Filter = _Filter;
                CollectionCostiMedi.Refresh();
            }
        }
        private int _IdTipoTitolo;
        private int IdTipoTitolo
        {
            get { return _IdTipoTitolo; }
            set
            {
                _IdTipoTitolo = value;
                PtfCollectionView.Filter = _Filter;
                PtfCollectionView.Refresh();
                CollectionCostiMedi.Filter = _Filter;
                CollectionCostiMedi.Refresh();
            }
        }
        #endregion

        #region SintesiSoldi
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
        /// il riepilogo dei soldi per la gestione Dany&Fla_Volatili
        /// </summary>
        public SintesiSoldiList SintesiSoldiDFV
        {
            get { return GetValue(() => SintesiSoldiDFV); }
            private set { SetValue(() => SintesiSoldiDFV, value); }
        }

        #endregion

        #region Parametri comuni
        /// <summary>
        /// combo box con la lista dei C/C
        /// </summary>
        public RegistryLocationList ListConti
        {
            get { return GetValue(() => ListConti); }
            set { SetValue(() => ListConti, value); }
        }
        /// <summary>
        /// combo box con la lista delle gestioni
        /// </summary>
        public RegistryOwnersList ListGestioni
        {
            get { return GetValue(() => ListGestioni); }
            set { SetValue(() => ListGestioni, value); }
        }
        /// <summary>
        /// Combo box con i tipi di movimenti
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

        public RegistryShareTypeList ListTipoTitoli
        {
            get { return GetValue(() => ListTipoTitoli); }
            set { SetValue(() => ListTipoTitoli, value); }
        }
        /// <summary>
        /// La ricerca degli isin dei titoli per l'acquisto / vendita
        /// </summary>
        public string SrchShares
        {
            get { return GetValue(() => SrchShares); }
            set
            {
                SetValue(() => SrchShares, value);
                ISIN = value;
                SharesListView.Filter = _Filter;
                SharesListView.Refresh();
            }
        }
        /// <summary>
        /// Combo box con i titoli da selezionare filtrato da SrchShares
        /// </summary>
        public ListCollectionView SharesListView
        {
            get { return GetValue(() => SharesListView); }
            set { SetValue(() => SharesListView, value); }
        }

        #endregion

        #region Totali operazione
        /// <summary>
        /// Sono le azioni possedute di un determinato titolo
        /// </summary>
        public double SharesOwned
        {
            get { return GetValue(() => SharesOwned); }
            private set { SetValue(() => SharesOwned, value); }
        }
        /// <summary>
        /// E' l'importo dato dai titoli per il costo unitario
        /// </summary>
        public double ImportoTotale
        {
            get { return GetValue(() => ImportoTotale); }
            set { SetValue(() => ImportoTotale, value); }
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
        /// E' il totale contabile in valuta locale 
        /// (vengono considerati il disaggio, la RF e la Tobin Tax
        /// </summary>
        public double TotaleContabile
        {
            get { return GetValue<double>(() => TotaleContabile); }
            set { SetValue<double>(() => TotaleContabile, value); }
        }
        /// <summary>
        /// Totale Contabile convertito in euro
        /// </summary>
        public double AmountChangedValue
        {
            get { return GetValue<double>(() => AmountChangedValue); }
            set { SetValue<double>(() => AmountChangedValue, value); }
        }
        #endregion

        #region PrivateFields
        /// <summary>
        /// Elenco con i titoli disponibili
        /// </summary>
        private ObservableCollection<RegistryShare> SharesList
        {
            get { return _SharesList; }
            set
            {
                _SharesList = value;
                SharesListView = new ListCollectionView(value);
            }
        }
        /// <summary>
        /// E' la valuta disponibile per effettuare acquisti
        /// </summary>
        private double CurrencyAvailable
        {
            get { return _CurrencyAvailable; }
            set { _CurrencyAvailable = value; }
        }
        #endregion

        #region DataGrid
        /// <summary>
        /// Elenco con tutti i records del portafoglio
        /// </summary>
        public PortafoglioTitoliList ListPortafoglioTitoli
        {
            get { return GetValue(() => ListPortafoglioTitoli); }
            private set { SetValue(() => ListPortafoglioTitoli, value); PtfCollectionView = CollectionViewSource.GetDefaultView(value); }
        }
        /// <summary>
        /// Contiene la lista di tutti i titoli attivi,
        /// con il n. di titoli e il costo medio di carico comprensivo di tutti i costi sostenuti.
        /// </summary>
        public PortafoglioTitoliList ListCostiMediTitoli
        {
            get { return GetValue(() => ListCostiMediTitoli); }
            private set { SetValue(() => ListCostiMediTitoli, value); CollectionCostiMedi = CollectionViewSource.GetDefaultView(value); }
        }
        public System.ComponentModel.ICollectionView CollectionCostiMedi
        {
            get { return GetValue(() => CollectionCostiMedi); }
            set { SetValue(() => CollectionCostiMedi, value); }
        }

        public System.ComponentModel.ICollectionView PtfCollectionView
        {
            get { return GetValue(() => PtfCollectionView); }
            set { SetValue(() => PtfCollectionView, value); }
        }
        /// <summary>
        /// Singolo record del portafoglio
        /// </summary>
        public PortafoglioTitoli RecordPortafoglioTitoli
        {
            get { return GetValue(() => RecordPortafoglioTitoli); }
            set { SetValue(() => RecordPortafoglioTitoli, value); }
        }
        #endregion

        #region Abilitazioni
        /// <summary>
        /// Gestisce l'abilitazione del campo TobinTax
        /// </summary>
        public bool TobinOk
        {
            get { return GetValue(() => TobinOk); }
            set { SetValue(() => TobinOk, value); }
        }
        /// <summary>
        /// Gestisce l'abilitazione del campo Disaggio
        /// </summary>
        public bool DisaggioOk
        {
            get { return GetValue(() => DisaggioOk); }
            set { SetValue(() => DisaggioOk, value); }
        }
        /// <summary>
        /// Gestisce l'abilitazione del campo Ritenuta Fiscale
        /// </summary>
        public bool RitenutaOk
        {
            get { return GetValue(() => RitenutaOk); }
            set { SetValue(() => RitenutaOk, value); }
        }
        /// <summary>
        /// Gestisce l'abilitazione del campo Ritenuta Fiscale
        /// </summary>
        public bool CanModifyBaseParameters
        {
            get { return GetValue(() => CanModifyBaseParameters); }
            private set { SetValue(() => CanModifyBaseParameters, value); }
        }

        private bool VerificheDati()
        {
            if (!CanUpdateDelete)
            {// verifico la disponibilità di liquidità in conto corrente
                CurrencyAvailable = _liquidAssetServices.GetCurrencyAvailable(RecordPortafoglioTitoli.Id_gestione, RecordPortafoglioTitoli.Id_Conto, RecordPortafoglioTitoli.Id_valuta)[0].Disponibili;
                if (CurrencyAvailable < Math.Abs(TotalLocalValue) && RecordPortafoglioTitoli.Id_tipo_movimento == 5)
                {
                    MessageBox.Show("Non hai abbastanza soldi per questo acquisto!", "Acquisto Vendita Titoli", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return false;
                }
                SharesOwned = _liquidAssetServices.GetSharesQuantity(RecordPortafoglioTitoli.Id_gestione, RecordPortafoglioTitoli.Id_Conto, (uint)RecordPortafoglioTitoli.Id_titolo);
                if ((SharesOwned == 0 || SharesOwned < RecordPortafoglioTitoli.N_titoli * -1) && RecordPortafoglioTitoli.Id_tipo_movimento == 6)
                {
                    MessageBox.Show("C'è un problema sul numero di titoli da vendere!", "Acuisto Vendita Titoli", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            if ((RecordPortafoglioTitoli.Id_tipo_movimento == 5 && RecordPortafoglioTitoli.N_titoli < 0) ||
                    (RecordPortafoglioTitoli.Id_tipo_movimento == 6 && RecordPortafoglioTitoli.N_titoli > 0))
            {
                MessageBox.Show(string.Format("L'operazione {0} non è corretta rispetto ai titoli inseriti", RecordPortafoglioTitoli.Desc_tipo_movimento), "Acquisto Vendita Titoli",
                     MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
        #endregion

        #region command
        private void UpdateDB()
        {
            try
            {
                if (!VerificheDati()) return;
                // estraggo tutti i record di portafoglio coinvolti sulla base di link_movimenti
                PortafoglioTitoliList ptl = _liquidAssetServices.GetManagerLiquidAssetListByLinkMovimenti(RecordPortafoglioTitoli.Link_Movimenti);
                if (ptl.Count == 1)
                {
                    // il record in modifica è l'unico
                    _liquidAssetServices.UpdateManagerLiquidAsset(RecordPortafoglioTitoli);
                    _liquidAssetServices.UpdateRecordContoCorrente(new ContoCorrente(
                        RecordPortafoglioTitoli, TotaleContabile, TipologiaSoldi.Capitale, 
                        _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.Capitale)), TipologiaIDContoCorrente.IdContoTitoli);
                }
                else
                {
                    double valoreAcquisto = 0;      // n. azioni per costo unitario in valuta
                    double numeroAzioni = 0;        // n. azioni
                    PortafoglioTitoli portafoglio;
                    foreach (PortafoglioTitoli pt in ptl)
                    {
                        portafoglio = pt;
                        if (portafoglio.Id_portafoglio == RecordPortafoglioTitoli.Id_portafoglio)
                        {
                            portafoglio = RecordPortafoglioTitoli;
                            _liquidAssetServices.UpdateManagerLiquidAsset(RecordPortafoglioTitoli);
                        }
                        if (portafoglio.Id_tipo_movimento == 5)
                        {
                            valoreAcquisto += portafoglio.Importo_totale + (portafoglio.Commissioni_totale + portafoglio.TobinTax + portafoglio.Disaggio_anticipo_cedole) * -1;
                            numeroAzioni += portafoglio.N_titoli;
                            if (pt.Id_portafoglio == RecordPortafoglioTitoli.Id_portafoglio)
                            {
                                _liquidAssetServices.UpdateRecordContoCorrente(
                                    new ContoCorrente(RecordPortafoglioTitoli, TotaleContabile, TipologiaSoldi.Capitale, 
                                    _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.Capitale)), TipologiaIDContoCorrente.IdContoTitoli);
                            }
                        }
                        else if (portafoglio.Id_tipo_movimento == 6)
                        {
                            double _valoreAcquisto;
                            double _valoreVendita;
                            _valoreAcquisto = valoreAcquisto / numeroAzioni * portafoglio.N_titoli * -1; //valore medio di acquisto delle azioni che si vendono
                            _valoreVendita = portafoglio.Importo_totale + (portafoglio.Commissioni_totale + portafoglio.TobinTax + portafoglio.Disaggio_anticipo_cedole + portafoglio.RitenutaFiscale) * -1;
                            ContoCorrenteList CCs = _liquidAssetServices.GetContoCorrenteByIdPortafoglio(portafoglio.Id_portafoglio);
                            ContoCorrente CCcapitale;
                            ContoCorrente CCprofitloss;
                            if (numeroAzioni + portafoglio.N_titoli != 0)
                            {
                                // ricalcolo valoreAcquisto e numeroAzioni rimanenti
                                valoreAcquisto = valoreAcquisto / numeroAzioni * (numeroAzioni + portafoglio.N_titoli);
                                numeroAzioni = numeroAzioni + portafoglio.N_titoli;
                            }
                            if (CCs.Count != 2) throw new Exception("Ci devono essere 2 record con lo stesso id_portafoglio_titoli! >_< !");
                            if (_valoreAcquisto + _valoreVendita > 0)
                            {
                                CCcapitale = new ContoCorrente(pt, _valoreAcquisto * -1, TipologiaSoldi.Capitale,
                                    _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.Capitale));
                                CCprofitloss = new ContoCorrente(pt, (_valoreAcquisto + _valoreVendita), TipologiaSoldi.Utili_da_Vendite,
                                    _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento,(int)TipologiaSoldi.Utili_da_Vendite));

                                if (CCs[0].Id_Tipo_Soldi == (int)TipologiaSoldi.Capitale)
                                {
                                    CCcapitale.Id_RowConto = CCs[0].Id_RowConto;
                                    CCcapitale.Causale = RecordPortafoglioTitoli.Note;
                                    _liquidAssetServices.UpdateRecordContoCorrente(CCcapitale, TipologiaIDContoCorrente.IdContoCorrente);
                                    CCprofitloss.Id_RowConto = CCs[1].Id_RowConto;
                                    CCprofitloss.Causale = RecordPortafoglioTitoli.Note;
                                    _liquidAssetServices.UpdateRecordContoCorrente(CCprofitloss, TipologiaIDContoCorrente.IdContoCorrente);
                                    _liquidAssetServices.ModifySingoloGuadagno(CCprofitloss);
                                }
                            }
                            else if (_valoreAcquisto + _valoreVendita < 0)
                            {
                                CCcapitale = new ContoCorrente(pt, _valoreVendita, TipologiaSoldi.Capitale,
                                    _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.Capitale));
                                CCprofitloss = new ContoCorrente(pt, (_valoreVendita + _valoreAcquisto) * -1, TipologiaSoldi.PerditaCapitale,
                                    _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.PerditaCapitale));
                                if (CCs[0].Id_Tipo_Soldi == (int)TipologiaSoldi.Capitale)
                                {
                                    CCcapitale.Id_RowConto = CCs[0].Id_RowConto;
                                    CCcapitale.Causale = RecordPortafoglioTitoli.Note;
                                    _liquidAssetServices.UpdateRecordContoCorrente(CCprofitloss, TipologiaIDContoCorrente.IdContoCorrente);
                                    CCprofitloss.Id_RowConto = CCs[1].Id_RowConto;
                                    CCprofitloss.Causale = RecordPortafoglioTitoli.Note;
                                    _liquidAssetServices.UpdateRecordContoCorrente(CCprofitloss, TipologiaIDContoCorrente.IdContoCorrente);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi nel registrare le modifice ai records" + Environment.NewLine + err.Message, Application.Current.FindResource("DAF_Caption").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SaveCommand(object param)
        {
            try
            {
                if (!VerificheDati()) return;
                if (RecordPortafoglioTitoli.Id_tipo_movimento == 5)
                {
                    PortafoglioTitoli MLA = new PortafoglioTitoli();
                    _liquidAssetServices.AddManagerLiquidAsset(RecordPortafoglioTitoli);    // ho inserito il movimento in portafoglio
                    MLA = _liquidAssetServices.GetLastShareMovementByOwnerAndLocation(RecordPortafoglioTitoli.Id_gestione, RecordPortafoglioTitoli.Id_Conto); // ricarico l'ultimo record
                    _liquidAssetServices.InsertAccountMovement(new ContoCorrente(MLA, TotaleContabile, TipologiaSoldi.Capitale,
                        _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.Capitale)));     // ho inserito il movimento in conto corrente
                    // reimposto la griglia con quanto inserito
                    ListPortafoglioTitoli = _liquidAssetServices.GetManagerLiquidAssetListByOwnerAndLocation();
                    CanInsert = false;          // disabilito la possibilità di un inserimento accidentale --> buttare --> GetManagerSharesMovementByOwnerAndLocation
                }
                else if (RecordPortafoglioTitoli.Id_tipo_movimento == 6)
                {
                    // estraggo tutti gli acquisti / vendite del titolo ancora attive
                    Ptf_CCList ptf_CCs = _liquidAssetServices.GetShareActiveAndAccountMovement(RecordPortafoglioTitoli.Id_gestione, RecordPortafoglioTitoli.Id_Conto, (int)RecordPortafoglioTitoli.Id_titolo);
                    double valoreAcquisto = 0;      // n. azioni per costo unitario in valuta
                    double numeroAzioni = 0;        // n. azioni
                    foreach (Ptf_CC row in ptf_CCs)
                    {
                        //se il movimento estratto è un acquisto e per tutti gli acquisti consecutivi ne sommo costo e quote:
                        if (row.Id_tipo_movimento == 5)
                        {
                            //nel caso sia stato comprato in euro e adesso in maschera c'è un valore diverso da euro
                            if (row.Id_valuta == 1 && RecordPortafoglioTitoli.Id_valuta != 1)
                            {
                                valoreAcquisto += (row.ValoreAzione + (row.Commissioni_totale + row.Disaggio_anticipo_cedole) * -1) * row.Valore_di_cambio;
                            }
                            //nel caso sia stato comprato e venduto in una valuta diversa da euro
                            else if (row.Id_valuta == RecordPortafoglioTitoli.Id_valuta && row.Id_valuta != 1)
                            {
                                valoreAcquisto += row.ValoreAzione + (row.Commissioni_totale + row.Disaggio_anticipo_cedole) * -1;
                            }
                            //nel caso sia stato comprato e venduto in euro
                            else
                            {
                                valoreAcquisto += row.ValoreAzione + (row.Commissioni_totale + row.TobinTax + row.Disaggio_anticipo_cedole) * -1;
                            }
                            numeroAzioni += row.N_titoli;
                        }
                        //se il movimento estratto è una vendita e che non sia lo stessa vendita
                        else if (row.Id_tipo_movimento == 6 && row.Id_portafoglio_titoli != RecordPortafoglioTitoli.Id_portafoglio && row.Id_Tipo_Soldi == 1)
                        {
                            //nel caso i precedenti movimenti (acquisti) e questa vendita NON azzerino il totale azioni
                            //vuol dire che sono rimasti dei pezzi invenduti e quindi ne calcolo il costo medio rimanente
                            if (numeroAzioni + row.N_titoli != 0)
                            {
                                valoreAcquisto = valoreAcquisto / numeroAzioni * (numeroAzioni + row.N_titoli);
                                numeroAzioni += row.N_titoli;
                            }
                        }
                    }
                    //ciclo del passato finito calcolo il profit loss nel caso di vendita totale
                    if (numeroAzioni + RecordPortafoglioTitoli.N_titoli == 0)
                    {
                        // disattivo tutti i record coinvolti e uniformo il link_movimenti
                        foreach (Ptf_CC row in ptf_CCs)
                        {
                            if (row.Id_Tipo_Soldi == 1)
                            {
                                PortafoglioTitoli pt = _liquidAssetServices.GetPortafoglioTitoliById(row.Id_portafoglio_titoli);
                                pt.Attivo = 0;
                                pt.Link_Movimenti = RecordPortafoglioTitoli.Link_Movimenti;
                                _liquidAssetServices.UpdateManagerLiquidAsset(pt);
                            }
                        }
                        RecordPortafoglioTitoli.Attivo = 0;
                        _liquidAssetServices.AddManagerLiquidAsset(RecordPortafoglioTitoli);    // ho inserito il movimento in portafoglio titoli
                        RecordPortafoglioTitoli = _liquidAssetServices.GetLastShareMovementByOwnerAndLocation(RecordPortafoglioTitoli.Id_gestione, RecordPortafoglioTitoli.Id_Conto); // ricarico l'ultimo record
                        if (valoreAcquisto + TotaleContabile > 0)
                        {
                            _liquidAssetServices.InsertAccountMovement(new ContoCorrente(RecordPortafoglioTitoli, valoreAcquisto * -1, TipologiaSoldi.Capitale,
                                _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.Capitale)));
                            _liquidAssetServices.InsertAccountMovement(new ContoCorrente(RecordPortafoglioTitoli, valoreAcquisto + TotaleContabile, TipologiaSoldi.Utili_da_Vendite,
                                _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.Utili_da_Vendite)));
                            // inserisco gli utili per quota soci
                            _liquidAssetServices.AddSingoloGuadagno(new ContoCorrente(RecordPortafoglioTitoli, valoreAcquisto + TotaleContabile, TipologiaSoldi.Utili_da_Vendite,
                                _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.Utili_da_Vendite)));

                        }
                        else if (valoreAcquisto + TotaleContabile < 0)
                        {
                            // per inserire le perdite di capitale nella tabella dei guadagni totale anno si deve cercare il periodo quote
                            // con la tipologia soldi degli utili (sempre uguale per la gestione rubiu e no rubiu)
                            _liquidAssetServices.InsertAccountMovement(new ContoCorrente(RecordPortafoglioTitoli, TotaleContabile, TipologiaSoldi.Capitale,
                                _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.Capitale)));
                            _liquidAssetServices.InsertAccountMovement(new ContoCorrente(RecordPortafoglioTitoli, (TotaleContabile + valoreAcquisto) * -1, TipologiaSoldi.PerditaCapitale,
                                _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.Utili_da_Vendite)));
                            _liquidAssetServices.AddSingoloGuadagno(new ContoCorrente(RecordPortafoglioTitoli, valoreAcquisto + TotaleContabile, TipologiaSoldi.PerditaCapitale,
                                _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.Utili_da_Vendite)));
                        }
                    }
                    else //e nel caso di vendita parziale
                    {
                        // uniformo il link_movimenti
                        foreach (Ptf_CC row in ptf_CCs)
                        {
                            if (row.Id_Tipo_Soldi == 1)
                            {
                                PortafoglioTitoli pt = _liquidAssetServices.GetPortafoglioTitoliById(row.Id_portafoglio_titoli);
                                pt.Link_Movimenti = RecordPortafoglioTitoli.Link_Movimenti;
                                _liquidAssetServices.UpdateManagerLiquidAsset(pt);
                            }
                        }
                        RecordPortafoglioTitoli.ProfitLoss = valoreAcquisto / numeroAzioni * RecordPortafoglioTitoli.N_titoli * -1 +
                            (RecordPortafoglioTitoli.Importo_totale + (RecordPortafoglioTitoli.Commissioni_totale + RecordPortafoglioTitoli.TobinTax +
                            RecordPortafoglioTitoli.Disaggio_anticipo_cedole + RecordPortafoglioTitoli.RitenutaFiscale) * -1);

                        _liquidAssetServices.AddManagerLiquidAsset(RecordPortafoglioTitoli);
                        RecordPortafoglioTitoli = _liquidAssetServices.GetLastShareMovementByOwnerAndLocation(RecordPortafoglioTitoli.Id_gestione, RecordPortafoglioTitoli.Id_Conto);
                        valoreAcquisto = valoreAcquisto / numeroAzioni * RecordPortafoglioTitoli.N_titoli * -1;
                        if (valoreAcquisto + TotaleContabile > 0)
                        {
                            _liquidAssetServices.InsertAccountMovement(new ContoCorrente(RecordPortafoglioTitoli, valoreAcquisto * -1, TipologiaSoldi.Capitale,
                                _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.Capitale)));
                            _liquidAssetServices.InsertAccountMovement(new ContoCorrente(RecordPortafoglioTitoli, valoreAcquisto + TotaleContabile, TipologiaSoldi.Utili_da_Vendite,
                                _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.Utili_da_Vendite)));
                            // Inserisco il guadagno ripartito per i soci
                            _liquidAssetServices.AddSingoloGuadagno(new ContoCorrente(RecordPortafoglioTitoli, valoreAcquisto + TotaleContabile, TipologiaSoldi.Utili_da_Vendite,
                                _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.Utili_da_Vendite)));
                        }
                        else if (valoreAcquisto + TotaleContabile < 0)
                        {
                            // per inserire le perdite di capitale nella tabella dei guadagni totale anno si deve cercare il periodo quote
                            // con la tipologia soldi degli utili (sempre uguale per la gestione rubiu e no rubiu)
                            _liquidAssetServices.InsertAccountMovement(new ContoCorrente(RecordPortafoglioTitoli, TotaleContabile, TipologiaSoldi.Capitale,
                                _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.Capitale)));
                            _liquidAssetServices.InsertAccountMovement(new ContoCorrente(RecordPortafoglioTitoli, (TotaleContabile + valoreAcquisto) * -1, TipologiaSoldi.PerditaCapitale,
                                _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.Utili_da_Vendite)));
                            _liquidAssetServices.AddSingoloGuadagno(new ContoCorrente(RecordPortafoglioTitoli, valoreAcquisto + TotaleContabile, TipologiaSoldi.PerditaCapitale,
                                _liquidAssetServices.GetIdPeriodoQuote(RecordPortafoglioTitoli.Data_Movimento, (int)TipologiaSoldi.Utili_da_Vendite)));
                        }
                    }
                }
                Init();
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
            UpdateDB();
            MessageBox.Show("Record modificato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
            Init();
        }
        public void DeleteCommand(object param)
        {
            try
            {
                // estraggo tutti i record di portafoglio coinvolti sulla base di link_movimenti
                PortafoglioTitoliList ptl = _liquidAssetServices.GetManagerLiquidAssetListByLinkMovimenti(RecordPortafoglioTitoli.Link_Movimenti);
                foreach (PortafoglioTitoli pt in ptl)
                {
                    pt.Attivo = 1;
                    _liquidAssetServices.UpdateManagerLiquidAsset(pt);
                }
                ContoCorrenteList CCL = _liquidAssetServices.GetContoCorrenteByIdPortafoglio(RecordPortafoglioTitoli.Id_portafoglio);
                foreach (ContoCorrente cc in CCL)
                    if (cc.Id_Tipo_Soldi == (int)TipologiaSoldi.Utili_da_Vendite)
                        _liquidAssetServices.DeleteRecordGuadagno_Totale_anno(cc.Id_RowConto);                          // Registro l'eliminazione in guadagni totale anno
                _liquidAssetServices.DeleteContoCorrenteByIdPortafoglioTitoli(RecordPortafoglioTitoli.Id_portafoglio);  // registro l'eliminazione in conto corrente
                _liquidAssetServices.DeleteManagerLiquidAsset(RecordPortafoglioTitoli.Id_portafoglio);                  // registro l'eliminazione dal portafoglio
                UpdateDB();
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
            AcquistoVenditaTitoliView MFMV = param as AcquistoVenditaTitoliView;
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
            set
            {
                SetValue(() => CanUpdateDelete, value);
                CanModifyBaseParameters = !value;
            }
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

using FinanceManager.Events;
using FinanceManager.Models;
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
        public ICommand CloseMeCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand EraseCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        Predicate<object> _Filter;

        public AcquistoVenditaTitoliViewModel
            (IRegistryServices services, IManagerLiquidAssetServices liquidAssetServices)
        {
            _registryServices = services ?? throw new ArgumentNullException("Services in Manager Portfolio Movement View Model");
            _liquidAssetServices = liquidAssetServices ?? throw new ArgumentNullException("Liquid Asset Services in Manager Portfolio Movement View Model");
            CloseMeCommand = new CommandHandler(CloseMe);
            SetUpData();
            Init();
            InsertCommand = new CommandHandler(SaveCommand, CanSave);
            ModifyCommand = new CommandHandler(UpdateCommand, CanModify);
            EraseCommand = new CommandHandler(DeleteCommand, CanModify);
            ClearCommand = new CommandHandler(CleanCommand);
            SintesiSoldiR = _liquidAssetServices.GetCurrencyAvailable(1);
            SintesiSoldiDF = _liquidAssetServices.GetCurrencyAvailable(2);
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

                SharesList = new ObservableCollection<RegistryShare>(_registryServices.GetRegistryShareList());
                _Filter = new Predicate<object>(Filter);
                RecordPortafoglioTitoli = new PortafoglioTitoli();
                ListPortafoglioTitoli = new PortafoglioTitoliList();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Set up Acquisto Vendita Titoli", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Init()
        {
            TobinOk = false;
            DisaggioOk = false;
            RitenutaOk = false;
            ImportoTotale = 0;
            TotalLocalValue = 0;
            TotaleContabile = 0;
            AmountChangedValue = 0;
            SrchShares = "";
            try
            {
                ListSelectedPortafoglioTitoli = new PortafoglioTitoliList();
                RecordPortafoglioTitoli = new PortafoglioTitoli();
                ListPortafoglioTitoli = _liquidAssetServices.GetManagerLiquidAssetListByOwnerAndLocation();
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "Init Acquisto Vendita Titoli", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region events
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
                    RecordPortafoglioTitoli.Id_conto = RL.Id_conto;
                    RecordPortafoglioTitoli.Desc_conto = RL.Desc_conto;
                }
                if (e.AddedItems[0] is RegistryOwner RO)
                {
                    RecordPortafoglioTitoli.Id_gestione = RO.Id_gestione;
                    RecordPortafoglioTitoli.Nome_Gestione = RO.Nome_Gestione;
                }
                if (e.AddedItems[0] is RegistryCurrency RC)
                {
                    RecordPortafoglioTitoli.Id_valuta = RC.IdCurrency;
                    if (RC.IdCurrency > 0)
                        CurrencyAvailable = _liquidAssetServices.GetCurrencyAvailable(RecordPortafoglioTitoli.Id_gestione, RecordPortafoglioTitoli.Id_conto, RC.IdCurrency)[0].Disponibili;
                    if (CurrencyAvailable == 0 && RecordPortafoglioTitoli.Id_tipo_movimento == 5)
                    {
                        MessageBox.Show("Non hai soldi in questa valuta!", "Acquisto Vendita Titoli", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        Init();
                        return;
                    }
                    if (RC.IdCurrency == 1)
                    {
                        RecordPortafoglioTitoli.Valore_di_cambio = 1;
                    }
                }
                if (e.AddedItems[0] is RegistryShare RS)
                {
                    RecordPortafoglioTitoli.Id_titolo = RS.IdShare;
                    RecordPortafoglioTitoli.Id_tipo_titolo = RS.IdShareType;
                    RecordPortafoglioTitoli.Isin = RS.Isin;
                    RecordPortafoglioTitoli.Id_azienda = RS.IdFirm;
                    SharesOwned = _liquidAssetServices.GetSharesQuantity(RecordPortafoglioTitoli.Id_gestione, RecordPortafoglioTitoli.Id_conto, (uint)RecordPortafoglioTitoli.Id_titolo);
                    if (SharesOwned == 0 && RecordPortafoglioTitoli.Id_tipo_movimento == 6)
                    {
                        MessageBox.Show("Non hai titoli da vendere con queste impostazioni!", "Acuisto Vendita Titoli", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        RecordPortafoglioTitoli.Id_titolo = 0;
                        RS.IdShare = 0;
                        return;
                    }
                }
                if (e.AddedItems[0] is DateTime DT)
                {
                    RecordPortafoglioTitoli.Data_Movimento = DT.Date;
                }
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
                ListSelectedPortafoglioTitoli = _liquidAssetServices.GetManagerLiquidAssetListByOwnerLocationAndTitolo(PT.Id_gestione, PT.Id_conto, (int)PT.Id_titolo);
                UpdateTotals();
                CanUpdateDelete = true;
                CanInsert = false;
            }
        }

        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (e.Key == Key.Decimal)
            {
                textBox.AppendText(",");
                textBox.SelectionStart = textBox.Text.Length;
                e.Handled = true;
            }
        }


        public void LostFocus(object sender, EventArgs e)
        {
            TextBox TB = sender as TextBox;
            double converted;
            if (!string.IsNullOrEmpty(TB.Text) && double.TryParse(TB.Text, out converted))
            {
                switch (TB.Name)
                {
                    case "unityLocalAmount":
                        RecordPortafoglioTitoli.Costo_unitario_in_valuta = Convert.ToDouble(TB.Text);
                        break;
                    case "NShares":
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
                    case "CommissionValue":
                        RecordPortafoglioTitoli.Commissioni_totale = Convert.ToDouble(TB.Text);
                        break;
                    case "Valore_di_cambio":
                        RecordPortafoglioTitoli.Valore_di_cambio = Convert.ToDouble(TB.Text);
                        break;
                    case "TobinTaxValue":
                        RecordPortafoglioTitoli.TobinTax = Convert.ToDouble(TB.Text);
                        break;
                    case "RitenutaFiscale":
                        RecordPortafoglioTitoli.RitenutaFiscale = Convert.ToDouble(TB.Text);
                        break;
                    case "DisaggioValue":
                        RecordPortafoglioTitoli.Disaggio_anticipo_cedole = Convert.ToDouble(TB.Text);
                        break;
                    case "Note":
                        RecordPortafoglioTitoli.Note = TB.Text;
                        break;
                }
            }
            UpdateTotals();
        }

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
                    if ((RecordPortafoglioTitoli.Id_tipo_titolo == 1 || RecordPortafoglioTitoli.Id_tipo_titolo == 4) && RecordPortafoglioTitoli.Id_valuta == 1)
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

        public double SharesOwned
        {
            get { return GetValue(() => SharesOwned); }
            set { SetValue(() => SharesOwned, value); }
        }
        /// <summary>
        /// Elenco con tutti i records del portafoglio
        /// </summary>
        public PortafoglioTitoliList ListPortafoglioTitoli
        {
            get { return GetValue(() => ListPortafoglioTitoli); }
            set { SetValue(() => ListPortafoglioTitoli, value); }
        }
        /// <summary>
        /// Elenco con tutti i records selezionati dall'elenco generale
        /// </summary>
        public PortafoglioTitoliList ListSelectedPortafoglioTitoli
        {
            get { return GetValue(() => ListSelectedPortafoglioTitoli); }
            set { SetValue(() => ListSelectedPortafoglioTitoli, value); }
        }
        /// <summary>
        /// Singolo record del portafoglio
        /// </summary>
        public PortafoglioTitoli RecordPortafoglioTitoli
        {
            get { return GetValue(() => RecordPortafoglioTitoli); }
            set { SetValue(() => RecordPortafoglioTitoli, value); }
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
                PortafoglioTitoli MLA = new PortafoglioTitoli();
                MLA.Id_gestione = RecordPortafoglioTitoli.Id_gestione;
                MLA.Id_conto = RecordPortafoglioTitoli.Id_conto;
                MLA.Id_valuta = RecordPortafoglioTitoli.Id_valuta;
                MLA.Id_tipo_movimento = RecordPortafoglioTitoli.Id_tipo_movimento;
                MLA.Id_titolo = RecordPortafoglioTitoli.Id_titolo;
                MLA.Data_Movimento = RecordPortafoglioTitoli.Data_Movimento;
                MLA.Importo_totale = RecordPortafoglioTitoli.Importo_totale;
                MLA.N_titoli = RecordPortafoglioTitoli.N_titoli;
                MLA.Costo_unitario_in_valuta = RecordPortafoglioTitoli.Costo_unitario_in_valuta;
                MLA.Commissioni_totale = RecordPortafoglioTitoli.Commissioni_totale;
                MLA.TobinTax = RecordPortafoglioTitoli.TobinTax;
                MLA.Disaggio_anticipo_cedole = RecordPortafoglioTitoli.Disaggio_anticipo_cedole;
                MLA.RitenutaFiscale = RecordPortafoglioTitoli.RitenutaFiscale;
                MLA.Valore_di_cambio = RecordPortafoglioTitoli.Valore_di_cambio;
                MLA.Note = RecordPortafoglioTitoli.Note;

                _liquidAssetServices.AddManagerLiquidAsset(MLA);    // ho inserito il movimento in portafoglio
                MLA = _liquidAssetServices.GetLastShareMovementByOwnerAndLocation(RecordPortafoglioTitoli.Id_gestione, RecordPortafoglioTitoli.Id_conto); // ricarico l'ultimo record
                _liquidAssetServices.InsertAccountMovement(ContoCorrente(MLA));     // ho inserito il movimento in conto corrente
                // reimposto la griglia con quanto inserito
                ListPortafoglioTitoli = _liquidAssetServices.GetManagerSharesMovementByOwnerAndLocation(RecordPortafoglioTitoli.Id_gestione, RecordPortafoglioTitoli.Id_conto);
                CanInsert = false;          // disabilito la possibilità di un inserimento accidentale
                CanUpdateDelete = true;     // abilito la possibilità di modificare / cancellare il record

                RecordPortafoglioTitoli = MLA;
                SharesOwned = _liquidAssetServices.GetSharesQuantity(RecordPortafoglioTitoli.Id_gestione, RecordPortafoglioTitoli.Id_conto, (uint)RecordPortafoglioTitoli.Id_titolo);
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
        private ContoCorrente ContoCorrente (PortafoglioTitoli portafoglioTitoli)
        {
            ContoCorrente cc = new ContoCorrente()
            {
                Id_Conto = portafoglioTitoli.Id_conto,
                Id_Quote_Investimenti = 0,
                Id_Valuta = portafoglioTitoli.Id_valuta,
                Id_Portafoglio_Titoli = portafoglioTitoli.Id_portafoglio,
                Id_tipo_movimento = portafoglioTitoli.Id_tipo_movimento,
                Id_Gestione = portafoglioTitoli.Id_gestione,
                Id_Titolo = (int)portafoglioTitoli.Id_titolo,
                DataMovimento = portafoglioTitoli.Data_Movimento,
                Ammontare = TotaleContabile,
                Valore_Cambio = portafoglioTitoli.Valore_di_cambio,
                Causale = portafoglioTitoli.Note,
                Id_Tipo_Soldi = 1
            };
            return cc;
        }
        public void UpdateCommand(object param)
        {
            try
            {
                _liquidAssetServices.UpdateManagerLiquidAsset(RecordPortafoglioTitoli);                                   //registro la modifica in portafoglio
                _liquidAssetServices.UpdateContoCorrenteByIdPortafoglioTitoli(ContoCorrente(RecordPortafoglioTitoli));    //registro la modifica in conto corrente

                // aggiorno la disponibilità
                SintesiSoldiR = _liquidAssetServices.GetCurrencyAvailable(1);
                SintesiSoldiDF = _liquidAssetServices.GetCurrencyAvailable(2);

                // reimposto la griglia con quanto inserito
                ListPortafoglioTitoli = _liquidAssetServices.GetManagerSharesMovementByOwnerAndLocation(RecordPortafoglioTitoli.Id_gestione, RecordPortafoglioTitoli.Id_conto);
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
                _liquidAssetServices.DeleteContoCorrenteByIdPortafoglioTitoli(RecordPortafoglioTitoli.Id_portafoglio);  // registro l'eliminazione in conto corrente
                _liquidAssetServices.DeleteManagerLiquidAsset(RecordPortafoglioTitoli.Id_portafoglio);                  // registro l'eliminazione dal portafoglio
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

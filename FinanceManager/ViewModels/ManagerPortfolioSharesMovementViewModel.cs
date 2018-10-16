using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class ManagerPortfolioSharesMovementViewModel : ViewModelBase
    {
        private IRegistryServices _services;
        private IManagerLiquidAssetServices _liquidAssetServices;
        public ICommand CloseMeCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand EraseCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        private int[] enabledMovement = { 5, 6 };
        Predicate<object> _Filter;

        public ManagerPortfolioSharesMovementViewModel(IRegistryServices services, IManagerLiquidAssetServices liquidAssetServices)
        {
            _services = services ?? throw new ArgumentNullException("Services in Manager Portfolio Movement View Model");
            _liquidAssetServices = liquidAssetServices ?? throw new ArgumentNullException("Liquid Asset Services in Manager Portfolio Movement View Model");
            SetUpViewModel();
            CloseMeCommand = new CommandHandler(CloseMe);
            InsertCommand = new CommandHandler(SaveCommand, CanSave);
            ModifyCommand = new CommandHandler(UpdateCommand, CanModify);
            EraseCommand = new CommandHandler(DeleteCommand, CanModify);
            ClearCommand = new CommandHandler(CleanCommand);
        }

        private void SetUpViewModel()
        {
            try
            {
                OwnerList = new ObservableCollection<RegistryOwner>(_services.GetRegistryOwners());
                MovementList = new ObservableCollection<RegistryMovementType>(_services.GetRegistryMovementTypesList());
                LocationList = new ObservableCollection<RegistryLocation>(_services.GetRegistryLocationList());
                CurrencyList = new ObservableCollection<RegistryCurrency>(_services.GetRegistryCurrencyList());
                SharesList = new ObservableCollection<RegistryShare>(_services.GetRegistryShareList());
                RowLiquidAsset = new ManagerLiquidAsset();
                RowLiquidAsset.Data_Movimento = DateTime.Now;
                CanUpdateDelete = false;
                CanInsert = false;
                _Filter = new Predicate<object>(Filter);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "ManagerPortfolioSharesView", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region events
        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ComboBox CB = sender as ComboBox;
                RegistryOwner RO = e.AddedItems[0] as RegistryOwner;
                RegistryLocation RL = e.AddedItems[0] as RegistryLocation;
                RegistryMovementType RMT = e.AddedItems[0] as RegistryMovementType;
                RegistryCurrency RC = e.AddedItems[0] as RegistryCurrency;
                RegistryShare RS = e.AddedItems[0] as RegistryShare;
                DateTime DT = DateTime.Now;
                ComboBoxItem CBI = e.AddedItems[0] as ComboBoxItem;
                ManagerLiquidAsset MLA = e.AddedItems[0] as ManagerLiquidAsset;

                bool dTime = false;
                try
                {
                    DT = (DateTime)e.AddedItems[0];
                    dTime = true;
                }
                catch { }

                if (RO != null)
                {
                    SelectedOwner = RO.OwnerName;
                    RowLiquidAsset = new ManagerLiquidAsset();
                    AmountChangedValue = 0;
                    RowLiquidAsset.Data_Movimento = DT.Date;
                    RowLiquidAsset.Id_gestione = RO.IdOwner;
                    RowLiquidAsset.Nome_Gestione = RO.OwnerName;
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbLocation", true);

                }
                if (RL != null)
                {
                    RowLiquidAsset.Id_conto = RL.IdLocation;
                    RowLiquidAsset.Desc_conto = RL.DescLocation;
                    LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerSharesMovementByOwnerAndLocation(RowLiquidAsset.Id_gestione, RL.IdLocation));
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbMovement", true);
                }
                if (RMT != null)
                {
                    RowLiquidAsset.Id_tipo_movimento = RMT.IdMovement;
                    RowLiquidAsset.Desc_tipo_movimento = RMT.DescMovement;
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbCurrency", true);
                }
                if (RC != null)
                {
                    RowLiquidAsset.Id_valuta = RC.IdCurrency;
                    RowLiquidAsset.Cod_valuta = RC.CodeCurrency;
                    SetAvailableLiquidity(_liquidAssetServices.GetCurrencyAvailable(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto, RowLiquidAsset.Id_valuta));
                    SetProfitLoss(_liquidAssetServices.GetProfitLossByCurrency(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto, RowLiquidAsset.Id_valuta));
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbShares", true);
                }
                if (RS != null)
                {
                    RowLiquidAsset.Id_titolo = RS.IdShare;
                    RowLiquidAsset.Desc_titolo = RS.DescShare;
                    RowLiquidAsset.Isin = RS.Isin;
                    RowLiquidAsset.Id_tipo_titolo = RS.IdShareType;
                    SharesOwned = _liquidAssetServices.GetSharesQuantity(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto, (uint)RowLiquidAsset.Id_titolo).ToString();

                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "unityLocalAmount", true);
                }
                if (dTime)
                    RowLiquidAsset.Data_Movimento = DT.Date;
                if (CBI != null)
                    RowLiquidAsset.Available = Convert.ToBoolean(CBI.Content);
                if (MLA != null)
                {
                    RowLiquidAsset = MLA;
                    Grid FormGrid = ((Grid)((DataGrid)sender).Parent).Parent as Grid;
                    EnableControl.EnableControlInGrid(FormGrid, "NShares", true);
                    EnableControl.EnableControlInGrid(FormGrid, "CommissionValue", true);
                    EnableControl.EnableControlInGrid(FormGrid, "Valore_di_cambio", true);
                    UpdateTotals();
                    if (RowLiquidAsset.Id_tipo_movimento == 5 && !(RowLiquidAsset.Id_tipo_titolo == 2 || RowLiquidAsset.Id_tipo_titolo == 16 ||
                        RowLiquidAsset.Id_tipo_titolo == 9 || RowLiquidAsset.Id_tipo_titolo == 14 || RowLiquidAsset.Id_tipo_titolo == 18))
                    {
                        EnableControl.EnableControlInGrid(FormGrid, "TobinTaxValue", true);

                    }
                    else if (RowLiquidAsset.Id_tipo_titolo == 2 || RowLiquidAsset.Id_tipo_titolo == 16 ||
                        RowLiquidAsset.Id_tipo_titolo == 9 || RowLiquidAsset.Id_tipo_titolo == 14 || RowLiquidAsset.Id_tipo_titolo == 18)
                    {
                        EnableControl.EnableControlInGrid(FormGrid, "DisaggioValue", true);
                    }
                    CanUpdateDelete = true;
                }
            }
        }

        public void LostFocus(object sender, EventArgs e)
        {
            TextBox TB = sender as TextBox;
            double converted;
            try
            {
                if (!string.IsNullOrEmpty(TB.Text) && double.TryParse(TB.Text, out converted))
                {
                    switch (TB.Name)
                    {
                        case "unityLocalAmount":
                            RowLiquidAsset.Costo_unitario_in_valuta = Convert.ToDouble(TB.Text);
                            if (RowLiquidAsset.Costo_unitario_in_valuta > 0)
                            {
                                EnableControl.EnableControlInGrid(TB.Parent as Grid, "NShares", true);
                            }
                            break;
                        case "NShares":
                            if (RowLiquidAsset.Id_tipo_movimento == 5 && Convert.ToDouble(TB.Text) > 0)
                            {
                                EnableControl.EnableControlInGrid(TB.Parent as Grid, "CommissionValue", true);
                                RowLiquidAsset.N_titoli = Convert.ToDouble(TB.Text);
                            }
                            else if (RowLiquidAsset.Id_tipo_movimento == 6 && Convert.ToDouble(TB.Text) < 0)
                            {
                                EnableControl.EnableControlInGrid(TB.Parent as Grid, "CommissionValue", true);
                                RowLiquidAsset.N_titoli = Convert.ToDouble(TB.Text);
                            }
                            else
                            {
                                MessageBox.Show("Inserire un importo positivo se si compra o negativo se si vende.", "DAF-C registrazione movimenti titoli", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            break;
                        case "CommissionValue":
                            RowLiquidAsset.Commissioni_totale = Convert.ToDouble(TB.Text);
                            EnableControl.EnableControlInGrid(TB.Parent as Grid, "Valore_di_cambio", true);
                            break;
                        case "Valore_di_cambio":
                            RowLiquidAsset.Valore_di_cambio = Convert.ToDouble(TB.Text);
                            if (RowLiquidAsset.Id_tipo_movimento == 5 && !(RowLiquidAsset.Id_tipo_titolo == 2 || RowLiquidAsset.Id_tipo_titolo == 16 ||
                                RowLiquidAsset.Id_tipo_titolo == 9 || RowLiquidAsset.Id_tipo_titolo == 14 || RowLiquidAsset.Id_tipo_titolo == 18))
                                EnableControl.EnableControlInGrid(TB.Parent as Grid, "TobinTaxValue", true);
                            else if (RowLiquidAsset.Id_tipo_movimento == 5 && (RowLiquidAsset.Id_tipo_titolo == 2 || RowLiquidAsset.Id_tipo_titolo == 16 ||
                                RowLiquidAsset.Id_tipo_titolo == 9 || RowLiquidAsset.Id_tipo_titolo == 14 || RowLiquidAsset.Id_tipo_titolo == 18))
                                EnableControl.EnableControlInGrid(TB.Parent as Grid, "DisaggioValue", true);
                            else if (RowLiquidAsset.Id_tipo_movimento == 6)
                                EnableControl.EnableControlInGrid(TB.Parent as Grid, "RitenutaFiscale", true);
                            if (RowLiquidAsset.Id_portafoglio == 0)
                                CanInsert = true;
                            break;
                        case "TobinTaxValue":
                            RowLiquidAsset.TobinTax = Convert.ToDouble(TB.Text);
                            if (RowLiquidAsset.Id_portafoglio == 0)
                                CanInsert = true;
                            break;
                        case "RitenutaFiscale":
                            RowLiquidAsset.RitenutaFiscale = Convert.ToDouble(TB.Text);
                            break;
                        case "DisaggioValue":
                            if (RowLiquidAsset.Id_portafoglio == 0)
                                CanInsert = true;
                            break;
                        case "Note":
                            RowLiquidAsset.Note = TB.Text;
                            break;
                    }
                    UpdateTotals();
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "Movimenti Titoli", MessageBoxButton.OK, MessageBoxImage.Error);
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
        #endregion events

        private void UpdateTotals()
        {
            // verifico che ci sia un importo unitario e un numero di azioni
            if (RowLiquidAsset.Costo_unitario_in_valuta >= 0 && RowLiquidAsset.N_titoli != 0)
            {
                // Totale 1 VALORE TRANSAZIONE (conteggio diverso fra obbligazioni e tutto il resto)
                RowLiquidAsset.Importo_totale = RowLiquidAsset.Id_tipo_titolo != 2 ? -1 * (RowLiquidAsset.Costo_unitario_in_valuta * RowLiquidAsset.N_titoli) :
                    -1 * (RowLiquidAsset.Costo_unitario_in_valuta * RowLiquidAsset.N_titoli) / 100;

                if (RowLiquidAsset.Id_tipo_movimento == 5) //acquisto
                {
                    // totale 2 valore transazione più commissioni (negativo in caso di acquisto)
                    TotalLocalValue = RowLiquidAsset.Importo_totale + RowLiquidAsset.Commissioni_totale * -1;
                    // totale contabile in valuta
                    TotaleContabile = RowLiquidAsset.Id_valuta == 1 ?
                        TotalLocalValue + (RowLiquidAsset.TobinTax + RowLiquidAsset.Disaggio_anticipo_cedole + RowLiquidAsset.RitenutaFiscale) * -1 :
                        TotalLocalValue + (RowLiquidAsset.Disaggio_anticipo_cedole + (RowLiquidAsset.RitenutaFiscale * RowLiquidAsset.Valore_di_cambio)) * -1;

                }
                else if (RowLiquidAsset.Id_tipo_movimento == 6) //vendita
                {
                    // totale 2 valore transazione più commissioni (positivo in caso di vendita)
                    TotalLocalValue = RowLiquidAsset.Importo_totale - RowLiquidAsset.Commissioni_totale;
                    // totale contabile in valuta
                    TotaleContabile = RowLiquidAsset.Id_valuta == 1 ?
                        TotalLocalValue - (RowLiquidAsset.TobinTax + RowLiquidAsset.Disaggio_anticipo_cedole + RowLiquidAsset.RitenutaFiscale) :
                        TotalLocalValue - (RowLiquidAsset.Disaggio_anticipo_cedole + (RowLiquidAsset.RitenutaFiscale * RowLiquidAsset.Valore_di_cambio));
                }
            }
            AmountChangedValue = TotaleContabile;
            // totale contabile in euro calcolato solo se è inserito un valore di cambio
            if (RowLiquidAsset.Id_valuta != 1)
                AmountChangedValue = RowLiquidAsset.Valore_di_cambio == 0 ? 0 : (TotaleContabile / RowLiquidAsset.Valore_di_cambio);
        }
        private void ProfitLossCalculation()
        {
            ManagerLiquidAssetList MLAL = _liquidAssetServices.GetShareMovements(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto, (uint)RowLiquidAsset.Id_titolo);
            double PrezzoAcq = 0;
            double NAcq = 0;
            foreach (ManagerLiquidAsset MLA in MLAL)
            {
                //interrompo il ciclo se i movimenti estratti sono in data > di quella nella maschera
                if (MLA.Data_Movimento > RowLiquidAsset.Data_Movimento) break;
                //se il movimento estratto è un acquisto e per tutti gli acquisti consecutivi ne sommo costo e quote:
                if (MLA.Id_tipo_movimento == 5)
                {
                    //nel caso sia stato comprato in euro e adesso in maschera c'è un valore diverso da euro
                    if (MLA.Cod_valuta == "EUR" && RowLiquidAsset.Cod_valuta != "EUR")
                    {
                        PrezzoAcq += (MLA.Importo_totale + (MLA.Commissioni_totale + MLA.Disaggio_anticipo_cedole) * -1) * MLA.Valore_di_cambio;
                    }
                    //nel caso sia stato comprato e venduto in una valuta diversa da euro
                    else if (MLA.Cod_valuta == RowLiquidAsset.Cod_valuta && MLA.Cod_valuta != "EUR")
                    {
                        PrezzoAcq += MLA.Importo_totale + (MLA.Commissioni_totale + MLA.Disaggio_anticipo_cedole) * -1;
                    }
                    //nel caso sia stato comprato e venduto in euro
                    else
                    {
                        PrezzoAcq += MLA.Importo_totale + (MLA.Commissioni_totale + MLA.TobinTax + MLA.Disaggio_anticipo_cedole) * -1;
                    }
                    NAcq += MLA.N_titoli;
                }
                //se il movimento estratto è una vendita e che non sia lo stessa vendita
                else if (MLA.Id_tipo_movimento == 6 && MLA.Id_portafoglio != RowLiquidAsset.Id_portafoglio)
                {
                    //nel caso i precedenti movimenti (acquisti) e questa vendita azzerino il totale azioni
                    //vuol dire che l'operazione è stata conclusa nel passato e azzero i contatori
                    if (NAcq + MLA.N_titoli == 0)
                    {
                        PrezzoAcq = 0;
                        NAcq = 0;
                    }
                    //nel caso i precedenti movimenti (acquist) e questa vendita NON azzerino il totale azioni
                    //vuol dire che sono rimasti dei pezzi invenduti e quindi ne calcolo il costo medio rimanente
                    else if (NAcq + MLA.N_titoli != 0)
                    {
                        PrezzoAcq = PrezzoAcq / NAcq * (NAcq + MLA.N_titoli);
                        NAcq = NAcq + MLA.N_titoli;
                    }
                }
            }
            //ciclo del passato finito calcolo il profit loss nel caso di vendita totale
            if (NAcq + RowLiquidAsset.N_titoli == 0)
            {
                RowLiquidAsset.ProfitLoss = PrezzoAcq + TotaleContabile;
            }
            else //e nel caso di vendita parziale
            {
                RowLiquidAsset.ProfitLoss = PrezzoAcq / NAcq * RowLiquidAsset.N_titoli * -1 +
                    (RowLiquidAsset.Importo_totale + (RowLiquidAsset.Commissioni_totale + RowLiquidAsset.TobinTax + RowLiquidAsset.Disaggio_anticipo_cedole + RowLiquidAsset.RitenutaFiscale) * -1);
            }
            PrezzoAcq = 0;
            NAcq = 0;
            //fine calcolo profit loss
        }

        /// <summary>
        /// Il profit loss calcolato alla vendita di un titolo
        /// </summary>
        public string ProfitLoss
        {
            get { return GetValue<string>(() => ProfitLoss); }
            set { SetValue<string>(() => ProfitLoss, value); }
        }

        private void SetProfitLoss(double PL)
        {
            ProfitLoss = string.Format("Il tuo profit loss in {0} è di: {1}", RowLiquidAsset.Cod_valuta, PL.ToString("#,##0.0#", CultureInfo.CreateSpecificCulture("it-IT")));
        }

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

        public string SelectedOwner
        {
            get { return GetValue<string>(() => SelectedOwner); }
            set
            {
                if (value == "")
                    SetValue<string>(() => SelectedOwner, "");
                else
                    SetValue<string>(() => SelectedOwner, "Elenco Movimenti di: " + value);
            }
        }

        #region Models

        public string SharesOwned
        {
            get { return GetValue(() => SharesOwned); }
            set
            {
                string txt = string.Format("Totale titoli {0} posseduti: {1}.", RowLiquidAsset.Desc_titolo, value);
                if (RowLiquidAsset.Desc_titolo == null)
                    txt = "";
                SetValue(() => SharesOwned, txt);
            }
        }
        /// <summary>
        /// E' il totale comprensivo delle commissioni
        /// </summary>
        public double TotalLocalValue
        {
            get { return GetValue<double>("TotalLocalValue"); }
            set { SetValue("TotalLocalValue", value); }
        }

        public ManagerLiquidAsset RowLiquidAsset
        {
            get { return GetValue<ManagerLiquidAsset>(() => RowLiquidAsset); }
            set { SetValue<ManagerLiquidAsset>(() => RowLiquidAsset, value); }
        }

        public double TotaleContabile
        {
            get { return GetValue<double>(() => TotaleContabile); }
            set { SetValue<double>(() => TotaleContabile, value); }
        }
        /// <summary>
        /// Totale Contabile convertita in euro
        /// </summary>
        public double AmountChangedValue
        {
            get { return GetValue<double>(() => AmountChangedValue); }
            set { SetValue<double>(() => AmountChangedValue, value); }
        }
        /// <summary>
        /// Liquidità disponibile agli investimenti
        /// </summary>
        public string AvailableLiquidity
        {
            get { return GetValue<string>(() => AvailableLiquidity); }
            set { SetValue<string>(() => AvailableLiquidity, value); }
        }

        private void SetAvailableLiquidity(DataTable DT)
        {
            double d1 = 0;
            double d2 = 0;

            if (DT.Rows.Count == 0)
            {
                AvailableLiquidity = "";
                return;
            }
            else if (DT.Rows.Count == 1)
            {
                d1 = (double)((DataRow)DT.Rows[0]).ItemArray[0];
            }
            else if (DT.Rows.Count == 2)
            {
                d1 = (double)((DataRow)DT.Rows[0]).ItemArray[0];
                d2 = (double)((DataRow)DT.Rows[1]).ItemArray[0];
                if (d2 < 0) d1 = d1 + d2;
            }
            string v1 = d1.ToString("#,##0.0#", CultureInfo.CreateSpecificCulture("it-IT"));
            string v2 = d2.ToString("#,##0.0#", CultureInfo.CreateSpecificCulture("it-IT"));
            AvailableLiquidity = string.Format("Sono disponibili {0} {1} e {0} {2} messi da parte.", RowLiquidAsset.Cod_valuta, v1, v2);
        }

        #endregion

        #region Collections

        public ObservableCollection<RegistryShare> SharesList
        {
            get { return GetValue(() => SharesList); }
            set
            {
                SetValue(() => SharesList, value);
                SharesListView = new ListCollectionView(value);
            }
        }

        public ListCollectionView SharesListView
        {
            get { return GetValue(() => SharesListView); }
            set { SetValue(() => SharesListView, value); }
        }

        public ObservableCollection<ManagerLiquidAsset> LiquidAssetList
        {
            get { return GetValue(() => LiquidAssetList); }
            set { SetValue(() => LiquidAssetList, value); }
        }

        public ObservableCollection<RegistryCurrency> CurrencyList
        {
            get { return GetValue(() => CurrencyList); }
            set { SetValue(() => CurrencyList, value); }
        }

        public ObservableCollection<RegistryLocation> LocationList
        {
            get { return GetValue(() => LocationList); ; }
            set { SetValue(() => LocationList, value); }
        }

        public ObservableCollection<RegistryMovementType> MovementList
        {
            get { return GetValue(() => MovementList); ; }
            set { SetValue(() => MovementList, value); }
        }

        public ObservableCollection<RegistryOwner> OwnerList
        {
            get { return GetValue(() => OwnerList); ; }
            set { SetValue(() => OwnerList, value); }
        }
        #endregion

        #region Command
        public void SaveCommand(object param)
        {
            try
            {
                ProfitLossCalculation();
                ManagerLiquidAsset MLA = new ManagerLiquidAsset();
                MLA.Id_gestione = RowLiquidAsset.Id_gestione;
                MLA.Id_conto = RowLiquidAsset.Id_conto;
                MLA.Id_tipo_movimento = RowLiquidAsset.Id_tipo_movimento;
                MLA.Id_valuta = RowLiquidAsset.Id_valuta;

                MLA.Id_tipo_titolo = RowLiquidAsset.Id_tipo_titolo;
                MLA.Id_titolo = RowLiquidAsset.Id_titolo;
                MLA.Costo_unitario_in_valuta = RowLiquidAsset.Costo_unitario_in_valuta;
                MLA.N_titoli = RowLiquidAsset.N_titoli;
                MLA.Commissioni_totale = RowLiquidAsset.Commissioni_totale;
                MLA.Valore_di_cambio = RowLiquidAsset.Valore_di_cambio;
                MLA.TobinTax = RowLiquidAsset.TobinTax;
                MLA.Disaggio_anticipo_cedole = RowLiquidAsset.Disaggio_anticipo_cedole;
                MLA.RitenutaFiscale = RowLiquidAsset.RitenutaFiscale;
                MLA.ProfitLoss = RowLiquidAsset.ProfitLoss;
                MLA.Importo_totale = RowLiquidAsset.Importo_totale;
                MLA.Data_Movimento = RowLiquidAsset.Data_Movimento;
                MLA.Note = RowLiquidAsset.Note;

                MLA.Available = true;

                _liquidAssetServices.AddManagerLiquidAsset(MLA);
                // reimposto la griglia con quanto inserito
                LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerSharesMovementByOwnerAndLocation(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto));
                CanInsert = false;          // disabilito la possibilità di un inserimento accidentale
                CanUpdateDelete = true;     // abilito la possibilità di modificare / cancellare il record
                MLA = _liquidAssetServices.GetLastShareMovementByOwnerAndLocation(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto);
                RowLiquidAsset = MLA;
                SetAvailableLiquidity(_liquidAssetServices.GetCurrencyAvailable(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto, RowLiquidAsset.Id_valuta));
                SharesOwned = _liquidAssetServices.GetSharesQuantity(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto, (uint)RowLiquidAsset.Id_titolo).ToString();
                SrchShares = "";
                MessageBox.Show("Record caricato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi nel caricamento del record: " + Environment.NewLine +
                    err.Message, Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateCommand(Object param)
        {
            try
            {
                ProfitLossCalculation();
                _liquidAssetServices.UpdateManagerLiquidAsset(RowLiquidAsset);
                // reimposto la griglia con quanto inserito
                LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerSharesMovementByOwnerAndLocation(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto));
                SrchShares = "";
                MessageBox.Show("Record modificato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi nel modificare il record" + Environment.NewLine + err.Message, Application.Current.FindResource("DAF_Caption").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteCommand(Object param)
        {
            try
            {
                _liquidAssetServices.DeleteManagerLiquidAsset(RowLiquidAsset.Id_portafoglio);
                SetUpViewModel();
                SrchShares = "";
                MessageBox.Show("Record eliminato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi nell'eliminare il record" + Environment.NewLine + err.Message, Application.Current.FindResource("DAF_Caption").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CleanCommand(Object param)
        {
            try
            {
                Button button = param as Button;
                SrchShares = "";
                if (button.Name == "btnClearAll")
                {
                    SetUpViewModel();
                    LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>();
                    SelectedOwner = "";
                }
                else
                {
                    ManagerLiquidAsset MLA = new ManagerLiquidAsset();
                    MLA = RowLiquidAsset;
                    SetUpViewModel();
                    foreach (object o in ((Grid)button.Parent).Children)
                    {
                        if (o.GetType() == typeof(ComboBox))
                        {
                            ComboBox comboBox = o as ComboBox;
                            if (comboBox.Name == "cbOwner")
                            {
                                comboBox.SelectedValue = MLA.Id_gestione;
                                RowLiquidAsset.Id_gestione = MLA.Id_gestione;
                            }
                            else if (comboBox.Name == "cbLocation")
                            {
                                comboBox.SelectedValue = MLA.Id_conto;
                                RowLiquidAsset.Id_conto = MLA.Id_conto;
                            }
                        }
                    }
                }
                CanInsert = false;
                CanUpdateDelete = false;
                SrchShares = string.Empty;
                ProfitLoss = string.Empty;
                TotaleContabile = 0;
                TotalLocalValue = 0;
                AmountChangedValue = 0;
                SharesOwned = "0";
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
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

        public void CloseMe(object param)
        {
            ManagerPortfolioSharesMovementView MPSMV = param as ManagerPortfolioSharesMovementView;
            DockPanel wp = MPSMV.Parent as DockPanel;
            wp.Children.Remove(MPSMV);
        }

        #endregion

    }
}

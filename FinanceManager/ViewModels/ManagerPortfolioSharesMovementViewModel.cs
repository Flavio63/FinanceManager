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
                ShareTypeList = new ObservableCollection<RegistryShareType>(_services.GetRegistryShareTypeList());
                MarketShareList = new ObservableCollection<RegistryMarket>(_services.GetRegistryMarketList());
                RowLiquidAsset = new ManagerLiquidAsset();
                RowLiquidAsset.MovementDate = DateTime.Now;
                CanUpdateDelete = false;
                CanInsert = false;
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
                RegistryShareType RST = e.AddedItems[0] as RegistryShareType;
                RegistryShare RS = e.AddedItems[0] as RegistryShare;
                RegistryMarket RM = e.AddedItems[0] as RegistryMarket;
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
                    RowLiquidAsset.MovementDate = DT.Date;
                    RowLiquidAsset.IdOwner = RO.IdOwner;
                    RowLiquidAsset.OwnerName = RO.OwnerName;
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbLocation", true);

                }
                if (RL != null)
                {
                    RowLiquidAsset.IdLocation = RL.IdLocation;
                    RowLiquidAsset.DescLocation = RL.DescLocation;
                    LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerSharesMovementByOwnerAndLocation(RowLiquidAsset.IdOwner, RL.IdLocation));
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbMovement", true);
                }
                if (RMT != null)
                {
                    RowLiquidAsset.IdMovement = RMT.IdMovement;
                    RowLiquidAsset.DescMovement = RMT.DescMovement;
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbCurrency", true);
                }
                if (RC != null)
                {
                    RowLiquidAsset.IdCurrency = RC.IdCurrency;
                    RowLiquidAsset.CodeCurrency = RC.CodeCurrency;
                    SetAvailableLiquidity(_liquidAssetServices.GetCurrencyAvailable(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation, RowLiquidAsset.IdCurrency));
                    SetProfitLoss(_liquidAssetServices.GetProfitLossByCurrency(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation, RowLiquidAsset.IdCurrency));
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbShareType", true);
                }
                if (RST != null)
                {
                    RowLiquidAsset.IdShareType = RST.IdShareType;
                    RowLiquidAsset.DescShareType = RST.TypeName;
                    SharesList = new ObservableCollection<RegistryShare>(_services.GetSharesByType(RowLiquidAsset.IdShareType));
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbShares", true);
                }
                if (RS != null)
                {
                    RowLiquidAsset.IdShare = RS.IdShare;
                    RowLiquidAsset.DescShare = RS.DescShare;
                    RowLiquidAsset.Isin = RS.Isin;
                    SharesOwned = _liquidAssetServices.GetSharesQuantity(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation, (uint)RowLiquidAsset.IdShare).ToString();

                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbMarket", true);
                }
                if (RM != null)
                {
                    RowLiquidAsset.IdMarket = RM.IdMarket;
                    RowLiquidAsset.DescMarket = RM.DescMarket;
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "unityLocalAmount", true);
                }
                if (dTime)
                    RowLiquidAsset.MovementDate = DT.Date;
                if (CBI != null)
                    RowLiquidAsset.Available = Convert.ToBoolean(CBI.Content);
                if (MLA != null)
                {
                    RowLiquidAsset = MLA;
                    Grid FormGrid = ((Grid)((DataGrid)sender).Parent).Parent as Grid;
                    EnableControl.EnableControlInGrid(FormGrid, "NShares", true);
                    EnableControl.EnableControlInGrid(FormGrid, "CommissionValue", true);
                    EnableControl.EnableControlInGrid(FormGrid, "ExchangeValue", true);
                    UpdateTotals();
                    if (RowLiquidAsset.IdMovement == 5 && !(RowLiquidAsset.IdShareType == 2 || RowLiquidAsset.IdShareType == 16 ||
                        RowLiquidAsset.IdShareType == 9 || RowLiquidAsset.IdShareType == 14 || RowLiquidAsset.IdShareType == 18))
                    {
                        EnableControl.EnableControlInGrid(FormGrid, "TobinTaxValue", true);

                    }
                    else if (RowLiquidAsset.IdShareType == 2 || RowLiquidAsset.IdShareType == 16 ||
                        RowLiquidAsset.IdShareType == 9 || RowLiquidAsset.IdShareType == 14 || RowLiquidAsset.IdShareType == 18)
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
            if (!string.IsNullOrEmpty(TB.Text) && double.TryParse(TB.Text, out converted))
            {
                switch (TB.Name)
                {
                    case "unityLocalAmount":
                        RowLiquidAsset.UnityLocalValue = Convert.ToDouble(TB.Text);
                        if (RowLiquidAsset.UnityLocalValue > 0)
                        {
                            EnableControl.EnableControlInGrid(TB.Parent as Grid, "NShares", true);
                        }
                        break;
                    case "NShares":
                        if (RowLiquidAsset.IdMovement == 5 && Convert.ToDouble(TB.Text) > 0)
                        {
                            EnableControl.EnableControlInGrid(TB.Parent as Grid, "CommissionValue", true);
                            RowLiquidAsset.SharesQuantity = Convert.ToDouble(TB.Text);
                        }
                        else if (RowLiquidAsset.IdMovement == 6 && Convert.ToDouble(TB.Text) < 0)
                        {
                            EnableControl.EnableControlInGrid(TB.Parent as Grid, "CommissionValue", true);
                            RowLiquidAsset.SharesQuantity = Convert.ToDouble(TB.Text);
                        }
                        else
                        {
                            MessageBox.Show("Inserire un importo positivo se si compra o negativo se si vende.", "DAF-C registrazione movimenti titoli", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        break;
                    case "CommissionValue":
                        RowLiquidAsset.TotalCommission = Convert.ToDouble(TB.Text);
                        EnableControl.EnableControlInGrid(TB.Parent as Grid, "ExchangeValue", true);
                        break;
                    case "ExchangeValue":
                        RowLiquidAsset.ExchangeValue = Convert.ToDouble(TB.Text);
                        if (RowLiquidAsset.IdMovement == 5 && !(RowLiquidAsset.IdShareType == 2 || RowLiquidAsset.IdShareType == 16 ||
                            RowLiquidAsset.IdShareType == 9 || RowLiquidAsset.IdShareType == 14 || RowLiquidAsset.IdShareType == 18))
                            EnableControl.EnableControlInGrid(TB.Parent as Grid, "TobinTaxValue", true);
                        else if (RowLiquidAsset.IdMovement == 5 && (RowLiquidAsset.IdShareType == 2 || RowLiquidAsset.IdShareType == 16 ||
                            RowLiquidAsset.IdShareType == 9 || RowLiquidAsset.IdShareType == 14 || RowLiquidAsset.IdShareType == 18))
                            EnableControl.EnableControlInGrid(TB.Parent as Grid, "DisaggioValue", true);
                        else if (RowLiquidAsset.IdMovement == 6)
                            EnableControl.EnableControlInGrid(TB.Parent as Grid, "RitenutaFiscale", true);
                        if (RowLiquidAsset.idLiquidAsset == 0)
                            CanInsert = true;
                        break;
                    case "TobinTaxValue":
                        RowLiquidAsset.TobinTax = Convert.ToDouble(TB.Text);
                        if (RowLiquidAsset.idLiquidAsset == 0)
                            CanInsert = true;
                        break;
                    case "RitenutaFiscale":
                        RowLiquidAsset.RitenutaFiscale = Convert.ToDouble(TB.Text);
                        break;
                    case "DisaggioValue":
                        if (RowLiquidAsset.idLiquidAsset == 0)
                            CanInsert = true;
                        break;
                    case "Note":
                        RowLiquidAsset.Note = TB.Text;
                        break;
                }
                UpdateTotals();
            }
        }
        #endregion events

        private void UpdateTotals()
        {
            RowLiquidAsset.Amount = RowLiquidAsset.IdShareType != 2 ? -1 * (RowLiquidAsset.UnityLocalValue * RowLiquidAsset.SharesQuantity) :
                -1 * (RowLiquidAsset.UnityLocalValue * RowLiquidAsset.SharesQuantity) / 100;

            TotalLocalValue = RowLiquidAsset.Amount < 0 ? RowLiquidAsset.Amount + RowLiquidAsset.TotalCommission * -1 : RowLiquidAsset.Amount - RowLiquidAsset.TotalCommission;

            AmountChangedValue = RowLiquidAsset.IdCurrency == 1 ? TotalLocalValue + (RowLiquidAsset.TobinTax + RowLiquidAsset.DisaggioCoupons + RowLiquidAsset.RitenutaFiscale) * -1
                : TotalLocalValue / RowLiquidAsset.ExchangeValue + (RowLiquidAsset.TobinTax + RowLiquidAsset.DisaggioCoupons + RowLiquidAsset.RitenutaFiscale) * -1;

            if (RowLiquidAsset.IdMovement == 6 && RowLiquidAsset.SharesQuantity != 0)
            {
                ManagerLiquidAssetList MLAL = _liquidAssetServices.GetShareMovements(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation, (uint)RowLiquidAsset.IdShare);
                double PrezzoAcq = 0;
                double NAcq = 0;
                foreach (ManagerLiquidAsset MLA in MLAL)
                {
                    if (RowLiquidAsset.idLiquidAsset > 0 && MLA.idLiquidAsset >= RowLiquidAsset.idLiquidAsset) break;
                    if (MLA.IdMovement == 5)
                    {
                        if (MLA.CodeCurrency == "EUR" && RowLiquidAsset.CodeCurrency != "EUR")
                        {
                            PrezzoAcq += (MLA.Amount + (MLA.TotalCommission + MLA.TobinTax + MLA.DisaggioCoupons + MLA.RitenutaFiscale) * -1) * MLA.ExchangeValue;
                        }
                        else if (MLA.CodeCurrency == RowLiquidAsset.CodeCurrency)
                        {
                            PrezzoAcq += MLA.Amount + (MLA.TotalCommission + MLA.TobinTax + MLA.DisaggioCoupons + MLA.RitenutaFiscale) * -1;
                        }
                        NAcq += MLA.SharesQuantity;
                    }
                    else if (MLA.IdMovement == 6 && MLA.idLiquidAsset != RowLiquidAsset.idLiquidAsset)
                    {
                        if (NAcq + MLA.SharesQuantity == 0)
                        {
                            PrezzoAcq = 0;
                            NAcq = 0;
                        }
                        else if (NAcq + MLA.SharesQuantity != 0)
                        {
                            PrezzoAcq = PrezzoAcq / NAcq * (NAcq + MLA.SharesQuantity);
                            NAcq = NAcq + MLA.SharesQuantity;
                        }
                    }
                }
                if (NAcq + RowLiquidAsset.SharesQuantity == 0)
                {
                    RowLiquidAsset.ProfitLoss = PrezzoAcq +
                        (RowLiquidAsset.Amount + (RowLiquidAsset.TotalCommission + RowLiquidAsset.TobinTax + RowLiquidAsset.DisaggioCoupons + RowLiquidAsset.RitenutaFiscale) * -1);
                }
                else
                {
                    RowLiquidAsset.ProfitLoss = PrezzoAcq / NAcq * RowLiquidAsset.SharesQuantity * -1 +
                        (RowLiquidAsset.Amount + (RowLiquidAsset.TotalCommission + RowLiquidAsset.TobinTax + RowLiquidAsset.DisaggioCoupons + RowLiquidAsset.RitenutaFiscale) * -1);
                }
                PrezzoAcq = 0;
                NAcq = 0;
            }
        }

        public string ProfitLoss
        {
            get { return GetValue<string>(() => ProfitLoss); }
            set { SetValue<string>(() => ProfitLoss, value); }
        }

        private void SetProfitLoss(double PL)
        {
            ProfitLoss = string.Format("Il tuo profit loss in {0} è di: {1}", RowLiquidAsset.CodeCurrency, PL.ToString("#,##0.0#", CultureInfo.CreateSpecificCulture("it-IT")));
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
                string txt = string.Format("Totale titoli {0} posseduti: {1}.", RowLiquidAsset.DescShare, value);
                if (RowLiquidAsset.DescShare == null)
                    txt = "";
                SetValue(() => SharesOwned, txt);
            }
        }

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

        public double AmountChangedValue
        {
            get { return GetValue<double>(() => AmountChangedValue); }
            set { SetValue<double>(() => AmountChangedValue, value); }
        }

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
            AvailableLiquidity = string.Format("Sono disponibili {0} {1} e {0} {2} messi da parte.", RowLiquidAsset.CodeCurrency, v1, v2);
        }

        #endregion

        #region Collections

        public ObservableCollection<RegistryMarket> MarketShareList
        {
            get { return GetValue(() => MarketShareList); }
            set { SetValue(() => MarketShareList, value); }
        }

        public ObservableCollection<RegistryShare> SharesList
        {
            get { return GetValue(() => SharesList); }
            set { SetValue(() => SharesList, value); }
        }

        public ObservableCollection<RegistryShareType> ShareTypeList
        {
            get { return GetValue(() => ShareTypeList); }
            set { SetValue(() => ShareTypeList, value); }
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
                ManagerLiquidAsset MLA = new ManagerLiquidAsset();
                MLA.IdOwner = RowLiquidAsset.IdOwner;
                MLA.IdLocation = RowLiquidAsset.IdLocation;
                MLA.IdMovement = RowLiquidAsset.IdMovement;
                MLA.IdCurrency = RowLiquidAsset.IdCurrency;

                MLA.IdShareType = RowLiquidAsset.IdShareType;
                MLA.IdShare = RowLiquidAsset.IdShare;
                MLA.IdMarket = RowLiquidAsset.IdMarket;
                MLA.UnityLocalValue = RowLiquidAsset.UnityLocalValue;
                MLA.SharesQuantity = RowLiquidAsset.SharesQuantity;
                MLA.TotalCommission = RowLiquidAsset.TotalCommission;
                MLA.ExchangeValue = RowLiquidAsset.ExchangeValue;
                MLA.TobinTax = RowLiquidAsset.TobinTax;
                MLA.DisaggioCoupons = RowLiquidAsset.DisaggioCoupons;
                MLA.RitenutaFiscale = RowLiquidAsset.RitenutaFiscale;
                MLA.ProfitLoss = RowLiquidAsset.ProfitLoss;
                MLA.Amount = RowLiquidAsset.Amount;
                MLA.MovementDate = RowLiquidAsset.MovementDate;
                MLA.Note = RowLiquidAsset.Note;

                MLA.Available = true;

                _liquidAssetServices.AddManagerLiquidAsset(MLA);
                // reimposto la griglia con quanto inserito
                LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerSharesMovementByOwnerAndLocation(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation));
                CanInsert = false;          // disabilito la possibilità di un inserimento accidentale
                CanUpdateDelete = true;     // abilito la possibilità di modificare / cancellare il record
                MLA = _liquidAssetServices.GetLastShareMovementByOwnerAndLocation(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation);
                RowLiquidAsset = MLA;
                SetAvailableLiquidity(_liquidAssetServices.GetCurrencyAvailable(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation, RowLiquidAsset.IdCurrency));
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
                _liquidAssetServices.UpdateManagerLiquidAsset(RowLiquidAsset);
                // reimposto la griglia con quanto inserito
                LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerSharesMovementByOwnerAndLocation(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation));
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
                _liquidAssetServices.DeleteManagerLiquidAsset(RowLiquidAsset.idLiquidAsset);
                SetUpViewModel();
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
                if (button.Name == "btnClearAll")
                {
                    SetUpViewModel();
                    LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>();
                    SelectedOwner = "";
                    CanInsert = false;
                    CanUpdateDelete = false;
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
                                comboBox.SelectedValue = MLA.IdOwner;
                                RowLiquidAsset.IdOwner = MLA.IdOwner;
                            }
                            else if (comboBox.Name == "cbLocation")
                            {
                                comboBox.SelectedValue = MLA.IdLocation;
                                RowLiquidAsset.IdLocation = MLA.IdLocation;
                            }
                        }
                    }
                    CanInsert = false;
                    CanUpdateDelete = false;
                }
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

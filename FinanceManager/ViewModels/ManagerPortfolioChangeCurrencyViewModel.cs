using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class ManagerPortfolioChangeCurrencyViewModel : ViewModelBase
    {
        private IRegistryServices _services;
        private IManagerLiquidAssetServices _liquidAssetServices;

        public ICommand CloseMeCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand EraseCommand { get; set; }

        private string _SelectedOwner;
        private int[] enabledMovement = { 3 };

        public ManagerPortfolioChangeCurrencyViewModel(IRegistryServices services, IManagerLiquidAssetServices liquidAssetServices)
        {
            _services = services ?? throw new ArgumentNullException("Services in Manager Portfolio Movement View Model");
            _liquidAssetServices = liquidAssetServices ?? throw new ArgumentNullException("Liquid Asset Services in Manager Portfolio Movement View Model");
            SetUpViewModel();
            CloseMeCommand = new CommandHandler(CloseMe);
            InsertCommand = new CommandHandler(SaveCommand, CanSave);
            ModifyCommand = new CommandHandler(UpdateCommand, CanModify);
            EraseCommand = new CommandHandler(DeleteCommand, CanModify);
        }

        private void SetUpViewModel()
        {
            try
            {
                OwnerList = new ObservableCollection<RegistryOwner>(_services.GetRegistryOwners());
                MovementList = new ObservableCollection<RegistryMovementType>(_services.GetRegistryMovementTypesList());
                LocationList = new ObservableCollection<RegistryLocation>(_services.GetRegistryLocationList());
                CurrencyListFrom = new ObservableCollection<RegistryCurrency>(_services.GetRegistryCurrencyList());
                CurrencyListTo = new ObservableCollection<RegistryCurrency>(_services.GetRegistryCurrencyList());
                SharesList = new ObservableCollection<RegistryShare>(_services.GetRegistryShareList());
                RowLiquidAsset = new ManagerLiquidAsset();
                IsLiquiditySaved = true;
                AmountChangedValue = 0;
                RowLiquidAsset.MovementDate = DateTime.Now;
                RowLiquidAsset.Available = IsLiquiditySaved;
                CanUpdateDelete = false;
                CanInsert = false;
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "ManagerPortfolioChangeCurrency", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Events

        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ComboBox CB = sender as ComboBox;
                RegistryOwner RO = e.AddedItems[0] as RegistryOwner;
                RegistryLocation RL = e.AddedItems[0] as RegistryLocation;
                RegistryMovementType RMT = e.AddedItems[0] as RegistryMovementType;

                RegistryCurrency RC1 = null;
                RegistryCurrency RC2 = null;
                if (CB != null)
                {
                    if (CB.Name == "cbCurrencyDa")
                        RC1 = e.AddedItems[0] as RegistryCurrency;
                    else if (CB.Name == "cbCurrencyA")
                        RC2 = e.AddedItems[0] as RegistryCurrency;
                }
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
                    AmountChangedValue = 0;
                    RowLiquidAsset.IdOwner = RO.IdOwner;
                    RowLiquidAsset.OwnerName = RO.OwnerName;
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbLocation", true);

                }
                if (RL != null)
                {
                    RowLiquidAsset.IdLocation = RL.IdLocation;
                    RowLiquidAsset.DescLocation = RL.DescLocation;
                    LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerLiquidAssetListByOwnerLocationAndMovementType(RowLiquidAsset.IdOwner, RL.IdLocation, enabledMovement));
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbMovement", true);
                }
                if (RMT != null)
                {
                    RowLiquidAsset.IdMovement = RMT.IdMovement;
                    RowLiquidAsset.DescMovement = RMT.DescMovement;
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbCurrencyDa", true);
                }
                if (RC1 != null)
                {
                    RowLiquidAsset.IdCurrency = RC1.IdCurrency;
                    RowLiquidAsset.CodeCurrency = RC1.CodeCurrency;
                    SetAvailableLiquidity(_liquidAssetServices.GetCurrencyAvailable(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation, RowLiquidAsset.IdCurrency));
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "amount", true);
                }
                if (RC2 != null)
                {
                    if (RC2.IdCurrency == RowLiquidAsset.IdCurrency)
                    {
                        CB.SelectedIndex = -1;
                        return;
                    }
                    RowLiquidAsset.IdCurrency2 = RC2.IdCurrency;
                    RowLiquidAsset.CodeCurrency2 = RC2.CodeCurrency;
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "ExchangeValue", true);
                }
                if (RS != null)
                {
                    RowLiquidAsset.IdShare = RS.IdShare;
                    RowLiquidAsset.Isin = RS.Isin;
                }
                if (dTime)
                    RowLiquidAsset.MovementDate = DT.Date;
                if (CBI != null)
                    RowLiquidAsset.Available = Convert.ToBoolean(CBI.Content);
                if (MLA != null)
                {
                    RowLiquidAsset = MLA;
                    RowLiquidAsset.Amount = MLA.Amount * -1;
                    CanUpdateDelete = true;
                }
            }
        }

        public void TextChanged(object sender, EventArgs e)
        {
            TextBox TB = sender as TextBox;
            switch (TB.Name)
            {
                case "Isin":
                    RowLiquidAsset.Isin = TB.Text;
                    break;
                case "Available":
                    RowLiquidAsset.Available = Convert.ToBoolean(TB.Text);
                    break;
                case "Note":
                    RowLiquidAsset.Note = TB.Text;
                    break;
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
                    case "amount":
                        RowLiquidAsset.Amount = Convert.ToDouble(TB.Text);
                        if (RowLiquidAsset.Amount > AmountAvailable)
                        {
                            RowLiquidAsset.Amount = 0;
                            TB.Text = "0,00";
                        }
                        else
                        {
                            EnableControl.EnableControlInGrid(TB.Parent as Grid, "cbCurrencyA", true);
                        }
                        break;
                    case "ExchangeValue":
                        RowLiquidAsset.ExchangeValue = Convert.ToDouble(TB.Text);
                        if (RowLiquidAsset.ExchangeValue == 0)
                            CanInsert = false;
                        else
                            CanInsert = true;
                        break;
                }
                RowLiquidAsset.AmountChangedValue = RowLiquidAsset.Amount * RowLiquidAsset.ExchangeValue;
                AmountChangedValue = RowLiquidAsset.AmountChangedValue;
            }
        }

        #endregion

        public string SelectedOwner
        {
            get { return _SelectedOwner; }
            set
            {
                _SelectedOwner = "Elenco Movimenti di: " + value;
                NotifyPropertyChanged("SelectedOwner");
            }
        }

        #region Models

        public ManagerLiquidAsset RowLiquidAsset
        {
            get { return GetValue(() => RowLiquidAsset); }
            set { SetValue(() => RowLiquidAsset, value); }
        }

        public double AmountChangedValue
        {
            get { return GetValue(() => AmountChangedValue); }
            set { SetValue(() => AmountChangedValue, value); }
        }

        public double AmountAvailable
        {
            get { return GetValue(() => AmountAvailable); }
            set { SetValue(() => AmountAvailable, value); }
        }
        public bool IsLiquiditySaved
        {
            get { return GetValue(() => IsLiquiditySaved); }
            set { SetValue(() => IsLiquiditySaved, value); }
        }
        public string AvailableLiquidity
        {
            get { return GetValue(() => AvailableLiquidity); }
            set { SetValue(() => AvailableLiquidity, value); }
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
            AmountAvailable = d1;
            AvailableLiquidity = string.Format("Sono disponibili {0} {1} e {0} {2} messi da parte.", RowLiquidAsset.CodeCurrency, v1, v2);
        }

        #endregion

        #region Collections

        public ObservableCollection<RegistryShare> SharesList
        {
            get { return GetValue(() => SharesList); }
            set { SetValue(() => SharesList, value); }
        }

        public ObservableCollection<ManagerLiquidAsset> LiquidAssetList
        {
            get { return GetValue(() => LiquidAssetList); }
            set
            {
                SetValue(() => LiquidAssetList, value);
            }
        }

        public ObservableCollection<RegistryCurrency> CurrencyListFrom
        {
            get { return GetValue(() => CurrencyListFrom); }
            set { SetValue(() => CurrencyListFrom, value); }
        }

        public ObservableCollection<RegistryCurrency> CurrencyListTo
        {
            get { return GetValue(() => CurrencyListTo); ; }
            set { SetValue(() => CurrencyListTo, value); }
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
        public void SaveCommand (object param)
        {
            try
            {
                ManagerLiquidAsset MLA = new ManagerLiquidAsset();
                MLA = RowLiquidAsset;
                MLA.IdShare = 0;
                MLA.Amount = RowLiquidAsset.Amount * -1;
                MLA.Note = RowLiquidAsset.Note == null ? "(" + RowLiquidAsset.CodeCurrency2 + " " + RowLiquidAsset.AmountChangedValue + ")" : 
                    RowLiquidAsset.Note + Environment.NewLine + "(" + RowLiquidAsset.CodeCurrency2 + " " + RowLiquidAsset.AmountChangedValue + ")";
                _liquidAssetServices.AddManagerLiquidAsset(MLA);

                MLA.IdCurrency = RowLiquidAsset.IdCurrency2;
                MLA.Amount = RowLiquidAsset.AmountChangedValue;
                MLA.ExchangeValue = 1 / RowLiquidAsset.ExchangeValue;
                MLA.Note = "(da " + RowLiquidAsset.CodeCurrency + " con cambio di " + RowLiquidAsset.ExchangeValue + ")";
                _liquidAssetServices.AddManagerLiquidAsset(MLA);

                LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(
                    _liquidAssetServices.GetManagerLiquidAssetListByOwnerLocationAndMovementType(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation, enabledMovement));
                SetUpViewModel();
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
                LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(
                    _liquidAssetServices.GetManagerLiquidAssetListByOwnerLocationAndMovementType(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation, enabledMovement));
                SetUpViewModel();
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
                LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(
                    _liquidAssetServices.GetManagerLiquidAssetListByOwnerLocationAndMovementType(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation, enabledMovement));
                SetUpViewModel();
                MessageBox.Show("Record eliminato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi nell'eliminare il record" + Environment.NewLine + err.Message, Application.Current.FindResource("DAF_Caption").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
            ManagerPortfolioChangeCurrencyView MPMV = param as ManagerPortfolioChangeCurrencyView;
            DockPanel wp = MPMV.Parent as DockPanel;
            wp.Children.Remove(MPMV);
        }

        #endregion
    }
}

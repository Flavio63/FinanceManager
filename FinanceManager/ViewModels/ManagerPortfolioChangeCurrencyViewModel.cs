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
        private ManagerLiquidAsset _RowLiquidAsset;
        private int[] enabledMovement = { 3 };
        private double _AmountChangedValue;
        private string _AvailableLiquidity;
        private double _AmountAvailable;

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
            OwnerList = new ObservableCollection<RegistryOwner>(_services.GetRegistryOwners());
            MovementList = new ObservableCollection<RegistryMovementType>(_services.GetRegistryMovementTypesList());
            LocationList = new ObservableCollection<RegistryLocation>(_services.GetRegistryLocationList());
            CurrencyListFrom = new ObservableCollection<RegistryCurrency>(_services.GetRegistryCurrencyList());
            CurrencyListTo = new ObservableCollection<RegistryCurrency>(_services.GetRegistryCurrencyList());
            SharesList = new ObservableCollection<RegistryShare>(_services.GetRegistryShareList());
            RowLiquidAsset = new ManagerLiquidAsset();
            RowLiquidAsset.MovementDate = DateTime.Now;
            CanUpdateDelete = false;
            CanInsert = false;
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
                    RowLiquidAsset = new ManagerLiquidAsset();
                    AmountChangedValue = 0;
                    RowLiquidAsset.MovementDate = DT.Date;
                    RowLiquidAsset.IdOwner = RO.IdOwner;
                    RowLiquidAsset.OwnerName = RO.OwnerName;
                    EnableControl(CB.Parent as Grid, "cbLocation", true);

                }
                if (RL != null)
                {
                    RowLiquidAsset.IdLocation = RL.IdLocation;
                    RowLiquidAsset.DescLocation = RL.DescLocation;
                    LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerLiquidAssetListByOwnerLocationAndMovementType(RowLiquidAsset.IdOwner, RL.IdLocation, enabledMovement));
                    EnableControl(CB.Parent as Grid, "cbMovement", true);
                }
                if (RMT != null)
                {
                    RowLiquidAsset.IdMovement = RMT.IdMovement;
                    RowLiquidAsset.DescMovement = RMT.DescMovement;
                    EnableControl(CB.Parent as Grid, "cbCurrencyDa", true);
                }
                if (RC1 != null)
                {
                    RowLiquidAsset.IdCurrency = RC1.IdCurrency;
                    RowLiquidAsset.CodeCurrency = RC1.CodeCurrency;
                    SetAvailableLiquidity(_liquidAssetServices.GetCurrencyAvailable(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation, RowLiquidAsset.IdCurrency));
                    EnableControl(CB.Parent as Grid, "amount", true);
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
                    EnableControl(CB.Parent as Grid, "ExchangeValue", true);
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
        private void EnableControl(Grid grid, string name, bool blSwitch)
        {
            foreach (object obj in grid.Children)
            {
                Control control = obj as Control;
                if (control != null)
                {
                    if (control.GetType().Name == "ComboBox" && control.Name == name)
                    {
                        control.IsEnabled = blSwitch;
                        return;
                    }
                    if (control.GetType().Name == "Button" && control.Name == name)
                    {
                        control.IsEnabled = blSwitch;
                        return;
                    }
                }
                TextBox textBox = obj as TextBox;
                if (textBox != null)
                    if (textBox.Name == name)
                    {
                        textBox.IsEnabled = blSwitch;
                        return;
                    }
            }
        }
        /*
        public void OnClick(object sender, RoutedEventArgs e)
        {
            Button B = sender as Button;
            if (B.Content.ToString() == "I")
            {
                try
                {
                    ManagerLiquidAsset MLA = new ManagerLiquidAsset();
                    MLA.IdOwner = RowLiquidAsset.IdOwner;
                    MLA.IdLocation = RowLiquidAsset.IdLocation;
                    MLA.IdMovement = RowLiquidAsset.IdMovement;
                    MLA.IdCurrency = RowLiquidAsset.IdCurrency;
                    MLA.Amount = RowLiquidAsset.Amount * -1;
                    MLA.ExchangeValue = RowLiquidAsset.ExchangeValue;
                    MLA.MovementDate = RowLiquidAsset.MovementDate;
                    MLA.Note = RowLiquidAsset.Note + Environment.NewLine + "(" + RowLiquidAsset.CodeCurrency2 + " " + RowLiquidAsset.AmountChangedValue + ")";
                    _liquidAssetServices.AddManagerLiquidAsset(MLA);

                    MLA.IdCurrency = RowLiquidAsset.IdCurrency2;
                    MLA.Amount = RowLiquidAsset.AmountChangedValue;
                    MLA.ExchangeValue = 1 / RowLiquidAsset.ExchangeValue;
                    MLA.Note = "(da " + RowLiquidAsset.CodeCurrency + " con cambio di " + RowLiquidAsset.ExchangeValue + ")";
                    _liquidAssetServices.AddManagerLiquidAsset(MLA);

                    LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerLiquidAssetListByOwner_MovementType(RowLiquidAsset.IdOwner, enabledMovement));
                    MessageBox.Show("Record caricato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception err)
                {
                    MessageBox.Show("Problemi nel caricamento del record: " + Environment.NewLine +
                        err.Message, Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            if (B.Content.ToString() == "M")
            {
                if (RowLiquidAsset.idLiquidAsset != 0)
                {
                    try
                    {
                        _liquidAssetServices.UpdateManagerLiquidAsset(RowLiquidAsset);
                        LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerLiquidAssetListByOwner_MovementType(RowLiquidAsset.IdOwner, enabledMovement));
                        MessageBox.Show("Record modificato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Problemi nel modificare il record" + Environment.NewLine + err.Message, Application.Current.FindResource("DAF_Caption").ToString(),
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            if (B.Content.ToString() == "E")
            {
                if (RowLiquidAsset.idLiquidAsset != 0)
                {
                    MessageBoxResult MBR = MessageBox.Show(string.Format("Il {0} del {1} per l'importo di {2} {3} verrà eliminato.", RowLiquidAsset.DescMovement, RowLiquidAsset.MovementDate.ToShortDateString(),
                        RowLiquidAsset.CodeCurrency, RowLiquidAsset.Amount) + Environment.NewLine + "Sei sicuro?", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (MBR == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _liquidAssetServices.DeleteManagerLiquidAsset(RowLiquidAsset.idLiquidAsset);
                            LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerLiquidAssetListByOwner_MovementType(RowLiquidAsset.IdOwner, enabledMovement));
                            MessageBox.Show("Record eliminato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("Problemi nell'eliminare il record" + Environment.NewLine + err.Message, Application.Current.FindResource("DAF_Caption").ToString(),
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                        return;
                }
            }
        }
        */
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
                            EnableControl(TB.Parent as Grid, "cbCurrencyA", true);
                        }
                        break;
                    case "ExchangeValue":
                        RowLiquidAsset.ExchangeValue = Convert.ToDouble(TB.Text);
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
            get { return _RowLiquidAsset; }
            set
            {
                _RowLiquidAsset = value;
                NotifyPropertyChanged("RowLiquidAsset");
            }
        }

        public double AmountChangedValue
        {
            get { return _AmountChangedValue; }
            set
            {
                _AmountChangedValue = value;
                NotifyPropertyChanged("AmountChangedValue");
            }
        }

        public double AmountAvailable
        {
            get { return _AmountAvailable; }
            set
            {
                _AmountAvailable = value;
                NotifyPropertyChanged("AmountAvailable");
            }
        }
        public string AvailableLiquidity
        {
            get { return _AvailableLiquidity; }
            set
            {
                _AvailableLiquidity = value;
                NotifyPropertyChanged("AvailableLiquidity");
            }
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
                MLA.IdOwner = RowLiquidAsset.IdOwner;
                MLA.IdLocation = RowLiquidAsset.IdLocation;
                MLA.IdMovement = RowLiquidAsset.IdMovement;
                MLA.IdCurrency = RowLiquidAsset.IdCurrency;
                MLA.Amount = RowLiquidAsset.Amount * -1;
                MLA.ExchangeValue = RowLiquidAsset.ExchangeValue;
                MLA.MovementDate = RowLiquidAsset.MovementDate;
                MLA.Note = RowLiquidAsset.Note == null ? "(" + RowLiquidAsset.CodeCurrency2 + " " + RowLiquidAsset.AmountChangedValue + ")" : 
                    RowLiquidAsset.Note + Environment.NewLine + "(" + RowLiquidAsset.CodeCurrency2 + " " + RowLiquidAsset.AmountChangedValue + ")";
                _liquidAssetServices.AddManagerLiquidAsset(MLA);

                MLA.IdCurrency = RowLiquidAsset.IdCurrency2;
                MLA.Amount = RowLiquidAsset.AmountChangedValue;
                MLA.ExchangeValue = 1 / RowLiquidAsset.ExchangeValue;
                MLA.Note = "(da " + RowLiquidAsset.CodeCurrency + " con cambio di " + RowLiquidAsset.ExchangeValue + ")";
                _liquidAssetServices.AddManagerLiquidAsset(MLA);
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

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
    public class ManagerPortfolioMovementViewModel : ViewModelBase
    {
        private IRegistryServices _services;
        private IManagerLiquidAssetServices _liquidAssetServices;
        private ObservableCollection<RegistryOwner> _OwnerList;
        private ObservableCollection<RegistryMovementType> _MovementList;
        private ObservableCollection<RegistryLocation> _LocationList;
        private ObservableCollection<RegistryCurrency> _CurrencyList;
        private ObservableCollection<RegistryShare> _ShareList;

        private ObservableCollection<ManagerLiquidAsset> _LiquidAssetList;
        public ICommand CloseMeCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand EraseCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        private string _SelectedOwner;
        private ManagerLiquidAsset _RowLiquidAsset;
        private int[] enabledMovement = { 1, 2, 4 };

        public ManagerPortfolioMovementViewModel(IRegistryServices services, IManagerLiquidAssetServices liquidAssetServices)
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
                RowLiquidAsset.MovementDate = DateTime.Now;
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "ManagerPortfolioMovementView", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    RowLiquidAsset.MovementDate = DT.Date;
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
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbCurrency", true);
                }
                if (RC != null)
                {
                    RowLiquidAsset.IdCurrency = RC.IdCurrency;
                    RowLiquidAsset.CodeCurrency = RC.CodeCurrency;
                    SetAvailableLiquidity(_liquidAssetServices.GetCurrencyAvailable(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation, RowLiquidAsset.IdCurrency));
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbShare", true);
                }
                if (RS != null)
                {
                    RowLiquidAsset.IdShare = RS.IdShare;
                    RowLiquidAsset.Isin = RS.Isin;
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "amount", true);
                }
                if (dTime)
                    RowLiquidAsset.MovementDate = DT.Date;
                if (CBI != null)
                    RowLiquidAsset.Available = Convert.ToBoolean(CBI.Content);
                if (MLA != null)
                {
                    RowLiquidAsset = MLA;
                    CanUpdateDelete = true;
                    CanInsert = false;
                }
            }
        }

        public void LostFocus(object sender, EventArgs e)
        {
            TextBox TB = sender as TextBox;
            switch (TB.Name)
            {
                case "amount":
                    RowLiquidAsset.Amount = Convert.ToDouble(TB.Text);
                    EnableControl.EnableControlInGrid(TB.Parent as Grid, "ExchangeValue", true);
                    break;
                case "ExchangeValue":
                    RowLiquidAsset.ExchangeValue = Convert.ToDouble(TB.Text);
                    EnableControl.EnableControlInGrid(TB.Parent as Grid, "Available", true);
                    if (RowLiquidAsset.idLiquidAsset == 0)
                        CanInsert = true;
                    break;
                case "Available":
                    RowLiquidAsset.Available = Convert.ToBoolean(TB.Text);
                    break;
                case "Note":
                    RowLiquidAsset.Note = TB.Text;
                    break;
            }
        }

        #endregion

        #region Command
        public void SaveCommand(object param)
        {
            try
            {
                RowLiquidAsset.ProfitLoss = RowLiquidAsset.IdMovement == 4 ? RowLiquidAsset.Amount : 0;

                _liquidAssetServices.AddManagerLiquidAsset(RowLiquidAsset);
                LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerLiquidAssetListByOwnerLocationAndMovementType(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation, enabledMovement));
                SetAvailableLiquidity(_liquidAssetServices.GetCurrencyAvailable(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation, RowLiquidAsset.IdCurrency));
                RowLiquidAsset = new ManagerLiquidAsset();
                MessageBox.Show("Record caricato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                SetUpViewModel();
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
                RowLiquidAsset.ProfitLoss = RowLiquidAsset.IdMovement == 4 ? RowLiquidAsset.Amount : 0;
                _liquidAssetServices.UpdateManagerLiquidAsset(RowLiquidAsset);
                LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerLiquidAssetListByOwnerLocationAndMovementType(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation, enabledMovement));
                SetAvailableLiquidity(_liquidAssetServices.GetCurrencyAvailable(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation, RowLiquidAsset.IdCurrency));
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
            MessageBoxResult MBR = MessageBox.Show(string.Format("Il {0} del {1} per l'importo di {2} {3} verrà eliminato.", RowLiquidAsset.DescMovement, RowLiquidAsset.MovementDate.ToShortDateString(),
                RowLiquidAsset.CodeCurrency, RowLiquidAsset.Amount) + Environment.NewLine + "Sei sicuro?", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MBR == MessageBoxResult.Yes)
            {
                try
                {
                    _liquidAssetServices.DeleteManagerLiquidAsset(RowLiquidAsset.idLiquidAsset);
                    LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerLiquidAssetListByOwnerLocationAndMovementType(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation, enabledMovement));
                    SetAvailableLiquidity(_liquidAssetServices.GetCurrencyAvailable(RowLiquidAsset.IdOwner, RowLiquidAsset.IdLocation, RowLiquidAsset.IdCurrency));
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

        public ObservableCollection<RegistryShare> SharesList
        {
            get { return _ShareList; }
            set
            {
                _ShareList = value;
                NotifyPropertyChanged("SharesList");
            }
        }

        public ObservableCollection<ManagerLiquidAsset> LiquidAssetList
        {
            get { return _LiquidAssetList; }
            set
            {
                _LiquidAssetList = value;
                NotifyPropertyChanged("LiquidAssetList");
            }
        }

        public ObservableCollection<RegistryCurrency> CurrencyList
        {
            get { return _CurrencyList; }
            set
            {
                _CurrencyList = value;
                NotifyPropertyChanged("CurrencyList");
            }
        }

        public ObservableCollection<RegistryLocation> LocationList
        {
            get { return _LocationList; }
            set
            {
                _LocationList = value;
                NotifyPropertyChanged("LocationList");
            }
        }

        public ObservableCollection<RegistryMovementType> MovementList
        {
            get { return _MovementList; }
            set
            {
                _MovementList = value;
                NotifyPropertyChanged("MovementList");
            }
        }

        public ObservableCollection<RegistryOwner> OwnerList
        {
            get { return _OwnerList; }
            set
            {
                _OwnerList = value;
                NotifyPropertyChanged("OwnerList");
            }
        }
        #endregion

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
                    StackPanel sp =button.Parent as StackPanel;

                    foreach ( object o in ((Grid)sp.Parent).Children )
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
            return CanUpdateDelete;
        }

        public void CloseMe(object param)
        {
            ManagerPortfolioMovementView MPMV = param as ManagerPortfolioMovementView;
            DockPanel wp = MPMV.Parent as DockPanel;
            wp.Children.Remove(MPMV);
        }

    }
}
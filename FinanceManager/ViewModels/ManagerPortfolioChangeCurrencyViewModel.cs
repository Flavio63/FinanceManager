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
        public ICommand ClearCommand { get; set; }

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
            ClearCommand = new CommandHandler(CleanCommand);
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
                RowLiquidAsset.Data_Movimento = DateTime.Now;
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
                    RowLiquidAsset.Id_gestione = RO.IdOwner;
                    RowLiquidAsset.Nome_Gestione = RO.OwnerName;
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbLocation", true);

                }
                if (RL != null)
                {
                    RowLiquidAsset.Id_conto = RL.IdLocation;
                    RowLiquidAsset.Desc_conto = RL.DescLocation;
                    LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerLiquidAssetListByOwnerLocationAndMovementType(RowLiquidAsset.Id_gestione, RL.IdLocation, enabledMovement));
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbMovement", true);
                }
                if (RMT != null)
                {
                    RowLiquidAsset.Id_tipo_movimento = RMT.IdMovement;
                    RowLiquidAsset.Desc_tipo_movimento = RMT.DescMovement;
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "cbCurrencyDa", true);
                }
                if (RC1 != null)
                {
                    RowLiquidAsset.Id_valuta = RC1.IdCurrency;
                    RowLiquidAsset.Cod_valuta = RC1.CodeCurrency;
                    SetAvailableLiquidity(_liquidAssetServices.GetCurrencyAvailable(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto, RowLiquidAsset.Id_valuta));
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "amount", true);
                }
                if (RC2 != null)
                {
                    if (RC2.IdCurrency == RowLiquidAsset.Id_valuta)
                    {
                        CB.SelectedIndex = -1;
                        return;
                    }
                    RowLiquidAsset.Id_valuta_2 = RC2.IdCurrency;
                    RowLiquidAsset.Code_valuta_2 = RC2.CodeCurrency;
                    EnableControl.EnableControlInGrid(CB.Parent as Grid, "Valore_di_cambio", true);
                }
                if (RS != null)
                {
                    RowLiquidAsset.Id_titolo = RS.IdShare;
                    RowLiquidAsset.Isin = RS.Isin;
                }
                if (dTime)
                    RowLiquidAsset.Data_Movimento = DT.Date;
                if (CBI != null)
                    RowLiquidAsset.Available = Convert.ToBoolean(CBI.Content);
                if (MLA != null)
                {
                    RowLiquidAsset = MLA;
                    RowLiquidAsset.Importo_totale = MLA.Importo_totale * -1;
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
                        RowLiquidAsset.Importo_totale = Convert.ToDouble(TB.Text);
                        if (RowLiquidAsset.Importo_totale > AmountAvailable)
                        {
                            RowLiquidAsset.Importo_totale = 0;
                            TB.Text = "0,00";
                        }
                        else
                        {
                            EnableControl.EnableControlInGrid(TB.Parent as Grid, "cbCurrencyA", true);
                        }
                        break;
                    case "Valore_di_cambio":
                        RowLiquidAsset.Valore_di_cambio = Convert.ToDouble(TB.Text);
                        if (RowLiquidAsset.Valore_di_cambio == 0)
                            CanInsert = false;
                        else
                            CanInsert = true;
                        break;
                }
                RowLiquidAsset.Importo_cambiato = RowLiquidAsset.Importo_totale * RowLiquidAsset.Valore_di_cambio;
                AmountChangedValue = RowLiquidAsset.Importo_cambiato;
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
            AvailableLiquidity = string.Format("Sono disponibili {0} {1} e {0} {2} messi da parte.", RowLiquidAsset.Cod_valuta, v1, v2);
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
                MLA.Id_titolo = 0;
                MLA.Importo_totale = RowLiquidAsset.Importo_totale * -1;
                MLA.Note = RowLiquidAsset.Note == null ? "(" + RowLiquidAsset.Code_valuta_2 + " " + RowLiquidAsset.Importo_cambiato + ")" : 
                    RowLiquidAsset.Note + Environment.NewLine + "(" + RowLiquidAsset.Code_valuta_2 + " " + RowLiquidAsset.Importo_cambiato + ")";
                _liquidAssetServices.AddManagerLiquidAsset(MLA);

                MLA.Id_valuta = RowLiquidAsset.Id_valuta_2;
                MLA.Importo_totale = RowLiquidAsset.Importo_cambiato;
                MLA.Valore_di_cambio = 1 / RowLiquidAsset.Valore_di_cambio;
                MLA.Note = "(da " + RowLiquidAsset.Cod_valuta + " con cambio di " + RowLiquidAsset.Valore_di_cambio + ")";
                _liquidAssetServices.AddManagerLiquidAsset(MLA);

                LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(
                    _liquidAssetServices.GetManagerLiquidAssetListByOwnerLocationAndMovementType(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto, enabledMovement));
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
                    _liquidAssetServices.GetManagerLiquidAssetListByOwnerLocationAndMovementType(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto, enabledMovement));
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
                _liquidAssetServices.DeleteManagerLiquidAsset(RowLiquidAsset.Id_portafoglio);
                LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(
                    _liquidAssetServices.GetManagerLiquidAssetListByOwnerLocationAndMovementType(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto, enabledMovement));
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
                    StackPanel sp = button.Parent as StackPanel;

                    foreach (object o in ((Grid)sp.Parent).Children)
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
                    CanInsert = false;
                    CanUpdateDelete = false;
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
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

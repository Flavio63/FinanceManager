using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class ManagerPortfolioChangeCurrencyViewModel : ViewModelBase
    {
        private IRegistryServices _services;
        private IManagerLiquidAssetServices _liquidAssetServices;
        private ObservableCollection<RegistryOwner> _OwnerList;
        private ObservableCollection<RegistryMovementType> _MovementList;
        private ObservableCollection<RegistryLocation> _LocationList;
        private ObservableCollection<RegistryCurrency> _CurrencyListFrom;
        private ObservableCollection<RegistryCurrency> _CurrencyListTo;
        private ObservableCollection<RegistryShare> _ShareList;

        private ObservableCollection<ManagerLiquidAsset> _LiquidAssetList;
        public ICommand CloseMeCommand { get; set; }

        private string _SelectedOwner;
        private ManagerLiquidAsset _RowLiquidAsset;
        private int[] enabledMovement = { 3 };

        public ManagerPortfolioChangeCurrencyViewModel(IRegistryServices services, IManagerLiquidAssetServices liquidAssetServices)
        {
            _services = services ?? throw new ArgumentNullException("Services in Manager Portfolio Movement View Model");
            _liquidAssetServices = liquidAssetServices ?? throw new ArgumentNullException("Liquid Asset Services in Manager Portfolio Movement View Model");
            OwnerList = new ObservableCollection<RegistryOwner>(_services.GetRegistryOwners());
            MovementList = new ObservableCollection<RegistryMovementType>(_services.GetRegistryMovementTypesList());
            LocationList = new ObservableCollection<RegistryLocation>(_services.GetRegistryLocationList());
            CurrencyListFrom = new ObservableCollection<RegistryCurrency>(_services.GetRegistryCurrencyList());
            CurrencyListTo = new ObservableCollection<RegistryCurrency>(_services.GetRegistryCurrencyList());
            SharesList = new ObservableCollection<RegistryShare>(_services.GetRegistryShareList());
            CloseMeCommand = new CommandHandler(CloseMe);
            RowLiquidAsset = new ManagerLiquidAsset();
            RowLiquidAsset.MovementDate = DateTime.Now;
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

                RegistryCurrency RC1 = new RegistryCurrency();
                RegistryCurrency RC2 = new RegistryCurrency();
                if (CB.Name == "cbCurrencyDa")
                    RC1 = e.AddedItems[0] as RegistryCurrency;
                else if (CB.Name == "cbCurrencyA")
                    RC2 = e.AddedItems[0] as RegistryCurrency;

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
                    RowLiquidAsset.MovementDate = DT.Date;
                    RowLiquidAsset.IdOwner = RO.IdOwner;
                    RowLiquidAsset.OwnerName = RO.OwnerName;
                    LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerLiquidAssetListByOwner_MovementType(RO.IdOwner, enabledMovement));
                    NotifyPropertyChanged("CbSelectionChanged");
                }
                if (RL != null)
                {
                    RowLiquidAsset.IdLocation = RL.IdLocation;
                    RowLiquidAsset.DescLocation = RL.DescLocation;
                }
                if (RMT != null)
                {
                    RowLiquidAsset.IdMovement = RMT.IdMovement;
                    RowLiquidAsset.DescMovement = RMT.DescMovement;
                }
                if (RC1 != null)
                {
                    RowLiquidAsset.IdCurrency = RC1.IdCurrency;
                    RowLiquidAsset.CodeCurrency = RC1.CodeCurrency;
                }
                if (RC2 != null)
                {
                    RowLiquidAsset.IdCurrency2 = RC2.IdCurrency;
                    RowLiquidAsset.CodeCurrency2 = RC2.CodeCurrency;
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
                    NotifyPropertyChanged("SelectedMovement");
                }
            }
        }

        public void OnClick(object sender, RoutedEventArgs e)
        {
            Button B = sender as Button;
            if (B.Content.ToString() == "I")
            {
                if (RowLiquidAsset.idLiquidAsset != 0)
                {
                    MessageBoxResult R = MessageBox.Show("I dati della maschera provengono da un record esistente, vuoi inserirlo come nuovo?",
                        Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (R == MessageBoxResult.Yes)
                    {
                        RowLiquidAsset.idLiquidAsset = 0;
                    }
                    else
                        return;
                }
                if (RowLiquidAsset.IdOwner == 0 || RowLiquidAsset.IdLocation == 0 ||
                    RowLiquidAsset.IdMovement == 0 || RowLiquidAsset.IdCurrency == 0)
                {
                    MessageBox.Show("I primi 4 campi devono essere tutti compilati.", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    try
                    {
                        _liquidAssetServices.AddManagerLiquidAsset(RowLiquidAsset);
                        LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerLiquidAssetListByOwner_MovementType(RowLiquidAsset.IdOwner, enabledMovement));
                        MessageBox.Show("Record caricato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Problemi nel caricamento del record: " + Environment.NewLine +
                            err.Message, Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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

        public void TextChanged(object sender, EventArgs e)
        {
            TextBox TB = sender as TextBox;
            switch (TB.Name)
            {
                case "amount":
                    RowLiquidAsset.Amount = Convert.ToDouble(TB.Text.Replace(",", "").Replace(".", ","));
                    break;
                case "ExchangeValue":
                    RowLiquidAsset.ExchangeValue = Convert.ToDouble(TB.Text.Replace(",", "").Replace(".", ","));
                    break;
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

        public ObservableCollection<RegistryCurrency> CurrencyListFrom
        {
            get { return _CurrencyListFrom; }
            set
            {
                _CurrencyListFrom = value;
                NotifyPropertyChanged("CurrencyListFrom");
            }
        }

        public ObservableCollection<RegistryCurrency> CurrencyListTo
        {
            get { return _CurrencyListTo; }
            set
            {
                _CurrencyListTo = value;
                NotifyPropertyChanged("CurrencyListTo");
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

        public void CloseMe(object param)
        {
            ManagerPortfolioChangeCurrencyView MPMV = param as ManagerPortfolioChangeCurrencyView;
            DockPanel wp = MPMV.Parent as DockPanel;
            wp.Children.Remove(MPMV);
        }
    }
}

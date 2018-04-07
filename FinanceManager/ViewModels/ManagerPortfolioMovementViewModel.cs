using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class ManagerPortfolioMovementViewModel : ViewModelBase
    {
        private static readonly string caption = "DAF-C Gestione Movimenti";
        private IRegistryServices _services;
        private IManagerLiquidAssetServices _liquidAssetServices;
        private ObservableCollection<RegistryOwner> _OwnerList;
        private ObservableCollection<RegistryMovementType> _MovementList;
        private ObservableCollection<RegistryLocation> _LocationList;
        private ObservableCollection<RegistryCurrency> _CurrencyList;

        private ObservableCollection<ManagerLiquidAsset> _LiquidAssetList;
        public ICommand CloseMeCommand { get; set; }

        private string _SelectedOwner;
        private ManagerLiquidAsset _RowLiquidAsset;

        public ManagerPortfolioMovementViewModel(IRegistryServices services, IManagerLiquidAssetServices liquidAssetServices)
        {
            _services = services ?? throw new ArgumentNullException("Services in Manager Portfolio Movement View Model");
            _liquidAssetServices = liquidAssetServices ?? throw new ArgumentNullException("Liquid Asset Services in Manager Portfolio Movement View Model");
            OwnerList = new ObservableCollection<RegistryOwner>(_services.GetRegistryOwners());
            MovementList = new ObservableCollection<RegistryMovementType>(_services.GetRegistryMovementTypesList());
            LocationList = new ObservableCollection<RegistryLocation>(_services.GetRegistryLocationList());
            CurrencyList = new ObservableCollection<RegistryCurrency>(_services.GetRegistryCurrencyList());
            CloseMeCommand = new CommandHandler(CloseMe);
            RowLiquidAsset = new ManagerLiquidAsset();
        }

        #region Events

        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RegistryOwner RO = e.AddedItems[0] as RegistryOwner;
            RegistryLocation RL = e.AddedItems[0] as RegistryLocation;
            RegistryMovementType RMT = e.AddedItems[0] as RegistryMovementType;
            RegistryCurrency RC = e.AddedItems[0] as RegistryCurrency;
            DateTime DT = new DateTime();
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

                RowLiquidAsset.IdOwner = RO.IdOwner;
                RowLiquidAsset.OwnerName = RO.OwnerName;
                LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerLiquidAssetList(RO.IdOwner));
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
            if (RC != null)
            {
                RowLiquidAsset.IdCurrency = RC.IdCurrency;
                RowLiquidAsset.CodeCurrency = RC.CodeCurrency;
            }
            if (dTime)
                RowLiquidAsset.MovementDate = DT.Date;

        }

        public void OnClick(object sender, RoutedEventArgs e)
        {
            Button B = sender as Button;
            if (B.Content.ToString() == "I")
            {
                if (RowLiquidAsset.idLiquidAsset != 0)
                {
                    MessageBoxResult R = MessageBox.Show("I dati della maschera provengono da un record esistente, vuoi inserirlo come nuovo?",
                        caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
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
                    MessageBox.Show("I primi 4 campi devono essere tutti compilati.", caption, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    try
                    {
                        _liquidAssetServices.AddManagerLiquidAsset(RowLiquidAsset);
                        MessageBox.Show("Record caricato!");
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Problemi nel caricamento del record: " + Environment.NewLine +
                            err.Message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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

        public void CloseMe(object param)
        {
            ManagerPortfolioMovementView MPMV = param as ManagerPortfolioMovementView;
            DockPanel wp = MPMV.Parent as DockPanel;
            wp.Children.Remove(MPMV);
        }
    }
}

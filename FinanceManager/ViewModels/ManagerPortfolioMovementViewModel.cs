using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private ObservableCollection<ManagerLiquidAsset> _LiquidAssetList;
        public ICommand CloseMeCommand { get; set; }

        public ManagerPortfolioMovementViewModel(IRegistryServices services, IManagerLiquidAssetServices liquidAssetServices)
        {
            _services = services ?? throw new ArgumentNullException("Services in Manager Portfolio Movement View Model");
            _liquidAssetServices = liquidAssetServices ?? throw new ArgumentNullException("Liquid Asset Services in Manager Portfolio Movement View Model");
            OwnerList = new ObservableCollection<RegistryOwner>(_services.GetRegistryOwners());
            MovementList = new ObservableCollection<RegistryMovementType>(_services.GetRegistryMovementTypesList());
            LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerLiquidAssetList(3)); // spostare al handle del owner cb
            CloseMeCommand = new CommandHandler(CloseMe);
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

        public void CloseMe(object param)
        {
            ManagerPortfolioMovementView MPMV = param as ManagerPortfolioMovementView;
            DockPanel wp = MPMV.Parent as DockPanel;
            wp.Children.Remove(MPMV);
        }
    }
}

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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class GestioneQuoteInvestitoriViewModel : ViewModelBase
    {
        private IRegistryServices _registryServices;
        private IManagerLiquidAssetServices _managerLiquidServices;
        public ICommand CloseMeCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand EraseCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand CosaFai { get; set; }

        public GestioneQuoteInvestitoriViewModel(IRegistryServices registryServices, IManagerLiquidAssetServices managerLiquidServices)
        {
            _registryServices = registryServices ?? throw new ArgumentNullException("Services in Manager Portfolio Movement View Model");
            _managerLiquidServices = managerLiquidServices ?? throw new ArgumentNullException("Liquid Asset Services in Manager Portfolio Movement View Model");

            Init();

            Visibility1_2 = Visibility.Collapsed;
            ListQuote = _managerLiquidServices.GetQuote();
            ListInvestitore = _managerLiquidServices.GetInvestitori();
        }

        private void Init()
        {
            CloseMeCommand = new CommandHandler(CloseMe);
            InsertCommand = new CommandHandler(SaveCommand, CanSave);
            ModifyCommand = new CommandHandler(UpdateCommand, CanModify);
            EraseCommand = new CommandHandler(DeleteCommand, CanModify);
            ClearCommand = new CommandHandler(CleanCommand);
            CosaFai = new CommandHandler(AzioneScelta);
            ListQuote = new ObservableCollection<Quote>();
            ListInvestitore = new ObservableCollection<Investitore>();
        }

        #region Getter&Setter
        public Visibility Visibility1_2
        {
            get { return GetValue(() => Visibility1_2); }
            set { SetValue(() => Visibility1_2, value); }
        }

        public ObservableCollection<Quote> ListQuote
        {
            get { return GetValue(() => ListQuote); }
            set { SetValue(() => ListQuote, value); }
        }

        public ObservableCollection<Investitore> ListInvestitore
        {
            get { return GetValue(() => ListInvestitore); }
            set { SetValue(() => ListInvestitore, value); }
        }

        #endregion

        #region Events

        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ComboBox CB = sender as ComboBox;
            }
        }

        public void LostFocus(object sender, EventArgs e)
        {
            TextBox TB = sender as TextBox;
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

        #region Commands
        public void AzioneScelta(object param)
        {
            string scelta = param.ToString();
            if (scelta == "Aggiungere" || scelta == "Togliere")
            {
                Visibility1_2 = Visibility.Visible;
            }
            else
            {
                Visibility1_2 = Visibility.Collapsed;
            }
        }
        public void SaveCommand(object param)
        {
            try
            {
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public void UpdateCommand(Object param)
        {
            try
            {
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public void DeleteCommand(Object param)
        {
            try
            {
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public void CleanCommand(Object param)
        {
            try
            {
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
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
            GestioneQuoteInvestitoriView view = param as GestioneQuoteInvestitoriView;
            DockPanel wp = view.Parent as DockPanel;
            wp.Children.Remove(view);
        }

        #endregion
    }
}

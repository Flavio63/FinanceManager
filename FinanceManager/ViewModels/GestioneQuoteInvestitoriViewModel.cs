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
        public ICommand OkCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand EraseCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand CosaFai { get; set; }

        public GestioneQuoteInvestitoriViewModel(IRegistryServices registryServices, IManagerLiquidAssetServices managerLiquidServices)
        {
            _registryServices = registryServices ?? throw new ArgumentNullException("Services in Manager Portfolio Movement View Model");
            _managerLiquidServices = managerLiquidServices ?? throw new ArgumentNullException("Liquid Asset Services in Manager Portfolio Movement View Model");

            Init();

            ListQuote = _managerLiquidServices.GetQuote();
            ListInvestitore = _managerLiquidServices.GetInvestitori();
            ListTabQuote = _managerLiquidServices.GetQuoteTab();

            Visibility1 = Visibility.Collapsed;
            Visibility2 = Visibility.Collapsed;
            Visibility3 = Visibility.Collapsed;
            Visibility4 = Visibility.Collapsed;
        }

        private void Init()
        {
            CloseMeCommand = new CommandHandler(CloseMe);
            OkCommand = new CommandHandler(SaveCommand, CanSave);
            ModifyCommand = new CommandHandler(UpdateCommand, CanModify);
            EraseCommand = new CommandHandler(DeleteCommand, CanModify);
            ClearCommand = new CommandHandler(CleanCommand);
            CosaFai = new CommandHandler(AzioneScelta);
            ListQuote = new QuoteList();
            ListTabQuote = new QuoteTabList();
            ListInvestitore = new InvestitoreList();
            ActualQuote = new QuoteTab();
            ActualQuote.DataMovimento = DateTime.Now.Date;
        }

        #region Getter&Setter
        public Visibility Visibility1
        {
            get { return GetValue(() => Visibility1); }
            set { SetValue(() => Visibility1, value); }
        }

        public Visibility Visibility2
        {
            get { return GetValue(() => Visibility2); }
            set { SetValue(() => Visibility2, value); }
        }

        public Visibility Visibility3
        {
            get { return GetValue(() => Visibility3); }
            set { SetValue(() => Visibility3, value); }
        }

        public Visibility Visibility4
        {
            get { return GetValue(() => Visibility4); }
            set { SetValue(() => Visibility4, value); }
        }

        public QuoteList ListQuote
        {
            get { return GetValue(() => ListQuote); }
            set { SetValue(() => ListQuote, value); }
        }

        public QuoteTabList ListTabQuote
        {
            get { return GetValue(() => ListTabQuote); }
            set { SetValue(() => ListTabQuote, value); }
        }

        public QuoteTab quoteTab
        {
            get { return GetValue(() => quoteTab); }
            set { SetValue(() => quoteTab, value); }
        }

        public InvestitoreList ListInvestitore
        {
            get { return GetValue(() => ListInvestitore); }
            set { SetValue(() => ListInvestitore, value); }
        }

        public QuoteTab ActualQuote
        {
            get { return GetValue(() => ActualQuote); }
            set { SetValue(() => ActualQuote, value); }
        }

        public DateTime DataMovimento
        {
            get { return GetValue(() => DataMovimento); }
            set
            {
                SetValue(() => DataMovimento, value);
                ActualQuote.DataMovimento = value;
            }
        }

        #endregion

        #region Events

        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0].GetType().Name == "DateTime")
                {
                    DataMovimento = (DateTime)e.AddedItems[0];
                    return;
                }
                Investitore investitore = e.AddedItems[0] as Investitore;                
                if (investitore != null)
                {
                    ActualQuote.IdInvestitore = investitore.IdInvestitore;
                    ActualQuote.NomeInvestitore = investitore.NomeInvestitore;
                    return;
                }
            }
        }

        public void LostFocus(object sender, EventArgs e)
        {
            TextBox TB = sender as TextBox;
            if (TB != null && TB.Name == "txtAmmontare")
            {
                if (ActualQuote.IdMovimento == 2 && ActualQuote.Ammontare > 0)
                {
                    MessageBox.Show("Attenzione devi inserire una cifra negativa se vuoi prelevare", 
                        "Gestione Quote", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    ActualQuote.Ammontare = Convert.ToDouble(TB.Text.Replace(".", "").Replace(",", ".").Replace(" €", ""));
                }
            }
            else if(TB != null && TB.Name == "txtNote")
            {
                ActualQuote.Note = TB.Text;
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

        #region Commands
        public void AzioneScelta(object param)
        {
            string scelta = param.ToString();
            if (scelta == "Aggiungi" || scelta == "Togli")
            {
                ActualQuote = new QuoteTab();
                DataMovimento = DateTime.Now.Date;
                ActualQuote.IdMovimento = scelta == "Aggiungi" ? 1 : 2;
                Visibility1 = Visibility.Visible;
                Visibility3 = Visibility.Visible;
                Visibility2 = Visibility.Collapsed;
                Visibility4 = Visibility.Collapsed;
            }
            else if (scelta == "Giroconto1" || scelta == "Giroconto2")
            {
                ActualQuote = new QuoteTab();
                DataMovimento = DateTime.Now.Date;
                Visibility1 = Visibility.Visible;
                Visibility3 = Visibility.Collapsed;
                Visibility2 = Visibility.Visible;
                Visibility4 = Visibility.Collapsed;
            }
            else if (scelta == "Verifica")
            {
                ActualQuote = new QuoteTab();

                DataMovimento = DateTime.Now.Date;
                Visibility1 = Visibility.Collapsed;
                Visibility3 = Visibility.Collapsed;
                Visibility2 = Visibility.Collapsed;
                Visibility4 = Visibility.Visible;
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

        public bool CanSave(object param)
        {
            if (ActualQuote.IdInvestitore > 0 && ActualQuote.Ammontare > 0 && ActualQuote.IdMovimento == 1)
                return true;
            else if (ActualQuote.IdInvestitore > 0 && ActualQuote.Ammontare < 0 && ActualQuote.IdMovimento == 2)
                return true;
            return false;
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

using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class RegistryCurrencyViewModel : INotifyPropertyChanged
    {
        IRegistryServices _services;
        private RegistryCurrency currency;
        private ObservableCollection<RegistryCurrency> _currencyList;
        public ICommand CloseMeCommand { get; set; }

        public RegistryCurrencyViewModel(IRegistryServices service)
        {
            _services = service ?? throw new ArgumentNullException("RegistryCurrencyModel With No Services");
            try
            {
                CurrencyList = new ObservableCollection<RegistryCurrency>(service.GetRegistryCurrencyList());
                CurrencyList.CollectionChanged += CollectionHasChanged;
            }
            catch (Exception err)
            {
                MessageBox.Show("Errore nella richiesta dei dati." + Environment.NewLine + err.Message, "DAF-C Lista Valute");
            }
            CloseMeCommand = new CommandHandler(CloseMe);
        }

        public void CollectionHasChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //ListCollectionView _currencyList = sender as ListCollectionView;
        }

        /// <summary>
        /// E' l'evento di edit nella cella di descrizione della gestione
        /// se il modello ha un valore di id vuol dire che è in modifica
        /// se il valore è zero vuol dire che è un inserimento di nuova gestione
        /// </summary>
        /// <param name="sender">la cella di descrizione</param>
        /// <param name="e">la conferma o meno della modifica</param>
        public void CellChanged(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {

                if (e.EditAction == DataGridEditAction.Commit)
                {
                    Currency = ((RegistryCurrency)e.Row.Item);
                    if (Currency.IdCurrency > 0)
                    {
                        _services.UpdateCurrency(Currency);
                    }
                    else
                    {
                        if (Currency.CodeCurrency != null && Currency.DescCurrency != null)
                        {
                            _services.AddCurrency(Currency);
                            CurrencyList = new ObservableCollection<RegistryCurrency>(_services.GetRegistryCurrencyList());
                        }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Errore nell'aggiornamento dei dati: " + err.Message);
            }
        }
        /// <summary>
        /// Resto in ascolto dei tasti premuti con la griglia attiva
        /// se è premuto il tasto delete lo intercetto e pongo la 
        /// domanda se si è sicuri, in caso affermativo elimino la gestione
        /// </summary>
        /// <param name="sender">tastiera</param>
        /// <param name="e">tasto premuto</param>
        public void DeleteRow(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DataGrid dg = sender as DataGrid;
                if (dg.SelectedIndex > 0)
                {
                    MessageBoxResult result = MessageBox.Show("Attenzione verrà elemininata la seguente valuta: " +
                        ((RegistryCurrency)dg.SelectedItem).DescCurrency + " - " + ((RegistryCurrency)dg.SelectedItem).CodeCurrency, "DAF-C Gestione Gestioni", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _services.DeleteCurrency(((RegistryCurrency)dg.SelectedItem).IdCurrency);
                            CurrencyList = new ObservableCollection<RegistryCurrency>(_services.GetRegistryCurrencyList());
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("Errore nell'eliminazione della gestione: " + Environment.NewLine + err.Message);
                            e.Handled = true;
                        }
                    }
                    else
                        e.Handled = true;
                }
            }
        }

        public ObservableCollection<RegistryCurrency> CurrencyList
        {
            get { return _currencyList; }
            set
            {
                _currencyList = value;
                NotifyPropertyChanged("CurrencyList");
            }
        }

        public RegistryCurrency Currency
        {
            get { return currency; }
            set
            {
                currency = value;
                NotifyPropertyChanged("Currency");
            }
        }
        /// <summary>
        /// Evento di chiusura della view Gestione gestioni
        /// </summary>
        /// <param name="param">La view che ha inviato l'evento</param>
        public void CloseMe(object param)
        {
            RegistryCurrencyView ROV = param as RegistryCurrencyView;
            DockPanel wp = ROV.Parent as DockPanel;
            wp.Children.Remove(ROV);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

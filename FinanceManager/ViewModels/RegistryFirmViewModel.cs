using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class RegistryFirmViewModel : ViewModelBase
    {
        private IRegistryServices _services;
        private RegistryFirm _firm;
        private ObservableCollection<RegistryFirm> _FirmList;
        public ICommand CloseMeCommand { get; set; }

        public RegistryFirmViewModel(IRegistryServices services)
        {
            _services = services ?? throw new ArgumentNullException("RegistryFirmViewModel With No Services");
            FirmList = new ObservableCollection<RegistryFirm>(services.GetRegistryFirmList());
            CloseMeCommand = new CommandHandler(CloseMe);
        }

        /// <summary>
        /// E' l'evento di edit nella cella di descrizione della gestione
        /// se il modello ha un valore di id vuol dire che è in modifica
        /// se il valore è zero vuol dire che è un inserimento di nuova gestione
        /// </summary>
        /// <param name="sender">la cella di descrizione</param>
        /// <param name="e">la conferma o meno della modifica</param>
        public void RowChanged(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {

                if (e.EditAction == DataGridEditAction.Commit)
                {
                    Firm = ((RegistryFirm)e.Row.Item);
                    if (Firm.IdFirm > 0)
                    {
                        _services.UpdateFirm(Firm);
                    }
                    else
                    {
                        _services.AddFirm(Firm.DescFirm);
                    }
                    FirmList = new ObservableCollection<RegistryFirm>(_services.GetRegistryFirmList());
                }
            }
            catch (Exception err)
            {
                if (err.Message != "'Sorting' non consentito durante una transazione AddNew o EditItem.")
                    MessageBox.Show("Errore nell'aggiornamento dei dati: " + err.Message);
                FirmList = new ObservableCollection<RegistryFirm>(_services.GetRegistryFirmList());
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
                    MessageBoxResult result = MessageBox.Show("Attenzione verrà elemininata la seguente azienda: " +
                        ((RegistryFirm)dg.SelectedItem).DescFirm, "DAF-C Gestione Mercato", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _services.DeleteFirm(((RegistryFirm)dg.SelectedItem).IdFirm);
                            FirmList = new ObservableCollection<RegistryFirm>(_services.GetRegistryFirmList());
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("Errore nell'eliminazione dell'Azienda: " + Environment.NewLine + err.Message);
                            e.Handled = true;
                        }
                    }
                    else
                        e.Handled = true;
                }
            }
        }

        public ObservableCollection<RegistryFirm> FirmList
        {
            get { return _FirmList; }
            set
            {
                _FirmList = value;
                NotifyPropertyChanged("FirmList");
            }
        }
        /// <summary>
        /// il modello della gestione
        /// </summary>
        public RegistryFirm Firm
        {
            get { return _firm; }
            set
            {
                if (value != null)
                {
                    _firm = value;
                    NotifyPropertyChanged("Firm");
                }
            }
        }
        /// <summary>
        /// Evento di chiusura della view Gestione gestioni
        /// </summary>
        /// <param name="param">La view che ha inviato l'evento</param>
        public void CloseMe(object param)
        {
            RegistryFirmView RFV = param as RegistryFirmView;
            DockPanel wp = RFV.Parent as DockPanel;
            wp.Children.Remove(RFV);
        }
    }
}

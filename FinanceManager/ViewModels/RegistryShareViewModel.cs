using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class RegistryShareViewModel : ViewModelBase
    {
        private IRegistryServices _services;
        public ICommand CloseMeCommand { get; set; }
        Predicate<object> _Filter;


        public RegistryShareViewModel(IRegistryServices services)
        {
            _services = services ?? throw new ArgumentNullException("RegistryLocationViewModel With No Services");
            _Filter = new Predicate<object>(Filter);
            ShareList = new ObservableCollection<RegistryShare>(services.GetRegistryShareList());
            ShareTypeList = new ObservableCollection<RegistryShareType>(services.GetRegistryShareTypeList());
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
                    Share = ((RegistryShare)e.Row.Item);

                    if (Share.IdShare > 0)
                    {
                        _services.UpdateShare(Share);
                    }
                    else
                    {
                        PropertyInfo[] properties = typeof(RegistryShare).GetProperties();
                        foreach (PropertyInfo pi in properties)
                        {
                            if ((pi.Name == "Desc_titolo" || pi.Name == "Isin") && pi.GetValue(Share) == null)
                            {
                                e.Cancel = true;
                                throw new Exception("Inserire tutti i valori prima di confermare cambiando riga.");
                            }
                            else if (pi.Name != "Id_titolo" && pi.GetValue(Share).ToString() == "0")
                            {
                                e.Cancel = true;
                                throw new Exception("Inserire tutti i valori prima di confermare cambiando riga.");
                            }
                        }

                        _services.AddShare(Share);
                        ShareList = new ObservableCollection<RegistryShare>(_services.GetRegistryShareList());

                    }
                }
            }
            catch (Exception err)
            {
                if (err.Message != "'Sorting' non consentito durante una transazione AddNew o EditItem.")
                    MessageBox.Show("Errore nell'aggiornamento dei dati: " + err.Message, "DAF-C Gestione Titoli", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (dg.SelectedIndex >= 0)
                {
                    MessageBoxResult result = MessageBox.Show("Attenzione verrà elemininata il seguente titolo: " +
                        ((RegistryShare)dg.SelectedItem).DescShare + " - " + ((RegistryShare)dg.SelectedItem).Isin,
                        "DAF-C Gestione Mercato", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _services.DeleteShare(((RegistryShare)dg.SelectedItem).IdShare);
                            ShareList = new ObservableCollection<RegistryShare>(_services.GetRegistryShareList());
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("Errore nell'eliminazione del Titolo: " + Environment.NewLine + err.Message);
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
            get { return GetValue(() => FirmList); }
            set { SetValue(() => FirmList, value); }
        }

        public string SrchShares
        {
            get { return GetValue(() => SrchShares); }
            set
            {
                SetValue(() => SrchShares, value);
                SharesListView.Filter = _Filter;
                SharesListView.Refresh();

            }
        }

        public bool Filter(object obj)
        {
            if (obj != null)
            {
                if (obj.GetType() == typeof(RegistryShare))
                {
                    var data = obj as RegistryShare;
                    if (!string.IsNullOrEmpty(SrchShares))
                        return data.Isin.ToUpper().Contains(SrchShares.ToUpper());
                }
            }
            return true;
        }

        public ObservableCollection<RegistryShareType> ShareTypeList
        {
            get { return GetValue(() => ShareTypeList); }
            set { SetValue(() => ShareTypeList, value); }
        }

        public ObservableCollection<RegistryShare> ShareList
        {
            get { return GetValue(() => ShareList); }
            private set
            {
                SetValue(() => ShareList, value);
                SharesListView = CollectionViewSource.GetDefaultView(value);
                //SharesListView = new ListCollectionView(value);
            }
        }

        public ICollectionView SharesListView
        {
            get { return GetValue(() => SharesListView); }
            set { SetValue(() => SharesListView, value); }
        }

        /// <summary>
        /// il modello della gestione
        /// </summary>
        public RegistryShare Share
        {
            get { return GetValue(() => Share); }
            set { SetValue(() => Share, value); }
        }
        /// <summary>
        /// Evento di chiusura della view Gestione gestioni
        /// </summary>
        /// <param name="param">La view che ha inviato l'evento</param>
        public void CloseMe(object param)
        {
            RegistryShareView RSV = param as RegistryShareView;
            DockPanel wp = RSV.Parent as DockPanel;
            wp.Children.Remove(RSV);
        }
    }
}

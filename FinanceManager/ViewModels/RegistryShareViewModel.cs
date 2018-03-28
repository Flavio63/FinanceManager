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
    public class RegistryShareViewModel
    {
        private IRegistryServices _services;
        private RegistryShare _share;
        private ObservableCollection<RegistryShare> _ShareList;
        private ObservableCollection<RegistryShareType> _ShareTypeList;
        public ICommand CloseMeCommand { get; set; }

        public RegistryShareViewModel(IRegistryServices services)
        {
            _services = services ?? throw new ArgumentNullException("RegistryLocationViewModel With No Services");
            ShareList = new ObservableCollection<RegistryShare>(services.GetRegistryShareList());
            ShareList.CollectionChanged += CollectionHasChanged;
            ShareTypeList = new ObservableCollection<RegistryShareType>(services.GetRegistryShareTypeList());
            ShareList.CollectionChanged += CollectionHasChanged;
            CloseMeCommand = new CommandHandler(CloseMe);
        }

        public void CollectionHasChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //ListCollectionView ownerList = sender as ListCollectionView;
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
                    Share = ((RegistryShare)e.Row.Item);
                    if (Share.IdShare > 0)
                    {
                        _services.UpdateShare(Share);
                    }
                    else
                    {
                        _services.AddShare(Share);
                        ShareList = new ObservableCollection<RegistryShare>(_services.GetRegistryShareList());

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

        public ObservableCollection<RegistryShareType> ShareTypeList
        {
            get { return _ShareTypeList; }
            set
            {
                _ShareTypeList = value;
                NotifyPropertyChanged("ShareTypeList");
            }
        }

        public ObservableCollection<RegistryShare> ShareList
        {
            get { return _ShareList; }
            private set
            {
                _ShareList = value;
                NotifyPropertyChanged("ShareList");
            }
        }
        /// <summary>
        /// il modello della gestione
        /// </summary>
        public RegistryShare Share
        {
            get { return _share; }
            set
            {
                if (value != null)
                {
                    _share = value;
                    NotifyPropertyChanged("Share");
                }
            }
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


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

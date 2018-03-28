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
using System.Windows.Data;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class RegistryLocationViewModel : INotifyPropertyChanged
    {
        private IRegistryServices _services;
        private RegistryLocation _location;
        private ObservableCollection<RegistryLocation> _LocationList;
        public ICommand CloseMeCommand { get; set; }

        public RegistryLocationViewModel(IRegistryServices services)
        {
            _services = services ?? throw new ArgumentNullException("RegistryLocationViewModel With No Services");
            LocationList = new ObservableCollection<RegistryLocation>(services.GetRegistryLocationList());
            LocationList.CollectionChanged += CollectionHasChanged;
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
                    Location = ((RegistryLocation)e.Row.Item);
                    if (Location.IdLocation > 0)
                    {
                        _services.UpdateLocation(Location);
                    }
                    else
                    {
                        _services.AddLocation(Location.DescLocation);
                        LocationList = new ObservableCollection<RegistryLocation>(_services.GetRegistryLocationList());

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
                    MessageBoxResult result = MessageBox.Show("Attenzione verrà elemininata la location: " +
                        ((RegistryLocation)dg.SelectedItem).DescLocation, "DAF-C Gestione Location", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _services.DeleteLocation(((RegistryLocation)dg.SelectedItem).IdLocation);
                            LocationList = new ObservableCollection<RegistryLocation>(_services.GetRegistryLocationList());
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("Errore nell'eliminazione della location: " + Environment.NewLine + err.Message);
                            e.Handled = true;
                        }
                    }
                    else
                        e.Handled = true;
                }
            }
        }

        public ObservableCollection<RegistryLocation> LocationList
        {
            get { return _LocationList; }
            private set
            {
                _LocationList = value;
                NotifyPropertyChanged("LocationList");
            }
        }
        /// <summary>
        /// il modello della gestione
        /// </summary>
        public RegistryLocation Location
        {
            get { return _location; }
            set
            {
                if (value != null)
                {
                    _location = value;
                    NotifyPropertyChanged("Location");
                }
            }
        }
        /// <summary>
        /// Evento di chiusura della view Gestione gestioni
        /// </summary>
        /// <param name="param">La view che ha inviato l'evento</param>
        public void CloseMe(object param)
        {
            RegistryLocationView RLV = param as RegistryLocationView;
            DockPanel wp = RLV.Parent as DockPanel;
            wp.Children.Remove(RLV);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

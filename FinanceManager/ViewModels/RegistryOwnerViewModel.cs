using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Events;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;
using FinanceManager.Views;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace FinanceManager.ViewModels
{
    public class RegistryOwnerViewModel : INotifyPropertyChanged
    {
        private IRegistryServices _services;
        private RegistryOwner owner;
        private ObservableCollection<RegistryOwner> _ownerList;
        public ICommand CloseMeCommand { get; set; }

        /// <summary>
        /// costruttore
        /// </summary>
        /// <param name="services">la gestione dei dati verso il database</param>
        public RegistryOwnerViewModel(IRegistryServices services)
        {
            _services = services ?? throw new ArgumentNullException("RegistryOwnerViewModel With No Services");
            OwnerList = new ObservableCollection<RegistryOwner>(services.GetRegistryOwners());
            OwnerList.CollectionChanged += CollectionHasChanged;
            CloseMeCommand = new CommandHandler(CloseMe);
        }

        public void CollectionHasChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ListCollectionView ownerList = sender as ListCollectionView;
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
                    Owner = ((RegistryOwner)e.Row.Item);
                    if (Owner.Id_gestione > 0)
                    {
                        _services.UpdateOwner(Owner);
                    }
                    else
                    {
                        _services.AddOwner(Owner.Nome_Gestione);
                        OwnerList = new ObservableCollection<RegistryOwner>(_services.GetRegistryOwners());
                        
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
                    MessageBoxResult result = MessageBox.Show("Attenzione verrà elemininata la gestione: " +
                        ((RegistryOwner)dg.SelectedItem).Nome_Gestione, "DAF-C Gestione Gestioni", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _services.DeleteOwner(((RegistryOwner)dg.SelectedItem).Id_gestione);
                            OwnerList = new ObservableCollection<RegistryOwner>(_services.GetRegistryOwners());
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

        public ObservableCollection<RegistryOwner> OwnerList
        {
            get { return _ownerList; }
            private set
            {
                _ownerList = value;
                NotifyPropertyChanged("OwnerList");
            }
        }
        /// <summary>
        /// il modello della gestione
        /// </summary>
        public RegistryOwner Owner
        {
            get { return owner; }
            set
            {
                if (value != null)
                {
                    owner = value;
                    NotifyPropertyChanged("Owner");
                }
            }
        }
        /// <summary>
        /// Evento di chiusura della view Gestione gestioni
        /// </summary>
        /// <param name="param">La view che ha inviato l'evento</param>
        public void CloseMe(object param)
        {
            RegistryOwnerView ROV = param as RegistryOwnerView;
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

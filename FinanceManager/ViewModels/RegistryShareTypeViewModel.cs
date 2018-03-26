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
    public class RegistryShareTypeViewModel : INotifyPropertyChanged
    {
        private IRegistryServices _services;
        private RegistryShareType registryShareType;
        private ObservableCollection<RegistryShareType> _ShareTypeList;
        public ICommand CloseMeCommand { get; set; }

        public RegistryShareTypeViewModel(IRegistryServices services)
        {
            _services = services ?? throw new ArgumentNullException("RegistryShareTypeViewModel With No Services");
            ShareTypeList = new ObservableCollection<RegistryShareType>(_services.GetRegistryShareTypeList());
            ShareTypeList.CollectionChanged += CollectionHasChanged;
            CloseMeCommand = new CommandHandler(CloseMe);
        }

        public void CollectionHasChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ListCollectionView shareTypeList = sender as ListCollectionView;
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
                    registryShareType = ((RegistryShareType)e.Row.Item);
                    if (registryShareType.IdShareType > 0)
                    {
                        _services.UpdateShareType(registryShareType);
                    }
                    else
                    {
                        _services.AddShareType(registryShareType.TypeName);
                        _ShareTypeList = new ObservableCollection<RegistryShareType>(_services.GetRegistryShareTypeList());

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
                    MessageBoxResult result = MessageBox.Show("Attenzione verrà elemininata la seguente tipologia: " +
                        ((RegistryShareType)dg.SelectedItem).TypeName, "DAF-C Gestione Gestioni", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _services.DeleteShareType(((RegistryShareType)dg.SelectedItem).IdShareType);
                            ShareTypeList = new ObservableCollection<RegistryShareType>(_services.GetRegistryShareTypeList());
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

        public ObservableCollection<RegistryShareType> ShareTypeList
        {
            get { return _ShareTypeList; }
            private set
            {
                _ShareTypeList = value;
                NotifyPropertyChanged("ShareTypeList");
            }
        }

        public RegistryShareType RegistryShareType
        {
            get { return registryShareType; }
            set
            {
                registryShareType = value;
                NotifyPropertyChanged("RegistryShareType");
            }
        }
        
        /// <summary>
        /// Evento di chiusura della view Gestione gestioni
        /// </summary>
        /// <param name="param">La view che ha inviato l'evento</param>
        public void CloseMe(object param)
        {
            RegistryShareTypeView RSTV = param as RegistryShareTypeView;
            WrapPanel wp = RSTV.Parent as WrapPanel;
            wp.Children.Remove(RSTV);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

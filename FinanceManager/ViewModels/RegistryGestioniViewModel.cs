using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace FinanceManager.ViewModels
{
    public class RegistryGestioniViewModel : INotifyPropertyChanged
    {
        IRegistryServices _services;
        private RegistryGestioni gestioni;
        private ObservableCollection<RegistryGestioni> _gestioniList;
        public ICommand CloseMeCommand { get; set; }

        public RegistryGestioniViewModel(IRegistryServices service)
        {
            _services = service ?? throw new ArgumentNullException("RegistryGestioniModel With No Services");
            try
            {
                GestioniList = new ObservableCollection<RegistryGestioni>(service.GetGestioneList());
                GestioniList.CollectionChanged += CollectionHasChanged;
            }
            catch (Exception err)
            {
                MessageBox.Show("Errore nella richiesta dei dati." + Environment.NewLine + err.Message, "DAF-C Lista Gestioni");
            }
            CloseMeCommand = new CommandHandler(CloseMe);
        }

        public void CollectionHasChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
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
                    Gestioni = ((RegistryGestioni)e.Row.Item);
                    if (Gestioni.Id_Gestione > 0)
                    {
                        _services.UpdateGestioneName(Gestioni);
                    }
                    else
                    {
                        if (Gestioni.Nome_Gestione != null)
                        {
                            _services.AddGestione(Gestioni);
                            GestioniList = new ObservableCollection<RegistryGestioni>(_services.GetGestioneList());
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
                    MessageBoxResult result = MessageBox.Show("Attenzione verrà elemininata la seguente gestione: " +
                        ((RegistryGestioni)dg.SelectedItem).Nome_Gestione, "DAF-C Gestione Gestioni", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _services.DeleteGestione(((RegistryGestioni)dg.SelectedItem).Id_Gestione);
                            GestioniList = new ObservableCollection<RegistryGestioni>(_services.GetGestioneList());
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

        public ObservableCollection<RegistryGestioni> GestioniList
        {
            get { return _gestioniList; }
            set
            {
                _gestioniList = value;
                NotifyPropertyChanged("GestioniList");
            }
        }

        public RegistryGestioni Gestioni
        {
            get { return gestioni; }
            set
            {
                gestioni = value;
                NotifyPropertyChanged("Gestioni");
            }
        }
        /// <summary>
        /// Evento di chiusura della view Gestione gestioni
        /// </summary>
        /// <param name="param">La view che ha inviato l'evento</param>
        public void CloseMe(object param)
        {
            RegistryGestioniView ROV = param as RegistryGestioniView;
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

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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace FinanceManager.ViewModels
{
    public class RegistryContoCorrenteViewModel : INotifyPropertyChanged
    {
        IRegistryServices _services;
        private RegistryLocation contoCorrente;
        private ObservableCollection<RegistryLocation> _contoCorrenteList;
        public ICommand CloseMeCommand { get; set; }

        public RegistryContoCorrenteViewModel(IRegistryServices service)
        {
            _services = service ?? throw new ArgumentNullException("Registry ContoCorrente Model With No Services");
            try
            {
                ContoCorrenteList = new ObservableCollection<RegistryLocation>(service.GetRegistryLocationList());
                ContoCorrenteList.CollectionChanged += CollectionHasChanged;
            }
            catch (Exception err)
            {
                MessageBox.Show("Errore nella richiesta dei dati." + Environment.NewLine + err.Message, "DAF-C Lista Conti Corrente");
            }
            CloseMeCommand = new CommandHandler(CloseMe);
        }

        public void CollectionHasChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //ListCollectionView _contoCorrenteList = sender as ListCollectionView;
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
                    ContoCorrente = ((RegistryLocation)e.Row.Item);
                    if (ContoCorrente.Id_Conto > 0)
                    {
                        _services.UpdateLocation(ContoCorrente);
                    }
                    else
                    {
                        if (ContoCorrente.Desc_Conto != null && ContoCorrente.Note != null)
                        {
                            _services.AddLocation(ContoCorrente.Desc_Conto, ContoCorrente.Note);
                            ContoCorrenteList = new ObservableCollection<RegistryLocation>(_services.GetRegistryLocationList());
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
                    MessageBoxResult result = MessageBox.Show("Attenzione verrà elemininata il seguente conto corrente: " +
                        ((RegistryLocation)dg.SelectedItem).Desc_Conto, "DAF-C Gestione Conti Corrente", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _services.DeleteLocation(((RegistryLocation)dg.SelectedItem).Id_Conto);
                            ContoCorrenteList = new ObservableCollection<RegistryLocation>(_services.GetRegistryLocationList());
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("Errore nell'eliminazione del conto corrente: " + Environment.NewLine + err.Message);
                            e.Handled = true;
                        }
                    }
                    else
                        e.Handled = true;
                }
            }
        }

        public ObservableCollection<RegistryLocation> ContoCorrenteList
        {
            get { return _contoCorrenteList; }
            set
            {
                _contoCorrenteList = value;
                NotifyPropertyChanged("ContoCorrenteList");
            }
        }

        public RegistryLocation ContoCorrente
        {
            get { return contoCorrente; }
            set
            {
                contoCorrente = value;
                NotifyPropertyChanged("ContoCorrente");
            }
        }
        /// <summary>
        /// Evento di chiusura della view Gestione gestioni
        /// </summary>
        /// <param name="param">La view che ha inviato l'evento</param>
        public void CloseMe(object param)
        {
            RegistryContoCorrenteView ROV = param as RegistryContoCorrenteView;
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

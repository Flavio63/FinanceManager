using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class RegistryMovementTypeViewModel : ViewModelBase
    {
        private IRegistryServices _services;
        private RegistryMovementType _movementType;
        private ObservableCollection<RegistryMovementType> _MovementTypeList;
        public ICommand CloseMeCommand { get; set; }

        public RegistryMovementTypeViewModel(IRegistryServices services)
        {
            _services = services ?? throw new ArgumentNullException("RegistryMovementTypeViewModel With No Services");
            MovementTypeList = new ObservableCollection<RegistryMovementType>(services.GetRegistryMovementTypesList());
            CloseMeCommand = new CommandHandler(CloseMe);
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
                    MovementType = ((RegistryMovementType)e.Row.Item);
                    if (MovementType.IdMovement > 0)
                    {
                        _services.UpdateMovementType(MovementType);
                    }
                    else
                    {
                        _services.AddMovementType(MovementType.DescMovement);
                        MovementTypeList = new ObservableCollection<RegistryMovementType>(_services.GetRegistryMovementTypesList());

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
            DataGrid tmp = e.OriginalSource as DataGrid;
            if (e.Key == Key.Delete && tmp != null)
            {
                DataGrid dg = sender as DataGrid;
                if (dg.SelectedIndex > 0)
                {
                    MessageBoxResult result = MessageBox.Show("Attenzione verrà elemininato la seguente tipologia: " +
                        ((RegistryMovementType)dg.SelectedItem).DescMovement, "DAF-C Gestione Movimenti", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _services.DeleteMovementType(((RegistryMovementType)dg.SelectedItem).IdMovement);
                            MovementTypeList = new ObservableCollection<RegistryMovementType>(_services.GetRegistryMovementTypesList());
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("Errore nell'eliminazione della tipologia di movimento: " + Environment.NewLine + err.Message);
                            e.Handled = true;
                        }
                    }
                    else
                        e.Handled = true;
                }
            }
        }

        public ObservableCollection<RegistryMovementType> MovementTypeList
        {
            get { return _MovementTypeList; }
            private set
            {
                _MovementTypeList = value;
                NotifyPropertyChanged("MovementTypeList");
            }
        }
        /// <summary>
        /// il modello della gestione
        /// </summary>
        public RegistryMovementType MovementType
        {
            get { return _movementType; }
            set
            {
                if (value != null)
                {
                    _movementType = value;
                    NotifyPropertyChanged("MovementType");
                }
            }
        }

        /// <summary>
        /// Evento di chiusura della view Gestione gestioni
        /// </summary>
        /// <param name="param">La view che ha inviato l'evento</param>
        public void CloseMe(object param)
        {
            RegistryMovementTypeView RFV = param as RegistryMovementTypeView;
            DockPanel wp = RFV.Parent as DockPanel;
            wp.Children.Remove(RFV);
        }
    }
}

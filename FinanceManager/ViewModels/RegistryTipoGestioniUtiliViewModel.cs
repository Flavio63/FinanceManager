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
    public class RegistryTipoGestioniUtiliViewModel : INotifyPropertyChanged
    {
        IRegistryServices _services;
        private RegistryTipoGestioniUtili tipoGestioniUtili;
        private ObservableCollection<RegistryTipoGestioniUtili> _tipoGestioniUtiliList;
        public ICommand CloseMeCommand { get; set; }

        public RegistryTipoGestioniUtiliViewModel(IRegistryServices service)
        {
            _services = service ?? throw new ArgumentNullException("RegistryTipoGestioniUtili With No Services");
            try
            {
                TipoGestioniUtiliList = new ObservableCollection<RegistryTipoGestioniUtili>(service.GetTipoGestioniUtiliList());
                TipoGestioniUtiliList.CollectionChanged += CollectionHasChanged;
            }
            catch (Exception err)
            {
                MessageBox.Show("Errore nella richiesta dei dati." + Environment.NewLine + err.Message, "DAF-C Lista Tipo Gestione Utili");
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
                    tipoGestioniUtili = ((RegistryTipoGestioniUtili)e.Row.Item);
                    if (tipoGestioniUtili.Id_TipoGestioneUtili > 0)
                    {
                        _services.UpdateTipoGestioniUtili(tipoGestioniUtili);
                    }
                    else
                    {
                        if (tipoGestioniUtili.DescrizioneGestioneUtili != null )
                        {
                            _services.InsertTipoGestioniUtili(tipoGestioniUtili);
                            TipoGestioniUtiliList = new ObservableCollection<RegistryTipoGestioniUtili>(_services.GetTipoGestioniUtiliList());
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
                    MessageBoxResult result = MessageBox.Show("Attenzione verrà elemininata la seguente tipologia quote utili: " +
                        ((RegistryTipoGestioniUtili)dg.SelectedItem).DescrizioneGestioneUtili, "DAF-C Gestione Tipo Utili", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _services.DeleteTipoGestioniUtili(((RegistryTipoGestioniUtili)dg.SelectedItem).Id_TipoGestioneUtili);
                            TipoGestioniUtiliList = new ObservableCollection<RegistryTipoGestioniUtili>(_services.GetTipoGestioniUtiliList());
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("Errore nell'eliminazione della tipologia gestione utili: " + Environment.NewLine + err.Message);
                            e.Handled = true;
                        }
                    }
                    else
                        e.Handled = true;
                }
            }
        }

        public ObservableCollection<RegistryTipoGestioniUtili> TipoGestioniUtiliList
        {
            get { return _tipoGestioniUtiliList; }
            set
            {
                _tipoGestioniUtiliList = value;
                NotifyPropertyChanged("TipoGestioniUtiliList");
            }
        }

        public RegistryTipoGestioniUtili TipoGestioniUtili
        {
            get { return tipoGestioniUtili; }
            set
            {
                tipoGestioniUtili = value;
                NotifyPropertyChanged("TipoGestioniUtiliList");
            }
        }
        /// <summary>
        /// Evento di chiusura della view Gestione gestioni
        /// </summary>
        /// <param name="param">La view che ha inviato l'evento</param>
        public void CloseMe(object param)
        {
            RegistryTipoGestioniUtiliView ROV = param as RegistryTipoGestioniUtiliView;
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

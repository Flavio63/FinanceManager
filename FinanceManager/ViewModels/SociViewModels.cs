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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace FinanceManager.ViewModels
{
    public class SociViewModels : INotifyPropertyChanged
    {
        private IRegistryServices _services;
        private Soci _Socio;
        private ObservableCollection<Soci> _sociList;
        public ICommand CloseMeCommand { get; set; }

        /// <summary>
        /// costruttore
        /// </summary>
        /// <param name="services">la gestione dei dati verso il database</param>
        public SociViewModels(IRegistryServices services)
        {
            _services = services ?? throw new ArgumentNullException("SociViewModel With No Services");
            try
            {
                ListaSoci = new ObservableCollection<Soci>(services.GetSociList());
                ListaSoci.CollectionChanged += CollectionHasChanged;
            }
            catch (Exception err)
            {
                MessageBox.Show("Errore nella richiesta dei dati." + Environment.NewLine + err.Message, "DAF-C Lista Socii");
            }
            CloseMeCommand = new CommandHandler(CloseMe);
        }

        public void CollectionHasChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ListCollectionView listaSoci = sender as ListCollectionView;
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
                    Socio = ((Soci)e.Row.Item);
                    if (Socio.Id_Socio > 0)
                    {
                        _services.UpdateSocioName(Socio);
                    }
                    else if (Socio.Nome_Socio != null )
                    {
                        _services.AddSocio(Socio);
                        ListaSoci = new ObservableCollection<Soci>(_services.GetSociList());
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
                    MessageBoxResult result = MessageBox.Show("Attenzione verrà elemininata il socio: " +
                        ((Soci)dg.SelectedItem).Nome_Socio, "DAF-C Gestione Soci", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _services.DeleteSocio(((Soci)dg.SelectedItem).Id_Socio);
                            ListaSoci = new ObservableCollection<Soci>(_services.GetSociList());
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


        public ObservableCollection<Soci> ListaSoci
        {
            get { return _sociList; }
            private set
            {
                _sociList = value;
                NotifyPropertyChanged("ListaSoci");
            }
        }
        /// <summary>
        /// il modello della gestione
        /// </summary>
        public Soci Socio
        {
            get { return _Socio; }
            set
            {
                if (value != null)
                {
                    _Socio = value;
                    NotifyPropertyChanged("Socio");
                }
            }
        }
        /// <summary>
        /// Evento di chiusura della view Gestione gestioni
        /// </summary>
        /// <param name="param">La view che ha inviato l'evento</param>
        public void CloseMe(object param)
        {
            SociView ROV = param as SociView;
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

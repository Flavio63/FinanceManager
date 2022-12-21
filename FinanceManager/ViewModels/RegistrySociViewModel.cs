using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using Renci.SshNet.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace FinanceManager.ViewModels
{
    internal class RegistrySociViewModel : INotifyPropertyChanged
    {
        IRegistryServices _services;
        private RegistrySoci soci;
        private ObservableCollection<RegistrySoci> _registrySoci;

        public ICommand CloseMeCommand { get; set; }

        public RegistrySociViewModel(IRegistryServices services) 
        { 
            _services = services ?? throw new ArgumentNullException("Registry Socio Model with NO services!");
            try
            {
                SociList = new ObservableCollection<RegistrySoci>(services.GetSociList());
                SociList.CollectionChanged += CollectionHasChanged;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Errore nella richiesta dei dati." + Environment.NewLine + ex.Message, "DAF-C Lista Socio");
            }
            CloseMeCommand = new CommandHandler(CloseMe);
        }

        public ObservableCollection<RegistrySoci> SociList
        {
            get { return _registrySoci; }
            set { _registrySoci = value;  NotifyPropertyChanged("SociList"); }
        }
        public RegistrySoci Socio
        {
            get { return soci; }
            set { soci = value; NotifyPropertyChanged("Socio"); }
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
                    Socio = ((RegistrySoci)e.Row.Item);
                    if (Socio.Id_Socio > 0)
                    {
                        _services.UpdateSocioName(Socio);
                    }
                    else
                    {
                            _services.AddSocio(Socio);
                            SociList = new ObservableCollection<RegistrySoci>(_services.GetSociList());
                    }
                }
            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show("Errore nell'aggiornamento dei dati: " + err.Message);
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
                    MessageBoxResult result = MessageBox.Show("Attenzione verrà elemininata il seguente socio: " +
                        ((RegistrySoci)dg.SelectedItem).Nome_Socio, "DAF-C Gestione Socio", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _services.DeleteSocio(((RegistrySoci)dg.SelectedItem).Id_Socio);
                            SociList = new ObservableCollection<RegistrySoci>(_services.GetSociList());
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("Errore nell'eliminazione del socio " + Environment.NewLine + err.Message);
                            e.Handled = true;
                        }
                    }
                    else
                        e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Evento di chiusura della view Gestione gestioni
        /// </summary>
        /// <param name="param">La view che ha inviato l'evento</param>
        public void CloseMe(object param)
        {
            RegistrySociView ROV = param as RegistrySociView;
            DockPanel wp = ROV.Parent as DockPanel;
            wp.Children.Remove(ROV);
        }


        public void CollectionHasChanged(object sender, NotifyCollectionChangedEventArgs e) { }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

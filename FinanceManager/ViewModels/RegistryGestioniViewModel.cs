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
    public class RegistryGestioniViewModel : ViewModelBase
    {
        IRegistryServices _services;
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand EraseCommand { get; set; }
        public ICommand CloseMeCommand { get; set; }

        public RegistryGestioniViewModel(IRegistryServices services)
        {
            _services = services ?? throw new ArgumentNullException("RegistryGestioniModel With No Services");
            try
            {
                InsertCommand = new CommandHandler(SaveCommand, CanSave);
                ModifyCommand = new CommandHandler(UpdateCommand, CanModify);
                EraseCommand = new CommandHandler(DeleteCommand, CanModify);
                // popolo le liste
                init();
                TipoGestioniUtiliList = _services.GetTipoGestioniUtiliList();
            }
            catch (Exception err)
            {
                MessageBox.Show("Errore nella richiesta dei dati." + Environment.NewLine + err.Message, "Gestione Soci", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            CloseMeCommand = new CommandHandler(CloseMe);
        }

        private void init()
        {
            TipoGestioniUtili = new RegistryTipoGestioniUtili();
            Gestione = new RegistryGestioni();
            GestioniList = _services.GetGestioneList();
            Nome_Gestione = string.Empty;
        }

        public RegistryGestioniList GestioniList
        {
            get { return GetValue(() => GestioniList); }
            set { SetValue(() => GestioniList, value); }
        }

        public RegistryGestioni Gestione
        {
            get { return GetValue(() => Gestione); }
            set { SetValue(() => Gestione, value); }
        }

        public RegistryTipoGestioniUtiliList TipoGestioniUtiliList
        {
            get { return GetValue(() => TipoGestioniUtiliList); }
            set { SetValue(() => TipoGestioniUtiliList, value); }
        }

        public RegistryTipoGestioniUtili TipoGestioniUtili
        {
            get { return GetValue(() => TipoGestioniUtili); }
            set { SetValue(() => TipoGestioniUtili, value); }
        }

        public string Nome_Gestione
        {
            get { return GetValue(() => Nome_Gestione); }
            set { SetValue(() => Nome_Gestione, value); Gestione.Nome_Gestione = value; }
        }
        /// <summary>
        /// Gestore dell'evento nei combo box dei parametri comuni
        /// </summary>
        /// <param name="sender">Combo Box</param>
        /// <param name="e">Cambio scelta item</param>
        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is RegistryTipoGestioniUtili RTGU)
                {
                    TipoGestioniUtili.Id_tipo_gestione = RTGU.Id_tipo_gestione;
                    TipoGestioniUtili.Tipo_Gestione = RTGU.Tipo_Gestione;
                }
            }
        }
        /// <summary>
        /// Imposto i campi sopra la griglia quando viene selezionata una riga
        /// </summary>
        /// <param name="sender">Grid dei dati</param>
        /// <param name="e">Cambio di selezione</param>
        public void GridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is RegistryGestioni RG)
                {
                    Gestione = RG;
                    Nome_Gestione = RG.Nome_Gestione;
                    TipoGestioniUtili.Id_tipo_gestione = RG.Id_tipo_gestione;
                    TipoGestioniUtili.Tipo_Gestione = RG.Tipo_Gestione;
                }
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
        public void SaveCommand(object param)
        {
            try
            {
                _services.AddGestione(Gestione);
                System.Windows.MessageBox.Show("Aggiornamento effettuato", "Gestione Gestioni", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show("Errore nell'aggiornamento dei dati: " + err.Message, "Gestione Gestioni", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            init();
        }
        private void UpdateCommand(object param)
        {
            try
            {
                _services.UpdateGestioneName(Gestione);
                System.Windows.MessageBox.Show("Aggiornamento effettuato", "Gestione Gestioni", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show("Errore nell'aggiornamento dei dati: " + err.Message, "Gestione Gestioni", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            init();
        }
        private void DeleteCommand(object param)
        {
            try
            {
                _services.DeleteGestione(Gestione.Id_Gestione);
                System.Windows.MessageBox.Show("Aggiornamento effettuato", "Gestione Gestioni", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show("Errore nell'aggiornamento dei dati: " + err.Message, "Gestione Gestioni", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            init();
        }

        public bool CanSave(object param)
        {
            if (Gestione.Id_Gestione == 0 && !String.IsNullOrEmpty(Nome_Gestione) && TipoGestioniUtili.Id_tipo_gestione != 0)
                return true;
            return false;
        }

        public bool CanModify (object param) 
        {
            if (Gestione.Id_Gestione > 0)
                return true;
            return false;
        }
    }
}

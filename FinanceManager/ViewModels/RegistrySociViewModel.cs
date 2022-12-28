using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using NPOI.SS.Formula.Functions;
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
    internal class RegistrySociViewModel : ViewModelBase
    {
        IRegistryServices _services;
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand EraseCommand { get; set; }

        public ICommand CloseMeCommand { get; set; }

        public RegistrySociViewModel(IRegistryServices services)
        {
            _services = services ?? throw new ArgumentNullException("Registry Socio Model with NO services!");
            try
            {
                InsertCommand = new CommandHandler(SaveCommand, CanSave);
                ModifyCommand = new CommandHandler(UpdateCommand, CanModify);
                EraseCommand = new CommandHandler(DeleteCommand, CanModify);
                // popolo le liste
                init();
                TipoGestioniUtiliList = services.GetTipoGestioniUtiliList();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Errore nella richiesta dei dati." + Environment.NewLine + ex.Message, "DAF-C Lista Socio");
            }
            CloseMeCommand = new CommandHandler(CloseMe);
        }

        private void init()
        {
            Socio = new RegistrySoci();
            TipoGestioniUtili = new RegistryTipoGestioniUtili();
            SociList = _services.GetSociList();
            Nome_Socio = string.Empty;
        }

        public RegistrySociList SociList
        {
            get { return GetValue(() => SociList); }
            set { SetValue(() => SociList, value); }
        }
        public RegistrySoci Socio
        {
            get { return GetValue(() => Socio); }
            set { SetValue(() => Socio, value); }
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

        public string Nome_Socio
        {
            get { return GetValue(() => Nome_Socio); }
            set { SetValue(() => Nome_Socio, value); Socio.Nome_Socio = value; }
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
                if (e.AddedItems[0] is RegistrySoci RS)
                {
                    Socio = RS;
                    Nome_Socio = RS.Nome_Socio;
                    TipoGestioniUtili.Id_tipo_gestione = RS.Id_tipo_gestione;
                    TipoGestioniUtili.Tipo_Gestione = RS.Tipo_Gestione;
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

        public void SaveCommand(object param)
        {
            try
            {
                _services.AddSocio(Socio);
                System.Windows.MessageBox.Show("Aggiornamento effettuato", "Gestione Soci", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show("Errore nell'aggiornamento dei dati: " + err.Message, "Gestione Soci", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            init();
        }
        private void UpdateCommand(object param)
        {
            try
            {
                _services.UpdateSocioName(Socio);
                System.Windows.MessageBox.Show("Aggiornamento effettuato", "Gestione Soci", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception err)
            {
                System.Windows.MessageBox.Show("Errore nell'aggiornamento dei dati: " + err.Message, "Gestione Soci", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            init();
        }
        private void DeleteCommand(object param)
        {
            try
            {
                _services.DeleteSocio(Socio.Id_Socio);
                System.Windows.MessageBox.Show("Aggiornamento effettuato", "Gestione Soci", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err )
            {
                System.Windows.MessageBox.Show("Errore nell'aggiornamento dei dati: " + err.Message, "Gestione Soci", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            init();
        }
        public bool CanSave(object param)
        {
            if (Socio.Id_Socio == 0 && !String.IsNullOrEmpty(Nome_Socio) && TipoGestioniUtili.Id_tipo_gestione != 0)
                return true;
            return false;
        }
        public bool CanModify(object param)
        {
            if (Socio.Id_Socio > 0)
                return true;
            return false;
        }

      
    }
}

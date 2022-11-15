using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using FinanceManager.Views;
using FinanceManager.Models.Enumeratori;

namespace FinanceManager.ViewModels
{
    public class ContiSociGestioniViewModel : ViewModelBase
    {
        private readonly IRegistryServices _registryServices;
        public ICommand CloseMeCommand { get; set; }

        public ContiSociGestioniViewModel (IRegistryServices registryServices)
        {
            _registryServices = registryServices ?? throw new ArgumentNullException("Manca collegamento con richiesta dati anagrafica Giroconto View Model");
            CloseMeCommand = new CommandHandler(CloseMe);
            init();
        }

        private void init()
        {
            registryLocations = _registryServices.GetRegistryLocationList();
            registryGestioni = _registryServices.GetGestioneList();
            registrySoci = _registryServices.GetSociList();
        }

        #region Get&Set
        #region DataGrid
        public RegistryLocationList registryLocations
        {
            get { return GetValue(()=>registryLocations); }
            set { SetValue(()=>registryLocations, value); }
        }
        public RegistryGestioniList registryGestioni
        {
            get { return GetValue(()=>registryGestioni); }
            set { SetValue(()=>registryGestioni, value); }
        }
        public RegistrySociList registrySoci
        {
            get { return GetValue(()=>registrySoci); }
            set { SetValue(()=>registrySoci, value); }
        }
        #endregion

        public RegistryLocation ContoCorrente
        {
            get { return GetValue(()=>ContoCorrente); }
            set { SetValue(()=>ContoCorrente, value); }
        }
        #endregion
        #region eventi
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
                if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is RegistryLocation RL)
                {
                    ContoCorrente = ((RegistryLocation)e.Row.Item);
                    if (ContoCorrente.Id_Conto > 0)
                    {
                        _registryServices.UpdateLocation(ContoCorrente);
                    }
                    else if (!string.IsNullOrEmpty(ContoCorrente.Desc_Conto) && !string.IsNullOrEmpty(ContoCorrente.Note))
                    {
                        _registryServices.AddLocation(ContoCorrente.Desc_Conto, ContoCorrente.Note);
                        registryLocations = _registryServices.GetRegistryLocationList();
                    }
                }
                else if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is RegistryGestioni RG)
                {
                    if (RG.Id_Gestione > 0)
                    {
                        _registryServices.UpdateGestioneName(RG);
                    }
                    else if (RG.Nome_Gestione != null && RG.Id_Tipo_Gestione > 0 && RG.Id_Conto > 1)
                    {
                        TipologiaGestione TG = (TipologiaGestione) RG.Id_Tipo_Gestione;
                        RG.Tipo_Gestione = TG.GetDisplayName();
                        _registryServices.AddGestione(RG);
                        registryGestioni = _registryServices.GetGestioneList();
                    }
                }
                else if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is RegistrySoci RS)
                {
                    if (RS.Id_Socio > 0)
                    {
                        _registryServices.UpdateSocioName(RS);
                    }
                    else if (string.IsNullOrEmpty(RS.Nome_Socio) && RS.Id_Conto > 0)
                    {
                        _registryServices.AddSocio(RS);
                        registrySoci = _registryServices.GetSociList();
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
                if (dg.SelectedIndex > 0 && dg.Name == "_conti")
                {
                    MessageBoxResult result = MessageBox.Show("Attenzione verrà elemininata la location: " +
                        ((RegistryLocation)dg.SelectedItem).Desc_Conto, "DAF-C Gestione Location", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _registryServices.DeleteLocation(((RegistryLocation)dg.SelectedItem).Id_Conto);
                            registryLocations = _registryServices.GetRegistryLocationList();
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("Errore nell'eliminazione della location: " + Environment.NewLine + err.Message);
                            e.Handled = true;
                        }
                    }
                }
                else if (dg.SelectedIndex > 0 && dg.Name == "_gestioni")
                {
                    MessageBoxResult result = MessageBox.Show("Attenzione verrà elemininata la gestione: " +
                        ((RegistryGestioni)dg.SelectedItem).Nome_Gestione, "DAF-C Gestione Location", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _registryServices.DeleteGestione(((RegistryGestioni)dg.SelectedItem).Id_Gestione);
                            registryGestioni = _registryServices.GetGestioneList();
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("Errore nell'eliminazione della gestione: " + Environment.NewLine + err.Message);
                            e.Handled = true;
                        }
                    }
                }
                else if (dg.SelectedIndex > 0 && dg.Name == "_soci")
                {
                    MessageBoxResult result = MessageBox.Show("Attenzione verrà elemininata il/la socio/a: " +
                        ((RegistrySoci)dg.SelectedItem).Nome_Socio, "DAF-C Gestione Location", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _registryServices.DeleteGestione(((RegistrySoci)dg.SelectedItem).Id_Socio);
                            registrySoci = _registryServices.GetSociList();
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("Errore nell'eliminazione del/la socio/a: " + Environment.NewLine + err.Message);
                            e.Handled = true;
                        }
                    }
                }
                e.Handled = true;
            }
        }
        /// <summary>
        /// Evento di chiusura della view Gestione gestioni
        /// </summary>
        /// <param name="param">La view che ha inviato l'evento</param>
        public void CloseMe(object param)
        {
            ContiSociGestioniView RLV = param as ContiSociGestioniView;
            DockPanel wp = RLV.Parent as DockPanel;
            wp.Children.Remove(RLV);
        }



        #endregion eventi
    }
}

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using FinanceManager.Events;
using FinanceManager.Services;
using FinanceManager.Views;
using FinanceManager.Models;
using System.Linq.Expressions;
using NPOI.SS.Formula.Functions;
using System.Security.Cryptography;

namespace FinanceManager.ViewModels
{
    public class GiroContoViewModel : ViewModelBase
    {
        private readonly IRegistryServices _registryServices;
        private readonly IContoCorrenteServices _contoCorrenteServices;

        public ICommand CloseMeCommand { get; set; }
        public ICommand ClearMeCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }

        public GiroContoViewModel
            (IRegistryServices registryServices, IManagerLiquidAssetServices managerLiquidServices, IContoCorrenteServices contoCorrenteServices)
        {
            _registryServices = registryServices ?? throw new ArgumentNullException("Manca collegamento con richiesta dati anagrafica Giroconto View Model");
            _contoCorrenteServices = contoCorrenteServices ?? throw new ArgumentNullException("Manca Concocorrente services");
            Init();
            ClearData();
        }

        private void Init()
        {
            CloseMeCommand = new CommandHandler(CloseMe);
            ClearMeCommand = new CommandHandler(ClearMe);
            InsertCommand = new CommandHandler(SaveCommand, CanSave);
            ModifyCommand = new CommandHandler(UpdateCommand, CanModify);
            ListValute = new RegistryCurrencyList();
            ListValute = _registryServices.GetRegistryCurrencyList();
            tipoSoldis = new TipoSoldiList();
            tipoSoldis = _registryServices.GetTipoSoldiList();
            //------------------- in doppio per i combo box Mittente e Ricevente ------------------------------
            ListGestioniMittente = new RegistryOwnersList();
            ListGestioniMittente = _registryServices.GetGestioneList();
            ListContoMittente = new RegistryLocationList();
            ListContoMittente = _registryServices.GetRegistryLocationList();
            ListGestioniRicevente = new RegistryOwnersList();
            ListGestioniRicevente = _registryServices.GetGestioneList();
            ListContoRicevente = new RegistryLocationList();
            ListContoRicevente = _registryServices.GetRegistryLocationList();
            //===================================================================================================
        }

        private void ClearData()
        {
            //------- I DATI DELLE GRIGLIE DI CONTI CORRENTI E CONTO INVESTITORE SOLO GIROCONTO ----------------
            ContoCorrenteList conti_Correnti = new ContoCorrenteList();
            conti_Correnti = _contoCorrenteServices.GetContoCorrenteList();
            var contos = from record in conti_Correnti where record.Id_tipo_movimento == 12 select record;
            ContiCorrenti = new ContoCorrenteList();
            foreach (ContoCorrente conto in contos)
            {
                if (conto.Id_tipo_movimento == 12)
                    ContiCorrenti.Add(conto);
            }

            //===================================================================================================
            ActualCCmittente = new ContoCorrente();
            ActualCCmittente.Id_tipo_movimento = 12;
            ActualCCmittente.DataMovimento = DateTime.Now;
            ActualCCricevente = new ContoCorrente();
            ActualCCricevente.Id_tipo_movimento = 12;
            ActualCCricevente.DataMovimento = DateTime.Now;

            TotaleMittenteConto = new ContoCorrenteList();
            TotaleRiceventeConto = new ContoCorrenteList();
            ContoMittente = true;
            GestioneMittente = false;
            ContoRicevente = false;
            GestioneRicevente = false;
            ValutaEnabled = false;
            IdContoMittente = 0;
            IdGestioneMittente = 0;
            IdContoRicevente = 0;
            IdGestioneRicevente = 0;
            IdValuta = 0;
            Valore = 0;
            IdTipoSoldi = 0;
        }

        #region Get_Set
        /// <summary>
        /// La lista dei conti correnti usata con filtro 
        /// sul movimento di giroconto
        /// </summary>
        public ContoCorrenteList ContiCorrenti
        {
            get { return GetValue(() => ContiCorrenti); }
            set { SetValue(() => ContiCorrenti, value); }
        }
        /// <summary>
        /// Sono i conti selezionati dalla lista conti correnti
        /// da usare nel caso di rollback delle modifiche
        /// </summary>
        public ContoCorrenteList ContiGemelli
        {
            get { return GetValue(() => ContiGemelli); }
            set { SetValue(() => ContiGemelli, value); }
        }
        /// <summary>
        /// E' il record per il giroconto mittente
        /// </summary>
        public ContoCorrente ActualCCmittente
        {
            get { return GetValue(() => ActualCCmittente); }
            set { SetValue(() => ActualCCmittente, value); }
        }
        /// <summary>
        /// E' il record per il giroconto ricevente
        /// </summary>
        public ContoCorrente ActualCCricevente
        {
            get { return GetValue(() => ActualCCricevente); }
            set { SetValue(() => ActualCCricevente, value); }
        }

        #region boolean per autorizzazioni a cascata
        /// <summary>
        /// Gestisce l'abilitazione fra investitore
        /// e conto / gestione nel campo "mittente"
        /// </summary>
        public bool ContoMittente
        {
            get { return GetValue(() => ContoMittente); }
            set { SetValue(() => ContoMittente, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione fra investitore
        /// e conto / gestione nel campo "ricevente"
        /// </summary>
        public bool ContoRicevente
        {
            get { return GetValue(() => ContoRicevente); }
            set { SetValue(() => ContoRicevente, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione fra investitore
        /// e conto / gestione nel campo "mittente"
        /// </summary>
        public bool GestioneMittente
        {
            get { return GetValue(() => GestioneMittente); }
            set { SetValue(() => GestioneMittente, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione fra investitore
        /// e conto / gestione nel campo "ricevente"
        /// </summary>
        public bool GestioneRicevente
        {
            get { return GetValue(() => GestioneRicevente); }
            set { SetValue(() => GestioneRicevente, value); }
        }


        public bool ValutaEnabled
        {
            get { return GetValue(() => ValutaEnabled); }
            set { SetValue(() => ValutaEnabled, value); }
        }

        #endregion

        #region filtri

        public int IdValuta
        {
            get { return GetValue(() => IdValuta); }
            set { SetValue(() => IdValuta, value); }
        }

        public int IdContoMittente
        {
            get { return GetValue(() => IdContoMittente); }
            set { SetValue(() => IdContoMittente, value); }
        }

        public int IdGestioneMittente
        {
            get { return GetValue(() => IdGestioneMittente); }
            set { SetValue(() => IdGestioneMittente, value); }
        }

        public int IdContoRicevente
        {
            get { return GetValue(() => IdContoRicevente); }
            set { SetValue(() => IdContoRicevente, value); }
        }

        public int IdGestioneRicevente
        {
            get { return GetValue(() => IdGestioneRicevente); }
            set { SetValue(() => IdGestioneRicevente, value); }
        }
        public int IdTipoSoldi
        {
            get { return GetValue(() => IdTipoSoldi); }
            set { SetValue(() => IdTipoSoldi, value); }
        }

        #endregion

        #region combobox

        /// <summary>
        /// E' la lista OwnerList da cui preleva solo
        /// la tipologia "gestore"
        /// </summary>
        public RegistryOwnersList ListGestioniMittente
        {
            get { return GetValue(() => ListGestioniMittente); }
            set { SetValue(() => ListGestioniMittente, value); }
        }

        /// <summary>
        /// E' la lista dei conti correnti
        /// </summary>
        public RegistryLocationList ListContoMittente
        {
            get { return GetValue(() => ListContoMittente); }
            set { SetValue(() => ListContoMittente, value); }
        }

        /// <summary>
        /// E' la lista OwnerList da cui preleva solo
        /// la tipologia "gestore"
        /// </summary>
        public RegistryOwnersList ListGestioniRicevente
        {
            get { return GetValue(() => ListGestioniRicevente); }
            set { SetValue(() => ListGestioniRicevente, value); }
        }

        /// <summary>
        /// E' la lista dei conti correnti
        /// </summary>
        public RegistryLocationList ListContoRicevente
        {
            get { return GetValue(() => ListContoRicevente); }
            set { SetValue(() => ListContoRicevente, value); }
        }

        public RegistryCurrencyList ListValute
        {
            get { return GetValue(() => ListValute); }
            set { SetValue(() => ListValute, value); }
        }

        public TipoSoldiList tipoSoldis
        {
            get { return GetValue(() => tipoSoldis); }
            set { SetValue(() => tipoSoldis, value); }
        }
        #endregion combobo

        // compila la griglia con la disponibilità finanziaria per selezione ------------------
        public ContoCorrenteList TotaleMittenteConto
        {
            get { return GetValue(() => TotaleMittenteConto); }
            set { SetValue(() => TotaleMittenteConto, value); }
        }

        public ContoCorrenteList TotaleRiceventeConto
        {
            get { return GetValue(() => TotaleRiceventeConto); }
            set { SetValue(() => TotaleRiceventeConto, value); }
        }
        //=======================================================================================

        #region dati da registrare
        /// <summary>
        /// è il valore da trasferire
        /// </summary>
        public double Valore
        {
            get { return GetValue(() => Valore); }
            set { SetValue(() => Valore, value); }
        }

        #endregion dati da registrare

        #endregion getter & setter

        #region Events
        public void LostFocus(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text != "" && ((TextBox)sender).Name == "importo")
            {
                try
                {
                    Valore = Convert.ToDouble(((TextBox)sender).Text);
                    ActualCCmittente.Ammontare = Valore < 0 ? Valore : Valore * -1;
                    ActualCCricevente.Ammontare = Valore > 0 ? Valore : Valore * -1;
                }
                catch { }
            }
            else if (((TextBox)sender).Text != "" && ((TextBox)sender).Name == "Val_Cambio")
            {
                ActualCCmittente.Valore_Cambio = Convert.ToDouble( ((TextBox)sender).Text);
                ActualCCricevente.Valore_Cambio = ActualCCmittente.Valore_Cambio;
            }
            else if (((TextBox)sender).Text != "" && ((TextBox)sender).Name == "Causale")
            {
                ActualCCmittente.Causale = ((TextBox)sender).Text;
                ActualCCricevente.Causale = ActualCCmittente.Causale;
            }
        }

        /// <summary>
        /// Controlla che il punto del tastierino numerico venga trasformato in virgola
        /// </summary>
        /// <param name="sender">Tastiera</param>
        /// <param name="e">Pressione del tasto</param>
        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Name == "importo")
                if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
                {
                    var pos = textBox.SelectionStart;
                    textBox.Text = textBox.Text.Insert(pos, ",");
                    textBox.SelectionStart = pos + 1;
                    e.Handled = true;
                }
        }

        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && sender is DatePicker DP)
            {
                ActualCCmittente.DataMovimento = (DateTime)e.AddedItems[0];
                ActualCCricevente.DataMovimento = (DateTime)e.AddedItems[0];
            }
            else if (e.AddedItems.Count > 0 && sender is ComboBox CB)
            {
                string CBname = CB.Name;
                // --- ATTIVO SELETTIVAMENTE I COMBO BOX E AGGIORNO LE GRIGLIE ---------
                switch (CBname)
                {
                    case "ContoMittente":
                        GestioneMittente = true;
                        ActualCCmittente.Id_Conto = ((RegistryLocation)e.AddedItems[0]).Id_Conto;
                        TotaleMittenteConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoMittente);
                        break;
                    case "GestioneMittente":
                        ContoMittente = false;
                        ContoRicevente = true;
                        ActualCCmittente.Id_Gestione = ((RegistryOwner)e.AddedItems[0]).Id_gestione;
                        TotaleMittenteConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoMittente, IdGestioneMittente);
                        break;
                    case "ContoRicevente":
                        GestioneRicevente = true;
                        ActualCCricevente.Id_Conto = ((RegistryLocation)e.AddedItems[0]).Id_Conto;
                        TotaleRiceventeConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoRicevente);
                        break;
                    case "GestioneRicevente":
                        ContoRicevente = false;
                        ValutaEnabled = true;
                        ActualCCricevente.Id_Gestione = ((RegistryOwner)e.AddedItems[0]).Id_gestione;
                        TotaleRiceventeConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoRicevente, IdGestioneRicevente);
                        break;
                    case "Valuta":
                        GestioneRicevente = false;
                        ActualCCmittente.Id_Valuta = ((RegistryCurrency)e.AddedItems[0]).IdCurrency;
                        ActualCCricevente.Id_Valuta = ActualCCmittente.Id_Valuta;
                        TotaleMittenteConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoMittente, IdGestioneMittente, IdValuta);
                        TotaleRiceventeConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoRicevente, IdGestioneRicevente, IdValuta);
                        break;
                    case "Tipo_Soldi":
                        ActualCCmittente.Id_Tipo_Soldi = ((TipoSoldi)e.AddedItems[0]).Id_Tipo_Soldi;
                        ActualCCricevente.Id_Tipo_Soldi = ActualCCmittente.Id_Tipo_Soldi;
                        TotaleMittenteConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoMittente, IdGestioneMittente, IdValuta, IdTipoSoldi);
                        TotaleRiceventeConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoRicevente, IdGestioneRicevente, IdValuta, IdTipoSoldi);
                        break;
                }
            }
            else if (e.AddedItems.Count > 0 && sender is DataGrid DS)
            {
                if (DS.Name == "CONTOtrasfatti" && e.AddedItems[0] is ContoCorrente CC)
                {
                    ContiGemelli = new ContoCorrenteList();
                    ContiGemelli = _contoCorrenteServices.Get2ContoCorrentes(CC.Modified);
                    if (CC.Ammontare < 0)
                    {
                        Valore = CC.Ammontare * -1;
                        ActualCCmittente = CC;
                        IdContoMittente = ActualCCmittente.Id_Conto;
                        IdGestioneMittente = ActualCCmittente.Id_Gestione;
                        ActualCCricevente = ContiGemelli[0].Id_RowConto == CC.Id_RowConto ? ContiGemelli[1] : ContiGemelli[0];
                        IdContoRicevente = ActualCCricevente.Id_Conto;
                        IdGestioneRicevente = ActualCCricevente.Id_Gestione;
                        IdValuta = ActualCCricevente.Id_Valuta;
                        IdTipoSoldi = ActualCCricevente.Id_Tipo_Soldi;
                    }
                    else if (CC.Ammontare > 0)
                    {
                        Valore = CC.Ammontare;
                        ActualCCricevente = CC;
                        IdContoRicevente = ActualCCricevente.Id_Conto;
                        IdGestioneRicevente = ActualCCricevente.Id_Gestione;
                        ActualCCmittente = ContiGemelli[0].Id_RowConto == CC.Id_RowConto ? ContiGemelli[1] : ContiGemelli[0];
                        IdContoMittente = ActualCCmittente.Id_Conto;
                        IdGestioneMittente = ActualCCmittente.Id_Gestione;
                        IdValuta = ActualCCmittente.Id_Valuta;
                        IdTipoSoldi = ActualCCmittente.Id_Tipo_Soldi;
                    }
                }

            }
        }

        #endregion

        #region Commands
        public void CloseMe(object param)
        {
            GiroContoView view = param as GiroContoView;
            DockPanel wp = view.Parent as DockPanel;
            wp.Children.Remove(view);
        }

        public void ClearMe(object param)
        {
            ClearData();
        }

        public bool CanSave(object param)
        {
            if (ActualCCmittente.Ammontare < 0 && ActualCCricevente.Ammontare > 0 && ActualCCricevente.Id_Valuta > 0 && 
                ActualCCmittente.Id_Tipo_Soldi > 0 && ActualCCmittente.Id_RowConto == 0)
                return true;
            return false;
        }

        public void SaveCommand(object param)
        {
            try
            {
                _contoCorrenteServices.InsertAccountMovement(ActualCCmittente);
                try
                {
                    _contoCorrenteServices.InsertAccountMovement(ActualCCricevente);
                }
                catch (Exception err)
                {
                    _contoCorrenteServices.DeleteRecordContoCorrente(_contoCorrenteServices.GetLastContoCorrente().Id_RowConto);
                    MessageBox.Show(string.Format("Errore nella registrazione del conto ricevente: ", Environment.NewLine, err.Message,
                        Environment.NewLine, "La registrazione del conto mittente è stata cancellata."), "Giroconto", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("Errore nella registrazione del conto mittente: ", Environment.NewLine, err.Message,
                    Environment.NewLine, "La registrazione del conto ricevente è stata sospesa."), "Giroconto", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("Registrazione effettuata correttamente.", "Giroconto", MessageBoxButton.OK, MessageBoxImage.Information);
            ClearMe(param);
        }

        public bool CanModify(object param)
        {
            if (ActualCCmittente.Id_RowConto > 0 && ActualCCricevente.Id_RowConto > 0)
                return true;
            return false;
        }

        public void UpdateCommand(object param)
        {
            try
            {
                _contoCorrenteServices.UpdateRecordContoCorrente(ActualCCmittente, 0);
                try
                {
                    _contoCorrenteServices.UpdateRecordContoCorrente(ActualCCricevente, 0);
                }
                catch (Exception err)
                {
                    _contoCorrenteServices.UpdateRecordContoCorrente(ContiGemelli[0], 0);
                    MessageBox.Show(string.Format("L'aggiornamento del ricevente ha dato un problema: ", Environment.NewLine, err.Message, Environment.NewLine,
                        "La modifica dei dati non è avvenuta"), "Errore Trasferimento Soldi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                MessageBox.Show("La modifica è avvenuto con successo.", "Trasferimento Soldi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("L'aggiornamento del mittente ha dato un problema: ", Environment.NewLine, err.Message, Environment.NewLine,
                    "La modifica dei dati non è avvenuta"), "Errore Trasferimento Soldi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ClearMe(param);
        }

        #endregion

    }
}

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
            //------------------- in doppio per i combo box Mittente e Ricevente ------------------------------
            ListGestioniMittente = new RegistryGestioniList();
            ListGestioniMittente = _registryServices.GetGestioneList();
            ListContoMittente = new RegistryLocationList();
            ListContoMittente = _registryServices.GetRegistryLocationList();
            ListSociMittente = new RegistrySociList();
            ListSociMittente = _registryServices.GetSociList();
            ListGestioniRicevente = new RegistryGestioniList();
            ListGestioniRicevente = _registryServices.GetGestioneList();
            ListContoRicevente = new RegistryLocationList();
            ListContoRicevente = _registryServices.GetRegistryLocationList();
            ListSociRicevente = new RegistrySociList();
            ListSociRicevente = _registryServices.GetSociList();
            ListTSoldiMittente = new TipoSoldiList();
            ListTSoldiMittente = _registryServices.GetTipoSoldiList();
            ListTSoldiRicevente = new TipoSoldiList();
            ListTSoldiRicevente = _registryServices.GetTipoSoldiList();
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
            ActualCCmittente.Valore_Cambio = 1;
            ActualCCmittente.DataMovimento = DateTime.Now;
            ActualCCricevente = new ContoCorrente();
            ActualCCricevente.Id_tipo_movimento = 12;
            ActualCCricevente.Valore_Cambio = 1;
            ActualCCricevente.DataMovimento = DateTime.Now;

            TotaleMittenteConto = new ContoCorrenteList();
            TotaleRiceventeConto = new ContoCorrenteList();
            SocioMittente = false;
            GestioneMittente = false;
            SocioRicevente = false;
            GestioneRicevente = false;
            ValutaEnabled = false;
            IdContoMittente = 0;
            IdSocioMittente = -1;
            IdGestioneMittente = -1;
            IdContoRicevente = 0;
            IdSocioRicevente = -1;
            IdGestioneRicevente = -1;
            IdTSoldiMittente = -1;
            IdTSoldiRicevente = -1;
            IdValuta = 0;
            Valore = 0;
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
        public bool SocioMittente
        {
            get { return GetValue(() => SocioMittente); }
            set { SetValue(() => SocioMittente, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione fra investitore
        /// e conto / gestione nel campo "ricevente"
        /// </summary>
        public bool SocioRicevente
        {
            get { return GetValue(() => SocioRicevente); }
            set { SetValue(() => SocioRicevente, value); }
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

        public int IdSocioMittente
        {
            get { return GetValue(() => IdSocioMittente); }
            set { SetValue(() => IdSocioMittente, value); }
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

        public int IdSocioRicevente
        {
            get { return GetValue(() => IdSocioRicevente); }
            set { SetValue(() => IdSocioRicevente, value); }
        }
        public int IdTSoldiMittente
        {
            get { return GetValue(() => IdTSoldiMittente); }
            set { SetValue(() => IdTSoldiMittente, value); }
        }
        public int IdTSoldiRicevente
        {
            get { return GetValue(() => IdTSoldiRicevente); }
            set { SetValue(() => IdTSoldiRicevente, value); }
        }

        #endregion

        #region combobox

        /// <summary>
        /// E' la lista OwnerList da cui preleva solo
        /// la tipologia "gestore"
        /// </summary>
        public RegistryGestioniList ListGestioniMittente
        {
            get { return GetValue(() => ListGestioniMittente); }
            set { SetValue(() => ListGestioniMittente, value); }
        }

        public RegistrySociList ListSociMittente
        {
            get { return GetValue(() => ListSociMittente); }
            set { SetValue(() => ListSociMittente, value); }
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
        public RegistryGestioniList ListGestioniRicevente
        {
            get { return GetValue(() => ListGestioniRicevente); }
            set { SetValue(() => ListGestioniRicevente, value); }
        }
        public RegistrySociList ListSociRicevente
        {
            get { return GetValue(() => ListSociRicevente); }
            set { SetValue(() => ListSociRicevente, value); }
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

        public TipoSoldiList ListTSoldiMittente
        {
            get { return GetValue(() => ListTSoldiMittente); }
            set { SetValue(() => ListTSoldiMittente, value); }
        }

        public TipoSoldiList ListTSoldiRicevente
        {
            get { return GetValue(() => ListTSoldiRicevente); }
            set { SetValue(() => ListTSoldiRicevente, value); }
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
                        ActualCCmittente.Id_Conto = ((RegistryLocation)e.AddedItems[0]).Id_Conto;
                        GestioneMittente = ActualCCmittente.Id_Conto > 1 ? true : false;
                        SocioMittente = ActualCCmittente.Id_Conto == 1 ? true : false;
                        TotaleMittenteConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoMittente);
                        break;
                    case "GestioneMittente":
                        ActualCCmittente.Id_Gestione = ((RegistryGestioni)e.AddedItems[0]).Id_Gestione;
                        ActualCCmittente.Id_Tipo_Gestione = ActualCCmittente.Id_Gestione == 1 ? 1 : 2;
                        TotaleMittenteConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoMittente, IdGestione: IdGestioneMittente);
                        break;
                    case "SocioMittente":
                        ActualCCmittente.Id_Socio = ((RegistrySoci)e.AddedItems[0]).Id_Socio;
                        TotaleMittenteConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoMittente, IdSocio: IdSocioMittente);
                        break;
                    case "TSoldiMittente":
                        ActualCCmittente.Id_Tipo_Soldi = ((TipoSoldi)e.AddedItems[0]).Id_Tipo_Soldi;
                        if (ActualCCmittente.Id_Conto > 1)
                            TotaleMittenteConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoMittente, IdGestione: IdGestioneMittente, IdTipoSoldi: IdTSoldiMittente, IdValuta: IdValuta);
                        else
                            TotaleMittenteConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoMittente, IdSocio: IdSocioMittente, IdTipoSoldi: IdTSoldiMittente, IdValuta: IdValuta);

                        break;
                    case "ContoRicevente":
                        ActualCCricevente.Id_Conto = ((RegistryLocation)e.AddedItems[0]).Id_Conto;
                        GestioneRicevente = ActualCCricevente.Id_Conto > 1 ? true : false;
                        SocioRicevente = ActualCCricevente.Id_Socio == 1 ? true : false;
                        TotaleRiceventeConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoRicevente);
                        break;
                    case "GestioneRicevente":
                        ValutaEnabled = true;
                        ActualCCricevente.Id_Gestione = ((RegistryGestioni)e.AddedItems[0]).Id_Gestione;
                        ActualCCricevente.Id_Tipo_Gestione = ActualCCricevente.Id_Gestione == 1 ? 1 : 2;
                        TotaleRiceventeConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoRicevente, IdGestione: IdGestioneRicevente);
                        break;
                    case "SocioRicevente":
                        ValutaEnabled = true;
                        ActualCCricevente.Id_Socio = ((RegistrySoci)e.AddedItems[0]).Id_Socio;
                        TotaleRiceventeConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoRicevente, IdSocio: IdSocioRicevente);
                        break;
                    case "TSoldiRicevente":
                        ActualCCricevente.Id_Tipo_Soldi = ((TipoSoldi)e.AddedItems[0]).Id_Tipo_Soldi;
                        if (ActualCCricevente.Id_Conto > 1)
                            TotaleRiceventeConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoRicevente, IdGestione: IdGestioneRicevente, IdTipoSoldi: IdTSoldiRicevente, IdValuta: IdValuta);
                        else
                            TotaleRiceventeConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoRicevente, IdSocio: IdSocioRicevente, IdTipoSoldi: IdTSoldiRicevente, IdValuta: IdValuta);

                        break;
                    case "Valuta":
                        ActualCCmittente.Id_Valuta = ((RegistryCurrency)e.AddedItems[0]).IdCurrency;
                        ActualCCricevente.Id_Valuta = ActualCCmittente.Id_Valuta;
                        if (ActualCCmittente.Id_Conto > 1)
                            TotaleMittenteConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoMittente, IdGestione: IdGestioneMittente, IdTipoSoldi: IdTSoldiMittente, IdValuta: IdValuta);
                        else
                            TotaleMittenteConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoMittente, IdSocio: IdSocioMittente, IdTipoSoldi: IdTSoldiMittente, IdValuta: IdValuta);
                        if (ActualCCricevente.Id_Conto > 1)
                            TotaleRiceventeConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoRicevente, IdGestione: IdGestioneRicevente, IdTipoSoldi: IdTSoldiRicevente, IdValuta: IdValuta);
                        else
                            TotaleRiceventeConto = _contoCorrenteServices.GetTotalAmountByAccount(IdContoRicevente, IdSocio: IdSocioRicevente, IdTipoSoldi: IdTSoldiRicevente, IdValuta: IdValuta);
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
                        IdSocioMittente = ActualCCmittente.Id_Socio;
                        IdGestioneMittente = ActualCCmittente.Id_Gestione;
                        IdTSoldiMittente = ActualCCmittente.Id_Tipo_Soldi;
                        ActualCCricevente = ContiGemelli[0].Id_RowConto == CC.Id_RowConto ? ContiGemelli[1] : ContiGemelli[0];
                        IdContoRicevente = ActualCCricevente.Id_Conto;
                        IdSocioRicevente = ActualCCricevente.Id_Socio;
                        IdGestioneRicevente = ActualCCricevente.Id_Gestione;
                        IdTSoldiRicevente = ActualCCricevente.Id_Tipo_Soldi;
                        IdValuta = ActualCCricevente.Id_Valuta;
                    }
                    else if (CC.Ammontare > 0)
                    {
                        Valore = CC.Ammontare;
                        ActualCCricevente = CC;
                        IdContoRicevente = ActualCCricevente.Id_Conto;
                        IdSocioRicevente = ActualCCricevente.Id_Socio;
                        IdGestioneRicevente = ActualCCricevente.Id_Gestione;
                        IdTSoldiRicevente = ActualCCricevente.Id_Tipo_Soldi;
                        ActualCCmittente = ContiGemelli[0].Id_RowConto == CC.Id_RowConto ? ContiGemelli[1] : ContiGemelli[0];
                        IdContoMittente = ActualCCmittente.Id_Conto;
                        IdSocioMittente = ActualCCmittente.Id_Socio;
                        IdGestioneMittente = ActualCCmittente.Id_Gestione;
                        IdTSoldiMittente = ActualCCmittente.Id_Tipo_Soldi;
                        IdValuta = ActualCCmittente.Id_Valuta;
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
                ActualCCmittente.Id_Tipo_Soldi > 0 && ActualCCmittente.Id_RowConto == 0 && ActualCCmittente.Valore_Cambio > 0)
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

using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using NPOI.SS.Formula.Functions;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;

namespace FinanceManager.ViewModels
{
    public class CambioValutaViewModel : ViewModelBase
    {
        IRegistryServices _registryServices;
        IContoCorrenteServices _contoCorrenteServices;
        public ICommand CloseMeCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand EraseCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        Predicate<object> _Filter;

        public CambioValutaViewModel(IRegistryServices registryServices, IContoCorrenteServices contoCorrenteServices)
        {
            _registryServices = registryServices ?? throw new ArgumentNullException("Registry Services in Gestione Conto Corrente");
            _contoCorrenteServices = contoCorrenteServices ?? throw new ArgumentNullException("Conto corrente services assente");
            CloseMeCommand = new CommandHandler(CloseMe);
            InsertCommand = new CommandHandler(SaveCommand, CanSave);
            ModifyCommand = new CommandHandler(UpdateCommand, CanModify);
            EraseCommand = new CommandHandler(DeleteCommand, CanModify);
            ClearCommand = new CommandHandler(CleanCommand);
            ListConti = new RegistryLocationList();
            ListConti = _registryServices.GetRegistryLocationList();
            ListSoci = new RegistrySociList();
            ListSoci = _registryServices.GetSociList();
            ListGestioni = new RegistryGestioniList();
            ListGestioni = _registryServices.GetGestioneList();
            ListTipoSoldi = new TipoSoldiList();
            ListTipoSoldi = _registryServices.GetTipoSoldiList();
            ListaValuteMittente = new RegistryCurrencyList();
            ListaValuteMittente = _registryServices.GetRegistryCurrencyList();
            ListaValuteRicevente = new RegistryCurrencyList();
            ListaValuteRicevente = _registryServices.GetRegistryCurrencyList();
            Init();
        }

        private void Init()
        {
            // popolo la griglia con la disponibilità di tutti i conti (codice 0) -----------------
            TotaleDisponibili = new ContoCorrenteList();
            TotaleDisponibili = _contoCorrenteServices.GetTotalAmountByAccount(0);
            //=====================================================================================
            // popolo la griglia con tutti i movimenti di cambio valuta ---------------------------
            ContoCorrenteList CCL = new ContoCorrenteList();
            TotaleMovimentiCambioValuta = new ContoCorrenteList();
            CCL = _contoCorrenteServices.GetContoCorrenteList();
            var ccl = from movimento in CCL
                      where movimento.Id_tipo_movimento == 3
                      select movimento;
            foreach (ContoCorrente conto in ccl)
                TotaleMovimentiCambioValuta.Add(conto);
            //=====================================================================================
            // i 2 record per il cambio valute con l'inizializzazione -----------------------------
            recordCCMittente = new ContoCorrente();
            recordCCRicevente = new ContoCorrente();
            recordCCMittente.Id_tipo_movimento = 3;
            recordCCRicevente.Id_tipo_movimento = 3;
            recordCCMittente.Modified = DateTime.Now;
            recordCCRicevente.Modified = recordCCMittente.Modified;
            Temporaneo = new ContoCorrente();
            //=====================================================================================
            // variabili per il filtro disponibilità --------------------------------------------
            IlConto = "";
            LaGestione = "";
            IlSocio = "";
            LaValuta = "";
            IlTipoSoldi = "";
            ContoGestioni = "Visible";
            ContoSoci = "Collapsed";
            _Filter = new Predicate<object>(Filter);
            //===================================================================================
        }


        #region get_set
        // visibilità del combo Socio / Gestioni
        public string ContoGestioni
        {
            get { return GetValue(() => ContoGestioni); }
            set { SetValue(() => ContoGestioni, value); }
        }
        public string ContoSoci
        {
            get { return GetValue(() => ContoSoci); }
            set { SetValue(() => ContoSoci, value); }
        }

        // combo box con la lista dei C/C-----------------------
        public RegistryLocationList ListConti
        {
            get { return GetValue(() => ListConti); }
            set { SetValue(() => ListConti, value); }
        }
        // combo box con la lista delle gestioni ---------------
        public RegistryGestioniList ListGestioni
        {
            get { return GetValue(() => ListGestioni); }
            set { SetValue(() => ListGestioni, value); }
        }
        // combo box con i soci
        public RegistrySociList ListSoci
        {
            get { return GetValue(() => ListSoci); }
            set { SetValue(() => ListSoci, value); }
        }
        // combo box con i tipi soldi --------------------------
        public TipoSoldiList ListTipoSoldi
        {
            get { return GetValue(() => ListTipoSoldi); }
            set { SetValue(() => ListTipoSoldi, value); }
        }
        // combo box con le valute di partenza -----------------
        public RegistryCurrencyList ListaValuteMittente
        {
            get { return GetValue(() => ListaValuteMittente); }
            set { SetValue(() => ListaValuteMittente, value); }
        }
        // combo box con le valute di arrivo  -----------------
        public RegistryCurrencyList ListaValuteRicevente
        {
            get { return GetValue(() => ListaValuteRicevente); }
            set { SetValue(() => ListaValuteRicevente, value); }
        }
        // il record del conto corrente con la valuta da cambiare
        public ContoCorrente recordCCMittente
        {
            get { return GetValue(() => recordCCMittente); }
            set { SetValue(() => recordCCMittente, value); }
        }
        // il record del conto corrente con la valuta cambiata
        public ContoCorrente recordCCRicevente
        {
            get { return GetValue(() => recordCCRicevente); }
            set { SetValue(() => recordCCRicevente, value); }
        }
        // il record temporaneo per aggiornare i campi delle view
        public ContoCorrente Temporaneo
        {
            get { return GetValue(() => Temporaneo); }
            set { SetValue(() => Temporaneo, value); }
        }
        /// <summary>
        /// Elenco con la somma delle disponibilità
        /// </summary>
        public ContoCorrenteList TotaleDisponibili
        {
            get { return GetValue(() => TotaleDisponibili); }
            set { SetValue(() => TotaleDisponibili, value); TotaleDisponibiliView = CollectionViewSource.GetDefaultView(value); }
        }
        public System.ComponentModel.ICollectionView TotaleDisponibiliView
        {
            get { return GetValue(() => TotaleDisponibiliView); }
            set { SetValue(() => TotaleDisponibiliView, value); }
        }
        /// <summary>
        /// Elenco con tutti movimenti di cambio valuta
        /// </summary>
        public ContoCorrenteList TotaleMovimentiCambioValuta
        {
            get { return GetValue(() => TotaleMovimentiCambioValuta); }
            set { SetValue(() => TotaleMovimentiCambioValuta, value); TotaleMovimentiCambioValutaView = CollectionViewSource.GetDefaultView(value); }
        }

        public System.ComponentModel.ICollectionView TotaleMovimentiCambioValutaView
        {
            get { return GetValue(() => TotaleMovimentiCambioValutaView); }
            set { SetValue(() => TotaleMovimentiCambioValutaView, value); }
        }

        // variabili usati nel filtro delle disponibilità ----------------------------------------------------------------------
        public string IlConto
        {
            get { return GetValue(() => IlConto); }
            set { SetValue(() => IlConto, value); TotaleDisponibiliView.Filter = _Filter; TotaleDisponibiliView.Refresh(); }
        }
        public string LaGestione
        {
            get { return GetValue(() => LaGestione); }
            set { SetValue(() => LaGestione, value); TotaleDisponibiliView.Filter = _Filter; TotaleDisponibiliView.Refresh(); }
        }
        public string IlSocio
        {
            get { return GetValue(() => IlSocio); }
            set { SetValue(() => IlSocio, value); TotaleDisponibiliView.Filter = _Filter; TotaleDisponibiliView.Refresh(); }
        }
        public string LaValuta
        {
            get { return GetValue(() => LaValuta); }
            set { SetValue(() => LaValuta, value); TotaleDisponibiliView.Filter = _Filter; TotaleDisponibiliView.Refresh(); }
        }
        public string IlTipoSoldi
        {
            get { return GetValue(() => IlTipoSoldi); }
            set { SetValue(() => IlTipoSoldi, value); TotaleDisponibiliView.Filter = _Filter; TotaleDisponibiliView.Refresh(); }
        }
        //======================================================================================================================
        #endregion get_set

        #region eventi
        /// <summary>
        /// Controlla che il punto del tastierino numerico venga trasformato in virgola
        /// </summary>
        /// <param name="sender">Tastiera</param>
        /// <param name="e">Pressione del tasto</param>
        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && !textBox.Name.Contains("Causale"))
                if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
                {
                    int pos = textBox.SelectionStart;
                    textBox.Text = textBox.Text.Insert(pos, ",");
                    textBox.SelectionStart = pos + 1;
                    e.Handled = true;
                }
        }

        /// <summary>
        /// Gestore dell'evento nei text box dei campi da riempire
        /// </summary>
        /// <param name="sender">Text Box</param>
        /// <param name="e">Uscita del campo</param>
        public void LostFocus(object sender, EventArgs e)
        {
            if (sender is TextBox TB)
            {
                switch (TB.Name)
                {
                    case "Valore_di_cambio":
                        recordCCRicevente.Valore_Cambio = 1 / recordCCMittente.Valore_Cambio;
                        if (recordCCMittente.Ammontare > 0)
                        {
                            Temporaneo = recordCCRicevente;
                            Temporaneo.Ammontare = recordCCMittente.Ammontare * recordCCMittente.Valore_Cambio;
                            recordCCRicevente = Temporaneo;
                        }
                        break;
                    case "cifra_da_cambiare":
                        if (recordCCMittente.Valore_Cambio > 0)
                        {
                            Temporaneo = recordCCRicevente;
                            Temporaneo.Ammontare = recordCCMittente.Ammontare * recordCCMittente.Valore_Cambio;
                            recordCCRicevente = Temporaneo;
                        }
                        break;
                    case "Causale":
                        recordCCMittente.Causale = TB.Text;
                        recordCCRicevente.Causale = TB.Text;
                        break;
                }
            }
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
                if (e.AddedItems[0] is DateTime DT)
                {
                    recordCCMittente.DataMovimento = DT.Date;
                    recordCCRicevente.DataMovimento = DT.Date;
                }
                else if (e.AddedItems[0] is RegistryLocation RL)
                {
                    IlConto = RL.Desc_Conto;
                    recordCCMittente.Id_Conto = RL.Id_Conto;
                    recordCCRicevente.Id_Conto = RL.Id_Conto;
                    if (RL.Id_Conto == 1)
                    {
                        ContoSoci = "Visible";
                        ContoGestioni = "Collapsed";
                    }
                    else if (RL.Id_Conto > 1)
                    {
                        ContoGestioni = "Visible";
                        ContoSoci = "Collapsed";
                    }
                }
                else if (e.AddedItems[0] is RegistrySoci RS)
                {
                    IlSocio = RS.Nome_Socio;
                    recordCCMittente.Id_Socio = RS.Id_Socio;
                    recordCCRicevente.Id_Socio = RS.Id_Socio;
                }
                else if (e.AddedItems[0] is RegistryGestioni RO)
                {
                    LaGestione = RO.Nome_Gestione;
                    recordCCMittente.Id_Gestione = RO.Id_Gestione;
                    recordCCRicevente.Id_Gestione = RO.Id_Gestione;
                }
                else if (e.AddedItems[0] is TipoSoldi TS)
                {
                    IlTipoSoldi = TS.Desc_Tipo_Soldi;
                    recordCCMittente.Id_Tipo_Soldi = TS.Id_Tipo_Soldi;
                    recordCCRicevente.Id_Tipo_Soldi = TS.Id_Tipo_Soldi;
                }
                else if (e.AddedItems[0] is RegistryCurrency RC)
                {
                    if (((ComboBox)sender).Name == "LaValuta")
                    {
                        LaValuta = RC.CodeCurrency;
                        recordCCMittente.Id_Valuta = RC.IdCurrency;
                    }
                    else if (LaValuta == RC.CodeCurrency)
                        ((ComboBox)sender).SelectedIndex = -1;
                    else
                        recordCCRicevente.Id_Valuta = RC.IdCurrency;
                }
            }
        }

        /// <summary> Gestore dell'evento nella grid con i movimenti di cambio</summary>
        /// <param name="sender">La grid</param>
        /// <param name="e">La selezione del record</param>
        public void GridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is ContoCorrente CC)
                {
                    ContoCorrenteList contoCorrentes = _contoCorrenteServices.Get2ContoCorrentes(CC.Modified);
                    if (contoCorrentes[0].Ammontare < 0)
                    {
                        contoCorrentes[0].Ammontare = contoCorrentes[0].Ammontare * -1;
                        recordCCMittente = contoCorrentes[0];
                        recordCCRicevente = contoCorrentes[1];
                    }
                    else if (contoCorrentes[0].Ammontare > 0)
                    {
                        recordCCRicevente = contoCorrentes[0];
                        contoCorrentes[1].Ammontare = contoCorrentes[1].Ammontare * -1;
                        recordCCMittente = contoCorrentes[1];
                    }
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// E' il filtro da applicare all'elenco delle azioni
        /// e contestualmente al datagrid sottostante
        /// </summary>
        public bool Filter(object obj)
        {
            if (obj != null)
            {
                if (obj is ContoCorrente CCL)
                {
                    if (!String.IsNullOrEmpty(IlConto) && string.IsNullOrEmpty(IlSocio) && string.IsNullOrEmpty(LaGestione) && string.IsNullOrEmpty(LaValuta) && string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Desc_Conto == IlConto;
                    else if (String.IsNullOrEmpty(IlConto) && !string.IsNullOrEmpty(IlSocio) && string.IsNullOrEmpty(LaGestione) && string.IsNullOrEmpty(LaValuta) && string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.NomeGestione == IlSocio;
                    else if (String.IsNullOrEmpty(IlConto) && string.IsNullOrEmpty(IlSocio) && !string.IsNullOrEmpty(LaGestione) && string.IsNullOrEmpty(LaValuta) && string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.NomeGestione == LaGestione;
                    else if (String.IsNullOrEmpty(IlConto) && string.IsNullOrEmpty(IlSocio) && string.IsNullOrEmpty(LaGestione) && !string.IsNullOrEmpty(LaValuta) && string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Cod_Valuta == LaValuta;
                    else if (String.IsNullOrEmpty(IlConto) && string.IsNullOrEmpty(IlSocio) && string.IsNullOrEmpty(LaGestione) && string.IsNullOrEmpty(LaValuta) && !string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Desc_Tipo_Soldi == IlTipoSoldi;
                    else if (!String.IsNullOrEmpty(IlConto) && !string.IsNullOrEmpty(IlSocio) && string.IsNullOrEmpty(LaGestione) && string.IsNullOrEmpty(LaValuta) && string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Desc_Conto == IlConto && CCL.NomeGestione == IlSocio;
                    else if (!String.IsNullOrEmpty(IlConto) && string.IsNullOrEmpty(IlSocio) && !string.IsNullOrEmpty(LaGestione) && string.IsNullOrEmpty(LaValuta) && string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Desc_Conto == IlConto && CCL.NomeGestione == LaGestione;
                    else if (!String.IsNullOrEmpty(IlConto) && string.IsNullOrEmpty(IlSocio) && string.IsNullOrEmpty(LaGestione) && !string.IsNullOrEmpty(LaValuta) && string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Desc_Conto == IlConto && CCL.Cod_Valuta == LaValuta;
                    else if (!String.IsNullOrEmpty(IlConto) && string.IsNullOrEmpty(IlSocio) && string.IsNullOrEmpty(LaGestione) && string.IsNullOrEmpty(LaValuta) && !string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Desc_Conto == IlConto && CCL.Desc_Tipo_Soldi == IlTipoSoldi;
                    else if (String.IsNullOrEmpty(IlConto) && !string.IsNullOrEmpty(IlSocio) && !string.IsNullOrEmpty(LaGestione) && string.IsNullOrEmpty(LaValuta) && string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.NomeGestione == IlSocio && CCL.NomeGestione == LaGestione;
                    else if (String.IsNullOrEmpty(IlConto) && !string.IsNullOrEmpty(IlSocio) && string.IsNullOrEmpty(LaGestione) && !string.IsNullOrEmpty(LaValuta) && string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.NomeGestione == IlSocio && CCL.Cod_Valuta == LaValuta;
                    else if (String.IsNullOrEmpty(IlConto) && !string.IsNullOrEmpty(IlSocio) && string.IsNullOrEmpty(LaGestione) && string.IsNullOrEmpty(LaValuta) && !string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.NomeGestione == IlSocio && CCL.Desc_Tipo_Soldi == IlTipoSoldi;
                    else if (String.IsNullOrEmpty(IlConto) && string.IsNullOrEmpty(IlSocio) && !string.IsNullOrEmpty(LaGestione) && !string.IsNullOrEmpty(LaValuta) && string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.NomeGestione == LaGestione && CCL.Cod_Valuta == LaValuta;
                    else if (String.IsNullOrEmpty(IlConto) && string.IsNullOrEmpty(IlSocio) && !string.IsNullOrEmpty(LaGestione) && string.IsNullOrEmpty(LaValuta) && !string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.NomeGestione == LaGestione && CCL.Desc_Tipo_Soldi == IlTipoSoldi;
                    else if (String.IsNullOrEmpty(IlConto) && string.IsNullOrEmpty(IlSocio) && string.IsNullOrEmpty(LaGestione) && !string.IsNullOrEmpty(LaValuta) && !string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Cod_Valuta == LaValuta && CCL.Desc_Tipo_Soldi == IlTipoSoldi;
                    else if (!String.IsNullOrEmpty(IlConto) && !string.IsNullOrEmpty(IlSocio) && !string.IsNullOrEmpty(LaGestione) && string.IsNullOrEmpty(LaValuta) && string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Desc_Conto == IlConto && CCL.NomeGestione == IlSocio && CCL.NomeGestione == LaGestione;
                    else if (!String.IsNullOrEmpty(IlConto) && !string.IsNullOrEmpty(IlSocio) && string.IsNullOrEmpty(LaGestione) && !string.IsNullOrEmpty(LaValuta) && string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Desc_Conto == IlConto && CCL.NomeGestione == IlSocio && CCL.Cod_Valuta == LaValuta;
                    else if (!String.IsNullOrEmpty(IlConto) && !string.IsNullOrEmpty(IlSocio) && string.IsNullOrEmpty(LaGestione) && string.IsNullOrEmpty(LaValuta) && !string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Desc_Conto == IlConto && CCL.NomeGestione == IlSocio && CCL.Desc_Tipo_Soldi == IlTipoSoldi;
                    else if (!String.IsNullOrEmpty(IlConto) && string.IsNullOrEmpty(IlSocio) && !string.IsNullOrEmpty(LaGestione) && !string.IsNullOrEmpty(LaValuta) && string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Desc_Conto == IlConto && CCL.NomeGestione == LaGestione && CCL.Cod_Valuta == LaValuta;
                    else if (!String.IsNullOrEmpty(IlConto) && string.IsNullOrEmpty(IlSocio) && !string.IsNullOrEmpty(LaGestione) && string.IsNullOrEmpty(LaValuta) && !string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Desc_Conto == IlConto && CCL.NomeGestione == LaGestione && CCL.Desc_Tipo_Soldi == IlTipoSoldi;
                    else if (!String.IsNullOrEmpty(IlConto) && string.IsNullOrEmpty(IlSocio) && string.IsNullOrEmpty(LaGestione) && !string.IsNullOrEmpty(LaValuta) && !string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Desc_Conto == IlConto && CCL.Cod_Valuta == LaValuta && CCL.Desc_Tipo_Soldi == IlTipoSoldi;
                    else if (String.IsNullOrEmpty(IlConto) && string.IsNullOrEmpty(IlSocio) && !string.IsNullOrEmpty(LaGestione) && !string.IsNullOrEmpty(LaValuta) && !string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.NomeGestione == LaGestione && CCL.Cod_Valuta == LaValuta && CCL.Desc_Tipo_Soldi == IlTipoSoldi;
                    else if (!String.IsNullOrEmpty(IlConto) && !string.IsNullOrEmpty(IlSocio) && !string.IsNullOrEmpty(LaGestione) && !string.IsNullOrEmpty(LaValuta) && string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Desc_Conto == IlConto && CCL.NomeGestione == IlSocio && CCL.NomeGestione == LaGestione && CCL.Cod_Valuta == LaValuta;
                    else if (!String.IsNullOrEmpty(IlConto) && !string.IsNullOrEmpty(IlSocio) && !string.IsNullOrEmpty(LaGestione) && string.IsNullOrEmpty(LaValuta) && !string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Desc_Conto == IlConto && CCL.NomeGestione == IlSocio && CCL.NomeGestione == LaGestione && CCL.Desc_Tipo_Soldi == IlTipoSoldi;
                    else if (String.IsNullOrEmpty(IlConto) && !string.IsNullOrEmpty(IlSocio) && !string.IsNullOrEmpty(LaGestione) && !string.IsNullOrEmpty(LaValuta) && !string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.NomeGestione == IlSocio && CCL.NomeGestione == LaGestione && CCL.Cod_Valuta == LaValuta && CCL.Desc_Tipo_Soldi == IlTipoSoldi;
                    else if (!String.IsNullOrEmpty(IlConto) && string.IsNullOrEmpty(IlSocio) && !string.IsNullOrEmpty(LaGestione) && !string.IsNullOrEmpty(LaValuta) && !string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Desc_Conto == IlConto && CCL.NomeGestione == LaGestione && CCL.Cod_Valuta == LaValuta && CCL.Desc_Tipo_Soldi == IlTipoSoldi;
                    else if (!String.IsNullOrEmpty(IlConto) && !string.IsNullOrEmpty(IlSocio) && string.IsNullOrEmpty(LaGestione) && !string.IsNullOrEmpty(LaValuta) && !string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Desc_Conto == IlConto && CCL.NomeGestione == IlSocio && CCL.Cod_Valuta == LaValuta && CCL.Desc_Tipo_Soldi == IlTipoSoldi;
                    else if (!String.IsNullOrEmpty(IlConto) && !string.IsNullOrEmpty(IlSocio) && !string.IsNullOrEmpty(LaGestione) && !string.IsNullOrEmpty(LaValuta) && !string.IsNullOrEmpty(IlTipoSoldi))
                        return CCL.Desc_Conto == IlConto && CCL.NomeGestione == IlSocio && CCL.NomeGestione == LaGestione && CCL.Cod_Valuta == LaValuta && CCL.Desc_Tipo_Soldi == IlTipoSoldi;
                }
            }
            return true;
        }
        #endregion eventi

        #region comandi
        public void CleanCommand(object param)
        {
            Init();
        }

        public void CloseMe(object param)
        {
            CambioValutaView CVV = param as CambioValutaView;
            DockPanel wp = CVV.Parent as DockPanel;
            wp.Children.Remove(CVV);
        }
        public void SaveCommand(object param)
        {
            // prima di salvare cambio segno all'ammontare del mittente
            recordCCMittente.Ammontare = recordCCMittente.Ammontare * -1;
            try
            {
                _contoCorrenteServices.InsertAccountMovement(recordCCMittente);
                try
                {
                    _contoCorrenteServices.InsertAccountMovement(recordCCRicevente);
                    MessageBox.Show("Cambio effettuato correttamente", "DAF-C Cambio Valuta", MessageBoxButton.OK, MessageBoxImage.Information);
                    Init();
                }
                catch (Exception err)
                {
                    _contoCorrenteServices.DeleteRecordContoCorrente(_contoCorrenteServices.GetLastContoCorrente().Id_RowConto);
                    MessageBox.Show("Errore nella registrazione dei dati ricevente." + Environment.NewLine + 
                        "Eliminato anche il record mittente" + Environment.NewLine + err.Message, "DAF-C Cambio valuta", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Errore nella registrazione dei dati." + Environment.NewLine + err.Message, "DAF-C Cambio valuta", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void UpdateCommand(object param)
        {
            // prima di salvare cambio segno all'ammontare del mittente
            recordCCMittente.Ammontare = recordCCMittente.Ammontare * -1;
            try
            {
                _contoCorrenteServices.UpdateRecordContoCorrente(recordCCMittente, Models.Enumeratori.TipologiaIDContoCorrente.IdContoCorrente);
                try
                {
                    _contoCorrenteServices.UpdateRecordContoCorrente(recordCCRicevente, Models.Enumeratori.TipologiaIDContoCorrente.IdContoCorrente);
                    MessageBox.Show("Modifica effettuata correttamente", "DAF-C Cambio Valuta", MessageBoxButton.OK, MessageBoxImage.Information);
                    Init();
                }
                catch (Exception err)
                {
                    _contoCorrenteServices.DeleteRecordContoCorrente(recordCCMittente.Id_RowConto);
                    MessageBox.Show("Errore nella registrazione dei dati ricevente." + Environment.NewLine +
                        "Eliminato anche il record mittente" + Environment.NewLine + err.Message, "DAF-C Cambio valuta", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch(Exception err)
            {
                MessageBox.Show("Errore nella registrazione dei dati." + Environment.NewLine + err.Message, "DAF-C Cambio valuta", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void DeleteCommand(object param)
        {
            try
            {
                _contoCorrenteServices.DeleteRecordContoCorrente(recordCCMittente.Id_RowConto);
                _contoCorrenteServices.DeleteRecordContoCorrente(recordCCRicevente.Id_RowConto);
                MessageBox.Show("Eliminazione effettuata correttamente", "DAF-C Cambio Valuta", MessageBoxButton.OK, MessageBoxImage.Information);
                Init();
            }
            catch (Exception err)
            {
                MessageBox.Show("Errore nell'eliminazione dei dati." + Environment.NewLine + err.Message, "DAF-C Cambio valuta", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public bool CanSave(object param)
        {
            if (recordCCMittente.Id_RowConto == 0 && recordCCMittente.Id_Conto > 0 && recordCCMittente.Id_Gestione > 0 && recordCCMittente.Id_Valuta > 0 &&
                recordCCMittente.Id_Tipo_Soldi > 0 && recordCCRicevente.Ammontare >0)
                return true;
            return false;
        }
        public bool CanModify(object param)
        {
            if (recordCCMittente.Id_RowConto > 0 && recordCCRicevente.Id_RowConto > 0)
                return true;
            return false;
        }
        #endregion comandi
    }
}

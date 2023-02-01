using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;

namespace FinanceManager.ViewModels
{
    public class CapitalsRegisterViewModel : ViewModelBase
    {
        private readonly IRegistryServices _registryServices;
        private readonly IManagerLiquidAssetServices _managerLiquidServices;
        private readonly IContoCorrenteServices _contoCorrenteServices;
        private readonly IQuoteGuadagniServices _quoteServices;

        public ICommand CloseMeCommand { get; set; }
        public ICommand ClearMeCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand RicalcolaCommand { get; set; }
        Predicate<object> _Filter;

        public CapitalsRegisterViewModel
            (IRegistryServices registryServices, IManagerLiquidAssetServices managerLiquidServices,
            IContoCorrenteServices contoCorrenteServices, IQuoteGuadagniServices quoteServices)
        {
            _registryServices = registryServices ?? throw new ArgumentNullException("Manca collegamento con richiesta dati anagrafica Capitali View Model");
            _managerLiquidServices = managerLiquidServices ?? throw new ArgumentNullException("Manca collegamento con richiesta dati conti Capitali View Model");
            _contoCorrenteServices = contoCorrenteServices ?? throw new ArgumentNullException("Manca collegamento con Conto Services");
            _quoteServices = quoteServices ?? throw new ArgumentNullException("Manca collegamento con Quote Services");
            CloseMeCommand = new CommandHandler(CloseMe);
            ClearMeCommand = new CommandHandler(ClearMe);
            InsertCommand = new CommandHandler(SaveCommand, CanSave);
            ModifyCommand = new CommandHandler(UpdateCommand, CanModify);
            RicalcolaCommand = new CommandHandler(RicalcoloUtili, CanDoIt);
            ListInvestitori = new RegistrySociList();
            ListInvestitori = _registryServices.GetSociList();
            // Filtro la tabella movimenti prendendo solo versamento e prelievo
            ListMovimento = new RegistryMovementTypeList();
            RegistryMovementTypeList ListaOriginale = new RegistryMovementTypeList();
            ListaOriginale = _registryServices.GetRegistryMovementTypesList();
            foreach (RegistryMovementType RMT in ListaOriginale)
            {
                if (RMT.Id_tipo_movimento == 1 || RMT.Id_tipo_movimento == 2)
                    ListMovimento.Add(RMT);
            }
            ListaValute = new RegistryCurrencyList();
            ListaValute = _registryServices.GetRegistryCurrencyList();
            Init();
        }
        /// <summary>
        /// alla pressione del tasto pulisci e ogni volta che finisce una operazione
        /// di prelievo / versamento
        /// </summary>
        private void Init()
        {
            ActualContoCorrente = new ContoCorrente();
            ActualContoCorrente.Id_Conto = 1;
            ActualContoCorrente.Id_Tipo_Soldi = (int)Models.Enumeratori.TipologiaSoldi.Capitale;
            ActualContoCorrente.Modified = DateTime.Now;
            Investimenti = new ContoCorrenteList();
            Investimenti = _contoCorrenteServices.GetCCListByInvestmentSituation();
            SintesiContoBase = _contoCorrenteServices.GetInvestmentSituation();
            _Filter = new Predicate<object>(Filter);
            Investitore = "";
            CodeCurrency = "";
        }

        #region Get_Set

        #region ComboBox
        public RegistrySociList ListInvestitori
        {
            get { return GetValue(() => ListInvestitori); }
            set { SetValue(() => ListInvestitori, value); }
        }

        public RegistryMovementTypeList ListMovimento
        {
            get { return GetValue(() => ListMovimento); }
            set { SetValue(() => ListMovimento, value); }
        }

        public RegistryCurrencyList ListaValute
        {
            get { return GetValue(() => ListaValute); }
            set { SetValue(() => ListaValute, value); }
        }

        public DateTime dataModifica
        {
            get { return GetValue(() => dataModifica); }
            set { SetValue(() => dataModifica, value); }
        }
        #endregion ComboBox

        #region DataGrid
        public InvestmentSituationList SintesiContoBase
        {
            get { return GetValue(() => SintesiContoBase); }
            set { SetValue(() => SintesiContoBase, value); SintesiCBase = CollectionViewSource.GetDefaultView(value); }
        }
        public System.ComponentModel.ICollectionView SintesiCBase
        {
            get { return GetValue(() => SintesiCBase); }
            set { SetValue(() => SintesiCBase, value); }
        }
        /// <summary>
        /// Elenco di movimenti del conto zero
        /// solo per versamenti e prelievi
        /// </summary>
        public ContoCorrenteList Investimenti
        {
            get { return GetValue(() => Investimenti); }
            set { SetValue(() => Investimenti, value); InvestCollection = CollectionViewSource.GetDefaultView(value); }
        }
        public System.ComponentModel.ICollectionView InvestCollection
        {
            get { return GetValue(() => InvestCollection); }
            set { SetValue(() => InvestCollection, value); }
        }
        #endregion DataGrid

        /// <summary>
        /// E' il record che si sta lavorando e che verrà
        /// salvato nel data base come capitali versati o prelevati
        /// </summary>
        public ContoCorrente ActualContoCorrente
        {
            get { return GetValue(() => ActualContoCorrente); }
            set { SetValue(() => ActualContoCorrente, value); }
        }
        #endregion Get_Set

        #region events
        /// <summary>
        /// per convertire il testo in numero nei campi dedicati alla valuta
        /// </summary>
        /// <param name="sender">Text Box</param>
        /// <param name="e">Evento</param>
        public void LostFocus(object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox textBox = sender as TextBox;
                if (textBox.Name == "importo")
                {
                    try
                    {
                        ActualContoCorrente.Ammontare = Convert.ToDouble(textBox.Text);
                        if (ActualContoCorrente.Id_tipo_movimento == 2)
                            if (SintesiCBase.CurrentItem == null || ActualContoCorrente.Ammontare > ((InvestmentSituation)SintesiCBase.CurrentItem).Disponibile)
                            {
                                MessageBox.Show("Non hai abbastanza capitali.", "Registrazione Capitali",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                                textBox.Text = "0";
                            }
                    }
                    catch { }
                }
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
        /// <summary>
        /// Alla selezione nel data grid della tabella investimenti riempie i campi
        /// per una eventuale modifica e filtra i record del data grid di riepilogo
        /// Nel caso di nuovo inserimento la selezione dei Combo Box filtra i record
        /// del data grid di riepilogo disponibilità per verificare che si sta facendo
        /// </summary>
        /// <param name="sender">Data Grid / ComboBox</param>
        /// <param name="e">Evento di selezione</param>
        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && sender is DataGrid DS)
            {
                ActualContoCorrente = (ContoCorrente)e.AddedItems[0];
            }
            if (e.AddedItems.Count > 0 && sender is ComboBox)
            {
                if (e.AddedItems[0] is RegistrySoci)
                    Investitore = ((RegistrySoci)e.AddedItems[0]).Nome_Socio;
                if (e.AddedItems[0] is RegistryCurrency)
                    CodeCurrency = ((RegistryCurrency)e.AddedItems[0]).CodeCurrency;
            }
        }

        #endregion
        // tutta la gestione dei filtri del datagrid di riepilogo
        public bool Filter(object obj)
        {
            if (obj != null)
            {
                if (obj.GetType() == typeof(InvestmentSituation))
                {
                    var data = obj as InvestmentSituation;
                    if (!string.IsNullOrEmpty(data.Socio))
                    {
                        if (!string.IsNullOrEmpty(Investitore) && string.IsNullOrEmpty(CodeCurrency))
                            return data.Socio.ToUpper().Contains(Investitore.ToUpper());
                        else if (string.IsNullOrEmpty(Investitore) && !string.IsNullOrEmpty(CodeCurrency))
                            return data.CodValuta.ToLower().Contains(CodeCurrency.ToLower());
                        else if (!string.IsNullOrEmpty(Investitore) && !string.IsNullOrEmpty(CodeCurrency))
                            return data.Socio.ToLower().Contains(Investitore.ToLower()) && data.CodValuta.ToUpper().Contains(CodeCurrency.ToUpper());
                    }
                }
            }
            return true;
        }

        private string _Investitore;
        private string Investitore
        {
            get { return _Investitore; }
            set { _Investitore = value; SintesiCBase.Filter = _Filter; SintesiCBase.Refresh(); }
        }
        private string _CodeCurrency;
        private string CodeCurrency
        {
            get { return _CodeCurrency; }
            set { _CodeCurrency = value; SintesiCBase.Filter = _Filter; SintesiCBase.Refresh(); }
        }

        #region Commands
        public void CloseMe(object param)
        {
            CapitalsRegisterView view = param as CapitalsRegisterView;
            DockPanel wp = view.Parent as DockPanel;
            wp.Children.Remove(view);
        }
        public void ClearMe(object param = null)
        {
            Init();
        }
        public bool CanSave(object param)
        {
            if (ActualContoCorrente.Id_Socio > 0 && ActualContoCorrente.Ammontare != 0 && ActualContoCorrente.Id_Valuta > 0 && ActualContoCorrente.Id_RowConto == 0 &&
                (ActualContoCorrente.Id_tipo_movimento == 1 || ActualContoCorrente.Id_tipo_movimento == 2) && ActualContoCorrente.Valore_Cambio > 0)
                return true;
            return false;
        }
        public void SaveCommand(object param)
        {
            try
            {
                // determino chi sta inserendo e su quella base assegno il tipo di gestione utili
                ActualContoCorrente.Id_Tipo_Gestione = ActualContoCorrente.Id_Socio == 3 ? 2 : 1;
                // verifico se alla stessa data con lo stesso tipo gestione ci siano altri inserimenti
                int result = Convert.ToInt16(_quoteServices.VerifyInvestmentDate(ActualContoCorrente, ActualContoCorrente.Id_Tipo_Gestione));
                if (result == -1)
                {
                    // prelevo l'ultimo record del socio con la stessa valuta
                    QuoteGuadagno quoteGuadagno = _quoteServices.GetLastRecordBySocioValuta(ActualContoCorrente.Id_Socio, 
                        ActualContoCorrente.Id_Valuta, ActualContoCorrente.Id_Tipo_Gestione);
                    // verifico se è inserimento o prelievo
                    if (ActualContoCorrente.Id_tipo_movimento == 2)
                    {
                        ActualContoCorrente.Ammontare = ActualContoCorrente.Ammontare > 0 ? ActualContoCorrente.Ammontare * -1 : ActualContoCorrente.Ammontare;
                        // verifico che il totale a disposizione del socio permetta il prelievo
                        if (quoteGuadagno.cum_socio + ActualContoCorrente.Ammontare < 0)
                        {
                            MessageBox.Show("Il prelievo supera la disponibilità.", "Gestioni Capitali", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                    }
                    NuovoInserimento(quoteGuadagno);
                }
                else
                {
                    // è già stato fatto un inserimento pari data e tipo gestione quindi recupero il valore
                    ActualContoCorrente.Id_Quote_Periodi = result;
                    // prelevo l'ultimo record del socio con la stessa valuta
                    QuoteGuadagno quoteGuadagno = _quoteServices.GetLastRecordBySocioValuta(ActualContoCorrente.Id_Socio, ActualContoCorrente.Id_Valuta, ActualContoCorrente.Id_Tipo_Gestione);
                    // verifico se è inserimento o prelievo
                    if (ActualContoCorrente.Id_tipo_movimento == 2)
                    {
                        ActualContoCorrente.Ammontare = ActualContoCorrente.Ammontare > 0 ? ActualContoCorrente.Ammontare * -1 : ActualContoCorrente.Ammontare;
                        // verifico che il totale a disposizione del socio permetta il prelievo
                        if (quoteGuadagno.cum_socio + ActualContoCorrente.Ammontare < 0)
                        {
                            MessageBox.Show("Il prelievo supera la disponibilità.", "Gestioni Capitali", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                    }
                    // registro l'inserimento
                    _contoCorrenteServices.InsertAccountMovement(ActualContoCorrente);
                    // riprendo il record inserito
                    ActualContoCorrente = _contoCorrenteServices.GetLastContoCorrente();
                    // calcolo il nuovo totale
                    double NuovoCumTotale = quoteGuadagno.cum_totale + ActualContoCorrente.Ammontare;
                    quoteGuadagno.cum_totale = NuovoCumTotale;
                    // calcolo il nuovo cum_socio
                    quoteGuadagno.cum_socio = quoteGuadagno.cum_socio + ActualContoCorrente.Ammontare;
                    // calcolo la nuova quota socio
                    quoteGuadagno.quota = quoteGuadagno.cum_socio / NuovoCumTotale;
                    quoteGuadagno.id_conto_corrente = ActualContoCorrente.Id_RowConto;
                    quoteGuadagno.ammontare = ActualContoCorrente.Ammontare;
                    // modifico i valori nel record quoteGuadagno
                    _quoteServices.ModifyQuoteGuadagno(quoteGuadagno);
                    // per ogni socio modifico il record standard per il calcolo delle quote
                    RegistrySociList registrySocis = _registryServices.GetSociList();
                    foreach (RegistrySoci soci in registrySocis)
                    {
                        if (soci.Id_Socio == ActualContoCorrente.Id_Socio) continue;
                        if (soci.Id_tipo_gestione != ActualContoCorrente.Id_Tipo_Gestione) continue;
                        // calcolo il nuovo totale
                        quoteGuadagno = _quoteServices.GetLastRecordBySocioValuta(soci.Id_Socio, ActualContoCorrente.Id_Valuta, ActualContoCorrente.Id_Tipo_Gestione);
                        quoteGuadagno.cum_totale = NuovoCumTotale;
                        // calcolo la nuova quota socio
                        quoteGuadagno.quota = quoteGuadagno.cum_socio / NuovoCumTotale;
                        // modifico il record delle quote guadagno
                        _quoteServices.ModifyQuoteGuadagno(quoteGuadagno);
                    }

                }
                // aggiorno la tabella con i guadagni totali -- 22-12-2022 da verificarne l'utilità IN OGNI caso la prima parte è superata
                // _quoteServices.UpdateGuadagniTotaleAnno(ActualContoCorrente.Id_Quote_Periodi, ActualContoCorrente.Id_Tipo_Gestione);
                MessageBox.Show("Il movimento di capitali è stato registrato con successo.", "Registrazione Capitali",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show(String.Format("La registrazione ha generato il seguente errore: {0} {1}", Environment.NewLine, err.Message), "Registrazione Capitali",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            ClearMe();
        }
        public bool CanModify(object param)
        {
            if (ActualContoCorrente.Id_RowConto > 0)
                return true;
            return false;
        }
        public void UpdateCommand(object param)
        {
            try
            {
                // prelevo l'ultimo record delle quote del socio con la stessa valuta
                QuoteGuadagno quoteGuadagno = _quoteServices.GetLastRecordBySocioValuta(ActualContoCorrente.Id_Socio, ActualContoCorrente.Id_Valuta, ActualContoCorrente.Id_Tipo_Gestione);
                // verifico se è inserimento o prelievo
                if (ActualContoCorrente.Id_tipo_movimento == 2)
                {
                    ActualContoCorrente.Ammontare = ActualContoCorrente.Ammontare > 0 ? ActualContoCorrente.Ammontare * -1 : ActualContoCorrente.Ammontare;
                    // verifico che il totale a disposizione del socio permetta il prelievo
                    if (quoteGuadagno.cum_socio + ActualContoCorrente.Ammontare < 0)
                    {
                        MessageBox.Show("Il prelievo supera la disponibilità.", "Gestioni Capitali", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }
                // prelevo il record prima di modificarlo
                ContoCorrente conto = _contoCorrenteServices.GetContoCorrenteByIdCC(ActualContoCorrente.Id_RowConto);
                // se ho cambiato la data devo anche aggiornare le quote per periodo per la vecchia data e poi per la nuova
                if (conto.DataMovimento != ActualContoCorrente.DataMovimento)
                {
                    // cambio le quote relative alla data pre-esistente
                    double NuovoCumTotale = quoteGuadagno.cum_totale - conto.Ammontare;
                    quoteGuadagno.cum_totale = NuovoCumTotale;
                    quoteGuadagno.cum_socio = quoteGuadagno.cum_socio - conto.Ammontare;
                    quoteGuadagno.quota = quoteGuadagno.cum_socio / NuovoCumTotale;
                    quoteGuadagno.id_conto_corrente = 0;
                    quoteGuadagno.ammontare = 0;
                    _quoteServices.ModifyQuoteGuadagno(quoteGuadagno);
                    // per ogni socio devo modficare le quote alla data pre-esistente
                    RegistrySociList registrySocis = _registryServices.GetSociList();
                    foreach (RegistrySoci soci in registrySocis)
                    {
                        if (soci.Id_Socio == conto.Id_Socio) continue;
                        quoteGuadagno = _quoteServices.GetLastRecordBySocioValuta(soci.Id_Socio, conto.Id_Valuta, conto.Id_Tipo_Gestione);
                        quoteGuadagno.cum_totale = NuovoCumTotale;
                        quoteGuadagno.quota = quoteGuadagno.cum_socio / NuovoCumTotale;
                        _quoteServices.ModifyQuoteGuadagno(quoteGuadagno);
                    }
                    // verifico se è una data già esistente
                    int result = Convert.ToInt16(_quoteServices.VerifyInvestmentDate(ActualContoCorrente, ActualContoCorrente.Id_Tipo_Gestione));
                    if (result == -1) //non è mai stato effettuato un versamento / prelievo in questa data
                    {
                        NuovoInserimento(_quoteServices.GetLastRecordBySocioValuta(ActualContoCorrente.Id_Socio, ActualContoCorrente.Id_Valuta, ActualContoCorrente.Id_Tipo_Gestione));
                    }
                    else
                    {
                        ActualContoCorrente.Id_Quote_Periodi = result;
                        // prelevo l'ultimo record del socio con la stessa valuta
                        quoteGuadagno = _quoteServices.GetLastRecordBySocioValuta(ActualContoCorrente.Id_Socio, ActualContoCorrente.Id_Valuta, ActualContoCorrente.Id_Tipo_Gestione);
                        // registro la modifica del conto
                        _contoCorrenteServices.UpdateRecordContoCorrente(ActualContoCorrente, Models.Enumeratori.TipologiaIDContoCorrente.IdContoCorrente);
                        // calcolo il nuovo totale
                        NuovoCumTotale = quoteGuadagno.cum_totale + ActualContoCorrente.Ammontare;
                        quoteGuadagno.cum_totale = NuovoCumTotale;
                        // calcolo il nuovo cum_socio
                        quoteGuadagno.cum_socio = quoteGuadagno.cum_socio + ActualContoCorrente.Ammontare;
                        // calcolo la nuova quota socio
                        quoteGuadagno.quota = quoteGuadagno.cum_socio / NuovoCumTotale;
                        quoteGuadagno.id_conto_corrente = ActualContoCorrente.Id_RowConto;
                        quoteGuadagno.ammontare = ActualContoCorrente.Ammontare;
                        // modifico i valori nel record quoteGuadagno
                        _quoteServices.ModifyQuoteGuadagno(quoteGuadagno);
                        // per ogni socio modifico il record standard per il calcolo delle quote
                        registrySocis = _registryServices.GetSociList();
                        foreach (RegistrySoci soci in registrySocis)
                        {
                            if (soci.Id_Socio == ActualContoCorrente.Id_Socio) continue;
                            // calcolo il nuovo totale
                            //                            quoteGuadagno = new QuoteGuadagno();
                            quoteGuadagno = _quoteServices.GetLastRecordBySocioValuta(soci.Id_Socio, ActualContoCorrente.Id_Valuta, ActualContoCorrente.Id_Tipo_Gestione);
                            quoteGuadagno.cum_totale = NuovoCumTotale;
                            // calcolo la nuova quota socio
                            quoteGuadagno.quota = quoteGuadagno.cum_socio / NuovoCumTotale;
                            // modifico il record delle quote guadagno
                            _quoteServices.ModifyQuoteGuadagno(quoteGuadagno);
                        }

                    }
                }
                else
                {
                    double NuovoCumTotale = quoteGuadagno.cum_totale - conto.Ammontare + ActualContoCorrente.Ammontare;
                    quoteGuadagno.cum_totale = NuovoCumTotale;
                    quoteGuadagno.cum_socio = quoteGuadagno.cum_socio - conto.Ammontare + ActualContoCorrente.Ammontare;
                    quoteGuadagno.quota = quoteGuadagno.cum_socio / NuovoCumTotale;
                    quoteGuadagno.ammontare = ActualContoCorrente.Ammontare;
                    _quoteServices.ModifyQuoteGuadagno(quoteGuadagno);
                    _contoCorrenteServices.UpdateRecordContoCorrente(ActualContoCorrente, Models.Enumeratori.TipologiaIDContoCorrente.IdContoCorrente);
                    // per ogni socio devo modficare le quote alla data pre-esistente
                    RegistrySociList registrySocis = _registryServices.GetSociList();
                    foreach (RegistrySoci soci in registrySocis)
                    {
                        if (soci.Id_Socio == conto.Id_Socio) continue;
                        quoteGuadagno = _quoteServices.GetLastRecordBySocioValuta(soci.Id_Socio, conto.Id_Valuta, conto.Id_Tipo_Gestione);
                        quoteGuadagno.cum_totale = NuovoCumTotale;
                        quoteGuadagno.quota = quoteGuadagno.cum_socio / NuovoCumTotale;
                        _quoteServices.ModifyQuoteGuadagno(quoteGuadagno);
                    }

                }
                MessageBox.Show("La modifica è stata registrata con successo.", "Registrazione Capitali",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show(String.Format("La registrazione ha generato il seguente errore: {0} {1}", Environment.NewLine, err.Message), "Registrazione Capitali",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            ClearMe();
        }
        private void NuovoInserimento(QuoteGuadagno quoteGuadagno)
        {
            QuotePeriodi quotePeriodi = _quoteServices.Update_InsertQuotePeriodi(ActualContoCorrente.DataMovimento, ActualContoCorrente.Id_Tipo_Gestione);
            ActualContoCorrente.Modified = quotePeriodi.DataInsert;
            ActualContoCorrente.Id_Quote_Periodi = quotePeriodi.IdPeriodoQuote;
            // registro l'inserimento
            _contoCorrenteServices.InsertAccountMovement(ActualContoCorrente);
            // riprendo il record inserito
            ActualContoCorrente = _contoCorrenteServices.GetLastContoCorrente();
            // calcolo il nuovo totale
            double NuovoCumTotale = quoteGuadagno.cum_totale + ActualContoCorrente.Ammontare;
            quoteGuadagno.cum_totale = NuovoCumTotale;
            // calcolo il nuovo cum_socio
            quoteGuadagno.cum_socio = quoteGuadagno.cum_socio + ActualContoCorrente.Ammontare;
            // calcolo la nuova quota socio
            quoteGuadagno.quota = quoteGuadagno.cum_socio / NuovoCumTotale;
            quoteGuadagno.id_conto_corrente = ActualContoCorrente.Id_RowConto;
            quoteGuadagno.ammontare = ActualContoCorrente.Ammontare;
            quoteGuadagno.id_quote_periodi = ActualContoCorrente.Id_Quote_Periodi;
            quoteGuadagno.id_socio = ActualContoCorrente.Id_Socio;
            quoteGuadagno.id_valuta = ActualContoCorrente.Id_Valuta;
            quoteGuadagno.id_tipo_gestione = ActualContoCorrente.Id_Tipo_Gestione;
            // inserisco il record delle quote guadagno
            _quoteServices.InsertRecordQuoteGuadagno(quoteGuadagno);
            // per ogni socio inserisco un record standard per il calcolo delle quote
            RegistrySociList registrySocis = _registryServices.GetSociList();
            foreach (RegistrySoci soci in registrySocis)
            {
                if (soci.Id_Socio == ActualContoCorrente.Id_Socio) continue;
                // calcolo il nuovo totale
                quoteGuadagno = _quoteServices.GetLastRecordBySocioValuta(soci.Id_Socio, ActualContoCorrente.Id_Valuta, ActualContoCorrente.Id_Tipo_Gestione);
                quoteGuadagno.cum_totale = NuovoCumTotale;
                // calcolo la nuova quota socio

                quoteGuadagno.quota = quoteGuadagno.cum_socio / NuovoCumTotale;
                quoteGuadagno.id_conto_corrente = 0;
                quoteGuadagno.ammontare = 0;
                quoteGuadagno.id_quote_periodi = ActualContoCorrente.Id_Quote_Periodi;
                quoteGuadagno.id_socio = soci.Id_Socio;
                quoteGuadagno.id_valuta = ActualContoCorrente.Id_Valuta;
                quoteGuadagno.id_tipo_gestione = ActualContoCorrente.Id_Tipo_Gestione;
                // inserisco il record delle quote guadagno
                _quoteServices.InsertRecordQuoteGuadagno(quoteGuadagno);
            }
        }
        private void RicalcoloUtili(object param)
        {
            try
            {
                QuotePeriodiList quotePeriodis = _quoteServices.GetQuotePeriodiList();
                // estraggo i record di conto_corrente in base a id_quote_periodi, id_tipo_gestione e valuta
                ContoCorrenteList contoCorrentes = _contoCorrenteServices.GetContoCorrenteList();
                // aggiorno l'id_quote_periodi utilizzando la data
                foreach (ContoCorrente conto in contoCorrentes)
                {
                    foreach (QuotePeriodi quotePeriodi in quotePeriodis)
                    {
                        if (conto.Id_Gestione == quotePeriodi.IdTipoGestione && conto.DataMovimento >= quotePeriodi.DataInizio && conto.DataMovimento <= quotePeriodi.DataFine)
                        {
                            if (conto.Id_Quote_Periodi != quotePeriodi.IdPeriodoQuote)
                            {
                                conto.Id_Quote_Periodi = quotePeriodi.IdPeriodoQuote;
                                _contoCorrenteServices.UpdateRecordContoCorrente(conto, Models.Enumeratori.TipologiaIDContoCorrente.IdContoCorrente);
                                _quoteServices.UpdateGuadagniTotaleAnno(conto.Id_RowConto);
                            }
                            break;
                        }
                    }
                }
                MessageBox.Show("Le tabelle conto_corrente e guadagni_totale_anno sono state ricalcolate", "Registrazione Capitali", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show(String.Format("Il ricalcolo ha generato il seguente errore: {0} {1}", Environment.NewLine, err.Message), "Registrazione Capitali",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool CanDoIt(object param)
        {
            return true;
        }
        #endregion
    }
}

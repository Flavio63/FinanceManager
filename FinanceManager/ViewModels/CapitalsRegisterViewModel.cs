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
                if (ActualContoCorrente.Id_tipo_movimento == 2)
                {
                    ActualContoCorrente.Ammontare = ActualContoCorrente.Ammontare < 0 ?  ActualContoCorrente.Ammontare * -1 : ActualContoCorrente.Ammontare;
                    // devo verificare di avere i soldi che sto togliendo dal conto finanza
                    ContoCorrenteList ccl = _contoCorrenteServices.GetTotalAmountByAccount(1, 0, ActualContoCorrente.Id_Socio,
                        (int)Models.Enumeratori.TipologiaSoldi.Capitale, ActualContoCorrente.Id_Valuta);
                    if (ccl[0].Ammontare > ActualContoCorrente.Ammontare)
                    {
                        MessageBox.Show("La cifra che vuoi prelevare è più grande di quella che possiedi! ;) ", "Registrazione Capitali",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    // verifico se alla stessa data con lo stesso tipo gestione ci siano altri prelievi
                    int result = Convert.ToInt16(_quoteServices.VerifyInvestmentDate(ActualContoCorrente, ActualContoCorrente.Id_Tipo_Gestione));
                    if (result == -1)
                    {
                        // non esiste alla stessa data con il tipo di gestione quindi devo aggiungere un nuovo periodo quote
                        QuotePeriodi quotePeriodi = _quoteServices.Update_InsertQuotePeriodi(ActualContoCorrente.DataMovimento, ActualContoCorrente.Id_Tipo_Gestione);
                        ActualContoCorrente.Modified = quotePeriodi.DataInsert;
                        ActualContoCorrente.Id_Quote_Periodi = quotePeriodi.IdPeriodoQuote;
                        // registro il prelievo
                        _contoCorrenteServices.InsertAccountMovement(ActualContoCorrente);
                        // inserisco la nuova quota di attribuzione utili
                        _quoteServices.ComputesAndInsertQuoteGuadagno(ActualContoCorrente.Id_Tipo_Gestione, ActualContoCorrente.Id_Quote_Periodi);
                    }
                    else
                    {
                        // è già stato fatto un inserimento pari data e tipo gestione quindi recupero il valore
                        ActualContoCorrente.Id_Quote_Periodi = result;
                        // registro il prelievo
                        _contoCorrenteServices.InsertAccountMovement(ActualContoCorrente);
                        // modifico le quote di attribuzione utili
                        _quoteServices.ComputesAndModifyQuoteGuadagno(ActualContoCorrente.Id_Tipo_Gestione);
                    }
                }
                else
                {
                    // determino chi sta inserendo e su quella base assegno il tipo di gestione utili
                    ActualContoCorrente.Id_Tipo_Gestione = ActualContoCorrente.Id_Socio == 3 ? 2 : 1;
                    // verifico se alla stessa data con lo stesso tipo gestione ci siano altri inserimenti
                    int result = Convert.ToInt16( _quoteServices.VerifyInvestmentDate(ActualContoCorrente, ActualContoCorrente.Id_Tipo_Gestione));
                    if (result == -1)
                    {
                        // non esiste alla stessa data con il tipo di gestione quindi devo aggiungere un nuovo periodo quote
                        QuotePeriodi quotePeriodi = _quoteServices.Update_InsertQuotePeriodi(ActualContoCorrente.DataMovimento, ActualContoCorrente.Id_Tipo_Gestione);
                        ActualContoCorrente.Modified = quotePeriodi.DataInsert;
                        ActualContoCorrente.Id_Quote_Periodi = quotePeriodi.IdPeriodoQuote;
                        // registro l'inserimento
                        _contoCorrenteServices.InsertAccountMovement(ActualContoCorrente);
                        // inserisco la nuova quota di attribuzione utili
                        _quoteServices.ComputesAndInsertQuoteGuadagno(ActualContoCorrente.Id_Tipo_Gestione, ActualContoCorrente.Id_Quote_Periodi);
                    }
                    else
                    {
                        // è già stato fatto un inserimento pari data e tipo gestione quindi recupero il valore
                        ActualContoCorrente.Id_Quote_Periodi = result;
                        // registro il prelievo
                        _contoCorrenteServices.InsertAccountMovement(ActualContoCorrente);
                        // modifico le quote di attribuzione utili
                        _quoteServices.ComputesAndModifyQuoteGuadagno(ActualContoCorrente.Id_Tipo_Gestione);
                    }
                    // si aggiorna il periodo quote in conto corrente e nel caso si deve ricalcolare la tabela dei guadagni totale anno
                }
                // aggiorno la tabella con i guadagni totali
                _quoteServices.UpdateGuadagniTotaleAnno(ActualContoCorrente.Id_Quote_Periodi, ActualContoCorrente.Id_Tipo_Gestione);
                MessageBox.Show("Il movimento di capitali è stato registrato con successo.", "Registrazione Capitali",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show(String.Format("La registrazione ha generato il seguente errore:", Environment.NewLine, err.Message), "Registrazione Capitali",
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
                if (ActualContoCorrente.Id_tipo_movimento == 2 && ActualContoCorrente.Ammontare > 0)
                    ActualContoCorrente.Ammontare = ActualContoCorrente.Ammontare * -1;
                ContoCorrente conto = _contoCorrenteServices.GetContoCorrenteByIdCC(ActualContoCorrente.Id_RowConto);
                int Id_Tipo_Gestione = conto.Id_Socio == 3 ? 2 : 1;
                // se ho cambiato la data devo anche aggiornare le quote per periodo
                if (conto.DataMovimento != ActualContoCorrente.DataMovimento)
                {
                    // verifico se è una data già esistente
                    object result = _quoteServices.VerifyInvestmentDate(ActualContoCorrente, Id_Tipo_Gestione); // verifico se alla stessa data c'è già un inserimento
                    if (result is long) //non è mai stato effettuato un versamento / prelievo in questa data
                    {
                        NuovoMovimento(Id_Tipo_Gestione);
                    }
                }
                // aggiorno il conto corrente con le modifiche inserite
                _contoCorrenteServices.UpdateRecordContoCorrente(ActualContoCorrente, Models.Enumeratori.TipologiaIDContoCorrente.IdContoCorrente);
                // se ho cambiato somma o data devo aggiornare le quote di competenza
                if (conto.Ammontare != ActualContoCorrente.Ammontare || conto.DataMovimento != ActualContoCorrente.DataMovimento)
                {
                    _quoteServices.ComputesAndModifyQuoteGuadagno(Id_Tipo_Gestione);
                    // aggiorno la tabella con i guadagni totali
                    _quoteServices.UpdateGuadagniTotaleAnno(ActualContoCorrente.Id_Quote_Periodi, Id_Tipo_Gestione);
                }
                MessageBox.Show("La modifica è stata registrata con successo.", "Registrazione Capitali",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show(String.Format("La registrazione ha generato il seguente errore:\r\n", err.Message), "Registrazione Capitali",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            ClearMe();
        }

        private void NuovoMovimento(int Id_Tipo_Gestione)
        {
            QuotePeriodi quotePeriodi = _quoteServices.Update_InsertQuotePeriodi(ActualContoCorrente.DataMovimento, Id_Tipo_Gestione);
            ActualContoCorrente.Modified = quotePeriodi.DataInsert;
            ActualContoCorrente.Id_Quote_Periodi = quotePeriodi.IdPeriodoQuote;
            if (ActualContoCorrente.Id_Socio != 3)
            {
                _contoCorrenteServices.InsertAccountMovement(ActualContoCorrente); // inserisco il nuovo movimento di capitali
                ActualContoCorrente.Id_Socio = ActualContoCorrente.Id_Socio == 1 ? 2 : 1;
                ActualContoCorrente.Ammontare = 0;
                ActualContoCorrente.Valore_Cambio = 0;
                ActualContoCorrente.Cod_Valuta = "";
                ActualContoCorrente.Causale = "Inserimento per Quote";
                _contoCorrenteServices.InsertAccountMovement(ActualContoCorrente); // inserisco il movimento a 0 per effettuare le quote corrette.
                _quoteServices.ComputesAndInsertQuoteGuadagno(Id_Tipo_Gestione, ActualContoCorrente.Id_Quote_Periodi);
            }
            else if (ActualContoCorrente.Id_Socio == 3)
            {
                _contoCorrenteServices.InsertAccountMovement(ActualContoCorrente); // inserisco il nuovo movimento di capitali
                ActualContoCorrente.Id_Socio = 1; ActualContoCorrente.Ammontare = 0; ActualContoCorrente.Causale = "Inserimento per Quota Aury";
                _contoCorrenteServices.InsertAccountMovement(ActualContoCorrente); // FLAVIO inserisco il movimento a 0 per effettuare le quote corrette.
                ActualContoCorrente.Id_Socio = 2;
                _contoCorrenteServices.InsertAccountMovement(ActualContoCorrente); // DANIELA inserisco il movimento a 0 per effettuare le quote corrette.
                _quoteServices.ComputesAndInsertQuoteGuadagno(Id_Tipo_Gestione, ActualContoCorrente.Id_Quote_Periodi);
            }

        }
        #endregion
    }
}

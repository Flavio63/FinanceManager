using FinanceManager.Events;
using FinanceManager.Services;
using FinanceManager.Views;
using FinanceManager.Models;
using System;
using System.Data;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace FinanceManager.ViewModels
{
    public class ManagerProfitViewModel : ViewModelBase
    {
        private IQuoteGuadagniServices _guadagniServices;
        private IRegistryServices _registryServices;
        private IContoCorrenteServices _contoCorrenteServices;
        public ICommand CloseMeCommand { get; set; }
        public ICommand ClearMeCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }

        public ManagerProfitViewModel(IQuoteGuadagniServices guadagniServices, IRegistryServices registryServices, IContoCorrenteServices contoCorrenteServices)
        {
            _guadagniServices = guadagniServices ?? throw new ArgumentNullException("Manca il link a QuoteGuadagniServices");
            _registryServices = registryServices ?? throw new ArgumentNullException("Manca il link a RegistryServices");
            _contoCorrenteServices = contoCorrenteServices ?? throw new ArgumentNullException("Manca il link a ContoCorrenteServices");
            CloseMeCommand = new CommandHandler(CloseMe);
            ClearMeCommand = new CommandHandler(ClearMe);
            InsertCommand = new CommandHandler(SaveCommand, CanSave);
            ModifyCommand = new CommandHandler(UpdateCommand, CanModify);
            Soci = _registryServices.GetSociList();
            Valuta = _registryServices.GetRegistryCurrencyList();
            Init();
        }

        private void Init()
        {
            Guadagni = _guadagniServices.GetTotaleCumulatoAnnoSocioValuta();
            ElencoPrelievi = _guadagniServices.GetMovimentiPrelievi();
            contoCorrente = new ContoCorrente();
            contoCorrente.Id_tipo_movimento = 2;
            Id_guadagno = 0;
        }

        #region Get&Set
        /// <summary>
        /// Riempie la tabella con i guadagni per socio
        /// </summary>
        public DataTable Guadagni
        {
            get { return GetValue(() => Guadagni); }
            set { SetValue(() => Guadagni, value); }
        }

        public DataTable ElencoPrelievi
        {
            get { return GetValue(() => ElencoPrelievi); }
            set { SetValue(() => ElencoPrelievi, value); }
        }

        public RegistrySociList Soci
        {
            get { return GetValue(() => Soci); }
            set { SetValue(() => Soci, value); }
        }

        public RegistryCurrencyList Valuta
        {
            get { return GetValue(() => Valuta); }
            set { SetValue(() => Valuta, value); }
        }

        public ContoCorrente contoCorrente
        {
            get { return GetValue(() => contoCorrente); }
            set { SetValue(() => contoCorrente, value); }
        }

        public ContoCorrente contoBackup
        {
            get { return GetValue(() => contoBackup); }
            set { SetValue(() => contoBackup, value); }
        }

        public int Id_guadagno
        {
            get { return GetValue(() => Id_guadagno); }
            private set { SetValue(() => Id_guadagno, value); }
        }

        #endregion

        #region events
        /// <summary>
        /// Imposto is campi sopra la griglia quando viene selezionata una riga
        /// nell'elenco sottostante
        /// </summary>
        /// <param name="sender">Grid dei dati</param>
        /// <param name="e">Cambio di selezione</param>
        public void GridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                contoBackup = new ContoCorrente();
                contoBackup.Id_tipo_movimento = 2;
                DataRowView dataRowView = ((DataRowView)((DataGrid)e.Source).CurrentItem);

                contoBackup.Id_RowConto = Convert.ToInt32(dataRowView.Row.ItemArray[8]);
                contoBackup.Id_Socio = Convert.ToInt32(dataRowView.Row.ItemArray[1]);
                contoBackup.Ammontare = Convert.ToDouble( dataRowView.Row.ItemArray[3]);
                contoBackup.Id_Valuta = Convert.ToInt32(dataRowView.Row.ItemArray[4]);
                contoBackup.DataMovimento = Convert.ToDateTime(dataRowView.Row.ItemArray[6]);
                contoBackup.Causale = dataRowView.Row.ItemArray[7].ToString();
                Id_guadagno = Convert.ToInt32(dataRowView.Row.ItemArray[0]);
                contoCorrente = contoBackup;
            }
            e.Handled = true;
        }
        /// <summary>
        /// Controlla che il punto del tastierino numerico venga trasformato in virgola
        /// </summary>
        /// <param name="sender">Tastiera</param>
        /// <param name="e">Pressione del tasto</param>
        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Name == "ammontare")
                if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
                {
                    var pos = textBox.SelectionStart;
                    textBox.Text = textBox.Text.Insert(pos, ",");
                    textBox.SelectionStart = pos + 1;
                    e.Handled = true;
                }
        }
        #endregion

        #region Commands
        private void CloseMe(object param)
        {
            ManagerProfitView view = param as ManagerProfitView;
            DockPanel wp = view.Parent as DockPanel;
            wp.Children.Remove(view);
        }

        private void SaveCommand(object param)
        {
            // verifico la disponibilità
            double disponibili = _guadagniServices.GetTotaleSocioValuta(contoCorrente.Id_Socio, contoCorrente.Id_Valuta);
            contoCorrente.Ammontare = contoCorrente.Ammontare > 0 ? contoCorrente.Ammontare * -1 : contoCorrente.Ammontare;
            if (disponibili + contoCorrente.Ammontare < 0)
            {
                MessageBox.Show("Non puoi prelevare più di quanto hai!", "Prelievo Utili",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else
            {
                try
                {
                    // registro in conto corrente
                    _contoCorrenteServices.InsertAccountMovement(contoCorrente);
                    try
                    {
                        // riprendo il record appena registrato e lo uso per
                        contoCorrente = _contoCorrenteServices.GetLastContoCorrente();
                        // registro in quote_guadagni_totale_anno
                        _guadagniServices.AddPrelievo(contoCorrente);
                        MessageBox.Show("Il prelievo è stato registrato correttamente", "Prelievo Utili", MessageBoxButton.OK, MessageBoxImage.Information);
                        Init();
                    }
                    catch (Exception err)
                    {
                        _contoCorrenteServices.DeleteRecordContoCorrente(contoCorrente.Id_RowConto);
                        MessageBox.Show(string.Format("Problemi nel caricamento del prelievo{0}{1}{2}{3}", Environment.NewLine,
                            "Il caricamento in conto corrente è stato annullato!", Environment.NewLine, err.Message), "Prelievo Utili",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        Init();
                    }
                }
                catch(Exception err)
                {
                    MessageBox.Show(string.Format("Problemi nel caricamento in conto corrente{0}{1}", Environment.NewLine, err.Message), "Prelievo Utili", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Init();
                }
            }
        }

        private void UpdateCommand(object param)
        {
            contoCorrente.Ammontare = contoCorrente.Ammontare > 0 ? contoCorrente.Ammontare * -1 : contoCorrente.Ammontare;
            try
            {
                _contoCorrenteServices.UpdateRecordContoCorrente(contoCorrente, Models.Enumeratori.TipologiaIDContoCorrente.IdContoCorrente);
                try
                {
                    _guadagniServices.ModifyPrelievo(contoCorrente, Id_guadagno);
                    Init();
                }
                catch (Exception err)
                {
                    _contoCorrenteServices.UpdateRecordContoCorrente(contoBackup, Models.Enumeratori.TipologiaIDContoCorrente.IdContoCorrente);
                    MessageBox.Show(string.Format("Problemi nella modifica in guadagni_totale_anno{0}{1}{2}{3}", Environment.NewLine, 
                        "L'aggiornamento su conto corrente è stato ripristinato", Environment.NewLine, 
                        err.Message), "Prelievo Utili", MessageBoxButton.OK, MessageBoxImage.Error);
                    Init();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("Problemi nella modifica in conto corrente{0}{1}", Environment.NewLine, err.Message), "Prelievo Utili",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Init();
            }
        }

        private void ClearMe(object param = null)
        {
            Init();
        }
        private bool CanSave(object param)
        {
            if (contoCorrente.Id_Socio > 0 && contoCorrente.Id_Valuta > 0 && contoCorrente.Ammontare > 0)
                return true;
            return false;
        }

        private bool CanModify(object param)
        {
            if (Id_guadagno > 0)
                return true;
            return false;
        }

        #endregion

    }
}

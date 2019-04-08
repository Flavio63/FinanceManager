using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class SchedeTitoliViewModel : ViewModelBase
    {
        private readonly IRegistryServices _registryServices;

        public ICommand CloseMeCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        Predicate<object> _Filter;

        public SchedeTitoliViewModel(IRegistryServices registryServices)
        {
            _registryServices = registryServices ?? throw new ArgumentNullException("SchedeTitoliViewModel senza registryServices");

            CloseMeCommand = new CommandHandler(CloseMe);
            InsertCommand = new CommandHandler(InsertData, CanInsertData);
            ClearCommand = new CommandHandler(ClearReport, CanClearForm);
            ModifyCommand = new CommandHandler(ModifyData, CanModifyData);
            DeleteCommand = new CommandHandler(DeleteData, CanModifyData);
            SetUpViewModel();
        }

        private void SetUpViewModel()
        {
            CanCompileNewRecord = true;
            SharesListView = new ListCollectionView(_registryServices.GetRegistryShareList());
            Firms = _registryServices.GetRegistryFirmList();
            TipoTitoli = _registryServices.GetRegistryShareTypeList();
            _Filter = new Predicate<object>(Filter);
            ActualRecord = new ShareSettori();
            SrchShares = "";
        }

        #region events
        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is RegistryShare RS)
                {
                    ActualRecord = _registryServices.GetTitoloCompletoById(RS.id_titolo);
                }
                else if (e.AddedItems[0] is RegistryFirm RF)
                {
                    ActualRecord.id_azienda = RF.id_azienda;
                }
                else if (e.AddedItems[0] is RegistryShareType RST)
                {
                    ActualRecord.id_tipo_titolo = RST.id_tipo_titolo;
                }
            }
        }

        /// <summary>
        /// Controlla che il punto del tastierino numerico e della tastiera
        /// venga trasformato in virgola ma solo per alcuni campi testo
        /// </summary>
        /// <param name="sender">Tastiera</param>
        /// <param name="e">Pressione del tasto</param>
        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Name.Contains("dbl"))
                if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
                {
                    int pos = textBox.SelectionStart;
                    textBox.Text = textBox.Text.Insert(pos, ",");
                    textBox.SelectionStart = pos + 1;
                    e.Handled = true;
                }
        }

        #endregion

        #region Getter&Setter
        /// <summary>
        /// E' il record nella maschera con id_titolo = 0 se nuovo
        /// </summary>
        public ShareSettori ActualRecord
        {
            get { return GetValue(() => ActualRecord); }
            set { SetValue(() => ActualRecord, value); }
        }
        
        /// <summary>
        /// E' il filtro da applicare all'elenco delle azioni
        /// e contestualmente al datagrid sottostante
        /// </summary>
        public bool Filter(object obj)
        {
            if (obj != null)
            {
                if (obj.GetType() == typeof(RegistryShare))
                {
                    var data = obj as RegistryShare;
                    if (!string.IsNullOrEmpty(SrchShares))
                        return data.Isin.ToUpper().Contains(SrchShares.ToUpper());
                }
            }
            return true;
        }

        /// <summary>
        /// La ricerca degli isin dei titoli per l'acquisto / vendita
        /// </summary>
        public string SrchShares
        {
            get { return GetValue(() => SrchShares); }
            set
            {
                SetValue(() => SrchShares, value);
                SharesListView.Filter = _Filter;
                SharesListView.Refresh();
            }
        }

        /// <summary>
        /// Combo box con i titoli da selezionare filtrato da SrchShares
        /// </summary>
        public ListCollectionView SharesListView
        {
            get { return GetValue(() => SharesListView); }
            set { SetValue(() => SharesListView, value); }
        }

        /// <summary>
        /// Combo box con le aziende
        /// </summary>
        public RegistryFirmList Firms
        {
            get { return GetValue(() => Firms); }
            private set { SetValue(() => Firms, value); }
        }

        /// <summary>
        /// Combo box con la tipologia dei titoli
        /// </summary>
        public RegistryShareTypeList TipoTitoli
        {
            get { return GetValue(() => TipoTitoli); }
            private set { SetValue(() => TipoTitoli, value); }
        }

        #endregion

        #region command
        public bool CanCompileNewRecord
        {
            get { return GetValue(() => CanCompileNewRecord); }
            private set { SetValue(() => CanCompileNewRecord, value); }
        }

        public bool CanInsertData(object param)
        {
            if (ActualRecord.id_titolo > 0)
                return false;
            return true;
        }

        public bool CanClearForm(object param)
        {
            return true;
        }

        public bool CanModifyData(object param)
        {
            if (ActualRecord.id_titolo > 0)
                return true;
            return false;
        }

        public void CloseMe(object param)
        {
            SchedeTitoliView MRV = param as SchedeTitoliView;
            DockPanel wp = MRV.Parent as DockPanel;
            wp.Children.Remove(MRV);
        }

        public void ClearReport(object param)
        {
            SetUpViewModel();
        }

        public void InsertData(object param)
        {
            try
            {
                ActualRecord.Isin = ActualRecord.Isin.ToUpper();
                _registryServices.AddShare(ActualRecord);
                SetUpViewModel();
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("C'è il seguente problema {0} nel caricare il titolo {1}", err, ActualRecord.desc_titolo), "Finance Manager - Scheda Titoli",
                     MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ModifyData(object param)
        {
            try
            {
                _registryServices.UpdateShare(ActualRecord);
                SetUpViewModel();
            }
            catch(Exception err)
            {
                MessageBox.Show(string.Format("C'è il seguente problema {0} nel modificare il titolo {1}", err, ActualRecord.desc_titolo), "Finance Manager - Scheda Titoli",
                     MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteData(object param)
        {
            MessageBoxResult risposta = MessageBox.Show(string.Format("Attenzione stai per eliminare {0} e non potrai tornare indietro. Voui procedere?", ActualRecord.desc_titolo),
                "Finance Manager - Scheda Titoli", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (risposta == MessageBoxResult.Yes)
                try
                {
                    _registryServices.DeleteShare(ActualRecord.id_titolo);
                    SetUpViewModel();
                }
                catch(Exception err)
                {
                    MessageBox.Show(string.Format("C'è il seguente problema {0} con l'eliminazione del titolo {1}.", err, ActualRecord.desc_titolo), "Finance Manager - Scheda Titoli",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }
        #endregion
    }
}

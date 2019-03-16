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
        public ICommand NuovoRecord { get; set; }
        Predicate<object> _Filter;

        public SchedeTitoliViewModel(IRegistryServices registryServices)
        {
            _registryServices = registryServices ?? throw new ArgumentNullException("SchedeTitoliViewModel senza registryServices");

            CloseMeCommand = new CommandHandler(CloseMe);
            InsertCommand = new CommandHandler(InsertData, CanInsertData);
            ClearCommand = new CommandHandler(ClearReport, CanClearForm);
            ModifyCommand = new CommandHandler(ModifyData, CanModifyData);
            DeleteCommand = new CommandHandler(DeleteData, CanModifyData);
            NuovoRecord = new CommandHandler(NewRecord, CanNewRecord);
            SetUpViewModel();
        }

        private void SetUpViewModel()
        {
            CanCompileNewRecord = true;
            SharesList = new ObservableCollection<RegistryShare>(_registryServices.GetRegistryShareList());
            _Filter = new Predicate<object>(Filter);
        }

        #region events
        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] is RegistryShare RS)
            {
            }

        }

        #endregion

        #region Getter&Setter
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

        public ObservableCollection<RegistryShare> SharesList
        {
            get { return GetValue(() => SharesList); }
            private set { SetValue(() => SharesList, value); SharesListView = new ListCollectionView(value); }
        }
        
        public bool CanCompileNewRecord
        {
            get { return GetValue(() => CanCompileNewRecord); }
            private set { SetValue(() => CanCompileNewRecord, value); }
        }
        #endregion

        #region command
        public bool CanNewRecord(object param)
        {
            return CanCompileNewRecord;
        }

        public bool CanInsertData(object param)
        {
            return true;
        }

        public bool CanClearForm(object param)
        {
            return true;
        }

        public bool CanModifyData(object param)
        {
            return true;
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

        public void NewRecord(object param)
        {

        }

        public void InsertData(object param)
        {

        }

        public void ModifyData(object param)
        {

        }

        public void DeleteData(object param)
        {

        }
        #endregion
    }
}

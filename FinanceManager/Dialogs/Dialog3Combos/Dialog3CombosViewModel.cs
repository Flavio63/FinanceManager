using FinanceManager.Dialogs.DialogService;
using FinanceManager.Events;
using FinanceManager.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinanceManager.Dialogs.Dialog3Combos
{
    public class Dialog3CombosViewModel : DialogViewModelBase
    {
        #region commands
        public ICommand YesCommand
        {
            get { return GetValue(() => YesCommand); }
            set { SetValue(() => YesCommand, value); }
        }

        public ICommand NoCommand
        {
            get { return GetValue(() => NoCommand); }
            set { SetValue(() => NoCommand, value); }
        }

        public ICommand CancelCommand
        {
            get { return GetValue(() => CancelCommand); }
            set { SetValue(() => CancelCommand, value); }
        }
        #endregion

        public Dialog3CombosViewModel(string message, RegistryLocationList locationList, RegistryOwnersList ownerList, TipoSoldiList tipoSoldis,
            RegistryLocation location = null, RegistryOwner owner = null, TipoSoldi soldi = null)
            : base (message)
        {
            RegistryLocations = locationList;
            RegistryOwners = ownerList;
            TipoSoldis = tipoSoldis;
            NoCommand = new CommandHandler(OnNoClicked);
            CancelCommand = new CommandHandler(OnCancelClicked);
            YesCommand = new CommandHandler(OnYesClicked);
            if (location == null || owner == null || soldi == null)
            {
                Id_Location = 0;
                Id_Owner = 0;
                Id_Soldi = 0;
            }
            else
            {
                Id_Location = location.Id_conto;
                Id_Owner = owner.Id_gestione;
                Id_Soldi = soldi.Id_Tipo_Soldi;
                Location = new RegistryLocation() { Id_conto = location.Id_conto, Desc_conto = location.Desc_conto };
                Owner = new RegistryOwner() { Id_gestione = owner.Id_gestione, Nome_Gestione = owner.Nome_Gestione };
                Soldi = new TipoSoldi() { Id_Tipo_Soldi = soldi.Id_Tipo_Soldi, Desc_Tipo_Soldi = soldi.Desc_Tipo_Soldi };
            }
        }

        public void CloseDialogWithResult(Window window, DialogResult result, RegistryLocation location, RegistryOwner owner, TipoSoldi soldi)
        {
            CloseDialogWithResult(window, result);
            this.Location = location;
            this.Owner = owner;
            this.Soldi = soldi;
        }

        private void OnYesClicked(object param)
        {
            CloseDialogWithResult(param as Window, DialogResult.Yes, Location, Owner, Soldi);
        }

        private void OnNoClicked(object param)
        {
            CloseDialogWithResult(param as Window, DialogResult.No);
        }

        private void OnCancelClicked(object param)
        {
            CloseDialogWithResult(param as Window, DialogResult.Cancel);
        }

        public RegistryLocation Location { get; set; }
        public RegistryOwner Owner { get; set; }
        public TipoSoldi Soldi { get; set; }

        public int Id_Location { get; set; }
        public int Id_Owner { get; set; }
        public int Id_Soldi { get; set; }

        public RegistryLocationList RegistryLocations { get; private set; }
        public RegistryOwnersList RegistryOwners { get; private set; }
        public TipoSoldiList TipoSoldis { get; private set; }

        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is RegistryLocation location)
                Location = location;
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is RegistryOwner owner)
                Owner = owner;
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is TipoSoldi soldi)
                Soldi = soldi;
        }
    }
}

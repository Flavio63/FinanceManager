using FinanceManager.Dialogs.DialogService;
using FinanceManager.Events;
using FinanceManager.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinanceManager.Dialogs.DialogYesNo
{
    public class DialogYesNoViewModel : DialogViewModelBase
    {
        private ICommand yesCommand = null;
        public ICommand YesCommand
        {
            get { return yesCommand; }
            set { yesCommand = value; }
        }

        private ICommand noCommand = null;
        public ICommand NoCommand
        {
            get { return noCommand; }
            set { noCommand = value; }
        }

        private RegistryLocation RegistryLocation { get; set; }
        private RegistryOwner RegistryOwner { get; set; }

        public DialogYesNoViewModel(string message, RegistryLocationList registryLocations, RegistryOwnersList registryOwners)
            : base(message, registryLocations, registryOwners)
        {
            this.yesCommand = new CommandHandler(OnYesClicked);
            this.noCommand = new CommandHandler(OnNoClicked);
        }

        private void OnYesClicked(object param)
        {
            this.CloseDialogWithResult(param as Window, DialogResult.Yes, RegistryLocation, RegistryOwner);
        }

        private void OnNoClicked(object param)
        {
            this.CloseDialogWithResult(param as Window, DialogResult.No);
        }

        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is RegistryLocation location)
                RegistryLocation = location;
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is RegistryOwner owner)
                RegistryOwner = owner;
        }
    }
}

using FinanceManager.Models;
using System.Windows;

namespace FinanceManager.Dialogs.DialogService
{
    public abstract class DialogViewModelBase
    {

        public DialogResult UserDialogResult
        {
            get;
            private set;
        }

        public void CloseDialogWithResult(Window dialog, DialogResult result)
        {
            this.UserDialogResult = result;
            if (dialog != null)
                dialog.DialogResult = true;
        }

        public void CloseDialogWithResult(Window dialog, DialogResult result, RegistryLocation location, RegistryOwner owner)
        {
            this.UserDialogResult = result;
            this.Location = location;
            this.Owner = owner;
            if (dialog != null)
                dialog.DialogResult = true;
        }

        public RegistryLocation Location
        {
            get;
            private set;
        }

        public RegistryOwner Owner
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            private set;
        }

        public RegistryLocationList LocationList
        {
            get;
            private set;
        }

        public RegistryOwnersList OwnerList
        {
            get;
            private set;
        }
        public DialogViewModelBase(string messagge, RegistryLocationList registryLocations, RegistryOwnersList registryOwners)
        {
            this.Message = messagge;
            this.LocationList = registryLocations;
            this.OwnerList = registryOwners;
        }
    }
}

using FinanceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FinanceManager.Dialogs.DialogService
{
    public static class DialogService
    {
        public static DialogResult OpenDialog(DialogViewModelBase vm, Window owner)
        {
            DialogWindow win = new DialogWindow();
            if (owner != null)
                win.Owner = owner;
            win.DataContext = vm;
            win.ShowDialog();
            DialogResult result = (win.DataContext as DialogViewModelBase).UserDialogResult;
            Location = (win.DataContext as DialogViewModelBase).Location;
            Owner = (win.DataContext as DialogViewModelBase).Owner;
            return result;
        }

        public static RegistryLocation Location
        {
            get;
            private set;
        }

        public static RegistryOwner Owner
        {
            get;
            private set;
        }
    }
}

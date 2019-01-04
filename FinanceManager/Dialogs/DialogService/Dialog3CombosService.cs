using FinanceManager.Dialogs.Dialog3Combos;
using FinanceManager.Models;
using System.Windows;

namespace FinanceManager.Dialogs.DialogService
{
    public static class Dialog3CombosService
    {
        public static DialogResult OpenDialog(Dialog3CombosViewModel vm, Window winOwner)
        {
            DialogWindow win = new DialogWindow();
            if (winOwner != null)
                win.Owner = winOwner;
            win.DataContext = vm;
            win.ShowDialog();
            DialogResult result = (win.DataContext as Dialog3CombosViewModel).UserDialogResult;
            Location = (win.DataContext as Dialog3CombosViewModel).Location;
            Owner = (win.DataContext as Dialog3CombosViewModel).Owner;
            Soldi = (win.DataContext as Dialog3CombosViewModel).Soldi;
            return result;
        }

        public static RegistryLocation Location { get; private set; }

        public static RegistryOwner Owner { get; private set; }

        public static TipoSoldi Soldi { get; private set; }
    }
}

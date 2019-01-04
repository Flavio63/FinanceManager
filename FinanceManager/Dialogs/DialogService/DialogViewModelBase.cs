using FinanceManager.Events;
using FinanceManager.Models;
using System.Windows;

namespace FinanceManager.Dialogs.DialogService
{
    public abstract class DialogViewModelBase : ViewModelBase
    {

        public DialogResult UserDialogResult
        {
            get;
            private set;
        }

        public virtual void CloseDialogWithResult(Window dialog, DialogResult result)
        {
            this.UserDialogResult = result;
            if (dialog != null)
                dialog.DialogResult = true;
        }

        public string Message { get; private set; }

        public DialogViewModelBase(string messagge)
        {
            this.Message = messagge;
        }
    }
}

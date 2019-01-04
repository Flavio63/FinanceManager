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

        public DialogYesNoViewModel(string message)
            : base(message)
        {
            this.yesCommand = new CommandHandler(OnYesClicked);
            this.noCommand = new CommandHandler(OnNoClicked);
        }

        private void OnYesClicked(object param)
        {
            this.CloseDialogWithResult(param as Window, DialogResult.Yes);
        }

        private void OnNoClicked(object param)
        {
            this.CloseDialogWithResult(param as Window, DialogResult.No);
        }
    }
}

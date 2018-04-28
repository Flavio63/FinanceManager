using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinanceManager.Events
{
    public class CommandHandler : ICommand
    {
        Action<object> _TargetExecuteMethod;
        Predicate<object> _TargetCanExecuteMethod;

        #region costruttori
        public CommandHandler(Action<object> executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }

        public CommandHandler(Action<object> executeMethod, Predicate<object> canExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod ?? throw new ArgumentNullException("executeMethod");
            _TargetCanExecuteMethod = canExecuteMethod;
        }
        #endregion

        #region implementazione di ICommand
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _TargetCanExecuteMethod == null ? true : _TargetCanExecuteMethod(parameter);
        }

        public void Execute(object parameter)
        {
            _TargetExecuteMethod(parameter);
        }
        #endregion
    }
}

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
        Action _TargetExecuteSimpleMethod;
        Func<bool> _TargetCanExecuteSimpleMethod;
        Action<object> _TargetExecuteMethod;
        Func<object, bool> _TargetCanExecuteMethod;

        #region costruttori
        public CommandHandler(Action executeMethod)
        {
            _TargetExecuteSimpleMethod = executeMethod;
        }

        public CommandHandler(Action executeMethod, Func<bool> canExecuteMethod = null)
        {
            _TargetExecuteSimpleMethod = executeMethod;
            _TargetCanExecuteSimpleMethod = canExecuteMethod;
        }

        public CommandHandler(Action<object> executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }

        public CommandHandler(Action<object> executeMethod, Func<object, bool> canExecuteMethod = null)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetCanExecuteMethod = canExecuteMethod;
        }
        #endregion

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        #region implementazione di ICommand
        public event EventHandler CanExecuteChanged = delegate { };

        public bool CanExecute(object parameter)
        {
            if (parameter != null)
            {
                if (_TargetCanExecuteMethod != null)
                {
                    return _TargetCanExecuteMethod(parameter);
                }

                if (_TargetExecuteMethod != null)
                {
                    return true;
                }
            }
            else
            {
                if (_TargetCanExecuteSimpleMethod != null)
                {
                    return _TargetCanExecuteSimpleMethod();
                }

                if (_TargetExecuteSimpleMethod != null)
                {
                    return true;
                }
            }
            return false;
        }

        public void Execute(object parameter)
        {
            if (parameter != null)
                _TargetExecuteMethod?.Invoke(parameter);
            else
                _TargetExecuteSimpleMethod?.Invoke();
        }
        #endregion
    }
}

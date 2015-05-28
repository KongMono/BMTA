using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace BMTA.Helper
{
    public class DelegateCommand<T> : ICommand
    {

        private Action<T> _command;
        private Func<T, bool> _canExecute;

        public DelegateCommand(Action<T> command)
            : this(command, (x) => true)
        {

        }

        public DelegateCommand(Action<T> command, Func<T, bool> canExecute)
        {
            _command = command;
            _canExecute = canExecute;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute.Invoke((T)parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
                _command.Invoke((T)parameter);
        }

        #endregion
    }
}

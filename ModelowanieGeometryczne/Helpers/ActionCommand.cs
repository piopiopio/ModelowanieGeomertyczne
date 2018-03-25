using System;
using System.Windows.Input;

namespace ModelowanieGeometryczne.Helpers
{
    public class ActionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action _action;

        public ActionCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter) { return true; }

        public void Execute(object parameter)
        {
            _action();
        }
    }
}


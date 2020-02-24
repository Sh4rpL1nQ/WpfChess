using System;
using System.Windows.Input;

namespace Chess
{
    public class ActionCommand : ICommand
    {
        public Action<object> Action { get; set; }
        public Predicate<object> Predicate { get; set; }

        public ActionCommand()
        {

        }

        public ActionCommand(Action<object> action) : this(action, null)
        {
        }

        public ActionCommand(Action<object> action, Predicate<object> predicate)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this.Action = action;
            this.Predicate = predicate;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (Predicate == null)
            {
                return true;
            }

            return Predicate(parameter);
        }

        public void Execute()
        {
            Execute(null);
        }

        public void Execute(object parameter)
        {
            Action(parameter);
        }
    }
}
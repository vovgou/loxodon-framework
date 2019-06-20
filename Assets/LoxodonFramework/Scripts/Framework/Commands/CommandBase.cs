using System;

namespace Loxodon.Framework.Commands
{
    public abstract class CommandBase : ICommand
    {
        private readonly object _lock = new object();
        private EventHandler canExecuteChanged;

        public event EventHandler CanExecuteChanged
        {
            add { lock (_lock) { this.canExecuteChanged += value; } }
            remove { lock (_lock) { this.canExecuteChanged -= value; } }
        }

        protected virtual void RaiseCanExecuteChanged()
        {
            var handler = this.canExecuteChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);
    }
}

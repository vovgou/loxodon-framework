using System;

namespace Loxodon.Framework.Commands
{
    public class SimpleCommand : ICommand
    {
        private bool enabled = true;
        private readonly Action execute;
        private readonly object _lock = new object();
        private EventHandler canExecuteChanged;

        public SimpleCommand(Action execute, bool keepStrongRef = false)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = keepStrongRef ? execute : execute.AsWeak();
        }

        public event EventHandler CanExecuteChanged
        {
            add { lock (_lock) { this.canExecuteChanged += value; } }
            remove { lock (_lock) { this.canExecuteChanged -= value; } }
        }

        public bool Enabled
        {
            get { return this.enabled; }
            set
            {
                if (this.enabled == value)
                    return;

                this.enabled = value;
                this.RaiseCanExecuteChanged();
            }
        }

        protected void RaiseCanExecuteChanged()
        {
            var handler = this.canExecuteChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return this.Enabled;
        }

        public virtual void Execute(object parameter)
        {
            if (this.CanExecute(parameter) && this.execute != null)
                this.execute();
        }
    }

    public class SimpleCommand<T> : ICommand
    {
        private bool enabled = true;
        private readonly Action<T> execute;
        private readonly object _lock = new object();
        private EventHandler canExecuteChanged;

        public SimpleCommand(Action<T> execute, bool keepStrongRef = false)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = keepStrongRef ? execute : execute.AsWeak();
        }

        public event EventHandler CanExecuteChanged
        {
            add { lock (_lock) { this.canExecuteChanged += value; } }
            remove { lock (_lock) { this.canExecuteChanged -= value; } }
        }

        public bool Enabled
        {
            get { return this.enabled; }
            set
            {
                if (this.enabled == value)
                    return;

                this.enabled = value;
                this.RaiseCanExecuteChanged();
            }
        }

        protected void RaiseCanExecuteChanged()
        {
            var handler = this.canExecuteChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return this.Enabled;
        }

        public virtual void Execute(object parameter)
        {
            if (this.CanExecute(parameter) && this.execute != null)
                this.execute((T)parameter);
        }
    }
}


using Loxodon.Framework.Commands;
using System;

namespace Loxodon.Framework.Binding.Commands
{
    public class ParameterWrapCommand : ICommand
    {
        private readonly object _lock = new object();
        private readonly ICommand wrappedCommand;
        private readonly object commandParameter;
        public ParameterWrapCommand(ICommand wrappedCommand, object commandParameter)
        {
            this.wrappedCommand = wrappedCommand;
            this.commandParameter = commandParameter;
        }

        public event EventHandler CanExecuteChanged
        {
            add { lock (_lock) { this.wrappedCommand.CanExecuteChanged += value; } }
            remove { lock (_lock) { this.wrappedCommand.CanExecuteChanged -= value; } }
        }

        public bool CanExecute(object parameter)
        {
            return wrappedCommand.CanExecute(commandParameter);
        }

        public void Execute(object parameter)
        {
            if (this.CanExecute(commandParameter))
                wrappedCommand.Execute(commandParameter);
        }
    }
}

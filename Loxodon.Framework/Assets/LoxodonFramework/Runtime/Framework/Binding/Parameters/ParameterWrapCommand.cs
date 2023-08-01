/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using Loxodon.Framework.Commands;
using System;

namespace Loxodon.Framework.Binding.Parameters
{
    public class ParameterWrapCommand : ParameterWrapBase, ICommand
    {
        private readonly object _lock = new object();
        private readonly ICommand wrappedCommand;
        public ParameterWrapCommand(ICommand wrappedCommand, ICommandParameter commandParameter) : base(commandParameter)
        {
            if (wrappedCommand == null)
                throw new ArgumentNullException("wrappedCommand");

            this.wrappedCommand = wrappedCommand;
        }

        public event EventHandler CanExecuteChanged
        {
            add { lock (_lock) { this.wrappedCommand.CanExecuteChanged += value; } }
            remove { lock (_lock) { this.wrappedCommand.CanExecuteChanged -= value; } }
        }

        public bool CanExecute(object parameter)
        {
            return wrappedCommand.CanExecute(GetParameterValue());
        }

        public void Execute(object parameter)
        {
            var param = GetParameterValue();
            if (wrappedCommand.CanExecute(param))
                wrappedCommand.Execute(param);
        }
    }

    public class ParameterWrapCommand<T> : ICommand
    {
        private readonly object _lock = new object();
        private readonly ICommand<T> wrappedCommand;
        private readonly ICommandParameter<T> commandParameter;
        public ParameterWrapCommand(ICommand<T> wrappedCommand, ICommandParameter<T> commandParameter)
        {
            if (wrappedCommand == null)
                throw new ArgumentNullException("wrappedCommand");
            if (commandParameter == null)
                throw new ArgumentNullException("commandParameter");

            this.commandParameter = commandParameter;
            this.wrappedCommand = wrappedCommand;
        }

        public event EventHandler CanExecuteChanged
        {
            add { lock (_lock) { this.wrappedCommand.CanExecuteChanged += value; } }
            remove { lock (_lock) { this.wrappedCommand.CanExecuteChanged -= value; } }
        }

        public bool CanExecute(object parameter)
        {
            return wrappedCommand.CanExecute(commandParameter.GetValue());
        }

        public void Execute(object parameter)
        {
            var param = commandParameter.GetValue();
            if (wrappedCommand.CanExecute(param))
                wrappedCommand.Execute(param);
        }
    }
}

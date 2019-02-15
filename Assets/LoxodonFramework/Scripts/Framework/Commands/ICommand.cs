using System;

namespace Loxodon.Framework.Commands
{
    /// <summary>
    /// 
    /// </summary>
	public interface ICommand
	{
		/// <summary>
		/// Occurs when can execute changed.
		/// </summary>
		event EventHandler CanExecuteChanged;

        /// <summary>
        /// Determines whether this instance can execute the specified parameter.
        /// </summary>
        /// <param name="parameter">Parameter.</param>
        /// <returns><c>true</c> if this instance can execute the specified parameter; otherwise, <c>false</c>.</returns>
        bool CanExecute (object parameter);

		/// <summary>
		/// Execute the specified parameter.
		/// </summary>
		/// <param name="parameter">Parameter.</param>
		void Execute (object parameter);
	}
}


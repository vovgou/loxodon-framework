namespace Loxodon.Framework.Views
{
    public interface IDialog 
    {
        /// <summary>
        /// Show the dialog box.
        /// </summary>
        void Show();

        /// <summary>
        /// Cancel the dialog box.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Suspend the coroutine,until the dialog box closed.
        /// eg:
        /// yiled return dialog.WaitForClosed();
        /// </summary>
        /// <returns></returns>
        object WaitForClosed();
    }
}

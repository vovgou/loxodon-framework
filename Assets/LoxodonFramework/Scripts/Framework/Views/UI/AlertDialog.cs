using System;

using Loxodon.Log;
using Loxodon.Framework.Execution;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Contexts;

namespace Loxodon.Framework.Views
{
    public class AlertDialog : IDialog
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AlertDialog));

        public const int BUTTON_POSITIVE = -1;
        public const int BUTTON_NEGATIVE = -2;
        public const int BUTTON_NEUTRAL = -3;

        private const string DEFAULT_VIEW_NAME = "UI/AlertDialog";

        private static string viewName;
        public static string ViewName
        {
            get { return string.IsNullOrEmpty(viewName) ? DEFAULT_VIEW_NAME : viewName; }
            set { viewName = value; }
        }

        /// <summary>
        /// Displays information to the user. 
        /// </summary>
        /// <param name="message">The message to be shown to the user.</param>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <returns>A AlertDialog.</returns>
        public static AlertDialog ShowMessage(
            string message,
            string title)
        {
            return ShowMessage(message, title, null, null, null, true, null);
        }

        /// <summary>
        /// Displays information to the user. 
        /// </summary>
        /// <param name="message">The message to be shown to the user.</param>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="buttonText">The text shown in the only button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="afterHideCallback">A callback that should be executed after
        /// the dialog box is closed by the user.</param>
        /// <returns>A AlertDialog.</returns>
        public static AlertDialog ShowMessage(
            string message,
            string title,
            string buttonText,
            Action<int> afterHideCallback)
        {
            return ShowMessage(message, title, buttonText, null, null, false, afterHideCallback);
        }

        /// <summary>
        /// Displays information to the user.
        /// </summary>
        /// <param name="message">The message to be shown to the user.</param>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="confirmButtonText">The text shown in the "confirm" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="cancelButtonText">The text shown in the "cancel" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="afterHideCallback">A callback that should be executed after
        /// the dialog box is closed by the user. The callback method will get a boolean
        /// parameter indicating if the "confirm" button (true) or the "cancel" button
        /// (false) was pressed by the user.</param>
        /// <returns>A AlertDialog.</returns>
        public static AlertDialog ShowMessage(
            string message,
            string title,
            string confirmButtonText,
            string cancelButtonText,
            Action<int> afterHideCallback)
        {
            return ShowMessage(message, title, confirmButtonText, null, cancelButtonText, false, afterHideCallback);
        }

        /// <summary>
        /// Displays information to the user. 
        /// </summary>
        /// <param name="message">The message to be shown to the user.</param>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="confirmButtonText">The text shown in the "confirm" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="neutralButtonText">The text shown in the "neutral" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="cancelButtonText">The text shown in the "cancel" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="canceledOnTouchOutside">Whether the dialog box is canceled when 
        /// touched outside the window's bounds. </param>
        /// <param name="afterHideCallback">A callback that should be executed after
        /// the dialog box is closed by the user. The callback method will get a boolean
        /// parameter indicating if the "confirm" button (true) or the "cancel" button
        /// (false) was pressed by the user.</param>
        /// <returns>A AlertDialog.</returns>
        public static AlertDialog ShowMessage(
            string message,
            string title,
            string confirmButtonText,
            string neutralButtonText,
            string cancelButtonText,
            bool canceledOnTouchOutside,
            Action<int> afterHideCallback)
        {
            AlertDialogViewModel viewModel = new AlertDialogViewModel();
            viewModel.Message = message;
            viewModel.Title = title;
            viewModel.ConfirmButtonText = confirmButtonText;
            viewModel.NeutralButtonText = neutralButtonText;
            viewModel.CancelButtonText = cancelButtonText;
            viewModel.CanceledOnTouchOutside = canceledOnTouchOutside;
            viewModel.Click = afterHideCallback;
            viewModel.Closed = false;

            ApplicationContext context = Context.GetApplicationContext();
            IUIViewLocator locator = context.GetService<IUIViewLocator>();
            if (locator == null)
            {
                if (log.IsWarnEnabled)
                    log.Warn("Not found the \"IUIViewLocator\".");

                throw new NotFoundException("Not found the \"IUIViewLocator\".");
            }
            AlertDialogWindow window = locator.LoadView<AlertDialogWindow>(ViewName);

            if (window == null)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("Not found the \"{0}\".", typeof(AlertDialogWindow).Name);

                throw new NotFoundException("Not found the \"AlertDialogWindow\".");
            }

            AlertDialog dialog = new AlertDialog(window, viewModel);
            dialog.Show();
            return dialog;
        }

        /// <summary>
        /// Displays information to the user. 
        /// </summary>
        /// <param name="contentView">The custom view to be shown to the user.</param>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="confirmButtonText">The text shown in the "confirm" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="neutralButtonText">The text shown in the "neutral" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="cancelButtonText">The text shown in the "cancel" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="canceledOnTouchOutside">Whether the dialog box is canceled when 
        /// touched outside the window's bounds. </param>
        /// <param name="afterHideCallback">A callback that should be executed after
        /// the dialog box is closed by the user. The callback method will get a boolean
        /// parameter indicating if the "confirm" button (true) or the "cancel" button
        /// (false) was pressed by the user.</param>
        /// <returns>A AlertDialog.</returns>
        public static AlertDialog ShowMessage(
            IUIView contentView,
            string title,
            string confirmButtonText,
            string neutralButtonText,
            string cancelButtonText,
            bool canceledOnTouchOutside,
            Action<int> afterHideCallback)
        {
            AlertDialogViewModel viewModel = new AlertDialogViewModel();
            viewModel.ContentView = contentView;
            viewModel.Title = title;
            viewModel.ConfirmButtonText = confirmButtonText;
            viewModel.NeutralButtonText = neutralButtonText;
            viewModel.CancelButtonText = cancelButtonText;
            viewModel.CanceledOnTouchOutside = canceledOnTouchOutside;
            viewModel.Click = afterHideCallback;
            viewModel.Closed = false;

            ApplicationContext context = Context.GetApplicationContext();
            IUIViewLocator locator = context.GetService<IUIViewLocator>();
            if (locator == null)
            {
                if (log.IsWarnEnabled)
                    log.Warn("Not found the \"IUIViewLocator\".");

                throw new NotFoundException("Not found the \"IUIViewLocator\".");
            }
            AlertDialogWindow window = locator.LoadView<AlertDialogWindow>(ViewName);

            if (window == null)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("Not found the \"{0}\".", typeof(AlertDialogWindow).Name);

                throw new NotFoundException("Not found the \"AlertDialogWindow\".");
            }

            AlertDialog dialog = new AlertDialog(window, viewModel);
            dialog.Show();
            return dialog;
        }

        private AlertDialogWindow window;
        private AlertDialogViewModel viewModel;

        public AlertDialog(AlertDialogWindow window, AlertDialogViewModel viewModel)
        {
            this.window = window;
            this.viewModel = viewModel;
        }

        public virtual object WaitForClosed()
        {
            return Executors.WaitWhile(() => !this.viewModel.Closed);
        }

        public void Show()
        {
            this.window.ViewModel = this.viewModel;
            this.window.Create();
            this.window.Show();
        }

        public void Cancel()
        {
            this.window.Cancel();
        }
    }

    public class AlertDialogViewModel : ViewModelBase
    {
        private string title;
        private string message;
        private IUIView contentView;
        private string confirmButtonText;
        private string neutralButtonText;
        private string cancelButtonText;
        private bool canceledOnTouchOutside;
        private bool closed;
        private Action<int> click;

        public string Title
        {
            get { return this.title; }
            set { this.Set<string>(ref this.title, value, "Title"); }
        }

        public string Message
        {
            get { return this.message; }
            set { this.Set<string>(ref this.message, value, "Message"); }
        }

        public IUIView ContentView
        {
            get { return this.contentView; }
            set { this.Set<IUIView>(ref this.contentView, value, "ContentView"); }
        }

        public string ConfirmButtonText
        {
            get { return this.confirmButtonText; }
            set { this.Set<string>(ref this.confirmButtonText, value, "ConfirmButtonText"); }
        }

        public string NeutralButtonText
        {
            get { return this.neutralButtonText; }
            set { this.Set<string>(ref this.neutralButtonText, value, "NeutralButtonText"); }
        }

        public string CancelButtonText
        {
            get { return this.cancelButtonText; }
            set { this.Set<string>(ref this.cancelButtonText, value, "CancelButtonText"); }
        }

        public bool CanceledOnTouchOutside
        {
            get { return this.canceledOnTouchOutside; }
            set { this.Set<bool>(ref this.canceledOnTouchOutside, value, "CanceledOnTouchOutside"); }
        }

        public Action<int> Click
        {
            get { return this.click; }
            set { this.Set<Action<int>>(ref this.click, value, "Click"); }
        }

        public bool Closed
        {
            get { return this.closed; }
            set { this.Set<bool>(ref this.closed, value, "Closed"); }
        }

        public void OnClick(int which)
        {
            try
            {
                var click = this.Click;
                if (click != null)
                    click(which);
            }
            catch (Exception) { }
            finally
            {
                this.Closed = true;
            }
        }
    }
}

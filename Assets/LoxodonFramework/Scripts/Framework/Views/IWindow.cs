using System;

namespace Loxodon.Framework.Views
{
    public enum WindowType
    {
        /// <summary>
        /// Full screen window.
        /// </summary>
        FULL,

        /// <summary>
        /// The pop-up window
        /// </summary>
        POPUP,

        /// <summary>
        /// Dialog window
        /// </summary>
        DIALOG,

        /// <summary>
        /// The progress bar dialog window
        /// </summary>
        PROGRESS 
    }

    public enum WindowState
    {
        NONE,
        CREATE_BEGIN,
        CREATE_END,
        ENTER_ANIMATION_BEGIN,
        VISIBLE,
        ENTER_ANIMATION_END,
        ACTIVATION_ANIMATION_BEGIN,
        ACTIVATED,
        ACTIVATION_ANIMATION_END,
        PASSIVATION_ANIMATION_BEGIN,
        PASSIVATED,
        PASSIVATION_ANIMATION_END,
        EXIT_ANIMATION_BEGIN,
        INVISIBLE,
        EXIT_ANIMATION_END,
        DISSMISS_BEGIN,
        DISMISS_END
    }

    public class WindowStateEventArgs : EventArgs
    {
        private WindowState state;
        public WindowStateEventArgs(WindowState state)
        {
            this.state = state;
        }

        public WindowState State { get { return this.state; } }
    }

    /// <summary>
    /// Window
    /// </summary>
    public interface IWindow
    {
        /// <summary>
        /// Triggered when the Visibility's value to be changed.
        /// </summary>
        event EventHandler VisibilityChanged;

        /// <summary>
        /// Triggered when the Activated's value to be changed.
        /// </summary>
        event EventHandler ActivatedChanged;

        /// <summary>
        /// Triggered when the window is dismissed.
        /// </summary>
        event EventHandler OnDismissed;

        /// <summary>
        /// Triggered when the WindowState's value to be changed.
        /// </summary>
        event EventHandler<WindowStateEventArgs> StateChanged;

        /// <summary>
        /// The name of the window.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Returns  "true" if this window created.
        /// </summary>
        bool Created { get; }

        /// <summary>
        /// Returns  "true" if this window dismissed.
        /// </summary>
        bool Dismissed { get; }

        /// <summary>
        /// Returns  "true" if this window visibility.
        /// </summary>
        bool Visibility { get; }

        /// <summary>
        /// Returns  "true" if this window activated.
        /// </summary>
        bool Activated { get; }

        /// <summary>
        /// window type.
        /// </summary>
        WindowType WindowType { get; set; }

        /// <summary>
        /// Create window
        /// </summary>
        /// <param name="bundle"></param>
        void Create(IBundle bundle = null);

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        ITransition Show(bool ignoreAnimation = false);

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        ITransition Hide(bool ignoreAnimation = false);

        /// <summary>
        /// 
        /// </summary>
        ITransition Dismiss(bool ignoreAnimation = false);

    }
}

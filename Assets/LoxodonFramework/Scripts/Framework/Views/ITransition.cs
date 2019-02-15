using System;

namespace Loxodon.Framework.Views
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITransition
    {
        /// <summary>
        /// Returns  "true" if this transition finished.
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        /// Wait for the result,suspends the coroutine execution.
        /// eg: yield return transition.WaitForDone();
        /// </summary>
        object WaitForDone();

        /// <summary>
        /// Disable animation
        /// </summary>
        /// <param name="disabled"></param>
        /// <returns></returns>
        ITransition DisableAnimation(bool disabled);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        ITransition OnStart(Action callback);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        ITransition OnStateChanged(Action<IWindow, WindowState> callback);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        ITransition OnFinish(Action callback);
    }
}

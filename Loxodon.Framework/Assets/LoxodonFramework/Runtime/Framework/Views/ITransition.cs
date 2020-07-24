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

using Loxodon.Framework.Asynchronous;
using System;

namespace Loxodon.Framework.Views
{
    public enum ActionType
    {
        None,
        Hide,
        Dismiss
    }

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

#if NETFX_CORE || NET_STANDARD_2_0 || NET_4_6
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IAwaiter GetAwaiter();
#endif

        /// <summary>
        /// Disable animation
        /// </summary>
        /// <param name="disabled"></param>
        /// <returns></returns>
        ITransition DisableAnimation(bool disabled);

        /// <summary>
        /// Sets the layer of the window in the window manager, 0 is the top layer.
        /// This method is only valid when showing a window.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        ITransition AtLayer(int layer);

        /// <summary>
        /// Sets a processing policy. When a window is covered, hide it, dismiss it or do nothing.
        /// This method is only valid when showing a window.
        /// </summary>
        /// <example>
        /// This is an example, the default processing policy is as follows:
        /// <code>
        /// (previous,current) => {
        ///     if (previous == null || previous.WindowType == WindowType.FULL)
        ///         return ActionType.None;
        ///     if (previous.WindowType == WindowType.POPUP)    
        ///         return ActionType.Dismiss;
        ///     return ActionType.None;
        /// }
        /// </code>
        /// </example>
        /// <param name="policy"></param>
        /// <returns></returns>
        ITransition Overlay(Func<IWindow, IWindow, ActionType> policy);

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

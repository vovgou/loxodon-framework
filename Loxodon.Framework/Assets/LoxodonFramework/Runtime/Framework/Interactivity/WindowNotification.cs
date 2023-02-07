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

namespace Loxodon.Framework.Interactivity
{
    public class WindowNotification
    {
        public static WindowNotification CreateShowNotification(bool ignoreAnimation = true, bool waitDismissed = false)
        {
            return new WindowNotification(ActionType.SHOW, ignoreAnimation, null, waitDismissed);
        }

        public static WindowNotification CreateShowNotification(object viewModel, bool ignoreAnimation = true, bool waitDismissed = false)
        {
            return new WindowNotification(ActionType.SHOW, ignoreAnimation, viewModel, waitDismissed);
        }

        public static WindowNotification CreateHideNotification(bool ignoreAnimation = true)
        {
            return new WindowNotification(ActionType.HIDE, ignoreAnimation);
        }

        public static WindowNotification CreateDismissNotification(bool ignoreAnimation = true)
        {
            return new WindowNotification(ActionType.DISMISS, ignoreAnimation);
        }

        public WindowNotification(ActionType actionType) : this(actionType, true, null)
        {
        }

        public WindowNotification(ActionType actionType, bool ignoreAnimation) : this(actionType, ignoreAnimation, null)
        {
        }

        public WindowNotification(ActionType actionType, object viewModel, bool waitDismissed = false) : this(actionType, true, viewModel, waitDismissed)
        {
        }

        public WindowNotification(ActionType actionType, bool ignoreAnimation, object viewModel, bool waitDismissed = false)
        {
            this.IgnoreAnimation = ignoreAnimation;
            this.ActionType = actionType;
            this.ViewModel = viewModel;
            this.WaitDismissed = waitDismissed;
        }

        public bool IgnoreAnimation { get; private set; }

        public ActionType ActionType { get; private set; }

        public object ViewModel { get; private set; }

        public bool WaitDismissed { get; private set; }
    }

    public enum ActionType
    {
        CREATE,
        SHOW,
        HIDE,
        DISMISS
    }
}

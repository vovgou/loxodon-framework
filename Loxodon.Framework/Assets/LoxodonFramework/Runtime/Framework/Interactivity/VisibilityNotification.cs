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
    public class VisibilityNotification
    {
        public static VisibilityNotification CreateShowNotification(bool waitDisabled = false)
        {
            return new VisibilityNotification(true, null, waitDisabled);
        }

        public static VisibilityNotification CreateShowNotification(object viewModel, bool waitDisabled = false)
        {
            return new VisibilityNotification(true, viewModel, waitDisabled);
        }

        public static VisibilityNotification CreateHideNotification()
        {
            return new VisibilityNotification(false);
        }

        public bool Visible { get; private set; }
        public object ViewModel { get; private set; }
        public bool WaitDisabled { get; private set; }

        public VisibilityNotification(bool visible) : this(visible, null)
        {
        }

        public VisibilityNotification(bool visible, object viewModel) : this(visible, viewModel, false)
        {
        }

        public VisibilityNotification(bool visible, object viewModel, bool waitDisabled)
        {
            this.Visible = visible;
            this.ViewModel = viewModel;
            this.WaitDisabled = waitDisabled;
        }
    }
}

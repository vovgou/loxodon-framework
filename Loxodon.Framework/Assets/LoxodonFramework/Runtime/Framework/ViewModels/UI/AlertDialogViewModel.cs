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

using System;

namespace Loxodon.Framework.ViewModels
{
    public class AlertDialogViewModel : ViewModelBase
    {
        protected string title;
        protected string message;
        protected string confirmButtonText;
        protected string neutralButtonText;
        protected string cancelButtonText;
        protected bool canceledOnTouchOutside;
        protected bool closed;
        protected int result;
        protected Action<int> click;

        /// <summary>
        /// The title of the dialog box. This may be null.
        /// </summary>
        public virtual string Title
        {
            get { return this.title; }
            set { this.Set(ref this.title, value); }
        }

        /// <summary>
        /// The message to be shown to the user.
        /// </summary>
        public virtual string Message
        {
            get { return this.message; }
            set { this.Set(ref this.message, value); }
        }

        /// <summary>
        /// The text shown in the "confirm" button in the dialog box. 
        /// If left null, the button will be invisible.
        /// </summary>
        public virtual string ConfirmButtonText
        {
            get { return this.confirmButtonText; }
            set { this.Set(ref this.confirmButtonText, value); }
        }

        /// <summary>
        /// The text shown in the "neutral" button in the dialog box. 
        /// If left null, the button will be invisible.
        /// </summary>
        public virtual string NeutralButtonText
        {
            get { return this.neutralButtonText; }
            set { this.Set(ref this.neutralButtonText, value); }
        }

        /// <summary>
        /// The text shown in the "cancel" button in the dialog box. 
        /// If left null, the button will be invisible.
        /// </summary>
        public virtual string CancelButtonText
        {
            get { return this.cancelButtonText; }
            set { this.Set(ref this.cancelButtonText, value); }
        }

        /// <summary>
        /// Whether the dialog box is canceled when 
        /// touched outside the window's bounds. 
        /// </summary>
        public virtual bool CanceledOnTouchOutside
        {
            get { return this.canceledOnTouchOutside; }
            set { this.Set(ref this.canceledOnTouchOutside, value); }
        }

        /// <summary>
        /// A callback that should be executed after
        /// the dialog box is closed by the user. The callback method will get a boolean
        /// parameter indicating if the "confirm" button (true) or the "cancel" button
        /// (false) was pressed by the user.
        /// </summary>
        public virtual Action<int> Click
        {
            get { return this.click; }
            set { this.Set(ref this.click, value); }
        }

        /// <summary>
        /// The dialog box has been closed.
        /// </summary>
        public virtual bool Closed
        {
            get { return this.closed; }
            protected set { this.Set(ref this.closed, value); }
        }

        /// <summary>
        /// result
        /// </summary>
        public virtual int Result
        {
            get { return this.result; }
        }

        public virtual void OnClick(int which)
        {
            try
            {
                this.result = which;
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

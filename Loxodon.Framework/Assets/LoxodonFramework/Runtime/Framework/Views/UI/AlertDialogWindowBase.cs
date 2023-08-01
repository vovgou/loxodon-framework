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

using Loxodon.Framework.ViewModels;
using UnityEngine;

namespace Loxodon.Framework.Views
{
    public abstract class AlertDialogWindowBase : Window
    {
        public GameObject Content;

        protected IUIView contentView;

        protected AlertDialogViewModel viewModel;

        public virtual IUIView ContentView
        {
            get { return this.contentView; }
            set
            {
                if (this.contentView == value)
                    return;

                if (this.contentView != null)
                    GameObject.Destroy(this.contentView.Owner);

                this.contentView = value;
                if (this.contentView != null && this.contentView.Owner != null && this.Content != null)
                {
                    this.contentView.Visibility = true;
                    this.contentView.Transform.SetParent(this.Content.transform, false);
                }
            }
        }

        public virtual AlertDialogViewModel ViewModel
        {
            get { return this.viewModel; }
            set
            {
                if (this.viewModel == value)
                    return;

                this.viewModel = value;
                this.OnChangeViewModel();
            }
        }
        protected override void OnCreate(IBundle bundle)
        {
            this.WindowType = WindowType.DIALOG;
        }

        protected abstract void OnChangeViewModel();

        public abstract void Cancel();
    }
}

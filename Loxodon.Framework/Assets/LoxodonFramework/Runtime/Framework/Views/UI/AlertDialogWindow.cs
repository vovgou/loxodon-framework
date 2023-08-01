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
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Views
{
    public class AlertDialogWindow : AlertDialogWindowBase
    {
        public Text Title;

        public Text Message;

        public Button ConfirmButton;

        public Button NeutralButton;

        public Button CancelButton;

        public Button OutsideButton;

        public bool CanceledOnTouchOutside { get; set; }

        public override IUIView ContentView
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
                    if (this.Message != null)
                        this.Message.gameObject.SetActive(false);
                }
            }
        }

        protected virtual void Button_OnClick(int which)
        {
            try
            {
                this.viewModel.OnClick(which);
            }
            catch (Exception) { }
            finally
            {
                this.Dismiss();
            }
        }

        public override void Cancel()
        {
            this.Button_OnClick(AlertDialog.BUTTON_NEGATIVE);
        }

        protected override void OnCreate(IBundle bundle)
        {
            this.WindowType = WindowType.DIALOG;
        }

        protected override void OnChangeViewModel()
        {
            if (this.Message != null)
            {
                if (!string.IsNullOrEmpty(this.viewModel.Message))
                {
                    this.Message.gameObject.SetActive(true);
                    this.Message.text = this.viewModel.Message;
                    if (this.contentView != null && this.contentView.Visibility)
                        this.contentView.Visibility = false;
                }
                else
                    this.Message.gameObject.SetActive(false);
            }

            if (this.Title != null)
            {
                if (!string.IsNullOrEmpty(this.viewModel.Title))
                {
                    this.Title.gameObject.SetActive(true);
                    this.Title.text = this.viewModel.Title;
                }
                else
                    this.Title.gameObject.SetActive(false);
            }

            if (this.ConfirmButton != null)
            {
                if (!string.IsNullOrEmpty(this.viewModel.ConfirmButtonText))
                {
                    this.ConfirmButton.gameObject.SetActive(true);
                    this.ConfirmButton.onClick.AddListener(() => { this.Button_OnClick(AlertDialog.BUTTON_POSITIVE); });
                    Text text = this.ConfirmButton.GetComponentInChildren<Text>();
                    if (text != null)
                        text.text = this.viewModel.ConfirmButtonText;
                }
                else
                {
                    this.ConfirmButton.gameObject.SetActive(false);
                }
            }

            if (this.CancelButton != null)
            {
                if (!string.IsNullOrEmpty(this.viewModel.CancelButtonText))
                {
                    this.CancelButton.gameObject.SetActive(true);
                    this.CancelButton.onClick.AddListener(() => { this.Button_OnClick(AlertDialog.BUTTON_NEGATIVE); });
                    Text text = this.CancelButton.GetComponentInChildren<Text>();
                    if (text != null)
                        text.text = this.viewModel.CancelButtonText;
                }
                else
                {
                    this.CancelButton.gameObject.SetActive(false);
                }
            }

            if (this.NeutralButton != null)
            {
                if (!string.IsNullOrEmpty(this.viewModel.NeutralButtonText))
                {
                    this.NeutralButton.gameObject.SetActive(true);
                    this.NeutralButton.onClick.AddListener(() => { this.Button_OnClick(AlertDialog.BUTTON_NEUTRAL); });
                    Text text = this.NeutralButton.GetComponentInChildren<Text>();
                    if (text != null)
                        text.text = this.viewModel.NeutralButtonText;
                }
                else
                {
                    this.NeutralButton.gameObject.SetActive(false);
                }
            }

            this.CanceledOnTouchOutside = this.viewModel.CanceledOnTouchOutside;
            if (this.OutsideButton != null && this.CanceledOnTouchOutside)
            {
                this.OutsideButton.gameObject.SetActive(true);
                this.OutsideButton.interactable = true;
                this.OutsideButton.onClick.AddListener(() => { this.Button_OnClick(AlertDialog.BUTTON_NEGATIVE); });
            }
        }
    }
}

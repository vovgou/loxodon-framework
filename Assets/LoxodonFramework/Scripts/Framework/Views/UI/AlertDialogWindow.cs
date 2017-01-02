using System;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Views
{
    public class AlertDialogWindow : Window
    {
        public Text Title;

        public Text Message;

        public GameObject Content;

        public Button ConfirmButton;

        public Button NeutralButton;

        public Button CancelButton;

        public Button OutsideButton;

        public bool CanceledOnTouchOutside { get; set; }

        private IUIView contentView;

        private AlertDialogViewModel viewModel;

        public IUIView ContentView
        {
            get { return this.contentView; }
            set
            {
                this.contentView = value;
                if (this.contentView != null && this.contentView.Owner != null && this.Content != null)
                {
                    this.contentView.Transform.SetParent(this.Content.transform, false);
                }
            }
        }

        public AlertDialogViewModel ViewModel
        {
            get { return this.viewModel; }
            set
            {
                this.viewModel = value;
                this.OnChangeViewModel();
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

        public virtual void Cancel()
        {
            this.Button_OnClick(AlertDialog.BUTTON_NEGATIVE);
        }

        protected override void OnCreate(IBundle bundle)
        {
            this.WindowType = WindowType.DIALOG;
        }

        protected void OnChangeViewModel()
        {
            if (this.Message != null)
            {
                if (!string.IsNullOrEmpty(this.viewModel.Message))
                    this.Message.text = this.viewModel.Message;
                else
                    this.Message.gameObject.SetActive(false);
            }

            if (this.Content != null && this.viewModel.ContentView != null)
            {
                this.ContentView = this.viewModel.ContentView;
            }

            if (this.Title != null)
            {
                if (!string.IsNullOrEmpty(this.viewModel.Title))
                    this.Title.text = this.viewModel.Title;
                else
                    this.Title.gameObject.SetActive(false);
            }

            if (this.ConfirmButton != null)
            {
                if (!string.IsNullOrEmpty(this.viewModel.ConfirmButtonText))
                {
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
                this.OutsideButton.interactable = true;
                this.OutsideButton.onClick.AddListener(() => { this.Button_OnClick(AlertDialog.BUTTON_NEGATIVE); });
            }
        }
    }
}

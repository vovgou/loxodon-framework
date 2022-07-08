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

using Loxodon.Framework.Binding;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Loxodon.Framework.Examples
{
    public class Window1 : UIToolkitWindow
    {
        private IUIViewLocator locator;
        protected override void OnCreate(IBundle bundle)
        {
            WindowViewMode viewModel = new WindowViewMode();
            VisualElement listContainer = this.Q<VisualElement>("listContainer");
            var listView = new ListView(viewModel.Items);
            listView.makeItem = () => new Label();
            listView.bindItem = (e, i) => (e as Label).text = viewModel.Items[i];
            listView.selectionType = SelectionType.Multiple;
            listView.onItemsChosen += obj => Debug.Log(obj);
            listView.onSelectionChange += objects => Debug.Log(objects);
            listView.style.flexGrow = 1.0f;
            listContainer.Add(listView);

            this.locator = Context.GetApplicationContext().GetService<IUIViewLocator>();

            var bindingSet = this.CreateBindingSet(viewModel);
            bindingSet.Bind(this.Q<Button>("openDialog")).For(v => v.clickable).To(vm => vm.DialogClick);
            bindingSet.Bind(this.Q<Button>("openWindow")).For(v => v.clickable).To(vm => vm.WindowClick);
            bindingSet.Bind(this.Q<Button>("addItem")).For(v => v.clickable).To(vm => vm.AddItem);
            //bindingSet.Bind(listView).For(v => v.itemsSource).To(vm => vm.Items);
            bindingSet.Bind().For(v => v.OnOpenDialogWindow).To(vm => vm.OpenDialogRequest);
            bindingSet.Bind().For(v => v.OnOpenWindow).To(vm => vm.OpenWindowRequest);

            //如果是分子视图来开发，UIToolkit的子视图没法挂脚本，可以通过绑定集的ScopeKey来管理子视图的绑定
            //bindingSet.Bind().For(v => v.OnOpenWindow).To(vm => vm.OpenWindowRequest).WithScopeKey("xxSubView");

            bindingSet.Build();

            this.Q<Button>("close").clicked += () => this.Dismiss();
        }

        protected void OnOpenDialogWindow(object sender, InteractionEventArgs args)
        {
            var callback = args.Callback;
            AlertDialog.ShowMessage("测试", "标题", "OK", r =>
            {
                callback?.Invoke();
            });
        }

        protected void OnOpenWindow(object sender, InteractionEventArgs args)
        {
            IWindow window = locator.LoadWindow<IWindow>(this.WindowManager, "UI/Window2");
            window.Create();
            window.Show();
        }
    }

    public class WindowViewMode : ViewModelBase
    {
        private string name;
        private SimpleCommand dialogCommand;
        private SimpleCommand windowCommand;
        private InteractionRequest openDialogRequest;
        private InteractionRequest openWindowRequest;
        public readonly Observables.ObservableList<string> Items = new Observables.ObservableList<string>();
        public WindowViewMode()
        {
            this.openDialogRequest = new InteractionRequest(this);
            this.openWindowRequest = new InteractionRequest(this);
            this.dialogCommand = new SimpleCommand(() =>
            {
                this.dialogCommand.Enabled = false;
                this.openDialogRequest.Raise(() =>
                {
                    this.dialogCommand.Enabled = true;
                });
            });
            this.windowCommand = new SimpleCommand(() =>
            {
                this.openWindowRequest.Raise();
            });

            for (int i = 1; i <= 5; i++)
                Items.Add(i.ToString());
        }

        public string Name
        {
            get { return this.name; }
            set { this.Set<string>(ref name, value); }
        }

        public IInteractionRequest OpenDialogRequest
        {
            get { return this.openDialogRequest; }
        }

        public IInteractionRequest OpenWindowRequest
        {
            get { return this.openWindowRequest; }
        }

        public ICommand DialogClick
        {
            get { return this.dialogCommand; }
        }

        public ICommand WindowClick
        {
            get { return this.windowCommand; }
        }

        public void OnClick()
        {
            Debug.LogFormat("Button OnClick");
        }

        public void AddItem()
        {
            Items.Add("Item_" + Random.Range(1, 200));
        }
    }
}

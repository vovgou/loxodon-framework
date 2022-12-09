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

using Com.TheFallenGames.OSA.Demos.Common;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Observables;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Tutorials.OSA
{
    public class GridViewExampleViewModel : ViewModelBase
    {
        private int id = 0;
        private ObservableList<ItemViewModel> items;
        private ItemViewModel selectedItem;
        private SimpleCommand<ItemViewModel> itemSelectCommand;
        public GridViewExampleViewModel()
        {
            itemSelectCommand = new SimpleCommand<ItemViewModel>(OnItemSelect);
            this.items = this.CreateItems(3);
        }

        public ObservableList<ItemViewModel> Items
        {
            get { return this.items; }
            set { this.Set(ref items, value); }
        }

        public ItemViewModel SelectedItem
        {
            get { return this.selectedItem; }
            set { Set(ref selectedItem, value); }
        }

        public void AddItem()
        {
            items.Add(CreateItem());
        }

        public void ChangeItem()
        {
            if (items != null && items.Count > 0)
            {
                var model = items[0];
                model.Color = DemosUtil.GetRandomColor();
            }
        }

        public void MoveItem()
        {
            if (items != null && items.Count > 1)
            {
                items.Move(0, items.Count - 1);
            }
        }

        public void ResetItem()
        {
            items.Clear();
        }

        private void OnItemSelect(ItemViewModel item)
        {
            item.Selected = !item.Selected;
            if (item.Selected)
                this.SelectedItem = item;

            if (items != null && item.Selected)
            {
                foreach (var i in items)
                {
                    if (i == item)
                        continue;
                    i.Selected = false;
                }
            }
        }

        private ObservableList<ItemViewModel> CreateItems(int count)
        {
            var models = new ObservableList<ItemViewModel>();
            for (int i = 0; i < count; i++)
                models.Add(CreateItem());
            return models;
        }

        private ItemViewModel CreateItem()
        {
            return new ItemViewModel(this.itemSelectCommand)
            {
                Title = "Item #" + (id++),
            };
        }
    }

    public class GridViewExample : MonoBehaviour
    {
        public Button addButton;
        public Button changeButton;
        public Button moveButton;
        public Button resetButton;
        public GridViewBindingAdapter gridView;

        protected void Awake()
        {
            ApplicationContext context = Context.GetApplicationContext();
            BindingServiceBundle bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();
        }

        private void Start()
        {
            var bindingSet = this.CreateBindingSet<GridViewExample, GridViewExampleViewModel>();

            bindingSet.Bind(addButton).For(v => v.onClick).To(vm => vm.AddItem);
            bindingSet.Bind(changeButton).For(v => v.onClick).To(vm => vm.ChangeItem);
            bindingSet.Bind(moveButton).For(v => v.onClick).To(vm => vm.MoveItem);
            bindingSet.Bind(resetButton).For(v => v.onClick).To(vm => vm.ResetItem);
            bindingSet.Bind(gridView).For(v => v.Items).To(vm => vm.Items);
            bindingSet.Build();

            this.SetDataContext(new GridViewExampleViewModel());
        }
    }
}

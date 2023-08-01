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

using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using Loxodon.Framework.Binding;
using Loxodon.Log;
using System;
using System.Collections;
using System.Collections.Specialized;
using UnityEngine;

namespace Loxodon.Framework.Views.UI
{
    [DefaultExecutionOrder(1000)]
    public class GridViewBindingAdapter : GridAdapter<GridViewParams, GridCellViewHolder>, IBindingAdapter
    {
        private IList items;
        public IList Items
        {
            get { return this.items; }
            set
            {
                if (this.items == value)
                    return;

                INotifyCollectionChanged collection = items as INotifyCollectionChanged;
                if (collection != null)
                    collection.CollectionChanged -= OnCollectionChanged;

                this.items = value;

                collection = items as INotifyCollectionChanged;
                if (collection != null)
                    collection.CollectionChanged += OnCollectionChanged;

                this.OnItemsChanged();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            var collection = items as INotifyCollectionChanged;
            if (collection != null)
                collection.CollectionChanged -= OnCollectionChanged;
        }

        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (!IsInitialized)
                return;

            switch (eventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (InsertAtIndexSupported)
                        InsertItems(eventArgs.NewStartingIndex, eventArgs.NewItems.Count, Parameters.contentPanelEndEdgeStationary, Parameters.keepVelocity);
                    else
                        ResetItems(items.Count, false, false);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (RemoveFromIndexSupported)
                        RemoveItems(eventArgs.OldStartingIndex, eventArgs.OldItems.Count, Parameters.contentPanelEndEdgeStationary, Parameters.keepVelocity);
                    else
                        ResetItems(items.Count, Parameters.contentPanelEndEdgeStationary, Parameters.keepVelocity);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    ResetItems(items.Count, Parameters.contentPanelEndEdgeStationary, Parameters.keepVelocity);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ResetItems(items.Count, Parameters.contentPanelEndEdgeStationary, Parameters.keepVelocity);
                    break;
                case NotifyCollectionChangedAction.Move:
                    ResetItems(items.Count, Parameters.contentPanelEndEdgeStationary, Parameters.keepVelocity);
                    break;
            }
        }

        protected virtual void OnItemsChanged()
        {
            if (!IsInitialized)
                return;

            ResetItems(items != null ? items.Count : 0, Parameters.contentPanelEndEdgeStationary, Parameters.keepVelocity);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ResetItems(items != null ? items.Count : 0, Parameters.contentPanelEndEdgeStationary, Parameters.keepVelocity);
        }

        protected override void UpdateCellViewsHolder(GridCellViewHolder viewsHolder)
        {
            var model = items[viewsHolder.ItemIndex];
            viewsHolder.UpdateDataContext(model);
        }
    }

    [Serializable]
    public class GridViewParams : GridParams
    {
        public bool contentPanelEndEdgeStationary = false;
        public bool keepVelocity = false;
    }

    public class GridCellViewHolder : CellViewsHolder
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GridCellViewHolder));
        private static readonly IList EMPTY_LIST = new ArrayList(0);
        private UIView view;
        private IBindingAdapter bindingAdapter;
        public override void CollectViews()
        {
            base.CollectViews();
            this.view = root.GetComponent<UIView>();
            if (this.view == null)
                this.views.GetComponent<UIView>();

            this.bindingAdapter = root.GetComponent<IBindingAdapter>();
            if (this.bindingAdapter == null)
                this.views.GetComponent<IBindingAdapter>();

            if (view == null && bindingAdapter == null && log.IsWarnEnabled)
                log.WarnFormat("The GridView's Item named '{0}' is missing view script.", root.name);
        }

        public virtual void UpdateDataContext(object dataContext)
        {
            if (this.view != null)
                this.view.SetDataContext(dataContext);

            if (this.bindingAdapter != null)
            {
                IList list = dataContext == null ? EMPTY_LIST : dataContext as IList;
                if (list != null)
                    this.bindingAdapter.Items = list;
                else
                {
                    this.bindingAdapter.Items = EMPTY_LIST;
                    if (log.IsWarnEnabled)
                        log.Warn("The DataContext must be a list object.");
                }
            }
        }
    }
}

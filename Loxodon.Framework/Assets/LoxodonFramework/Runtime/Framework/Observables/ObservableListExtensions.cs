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
using System.Collections.Specialized;

namespace Loxodon.Framework.Observables
{
    public interface IConverter<From, To>
    {
        To Create(From from);

        void Update(From from, To to);
    }

    public static class ObservableListExtensions
    {
        public static ObservableList<To> ToList<From, To>(this ObservableList<From> list, IConverter<From, To> converter)
        {
            return new ObservableListWrapper<From, To>(list, converter);
        }

        class ObservableListWrapper<From, To> : ObservableList<To>, IDisposable
        {
            private readonly IConverter<From, To> converter;
            private readonly ObservableList<From> list;
            private bool disposedValue;

            public ObservableListWrapper(ObservableList<From> list, IConverter<From, To> converter)
            {
                this.list = list;
                this.converter = converter;
                foreach (var item in list)
                {
                    this.AddItem(converter.Create(item));
                }
                if (this.list != null)
                    this.list.CollectionChanged += OnCollectionChanged;
            }

            protected override bool ReadOnly()
            {
                return true;
            }

            private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
            {
                switch (eventArgs.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        this.InsertItem(eventArgs.NewStartingIndex, eventArgs.NewItems[0]);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        this.RemoveItem(eventArgs.OldStartingIndex, eventArgs.OldItems[0]);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        this.ReplaceItem(eventArgs.OldStartingIndex, eventArgs.OldItems[0], eventArgs.NewItems[0]);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        this.ResetItem();
                        break;
                    case NotifyCollectionChangedAction.Move:
                        this.MoveItem(eventArgs.OldStartingIndex, eventArgs.NewStartingIndex, eventArgs.NewItems[0]);
                        break;
                }
            }

            private void InsertItem(int index, object item)
            {
                To itemVM = converter.Create((From)item);
                base.InsertItem(index, itemVM);
            }

            private void RemoveItem(int index, object item)
            {
                base.RemoveItem(index);
            }

            private void ReplaceItem(int index, object oldItem, object item)
            {
                To itemVM = this[index];
                converter.Update((From)item, itemVM);
            }

            private void MoveItem(int oldIndex, int index, object item)
            {
                base.MoveItem(oldIndex, index);
            }

            private void ResetItem()
            {
                base.ClearItems();
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (this.list != null)
                        this.list.CollectionChanged -= OnCollectionChanged;
                    disposedValue = true;
                }
            }

            ~ObservableListWrapper()
            {
                Dispose(disposing: false);
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}

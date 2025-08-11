using Loxodon.Framework.Observables;
using UnityEngine;
namespace Loxodon.Framework.Tutorials
{
    public class ModelListAndViewModelListAutomaticSynchronizationExample : MonoBehaviour
    {
        void Start()
        {
            ObservableList<Item> list = new ObservableList<Item>();

            for (int i = 0; i < 10; i++)
            {
                list.Add(new Item("item" + i, i));
            }

            ObservableList<ItemVm> items = list.ToList(new ConverterImpl());

            UnityEngine.Debug.LogFormat("count:{0}", items.Count);


            list.Add(new Item("item" + 11, 11));

            UnityEngine.Debug.LogFormat("count:{0}", items.Count);

            list.RemoveAt(0);

            UnityEngine.Debug.LogFormat("count:{0} items[0].Item.Name:{1}", items.Count, items[0].Item.Name);

            list.Clear();

            UnityEngine.Debug.LogFormat("count:{0}", items.Count);

        }

        public class ConverterImpl : IConverter<Item, ItemVm>
        {
            public ItemVm Create(Item from)
            {
                return new ItemVm(false, from);
            }

            public void Update(Item from, ItemVm to)
            {
                to.Item = from;
            }
        }

        public class Item : ObservableObject
        {
            private string name;
            private int count;

            public Item(string name, int count)
            {
                this.name = name;
                this.count = count;
            }

            public string Name
            {
                get { return name; }
                set { Set(ref name, value); }
            }

            public int Count
            {
                get { return count; }
                set { Set(ref count, value); }
            }
        }

        public class ItemVm : ObservableObject
        {
            private bool selected;
            private Item item;

            public ItemVm(bool selected, Item item)
            {
                this.selected = selected;
                this.item = item;
            }

            public bool Selected
            {
                get { return selected; }
                set { Set(ref selected, value); }
            }

            public Item Item
            {
                get { return item; }
                set { Set(ref item, value); }
            }
        }
    }
}

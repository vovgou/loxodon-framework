using System.Collections.Specialized;
using UnityEngine;
using Loxodon.Framework.Observables;

namespace Loxodon.Framework.Tutorials
{
    public class Item:ObservableObject
	{
		private string title;
		private string iconPath;
		private string content;

		public string Title {
			get{ return this.title; }
			set{ this.Set<string> (ref this.title, value, "Title"); }
		}

		public string IconPath {
			get{ return this.iconPath; }
			set{ this.Set<string> (ref this.iconPath, value, "IconPath"); }
		}

		public string Content {
			get{ return this.content; }
			set{ this.Set<string> (ref this.content, value, "Content"); }
		}

		public override string ToString ()
		{
			return string.Format ("[Item: Title={0}, IconPath={1}, Content={2}]", Title, IconPath, Content);
		}
	}

	public class ObservableListExample : MonoBehaviour
	{
		private ObservableList<Item> list;

		protected void Start ()
		{
			this.list = new ObservableList<Item> ();
			list.CollectionChanged += OnCollectionChanged;

			list.Add (new Item (){ Title = "title1", IconPath = "xxx/xxx/icon1.png", Content = "this is a test." });
			list [0] = new  Item (){ Title = "title2", IconPath = "xxx/xxx/icon2.png", Content = "this is a test." };

			list.Clear ();
		}

		protected void OnDestroy ()
		{
			if (this.list != null) {
				this.list.CollectionChanged -= OnCollectionChanged;
				this.list = null;
			}
		}

		protected void OnCollectionChanged (object sender, NotifyCollectionChangedEventArgs eventArgs)
		{
			switch (eventArgs.Action) {
			case NotifyCollectionChangedAction.Add:
				foreach (Item item in eventArgs.NewItems) {
					Debug.LogFormat ("ADD item:{0}", item);
				}		
				break;
			case NotifyCollectionChangedAction.Remove:
				foreach (Item item in eventArgs.OldItems) {
					Debug.LogFormat ("REMOVE item:{0}", item);
				}	
				break;
			case NotifyCollectionChangedAction.Replace:				
				foreach (Item item in eventArgs.OldItems) {
					Debug.LogFormat ("REPLACE before item:{0}", item);
				}	
				foreach (Item item in eventArgs.NewItems) {
					Debug.LogFormat ("REPLACE after item:{0}", item);
				}
				break;
			case NotifyCollectionChangedAction.Reset:
				Debug.LogFormat ("RESET");
				break;
			case NotifyCollectionChangedAction.Move:
				break;
			}
		}
	}
}
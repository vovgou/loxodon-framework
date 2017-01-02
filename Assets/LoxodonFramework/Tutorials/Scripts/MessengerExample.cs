using UnityEngine;
using System.Collections;
using Loxodon.Framework.Messaging;

namespace Loxodon.Framework.Tutorials
{
	public class TestMessage:MessageBase
	{
		private string content;

		public TestMessage (object sender, string content) : base (sender)
		{
			this.content = content;
		}

		public string Content{ get { return this.content; } }
	}

	public class MessengerExample : MonoBehaviour
	{
		private IMessenger messenger;
		private System.IDisposable subscription;

		public IMessenger Messenger{ get { return this.messenger; } }

		void Start ()
		{
			this.messenger = new Messenger ();

			/* Subscribe to the message,if the "subscription" dispose,it will automatically cancel the subscription.  */
			this.subscription = this.messenger.Subscribe<TestMessage> (OnMessage);

			/*---------------------------------------------*/

			/* Post a message. */
			this.messenger.Publish (new TestMessage (this, "This is a test."));
		}

		protected void OnMessage (TestMessage message)
		{
			Debug.LogFormat ("Received:{0}", message.Content);
		}

		void OnDestroy ()
		{
			if (this.subscription != null) {
				this.subscription.Dispose ();
				this.subscription = null;
			}
		}

	}
}
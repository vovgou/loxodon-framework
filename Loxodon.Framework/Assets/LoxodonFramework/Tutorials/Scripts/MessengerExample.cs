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
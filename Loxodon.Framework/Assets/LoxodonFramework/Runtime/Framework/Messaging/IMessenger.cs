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

namespace Loxodon.Framework.Messaging
{
    /// <summary>
    /// The Messenger is a class allowing objects to exchange messages.
    /// </summary>
    public interface IMessenger
    {
        /// <summary>
        /// Subscribe a message.
        /// </summary>
        /// <param name="type">The type of message that the recipient subscribes for.</param>
        /// <param name="action">The action that will be executed when a message of type T is sent.</param>
        /// <returns>Disposable object that can be used to unsubscribe the message from the messenger.
        /// if the disposable object is disposed,the message is automatically unsubscribed.</returns>
        ISubscription<object> Subscribe(Type type, Action<object> action);

        /// <summary>
        /// Subscribe a message.
        /// </summary>
        /// <typeparam name="T">The type of message that the recipient subscribes for.</typeparam>
        /// <param name="action">The action that will be executed when a message of type T is sent.</param>
        /// <returns>Disposable object that can be used to unsubscribe the message from the messenger.
        /// if the disposable object is disposed,the message is automatically unsubscribed.</returns>
        ISubscription<T> Subscribe<T>(Action<T> action);

        /// <summary>
        /// Subscribe a message.
        /// </summary>
        /// <param name="channel">A name for a messaging channel.If a recipient subscribes
        /// using a channel, and a sender sends a message using the same channel, then this
        /// message will be delivered to the recipient. Other recipients who did not
        /// use a channel when subscribing (or who used a different channel) will not
        /// get the message. </param>
        /// <param name="type">The type of message that the recipient subscribes for.</param>
        /// <param name="action">The action that will be executed when a message of type T is sent.</param>
        /// <returns>Disposable object that can be used to unsubscribe the message from the messenger.
        /// if the disposable object is disposed,the message is automatically unsubscribed.</returns>
        ISubscription<object> Subscribe(string channel, Type type, Action<object> action);

        /// <summary>
        /// Subscribe a message.
        /// </summary>
        /// <typeparam name="T">The type of message that the recipient subscribes for.</typeparam>
        /// <param name="channel">A name for a messaging channel.If a recipient subscribes
        /// using a channel, and a sender sends a message using the same channel, then this
        /// message will be delivered to the recipient. Other recipients who did not
        /// use a channel when subscribing (or who used a different channel) will not
        /// get the message. </param>
        /// <param name="action">The action that will be executed when a message of type T is sent.</param>
        /// <returns>Disposable object that can be used to unsubscribe the message from the messenger.
        /// if the disposable object is disposed,the message is automatically unsubscribed.</returns>
        ISubscription<T> Subscribe<T>(string channel, Action<T> action);

        /// <summary>
        /// Publish a message to subscribed recipients. 
        /// </summary>
        /// <param name="message"></param>
        void Publish(object message);

        /// <summary>
        /// Publish a message to subscribed recipients. 
        /// </summary>
        /// <typeparam name="T">The type of message that will be sent.</typeparam>
        /// <param name="message">The message to send to subscribed recipients.</param>
        void Publish<T>(T message);

        /// <summary>
        /// Publish a message to subscribed recipients. 
        /// </summary>
        /// <param name="channel">A name for a messaging channel.If a recipient subscribes
        /// using a channel, and a sender sends a message using the same channel, then this
        /// message will be delivered to the recipient. Other recipients who did not
        /// use a channel when subscribing (or who used a different channel) will not
        /// get the message. </param>
        /// <param name="message">The message to send to subscribed recipients.</param>
        void Publish(string channel, object message);

        /// <summary>
        /// Publish a message to subscribed recipients. 
        /// </summary>
        /// <typeparam name="T">The type of message that will be sent.</typeparam>
        /// <param name="channel">A name for a messaging channel.If a recipient subscribes
        /// using a channel, and a sender sends a message using the same channel, then this
        /// message will be delivered to the recipient. Other recipients who did not
        /// use a channel when subscribing (or who used a different channel) will not
        /// get the message. </param>
        /// <param name="message">The message to send to subscribed recipients.</param>
        void Publish<T>(string channel, T message);

    }
}

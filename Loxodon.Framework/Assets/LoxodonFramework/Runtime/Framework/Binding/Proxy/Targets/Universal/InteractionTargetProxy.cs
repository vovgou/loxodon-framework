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

using Loxodon.Framework.Interactivity;
using System;
using System.Threading;
using UnityEngine;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class InteractionTargetProxy : TargetProxyBase, IObtainable
    {
        private readonly EventHandler<InteractionEventArgs> handler;
        private readonly IInteractionAction interactionAction;
        private SendOrPostCallback postCallback;
        public InteractionTargetProxy(object target, IInteractionAction interactionAction) : base(target)
        {
            this.interactionAction = interactionAction;
            this.handler = OnRequest;
        }

        public override Type Type { get { return typeof(EventHandler<InteractionEventArgs>); } }

        public override BindingMode DefaultMode { get { return BindingMode.OneWayToSource; } }

        public object GetValue()
        {
            return handler;
        }

        public TValue GetValue<TValue>()
        {
            return (TValue)GetValue();
        }

        private void OnRequest(object sender, InteractionEventArgs args)
        {
            if (UISynchronizationContext.InThread)
            {
                var target = this.Target;
                if (target == null || (target is Behaviour behaviour && !behaviour.isActiveAndEnabled))
                    return;

                this.interactionAction.OnRequest(sender, args);
            }
            else
            {
                if (postCallback == null)
                {
                    postCallback = state =>
                    {
                        PostArgs postArgs = (PostArgs)state;
                        var target = this.Target;
                        if (target == null || (target is Behaviour behaviour && !behaviour.isActiveAndEnabled))
                            return;

                        this.interactionAction.OnRequest(postArgs.sender, postArgs.args);
                    };
                }
                UISynchronizationContext.Post(postCallback, new PostArgs(sender, args));
            }
        }

        class PostArgs
        {
            public PostArgs(object sender, InteractionEventArgs args)
            {
                this.sender = sender;
                this.args = args;
            }

            public object sender;
            public InteractionEventArgs args;
        }
    }
}

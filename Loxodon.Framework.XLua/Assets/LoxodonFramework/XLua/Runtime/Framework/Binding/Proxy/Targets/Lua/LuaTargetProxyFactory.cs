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
using Loxodon.Framework.Observables;
using XLua;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class LuaTargetProxyFactory : ITargetProxyFactory
    {
        public ITargetProxy CreateProxy(object target, BindingDescription description)
        {
            if (target == null || !(target is ILuaExtendable))
                return null;

            LuaTable metatable = (target as ILuaExtendable).GetMetatable();
            if (metatable == null || !metatable.ContainsKey(description.TargetName))
                return null;

            var obj = metatable.Get<object>(description.TargetName);
            if (obj != null)
            {
                LuaFunction function = obj as LuaFunction;
                if (function != null)
                    return new LuaMethodTargetProxy(target, function);

                IObservableProperty observableValue = obj as IObservableProperty;
                if (observableValue != null)
                    return new ObservableTargetProxy(target, observableValue);

                IInteractionAction interactionAction = obj as IInteractionAction;
                if (interactionAction != null)
                    return new InteractionTargetProxy(target, interactionAction);
            }
            return new LuaTableTargetProxy(target, description.TargetName);
        }
    }
}
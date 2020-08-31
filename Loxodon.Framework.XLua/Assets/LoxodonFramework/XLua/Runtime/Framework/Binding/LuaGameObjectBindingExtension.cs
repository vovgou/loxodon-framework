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

using Loxodon.Framework.Binding.Binders;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Contexts;
using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Loxodon.Framework.Binding.Lua
{
    [LuaCallCSharp]
    public static class LuaGameObjectBindingExtension
    {
        private static IBinder binder;
        public static IBinder Binder
        {
            get
            {
                if (binder == null)
                    binder = Context.GetApplicationContext().GetService<IBinder>();

                if (binder == null)
                    throw new Exception("Data binding service is not initialized,please create a LuaBindingServiceBundle service before using it.");

                return binder;
            }
        }

        public static IBindingContext BindingContext(this GameObject gameObject)
        {
            if (gameObject == null)
                return null;

            BindingContextLifecycle bindingContextLifecycle = gameObject.GetComponent<BindingContextLifecycle>();
            if (bindingContextLifecycle == null)
                bindingContextLifecycle = gameObject.AddComponent<BindingContextLifecycle>();

            IBindingContext bindingContext = bindingContextLifecycle.BindingContext;
            if (bindingContext == null)
            {
                bindingContext = new BindingContext(gameObject, Binder);
                bindingContextLifecycle.BindingContext = bindingContext;
            }
            return bindingContext;
        }

        public static LuaBindingSet CreateBindingSet(this GameObject gameObject)
        {
            IBindingContext context = gameObject.BindingContext();
            return new LuaBindingSet(context, gameObject);
        }

        public static void SetDataContext(this GameObject gameObject, object dataContext)
        {
            gameObject.BindingContext().DataContext = dataContext;
        }

        public static void AddBinding(this GameObject gameObject, BindingDescription bindingDescription)
        {
            gameObject.BindingContext().Add(gameObject, bindingDescription);
        }

        public static void AddBindings(this GameObject gameObject, IEnumerable<BindingDescription> bindingDescriptions)
        {
            gameObject.BindingContext().Add(gameObject, bindingDescriptions);
        }

        public static void AddBinding(this GameObject gameObject, object target, BindingDescription bindingDescription, object key = null)
        {
            gameObject.BindingContext().Add(target, bindingDescription, key);
        }

        public static void AddBindings(this GameObject gameObject, object target, IEnumerable<BindingDescription> bindingDescriptions, object key = null)
        {
            gameObject.BindingContext().Add(target, bindingDescriptions, key);
        }

        public static void ClearBindings(this GameObject gameObject, object key)
        {
            gameObject.BindingContext().Clear(key);
        }

        public static void ClearAllBindings(this GameObject gameObject)
        {
            gameObject.BindingContext().Clear();
        }
    }
}

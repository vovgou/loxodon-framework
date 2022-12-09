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

using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Execution;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Prefs;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using XLua;
using IAsyncResult = Loxodon.Framework.Asynchronous.IAsyncResult;

namespace Loxodon.Framework.Editors
{
    public static class LuaFrameworkGenConfig
    {
        [LuaCallCSharp]
        public static List<Type> lua_call_cs_list = new List<Type>()
        {
            typeof(Executors),
            typeof(Context),
            typeof(ApplicationContext),
            typeof(PlayerContext),
            typeof(Preferences),
            typeof(ILocalization),
            typeof(Localization),
            typeof(Messenger),
            typeof(SimpleCommand),
            typeof(AsyncResult),
            typeof(AsyncResult<IViewModel>),
            typeof(ObservableDictionary<object,object>),
            typeof(ObservableList<object>),
            typeof(NotifyCollectionChangedEventArgs),
            typeof(NotifyCollectionChangedEventHandler),
            typeof(ITransition),
            typeof(WindowContainer),
            typeof(ProgressResult<float, IWindow>),
            typeof(ProgressResult<float, IView>),
            typeof(ProgressResult<float, IWindow>),
            typeof(ProgressResult<float, IView>),
            typeof(IView),
            typeof(IWindow),
            typeof(Window),
            typeof(IWindowManager),
            typeof(WindowManager),
            typeof(IUIViewLocator),
            typeof(DefaultUIViewLocator),
            typeof(Type),
            typeof(CoroutineAwaiterExtensions)
        };

        [CSharpCallLua]
        public static List<Type> cs_call_lua_list = new List<Type>()
        {
            typeof(IEnumerator),
            typeof(Action),
            typeof(Action<LuaTable>),
            typeof(Action<MonoBehaviour>),
            typeof(Func<MonoBehaviour, ILuaTask>),
            typeof(Action<IWindow,WindowState>),
            typeof(Action<LuaWindow>),
            typeof(Action<LuaWindow,IBundle>),
            typeof(NotifyCollectionChangedEventHandler),
            typeof(Action<float>),
            typeof(Action<int>),
            typeof(Action<string>),
            typeof(Action<object>),
            typeof(Action<Exception>),
            typeof(Action<IAsyncResult>),
            typeof(EventHandler<WindowStateEventArgs>),
            typeof(EventHandler),
            typeof(Func<object>),
            typeof(IViewModel),
            typeof(IAwaiter),
            typeof(IAwaiter<object>),
            typeof(IAwaiter<int>),
            typeof(ILuaTask<int>)
        };

        [BlackList]
        public static List<List<string>> BlackList = new List<List<string>>()  {
               new List<string>(){"System.Type", "IsSZArray"},
               new List<string>(){"System.Type", "MakeGenericSignatureType","System.Type","System.Type[]"},
               new List<string>(){"System.Type", "IsCollectible"}
        };
    }
}
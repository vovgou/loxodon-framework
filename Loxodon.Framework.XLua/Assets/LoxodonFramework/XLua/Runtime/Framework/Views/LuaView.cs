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
using Loxodon.Framework.Views.Variables;
using System;
using UnityEngine;
using XLua;

namespace Loxodon.Framework.Views
{
    [LuaCallCSharp]
    public class LuaView : View, ILuaExtendable
    {
        public ScriptReference script;
        public VariableArray variables;

        protected LuaTable scriptEnv;
        protected LuaTable metatable;
        protected Action<MonoBehaviour> onAwake;
        protected Action<MonoBehaviour> onEnable;
        protected Action<MonoBehaviour> onDisable;
        protected Func<MonoBehaviour, ILuaTask> onStart;
        protected Action<MonoBehaviour> onUpdate;
        protected Action<MonoBehaviour> onDestroy;

        public virtual LuaTable GetMetatable()
        {
            return this.metatable;
        }

        protected virtual void Initialize()
        {
            var luaEnv = LuaEnvironment.LuaEnv;
            scriptEnv = luaEnv.NewTable();

            LuaTable meta = luaEnv.NewTable();
            meta.Set("__index", luaEnv.Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            scriptEnv.Set("target", this);

            string scriptText = (script.Type == ScriptReferenceType.TextAsset) ? script.Text.text : string.Format("require(\"framework.System\");local cls = require(\"{0}\");return extends(target,cls);", script.Filename);
            object[] result = luaEnv.DoString(scriptText, string.Format("{0}({1})", "LuaView", this.name), scriptEnv);

            if (result.Length != 1 || !(result[0] is LuaTable))
                throw new Exception("");

            metatable = (LuaTable)result[0];
            if (variables != null && variables.Variables != null)
            {
                foreach (var variable in variables.Variables)
                {
                    var name = variable.Name.Trim();
                    if (string.IsNullOrEmpty(name))
                        continue;

                    metatable.Set(name, variable.GetValue());
                }
            }

            onAwake = metatable.Get<Action<MonoBehaviour>>("awake");
            onEnable = metatable.Get<Action<MonoBehaviour>>("enable");
            onDisable = metatable.Get<Action<MonoBehaviour>>("disable");
            onStart = metatable.Get<Func<MonoBehaviour, ILuaTask>>("start");
            onUpdate = metatable.Get<Action<MonoBehaviour>>("update");
            onDestroy = metatable.Get<Action<MonoBehaviour>>("destroy");
        }

        protected virtual void Awake()
        {
            this.Initialize();

            if (onAwake != null)
                onAwake(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (onEnable != null)
                onEnable(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (onDisable != null)
                onDisable(this);
        }

        protected virtual async void Start()
        {
            if (onStart != null)
            {
                ILuaTask task = onStart(this);
                if (task != null)
                    await task;
            }
        }

        protected virtual void Update()
        {
            if (onUpdate != null)
                onUpdate(this);
        }

        protected virtual void OnDestroy()
        {
            if (onDestroy != null)
                onDestroy(this);

            onDestroy = null;
            onUpdate = null;
            onStart = null;
            onEnable = null;
            onDisable = null;
            onAwake = null;

            if (metatable != null)
            {
                metatable.Dispose();
                metatable = null;
            }

            if (scriptEnv != null)
            {
                scriptEnv.Dispose();
                scriptEnv = null;
            }
        }
    }
}
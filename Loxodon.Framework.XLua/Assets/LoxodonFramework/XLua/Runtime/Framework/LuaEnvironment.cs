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
using System.Collections;
using UnityEngine;
using Loxodon.Framework.Execution;
using XLua;
using Loxodon.Log;
using IAsyncResult = Loxodon.Framework.Asynchronous.IAsyncResult;

namespace Loxodon.Framework
{
    public static class LuaEnvironment
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LuaEnvironment));

        private static float interval = 2;
        private static WaitForSecondsRealtime wait;
        private static LuaEnv luaEnv;
        private static IAsyncResult result;

        public static float Interval
        {
            get { return interval; }
            set
            {
                if (interval <= 0)
                    return;

                interval = value;
                wait = new WaitForSecondsRealtime(interval);
            }
        }

        public static LuaEnv LuaEnv
        {
            get
            {
                if (luaEnv == null)
                {
                    luaEnv = new LuaEnv();
                    if (result != null)
                        result.Cancel();

                    wait = new WaitForSecondsRealtime(interval);
                    result = Executors.RunOnCoroutine(DoTick());
                }

                return luaEnv;
            }
        }

        public static void Dispose()
        {
            if (result != null)
            {
                result.Cancel();
                result = null;
            }

            if (luaEnv != null)
            {
                luaEnv.Dispose();
                luaEnv = null;
            }

            wait = null;
        }

        private static IEnumerator DoTick()
        {
            while (true)
            {
                yield return wait;
                try
                {
                    luaEnv.Tick();
                }
                catch (Exception e)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("LuaEnv.Tick Error:{0}", e);
                }
            }
        }
    }
}
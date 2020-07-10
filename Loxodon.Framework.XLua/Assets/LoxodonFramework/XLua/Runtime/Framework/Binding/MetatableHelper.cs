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

using XLua;

namespace Loxodon.Framework.Binding
{
    [CSharpCallLua]
    delegate LuaTable GetMetatableHandler(object target);

    [CSharpCallLua]
    delegate LuaTable SetMetatableHandler(object target, LuaTable metatable);

    public static class MetatableHelper
    {
        static GetMetatableHandler getter;
        static SetMetatableHandler setter;
        public static LuaTable GetMetatable(object target)
        {
            if (getter != null)
                return getter(target);

            getter = LuaEnvironment.LuaEnv.Global.GetInPath<GetMetatableHandler>("debug.getmetatable");
            if (getter == null)
                return null;
            return getter(target);
        }

        public static void SetMetatable(object target, LuaTable metatable)
        {
            if (setter != null)
            {
                setter(target, metatable);
                return;
            }

            setter = LuaEnvironment.LuaEnv.Global.GetInPath<SetMetatableHandler>("debug.setmetatable");
            if (setter != null)
                setter(target, metatable);
        }
    }
}
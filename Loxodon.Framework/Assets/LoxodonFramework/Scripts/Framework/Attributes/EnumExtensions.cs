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
using System.Reflection;
#if NETFX_CORE
using System.Collections.Generic;
#endif

namespace Loxodon.Framework.Attributes
{
    public static class EnumExtensions
    {
        public static string GetRemark(this Enum e)
        {
            Type type = e.GetType();
            FieldInfo fieldInfo = type.GetField(e.ToString());
            if (fieldInfo == null)
                return string.Empty;

#if NETFX_CORE
            IEnumerable<Attribute> attrs = fieldInfo.GetCustomAttributes(typeof(RemarkAttribute), false);
#else
            object[] attrs = fieldInfo.GetCustomAttributes(typeof(RemarkAttribute), false);
#endif
            foreach (RemarkAttribute attr in attrs)
                return attr.Remark;

            return string.Empty;
        }
    }    
}

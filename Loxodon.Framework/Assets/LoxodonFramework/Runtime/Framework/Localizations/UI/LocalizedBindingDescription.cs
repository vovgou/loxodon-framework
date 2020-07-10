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

using Loxodon.Framework.Binding;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Loxodon.Framework.Localizations
{
    [Serializable]
    public class LocalizedBindingDescriptionSet
    {
        public List<LocalizedBindingDescription> descriptions = new List<LocalizedBindingDescription>();
    }

    [Serializable]
    public class LocalizedBindingDescription
    {
        [SerializeField]
        public string TypeName;

        [SerializeField]
        public string PropertyName;

        [SerializeField]
        public string Key;

        [SerializeField]
        public BindingMode Mode = BindingMode.OneWay;

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(TypeName).Append(" ");
            buf.Append("{binding ").Append(PropertyName);
            buf.Append(" Key:").Append(Key);
            buf.Append(" Mode:").Append(Mode);
            buf.Append(" }");
            return buf.ToString();
        }
    }
}

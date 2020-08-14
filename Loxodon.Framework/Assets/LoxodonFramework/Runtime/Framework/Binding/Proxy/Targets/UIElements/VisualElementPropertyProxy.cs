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

#if UNITY_2019_1_OR_NEWER
using Loxodon.Framework.Binding.Reflection;
using UnityEngine.UIElements;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class VisualElementPropertyProxy<TValue> : PropertyTargetProxy
    {
        private readonly INotifyValueChanged<TValue> notifyValueChanged;
        public VisualElementPropertyProxy(object target, IProxyPropertyInfo propertyInfo) : base(target, propertyInfo)
        {
            //不能所有的属性都自动绑定，可能绑定的其他值，与事件不匹配的值
            if (target is INotifyValueChanged<TValue>)
                this.notifyValueChanged = (INotifyValueChanged<TValue>)target;
            else
                notifyValueChanged = null;
        }

        public override BindingMode DefaultMode { get { return BindingMode.TwoWay; } }

        protected override void DoSubscribeForValueChange(object target)
        {
            if (this.notifyValueChanged == null || target == null)
                return;

            notifyValueChanged.RegisterValueChangedCallback(OnValueChanged);
        }

        protected override void DoUnsubscribeForValueChange(object target)
        {
            if (notifyValueChanged != null)
                notifyValueChanged.UnregisterValueChangedCallback(OnValueChanged);
        }

        private void OnValueChanged(ChangeEvent<TValue> e)
        {
            this.RaiseValueChanged();
        }
    }
}
#endif
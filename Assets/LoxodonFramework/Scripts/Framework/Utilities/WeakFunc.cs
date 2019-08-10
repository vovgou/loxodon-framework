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

namespace Loxodon.Framework.Utilities
{
    public class WeakFunc<TResult> : WeakBase<Func<TResult>>
    {
        public WeakFunc(Func<TResult> func) : this(func == null ? null : func.Target, func)
        {
        }

        public WeakFunc(object target, Func<TResult> func) : base(target, func)
        {
        }

        public virtual TResult Execute()
        {
            if (!IsAlive)
                return default(TResult);

            if (this.del != null)
                return this.del();

            var target = this.targetReference.Target;
            return (TResult)this.targetMethod.Invoke(target, null);
        }

        public override object Execute(params object[] parameters)
        {
            return Execute();
        }
    }

    public class WeakFunc<T, TResult> : WeakBase<Func<T, TResult>>
    {
        public WeakFunc(Func<T, TResult> func) : this(func == null ? null : func.Target, func)
        {
        }

        public WeakFunc(object target, Func<T, TResult> func) : base(target, func)
        {
        }

        public virtual TResult Execute(T parameter)
        {
            if (!IsAlive)
                return default(TResult);

            if (this.del != null)
                return this.del(parameter);

            var target = this.targetReference.Target;
            return (TResult)this.targetMethod.Invoke(target, new object[] { parameter });
        }

        public override object Execute(params object[] parameters)
        {
            return Execute((T)parameters[0]);
        }
    }

    public class WeakFunc<T1, T2, TResult> : WeakBase<Func<T1, T2, TResult>>
    {
        public WeakFunc(Func<T1, T2, TResult> func) : this(func == null ? null : func.Target, func)
        {
        }

        public WeakFunc(object target, Func<T1, T2, TResult> func) : base(target, func)
        {
        }

        public virtual TResult Execute(T1 t1, T2 t2)
        {
            if (!IsAlive)
                return default(TResult);

            if (this.del != null)
                return this.del(t1, t2);

            var target = this.targetReference.Target;
            return (TResult)this.targetMethod.Invoke(target, new object[] { t1, t2 });
        }

        public override object Execute(params object[] parameters)
        {
            return Execute((T1)parameters[0], (T2)parameters[1]);
        }
    }

    public class WeakFunc<T1, T2, T3, TResult> : WeakBase<Func<T1, T2, T3, TResult>>
    {
        public WeakFunc(Func<T1, T2, T3, TResult> func) : this(func == null ? null : func.Target, func)
        {
        }

        public WeakFunc(object target, Func<T1, T2, T3, TResult> func) : base(target, func)
        {
        }

        public virtual TResult Execute(T1 t1, T2 t2, T3 t3)
        {
            if (!IsAlive)
                return default(TResult);

            if (this.del != null)
                return this.del(t1, t2, t3);

            var target = this.targetReference.Target;
            return (TResult)this.targetMethod.Invoke(target, new object[] { t1, t2, t3 });
        }

        public override object Execute(params object[] parameters)
        {
            return Execute((T1)parameters[0], (T2)parameters[1], (T3)parameters[2]);
        }
    }

    public class WeakFunc<T1, T2, T3, T4, TResult> : WeakBase<Func<T1, T2, T3, T4, TResult>>
    {
        public WeakFunc(Func<T1, T2, T3, T4, TResult> func) : this(func == null ? null : func.Target, func)
        {
        }

        public WeakFunc(object target, Func<T1, T2, T3, T4, TResult> func) : base(target, func)
        {
        }

        public virtual TResult Execute(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (!IsAlive)
                return default(TResult);

            if (this.del != null)
                return this.del(t1, t2, t3, t4);

            var target = this.targetReference.Target;
            return (TResult)this.targetMethod.Invoke(target, new object[] { t1, t2, t3, t4 });
        }

        public override object Execute(params object[] parameters)
        {
            return Execute((T1)parameters[0], (T2)parameters[1], (T3)parameters[2], (T4)parameters[3]);
        }
    }
}
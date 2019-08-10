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
    public class WeakAction : WeakBase<Action>
    {
        public WeakAction(Action action) : this(action == null ? null : action.Target, action)
        {
        }

        public WeakAction(object target, Action action) : base(target, action)
        {
        }

        public virtual void Execute()
        {
            if (!IsAlive)
                return;

            if (this.del != null)
            {
                this.del();
                return;
            }

            var target = this.targetReference.Target;
            this.targetMethod.Invoke(target, null);
        }

        public override object Execute(params object[] parameters)
        {
            Execute();
            return null;
        }
    }

    public class WeakAction<T> : WeakBase<Action<T>>
    {
        public WeakAction(Action<T> action) : this(action == null ? null : action.Target, action)
        {
        }

        public WeakAction(object target, Action<T> action) : base(target, action)
        {
        }

        public virtual void Execute(T parameter)
        {
            if (!IsAlive)
                return;

            if (this.del != null)
            {
                this.del(parameter);
                return;
            }

            var target = this.targetReference.Target;
            this.targetMethod.Invoke(target, new object[] { parameter });
        }

        public override object Execute(params object[] parameters)
        {
            Execute((T)parameters[0]);
            return null;
        }
    }

    public class WeakAction<T1, T2> : WeakBase<Action<T1, T2>>
    {
        public WeakAction(Action<T1, T2> action) : this(action == null ? null : action.Target, action)
        {
        }

        public WeakAction(object target, Action<T1, T2> action) : base(target, action)
        {
        }

        public virtual void Execute(T1 t1, T2 t2)
        {
            if (!IsAlive)
                return;

            if (this.del != null)
            {
                this.del(t1, t2);
                return;
            }

            var target = this.targetReference.Target;
            this.targetMethod.Invoke(target, new object[] { t1, t2 });
        }

        public override object Execute(params object[] parameters)
        {
            Execute((T1)parameters[0], (T2)parameters[1]);
            return null;
        }
    }

    public class WeakAction<T1, T2, T3> : WeakBase<Action<T1, T2, T3>>
    {
        public WeakAction(Action<T1, T2, T3> action) : this(action == null ? null : action.Target, action)
        {
        }

        public WeakAction(object target, Action<T1, T2, T3> action) : base(target, action)
        {
        }

        public virtual void Execute(T1 t1, T2 t2, T3 t3)
        {
            if (!IsAlive)
                return;

            if (this.del != null)
            {
                this.del(t1, t2, t3);
                return;
            }

            var target = this.targetReference.Target;
            this.targetMethod.Invoke(target, new object[] { t1, t2, t3 });
        }
        public override object Execute(params object[] parameters)
        {
            Execute((T1)parameters[0], (T2)parameters[1], (T3)parameters[2]);
            return null;
        }
    }

    public class WeakAction<T1, T2, T3, T4> : WeakBase<Action<T1, T2, T3, T4>>
    {
        public WeakAction(Action<T1, T2, T3, T4> action) : this(action == null ? null : action.Target, action)
        {
        }

        public WeakAction(object target, Action<T1, T2, T3, T4> action) : base(target, action)
        {
        }

        public virtual void Execute(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (!IsAlive)
                return;

            if (this.del != null)
            {
                this.del(t1, t2, t3, t4);
                return;
            }

            var target = this.targetReference.Target;
            this.targetMethod.Invoke(target, new object[] { t1, t2, t3, t4 });
        }

        public override object Execute(params object[] parameters)
        {
            Execute((T1)parameters[0], (T2)parameters[1], (T3)parameters[2], (T4)parameters[3]);
            return null;
        }
    }
}
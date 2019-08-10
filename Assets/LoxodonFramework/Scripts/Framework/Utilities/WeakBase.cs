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
using System.Runtime.CompilerServices;

namespace Loxodon.Framework.Utilities
{
    public interface IExecute
    {
        object Execute(params object[] parameters);
    }

    public abstract class WeakBase<TDelegate> : IExecute where TDelegate : class
    {
        private readonly bool isStatic;
        private int hashCode = 0;

        protected TDelegate del;
        protected WeakReference targetReference;
        protected MethodInfo targetMethod;

        public WeakBase(TDelegate del) : this(null, del)
        {
        }

        public WeakBase(object target, TDelegate del)
        {
            this.hashCode = del.GetHashCode();
            var dd = del as Delegate;

#if NETFX_CORE
            this.isStatic = dd.GetMethodInfo().IsStatic;
#else
            this.isStatic = dd.Method.IsStatic;
#endif
            if (this.isStatic || (target != null && !target.Equals(dd.Target)) || this.IsClosure(dd))
            {
                this.del = del;
                if (target != null)
                    this.targetReference = new WeakReference(target);
            }
            else
            {
#if NETFX_CORE
                this.targetMethod = dd.GetMethodInfo();
#else
                this.targetMethod = dd.Method;
#endif
                this.targetReference = new WeakReference(dd.Target);
            }
        }

        protected bool IsStatic { get { return this.isStatic; } }

        public bool IsAlive
        {
            get
            {
                if (this.del != null)
                {
                    if (this.targetReference != null && !this.targetReference.IsAlive)
                    {
                        this.targetReference = null;
                        this.del = null;
                        return false;
                    }
                    return true;
                }

                if (this.targetReference != null)
                    return this.targetReference.IsAlive;

                return false;
            }
        }

        protected bool IsClosure(Delegate del)
        {
#if NETFX_CORE
            if (del == null || del.GetMethodInfo().IsStatic || del.Target == null)
                return false;

            var type = del.Target.GetType();
            var isCompilerGenerated = type.GetTypeInfo().GetCustomAttribute(typeof(CompilerGeneratedAttribute), false) !=null;
            var isNested = type.IsNested;
            return isNested && isCompilerGenerated;
#else
            if (del == null || del.Method.IsStatic || del.Target == null)
                return false;

            var type = del.Target.GetType();
            var isInvisible = !type.IsVisible;
            var isCompilerGenerated = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length != 0;
            var isNested = type.IsNested && type.MemberType == MemberTypes.NestedType;
            return isNested && isCompilerGenerated && isInvisible;
#endif
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (obj == null || !(obj is WeakBase<TDelegate>))
                return false;

            WeakBase<TDelegate> other = (WeakBase<TDelegate>)obj;
            if (this.isStatic != other.isStatic)
                return false;

            if (this.del != null)
            {
                if ((this.targetReference == null && other.targetReference == null) || (this.targetReference != null && other.targetReference != null && this.targetReference.Target == other.targetReference.Target))
                    return this.del.Equals(other.del);

                return false;
            }

            return this.targetMethod.Equals(other.targetMethod) && this.targetReference.Target == other.targetReference.Target;
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public abstract object Execute(params object[] parameters);
    }
}

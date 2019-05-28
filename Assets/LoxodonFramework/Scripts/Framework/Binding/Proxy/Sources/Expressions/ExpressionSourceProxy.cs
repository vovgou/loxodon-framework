using System;
using System.Collections.Generic;

namespace Loxodon.Framework.Binding.Proxy.Sources.Expressions
{
    public class ExpressionSourceProxy : NotifiableSourceProxyBase, IExpressionSourceProxy
    {
        private bool disposed = false;
        private readonly Type type;
        private readonly Func<object[], object> func;
        private readonly List<ISourceProxy> inners = new List<ISourceProxy>();

        public ExpressionSourceProxy(object source, Func<object[], object> func, Type type, List<ISourceProxy> inners) : base(source)
        {
            this.type = type;
            this.func = func;
            this.inners = inners;

            if (this.inners == null || this.inners.Count <= 0)
                return;

            foreach (ISourceProxy proxy in this.inners)
            {
                if (proxy is INotifiable)
                    ((INotifiable)proxy).ValueChanged += OnValueChanged;
            }
        }

        public override Type Type { get { return this.type; } }

        public virtual object GetValue()
        {
            if (this.source != null)
            {
                return func(new object[] { this.source });
            }
            else
            {
                return func(null);
            }
        }

        public virtual TValue GetValue<TValue>()
        {
            return (TValue)this.GetValue();
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            RaiseValueChanged();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (this.inners != null && this.inners.Count > 0)
                    {
                        foreach (ISourceProxy proxy in this.inners)
                        {
                            if (proxy is INotifiable)
                                ((INotifiable)proxy).ValueChanged -= OnValueChanged;
                            proxy.Dispose();
                        }
                        this.inners.Clear();
                    }
                }
                disposed = true;
                base.Dispose(disposing);
            }
        }
    }

    public class ExpressionSourceProxy<T, TResult> : NotifiableSourceProxyBase, IExpressionSourceProxy
    {
        private bool disposed = false;
        private readonly Func<T, TResult> func;
        private readonly List<ISourceProxy> inners = new List<ISourceProxy>();

        public ExpressionSourceProxy(T source, Func<T, TResult> func, List<ISourceProxy> inners) : base(source)
        {
            this.func = func;
            this.inners = inners;

            if (this.inners == null || this.inners.Count <= 0)
                return;

            foreach (ISourceProxy proxy in this.inners)
            {
                if (proxy is INotifiable)
                    ((INotifiable)proxy).ValueChanged += OnValueChanged;
            }
        }

        public override Type Type { get { return typeof(TResult); } }

        public virtual object GetValue()
        {
            return func((T)this.source);
        }

        public virtual TValue GetValue<TValue>()
        {
            return (TValue)GetValue();
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            RaiseValueChanged();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (this.inners != null && this.inners.Count > 0)
                    {
                        foreach (ISourceProxy proxy in this.inners)
                        {
                            if (proxy is INotifiable)
                                ((INotifiable)proxy).ValueChanged -= OnValueChanged;

                            proxy.Dispose();
                        }
                        this.inners.Clear();
                    }
                }
                disposed = true;
                base.Dispose(disposing);
            }
        }
    }

    public class ExpressionSourceProxy<TResult> : NotifiableSourceProxyBase, IExpressionSourceProxy
    {
        private readonly Func<TResult> func;

        public ExpressionSourceProxy(Func<TResult> func) : base(null)
        {
            this.func = func;
        }

        public override Type Type { get { return typeof(TResult); } }

        public virtual object GetValue()
        {
            return func();
        }

        public virtual TValue GetValue<TValue>()
        {
            return (TValue)GetValue();
        }
    }
}

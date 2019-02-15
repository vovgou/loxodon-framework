using System;
using System.Collections.Generic;

namespace Loxodon.Framework.Binding.Proxy.Sources.Expressions
{
    public class ExpressionSourceProxy : AbstractProxy, IExpressionSourceProxy
    {
        private bool disposed = false;
        private object source;
        private Type type;
        private Func<object[], object> func;
        private List<ISourceProxy> inners = new List<ISourceProxy>();

        private readonly object eventLock = new object();
        private EventHandler<EventArgs> valueChanged;
        public event EventHandler<EventArgs> ValueChanged
        {
            add { lock (eventLock) { this.valueChanged += value; } }
            remove { lock (eventLock) { this.valueChanged -= value; } }
        }

        public ExpressionSourceProxy(object source, Func<object[], object> func, Type type, List<ISourceProxy> inners)
        {
            this.source = source;
            this.type = type;
            this.func = func;
            this.inners = inners;

            if (this.inners == null || this.inners.Count <= 0)
                return;

            foreach (ISourceProxy proxy in this.inners)
            {
                if (proxy is INotifiable<EventArgs>)
                    ((INotifiable<EventArgs>)proxy).ValueChanged += OnValueChanged;
            }
        }

        public Type Type { get { return this.type; } }

        public object Source { get { return source; } }

        public object GetValue()
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

        protected void RaiseValueChanged()
        {
            if (this.valueChanged != null)
                this.valueChanged(this, EventArgs.Empty);
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
                            if (proxy is INotifiable<EventArgs>)
                                ((INotifiable<EventArgs>)proxy).ValueChanged -= OnValueChanged;
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

    public class ExpressionSourceProxy<T, TResult> : AbstractProxy, IExpressionSourceProxy
    {
        private bool disposed = false;
        private T source;
        private Func<T, TResult> func;
        private List<ISourceProxy> inners = new List<ISourceProxy>();

        private readonly object eventLock = new object();
        private EventHandler<EventArgs> valueChanged;
        public event EventHandler<EventArgs> ValueChanged
        {
            add { lock (eventLock) { this.valueChanged += value; } }
            remove { lock (eventLock) { this.valueChanged -= value; } }
        }

        public ExpressionSourceProxy(T source, Func<T, TResult> func, List<ISourceProxy> inners)
        {
            this.source = source;
            this.func = func;
            this.inners = inners;

            if (this.inners == null || this.inners.Count <= 0)
                return;

            foreach (ISourceProxy proxy in this.inners)
            {
                if (proxy is INotifiable<EventArgs>)
                    ((INotifiable<EventArgs>)proxy).ValueChanged += OnValueChanged;
            }
        }

        public Type Type { get { return typeof(TResult); } }

        public object Source { get { return source; } }

        public object GetValue()
        {
            return func(this.source);
        }

        protected void RaiseValueChanged()
        {
            if (this.valueChanged != null)
                this.valueChanged(this, EventArgs.Empty);
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
                            if (proxy is INotifiable<EventArgs>)
                                ((INotifiable<EventArgs>)proxy).ValueChanged -= OnValueChanged;

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

    public class ExpressionSourceProxy<TResult> : AbstractProxy, IExpressionSourceProxy
    {
        private Func<TResult> func;

        private readonly object eventLock = new object();
        private EventHandler<EventArgs> valueChanged;
        public event EventHandler<EventArgs> ValueChanged
        {
            add { lock (eventLock) { this.valueChanged += value; } }
            remove { lock (eventLock) { this.valueChanged -= value; } }
        }

        public ExpressionSourceProxy(Func<TResult> func)
        {
            this.func = func;
        }

        public Type Type { get { return typeof(TResult); } }

        public object Source { get { return null; } }

        public object GetValue()
        {
            return func();
        }
    }
}

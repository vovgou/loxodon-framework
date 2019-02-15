using UnityEngine;
namespace Loxodon.Framework.Binding.Contexts
{
    public class BindingContextLifecycle : MonoBehaviour
    {
        private IBindingContext bindingContext;
        public IBindingContext BindingContext
        {
            get { return this.bindingContext; }
            set
            {
                if (this.bindingContext == value)
                    return;

                if (this.bindingContext != null)
                    this.bindingContext.Dispose();

                this.bindingContext = value;
            }
        }

        protected virtual void OnDestroy()
        {
            if (this.bindingContext != null)
            {
                this.bindingContext.Dispose();
                this.bindingContext = null;
            }
        }
    }
}

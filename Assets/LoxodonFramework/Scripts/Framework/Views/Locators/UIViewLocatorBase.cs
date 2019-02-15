using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Views
{
    public abstract class UIViewLocatorBase : IUIViewLocator
    {
        public virtual IView LoadView(string name)
        {
            return LoadView<IView>(name);
        }

        public abstract T LoadView<T>(string name) where T : IView;

        public virtual Window LoadWindow(string name)
        {
            return LoadWindow<Window>(name);
        }

        public abstract T LoadWindow<T>(string name) where T : Window;

        public virtual Window LoadWindow(IWindowManager windowManager, string name)
        {
            return LoadWindow<Window>(windowManager, name);
        }

        public abstract T LoadWindow<T>(IWindowManager windowManager, string name) where T : Window;


        public virtual IProgressTask<float, IView> LoadViewAsync(string name)
        {
            return LoadViewAsync<IView>(name);
        }

        public abstract IProgressTask<float, T> LoadViewAsync<T>(string name) where T : IView;

        public virtual IProgressTask<float, Window> LoadWindowAsync(string name)
        {
            return LoadWindowAsync<Window>(name);
        }

        public abstract IProgressTask<float, T> LoadWindowAsync<T>(string name) where T : Window;

        public virtual IProgressTask<float, Window> LoadWindowAsync(IWindowManager windowManager, string name)
        {
            return LoadWindowAsync<Window>(windowManager, name);
        }

        public abstract IProgressTask<float, T> LoadWindowAsync<T>(IWindowManager windowManager, string name) where T : Window;
    }
}

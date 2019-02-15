using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Views
{
    public interface IUIViewLocator
    {
        IView LoadView(string name);

        T LoadView<T>(string name) where T : IView;

        Window LoadWindow(string name);

        T LoadWindow<T>(string name) where T : Window;

        Window LoadWindow(IWindowManager windowManager, string name);

        T LoadWindow<T>(IWindowManager windowManager, string name) where T : Window;

        IProgressTask<float, IView> LoadViewAsync(string name);

        IProgressTask<float, T> LoadViewAsync<T>(string name) where T : IView;

        IProgressTask<float, Window> LoadWindowAsync(string name);

        IProgressTask<float, T> LoadWindowAsync<T>(string name) where T : Window;

        IProgressTask<float, Window> LoadWindowAsync(IWindowManager windowManager, string name);

        IProgressTask<float, T> LoadWindowAsync<T>(IWindowManager windowManager, string name) where T : Window;
    }
}

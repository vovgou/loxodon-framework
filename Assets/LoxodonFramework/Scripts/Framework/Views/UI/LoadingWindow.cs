namespace Loxodon.Framework.Views
{
    public class LoadingWindow : Window
    {
        protected override void OnCreate(IBundle bundle)
        {
            this.WindowType = WindowType.PROGRESS;
        }
    }
}

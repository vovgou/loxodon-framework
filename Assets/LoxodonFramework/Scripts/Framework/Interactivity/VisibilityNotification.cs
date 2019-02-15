namespace Loxodon.Framework.Interactivity
{
    public class VisibilityNotification
    {
        private bool visible;

        public VisibilityNotification(bool visible)
        {
            this.visible = visible;
        }

        public bool Visible
        {
            get { return this.visible; }
        }
    }
}

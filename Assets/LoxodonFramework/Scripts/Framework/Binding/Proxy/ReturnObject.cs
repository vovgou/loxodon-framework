namespace Loxodon.Framework.Binding.Proxy
{
    public class ReturnObject
    {
        public static readonly ReturnObject NOTHING = new ReturnObject("Nothing");
        public static readonly ReturnObject UNSET = new ReturnObject("Unset");

        private readonly string content;

        private ReturnObject(string content)
        {
            this.content = content;
        }

        public override string ToString()
        {
            return this.content;
        }
    }
}

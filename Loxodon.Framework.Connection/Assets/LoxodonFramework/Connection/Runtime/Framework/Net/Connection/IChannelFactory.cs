namespace Loxodon.Framework.Net.Connection
{
    public interface IChannelFactory
    {
        IChannel<IMessage> CreateChannel();
    }
}

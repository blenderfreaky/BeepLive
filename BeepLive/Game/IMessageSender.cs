namespace BeepLive.Game
{
    public interface IMessageSender
    {
        void SendMessage<T>(T message);
    }
}
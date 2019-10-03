namespace BeepLive.Net
{
    using System;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class NetTcpClient : IDisposable
    {
        public TcpClient Client { get; }
        public StreamProtobuf StreamProtobuf { get; }

        public NetworkStream Stream { get; }

        public event ClientPacketReveivedEventHandler PacketReceivedEvent;

        public NetTcpClient(TcpClient client, StreamProtobuf streamProtobuf)
        {
            Client = client;
            Stream = client.GetStream();

            StreamProtobuf = streamProtobuf;
        }

        public async Task AcceptPackets()
        {
            await Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (StreamProtobuf.ReadNext(Stream, out object value)) PacketReceivedEvent(this, value);
                }
            }, TaskCreationOptions.LongRunning).ConfigureAwait(false);
        }

        public void SendToStream<T>(T obj)
        {
            StreamProtobuf.WriteNext(Stream, obj);
        }

        public override string ToString() => $"{nameof(Client)}: {Client}";

        #region IDisposable Support

        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Stream.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~NetTcpClient()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
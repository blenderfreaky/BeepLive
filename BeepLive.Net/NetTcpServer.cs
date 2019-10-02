namespace BeepLive.Net
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class NetTcpServer : IDisposable
    {
        public TcpListener TcpListener { get; }
        public StreamProtobuf StreamProtobuf { get; }

        public IProducerConsumerCollection<NetTcpClient> Clients { get; }

        public event ServerPacketReveivedEventHandler PacketReceivedEvent;

        public NetTcpServer(TcpListener tcpListener, StreamProtobuf streamProtobuf)
        {
            TcpListener = tcpListener;
            TcpListener.Start();

            StreamProtobuf = streamProtobuf;

            Clients = new ConcurrentBag<NetTcpClient>();
        }

        public async Task AcceptClients(Predicate<NetTcpServer, TcpClient> shouldAcceptClient,
                                        Predicate<NetTcpServer> keepAcceptingClients)
        {
            var client = await TcpListener.AcceptTcpClientAsync().ConfigureAwait(false);

            if (shouldAcceptClient(this, client)) Clients.TryAdd(new NetTcpClient(client, StreamProtobuf));

            if (!keepAcceptingClients(this)) return;

            await AcceptClients(shouldAcceptClient, keepAcceptingClients).ConfigureAwait(false); // Stack overflow, yeet!
        }

        public async Task AcceptPackets()
        {
            await Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        foreach (var client in Clients)
                        {
                            if (StreamProtobuf.ReadNext(client.Stream, out object value)) PacketReceivedEvent(this, client, value);
                        }
                    }
                }, TaskCreationOptions.LongRunning).ConfigureAwait(false);
        }

        public void Broadcast<T>(T obj)
        {
            foreach (var client in Clients) client.SendToStream(obj);
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    foreach (var client in Clients) client.Dispose();
                    TcpListener.Stop();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~NetTcpServer()
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
        #endregion
    }
}

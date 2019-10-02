namespace BeepLive.Net
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class NetTcpServer : IDisposable
    {
        public TcpListener TcpListener { get; }
        public StreamProtobuf StreamProtobuf { get; }
        public ICollection<(TcpClient Client, NetworkStream Stream)> Clients { get; }

        public event PacketReveivedEventHandler PacketReceivedEvent;

        public NetTcpServer(TcpListener tcpListener)
        {
            TcpListener = tcpListener;
            TcpListener.Start();
        }

        public async Task AcceptClients(Predicate<NetTcpServer, TcpClient> shouldAcceptClient,
                                        Predicate<NetTcpServer> keepAcceptingClients)
        {
            var client = await TcpListener.AcceptTcpClientAsync().ConfigureAwait(false);

            if (shouldAcceptClient(this, client)) Clients.Add((client, client.GetStream()));

            if (!keepAcceptingClients(this)) return;

            await AcceptClients(shouldAcceptClient, keepAcceptingClients).ConfigureAwait(false); // Stack overflow, yeet!
        }

        public async Task AcceptPackets()
        {
            await Task.Factory.StartNew(() =>
                {
                    foreach ((TcpClient client, NetworkStream stream) in Clients)
                    {
                        if (StreamProtobuf.ReadNext(stream, out object value)) PacketReceivedEvent(this, client, stream, value);
                    }
                }, TaskCreationOptions.LongRunning).ConfigureAwait(false);
        }

        public void Broadcast<T>(T obj)
        {
            foreach ((TcpClient client, NetworkStream stream) in Clients) StreamProtobuf.WriteNext(stream, obj);
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
                    foreach ((TcpClient client, NetworkStream stream) in Clients) stream.Dispose();
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

    /// <summary>
    /// Represents the method that defines a set of criteria and determines whether the
    /// specified object meets those criteria.
    /// </summary>
    /// <typeparam name="T1">The object to compare against the criteria defined within the method represented
    /// by this delegate.</typeparam>
    /// <param name="obj1">The type of the object to compare.</param>
    /// <typeparam name="T2">The object to compare against the criteria defined within the method represented
    /// by this delegate.</typeparam>
    /// <param name="obj2">The type of the object to compare.</param>
    public delegate bool Predicate<in T1, in T2>(T1 obj1, T2 obj2);

    public delegate void PacketReveivedEventHandler(NetTcpServer netTcpServer, TcpClient client, NetworkStream stream, object obj);
}

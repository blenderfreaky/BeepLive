using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System;
namespace BeepLive.Server
{
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;

    public class Server
    {
        private HttpListener Fetcher;
        private Func<string, string> Action;

        public Server(IPAddress address, Func<string, string> meth)
        {
            Fetcher = new HttpListener();
            Fetcher.Prefixes.Add(address.ToString());
            Action = meth;
        }

        public void Run() => ThreadPool.QueueUserWorkItem(Listen);

        private void Listen(object stateInfo)
        {
            while(Fetcher.IsListening)
            {
                ThreadPool.QueueUserWorkItem(call =>
                {
                    HttpListenerContext context = stateInfo as HttpListenerContext;
                    using Stream contextStream = context.Request.InputStream;
                    using StreamReader reader = new StreamReader(contextStream);
                    string data = reader.ReadToEnd();
                    byte[] u = Encoding.UTF8.GetBytes(Action(data));
                    context.Response.OutputStream.Write(new ReadOnlySpan<byte>(u));
                }, Fetcher.GetContext());
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace JapaneseAutocompleteServer
{
    class Server
    {
        private TcpListener _listener;
        private int _port;

        public IClientHandler Handler { get; set; }

        public Server(int port, IClientHandler handler)
        {
            //if (port <= IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            //    throw new ArgumentOutOfRangeException("incorrect port number");

            if (handler == null)
                throw new ArgumentNullException("handler must not be null");

            _listener = new TcpListener(IPAddress.Any, port);
            Handler = handler;

        }

        ~Server()
        {
            Stop();
        }

        public void Start()
        {
            if (_listener == null)
                _listener = new TcpListener(IPAddress.Any, _port);

            _listener.Start();

            ListeningLoop();
        }

        public void Stop()
        {
            _listener.Stop();
        }

        private void ListeningLoop()
        {
            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();
                
                int workerThreads, completionPortThreads;
                ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);

                if (workerThreads > 0)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ClientWaitCallback), client);
                }
                else
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(ClientWaitCallback));
                    thread.Start(client);
                }
            }
        }

        private void ClientWaitCallback(object client)
        {
            Handler.ProcessCLient((TcpClient)client);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Suggestion;
using System.IO;

namespace JapaneseAutocompleteServer
{
    delegate void CommandHandler(string[] args, TcpClient client);

    class ClientHandler : JapaneseAutocompleteServer.IClientHandler
    {
        private readonly int TimeOut = 60;

        private Vocabulary _vocab;
        private Dictionary<string, CommandHandler> _commands;
        private System.Diagnostics.Stopwatch _timer;

        public ClientHandler(string vocabularyFile)
        {
            _vocab = new Vocabulary(vocabularyFile);

            _commands = new Dictionary<string, CommandHandler>();
            _commands.Add("get", GetCommandHandler);

            _timer = new System.Diagnostics.Stopwatch();
        }

        public void ProcessCLient(TcpClient client)
        {
            Console.Out.WriteLine("client " + client.ToString() + " connected");
            
            client.ReceiveTimeout = TimeOut * 1000;

            byte[] buffer = new byte[client.ReceiveBufferSize];
            _timer.Restart();

            Stream stream = client.GetStream();
            int count = 1;
            string requestStr = "";

            while (count > 0 && _timer.Elapsed.Seconds < TimeOut)
            {
                try
                {
                    count = stream.Read(buffer, 0, client.ReceiveBufferSize);

                    requestStr += Encoding.ASCII.GetString(buffer, 0, count);

                    int end = requestStr.IndexOf("\n\r");
                    if (end >= 0)
                    {
                        string[] command = requestStr.Substring(0, end).Split(' ');

                        stream.Flush();

                        if (_commands.ContainsKey(command[0]))
                        {
                            _commands[command[0]](command.Skip(1).ToArray(), client);
                        }

                        requestStr = "";
                        _timer.Restart();
                    }
                }
                catch
                {
                    break;
                }
            }

            Console.Out.WriteLine("client " + client.ToString() + " lost");
            client.Close();
            _timer.Stop();
        }

        protected void GetCommandHandler(string[] args, TcpClient client)
        {
            if (args.Length < 1)
                return;

            Stream stream = client.GetStream();

            TextWriter tw = new StreamWriter(client.GetStream());

            string[] completions = _vocab.GetComplitions(args[0], 10);

            byte[] buffer;
            foreach(string s in completions)
            {
                string response = s + "\n\r";
                buffer = Encoding.ASCII.GetBytes(response);
                stream.Write(buffer, 0, response.Length);
            }

            buffer = Encoding.ASCII.GetBytes("\n\r\n\r");
            stream.Write(buffer, 0, buffer.Length);
        }

    }

}

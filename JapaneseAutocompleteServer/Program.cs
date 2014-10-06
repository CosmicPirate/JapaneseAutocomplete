using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace JapaneseAutocompleteServer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Out.Write(@"Использование: JapaneseAutocompleteServer.exe <файл со словами> <порт>");
                return;
            }

            int port = -1;

            if (!int.TryParse(args[1], out port) || port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            {
                Console.Out.WriteLine("Порт не правильный");
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.Out.WriteLine("Файл не существует");
                return;
            }

            Server server = new Server(port, new ClientHandler(args[0]));
            server.Start();
        }


    }
}

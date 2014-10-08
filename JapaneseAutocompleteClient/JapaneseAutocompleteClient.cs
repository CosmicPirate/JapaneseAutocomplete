using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Suggestion;

namespace JapaneseAutocompleteClient
{
    class JapaneseAutocompleteClient
    {
        static void Main(string[] args)
        {
            int port = -1;
            IPAddress serverAddr;

            if (args.Length != 2)
                Console.Out.WriteLine(@"Использование: JapaneseAutocompleteClient.exe <ip-адрес> <порт>");

            if (!IPAddress.TryParse(args[0], out serverAddr))
            {
                Console.Out.WriteLine("IP аддрес не правильный");
            }

            if (!int.TryParse(args[1], out port) || port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            {
                Console.Out.WriteLine("Порт не правильный");
                return;
            }

            IPEndPoint serverEndPoint = new IPEndPoint(serverAddr, port);

            Vocabulary vocab = null;
            try
            {
                vocab = new Vocabulary(serverEndPoint);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                return;
            }

            Console.Out.WriteLine("Вводите слова. Для выхода введите пустую строку.");
            Console.Out.Write("> ");

            string str = Console.In.ReadLine();

            while(str.Length > 0)
            {
                try
                {
                    string[] completions = vocab.GetCompletionsByNetwork(str, 10, serverEndPoint);
                    foreach (string s in completions)
                    {
                        Console.Out.WriteLine(s);
                    }
                }
                catch (System.IO.IOException ex)
                {
                    Console.Out.WriteLine(ex.Message);
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    Console.Out.WriteLine(ex.Message);
                }

                Console.Out.Write("> ");

                str = Console.In.ReadLine();
            }
        }
    }
}

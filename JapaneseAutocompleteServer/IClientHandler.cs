using System;
namespace JapaneseAutocompleteServer
{
    interface IClientHandler
    {
        void ProcessCLient(System.Net.Sockets.TcpClient client);
    }
}

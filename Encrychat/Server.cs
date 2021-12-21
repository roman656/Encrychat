using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Encrychat
{
    public class Server
    {
        private readonly IPAddress _localAddress = IPAddress.Any;
        private readonly List<Client> _clients = new ();
        private TcpListener _listener;

        public void Listen()
        {
            try
            {
                _listener = new TcpListener(_localAddress, Program.Port);
                _listener.Start();

                while (true)
                {
                    Console.WriteLine("Ожидание подключений...");
                    
                    var tcpClient = _listener.AcceptTcpClient();
                    
                    Console.WriteLine("Принят запрос на подключение.");
                    
                    var client = new Client(tcpClient, this);
                    var clientThread = new Thread(client.Process);
                    clientThread.Start();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Disconnect();
            }
        }
        
        public void BroadcastMessage(string message, string senderIndex)
        {
            var data = Encoding.UTF8.GetBytes(message);
            
            for (var i = 0; i < _clients.Count; i++)
            {
                if (_clients[i].Index != senderIndex)
                {
                    _clients[i].Stream.Write(data, 0, data.Length);
                }
            }
        }
        
        public void AddConnection(Client client) => _clients.Add(client);

        public void DeleteConnection(string clientIndex)
        {
            var client = _clients.FirstOrDefault(client => client.Index == clientIndex);
            
            if (client != null)
            {
                _clients.Remove(client);
            }
        }

        public void Disconnect()
        {
            _listener.Stop();
 
            for (var i = 0; i < _clients.Count; i++)
            {
                _clients[i].Close();
            }
            
            Environment.Exit(0);    // Завершение процесса.
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Encrychat
{
    public class Server
    {
        public readonly List<Client> Clients = new ();
        private TcpListener _listener;

        public void Listen()
        {
            try
            {
                _listener = new TcpListener(Program.LocalAddress, Program.Port);
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
        
        public void SendBroadcastMessage(string message, string senderIndex)
        {
            var data = Encoding.UTF8.GetBytes(message);
            
            for (var i = 0; i < Clients.Count; i++)
            {
                if (Clients[i].Index != senderIndex)
                {
                    Clients[i].Stream.Write(data, 0, data.Length);
                }
            }
        }
        
        public void AddConnection(Client client) => Clients.Add(client);

        public void DeleteConnection(string clientIndex)
        {
            var client = Clients.FirstOrDefault(client => client.Index == clientIndex);
            
            if (client != null)
            {
                Clients.Remove(client);
            }
        }

        public void Disconnect()
        {
            _listener.Stop();
 
            for (var i = 0; i < Clients.Count; i++)
            {
                Clients[i].Close();
            }
            
            Environment.Exit(0);    // Завершение процесса.
        }
    }
}
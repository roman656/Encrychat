using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Encrychat
{
    public class Client
    {
        public readonly string Index = Guid.NewGuid().ToString();
        private readonly TcpClient _client;
        private readonly Server _server;
        public string Username;
        public NetworkStream Stream { get; private set; }
 
        public Client(TcpClient client, Server server)
        {
            _client = client;
            _server = server;
            _server.AddConnection(this);
        }
 
        public void Process()
        {
            try
            {
                Stream = _client.GetStream();
                
                var initMessage = GetMessage();
                Username = initMessage;
                
                var message = $"-[{Username} вошел в чат]-\n";
                _server.SendBroadcastMessage(message, Index);

                while (true)
                {
                    try
                    {
                        Thread.Sleep(30);
                        message = GetMessage();
                    }
                    catch
                    {
                        message = $"-[{Username} покинул чат]-\n";
                        _server.SendBroadcastMessage(message, Index);
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                _server.DeleteConnection(Index);
                Close();
            }
        }
        
        private string GetMessage()
        {
            var data = new byte[_client.ReceiveBufferSize];
            
            Stream.Read(data, 0, _client.ReceiveBufferSize);
            
            return Encoding.UTF8.GetString(data);
        }
        
        public void Close()
        {
            Stream?.Close();
            _client?.Close();
        }
    }
}
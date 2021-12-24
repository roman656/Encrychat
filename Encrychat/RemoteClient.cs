using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Encrychat
{
    public class RemoteClient
    {
        public readonly string Index = Guid.NewGuid().ToString();
        private readonly TcpClient _client;
        private readonly LocalListener _localListener;
        public string Username;
        public string PublicKey;
        public NetworkStream Stream { get; private set; }
 
        public RemoteClient(TcpClient client, LocalListener localListener)
        {
            _client = client;
            _localListener = localListener;
            _localListener.AddConnection(this);
        }
 
        public void Process()
        {
            try
            {
                Stream = _client.GetStream();
                
                var initMessage = GetMessage();
                Username = initMessage;
                
                var message = $"-[{Username} вошел в чат]-\n";
                //_localServer.SendBroadcastMessage(message, Index);

                while (true)
                {
                    try
                    {
                        Thread.Sleep(50);
                        message = GetMessage();
                    }
                    catch
                    {
                        message = $"-[{Username} покинул чат]-\n";
                        //_localServer.SendBroadcastMessage(message, Index);
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
                _localListener.DeleteConnection(Index);
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
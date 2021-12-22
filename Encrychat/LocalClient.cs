using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Encrychat
{
    public class LocalClient
    {
        public string Username = Settings.DefaultUsername;
        private readonly TcpClient _client = new ();
        private readonly NetworkStream _stream;

        public LocalClient()
        {
            try
            {
                _client.Connect(Settings.LocalAddress, Settings.Port);
                _stream = _client.GetStream();
                
                SendMessage(Username);

                var receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Disconnect();
            }
        }

        public void SendMessage(string message)
        {
            var data = Encoding.UTF8.GetBytes(message);
            _stream.Write(data, 0, data.Length);
        }

        private void ReceiveMessage()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(30);
                    var data = new byte[_client.ReceiveBufferSize];
                    _stream.Read(data, 0, _client.ReceiveBufferSize);
                    var message = Encoding.UTF8.GetString(data);
                }
            }
            catch
            {
                Console.WriteLine("Подключение было прервано.");
                Disconnect();
            }
        }
 
        private void Disconnect()
        {
            _stream?.Close();
            _client?.Close();
            Environment.Exit(0);
        }
    }
}
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Encrychat
{
    public class RemoteClient
    {
        public readonly IPAddress Address;
        private readonly string _publicKey;
        private TcpClient _client;
        private NetworkStream _stream;
        private readonly AddMessage _addMessageDelegate;
 
        public RemoteClient(IPAddress address, string publicKey, AddMessage addMessageDelegate)
        {
            Address = address;
            _publicKey = publicKey;
            _addMessageDelegate = addMessageDelegate;
        }
 
        public void SendMessage(string message)
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(Address, Settings.Port);
                _stream = _client.GetStream();
                
                var encryptedMessage = Encryptor.Encrypt(message, _publicKey);
                _addMessageDelegate.Invoke(Address, encryptedMessage, true);
                var data = Encoding.UTF8.GetBytes(encryptedMessage);
                _stream.Write(data, 0, data.Length);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                Close();
            }
        }
        
        public string GetMessage(TcpClient client)
        {
            var data = new byte[Settings.MessageDataBufferSize];
            var builder = new StringBuilder();
            
            try
            {
                _client = client;
                _stream = _client.GetStream();

                do
                {
                    var readBytesAmount = _stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.UTF8.GetString(data, 0, readBytesAmount));
                }
                while (_stream.DataAvailable);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                Close();
            }
            
            _addMessageDelegate.Invoke(Address, builder.ToString(), true);
            var decryptedMessage = Encryptor.Decrypt(builder.ToString());
            return decryptedMessage;
        }
        
        private void Close()
        {
            _stream?.Close();
            _client?.Close();
        }
    }
}
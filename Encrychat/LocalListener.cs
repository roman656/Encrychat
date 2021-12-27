using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Encrychat
{
    public class LocalListener
    {
        private readonly List<RemoteClient> _remoteClients = new ();
        private readonly Thread _listenThread;
        private TcpListener _listener;
        private readonly AddMessage _addMessageDelegate;

        public LocalListener(AddMessage addMessageDelegate)
        {
            try
            {
                _addMessageDelegate = addMessageDelegate;
                _listenThread = new Thread(Listen);
                _listenThread.IsBackground = true;
                _listenThread.Start();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private void Listen()
        {
            try
            {
                _listener = new TcpListener(Settings.LocalAddress, Settings.Port);
                _listener.Start();

                while (true)
                {
                    var remoteClient = _listener.AcceptTcpClient();
                    var address = ((IPEndPoint)remoteClient.Client.RemoteEndPoint)?.Address;
                    var message = string.Empty;
                    
                    foreach (var client in _remoteClients)
                    {
                        if (client.Address.Equals(address))
                        {
                            message = client.GetMessage(remoteClient);
                            break;
                        }
                    }
                    
                    _addMessageDelegate.Invoke(address, message, false);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                _listener?.Stop();
            }
        }
        
        public void SendMessageToAllClients(string message)
        {
            foreach (var client in _remoteClients)
            {
                client.SendMessage(message);
            }
        }
        
        public bool AddRemoteClient(RemoteClient newClient)
        {
            var hasThisClient = false;
            
            foreach (var client in _remoteClients)
            {
                if (client.Address.Equals(newClient.Address))
                {
                    hasThisClient = true;
                    break;
                }
            }

            if (!hasThisClient)
            {
                _remoteClients.Add(newClient);
            }

            return !hasThisClient;
        }
    }
}
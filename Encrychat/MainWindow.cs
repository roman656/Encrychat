using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace Encrychat
{
    public delegate void AddMessage(IPAddress userAddress, string message, bool isEncrypted);
    
    public class MainWindow : Window
    {
        [UI] private Button _sendButton;
        [UI] private Button _keysButton;
        [UI] private TextView _chatTextField;
        [UI] private TextView _messageTextField;
        private readonly AddMessage _addMessageDelegate;
        private readonly LocalListener _localListener;
        private readonly UdpClient _receivingClient = new (Settings.Port);
        private Thread _receivingThread;

        public MainWindow() : this(new Builder("MainWindow.glade")) {}
        
        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);
            
            DeleteEvent += WindowDeleteEvent;
            _sendButton.Clicked += SendButtonClicked;
            _keysButton.Clicked += KeysButtonClicked;

            _addMessageDelegate = PrintMessageToChat;
            _localListener = new LocalListener(_addMessageDelegate);
            
            InitializeReceiver();
            SendInitialBroadcastMessage(Encryptor.PublicKey);
        }

        private static void SendInitialBroadcastMessage(string message)
        {
            var data = Encoding.UTF8.GetBytes(message);
            var client = new UdpClient();
            
            client.EnableBroadcast = true;
            client.Send(data, data.Length, IPAddress.Broadcast.ToString(), Settings.Port);
            client.Close();
        }
        
        private static void SendInitialDirectMessage(IPAddress address, string message)
        {
            var data = Encoding.UTF8.GetBytes(message);
            var client = new UdpClient();
   
            client.Send(data, data.Length, address.ToString(), Settings.Port);
            client.Close();
        }
        
        private void InitializeReceiver()
        {
            _receivingThread = new Thread(Receiver);
            _receivingThread.IsBackground = true;
            _receivingThread.Start();
        }
        
        private void Receiver()
        {
            IPEndPoint endPoint = null;

            try
            {
                while (true)
                {
                    var data = _receivingClient.Receive(ref endPoint);
                    
                    if (endPoint.Address.Equals(Settings.LocalAddress))
                    {
                        continue;
                    }
                    
                    var message = Encoding.ASCII.GetString(data);
                    var wasAdded = _localListener.AddRemoteClient(new RemoteClient(endPoint.Address, message, _addMessageDelegate));
                    
                    if (wasAdded)
                    {
                        SendInitialDirectMessage(endPoint.Address, Encryptor.PublicKey);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                _receivingClient.Close();
            }
        }

        private void WindowDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
            Environment.Exit(0);
        }

        private void KeysButtonClicked(object sender, EventArgs a) => PrintKeysToChat();

        private void SendButtonClicked(object sender, EventArgs a)
        {
            var message = _messageTextField.Buffer.Text.Trim();
            
            if (message != string.Empty)
            {
                PrintMessageToChat(Settings.LocalAddress, message);
                _messageTextField.Buffer.Text = string.Empty;
                _localListener.SendMessageToAllClients(message);
            }
        }

        private void PrintMessageToChat(IPAddress userAddress, string message, bool isEncrypted = false)
        {
            _chatTextField.Buffer.Text += $"[{userAddress}]{(isEncrypted ? "[шифрованное]" : "")}: {message}\n";
        }
        
        private void PrintKeysToChat()
        {
            _chatTextField.Buffer.Text += Encryptor.GetKeysMessage();
        }
    }
}
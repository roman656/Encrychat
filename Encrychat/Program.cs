using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Gtk;

namespace Encrychat
{
    public class Program
    {
        public const int Port = 8888;
        public const string DefaultUsername = "NewUser";
        public static string Username = DefaultUsername;
        private static readonly TcpClient Client = new ();
        private static NetworkStream _stream;
        private static readonly Server Server = new ();
        private static readonly Thread ListenThread = new (Server.Listen);
        
        [STAThread]
        public static void Main(string[] args)
        {
            StartServer();
            StartClient();
            Application.Init();

            var application = new Application("roman656.Encrychat", GLib.ApplicationFlags.None);
            var mainWindow = new MainWindow();
            
            application.Register(GLib.Cancellable.Current);
            application.AddWindow(mainWindow);
            mainWindow.Show();
            Application.Run();
        }

        private static void StartServer()
        {
            try
            {
                ListenThread.Start();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Server.Disconnect();
            }
        }

        private static void StartClient()
        {
            try
            {
                Client.Connect("localhost", Port);
                _stream = Client.GetStream();
                SendMessage(Username);

                var receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        public static void SendMessage(string message)
        {
            var data = Encoding.UTF8.GetBytes(message);
            _stream.Write(data, 0, data.Length);
        }

        private static void ReceiveMessage()
        {
            try
            {
                while (true)
                {
                    var data = new byte[Client.ReceiveBufferSize];
                
                    _stream.Read(data, 0, Client.ReceiveBufferSize);
                
                    var message = Encoding.UTF8.GetString(data);
                }
            }
            catch
            {
                Console.WriteLine("Подключение было прервано.");
                Disconnect();
            }
        }
 
        private static void Disconnect()
        {
            _stream?.Close();
            Client?.Close();
            Environment.Exit(0);
        }
    }
}
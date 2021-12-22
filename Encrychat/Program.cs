using System;
using System.Threading;
using Gtk;

namespace Encrychat
{
    public static class Program
    {
        private static LocalServer _localServer = new ();
        private static readonly Thread ListenThread = new (_localServer.Listen);
        private static LocalClient _localClient;

        [STAThread]
        public static void Main(string[] args)
        {
            StartServer();
            _localClient = new LocalClient();
            Application.Init();

            var application = new Application("roman656.Encrychat", GLib.ApplicationFlags.None);
            var mainWindow = new MainWindow(_localServer);
            
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
                _localServer.Disconnect();
            }
        }
    }
}
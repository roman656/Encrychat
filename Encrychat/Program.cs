using System;
using Gtk;

namespace Encrychat
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            var application = new Application("roman656.Encrychat", GLib.ApplicationFlags.None);
            var mainWindow = new MainWindow();
            
            application.Register(GLib.Cancellable.Current);
            application.AddWindow(mainWindow);
            mainWindow.Show();
            Application.Run();
        }
    }
}
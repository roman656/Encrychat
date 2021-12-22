using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace Encrychat
{
    public class MainWindow : Window
    {
        [UI] private Label _messageLabel;
        [UI] private Label _membersLabel;
        [UI] private Button _sendButton;
        [UI] private Button _keysButton;
        [UI] private TextView _chatTextField;
        [UI] private TextView _messageTextField;
        [UI] private TextView _usernameTextField;
        [UI] private TextView _membersList;
        private readonly LocalServer _localServer;

        public MainWindow(LocalServer localServer) : this(new Builder("MainWindow.glade"))
        {
            _localServer = localServer;
        }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);
            
            _usernameTextField.Buffer.Text = Settings.DefaultUsername;
            //UpdateMembersListView();
            DeleteEvent += WindowDeleteEvent;
            _sendButton.Clicked += SendButtonClicked;
            _usernameTextField.Buffer.Changed += UsernameChanged;
        }

        private void WindowDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
            Environment.Exit(0);
        }

        private void UsernameChanged(object sender, EventArgs a)
        {
            var newUserName = _usernameTextField.Buffer.Text.Trim();

            if (newUserName != string.Empty)
            {
                // сообщить другим новый ник. (старый\nновый)
                //_usernames[0] = newUserName;
                UpdateMembersListView();
            }
        }

        private void UpdateMembersListView()
        {
            _membersList.Buffer.Text = string.Empty;

            for (var i = 0; i < _localServer.Clients.Count; i++)
            {
                _membersList.Buffer.Text += _localServer.Clients[i].Username + (i == 0 ? " (Вы)\n" : "\n");
            }
        }

        private void SendButtonClicked(object sender, EventArgs a)
        {
            var message = _messageTextField.Buffer.Text.Trim();
            
            if (message != string.Empty)
            {
                var resultMessage = _localServer.Clients[0].Username + ": " + message + "\n";

                _chatTextField.Buffer.Text += resultMessage;
                _messageTextField.Buffer.Text = string.Empty;
                //Program.SendMessage(resultMessage);
            }
        }
    }
}
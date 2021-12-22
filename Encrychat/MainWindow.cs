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
        [UI] public TextView _chatTextField;
        [UI] private TextView _messageTextField;
        [UI] private TextView _usernameTextField;
        [UI] private TextView _membersList;

        public MainWindow() : this(new Builder("MainWindow.glade")) {}

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);
            
            _usernameTextField.Buffer.Text = Program.Server.Clients[0].Username;
            UpdateMembersListView();
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

            for (var i = 0; i < Program.Server.Clients.Count; i++)
            {
                _membersList.Buffer.Text += Program.Server.Clients[i].Username + (i == 0 ? " (Вы)\n" : "\n");
            }
        }

        private void SendButtonClicked(object sender, EventArgs a)
        {
            var message = _messageTextField.Buffer.Text.Trim();
            
            if (message != string.Empty)
            {
                var resultMessage = Program.Server.Clients[0].Username + ": " + message + "\n";

                _chatTextField.Buffer.Text += resultMessage;
                _messageTextField.Buffer.Text = string.Empty;
                Program.SendMessage(resultMessage);
            }
        }
    }
}
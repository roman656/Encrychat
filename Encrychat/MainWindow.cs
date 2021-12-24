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
        private readonly LocalListener _localListener;
        private readonly LocalClient _localClient;

        public MainWindow() : this(new Builder("MainWindow.glade")) {}
        
        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);
            
            DeleteEvent += WindowDeleteEvent;
            _sendButton.Clicked += SendButtonClicked;
            _keysButton.Clicked += KeysButtonClicked;
            _usernameTextField.Buffer.Changed += UsernameChanged;

            _localListener = new LocalListener();
            _localClient = new LocalClient();
            _usernameTextField.Buffer.Text = _localClient.Username;
            UpdateMembersListView();
        }

        private void WindowDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
            Environment.Exit(0);
        }

        private void KeysButtonClicked(object sender, EventArgs a) => PrintKeysToChat();

        private void UsernameChanged(object sender, EventArgs a)
        {
            var newUserName = _usernameTextField.Buffer.Text.Trim();

            if (newUserName != string.Empty)
            {
                // сообщить другим новый ник. (старый\nновый)
                _localClient.Username = newUserName;
                UpdateMembersListView();
            }
        }

        private void UpdateMembersListView()
        {
            _membersList.Buffer.Text = string.Empty;
            _membersList.Buffer.Text += $"{_localClient.Username} (Вы)\n";

            foreach (var client in _localListener.Clients)
            {
                _membersList.Buffer.Text += $"{client.Username}\n";
            }
        }

        private void SendButtonClicked(object sender, EventArgs a)
        {
            var message = _messageTextField.Buffer.Text.Trim();
            
            if (message != string.Empty)
            {
                //var encryptedMessage = Encryptor.Encrypt(message);
                
                PrintMessageToChat(_localClient.Username, message);
                //PrintMessageToChat($"{_localClient.Username}][шифрованное", encryptedMessage);
                _messageTextField.Buffer.Text = string.Empty;
                _localListener.SendBroadcastMessage(message, _localClient.Index);
            }
        }

        private void PrintMessageToChat(string username, string message)
        {
            _chatTextField.Buffer.Text += $"[{username}]: {message}\n";
        }
        
        private void PrintKeysToChat()
        {
            _chatTextField.Buffer.Text += Encryptor.GetKeysMessage();
        }
    }
}
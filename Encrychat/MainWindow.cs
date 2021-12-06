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

        public MainWindow() : this(new Builder("MainWindow.glade")) {}

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);

            _usernameTextField.Buffer.Text = "DefaultUsername";
            _membersList.Buffer.Text += _usernameTextField.Buffer.Text + "\n";
            DeleteEvent += Window_DeleteEvent;
            _sendButton.Clicked += SendButtonClicked;
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void SendButtonClicked(object sender, EventArgs a)
        {
            if (_messageTextField.Buffer.Text != string.Empty)
            {
                _chatTextField.Buffer.Text +=
                        _usernameTextField.Buffer.Text + ": " + _messageTextField.Buffer.Text + "\n";
                _messageTextField.Buffer.Text = string.Empty;
            }
        }
    }
}
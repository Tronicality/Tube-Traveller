using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Tube_Traveller.Accounts;

namespace Tube_Traveller
{
    /// <summary>
    /// Interaction logic for AccountWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private Account? userAccount;
        public Account? GetAccount() => userAccount;

        private List<string>? stations;
        public void SetStations(List<string> newStations) { stations = newStations; }

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            SignUpWindow signUpWindow = new();
            signUpWindow.SetStations(stations!);

            bool? result = signUpWindow.ShowDialog();
            if (result == true) //Left through Register Button
            {
                userAccount = signUpWindow.GetAccount();
                userAccount?.SetId(Database.Database.GetUserId(userAccount.GetUsername()!)!);
                DialogResult = true; //Tells Main window that a new account has been made
            }
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, TextBox> requiredFields = new(){
                {"Login", UsernameBox},
                {"Password", PasswordBox}
            };
            if (CheckRequiredInfo(requiredFields))
            {
                try
                {
                    userAccount = Account.GetAccountFromDatabase(UsernameBox.Text, PasswordBox.Text);
                    if (userAccount != null)
                    {
                        DialogResult = true; //Tells main window account is found
                    }
                    FailedLoginLabel.Content = "Account is not found";
                }
                catch (System.NullReferenceException)
                {
                    FailedLoginLabel.Content = "Account is not found";
                }
            }
        }

        /// <summary>
        /// Whether the user has entered data within textboxes
        /// </summary>
        /// <param name="requiredFields">The textboxes that cannot be null or "". The key being the default textbox text type</param>
        /// <returns>true if all textboxes have inputs, else false</returns>
        private bool CheckRequiredInfo(Dictionary<string, TextBox> requiredFields)
        {
            List<string> missingFields = new();
            foreach (var field in requiredFields)
            {
                if (field.Value.Text == Text.Get(field.Key))
                {
                    missingFields.Add(field.Key);
                }
            }

            if (missingFields.Count > 0)
            {
                FailedText(missingFields.ToArray());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Presents the user of missing required textboxes
        /// </summary>
        /// <param name="missingFields">The missing textboxes</param>
        private void FailedText(string[] missingFields)
        {
            FailedLoginLabel.Content = "You have to enter ";

            foreach (var field in missingFields)
            {
                if (field == "Login")
                {
                    FailedLoginLabel.Content += ":Username or Email:"; //To make login not look weird
                }
                FailedLoginLabel.Content += $":{field}:";
            }
        }
        
        private void UsernameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameBox.Text == Text.Get("Login"))
            {
                UsernameBox.Text = string.Empty;
            }
        }
        private void UsernameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameBox.Text == string.Empty)
            {
                UsernameBox.Text = Text.Get("Login");
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Text == Text.Get("Password"))
            {
                PasswordBox.Text = string.Empty;
            }
        }
        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Text == string.Empty)
            {
                PasswordBox.Text = Text.Get("Password");
            }
        }
    }
}

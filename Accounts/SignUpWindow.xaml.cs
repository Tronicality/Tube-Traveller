using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tube_Traveller.Accounts
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        private Account userAccount;
        public Account? GetAccount() => userAccount;

        private List<string> stations = new();
        public void SetStations(List<string> newStations) 
        { 
            stations = newStations;
            stations.Sort();
            HomeStationComboBox.ItemsSource = stations;
        }

        public SignUpWindow()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, TextBox> requiredFields = new(){
                {"Username", UsernameBox},
                {"First Name", FirstNameBox},
                {"Password", PasswordBox}
            };

            if (CheckRequiredInfo(requiredFields) == true)
            {
                userAccount = new Account(UsernameBox.Text, FirstNameBox.Text, LastNameBox.Text, PasswordBox.Text, EmailBox.Text, "Regular", HomeStationComboBox.Text);

                if (userAccount.GetActiveAccount() == false)
                {
                    FailedRegistryLabel.Content = "Username Taken";
                }
                else
                {
                    DialogResult = true; //Tells the login window that a new account has been made
                }
            }
        }

        /// <summary>
        /// Whether the user has entered data within textboxes
        /// </summary>
        /// <param name="requiredFields">The textboxes that cannot be null or "". The key being the default textbox text type</param>
        /// <returns>true if all textboxes have inputs, else false</returns>
        private bool CheckRequiredInfo(Dictionary<string,TextBox> requiredFields)
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
            FailedRegistryLabel.Content = "You have to enter ";

            foreach (var field in missingFields)
            {
                FailedRegistryLabel.Content += $":{field}:";
            }
        }

        private void PreviousBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void UsernameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameBox.Text == Text.Get("Username"))
            {
                UsernameBox.Text = string.Empty;
            }
        }
        private void UsernameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameBox.Text == string.Empty)
            {
                UsernameBox.Text = Text.Get("Username");
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
        private void FirstNameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (FirstNameBox.Text == Text.Get("First Name"))
            {
                FirstNameBox.Text = string.Empty;
            }
        }
        private void FirstNameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (FirstNameBox.Text == string.Empty)
            {
                FirstNameBox.Text = Text.Get("First Name");
            }
        }
        private void LastNameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (LastNameBox.Text == Text.Get("Last Name"))
            {
                LastNameBox.Text = string.Empty;
            }
        }
        private void LastNameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (LastNameBox.Text == string.Empty)
            {
                LastNameBox.Text = Text.Get("Last Name");
            }
        }
        private void EmailBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (EmailBox.Text == Text.Get("Email"))
            {
                EmailBox.Text = string.Empty;
            }
        }
        private void EmailBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (EmailBox.Text == string.Empty)
            {
                EmailBox.Text = Text.Get("Email");
            }
        }
    }
}

using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Tube_Traveller.Accounts;

namespace Tube_Traveller
{
    /// <summary>
    /// Interaction logic for AccountSettingsWindow.xaml
    /// </summary>
    public partial class AccountSettingsWindow : Window
    {
        private bool _mouseEntered = false;
        private Account? userAccount;
        public Account? GetAccount() => userAccount;
        public void SetAccount(Account newAccount) { userAccount = newAccount; }

        private List<string> stations = new();
        public void SetStations(List<string> newStations)
        {
            stations = newStations;
            stations.Sort();

            for (int i = 0; i < stations.Count; i++)
            {
                HomeStationComboBox.Items.Add(stations[i]);
            }

            HomeStationComboBox.Items.Insert(0, string.Empty);
        }

        public AccountSettingsWindow()
        {
            InitializeComponent();
            
            RouteMethodComboBox.Items.Add(Text.GetRouteMethod_R());
            RouteMethodComboBox.Items.Add(Text.GetRouteMethod_A());
        }

        private void SetAccountInfo()
        {
            PasswordBox.Text = userAccount?.GetPassword();
            EmailBox.Text = userAccount?.GetEmail();
            RouteMethodComboBox.Text = userAccount?.GetRouteMethod();
            if (!string.IsNullOrEmpty(userAccount?.GetHomeStation()))
            {
                HomeStationComboBox.Text = userAccount.GetHomeStation();
            }
        }

        private void AlterAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            if (EmailBox.Text != string.Empty)
            {
                if (Account.ConfirmKey(EmailBox.Text))
                {
                    AlterAccount();
                }
                else
                {
                    FailedLabel.Content = "Invalid Email";
                }
            }
            else
            {
                AlterAccount();
            }
            
        }

        private void AlterAccount()
        {
            FailedLabel.Content = string.Empty;
            userAccount?.AlterAccount(userAccount.GetUsername()!, PasswordBox.Text, EmailBox.Text, RouteMethodComboBox.Text, HomeStationComboBox.Text);
            FailedLabel.Content = "Saved";
        }

        private void DeleteAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are You sure","",MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                Database.Database.DeleteAccount(userAccount!.GetId()!);
                userAccount = null;
                DialogResult = true;
            }
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void PreviousBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void SettingsWindow_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_mouseEntered == false)
            {
                SetAccountInfo();
                _mouseEntered = true;
            }
        }
    }
}

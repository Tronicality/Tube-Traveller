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
        private Account? _userAccount;
        public Account? GetAccount() => _userAccount;
        public void SetAccount(Account newAccount) { _userAccount = newAccount; }

        private List<string> _stations = new();
        public void SetStations(List<string> newStations)
        {
            _stations = newStations;
            _stations.Sort();

            for (int i = 0; i < _stations.Count; i++)
            {
                HomeStationComboBox.Items.Add(_stations[i]);
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
            PasswordBox.Text = _userAccount?.GetPassword();
            EmailBox.Text = _userAccount?.GetEmail();
            RouteMethodComboBox.Text = _userAccount?.GetRouteMethod();
            if (!string.IsNullOrEmpty(_userAccount?.GetHomeStation()))
            {
                HomeStationComboBox.Text = _userAccount.GetHomeStation();
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
            _userAccount?.AlterAccount(_userAccount.GetUsername()!, PasswordBox.Text, EmailBox.Text, RouteMethodComboBox.Text, HomeStationComboBox.Text);
            FailedLabel.Content = "Saved";
        }

        private void DeleteAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are You sure","",MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                Database.Database.DeleteAccount(_userAccount!.GetId()!);
                _userAccount = null;
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

using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;

namespace Tube_Traveller.Accounts
{
    public class Account : Database.Database
    {
        private string Id;
        public string GetId() => Id;
        private string Username;
        public string GetUsername() => Username;
        private string FirstName;
        public string GetFirstName() => FirstName;
        private string? LastName;
        public string? GetLastName() => LastName;
        private string Password;
        public string GetPassword() => Password;
        private string? Email;
        public string? GetEmail() => Email;
        private string RouteMethod;
        public string GetRouteMethod() => RouteMethod;
        private string? HomeStation;
        public string? GetHomeStation() => HomeStation;

        private bool activeAccount;
        public bool GetActiveAccount() => activeAccount;

        public Account()
        {

        }

        /// <summary>
        /// Creates a new account which even adds it to the database
        /// </summary>
        public Account(string username, string firstName, string? lastName, string password, string? email, string routeMethod, string? homeStation)
        {
            this.Username = username;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Password = password;
            this.Email = email;
            this.RouteMethod = routeMethod;
            this.HomeStation = homeStation;

            StoreAccount(); //Adds account to database
        }

        private void StoreAccount()
        {
            //Check if account username already exists
            string[]? userInfo = GetUserInfoByUsername(Username);
            try
            {
                if (userInfo[1] == Username)
                {
                    activeAccount = false; //Username Taken
                }
                else
                {
                    activeAccount = true;
                    AddAccount(this);
                    GetUserId(Username);
                }
            }
            catch (System.NullReferenceException)
            {
                activeAccount = true;
                AddAccount(this);
                GetUserId(Username);
            }
        }

        /// <summary>
        /// Gets Account from database
        /// </summary>
        /// <param name="givenKey">Username or email</param>
        /// <returns>The account if it exists, else null</returns>
        public static Account? GetAccountFromDatabase(string givenKey, string givenPassword)
        {
            return ConfirmAccount(LookForAccount(givenKey), givenPassword);
        }

        /// <summary>
        /// Checks whether the given password is correct for an account.
        /// </summary>
        /// <returns>If correct returns Account, else null</returns>
        private static Account? ConfirmAccount(Account? account, string givenPassword)
        {
            if (account?.GetPassword() == givenPassword)
            {
                return account;
            }
            return null;
        }

        /// <summary>
        /// Looks for whether the account based on the given key
        /// </summary>
        /// <param name="givenKey">Username or email</param>
        /// <returns>If the account exists returns the Account, else null</returns>
        private static Account? LookForAccount(string givenKey)
        {
            string[]? Information; //Used for getting both User and Extra information
            if (ConfirmKey(givenKey)) //Finds out whether key is username or email
            {
                Information = GetUserInfoByUsername(givenKey);
            }
            else
            {
                Information = GetUserInfoByEmail(givenKey);
            }

            if (Information != null) //Account has been found
            {
                Account userAccount = new();

                userAccount.Id = Information[0];
                userAccount.Username = Information[1];
                userAccount.FirstName = Information[2];
                userAccount.LastName = Information[3];
                userAccount.Password = Information[4];
                userAccount.Email = Information[5];

                Information = GetExtraInfo(userAccount.GetId());

                userAccount.RouteMethod = Information[1];
                userAccount.HomeStation = Information[2];

                return userAccount;
            }

            return null;
        }

        /// <summary>
        /// Finds out whether the key given is a username or email
        /// </summary>
        /// <param name="givenKey">Said username or eamil</param>
        /// <returns>true for Email, false for Username</returns>
        private static bool ConfirmKey(string givenKey)
        {
            Regex emailChecker = new(Text.Get("Email Pattern"));

            if (emailChecker.IsMatch(givenKey.ToLower()))
            {
                return false;
            }
            return true;
        }
    }
}
using System.Text.RegularExpressions;

namespace Tube_Traveller.Accounts
{
    public class Account : Database.Database
    {
        private string? Id;
        public string? GetId() => Id;
        public void SetId(string value) {Id = value;}

        private string? Username;
        public string? GetUsername() => Username;

        private string? Password;
        public string? GetPassword() => Password;

        private string? Email;
        public string? GetEmail() => Email;

        private string? RouteMethod;
        public string? GetRouteMethod() => RouteMethod;

        private string? HomeStation;
        public string? GetHomeStation() => HomeStation;

        private bool hasUniqueUsername; 
        public bool GetHasUniqueUsername() => hasUniqueUsername;

        public Account()
        {

        }

        /// <summary>
        /// Creates a new account which even adds it to the database
        /// </summary>
        public Account(string username, string password, string? email, string routeMethod, string? homeStation)
        {
            SetAccountValues(username, password, email, routeMethod, homeStation);
            StoreAccount(); //Adds account to database
        }

        private void SetAccountValues(string username, string password, string? email, string routeMethod, string? homeStation)
        {
            this.Username = username;
            this.Password = password;
            this.Email = email;
            this.RouteMethod = routeMethod;
            this.HomeStation = homeStation;
        }

        private void StoreAccount()
        {
            //Check if account username already exists
            string[]? userInfo = GetUserInfoByUsername(Username!);
            if (userInfo?[1] == Username)
            {
                hasUniqueUsername = false; //Username Taken
            }
            else
            {
                hasUniqueUsername = true;
                AddAccount(this);
                GetUserId(Username!);
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
            if (ConfirmKey(givenKey) == false) //Finds out whether key is username o+r email
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
                userAccount.Password = Information[2];
                userAccount.Email = Information[3];

                Information = GetExtraInfo(userAccount.GetId()!);

                userAccount.RouteMethod = Information![1];
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
        public static bool ConfirmKey(string givenKey)
        {
            Regex emailChecker = new(Text.GetEmailPattern());

            return emailChecker.IsMatch(givenKey);
        }

        public void AlterAccount(string username, string password, string? email, string routeMethod, string? homeStation)
        {
            SetAccountValues(username, password, email, routeMethod, homeStation);
            UpdateAccount(this);
        }
    }
}
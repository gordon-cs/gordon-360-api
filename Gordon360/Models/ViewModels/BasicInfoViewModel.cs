using System.Linq;

namespace Gordon360.Models.ViewModels
{
    public class BasicInfoViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ConcatonatedInfo { get; set; }
        public string Nickname { get; set; }

        public static implicit operator BasicInfoViewModel(ACCOUNT a)
        {
            BasicInfoViewModel vm = new BasicInfoViewModel
            {
                FirstName = a.firstname,
                LastName = a.lastname,
                UserName = a.AD_Username ?? "",
                ConcatonatedInfo = "",
                Nickname = ""
            };

            return vm;
        }

        public bool FirstNameMatches(string matchString)
        {
            return FirstName?.ToLower() == matchString;
        }

        public bool FirstNameStartsWith(string searchString)
        {
            return FirstName?.ToLower()?.StartsWith(searchString) ?? false;
        }

        public bool FirstNameContains(string searchString)
        {
            return FirstName?.ToLower()?.Contains(searchString) ?? false;
        }

        public bool LastNameMatches(string matchString)
        {
            return LastName?.ToLower() == matchString;
        }

        public bool LastNameStartsWith(string searchString)
        {
            return LastName?.ToLower()?.StartsWith(searchString) ?? false;
        }

        public bool LastNameContains(string searchString)
        {
            return LastName?.ToLower()?.Contains(searchString) ?? false;
        }

        public bool UsernameFirstNameStartsWith(string searchString)
        {
            return GetFirstNameFromUsername()?.StartsWith(searchString) ?? false;
        }

        public bool UsernameLastNameStartsWith(string searchString)
        {
            return GetLastNameFromUsername()?.StartsWith(searchString) ?? false;
        }

        public bool UsernameContains(string searchString)
        {
            return UserName?.ToLower()?.Contains(searchString) ?? false;
        }

        public string GetFirstNameFromUsername()
        {
            return UserName?.Split('.')?[0];
        }

        public string GetLastNameFromUsername()
        {
            return UserName.Contains('.') ? UserName?.Split('.')?[1] : null;
        }

        public bool NicknameMatches(string matchString)
        {
            return Nickname?.ToLower() == matchString;
        }

        public bool NicknameStartsWith(string searchString)
        {
            return Nickname?.ToLower()?.StartsWith(searchString) ?? false;
        }
    }

}
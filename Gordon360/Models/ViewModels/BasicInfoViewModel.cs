using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels
{
    public class BasicInfoViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ConcatonatedInfo { get; set; }
        public string Nickname { get; set; }
        public string MaidenName { get; set; }

        public static implicit operator BasicInfoViewModel(ACCOUNT a)
        {
            BasicInfoViewModel vm = new BasicInfoViewModel
            {
                FirstName = a.firstname,
                LastName = a.lastname,
                UserName = a.AD_Username ?? "",
                ConcatonatedInfo = "",
                Nickname = "",
                MaidenName = ""
            };

            return vm;
        }

        private bool FirstNameMatches(string matchString)
        {
            return FirstName?.ToLower() == matchString;
        }

        private bool FirstNameStartsWith(string searchString)
        {
            return FirstName?.ToLower()?.StartsWith(searchString) ?? false;
        }

        private bool FirstNameContains(string searchString)
        {
            return FirstName?.ToLower()?.Contains(searchString) ?? false;
        }

        private bool LastNameMatches(string matchString)
        {
            return LastName?.ToLower() == matchString;
        }

        private bool LastNameStartsWith(string searchString)
        {
            return LastName?.ToLower()?.StartsWith(searchString) ?? false;
        }

        private bool LastNameContains(string searchString)
        {
            return LastName?.ToLower()?.Contains(searchString) ?? false;
        }

        private bool UsernameFirstNameStartsWith(string searchString)
        {
            return GetFirstNameFromUsername()?.StartsWith(searchString) ?? false;
        }

        private bool UsernameLastNameStartsWith(string searchString)
        {
            return GetLastNameFromUsername()?.StartsWith(searchString) ?? false;
        }

        private bool UsernameContains(string searchString)
        {
            return UserName?.ToLower()?.Contains(searchString) ?? false;
        }

        private string GetFirstNameFromUsername()
        {
            return UserName?.Split('.')?[0] ?? "";
        }

        private string GetLastNameFromUsername()
        {
            return UserName.Contains('.') ? UserName?.Split('.')?[1] ?? "" : "";
        }

        private bool NicknameMatches(string matchString)
        {
            return Nickname?.ToLower() == matchString;
        }

        private bool NicknameStartsWith(string searchString)
        {
            return Nickname?.ToLower().StartsWith(searchString) ?? false;
        }

        private bool NicknameContains(string searchString)
        {
            return Nickname?.ToLower().Contains(searchString) ?? false;
        }

        private bool MaidenNameMatches(string matchString)
        {
            return MaidenName?.ToLower() == matchString;
        }

        private bool MaidenNameStartsWith(string searchString)
        {
            return MaidenName?.ToLower().StartsWith(searchString) ?? false;
        }

        private bool MaidenNameContains(string searchString)
        {
            return MaidenName?.ToLower().Contains(searchString) ?? false;
        }

        public (string matchedValue, int precedence)? MatchSearch(string search)
        {
            return this switch
            {
                _ when FirstNameMatches(search) => (FirstName, 0),
                _ when NicknameMatches(search) => (Nickname, 1),
                _ when LastNameMatches(search) => (LastName, 2),
                _ when MaidenNameMatches(search) => (MaidenName, 3),
                _ when FirstNameStartsWith(search) => (FirstName, 4),
                _ when NicknameStartsWith(search) => (Nickname, 5),
                _ when LastNameStartsWith(search) => (LastName, 6),
                _ when MaidenNameStartsWith(search) => (MaidenName, 7),
                _ when UsernameFirstNameStartsWith(search) => (GetFirstNameFromUsername(), 8),
                _ when UsernameLastNameStartsWith(search) => (GetLastNameFromUsername(), 9),
                _ when FirstNameContains(search) => (FirstName, 10),
                _ when NicknameContains(search) => (Nickname, 11),
                _ when LastNameContains(search) => (LastName, 12),
                _ when MaidenNameContains(search) => (MaidenName, 13),
                _ when UsernameContains(search) => (UserName, 14),
                _ => null
            };
        }

        public (string firstnameMatch, int firstnamePrecedence, string lastnameMatch, int lastnamePrecedence)? MatchSearch(string firstnameSearch, string lastnameSearch)
        {
            (string, int)? firstname = this switch
            {
                _ when FirstNameMatches(firstnameSearch) => (FirstName, 0),
                _ when NicknameMatches(firstnameSearch) => (Nickname, 1),
                _ when FirstNameStartsWith(firstnameSearch) => (FirstName, 4),
                _ when NicknameStartsWith(firstnameSearch) => (Nickname, 5),
                _ when UsernameFirstNameStartsWith(firstnameSearch) => (GetFirstNameFromUsername(), 8),
                _ when FirstNameContains(firstnameSearch) => (FirstName, 10),
                _ when NicknameContains(firstnameSearch) => (Nickname, 11),
                _ => null
            };

            if (firstname is not (string firstnameMatch, int firstnamePrecedence)) return null;

            (string, int)? lastname = this switch
            {
                _ when LastNameMatches(lastnameSearch) => (LastName, 2),
                _ when MaidenNameMatches(lastnameSearch) => (MaidenName, 3),
                _ when LastNameStartsWith(lastnameSearch) => (LastName, 6),
                _ when MaidenNameStartsWith(lastnameSearch) => (MaidenName, 7),
                _ when UsernameLastNameStartsWith(lastnameSearch) => (GetLastNameFromUsername(), 9),
                _ when LastNameContains(lastnameSearch) => (LastName, 12),
                _ when MaidenNameContains(lastnameSearch) => (MaidenName, 13),
                _ => null
            };

            if (lastname is not (string lastnameMatch, int lastnamePrecedence)) return null;

            return (firstnameMatch, firstnamePrecedence, lastnameMatch, lastnamePrecedence);
        }
    }
}

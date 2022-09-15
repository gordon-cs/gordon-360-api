using Gordon360.Models.CCT;
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

        public string? MatchSearch(string search)
        {
            (string, int)? match = this switch
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

            if (match is not (string matchedValue, int matchPrecedence)) return null;

            return string.Concat(Enumerable.Repeat("z", matchPrecedence)) + matchedValue;
        }

        public string? MatchSearch(string firstnameSearch, string lastnameSearch)
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

            var totalPrecedence = firstnamePrecedence + lastnamePrecedence;
            var keyBase = $"{firstnameMatch}1${lastnameMatch}";

            return string.Concat(Enumerable.Repeat("z", totalPrecedence)) + keyBase;
        }

        /// <Summary>
        ///   This function generates a key for each account
        ///   The key is of the form "z...keyBase" where z is repeated precedence times.
        /// </Summary>
        /// <remarks>
        ///   The leading precedence number of z's are used to put keep the highest precedence matches first.
        ///   The keyBase is used to sort within the precedence level.
        /// </remarks>
        ///
        /// <param name="keyBase">The base value to use for the key - i.e. the user's highest precedence info that matches the search string</param>
        /// <param name="precedence">Set where in the dictionary this key group will be ordered</param>


        /// <Summary>
        ///   This function generates a key for each account
        ///   The key is of the form "z...firstname1lastname" where z is repeated precedence times.
        /// </Summary>
        /// <remarks>
        ///   The leading precedence number of z's are used to put keep the highest precedence matches first.
        ///   The keyBase is used to sort within the precedence level.
        /// </remarks>
        ///
        /// <param name="firstnameKey">The firstname value to use for the key - i.e. the user's highest precedence firstname info that matches the search string</param>
        /// <param name="lastnameKey">The lastname value to use for the key - i.e. the user's highest precedence lastname info that matches the search string</param>
        /// <param name="precedence">Set where in the dictionary this key group will be ordered</param>
    }
}

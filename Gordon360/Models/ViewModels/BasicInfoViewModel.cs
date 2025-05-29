using System;
using System.Linq;

namespace Gordon360.Models.ViewModels;

public class BasicInfoViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Nickname { get; set; }
    public string MaidenName { get; set; }


    private static string NormalizeName(string input) =>
        new string(input?.Where(char.IsLetterOrDigit).ToArray() ?? Array.Empty<char>()).ToLower();

    private bool FirstNameEquals(string matchString)
    {
        return NormalizeName(FirstName) == NormalizeName(matchString);
    }

    private bool FirstNameStartsWith(string searchString)
    {
        return NormalizeName(FirstName).StartsWith(NormalizeName(searchString));
    }

    private bool FirstNameContains(string searchString)
    {
        return NormalizeName(FirstName).Contains(NormalizeName(searchString));
    }

    private bool LastNameEquals(string matchString)
    {
        return NormalizeName(LastName) == NormalizeName(matchString);
    }

    private bool LastNameNoSpecialCharacterEquals(string matchString)
    {
        // You can keep this method, but it is effectively same as LastNameEquals now.
        return NormalizeName(LastName) == NormalizeName(matchString);
    }

    private bool LastNameStartsWith(string searchString)
    {
        return NormalizeName(LastName).StartsWith(NormalizeName(searchString));
    }

    private bool LastNameContains(string searchString)
    {
        return NormalizeName(LastName).Contains(NormalizeName(searchString));
    }

    private bool UsernameFirstNameStartsWith(string searchString)
    {
        return NormalizeName(GetFirstNameFromUsername()).StartsWith(NormalizeName(searchString));
    }

    private bool UsernameLastNameStartsWith(string searchString)
    {
        return NormalizeName(GetLastNameFromUsername()).StartsWith(NormalizeName(searchString));
    }

    private bool UsernameContains(string searchString)
    {
        return NormalizeName(UserName).Contains(NormalizeName(searchString));
    }

    private string GetFirstNameFromUsername()
    {
        return UserName?.Split('.')?[0] ?? "";
    }

    private string GetLastNameFromUsername()
    {
        return UserName?.Contains('.') == true ? UserName.Split('.')[1] : "";
    }

    private bool NicknameEquals(string matchString)
    {
        return NormalizeName(Nickname) == NormalizeName(matchString);
    }

    private bool NicknameStartsWith(string searchString)
    {
        return NormalizeName(Nickname).StartsWith(NormalizeName(searchString));
    }

    private bool NicknameContains(string searchString)
    {
        return NormalizeName(Nickname).Contains(NormalizeName(searchString));
    }

    private bool MaidenNameEquals(string matchString)
    {
        return NormalizeName(MaidenName) == NormalizeName(matchString);
    }

    private bool MaidenNameStartsWith(string searchString)
    {
        return NormalizeName(MaidenName).StartsWith(NormalizeName(searchString));
    }

    private bool MaidenNameContains(string searchString)
    {
        return NormalizeName(MaidenName).Contains(NormalizeName(searchString));
    }

    /// <summary>
    /// Matches basic info fields against <c>search</c>, returning a match key representing the value and precedence of the first match, or <c>null</c>.
    /// </summary>
    /// 
    /// <remarks>
    /// The match key is leading 'z's equal to the precedence of the match, followed by the matched field.
    /// This key, when used to sort aplhabetically, will sort matched accounts by the precedence of the matched field and alphabetically within precedence level.
    /// The precedence of a match is determined by the following, in order:
    /// <list type="number">
    /// <item><description>How the search matches the field</description>
    ///     <list type="number">
    ///         <item><description>Equals</description></item>
    ///         <item><description>Starts With</description></item>
    ///         <item><description>Contains</description></item>
    ///     </list>
    /// </item>
    /// <item><description>Which field the search matches</description>
    ///     <list type="number">
    ///         <item><description>FirstName</description></item>
    ///         <item><description>NickName</description></item>
    ///         <item><description>LastName</description></item>
    ///         <item><description>MaidenName</description></item>
    ///         <item><description>UserName</description></item>
    ///     </list>
    /// </item>
    /// </list>
    /// 
    /// </remarks>
    /// 
    /// <param name="search">The search input to match against</param>
    /// <returns>The match key if <c>search</c> matched a field, or <c>null</c></returns>
    public string? MatchSearch(string search)
    {
        (string, int)? match = this switch
        {
            _ when LastNameNoSpecialCharacterEquals(search) => (LastName, 15),
            _ when FirstNameEquals(search) => (FirstName, 0),
            _ when NicknameEquals(search) => (Nickname, 1),
            _ when LastNameEquals(search) => (LastName, 2),
            _ when MaidenNameEquals(search) => (MaidenName, 3),
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

    /// <summary>
    /// Matches basic info fields against the first and last names of a search, returning a match key representing the value and precedence of the first match, or <c>null</c>.
    /// </summary>
    /// 
    /// <remarks>
    /// The match key is leading 'z's equal to the precedence of both matches, followed by the matched fields (first then last), separated by a '1' to sort short first names above longer first names.
    /// This key, when used to sort aplhabetically, will sort matched accounts by the precedence of the matched field and alphabetically within precedence level.
    /// The precedence of a match is determined by the following, in order:
    /// <list type="number">
    /// <item><description>How the search matches the field</description>
    ///     <list type="number">
    ///         <item><description>Equals</description></item>
    ///         <item><description>Starts With</description></item>
    ///         <item><description>Contains</description></item>
    ///     </list>
    /// </item>
    /// <item><description>Which field the search matches</description>
    ///     <list type="number">
    ///         <item><description>FirstName</description></item>
    ///         <item><description>NickName</description></item>
    ///         <item><description>LastName</description></item>
    ///         <item><description>MaidenName</description></item>
    ///         <item><description>UserName</description></item>
    ///     </list>
    /// </item>
    /// </list>
    /// 
    /// </remarks>
    /// 
    /// <param name="firstnameSearch">The first name of the search input to match against</param>
    /// <param name="lastnameSearch">The last name of the search input to match against</param>
    /// <returns>The match key if first and last name both matched a field, or <c>null</c></returns>
    public string? MatchSearch(string firstnameSearch, string lastnameSearch)
    {
        (string, int)? firstname = this switch
        {
            _ when FirstNameEquals(firstnameSearch) => (FirstName, 0),
            _ when NicknameEquals(firstnameSearch) => (Nickname, 1),
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
            _ when LastNameNoSpecialCharacterEquals(lastnameSearch) => (LastName, 15),
            _ when LastNameEquals(lastnameSearch) => (LastName, 2),
            _ when MaidenNameEquals(lastnameSearch) => (MaidenName, 3),
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
}

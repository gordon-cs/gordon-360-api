using Gordon360.Extensions.System;
using Gordon360.Models.Salesforce;
using System;
using System.Linq;

namespace Gordon360.Models.ViewModels;

public record BasicInfoViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Nickname { get; set; }
    public string MaidenName { get; set; }

    private string GetFirstNameFromUsername()
    {
        return UserName?.Split('.')?[0] ?? "";
    }

    private string GetLastNameFromUsername()
    {
        return UserName?.Contains('.') == true ? UserName.Split('.')[1] : "";
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
        (string, int)? match;
        if (FirstName.EqualsIgnoreCase(search)) { match = (FirstName, 0); }
        else if (Nickname.EqualsIgnoreCase(search)) { match = (Nickname, 1); }
        else if (LastName.EqualsIgnoreCase(search)) { match = (LastName, 2); }
        else if (MaidenName.EqualsIgnoreCase(search)) { match = (MaidenName, 3); }
        else if (FirstName.StartsWithIgnoreCase(search)) { match = (FirstName, 4); }
        else if (Nickname.StartsWithIgnoreCase(search)) { match = (Nickname, 5); }
        else if (LastName.StartsWithIgnoreCase(search)) { match = (LastName, 6); }
        else if (MaidenName.StartsWithIgnoreCase(search)) { match = (MaidenName, 7); }
        else if (GetFirstNameFromUsername().StartsWithIgnoreCase(search)) { match = (GetFirstNameFromUsername(), 8); }
        else if (GetLastNameFromUsername().StartsWithIgnoreCase(search)) { match = (GetLastNameFromUsername(), 9); }
        else if (FirstName.ContainsIgnoreCase(search)) { match = (FirstName, 10); }
        else if (Nickname.ContainsIgnoreCase(search)) { match = (Nickname, 11); }
        else if (LastName.ContainsIgnoreCase(search)) { match = (LastName, 12); }
        else if (MaidenName.ContainsIgnoreCase(search)) { match = (MaidenName, 13); }
        else if (UserName.ContainsIgnoreCase(search)) { match = (UserName, 14); }
        else { match = null; }

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
        (string, int)? firstname = null;
        if (FirstName.EqualsIgnoreCase(firstnameSearch)) { firstname = (FirstName, 0); }
        else if (Nickname.EqualsIgnoreCase(firstnameSearch)) { firstname = (Nickname, 1); }
        else if (FirstName.StartsWithIgnoreCase(firstnameSearch)) { firstname = (FirstName, 4); }
        else if (Nickname.StartsWithIgnoreCase(firstnameSearch)) { firstname = (Nickname, 5); }
        else if (GetFirstNameFromUsername().StartsWithIgnoreCase(firstnameSearch)) { firstname = (GetFirstNameFromUsername(), 8); }
        else if (FirstName.ContainsIgnoreCase(firstnameSearch)) { firstname = (FirstName, 10); }
        else if (Nickname.ContainsIgnoreCase(firstnameSearch)) { firstname = (Nickname, 11); }

        if (firstname is not (string firstnameMatch, int firstnamePrecedence)) return null;

        (string, int)? lastname = null;
        if (LastName.EqualsIgnoreCase(lastnameSearch)) { lastname = (LastName, 2); }
        else if (MaidenName.EqualsIgnoreCase(lastnameSearch)) { lastname = (MaidenName, 3); }
        else if (LastName.StartsWithIgnoreCase(lastnameSearch)) { lastname = (LastName, 6); }
        else if (MaidenName.StartsWithIgnoreCase(lastnameSearch)) { lastname = (MaidenName, 7); }
        else if (GetLastNameFromUsername().StartsWithIgnoreCase(lastnameSearch)) { lastname = (GetLastNameFromUsername(), 9); }
        else if (LastName.ContainsIgnoreCase(lastnameSearch)) { lastname = (LastName, 12); }
        else if (MaidenName.ContainsIgnoreCase(lastnameSearch)) { lastname = (MaidenName, 13); }

        if (lastname is not (string lastnameMatch, int lastnamePrecedence)) return null;

        var totalPrecedence = firstnamePrecedence + lastnamePrecedence;
        var keyBase = $"{firstnameMatch}1${lastnameMatch}";

        return string.Concat(Enumerable.Repeat("z", totalPrecedence)) + keyBase;
    }

    public static BasicInfoViewModel FromAccount(Account account)
    {
        return new BasicInfoViewModel
        {
            FirstName = account.FirstName,
            LastName = account.LastName,
            Nickname = account.Preferred_First_Name_Formula__pc,
            UserName = account.AD_Username__pc,
            MaidenName = account.FormerLastName__pc
        };
    }
}

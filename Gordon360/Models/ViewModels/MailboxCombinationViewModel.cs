using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels;

public sealed record MailboxCombinationViewModel(string Combination)
{
    public static MailboxCombinationViewModel? From(string combination) 
        => string.IsNullOrEmpty(combination) ? null : new MailboxCombinationViewModel(combination);
}
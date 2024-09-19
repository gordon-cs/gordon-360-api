namespace Gordon360.Models.ViewModels;

public sealed record MobilePhoneUpdateViewModel(string PersonalPhone, bool MakePrivate, bool SMSOptedIn = false);

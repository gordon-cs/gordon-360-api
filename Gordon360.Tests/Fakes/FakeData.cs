using Bogus;
using Gordon360.Models.ViewModels;
using System;
using System.Collections.Generic;

namespace Gordon360.Tests.Fakes
{
    public static class FakeData
    {
        public static Faker<EventViewModel> EventViewModelFaker => new Faker<EventViewModel>()
            .RuleFor(e => e.Event_ID, f => f.Random.Guid().ToString())
            .RuleFor(e => e.Event_Name, f => f.Lorem.Sentence(3))
            .RuleFor(e => e.Event_Title, f => f.Lorem.Sentence(2))
            .RuleFor(e => e.Event_Type_Name, f => f.Commerce.Department())
            .RuleFor(e => e.HasCLAWCredit, f => f.Random.Bool())
            .RuleFor(e => e.IsPublic, f => f.Random.Bool())
            .RuleFor(e => e.Description, f => f.Lorem.Paragraph())
            .RuleFor(e => e.StartDate, f => f.Date.Future().ToString("s"))
            .RuleFor(e => e.EndDate, f => f.Date.Future().ToString("s"))
            .RuleFor(e => e.Location, f => f.Address.City())
            .RuleFor(e => e.Organization, f => f.Company.CompanyName());

        public static Faker<AttendedEventViewModel> AttendedEventViewModelFaker => new Faker<AttendedEventViewModel>()
            .RuleFor(a => a.LiveID, f => f.Random.Guid().ToString())
            .RuleFor(a => a.CHDate, f => f.Date.Past())
            .RuleFor(a => a.CHTermCD, f => f.Random.String2(6, "0123456789"))
            .RuleFor(a => a.Required, f => f.Random.Int(0, 1))
            .RuleFor(a => a.Event_Name, f => f.Lorem.Sentence(3))
            .RuleFor(a => a.Event_Title, f => f.Lorem.Sentence(2))
            .RuleFor(a => a.Description, f => f.Lorem.Paragraph())
            .RuleFor(a => a.Organization, f => f.Company.CompanyName())
            .RuleFor(a => a.StartDate, f => f.Date.Past().ToString("s"))
            .RuleFor(a => a.EndDate, f => f.Date.Future().ToString("s"))
            .RuleFor(a => a.Location, f => f.Address.City());

        public static Faker<EmailViewModel> EmailViewModelFaker => new Faker<EmailViewModel>()
            .RuleFor(e => e.FirstName, f => f.Name.FirstName())
            .RuleFor(e => e.LastName, f => f.Name.LastName())
            .RuleFor(e => e.Email, (f, e) => f.Internet.Email(e.FirstName, e.LastName))
            .RuleFor(e => e.Description, f => f.Lorem.Word());

        // Helper function for a list of events
        public static List<EventViewModel> CreateEventList(int count = 5) => EventViewModelFaker.Generate(count);
        public static List<AttendedEventViewModel> CreateAttendedEventList(int count = 5) => AttendedEventViewModelFaker.Generate(count);
        public static List<EmailViewModel> CreateEmailList(int count = 5) => EmailViewModelFaker.Generate(count);
    }
} 
using Gordon360.Enums;
using Gordon360.Exceptions;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Static.Methods;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using static Gordon360.Services.MembershipService;

namespace Gordon360.Services;

/// <summary>
/// Service class to facilitate getting emails for members of an activity.
/// </summary>
public class EmailService : IEmailService
{
    private readonly CCTContext _context;
    private readonly IMembershipService _membershipService;
    private readonly IConfiguration _config;

    public EmailService(CCTContext context, IMembershipService membershipService, IConfiguration config)
    {
        _context = context;
        _membershipService = membershipService;
        _config = config;
    }

    /// <summary>
    /// Get a list of the emails for all members in the activity during the current session.
    /// </summary>
    /// <param name="activityCode">The code of the activity to get emails for.</param>
    /// <param name="sessionCode">Optionally, the session to get emails for. Defaults to the current session</param>
    /// <param name="participationTypes">The participation types to get emails of. If unspecified, gets emails of all participation types.</param>
    /// <returns>A list of emails (along with first and last name) associated with that activity</returns>
    public IEnumerable<EmailViewModel> GetEmailsForActivity(string activityCode, string? sessionCode = null, List<string>? participationTypes = null)
    {
        sessionCode ??= Helpers.GetCurrentSession(_context);

        var memberships = _membershipService.GetMemberships(
            activityCode: activityCode,
            sessionCode: sessionCode,
            participationTypes: participationTypes);

        return memberships.Join(_context.ACCOUNT, m => m.Username, a => a.AD_Username, (m, a) => new EmailViewModel
        {
            Email = a.email,
            FirstName = a.firstname,
            LastName = a.lastname,
            Description = m.Description
        });
    }

    /// <summary>
    /// Send a email to a list of email addresses
    /// </summary>
    /// <param name="to_emails">All addresses to send this email to</param>
    /// <param name="from_email">The address this email is sent from</param>
    /// <param name="subject">Subject of the email to be sent</param>
    /// <param name="email_content">The content of the email to be sent</param>
    /// <param name="password">Password of the email sender</param>
    /// <returns></returns>
    public void SendEmails(string[] to_emails, string from_email, string subject, string email_content, string password)
    {
        using var smtp = new SmtpClient();
        var credential = new NetworkCredential
        {
            UserName = from_email,
            Password = password
        };
        smtp.Credentials = credential;
        smtp.Host = _config["SmtpHost"];
        smtp.Port = 587;
        smtp.EnableSsl = true;

        var message = new MailMessage
        {
            From = new MailAddress(from_email),
            Subject = subject,
            Body = email_content,
            IsBodyHtml = true
        };
        message.Bcc.Add(new MailAddress(from_email));
        foreach (string to_email in to_emails)
        {
            message.To.Add(new MailAddress(to_email));
        }

        smtp.Send(message);
    }

    /// <summary>
    /// Send a email to members of an activity
    /// </summary>
    /// <param name="activityCode">The activity code to send this email to</param>
    /// <param name="sessionCode">The session of activity to select members from</param>
    /// <param name="from_email">The address this email is sent from</param>
    /// <param name="subject">Subject of the email to be sent</param>
    /// <param name="email_content">The content of the email to be sent</param>
    /// <param name="password">Password of the email sender</param>
    /// <returns></returns>
    public void SendEmailToActivity(string activityCode, string sessionCode, string from_email, string subject, string email_content, string password)
    {
        var credential = new NetworkCredential
        {
            UserName = from_email,
            Password = password
        };

        using var smtp = new SmtpClient
        {
            Credentials = credential,
            Host = _config["SmtpHost"],
            Port = 587,
            EnableSsl = true,
        };

        var message = new MailMessage
        {
            From = new MailAddress(from_email),
            Subject = subject,
            Body = email_content,
            IsBodyHtml = true
        };
        message.To.Add(new MailAddress(from_email));

        var to_emails = GetEmailsForActivity(activityCode, sessionCode).Select(x => x.Email);
        foreach (string to_email in to_emails)
        {
            message.Bcc.Add(new MailAddress(to_email));
        }

        smtp.Send(message);
    }
}
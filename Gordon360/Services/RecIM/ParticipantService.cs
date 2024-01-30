using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Static.Names;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services.RecIM;

public class ParticipantService : IParticipantService
{
    private readonly CCTContext _context;
    private readonly IAccountService _accountService;

    public ParticipantService(CCTContext context, IAccountService accountService)
    {
        _accountService = accountService;
        _context = context;
    }

    public IEnumerable<LookupViewModel>? GetParticipantLookup(string type)
    {
        return type switch
        {
            "status" => _context.ParticipantStatus.Where(query => query.ID != 0)
                        .Select(s => new LookupViewModel
                        {
                            ID = s.ID,
                            Description = s.Description
                        })
                        .AsEnumerable(),
            "activitypriv" => _context.ParticipantStatus.Where(query => query.ID != 0)
                        .Select(s => new LookupViewModel
                        {
                            ID = s.ID,
                            Description = s.Description
                        })
                        .AsEnumerable(),
            _ => null
        };
    }

    /// <summary>
    /// Fetches the account record with the specified username. Inclusive of non-gordon accounts made by Rec-IM
    /// </summary>
    /// <param name="username">The AD username associated with the account.</param>
    /// <returns>account information</returns>
    public AccountViewModel GetUnaffiliatedAccountByUsername(string username)
    {
        var account = _context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);
        if (account == null)
        {
            return new AccountViewModel()
            {
                ADUserName = username,
                AccountType = "Unaffiliated"
            };
        }
        return account;
    }

    public ParticipantExtendedViewModel? GetParticipantByUsername(string username, string? roleType = null)
    {
        ParticipantExtendedViewModel? participant = _context.ParticipantView.FirstOrDefault(pv => pv.Username == username);
        if (participant is null) return null;

        participant.Role = roleType;
        participant.Status = _context.ParticipantStatusHistory
            .Where(psh => psh.ParticipantUsername == username)
            .OrderByDescending(psh => psh.ID)
            .Select(psh => psh.Status.Description)
            .FirstOrDefault();

        return participant;
    }

    public async Task<ParticipantNotificationViewModel> SendParticipantNotificationAsync(string username, ParticipantNotificationUploadViewModel notificationVM)
    {
        var newNotification = new ParticipantNotification
        {
            ParticipantUsername = username,
            Message = notificationVM.Message,
            EndDate = notificationVM.EndDate,
            DispatchDate = DateTime.UtcNow
        };
        await _context.ParticipantNotification.AddAsync(newNotification);
        await _context.SaveChangesAsync();
        return newNotification;
    }

    public IEnumerable<ParticipantStatusExtendedViewModel> GetParticipantStatusHistory(string username)
    {
        var status = _context.ParticipantStatusHistory
                        .Where(psh => psh.ParticipantUsername == username)
                        .OrderByDescending(psh => psh.ID)
                            .Join(_context.ParticipantStatus,
                                psh => psh.StatusID,
                                ps => ps.ID,
                                (psh, ps) => new ParticipantStatusExtendedViewModel
                                {
                                    Username = username,
                                    Status = ps.Description,
                                    StartDate = psh.StartDate.SpecifyUtc(),
                                    EndDate = psh.EndDate.SpecifyUtc()
                                }).AsEnumerable();
        return status;
    }

    public IEnumerable<TeamExtendedViewModel> GetParticipantTeams(string username)
    {
        var teams = _context.ParticipantTeam
                 .Where(pt => pt.ParticipantUsername == username && pt.RoleTypeID != 0 && pt.RoleTypeID != 2)
                     .Join(_context.Team.Where(t => t.StatusID != 0),
                         pt => pt.TeamID,
                         t => t.ID,
                         (pt, t) => new TeamExtendedViewModel
                         {
                             ID = t.ID,
                             Activity = _context.Activity.FirstOrDefault(a => a.ID == t.ActivityID),
                             Name = t.Name,
                             Status = _context.TeamStatus
                                         .FirstOrDefault(ts => ts.ID == t.StatusID)
                                         .Description,
                             Logo = t.Logo,
                             TeamRecord = _context.SeriesTeam
                                 .Include(st => st.Team)
                                 .Where(st => st.TeamID == t.ID)
                                 .Select(st => (TeamRecordViewModel)st)

                         });
        return teams;
    }

    public IEnumerable<ParticipantExtendedViewModel> GetParticipants()
    {
        // This is Participant left join CustomParticipant left join Student
        //  left join FacStaff to get each participant's firstname and lastname
        var participants = from new_ps in (from p in ((from p in _context.Participant
                                                       join cp in _context.CustomParticipant on p.Username equals cp.Username into cpp_join
                                                       from cpp in cpp_join.DefaultIfEmpty()
                                                       select new
                                                       {
                                                           Username = p.Username,
                                                           IsAdmin = p.IsAdmin,
                                                           AllowEmails = p.AllowEmails,
                                                           Email = cpp.Email,
                                                           SpecifiedGender = p.SpecifiedGender,
                                                           IsCustom = p.IsCustom,
                                                           FirstName = cpp.FirstName,
                                                           LastName = cpp.LastName,
                                                       }))
                                           join s in _context.Student on p.Username equals s.AD_Username into ps_join
                                           from ps in ps_join.DefaultIfEmpty()
                                           select new
                                           {
                                               Username = p.Username,
                                               IsAdmin = p.IsAdmin,
                                               AllowEmails = p.AllowEmails,
                                               Email = p.Email,
                                               SpecifiedGender = p.SpecifiedGender,
                                               IsCustom = p.IsCustom,
                                               FirstName = p.FirstName ?? ps.FirstName,
                                               LastName = p.LastName ?? ps.LastName,
                                           })
                           join fs in _context.FacStaff on new_ps.Username equals fs.AD_Username into psfs_join
                           from psfs in psfs_join.DefaultIfEmpty()
                           select new ParticipantExtendedViewModel
                           {
                               Username = new_ps.Username,
                               IsAdmin = new_ps.IsAdmin,
                               Status = (from psh in _context.ParticipantStatusHistory
                                         join pstatus in _context.ParticipantStatus on psh.StatusID equals pstatus.ID
                                         where psh.ParticipantUsername == new_ps.Username
                                         orderby psh.ID descending
                                         select new
                                         {
                                             Description = pstatus.Description
                                         }
                                        ).FirstOrDefault().Description,
                               AllowEmails = new_ps.AllowEmails,
                               Email = new_ps.Email,
                               SpecifiedGender = new_ps.SpecifiedGender,
                               IsCustom = new_ps.IsCustom,
                               FirstName = new_ps.FirstName ?? psfs.FirstName,
                               LastName = new_ps.LastName ?? psfs.LastName,
                           };

        return participants.OrderBy(p => p.Username);
    }

    public IEnumerable<BasicInfoViewModel> GetAllCustomParticipantsBasicInfo()
    {
        var customParticipants = from p in _context.Participant
                                 join cp in _context.CustomParticipant on p.Username equals cp.Username
                                 where p.IsCustom == true
                                 select new BasicInfoViewModel
                                 {
                                     FirstName = cp.FirstName,
                                     LastName = cp.LastName,
                                     UserName = p.Username,
                                     Nickname = null,
                                     MaidenName = null,
                                 };
        return customParticipants;
    }

    public bool GetParticipantIsCustom(string username)
    {
        var isCustom = (from p in _context.Participant
                        where p.Username == username
                        select p.IsCustom).Single();
        return isCustom;
    }

    public async Task<ParticipantExtendedViewModel> PostParticipantAsync(string username, int? statusID)
    {
        // Find gender
        string user_gender =
            _context.Student.FirstOrDefault(s => s.AD_Username == username)?.Gender ??
            _context.FacStaff.FirstOrDefault(fs => fs.AD_Username == username)?.Gender ??
            "U";

        await _context.Participant.AddAsync(new Participant
        {
            Username = username,
            SpecifiedGender = user_gender,
        });
        await _context.ParticipantStatusHistory.AddAsync(new ParticipantStatusHistory
        {
            ParticipantUsername = username,
            StatusID = statusID ?? 4, //default to cleared
            StartDate = DateTime.UtcNow,
            //No defined end date for creation
        });
        await _context.SaveChangesAsync();
        var participant = GetParticipantByUsername(username);
        return participant;
    }

    public async Task<ParticipantExtendedViewModel> PostCustomParticipantAsync(string username, CustomParticipantViewModel newCustomParticipant)
    {
        var newUsername = GetCustomUnqiueUsername(username);

        await _context.Participant.AddAsync(new Participant
        {
            Username = newUsername,
            SpecifiedGender = newCustomParticipant.SpecifiedGender,
            IsCustom = true,
            AllowEmails = newCustomParticipant.AllowEmails,

        });
        await _context.CustomParticipant.AddAsync(new CustomParticipant
        {
            Username = newUsername,
            Email = newCustomParticipant.Email,
            FirstName = newCustomParticipant.FirstName,
            LastName = newCustomParticipant.LastName,
        });
        await _context.ParticipantStatusHistory.AddAsync(new ParticipantStatusHistory
        {
            ParticipantUsername = newUsername,
            StatusID = 4, //default to cleared
            StartDate = DateTime.UtcNow,
            //No defined end date for creation
        });
        await _context.SaveChangesAsync();
        var participant = GetParticipantByUsername(newUsername);
        return participant;
    }

    public async Task<ParticipantExtendedViewModel> SetParticipantAdminStatusAsync(string username, bool isAdmin)
    {
        var participant = _context.Participant.Find(username);
        participant.IsAdmin = isAdmin;
        await _context.SaveChangesAsync();
        return new ParticipantExtendedViewModel
        {
            Username = participant.Username,
            IsAdmin = participant.IsAdmin,
            IsCustom = participant.IsCustom,
            AllowEmails = participant.AllowEmails,
            SpecifiedGender = participant.SpecifiedGender
        };
    }

    public async Task<ParticipantExtendedViewModel> UpdateCustomParticipantAsync(string username, CustomParticipantPatchViewModel updatedParticipant)
    {
        var participant = _context.Participant.First((p) => p.Username == username && p.IsCustom == true);
        var customParticipant = _context.CustomParticipant.First((p) => p.Username == username);

        customParticipant.FirstName = updatedParticipant.FirstName ?? customParticipant.FirstName;
        customParticipant.LastName = updatedParticipant.LastName ?? customParticipant.LastName;
        participant.SpecifiedGender = updatedParticipant.SpecifiedGender ?? participant.SpecifiedGender;
        participant.AllowEmails = updatedParticipant.AllowEmails ?? participant.AllowEmails;
        customParticipant.Email = updatedParticipant.Email ?? customParticipant.Email;

        await _context.SaveChangesAsync();
        return GetParticipantByUsername(username);
    }

    public async Task<ParticipantExtendedViewModel> UpdateParticipantAllowEmailsAsync(string username, bool allowEmails)
    {
        var participant = _context.Participant.Find(username);
        participant.AllowEmails = allowEmails;
        await _context.SaveChangesAsync();
        return new ParticipantExtendedViewModel
        {
            Username = participant.Username,
            IsAdmin = participant.IsAdmin,
            IsCustom = participant.IsCustom,
            AllowEmails = participant.AllowEmails,
            SpecifiedGender = participant.SpecifiedGender
        };
    }


    public async Task<ParticipantActivityViewModel> UpdateParticipantActivityAsync(string username, ParticipantActivityPatchViewModel updatedParticipant)
    {
        var participantActivity = _context.ParticipantActivity
                                    .FirstOrDefault(pa => pa.ParticipantUsername == username
                                        && pa.ActivityID == updatedParticipant.ActivityID);

        participantActivity.PrivTypeID = updatedParticipant.ActivityPrivID ?? participantActivity.PrivTypeID;
        participantActivity.IsFreeAgent = updatedParticipant.IsFreeAgent ?? participantActivity.IsFreeAgent;


        await _context.SaveChangesAsync();
        return participantActivity;
    }

    public async Task<ParticipantStatusHistoryViewModel> UpdateParticipantStatusAsync(string username, ParticipantStatusPatchViewModel participantStatus)
    {
        // End previous status
        var prevStatus = _context.ParticipantStatusHistory
                           .Where(psh => psh.ParticipantUsername == username)
                           .OrderByDescending(psh => psh.ID)
                           .FirstOrDefault();
        prevStatus.EndDate = DateTime.UtcNow;

        var status = new ParticipantStatusHistory
        {
            ParticipantUsername = username,
            StatusID = participantStatus.StatusID,
            StartDate = DateTime.UtcNow,
            EndDate = participantStatus.EndDate
        };
        await _context.ParticipantStatusHistory.AddAsync(status);
        await _context.SaveChangesAsync();
        return status;
    }

    public bool IsParticipant(string username)
    {
        if (_context.Participant.FirstOrDefault(p => p.Username == username) is null)
        {
            return false;
        }
        var isPending = _context.ParticipantStatusHistory
                .Where(psh => psh.ParticipantUsername == username)
                .OrderByDescending(psh => psh.ID)
                .FirstOrDefault()
                .StatusID == 1;

        // 1 is pending
        return !isPending;
    }

    public bool IsAdmin(string username)
    {
        return _context.Participant.Any(p => p.Username == username && p.IsAdmin == true);
    }

    private string GetCustomUnqiueUsername(string username)
    {
        var customSuffix = RecIM_Resources.CUSTOM_PARTICIPANT_USERNAME_SUFFIX;
        var newUsername = username;
        var index = 2;
        while (_context.Participant.Any((p) => p.Username == newUsername + customSuffix && p.IsCustom == true))
        {
            newUsername = username + index.ToString();
            index++;
        }
        return newUsername + customSuffix;
    }
}


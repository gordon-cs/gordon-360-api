using Gordon360.Authorization;
using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Static.Names;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Extensions.System;
using Gordon360.Enums;
using System;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Services;


/// <summary>
/// Service Class that facilitates data transactions between the PosterController and the Poster database model.
/// </summary>
public class PosterService(CCTContext context, SessionService sessionService, MembershipService membershipService) : IPosterService
{
    public IEnumerable<PosterViewModel> GetPosters() {
        return context.Poster.Include(p => p.Status).Select(p => (PosterViewModel)p);
    }

    public IEnumerable<PosterViewModel> GetCurrentPosters()
    {
        return GetPosters().Where(p => p.ExpirationDate > DateTime.Now && p.Status == "Visible");
    }

    //currently will only get posters if someone is signed up for a club, can be modified to include all posters but prioritize 
    //personalized posters
    public IEnumerable<PosterViewModel> GetPersonalizedPostersByUsername(string username)
    {
        var currentSessionCode = sessionService.GetCurrentSession().SessionCode;
        var currentMembershipCodes = membershipService.GetMemberships(username: username, sessionCode: currentSessionCode)
            .Select(m => m.ActivityCode);

        var res = GetCurrentPosters().Where(p => currentMembershipCodes.Contains(p.ClubCode));
   
        return res;
    }

    public IEnumerable<string> GetPosterStatuses()
    {
        return context.PosterStatus.Select(ps => ps.Status).AsEnumerable();
    }

    public IEnumerable<PosterViewModel> GetPostersByActivityCode(string activityCode)
    {
        return GetPosters().Where(p => p.ClubCode == activityCode);
    }
    public IEnumerable<PosterViewModel> GetPosterByID(int posterID)
    {
        return GetPosters().Where(p => p.ID == posterID);
    }
    public async Task<PosterViewModel> PostPosterAsync(PosterUploadViewModel newPoster)
    {
        return null;
    }

    public async Task<PosterViewModel> UpdatePosterAsync(int posterID, PosterPatchViewModel updatedPoster)
    {
        return null;
    }

    public async Task<PosterViewModel> DeletePosterAsync(int posterID)
    {
        return null;
    }

}

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

namespace Gordon360.Services;


/// <summary>
/// Service Class that facilitates data transactions between the PosterController and the Poster database model.
/// </summary>
public class PosterService(CCTContext context) : IPosterService
{
    public IEnumerable<PosterViewModel> GetPosters() {
        return null;
    }
    public IEnumerable<PosterViewModel> GetCurrentPosters()
    {
        return null;
    }
    public IEnumerable<PosterViewModel> GetPostersByUsername(string username)
    {
        return null;
    }
    public IEnumerable<PosterViewModel> GetPostersByActivityCode(string activityCode)
    {
        return null;
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

using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels.RecIM;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Exceptions;


namespace Gordon360.Services.RecIM;

public class AffiliationService(CCTContext context) : IAffiliationService
{
    public async Task<string> AddPointsToAffilliationAsync(string affiliationName, AffiliationPointsUploadViewModel vm)
    {
        var affiliation = context.Affiliation.Find(affiliationName);
        if (affiliation is null) throw new ResourceNotFoundException();

        var exist = context.AffiliationPoints.FirstOrDefault(ap => ap.SeriesID == vm.SeriesID && ap.TeamID == vm.TeamID);

        if (exist is not null) 
            exist.Points = vm.Points ?? 0;
        else
            context.AffiliationPoints.Add(new AffiliationPoints
            {
                AffiliationName = affiliationName,
                TeamID = vm.TeamID,
                SeriesID = vm.SeriesID,
                Points = vm.Points ?? 0,
            });

        await context.SaveChangesAsync();

        return affiliationName;
    }

    public async Task DeleteAffiliationAsync(string affiliationName)
    {
        var teams = context.Team
            .Where(t => t.Affiliation == affiliationName);
        await teams.ForEachAsync(t => t.Affiliation = null);

        var affiliationPoints = context.AffiliationPoints
            .Where(ap => ap.AffiliationName == affiliationName);
        context.AffiliationPoints.RemoveRange(affiliationPoints);

        //required to remove fk constraints before proper removal
        await context.SaveChangesAsync();

        var affiliation = context.Affiliation.Find(affiliationName);
        if (affiliation is Affiliation a) context.Remove(a);
        await context.SaveChangesAsync();

    }

    public IEnumerable<AffiliationExtendedViewModel> GetAllAffiliationDetails()
    {
        return context.Affiliation
             .Select(a => new AffiliationExtendedViewModel
             {
                 Name = a.Name,
                 Points = context.AffiliationPoints
                     .Where(ap => ap.AffiliationName == a.Name)
                     .Select(ap => ap.Points)
                     .AsEnumerable()
                     .Sum(),
                 Series = context.AffiliationPoints
                      .Where(_ap => _ap.AffiliationName == a.Name)
                      .Select(ap => (SeriesViewModel) ap.Series)
                      .AsEnumerable()
             })
             .AsEnumerable();
    }

    public AffiliationExtendedViewModel GetAffiliationDetailsByName(string name)
    {
        var affiliation = context.Affiliation.Find(name);
        if (affiliation is null) throw new ResourceNotFoundException();

        return new AffiliationExtendedViewModel()
        {
            Name = name,
            Points = context.AffiliationPoints
                     .Where(ap => ap.AffiliationName == name)
                     .Select(ap => ap.Points)
                     .AsEnumerable()
                     .Sum(),
            Series = context.AffiliationPoints
                      .Where(_ap => _ap.AffiliationName == name)
                      .Select(ap => (SeriesViewModel) ap.Series)
                      .AsEnumerable()
        };
    }

    public async Task<string> CreateAffiliation(string affiliationName)
    {
        var existing = context.Affiliation.Find(affiliationName);

        if (existing is not null) throw new BadInputException() { ExceptionMessage = "Affiliation already exists" };

        context.Affiliation.Add(new Affiliation { Name = affiliationName });
        await context.SaveChangesAsync();

        return affiliationName;
    }

    public async Task<string> UpdateAffiliationAsync(string affiliationName, AffiliationPatchViewModel update)
    {
        var affiliation = context.Affiliation.Find(affiliationName);
        if (affiliation is null) throw new ResourceNotFoundException();
        affiliation.Name = update.Name ?? affiliation.Name;
        affiliation.Logo = update.Logo ?? affiliation.Logo;

        await context.SaveChangesAsync();
        return affiliation.Name;
    }
}

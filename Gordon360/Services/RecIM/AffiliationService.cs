﻿using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels.RecIM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Exceptions;


namespace Gordon360.Services.RecIM;

public class AffiliationService : IAffiliationService
{

    private readonly CCTContext _context;

    public AffiliationService(CCTContext context)
    {
        _context = context;
    }

    public async Task<string> AddPointsToAffilliationAsync(string affiliationName, AffiliationPointsUploadViewModel vm)
    {
        var affiliation = _context.Affiliation.Find(affiliationName);
        if (affiliation is null) throw new ResourceNotFoundException();

        var exist = _context.AffiliationPoints.FirstOrDefault(ap => ap.SeriesID == vm.SeriesID && ap.TeamID == vm.TeamID);

        if (exist is not null) 
            exist.Points = vm.Points ?? 0;
        else
            _context.AffiliationPoints.Add(new AffiliationPoints
            {
                AffiliationName = affiliationName,
                TeamID = vm.TeamID,
                SeriesID = vm.SeriesID,
                Points = vm.Points ?? 0,
            });

        await _context.SaveChangesAsync();

        return affiliationName;
    }

    public async Task DeleteAffiliationAsync(string affiliationName)
    {
        var teams = _context.Team
            .Where(t => t.Affiliation == affiliationName);
        await teams.ForEachAsync(t => t.Affiliation = null);

        var affiliationPoints = _context.AffiliationPoints
            .Where(ap => ap.AffiliationName == affiliationName);
        _context.AffiliationPoints.RemoveRange(affiliationPoints);

        //required to remove fk constraints before proper removal
        await _context.SaveChangesAsync();

        var affiliation = _context.Affiliation.Find(affiliationName);
        if (affiliation is Affiliation a) _context.Remove(a);
        await _context.SaveChangesAsync();

    }

    public IEnumerable<AffiliationExtendedViewModel> GetAllAffiliationDetails()
    {
        return _context.Affiliation
             .Select(a => new AffiliationExtendedViewModel
             {
                 Name = a.Name,
                 Points = _context.AffiliationPoints
                     .Where(ap => ap.AffiliationName == a.Name)
                     .Select(ap => ap.Points)
                     .AsEnumerable()
                     .Sum(),
                 Series = _context.AffiliationPoints
                      .Where(_ap => _ap.AffiliationName == a.Name)
                      .Select(ap => (SeriesViewModel) ap.Series)
                      .AsEnumerable()
             })
             .AsEnumerable();
    }

    public AffiliationExtendedViewModel GetAffiliationDetailsByName(string name)
    {
        var affiliation = _context.Affiliation.Find(name);
        if (affiliation is null) throw new ResourceNotFoundException();

        return new AffiliationExtendedViewModel()
        {
            Name = name,
            Points = _context.AffiliationPoints
                     .Where(ap => ap.AffiliationName == name)
                     .Select(ap => ap.Points)
                     .AsEnumerable()
                     .Sum(),
            Series = _context.AffiliationPoints
                      .Where(_ap => _ap.AffiliationName == name)
                      .Select(ap => (SeriesViewModel) ap.Series)
                      .AsEnumerable()
        };
    }

    public async Task<string> CreateAffiliation(string affiliationName)
    {
        var existing = _context.Affiliation.Find(affiliationName);

        if (existing is not null) throw new BadInputException() { ExceptionMessage = "Affiliation already exists" };

        _context.Affiliation.Add(new Affiliation { Name = affiliationName });
        await _context.SaveChangesAsync();

        return affiliationName;
    }

    public async Task<string> UpdateAffiliationAsync(string affiliationName, AffiliationPatchViewModel update)
    {
        var affiliation = _context.Affiliation.Find(affiliationName);
        if (affiliation is null) throw new ResourceNotFoundException();
        affiliation.Name = update.Name ?? affiliation.Name;
        affiliation.Logo = update.Logo ?? affiliation.Logo;

        await _context.SaveChangesAsync();
        return affiliation.Name;
    }
}

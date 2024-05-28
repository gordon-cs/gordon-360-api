using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Models.CCT.Context;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Hosting;

namespace Gordon360.Services.RecIM;

public class SportService(CCTContext context, IWebHostEnvironment webHostEnvironment, ServerUtils serverUtils) : ISportService
{
    public SportViewModel GetSportByID(int sportID)
    {
        return context.Sport.Find(sportID);
    }

    public async Task<SportViewModel> DeleteSportAsync(int sportID)
    {
        var sport = context.Sport.Find(sportID);
        var activities = context.Activity.Where(a => a.SportID == sportID);
        foreach (var activity in activities)
            activity.SportID = 0;
        context.Sport.Remove(sport);
        await context.SaveChangesAsync();
        return sport;
    }

    public IEnumerable<SportViewModel> GetSports()
    {
        return context.Sport.Select(s => (SportViewModel)s).AsEnumerable();
    }

    public async Task<SportViewModel> PostSportAsync(SportUploadViewModel newSport)
    {
        var sport = new Sport
        {
            Name = newSport.Name,
            Description = newSport.Description,
            Rules = newSport.Rules,
            Logo = newSport.Logo
        };

        await context.Sport.AddAsync(sport);
        await context.SaveChangesAsync();
        return sport;
    }

    public async Task<SportViewModel> UpdateSportAsync(int sportID, SportPatchViewModel updatedSport)
    {
        var sport = context.Sport.Find(sportID);
        sport.Name = updatedSport.Name ?? sport.Name;
        sport.Description = updatedSport.Description ?? sport.Description;
        sport.Rules = updatedSport.Rules ?? sport.Rules;

        // note: sport has not been tested
        if (updatedSport.Logo != null)
        {
            // ImageUtils.GetImageFormat checks whether the image type is valid (jpg/jpeg/png)
            var (extension, format, data) = ImageUtils.GetImageFormat(updatedSport.Logo.Image);

            string? imagePath = null;
            // remove old
            if (sport.Logo is not null && updatedSport.Logo.Image is null)
            {
                imagePath = GetImagePath(Path.GetFileName(sport.Logo));
                ImageUtils.DeleteImage(imagePath);
                sport.Logo = updatedSport.Logo.Image;
            }

            if (updatedSport.Logo.Image is not null)
            {
                // Use a unique alphanumeric GUID string as the file name
                var filename = $"{Guid.NewGuid().ToString("N")}.{extension}";
                imagePath = GetImagePath(filename);
                var url = GetImageURL(filename);
                sport.Logo = url;
                ImageUtils.UploadImage(imagePath, data, format);
            }
        }


        await context.SaveChangesAsync();

        return sport;
    }
    private string GetImagePath(string filename)
    {
        return Path.Combine(webHostEnvironment.ContentRootPath, "browseable", "uploads", "recim", "ruleset", filename);
    }

    private string GetImageURL(string filename)
    {
        var serverAddress = serverUtils.GetAddress();
        if (serverAddress is not string) throw new Exception("Could not upload Rec-IM Ruleset Image: Server Address is null");

        if (serverAddress.Contains("localhost"))
            serverAddress += '/';
        var url = $"{serverAddress}browseable/uploads/recim/ruleset/{filename}";
        return url;
    }
}


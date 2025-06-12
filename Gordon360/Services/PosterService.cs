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
using Gordon360.Utilities;
using Microsoft.Graph;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph.TermStore;


namespace Gordon360.Services;


/// <summary>
/// Service Class that facilitates data transactions between the PosterController and the Poster database model.
/// </summary>
public class PosterService(CCTContext context, 
                            ISessionService sessionService, 
                            IMembershipService membershipService, 
                            IWebHostEnvironment webHostEnvironment,
                            ServerUtils serverUtils) : IPosterService
{
    public IEnumerable<PosterViewModel> GetPosters() {
        return context.Poster.Include(p => p.Status).Select(p => (PosterViewModel)p);
    }

    public IEnumerable<PosterViewModel> GetCurrentPosters()
    {
        return GetPosters()
            .Where(p => p.ExpirationDate > DateTime.Now && p.Status == "Visible" && p.VisibleDate < DateTime.Now)
            .OrderByDescending(p => p.Priority)
            .ThenBy(p => p.ExpirationDate);
    }



    public IEnumerable<PosterViewModel> GetCurrentPostersByActivityCode(string activityCode)
    {
        return GetPosters()
            .Where(p => p.ExpirationDate > DateTime.Now 
            && p.Status == "Visible" 
            && p.VisibleDate < DateTime.Now 
            && p.ClubCode == activityCode)
            .OrderByDescending(p => p.Priority)
            .ThenBy(p => p.ExpirationDate);
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

    public PosterViewModel GetPosterByID(int posterID)
    {
        return GetPosters().Where(p => p.ID == posterID).FirstOrDefault();
    }
    public async Task<PosterViewModel> PostPosterAsync(PosterUploadViewModel newPoster)
    {
        var poster = newPoster.ToPoster();

        var (extension, format, data) = ImageUtils.GetImageFormat(newPoster.ImagePath);
        var filename = $"{Guid.NewGuid().ToString("N")}.{extension}";
        var imagePath = GetImagePath(filename);
        var url = GetImageURL(filename);
        poster.ImagePath = url;
        ImageUtils.UploadImage(imagePath, data, format);


        context.Poster.Add(poster);
        await context.SaveChangesAsync();

        return GetPosterByID(poster.ID);
    }

    public async Task<PosterViewModel> UpdatePosterAsync(int posterID, PosterPatchViewModel updatedPoster)


    {
        var poster = context.Poster.Find(posterID);
        if (poster == null) throw new ResourceNotFoundException
        { ExceptionMessage = $"Poster with ID: {posterID} not found" };

        poster.Title = updatedPoster.Title ?? poster.Title;
        poster.Description = updatedPoster.Description ?? poster.Description;
        poster.VisibleDate = updatedPoster.VisibleDate ?? poster.VisibleDate;
        poster.ExpirationDate = updatedPoster.ExpirationDate ?? poster.ExpirationDate;

        if (updatedPoster.Status is not null)
            poster.StatusID = context.PosterStatus
            .Where(ps => ps.Status.ToLower() == updatedPoster.Status.ToLower())
            .FirstOrDefault()?
            .ID ?? poster.StatusID;


        if (updatedPoster.ImagePath is not null && updatedPoster.ImagePath.StartsWith("data:image"))
        {
            var (extension, format, data) = ImageUtils.GetImageFormat(updatedPoster.ImagePath);

            string? imagePath = null;

            if (poster.ImagePath is not null)
            {
                imagePath = GetImagePath(Path.GetFileName(poster.ImagePath));
                ImageUtils.DeleteImage(imagePath);
                poster.ImagePath = updatedPoster.ImagePath;
            }
            else
            {
                var filename = $"{Guid.NewGuid().ToString("N")}.{extension}";
                imagePath = GetImagePath(filename);
                var url = GetImageURL(filename);
                poster.ImagePath = url;
                ImageUtils.UploadImage(imagePath, data, format);
            }
        }


        await context.SaveChangesAsync();

        return poster;
    }

    private string GetImagePath(string filename)
    {
        var directoryPath = Path.Combine(webHostEnvironment.ContentRootPath, "browseable", "uploads", "posters", "images");

        if (!System.IO.Directory.Exists(directoryPath))
        {
            System.IO.Directory.CreateDirectory(directoryPath);
        }

        return Path.Combine(directoryPath, filename);
    }


    private string GetImageURL(string filename)
    {
        var serverAddress = serverUtils.GetAddress();
        if (serverAddress is not string) throw new Exception("Could not upload poster: Server Address is null");

        var url = $"browseable/uploads/posters/images/{filename}";
        return url;
    }


    public async Task<PosterViewModel> DeletePosterAsync(int posterID)
    {
        var poster = await context.Poster
            .Include(p => p.Status)
            .FirstOrDefaultAsync(p => p.ID == posterID);

        if (poster == null)
            throw new ResourceNotFoundException { ExceptionMessage = $"Poster with ID: {posterID} not found" };

        if (!string.IsNullOrEmpty(poster.ImagePath))
        {
            var imagePath = GetImagePath(Path.GetFileName(poster.ImagePath));
            ImageUtils.DeleteImage(imagePath);
        }

        context.Poster.Remove(poster);
        await context.SaveChangesAsync();

        return (PosterViewModel)poster;
    }

    public async Task<PosterViewModel> HidePosterAsync(int posterID)
    {
        var poster = await context.Poster
            .Include(p => p.Status)  // <-- include Status here
            .FirstOrDefaultAsync(p => p.ID == posterID);

        if (poster == null)
            throw new ResourceNotFoundException { ExceptionMessage = $"Poster with ID: {posterID} not found" };

        poster.ExpirationDate = DateTime.Now;

        await context.SaveChangesAsync();

        return (PosterViewModel)poster;
    }




}

using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Models.CCT.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Hosting;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace Gordon360.Services.RecIM
{
    public class SportService : ISportService
    {
        private readonly CCTContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ServerUtils _serverUtils;

        public SportService(CCTContext context, IWebHostEnvironment webHostEnvironment, ServerUtils serverUtils)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _serverUtils = serverUtils;
        }

        public SportViewModel GetSportByID(int sportID)
        {
            return _context.Sport.Find(sportID);
        }

        public IEnumerable<SportViewModel> GetSports()
        {
            return _context.Sport.Select(s => (SportViewModel)s).AsEnumerable();
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
            
            // UNTESTED FEATURE
            if (sport.Logo != null)
            {
                // ImageUtils.GetImageFormat checks whether the image type is valid (jpg/jpeg/png)
                var (extension, format, data) = ImageUtils.GetImageFormat(sport.Logo);

                // Use a unique alphanumeric GUID string as the file name
                var filename = $"{Guid.NewGuid().ToString("N")}.{extension}";
                var imagePath = GetImagePath(filename);
                var url = GetImageURL(filename);

                ImageUtils.UploadImage(imagePath, data, format);

                sport.Logo = url;
            }

            await _context.Sport.AddAsync(sport);
            await _context.SaveChangesAsync();
            return sport;
        }

        public async Task<SportViewModel> UpdateSportAsync(int sportID, SportPatchViewModel updatedSport)
        {
            var sport = _context.Sport.Find(sportID);
            sport.Name = updatedSport.Name ?? sport.Name;
            sport.Description = updatedSport.Description ?? sport.Description;
            sport.Rules = updatedSport.Rules ?? sport.Rules;
            
            // UNTESTED FEATURE
            if (updatedSport.Logo != null && updatedSport.Logo != "NONE")
            {
                // ImageUtils.GetImageFormat checks whether the image type is valid (jpg/jpeg/png)
                var (extension, format, data) = ImageUtils.GetImageFormat(updatedSport.Logo);

                string? imagePath = null;
                // If old image exists, overwrite it with new image at same path
                if (sport.Logo != null)
                {
                    imagePath = GetImagePath(Path.GetFileName(sport.Logo));
                }
                // Otherwise, upload new image and save url to db
                else
                {
                    // Use a unique alphanumeric GUID string as the file name
                    var filename = $"{Guid.NewGuid().ToString("N")}.{extension}";
                    imagePath = GetImagePath(filename);
                    var url = GetImageURL(filename);
                    sport.Logo = url;
                }

                ImageUtils.UploadImage(imagePath, data, format);
            }

            //If the image property is null, it means that either the user
            //chose to remove the previous image or that there was no previous
            //image (DeleteImage is designed to handle this).
            else if (sport.Logo != null)
            {
                var imagePath = GetImagePath(Path.GetFileName(sport.Logo));

                ImageUtils.DeleteImage(imagePath);
                sport.Logo = updatedSport.Logo; //null
            }

            await _context.SaveChangesAsync();

            return sport;
        }
        private string GetImagePath(string filename)
        {
            return Path.Combine(_webHostEnvironment.ContentRootPath, "browseable", "uploads", "recim", "ruleset", filename);
        }

        private string GetImageURL(string filename)
        {
            var serverAddress = _serverUtils.GetAddress();
            if (serverAddress is not string) throw new Exception("Could not upload Rec-IM Ruleset Image: Server Address is null");

            if (serverAddress.Contains("localhost"))
                serverAddress += '/';
            var url = $"{serverAddress}browseable/uploads/recim/ruleset/{filename}";
            return url;
        }
    }
}


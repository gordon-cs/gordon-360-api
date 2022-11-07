using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Models.CCT.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Gordon360.Services.RecIM
{
    public class SportService : ISportService
    {
        private readonly CCTContext _context;

        public SportService(CCTContext context)
        {
            _context = context;
        }

        public SportViewModel GetSportByID(int sportID)
        {
            return _context.Sport
                    .FirstOrDefault(s => s.ID == sportID);
        }

        public IEnumerable<SportViewModel> GetSports()
        {
            return _context.Sport.Select(s => (SportViewModel)s).AsEnumerable();
        }

        public async Task<int> PostSport(SportUploadViewModel newSport)
        {
            var sport = new Sport
            {
                Name = newSport.Name,
                Description = newSport.Description,
                Rules = newSport.Rules,
                Logo = newSport.Logo ?? "defaultSportPicture.jpg" //will be a good idea to make default pictures
            };
            await _context.Sport.AddAsync(sport);
            await _context.SaveChangesAsync();
            return sport.ID;
        }

        public async Task UpdateSport(SportViewModel updatedSport)
        {
            var sport = _context.Sport.FirstOrDefault(s => s.ID == updatedSport.ID);
            sport.Name = updatedSport.Name ?? sport.Name;
            sport.Description = updatedSport.Description ?? sport.Description;
            sport.Rules = updatedSport.Rules ?? sport.Rules;
            sport.Logo = updatedSport.Logo ?? sport.Logo;
            await _context.SaveChangesAsync();

        }
    }
}


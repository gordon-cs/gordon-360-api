using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Gordon360.Services.RecIMServices
{
    public class LeagueService : ILeagueService
    {
        private readonly CCTContext _context;

        public LeagueService(CCTContext context)
        {
            _context = context;
        }


        public IEnumerable<League> GetLeagues()
        {
            var leagues = _context.League
                            .Include(l => l.Team)
                            .Include(l => l.Series)
                            .AsEnumerable();
            return leagues;
        }
        public IEnumerable<League> GetLeaguesByTime(DateTime? time)
        {
            var leagues = _context.League
                            .Where(l => time == null 
                                        ? !l.Completed 
                                        : l.RegistrationEnd > time)
                            .Include(l => l.Team)
                            .Include(l => l.Series)
                            .AsEnumerable();
            return leagues;
        }
        public League GetLeagueByID(int leagueID)
        {
            var result = _context.League
                            .Where(l => l.ID == leagueID)
                            .Include(l => l.Team)
                            .Include(l => l.Series)
                                .ThenInclude(s => s.Match)
                            .Include(l => l.UserLeague
                                .Join(_context.User,
                                    ul => ul.UserID, 
                                    u => u.ID, 
                                    (ul,u) => u))
                            .FirstOrDefault();
            return result;
        }
        public async Task UpdateLeague(League updatedLeague)
        {
            int leagueID = updatedLeague.ID;
            var league = _context.League
                            .FirstOrDefault(l => l.ID == updatedLeague.ID);
            league.Name = updatedLeague.Name == null ? league.Name : updatedLeague.Name;
            league.Logo = updatedLeague.Logo == null ? league.Logo : updatedLeague.Logo;
            league.RegistrationStart = updatedLeague.RegistrationStart == default 
                                            ? league.RegistrationStart 
                                            : updatedLeague.RegistrationStart;
            league.RegistrationEnd = updatedLeague.RegistrationEnd == default
                                  ? league.RegistrationEnd
                                  : updatedLeague.RegistrationEnd;
            league.TypeID = updatedLeague.TypeID == default 
                                ? league.TypeID 
                                : updatedLeague.TypeID;
            league.SportID = updatedLeague.SportID == default
                                ? league.SportID 
                                : updatedLeague.SportID;
            league.StatusID = updatedLeague.StatusID == null
                                ? league.StatusID 
                                : updatedLeague.StatusID;
            league.MinCapacity = updatedLeague.MinCapacity == null
                                   ? league.MinCapacity
                                   : updatedLeague.MinCapacity;
            league.MaxCapacity = updatedLeague.MaxCapacity == null
                                   ? league.MaxCapacity
                                   : updatedLeague.MaxCapacity;
            league.MaxCapacity = updatedLeague.MaxCapacity == null
                                   ? league.MaxCapacity
                                   : updatedLeague.MaxCapacity;
            league.SoloRegistration = updatedLeague.SoloRegistration;
            league.Completed = updatedLeague.Completed;

            _context.SaveChanges();
        }
        public async Task PostLeague(League newLeague)
        {
            _context.League.Add(newLeague);
            _context.SaveChanges();
        }
       
    }

 }


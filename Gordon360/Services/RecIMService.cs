using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services
{
    public class RecIMService : IRecIMService
    {
        private readonly CCTContext _context;

        public RecIMService(CCTContext context)
        {
            _context = context;
        }

     
        public IEnumerable<League> GetLeagues()
        {
            var leagues = _context.League.AsEnumerable();
            System.Diagnostics.Debug.WriteLine(leagues);
            return leagues;
        }
        public async Task PostLeague(League newLeague)
        {
            _context.League.Add(newLeague);
            _context.SaveChanges();
        }
        /// <summary>
        /// creates a hardcoded league
        /// </summary>
        /// <param></param>
        public async Task PostSmashLeague()
        {
            var newLeague = new League
                {
                    Name = "Super Smash Bros. Ultimate 1v1",
                    RegistrationStart = DateTime.Now,
                    RegistrationEnd = DateTime.Now,
                    TypeID = 1,
                    StatusID = 1,
                    SportID = 1,
                    MinCapacity = 1,
                    MaxCapacity = null,
                    SoloRegistration = true,
                    Logo = null,
                    Completed = false
                };
            _context.League.Add(newLeague);
            _context.SaveChanges();
        }

        public async Task PostSport(string name, string description, string ruless, string? logo)
        {
            
        }
        public IEnumerable<Sport> GetSports()
        {
            return null;
        }
        public async Task PostLeagueType(string name)
        {
           
        }
        public IEnumerable<LeagueType> GetLeagueTypes()
        {
            return null;
        }
        public async Task PostLeagueStatus(string name)
        {

        }
        public IEnumerable<LeagueStatus> GetLeagueStatuses()
        {
            return null;
        }
    }
}


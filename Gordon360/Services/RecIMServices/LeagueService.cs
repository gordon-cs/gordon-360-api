using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Microsoft.AspNetCore.Mvc;
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


        public IEnumerable<League> GetAllLeagues()
        {
            var leagues = _context.League.AsEnumerable();
            return leagues;
        }
        public League GetLeagueByID(int leagueID)
        {
            return null;
        }
        public HashSet<Series> GetAllLeagueSeries(int leagueID)
        {
            return null;
        }
        public HashSet<Team> GetLeagueTeams(int leagueID)
        {
            return null;
        }
        public IEnumerable<User> GetAllLeagueUsers(int leagueID)
        {
            return null;
        }
        public async Task PostLeague(League newLeague)
        {
            _context.League.Add(newLeague);
            _context.SaveChanges();
        }
        public async Task PostSeries(int leagueID, Series newSeries)
        {

        }
    }

 }


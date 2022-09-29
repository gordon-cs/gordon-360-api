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


        public IEnumerable<League> GetAllLeagues()
        {
            var leagues = _context.League.AsEnumerable();
            return leagues;
        }
        public League GetLeagueByID(int leagueID)
        {
            var result = _context.League
                            .Where(l => l.ID == leagueID)
                            .FirstOrDefault();
            return result;
        }
        public LeagueType GetLeagueType(int leagueID)
        {
            var result = (
                from l in _context.League
                join lt in _context.LeagueType on l.TypeID equals lt.ID
                where (l.ID == leagueID)
                select new LeagueType
                {
                    ID = lt.ID,
                    Description = lt.Description
                }).FirstOrDefault();

            return result;
        }
        public LeagueStatus GetLeagueStatus(int leagueID)
        {
            var result = (
                from l in _context.League
                join ls in _context.LeagueType on l.StatusID equals ls.ID
                where (l.ID == leagueID)
                select new LeagueStatus
                {
                    ID = ls.ID,
                    Description = ls.Description
                }).FirstOrDefault();

            return result;
        }
        public Sport GetLeagueSport(int leagueID)
        {
            var result = (
                from l in _context.League
                join s in _context.Sport on l.SportID equals s.ID
                where (l.ID == leagueID)
                select new Sport
                {
                    ID = s.ID,
                    Name = s.Name,
                    Description = s.Description,
                    Logo = s.Logo,
                    Rules = s.Rules
                }).FirstOrDefault();

            return result;
        }
        public List<Series> GetLeagueSeries(int leagueID)
        {
            var result = _context.Series
                            .Where(q => q.LeagueID == leagueID)
                            .OrderBy(d => d.StartDate)
                            .ToList();
            return result;
        }
        public List<Team> GetLeagueTeams(int leagueID)
        {
            var result = _context.Team
                            .Where(t => t.LeagueID == leagueID)
                            .OrderByDescending(t => t.Wins)
                            .ToList();
            return result;
        }
        public List<User> GetLeagueUsers(int leagueID)
        {
            var result = (
                from u in _context.User
                join ul in _context.UserLeague on u.ID equals ul.UserID
                where (ul.LeagueID == leagueID)
                select new User
                {
                    ID = u.ID
                }).ToList();

            return result;
        }
        public async Task PostLeague(League newLeague)
        {
            _context.League.Add(newLeague);
            _context.SaveChanges();
        }
       
    }

 }


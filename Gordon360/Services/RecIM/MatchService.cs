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
    public class MatchService : IMatchService
    {
        private readonly CCTContext _context;

        public MatchService(CCTContext context)
        {

            _context = context;
        }
        public MatchViewModel GetMatchByID(int matchID)
        {
            var match = _context.Match
                        .Where(m => m.ID == matchID)
                        .Select(_m => new MatchViewModel
                        {
                            ID = matchID,
                            Time = _m.Time,
                            Surface = _context.Surface
                                        .FirstOrDefault(s => s.ID == _m.SurfaceID)
                                        .Description,
                            Status = _context.MatchStatus
                                        .FirstOrDefault(ms => ms.ID == _m.StatusID)
                                        .Description,

                        }).FirstOrDefault();
            return match; 
        }
        public async Task PostMatch(CreateMatchViewModel match)
        {
            
        }
        public async Task UpdateTeamStats(MatchTeamViewModel match)
        {

        }
        public async Task AddParticipant(int matchID, string ADUserName)
        {

        }

    }

}


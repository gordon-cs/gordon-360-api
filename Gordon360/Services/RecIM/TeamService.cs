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
    public class TeamService : ITeamService
    {
        private readonly CCTContext _context;

        public TeamService(CCTContext context)
        {
            _context = context;
        }

        public Task AddUserToTeam(int teamID, int participantID)
        {
            throw new NotImplementedException();
        }

        public TeamViewModel? GetTeamByID(int teamID)
        {
            var matchTeam = _context.MatchTeam
                        .FirstOrDefault(mt => mt.TeamID == teamID);
            var match = _context.Match
                        .FirstOrDefault(m => m.ID == matchTeam.MatchID);
            var opponentMatchTeam = _context.MatchTeam
                        .FirstOrDefault(opmt => opmt.MatchID == match.ID && opmt.TeamID != teamID);
            var opponentTeam = _context.Team
                        .FirstOrDefault(opt => opt.ID == opponentMatchTeam.TeamID);

            var team = _context.Team
                .Where(t => t.ID == teamID)
                .Select(t => new TeamViewModel
                {
                    ID = teamID,
                    Name = t.Name,
                    Status = _context.TeamStatus
                            .FirstOrDefault(s => s.ID == t.StatusID)
                            .Description,
                    Private = t.Private,
                    Logo = t.Logo,
                    Match = t.MatchTeam
                        .Select(mt => new MatchViewModel
                        {
                            ID = mt.MatchID,
                            Time = match.Time,
                            Surface = _context.Surface
                                .FirstOrDefault(s => s.ID == match.SurfaceID)
                                .Description,
                            Status = _context.MatchStatus
                                .FirstOrDefault(s => s.ID == match.StatusID)
                                .Description,
                            SeriesID = match.SeriesID,
                        }),
                }).FirstOrDefault();
            return team;
        }

        public Task PostTeam(Team team)
        {
            throw new NotImplementedException();
        }

        public Task UpdateParticipantRole(int teamID, int participantID, RoleType participantRole)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTeam(Team updatedTeam)
        {
            throw new NotImplementedException();
        }
    }
}

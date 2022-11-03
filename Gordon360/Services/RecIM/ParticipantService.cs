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
using System.Text.RegularExpressions;

namespace Gordon360.Services.RecIM
{
    public class ParticipantService : IParticipantService
    {
        private readonly CCTContext _context;

        public ParticipantService(CCTContext context)
        {

            _context = context;
        }

        public ParticipantViewModel GetParticipant(int participantID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ParticipantViewModel> getParticipants()
        {
            throw new NotImplementedException();
        }

        public Task PostParticipant(int participantID)
        {
            throw new NotImplementedException();
        }
    }

}


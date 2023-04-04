using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Models.CCT.Context;
using Microsoft.Extensions.Configuration;


namespace Gordon360.Services.RecIM
{
    public class RecIMService : IRecIMService
    {
        private readonly CCTContext _context;
        private readonly IMatchService _matchService;
        private readonly IParticipantService _participantService;
        private readonly IAccountService _accountService;
        private readonly IConfiguration _config;

        public RecIMService(CCTContext context,IConfiguration config, IParticipantService participantSerivce, IMatchService matchService, IAccountService accountServices)
        {
            _context = context;
            _config = config;
            _matchService = matchService;
            _accountService = accountServices;
            _participantService = participantSerivce;
        }

       
    }
}


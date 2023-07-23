using Gordon360.Models.CCT.Context;

namespace Gordon360.Services.RecIM
{
    public class AffiliationService : IAffiliationService
    {

        private readonly CCTContext _context;
        private readonly IActivityService _activityService;
        private readonly ITeamService _teamService;


        public AffiliationService(CCTContext context, IActivityService activityService, ITeamService teamService)
        {
            _context = context;
            _activityService = activityService;
            _teamService = teamService; 
        }
    }
}
